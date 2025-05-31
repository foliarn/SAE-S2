using BiblioSysteme;

namespace Services
{
    /// <summary>
    /// Classe pour comparer les nœuds dans la file de priorité (évite O(n²))
    /// </summary>
    public class ComparateurNoeud : IComparer<Noeud>
    {
        public int Compare(Noeud x, Noeud y)
        {
            if (x == null || y == null) return 0;

            int comparison = x.CoutMinimal.CompareTo(y.CoutMinimal);
            if (comparison == 0)
            {
                // Si même distance, comparer par ID pour éviter les doublons
                comparison = x.ArretNoeud.IdArret.CompareTo(y.ArretNoeud.IdArret);
            }
            return comparison;
        }
    }

    /// <summary>
    /// Responsable de la construction du graphe de transport
    /// </summary>
    public static class ConstructeurGraphe
    {
        /// <summary>
        /// Construit le graphe à partir des lignes de transport
        /// </summary>
        /// <param name="graphe">Le graphe à remplir</param>
        /// <param name="lignes">Liste des lignes de transport</param>
        public static void ConstruireGraphe(Graphe graphe, List<Ligne> lignes)
        {
            try
            {
                if (graphe == null || lignes == null)
                {
                    throw new ArgumentNullException("Le graphe et les lignes ne peuvent pas être null");
                }

                // 1. Créer tous les nœuds (arrêts)
                CreerNoeuds(graphe, lignes);

                // 2. Créer les arêtes pour chaque ligne
                foreach (var ligne in lignes)
                {
                    CreerAretesLigne(graphe, ligne);
                }

                // 3. Créer les correspondances (connexions entre lignes au même arrêt)
                CreerCorrespondances(graphe);

                System.Diagnostics.Debug.WriteLine($"Graphe construit : {graphe.Noeuds.Count} nœuds, {graphe.Aretes.Count} arêtes");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur construction graphe : {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Crée tous les nœuds du graphe à partir des arrêts des lignes
        /// </summary>
        private static void CreerNoeuds(Graphe graphe, List<Ligne> lignes)
        {
            foreach (var ligne in lignes)
            {
                foreach (var arret in ligne.Arrets)
                {
                    if (!graphe.Noeuds.ContainsKey(arret.Arret.IdArret))
                    {
                        var noeud = new Noeud(arret.Arret.IdArret, arret.Arret.NomArret);
                        graphe.Noeuds[arret.Arret.IdArret] = noeud;
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"Créé {graphe.Noeuds.Count} nœuds");
        }

        /// <summary>
        /// Crée les arêtes pour une ligne donnée (connexions entre arrêts consécutifs)
        /// </summary>
        private static void CreerAretesLigne(Graphe graphe, Ligne ligne)
        {
            if (ligne.Arrets.Count < 2)
            {
                System.Diagnostics.Debug.WriteLine($"Ligne {ligne.NomLigne} : moins de 2 arrêts, ignorée");
                return;
            }

            int aretesCreees = 0;

            // Créer les arêtes dans les deux sens (aller et retour)
            for (int i = 0; i < ligne.Arrets.Count - 1; i++)
            {
                var arretDepart = ligne.Arrets[i];
                var arretArrivee = ligne.Arrets[i + 1];

                // Vérifier que les nœuds existent
                if (!graphe.Noeuds.TryGetValue(arretDepart.Arret.IdArret, out var noeudDepart) ||
                    !graphe.Noeuds.TryGetValue(arretArrivee.Arret.IdArret, out var noeudArrivee))
                {
                    System.Diagnostics.Debug.WriteLine($"Erreur : Nœud manquant pour ligne {ligne.NomLigne}");
                    continue;
                }

                // Temps de trajet entre ces deux arrêts
                int tempsTrajet = arretArrivee.TempsDepart - arretDepart.TempsDepart;

                // Arête aller
                var areteAller = new Arete(noeudDepart, noeudArrivee, ligne, tempsTrajet);
                noeudDepart.AretesSortantes.Add(areteAller);
                graphe.Aretes.Add(areteAller);
                aretesCreees++;

                // Arête retour
                var areteRetour = new Arete(noeudArrivee, noeudDepart, ligne, tempsTrajet);
                noeudArrivee.AretesSortantes.Add(areteRetour);
                graphe.Aretes.Add(areteRetour);
                aretesCreees++;
            }

            System.Diagnostics.Debug.WriteLine($"Ligne {ligne.NomLigne} : {aretesCreees} arêtes créées");
        }

        /// <summary>
        /// Crée les correspondances (changements de ligne au même arrêt)
        /// </summary>
        private static void CreerCorrespondances(Graphe graphe)
        {
            const int TEMPS_CORRESPONDANCE = 5; // 5 minutes pour changer de ligne

            // Pour chaque arrêt, regarder quelles lignes y passent
            var arretsParLigne = new Dictionary<int, List<Ligne>>();

            // Construire la map arrêt -> lignes
            foreach (var arete in graphe.Aretes.Where(a => !a.EstCorrespondance))
            {
                int idArret = arete.NoeudDepart.ArretNoeud.IdArret;
                if (!arretsParLigne.ContainsKey(idArret))
                {
                    arretsParLigne[idArret] = new List<Ligne>();
                }

                if (!arretsParLigne[idArret].Contains(arete.LigneUtilisee))
                {
                    arretsParLigne[idArret].Add(arete.LigneUtilisee);
                }
            }

            int correspondancesCreees = 0;

            // Créer les correspondances
            foreach (var kvp in arretsParLigne)
            {
                var idArret = kvp.Key;
                var lignes = kvp.Value;

                if (lignes.Count > 1) // Si plusieurs lignes passent par cet arrêt
                {
                    var noeud = graphe.Noeuds[idArret];

                    // Créer des correspondances entre toutes les paires de lignes
                    for (int i = 0; i < lignes.Count; i++)
                    {
                        for (int j = 0; j < lignes.Count; j++)
                        {
                            if (i != j) // Pas de correspondance vers la même ligne
                            {
                                var areteCorrespondance = new Arete(noeud, noeud, lignes[j], TEMPS_CORRESPONDANCE, true);
                                noeud.AretesSortantes.Add(areteCorrespondance);
                                graphe.Aretes.Add(areteCorrespondance);
                                correspondancesCreees++;
                            }
                        }
                    }

                    System.Diagnostics.Debug.WriteLine($"Arrêt {noeud.ArretNoeud.NomArret} : {lignes.Count} lignes, {lignes.Count * (lignes.Count - 1)} correspondances");
                }
            }

            System.Diagnostics.Debug.WriteLine($"Total correspondances créées : {correspondancesCreees}");
        }

        /// <summary>
        /// Valide la cohérence du graphe construit
        /// </summary>
        public static bool ValiderGraphe(Graphe graphe)
        {
            try
            {
                if (graphe == null)
                {
                    System.Diagnostics.Debug.WriteLine("Erreur : Graphe null");
                    return false;
                }

                int noeudsIsoles = 0;

                // Vérifier que tous les nœuds ont au moins une arête
                foreach (var noeud in graphe.Noeuds.Values)
                {
                    if (noeud.AretesSortantes.Count == 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"Attention : Nœud isolé {noeud.ArretNoeud.NomArret}");
                        noeudsIsoles++;
                    }
                }

                // Vérifier que toutes les arêtes pointent vers des nœuds existants
                foreach (var arete in graphe.Aretes)
                {
                    if (!graphe.Noeuds.ContainsKey(arete.NoeudArrivee.ArretNoeud.IdArret))
                    {
                        System.Diagnostics.Debug.WriteLine($"Erreur : Arête vers nœud inexistant");
                        return false;
                    }
                }

                // Le graphe est valide même avec quelques nœuds isolés
                if (noeudsIsoles > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"Avertissement : {noeudsIsoles} nœuds isolés trouvés");
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur validation graphe : {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Remet à zéro tous les nœuds pour une nouvelle recherche Dijkstra
        /// </summary>
        public static void ReinitialiserPourDijkstra(Graphe graphe)
        {
            if (graphe == null) return;

            foreach (var noeud in graphe.Noeuds.Values)
            {
                noeud.ReinitialiserDijkstra();
            }
        }
    }
}
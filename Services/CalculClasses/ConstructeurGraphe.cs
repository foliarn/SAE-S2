using BiblioSysteme;

namespace Services.CalculClasses
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

            // Créer les arêtes entre TOUS les arrêts dans les deux sens
            for (int i = 0; i < ligne.Arrets.Count; i++)
            {
                for (int j = 0; j < ligne.Arrets.Count; j++)
                {
                    if (i == j) continue; // Pas d'arête vers soi-même

                    var arretDepart = ligne.Arrets[i];
                    var arretArrivee = ligne.Arrets[j];

                    if (!graphe.Noeuds.TryGetValue(arretDepart.Arret.IdArret, out var noeudDepart) ||
                        !graphe.Noeuds.TryGetValue(arretArrivee.Arret.IdArret, out var noeudArrivee))
                        continue;

                    // Temps = différence absolue
                    int tempsTrajet = Math.Abs(arretArrivee.TempsDepart - arretDepart.TempsDepart);

                    var arete = new Arete(noeudDepart, noeudArrivee, ligne, tempsTrajet);
                    noeudDepart.AretesSortantes.Add(arete);
                    graphe.Aretes.Add(arete);
                    aretesCreees++;
                }
            }

            //System.Diagnostics.Debug.WriteLine($"Ligne {ligne.NomLigne} : {aretesCreees} arêtes créées");
        }

        /// <summary>
        /// Crée les correspondances (changements de ligne au même arrêt)
        /// </summary>
        private static void CreerCorrespondances(Graphe graphe)
        {
            const int TEMPS_CORRESPONDANCE = 0; // 5 minutes pour changer de ligne

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
                                //System.Diagnostics.Debug.WriteLine($"Correspondance : {noeud.ArretNoeud.NomArret} ligne {lignes[i].NomLigne} → ligne {lignes[j].NomLigne}");
                                correspondancesCreees++;
                            }
                        }
                    }

                    //System.Diagnostics.Debug.WriteLine($"Arrêt {noeud.ArretNoeud.NomArret} : {lignes.Count} lignes, {lignes.Count * (lignes.Count - 1)} correspondances");
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

        public static void DebugGrapheBidirectionnel(Graphe graphe, int idArretA, int idArretB)
        {
            if (!graphe.Noeuds.ContainsKey(idArretA) || !graphe.Noeuds.ContainsKey(idArretB))
            {
                System.Diagnostics.Debug.WriteLine("Arrêts non trouvés dans le graphe");
                return;
            }

            var noeudA = graphe.Noeuds[idArretA];
            var noeudB = graphe.Noeuds[idArretB];

            System.Diagnostics.Debug.WriteLine($"=== DEBUG GRAPHE BIDIRECTIONNEL ===");
            System.Diagnostics.Debug.WriteLine($"Arrêt A: {noeudA.ArretNoeud.NomArret} (ID: {idArretA})");
            System.Diagnostics.Debug.WriteLine($"Arrêt B: {noeudB.ArretNoeud.NomArret} (ID: {idArretB})");

            // Vérifier arêtes sortantes de A vers B
            var aretesAversB = noeudA.AretesSortantes.Where(a => a.NoeudArrivee.ArretNoeud.IdArret == idArretB).ToList();
            System.Diagnostics.Debug.WriteLine($"\nArêtes A → B: {aretesAversB.Count}");
            foreach (var arete in aretesAversB)
            {
                System.Diagnostics.Debug.WriteLine($"  Ligne: {arete.LigneUtilisee?.NomLigne}, Poids: {arete.Poids}, Correspondance: {arete.EstCorrespondance}");
            }

            // Vérifier arêtes sortantes de B vers A
            var aretesBversA = noeudB.AretesSortantes.Where(a => a.NoeudArrivee.ArretNoeud.IdArret == idArretA).ToList();
            System.Diagnostics.Debug.WriteLine($"\nArêtes B → A: {aretesBversA.Count}");
            foreach (var arete in aretesBversA)
            {
                System.Diagnostics.Debug.WriteLine($"  Ligne: {arete.LigneUtilisee?.NomLigne}, Poids: {arete.Poids}, Correspondance: {arete.EstCorrespondance}");
            }

            // Vérifier symétrie
            System.Diagnostics.Debug.WriteLine($"\n=== ANALYSE SYMÉTRIE ===");
            System.Diagnostics.Debug.WriteLine($"Nombre arêtes A→B: {aretesAversB.Count}");
            System.Diagnostics.Debug.WriteLine($"Nombre arêtes B→A: {aretesBversA.Count}");

            if (aretesAversB.Count != aretesBversA.Count)
            {
                System.Diagnostics.Debug.WriteLine("⚠️ ASYMÉTRIE DÉTECTÉE dans le nombre d'arêtes!");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("✅ Nombre d'arêtes symétrique");
            }

            
            System.Diagnostics.Debug.WriteLine($"\n=== CORRESPONDANCES ===");
            System.Diagnostics.Debug.WriteLine($"Correspondances depuis A: {noeudA.AretesSortantes.Count(a => a.EstCorrespondance)}");
            System.Diagnostics.Debug.WriteLine($"Correspondances depuis B: {noeudB.AretesSortantes.Count(a => a.EstCorrespondance)}");

            System.Diagnostics.Debug.WriteLine($"Arêtes normales depuis A: {noeudA.AretesSortantes.Count(a => !a.EstCorrespondance)}");
            System.Diagnostics.Debug.WriteLine($"Arêtes normales depuis B: {noeudB.AretesSortantes.Count(a => !a.EstCorrespondance)}");

            var correspondances = noeudA.AretesSortantes.Where(a => a.EstCorrespondance).ToList();
            System.Diagnostics.Debug.WriteLine($"Correspondances réelles dans AretesSortantes A: {correspondances.Count}");
            foreach (var c in correspondances)
                System.Diagnostics.Debug.WriteLine($"  - Vers ligne {c.LigneUtilisee.NomLigne}");

        }

    }
}
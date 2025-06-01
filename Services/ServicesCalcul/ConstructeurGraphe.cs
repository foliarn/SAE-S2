using BiblioSysteme;
using Services.ServicesClasses;

namespace Services.ServicesCalcul
{
    /// <summary>
    /// Constructeur de graphe
    /// </summary>
    public static class ConstructeurGraphe
    {
        /// <summary>
        /// Construit le graphe bidirectionnel pour la recherche d'itinéraires
        /// </summary>
        /// <param name="graphe">Le graphe à construire</param>
        /// <param name="lignes">Liste des lignes de transport</param>
        public static void ConstruireGraphe(Graphe graphe, List<Ligne> lignes)
        {
            if (graphe == null || lignes == null)
                throw new ArgumentNullException("Graphe et lignes ne peuvent pas être null");

            System.Diagnostics.Debug.WriteLine("=== CONSTRUCTION DU GRAPHE ===");

            // Créer tous les nœuds
            CreerTousLesNoeuds(graphe, lignes);

            // Créer les arêtes bidirectionnelles pour chaque ligne
            foreach (var ligne in lignes.Where(LigneService.EstLigneValide))
            {
                CreerAretesPourLigne(graphe, ligne);
            }

            // Créer les correspondances
            CreerCorrespondances(graphe, lignes);

            System.Diagnostics.Debug.WriteLine($"Graphe terminé : {graphe.Noeuds.Count} nœuds, {graphe.Aretes.Count} arêtes");
        }

        /// <summary>
        /// Crée tous les nœuds du graphe (un par arrêt unique)
        /// </summary>
        private static void CreerTousLesNoeuds(Graphe graphe, List<Ligne> lignes)
        {
            var arretsUniques = new HashSet<int>();

            // Parcourir toutes les lignes pour collecter tous les arrêts
            foreach (var ligne in lignes)
            {
                if (ligne.Arrets != null)
                {
                    foreach (var arretLigne in ligne.Arrets)
                    {
                        arretsUniques.Add(arretLigne.Arret.IdArret);
                    }
                }
            }

            // Créer un nœud pour chaque arrêt unique
            foreach (var idArret in arretsUniques)
            {
                // Trouver les détails de l'arrêt
                var arret = ArretService.TrouverArretParId(idArret, lignes);
                if (arret != null)
                {
                    var noeud = new Noeud(arret.IdArret, arret.NomArret);
                    graphe.Noeuds[idArret] = noeud;
                }
            }

            System.Diagnostics.Debug.WriteLine($"Créé {graphe.Noeuds.Count} nœuds");
        }

        /// <summary>
        /// Crée les arêtes bidirectionnelles pour une ligne donnée
        /// </summary>
        private static void CreerAretesPourLigne(Graphe graphe, Ligne ligne)
        {
            if (ligne.Arrets.Count < 2) return;

            int aretesCreees = 0;

            // Créer des arêtes entre TOUS les arrêts dans les DEUX sens 
            for (int i = 0; i < ligne.Arrets.Count; i++)
            {
                for (int j = 0; j < ligne.Arrets.Count; j++)
                {
                    if (i == j) continue; // Pas d'arête vers soi-même

                    var arretDepart = ligne.Arrets[i].Arret;
                    var arretArrivee = ligne.Arrets[j].Arret;

                    // Vérifier que les nœuds existent
                    if (!graphe.Noeuds.ContainsKey(arretDepart.IdArret) ||
                        !graphe.Noeuds.ContainsKey(arretArrivee.IdArret))
                        continue;

                    var noeudDepart = graphe.Noeuds[arretDepart.IdArret];
                    var noeudArrivee = graphe.Noeuds[arretArrivee.IdArret];

                    // Déterminer le sens et calculer le temps de trajet
                    bool sensNormal = LigneService.EstSensNormal(ligne, arretDepart, arretArrivee);
                    int tempsTrajet = ArretService.CalculerTempsTrajet(ligne, arretDepart, arretArrivee, sensNormal);

                    // Créer l'arête
                    var arete = new Arete(noeudDepart, noeudArrivee, ligne, tempsTrajet, false);

                    // Ajouter l'arête au nœud de départ et au graphe
                    noeudDepart.AretesSortantes.Add(arete);
                    graphe.Aretes.Add(arete);
                    aretesCreees++;
                }
            }

            System.Diagnostics.Debug.WriteLine($"Ligne {ligne.NomLigne} : {aretesCreees} arêtes créées");
        }

        /// <summary>
        /// Crée les correspondances (changements de ligne au même arrêt)
        /// </summary>
        private static void CreerCorrespondances(Graphe graphe, List<Ligne> lignes)
        {
            int correspondancesCreees = 0;

            // Pour chaque arrêt, trouver toutes les lignes qui y passent
            foreach (var noeud in graphe.Noeuds.Values)
            {
                var lignesAuNoeud = LigneService.ObtenirLignesParArret(noeud.ArretNoeud, lignes);

                if (lignesAuNoeud.Count > 1)
                {
                    // Créer des correspondances entre toutes les paires de lignes
                    for (int i = 0; i < lignesAuNoeud.Count; i++)
                    {
                        for (int j = 0; j < lignesAuNoeud.Count; j++)
                        {
                            if (i != j) // Pas de correspondance vers la même ligne
                            {
                                // Correspondance = arête de coût 0 vers la même station mais ligne différente
                                var areteCorrespondance = new Arete(noeud, noeud, lignesAuNoeud[j], 0, true);
                                noeud.AretesSortantes.Add(areteCorrespondance);
                                graphe.Aretes.Add(areteCorrespondance);
                                correspondancesCreees++;
                            }
                        }
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"Correspondances créées : {correspondancesCreees}");
        }

        /// <summary>
        /// Remet à zéro tous les nœuds pour une nouvelle recherche Dijkstra
        /// </summary>
        public static void ReinitialiserPourDijkstra(Graphe graphe)
        {
            if (graphe?.Noeuds == null) return;

            foreach (var noeud in graphe.Noeuds.Values)
            {
                noeud.ReinitialiserDijkstra();
            }

            System.Diagnostics.Debug.WriteLine("Graphe réinitialisé pour Dijkstra");
        }
    }
}
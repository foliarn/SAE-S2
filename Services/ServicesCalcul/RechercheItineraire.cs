using BiblioSysteme;
using Services.ServicesClasses;
using System.Reflection.Metadata;

namespace Services.ServicesCalcul
{
    /// <summary>
    /// Comparateur pour trier les nœuds dans la file de priorité de Dijkstra
    /// </summary>
    public class ComparateurNoeud : IComparer<Noeud>
    {
        public int Compare(Noeud x, Noeud y)
        {
            if (x == null || y == null) 
                return 0;

            // Comparer d'abord par coût minimal
            int comparison = x.CoutMinimal.CompareTo(y.CoutMinimal);
            
            if (comparison == 0)
            {
                // Si même coût, comparer par ID pour éviter les doublons dans le SortedSet
                comparison = x.ArretNoeud.IdArret.CompareTo(y.ArretNoeud.IdArret);
            }
            
            return comparison;
        }
    }

    /// <summary>
    /// Recherche d'itinéraires avec Dijkstra
    /// </summary>
    public static class RechercheItineraire
    {
        /// <summary>
        /// Trouve un itinéraire entre deux arrêts
        /// </summary>
        /// <param name="arretDepart">Arrêt de départ</param>
        /// <param name="arretDestination">Arrêt de destination</param>
        /// <param name="heureDepart">Heure souhaitée de départ</param>
        /// <returns>Itinéraire trouvé ou null si aucun chemin</returns>
        public static Itineraire TrouverItineraire(Arret arretDepart, Arret arretDestination, ParametresRecherche parametres)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== RECHERCHE ITINÉRAIRE ===");
                System.Diagnostics.Debug.WriteLine($"De : {arretDepart.NomArret} vers {arretDestination.NomArret}");
                System.Diagnostics.Debug.WriteLine($"Départ souhaité : {parametres.HeureDepart}");

                // Validation des paramètres
                if (arretDepart == null || arretDestination == null)
                    throw new ArgumentNullException("Les arrêts ne peuvent pas être null");

                if (arretDepart.IdArret == arretDestination.IdArret)
                    throw new ArgumentException("Arrêt de départ identique à l'arrivée");

                // Construire le graphe
                var graphe = new Graphe();
                ConstructeurGraphe.ConstruireGraphe(graphe, Init.toutesLesLignes);

                // Vérifier que les arrêts existent dans le graphe
                if (!graphe.Noeuds.ContainsKey(arretDepart.IdArret) ||
                    !graphe.Noeuds.ContainsKey(arretDestination.IdArret))
                {
                    System.Diagnostics.Debug.WriteLine("Un des arrêts n'existe pas dans le réseau");
                    return null;
                }

                // Exécuter Dijkstra
                var noeudDestination = ExecuterDijkstra(graphe, arretDepart.IdArret, arretDestination.IdArret, parametres);

                if (noeudDestination == null)
                {
                    System.Diagnostics.Debug.WriteLine("Aucun itinéraire trouvé");
                    return null;
                }

                // Reconstruire l'itinéraire
                var itineraire = ReconstruireItineraire(noeudDestination, arretDepart, arretDestination);

                System.Diagnostics.Debug.WriteLine($"Itinéraire trouvé : {itineraire.Etapes.Count} étapes, durée {itineraire.TempsTotal}");
                return itineraire;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur recherche : {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Exécute l'algorithme de Dijkstra adapté aux transports en commun
        /// </summary>
        private static Noeud ExecuterDijkstra(Graphe graphe, int idDepart, int idDestination, ParametresRecherche parametres)
        {
            // Réinitialiser le graphe
            ConstructeurGraphe.ReinitialiserPourDijkstra(graphe);

            // Initialiser le nœud de départ
            var noeudDepart = graphe.Noeuds[idDepart];
            noeudDepart.CoutMinimal = 0;
            noeudDepart.HeureArrivee = parametres.HeureDepart;

            // File de priorité (nœuds à traiter par coût croissant)
            var filePriorite = new SortedSet<Noeud>(new ComparateurNoeud());
            filePriorite.Add(noeudDepart);

            var noeudDestination = graphe.Noeuds[idDestination];
            var limiteTemps = parametres.HeureDepart.Add(parametres.TempsMaxRecherche);

            int iterations = 0;

            while (filePriorite.Count > 0)
            {
                iterations++;
                if (iterations % 1000 == 0)
                    System.Diagnostics.Debug.WriteLine($"Dijkstra : {iterations} itérations, file : {filePriorite.Count}");

                // Prendre le nœud avec le coût minimal
                var noeudCourant = filePriorite.Min;
                filePriorite.Remove(noeudCourant);

                // Si déjà visité, passer au suivant
                if (noeudCourant.Visite)
                    continue;

                // Marquer comme visité
                noeudCourant.Visite = true;

                // Si on atteint la destination, c'est fini !
                if (noeudCourant.ArretNoeud.IdArret == idDestination)
                {
                    System.Diagnostics.Debug.WriteLine($"Destination atteinte en {iterations} itérations");
                    return noeudCourant;
                }

                // Si on dépasse la limite de temps, abandonner ce chemin
                if (noeudCourant.HeureArrivee > limiteTemps)
                    continue;

                // Explorer tous les voisins
                foreach (var arete in noeudCourant.AretesSortantes)
                {
                    var noeudVoisin = arete.NoeudArrivee;

                    // Si déjà visité, ignorer
                    if (noeudVoisin.Visite)
                        continue;

                    // Calculer le nouveau coût et la nouvelle heure
                    TimeSpan prochainDepart, heureArrivee;
                    double cout;

                    if (arete.EstCorrespondance)
                    {
                        var sensNormal = true;
                        prochainDepart = ArretService.TrouverProchainHoraire(
                            arete.LigneUtilisee,
                            noeudCourant.ArretNoeud,
                            noeudCourant.HeureArrivee,
                            sensNormal);

                        cout = CalculCout.CalculerCoutCorrespondance(parametres, noeudCourant, prochainDepart);
                        heureArrivee = prochainDepart;
                    }
                    else
                    {
                        bool sensNormal = LigneService.EstSensNormal(arete.LigneUtilisee,
                            noeudCourant.ArretNoeud, arete.NoeudArrivee.ArretNoeud);

                        prochainDepart = ArretService.TrouverProchainHoraire(
                            arete.LigneUtilisee,
                            noeudCourant.ArretNoeud,
                            noeudCourant.HeureArrivee,
                            sensNormal);

                        heureArrivee = prochainDepart.Add(TimeSpan.FromMinutes(arete.Poids));
                        cout = CalculCout.CalculerCoutTrajet(parametres, noeudCourant, arete, prochainDepart);
                    }

                    // Si aucun service disponible, ignorer
                    if (cout == double.MaxValue)
                        continue;

                    // Si ce chemin est meilleur, mettre à jour
                    if (cout < noeudVoisin.CoutMinimal)
                    {
                        // Retirer de la file pour réinsertion avec nouveau coût
                        filePriorite.Remove(noeudVoisin);

                        // Mettre à jour le nœud
                        noeudVoisin.CoutMinimal = cout;
                        noeudVoisin.HeureArrivee = heureArrivee;
                        noeudVoisin.Precedent = noeudCourant;
                        noeudVoisin.AretePrecedente = arete;

                        // Réinsérer dans la file
                        filePriorite.Add(noeudVoisin);
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"Aucun chemin trouvé après {iterations} itérations");
            return null;
        }

        /// <summary>
        /// Reconstruit l'itinéraire en remontant depuis la destination
        /// </summary>
        private static Itineraire ReconstruireItineraire(Noeud noeudDestination, Arret arretDepart, Arret arretDestination)
        {
            var itineraire = new Itineraire(arretDepart, arretDestination);
            var etapes = new List<EtapeItineraire>();

            // Remonter le chemin depuis la destination
            var noeudCourant = noeudDestination;

            while (noeudCourant.Precedent != null)
            {
                var arete = noeudCourant.AretePrecedente;
                var noeudPrecedent = noeudCourant.Precedent;

                // Créer l'étape
                var etape = new EtapeItineraire(
                    new Arret(noeudPrecedent.ArretNoeud.IdArret, noeudPrecedent.ArretNoeud.NomArret),
                    new Arret(noeudCourant.ArretNoeud.IdArret, noeudCourant.ArretNoeud.NomArret),
                    arete.LigneUtilisee,
                    noeudPrecedent.HeureArrivee,
                    noeudCourant.HeureArrivee
                );

                etape.EstCorrespondance = arete.EstCorrespondance;
                etapes.Add(etape);

                noeudCourant = noeudPrecedent;
            }

            // Inverser la liste (construite à l'envers)
            etapes.Reverse();

            // Ajouter les étapes à l'itinéraire
            for (int i = 0; i < etapes.Count; i++)
            {
                etapes[i].NumeroEtape = i + 1;
                itineraire.Etapes.Add(etapes[i]);
            }

            // Calculer les horaires globaux
            if (etapes.Count > 0)
            {
                itineraire.HeureDepart = etapes.First().HeureDepart;
                itineraire.HeureArrivee = etapes.Last().HeureArrivee;
            }

            return itineraire;
        }
    }
}
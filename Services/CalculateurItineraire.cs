using BiblioBDD;
using BiblioSysteme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CalculateurItineraire
    {
        /// <summary>
        /// Calcule les itinéraires entre deux arrêts
        /// </summary>
        /// <param name="arretDepart">Arrêt de départ</param>
        /// <param name="arretDestination">Arrêt de destination</param>
        /// <param name="parametres">Paramètres de recherche utilisateur</param>
        /// <returns>Liste des itinéraires trouvés</returns>
        public static List<Itineraire> CalculerItineraires(Arret arretDepart, Arret arretDestination, ParametresRecherche parametres)
        {
            try
            {
                // Validation des paramètres
                if (arretDepart == null || arretDestination == null || parametres == null)
                {
                    System.Diagnostics.Debug.WriteLine("Erreur : Paramètres invalides");
                    return new List<Itineraire>();
                }

                if (arretDepart.IdArret == arretDestination.IdArret)
                {
                    System.Diagnostics.Debug.WriteLine("Erreur : Arrêt de départ identique à l'arrêt de destination");
                    return new List<Itineraire>();
                }

                // Construire le graphe
                var graphe = new Graphe();

                // 🔥🔥🔥 VERIFIER QUE TOUTESLESLIGNES EST ACTUALISÉ AVANT DE CONTINUER 🔥🔥🔥
                // 🔥🔥🔥 VERIFIER QUE TOUTESLESLIGNES EST ACTUALISÉ AVANT DE CONTINUER 🔥🔥🔥
                // 🔥🔥🔥 VERIFIER QUE TOUTESLESLIGNES EST ACTUALISÉ AVANT DE CONTINUER 🔥🔥🔥

                if (RecupDonnees.toutesLesLignes == null || RecupDonnees.toutesLesLignes.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("Erreur : Aucune ligne disponible");
                    return new List<Itineraire>();
                }

                ConstructeurGraphe.ConstruireGraphe(graphe, RecupDonnees.toutesLesLignes);

                ConstructeurGraphe.DebugGrapheBidirectionnel(graphe, arretDepart.IdArret, arretDestination.IdArret);

                // Vérifier que les arrêts existent dans le graphe
                if (!graphe.Noeuds.ContainsKey(arretDepart.IdArret) || !graphe.Noeuds.ContainsKey(arretDestination.IdArret))
                {
                    System.Diagnostics.Debug.WriteLine("Erreur : Arrêt non trouvé dans le graphe");
                    return new List<Itineraire>();
                }

                // Exécuter Dijkstra
                var noeudDestination = ExecuterDijkstra(graphe, arretDepart.IdArret, arretDestination.IdArret, parametres);

                if (noeudDestination == null || noeudDestination.CoutMinimal == double.MaxValue)
                {
                    System.Diagnostics.Debug.WriteLine("Aucun itinéraire trouvé");
                    return new List<Itineraire>();
                }

                // Reconstruire l'itinéraire
                var itineraire = ReconstruireItineraire(noeudDestination, arretDepart, arretDestination);

                return new List<Itineraire> { itineraire };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur calcul itinéraires : {ex.Message}");
                return new List<Itineraire>();
            }
        }

        /// <summary>
        /// Exécute l'algorithme de Dijkstra adapté aux transports en commun
        /// </summary>
        /// <param name="graphe">Le graphe de transport</param>
        /// <param name="idArretDepart">ID de l'arrêt de départ</param>
        /// <param name="idArretDestination">ID de l'arrêt de destination</param>
        /// <param name="parametres">Paramètres de recherche</param>
        /// <returns>Le nœud de destination avec le chemin optimal, ou null si aucun chemin</returns>
        private static Noeud ExecuterDijkstra(Graphe graphe, int idArretDepart, int idArretDestination, ParametresRecherche parametres)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== DIJKSTRA DEBUG ===");
                System.Diagnostics.Debug.WriteLine($"Départ: {idArretDepart}, Destination: {idArretDestination}");
                System.Diagnostics.Debug.WriteLine($"Heure: {parametres.HeureSouhaitee}");


                // Initialiser le graphe
                CalculItineraireServices.InitialiserGraphe(graphe, idArretDepart, parametres);

                // File de priorité pour traiter les nœuds par coût croissant
                var filePriorite = new SortedSet<Noeud>(new ComparateurNoeud());
                filePriorite.Add(graphe.Noeuds[idArretDepart]);

                var noeudDestination = graphe.Noeuds[idArretDestination];
                var tempsMaxRecherche = parametres.HeureSouhaitee.Add(parametres.TempsMaxRecherche);

                while (filePriorite.Count > 0)
                {
                    // Extraire le nœud avec le coût minimal
                    var noeudCourant = filePriorite.Min;
                    filePriorite.Remove(noeudCourant);

                    // Si déjà visité, passer au suivant
                    if (noeudCourant.Visite)
                        continue;

                    // Marquer comme visité
                    noeudCourant.Visite = true;

                    // Si on a atteint la destination, on a trouvé le chemin optimal
                    if (noeudCourant.ArretNoeud.IdArret == idArretDestination)
                    {
                        System.Diagnostics.Debug.WriteLine($"CHEMIN TROUVÉ - Coût: {noeudCourant.CoutMinimal:F1}");
                        // Reconstruire le chemin pour debug
                        var debug = noeudCourant;
                        while (debug.Precedent != null)
                        {
                            System.Diagnostics.Debug.WriteLine($"  {debug.Precedent.ArretNoeud.NomArret} → {debug.ArretNoeud.NomArret} (Ligne: {debug.AretePrecedente?.LigneUtilisee?.NomLigne})");
                            debug = debug.Precedent;
                        }
                        return noeudCourant;
                    }

                    // Si on dépasse le temps max de recherche, abandonner ce chemin
                    if (noeudCourant.HeureArrivee > tempsMaxRecherche)
                    {
                        System.Diagnostics.Debug.WriteLine($"Temps max dépassé à {noeudCourant.ArretNoeud.NomArret}");
                        continue;
                    }

                    // Examiner toutes les arêtes sortantes
                    foreach (var arete in noeudCourant.AretesSortantes)
                    {
                        var noeudVoisin = arete.NoeudArrivee;

                        // Si déjà visité, ignorer
                        if (noeudVoisin.Visite)
                            continue;

                        // Calculer le nouveau coût
                        double nouveauCout;
                        TimeSpan nouvelleHeure;

                        //Si c'est une correspondance :
                        if (arete.EstCorrespondance)
                        {
                            System.Diagnostics.Debug.WriteLine($"Correspondance détectée : {arete.NoeudDepart.ArretNoeud.NomArret} → ligne {arete.LigneUtilisee.NomLigne}");
                            var prochainDepart = CalculItineraireServices.TrouverProchainDepart(arete.NoeudArrivee.ArretNoeud, arete.LigneUtilisee, noeudCourant.HeureArrivee);

                            if (prochainDepart == TimeSpan.Zero)
                                continue; // Aucun service disponible, ignorer cette correspondance

                            nouveauCout = noeudCourant.CoutMinimal + CalculateurCout.CalculerCoutCorrespondance(arete, noeudCourant.HeureArrivee, prochainDepart, parametres);
                            nouvelleHeure = prochainDepart;
                        }
                        else
                        {
                            // Pour un trajet normal
                            nouveauCout = noeudCourant.CoutMinimal + CalculateurCout.CalculerCout(noeudCourant, arete, noeudCourant.HeureArrivee, parametres);

                            // Calculer l'heure d'arrivée
                            var prochainDepart = CalculItineraireServices.TrouverProchainDepart(noeudCourant.ArretNoeud, arete.LigneUtilisee, noeudCourant.HeureArrivee);
                            if (prochainDepart == TimeSpan.Zero)
                                continue; // Aucun service disponible

                            nouvelleHeure = prochainDepart.Add(TimeSpan.FromMinutes(arete.Poids));
                        }

                        // Si ce chemin est meilleur, mettre à jour
                        if (nouveauCout < noeudVoisin.CoutMinimal)
                        {
                            // Retirer de la file si déjà présent (pour réinsertion avec nouveau coût)
                            filePriorite.Remove(noeudVoisin);

                            // Mettre à jour le nœud
                            noeudVoisin.CoutMinimal = nouveauCout;
                            noeudVoisin.HeureArrivee = nouvelleHeure;
                            noeudVoisin.Precedent = noeudCourant;
                            noeudVoisin.AretePrecedente = arete;

                            // Ajouter à la file de priorité
                            filePriorite.Add(noeudVoisin);
                        }
                        if (arete.NoeudDepart.ArretNoeud.IdArret == arete.NoeudArrivee.ArretNoeud.IdArret)
                        {
                            System.Diagnostics.Debug.WriteLine($"Correspondance trouvée: {arete.NoeudDepart.ArretNoeud.NomArret} vers ligne {arete.LigneUtilisee.NomLigne}");
                        }
                        // Dans ExecuterDijkstra(), dans la boucle foreach (var arete in noeudCourant.AretesSortantes)
                        //System.Diagnostics.Debug.WriteLine($"Arete: {arete.NoeudDepart.ArretNoeud.NomArret}→{arete.NoeudArrivee.ArretNoeud.NomArret}, EstCorrespondance: {arete.EstCorrespondance}");
                    }
                    // Ajoutez ce debug dans la boucle Dijkstra
                    
                }

                System.Diagnostics.Debug.WriteLine("Aucun chemin trouvé vers la destination");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur Dijkstra : {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Reconstruit l'itinéraire à partir du nœud de destination
        /// </summary>
        /// <param name="noeudDestination">Nœud de destination avec le chemin optimal</param>
        /// <param name="arretDepart">Arrêt de départ</param>
        /// <param name="arretDestination">Arrêt de destination</param>
        /// <returns>Itinéraire complet</returns>
        private static Itineraire ReconstruireItineraire(Noeud noeudDestination, Arret arretDepart, Arret arretDestination)
        {
            try
            {
                var itineraire = new Itineraire(arretDepart, arretDestination);
                var etapes = new List<EtapeItineraire>();

                // Parcourir à l'envers pour construire le chemin
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

                // Inverser la liste (on l'a construite à l'envers)
                etapes.Reverse();

                // Ajouter les étapes à l'itinéraire
                for (int i = 0; i < etapes.Count; i++)
                {
                    etapes[i].NumeroEtape = i + 1;
                    ItineraireServices.AjouterEtape(itineraire, etapes[i]);
                }

                // Calculer les métadonnées
                itineraire.TypeItineraire = etapes.Any(e => e.EstCorrespondance) ? "Avec correspondance" : "Direct";

                System.Diagnostics.Debug.WriteLine($"Itinéraire reconstruit : {etapes.Count} étapes, durée {itineraire.TempsTotal}");

                return itineraire;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur reconstruction : {ex.Message}");
                return new Itineraire(arretDepart, arretDestination);
            }
        }
    }
}

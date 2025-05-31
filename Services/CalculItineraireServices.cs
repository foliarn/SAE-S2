using BiblioSysteme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public static class CalculItineraireServices
    {
        /// <summary>
        /// Trouve le prochain départ d'une ligne à un arrêt donné après une heure spécifiée
        /// </summary>
        /// <param name="arret">Arrêt où chercher le départ</param>
        /// <param name="ligne">Ligne de transport</param>
        /// <param name="heureActuelle">Heure actuelle du trajet</param>
        /// <returns>Le prochain horaire de départ ou TimeSpan.Zero si aucun service n'est disponible</returns>
        public static TimeSpan TrouverProchainDepart(Arret arret, Ligne ligne, TimeSpan heureActuelle)
        {
            try
            {
                // Utiliser le service existant pour obtenir tous les horaires à partir de l'heure actuelle
                // On récupère le premier horaire de passage (First)
                var horairesDisponibles = ArretService.GetHorairesPassage(arret, ligne, heureActuelle).First();

                if (horairesDisponibles == TimeSpan.Zero)
                {
                    return TimeSpan.Zero; // Aucun service disponible
                }

                return horairesDisponibles;

                // Retourner le premier horaire disponible
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur recherche prochain départ : {ex.Message}");
                return TimeSpan.Zero;
            }
        }

        /// <summary>
        /// Initialise le graphe pour l'algorithme de Dijkstra
        /// </summary>
        /// <param name="graphe">Le graphe à initialiser</param>
        /// <param name="idArretDepart">ID de l'arrêt de départ</param>
        /// <param name="parametres">Paramètres de recherche</param>
        public static void InitialiserGraphe(Graphe graphe, int idArretDepart, ParametresRecherche parametres)
        {
            // Réinitialiser tous les nœuds
            ConstructeurGraphe.ReinitialiserPourDijkstra(graphe);

            // Configurer le nœud de départ
            var noeudDepart = graphe.Noeuds[idArretDepart];
            noeudDepart.CoutMinimal = 0;
            noeudDepart.HeureArrivee = parametres.HeureSouhaitee;
            noeudDepart.Precedent = null;
            noeudDepart.AretePrecedente = null;

            System.Diagnostics.Debug.WriteLine($"Départ : {noeudDepart.ArretNoeud.NomArret} à {parametres.HeureSouhaitee}");
        }


    }
}

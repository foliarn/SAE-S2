using BiblioSysteme;

namespace Services
{
    
    public class CalculateurCout
    {

        /// <summary>
        /// Calcule le coût pour emprunter une arête (connexion) entre deux nœuds (arrêts)
        /// </summary>
        /// <param name="noeudActuel">Le noeud de départ</param>
        /// <param name="arete">L'arête à emprunter</param>
        /// <param name="heureActuelle">L'heure actuelle du trajet</param>
        /// <param name="parametres">Les paramètres de recherche (contraintes utilisateur)</param>
        /// <returns>Le coût du trajet en minutes</returns>
        public static double CalculerCout(Noeud noeudActuel, Arete arete, TimeSpan heureActuelle, ParametresRecherche parametres)
        {
            var prochainDepart = CalculItineraireServices.TrouverProchainDepart(arete.NoeudArrivee.ArretNoeud, arete.LigneUtilisee, heureActuelle);

            // Vérifier si l'arête est une correspondance (changement de ligne)
            if (arete.LigneUtilisee == null || arete.LigneUtilisee.NomLigne == "Correspondance")
            {
                return CalculerCoutCorrespondance(arete, heureActuelle, prochainDepart, parametres);
            }
            // Sinon, c'est un trajet normal sur une ligne
            return CalculerCoutTrajet(arete, heureActuelle, prochainDepart, parametres);
        }


        /// <summary>
        /// Calcule le coût pour un trajet normal sur une ligne
        /// </summary>
        /// <param name="arete">Arête (connexion) à explorer</param>
        /// <param name="heureActuelle">Heure actuelle du trajet</param>
        /// <param name="prochainDepart">L'heure du prochain départ</param>
        /// <param name="parametres">Paramètres de recherche (contraintes utilisateur)</param>
        /// <returns>Le coût du trajet en minutes</returns>
        private static double CalculerCoutTrajet(Arete arete, TimeSpan heureActuelle, TimeSpan prochainDepart, ParametresRecherche parametres)
        {
            if (prochainDepart == TimeSpan.Zero)
            {
                // Aucun service disponible (trop tard dans la journée)
                return double.MaxValue;
            }

            double tempsAttente = (prochainDepart - heureActuelle).TotalMinutes;

            // Temps de trajet sur la ligne (déjà stocké dans l'arête)
            double tempsTrajet = arete.Poids;

            // Préférences utilisateur
            double coutAttente = tempsAttente * parametres.CoefficientAttente;
            double coutTrajet = tempsTrajet * parametres.CoefficientTempsTransport;

            double coutTotal = coutAttente + coutTrajet;

            //System.Diagnostics.Debug.WriteLine($"Trajet {arete.NoeudDepart.ArretNoeud.NomArret} → {arete.NoeudArrivee.ArretNoeud.NomArret} " +
            //                                  $"(Ligne {arete.LigneUtilisee.NomLigne}): " +
            //                                  $"Attente={tempsAttente:F1}min, Trajet={tempsTrajet}min, Coût={coutTotal:F1}");

            return coutTotal;
        }

        /// <summary>
        /// Calcule le coût pour une correspondance (changement de ligne)
        /// </summary>
        /// <param name="arete">Arête représentant la correspondance (utile que pour le debug ici !)</param>
        /// <param name="heureActuelle">L'heure actuelle du trajet</param>
        /// <param name="prochainDepart">L'heure du prochain départ</param>
        /// <param name="parametres">Paramètres de recherche (contraintes utilisateur)</param>
        /// <returns>Le coût de la correspondance en minutes</returns>
        public static double CalculerCoutCorrespondance(Arete arete,TimeSpan heureActuelle, TimeSpan prochainDepart, ParametresRecherche parametres)
        {
            if (prochainDepart == TimeSpan.Zero)
            {
                // Aucun service disponible sur la ligne de correspondance
                return double.MaxValue;
            }

            // Calculer le temps d'attente réel (prochain bus)
            double tempsAttente = (prochainDepart - heureActuelle).TotalMinutes;

            double coutCorrespondance = tempsAttente * parametres.CoefficientCorrespondance;

            // Dans CalculateurCout.cs, méthode CalculerCoutCorrespondance()
            // Avant le return coutCorrespondance;
            System.Diagnostics.Debug.WriteLine($"Correspondance {arete.NoeudDepart.ArretNoeud.NomArret}: ligne {arete.LigneUtilisee.NomLigne}, attente={tempsAttente:F1}min, coût={coutCorrespondance:F1}");
            return coutCorrespondance;
        }
    }   
}
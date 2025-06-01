using BiblioSysteme;
using Services.ServicesClasses;

namespace Services.CalculClasses
{

    public class CalculateurCout
    {

        /// <summary>
        /// Calcule le coût pour emprunter une arête (connexion) entre deux nœuds (arrêts)
        /// MODIFIÉ : Prend en compte le sens de circulation
        /// </summary>
        /// <param name="noeudActuel">Le noeud de départ</param>
        /// <param name="arete">L'arête à emprunter</param>
        /// <param name="heureActuelle">L'heure actuelle du trajet</param>
        /// <param name="parametres">Les paramètres de recherche (contraintes utilisateur)</param>
        /// <returns>Le coût du trajet en minutes</returns>
        public static double CalculerCout(Noeud noeudActuel, Arete arete, TimeSpan heureActuelle, ParametresRecherche parametres)
        {
            // Déterminer le sens de circulation en comparant les nœuds
            bool sensNormal = DeterminerSensCirculation(arete);

            // Vérifier si l'arête est une correspondance (changement de ligne)
            if (arete.EstCorrespondance)
            {
                var prochainDepart = CalculItineraireServices.TrouverProchainDepart(arete.NoeudArrivee.ArretNoeud, arete.LigneUtilisee, heureActuelle, sensNormal);
                return CalculerCoutCorrespondance(arete, heureActuelle, prochainDepart, parametres);
            }

            // Sinon, c'est un trajet normal sur une ligne
            var prochainDepartTrajet = CalculItineraireServices.TrouverProchainDepart(arete.NoeudDepart.ArretNoeud, arete.LigneUtilisee, heureActuelle, sensNormal);
            return CalculerCoutTrajet(arete, heureActuelle, prochainDepartTrajet, parametres);
        }

        /// <summary>
        /// NOUVEAU : Détermine le sens de circulation pour une arête donnée
        /// </summary>
        /// <param name="arete">L'arête à analyser</param>
        /// <returns>True si sens normal, False si sens inverse</returns>
        private static bool DeterminerSensCirculation(Arete arete)
        {
            // Pour une correspondance, on considère le sens normal (peu importe, c'est au même arrêt)
            if (arete.EstCorrespondance)
                return true;

            // Pour un trajet normal, il faut comparer les ordres des arrêts dans la ligne
            try
            {
                var ligne = arete.LigneUtilisee;
                if (ligne?.Arrets == null) return true;

                var arretDepart = ligne.Arrets.FirstOrDefault(a => a.Arret.IdArret == arete.NoeudDepart.ArretNoeud.IdArret);
                var arretArrivee = ligne.Arrets.FirstOrDefault(a => a.Arret.IdArret == arete.NoeudArrivee.ArretNoeud.IdArret);

                if (arretDepart == null || arretArrivee == null)
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ Arrêts non trouvés dans la ligne {ligne.NomLigne}");
                    return true; // Par défaut sens normal
                }

                // Sens normal si ordre croissant, sens inverse si ordre décroissant
                bool sensNormal = arretDepart.Ordre < arretArrivee.Ordre;

                //System.Diagnostics.Debug.WriteLine($"Sens déterminé: {arretDepart.Arret.NomArret}(ordre:{arretDepart.Ordre}) → {arretArrivee.Arret.NomArret}(ordre:{arretArrivee.Ordre}) = {(sensNormal ? "Normal" : "Inverse")}");

                return sensNormal;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur détermination sens : {ex.Message}");
                return true; // Par défaut sens normal
            }
        }

        /// <summary>
        /// Calcule le coût pour un trajet normal sur une ligne
        /// MODIFIÉ : Utilise le bon arrêt de départ pour l'horaire
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

            // Temps de trajet sur la ligne (déjà stocké dans l'arête avec la bonne direction)
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
        /// MODIFIÉ : Logs améliorés
        /// </summary>
        /// <param name="arete">Arête représentant la correspondance</param>
        /// <param name="heureActuelle">L'heure actuelle du trajet</param>
        /// <param name="prochainDepart">L'heure du prochain départ</param>
        /// <param name="parametres">Paramètres de recherche (contraintes utilisateur)</param>
        /// <returns>Le coût de la correspondance en minutes</returns>
        public static double CalculerCoutCorrespondance(Arete arete, TimeSpan heureActuelle, TimeSpan prochainDepart, ParametresRecherche parametres)
        {
            if (prochainDepart == TimeSpan.Zero)
            {
                // Aucun service disponible sur la ligne de correspondance
                return double.MaxValue;
            }

            // Calculer le temps d'attente réel (prochain bus)
            double tempsAttente = (prochainDepart - heureActuelle).TotalMinutes;

            double coutCorrespondance = tempsAttente * parametres.CoefficientCorrespondance;

            System.Diagnostics.Debug.WriteLine($"Correspondance {arete.NoeudDepart.ArretNoeud.NomArret}: vers ligne {arete.LigneUtilisee.NomLigne}, attente={tempsAttente:F1}min, coût={coutCorrespondance:F1}");

            return coutCorrespondance;
        }
    }
}
using BiblioSysteme;
using Services.ServicesClasses;

namespace Services.ServicesCalcul
{
    /// <summary>
    /// Service de calcul de coûts pour les itinéraires
    /// </summary>
    public static class CalculCout
    {
        /// <summary>
        /// Calcule le coût d'une étape selon les préférences utilisateur
        /// </summary>
        /// <param name="parametres">Paramètres de recherche</param>
        /// <param name="tempsTrajet">Temps de trajet en minutes</param>
        /// <param name="tempsAttente">Temps d'attente en minutes</param>
        /// <param name="estCorrespondance">True si c'est une correspondance</param>
        /// <returns>Coût de l'étape</returns>
        public static double CalculerCoutEtape(ParametresRecherche parametres, double tempsTrajet, double tempsAttente, bool estCorrespondance)
        {
            if (parametres == null)
                throw new ArgumentNullException(nameof(parametres));

            double cout = 0;

            // Coût du temps de trajet
            cout += tempsTrajet * parametres.CoefficientTempsTrajet;

            // Coût du temps d'attente
            cout += tempsAttente * parametres.CoefficientTempsAttente;

            // Coût de la correspondance
            if (estCorrespondance)
                cout += parametres.CoefficientCorrespondance;

            return cout;
        }

        /// <summary>
        /// Calcule le coût spécifique d'une correspondance
        /// </summary>
        /// <param name="parametres">Paramètres de recherche</param>
        /// <param name="noeudCourant">Nœud où se fait la correspondance</param>
        /// <param name="prochainDepart">Heure du prochain départ de la nouvelle ligne</param>
        /// <returns>Coût de la correspondance</returns>
        public static double CalculerCoutCorrespondance(ParametresRecherche parametres, Noeud noeudCourant, TimeSpan prochainDepart)
        {
            if (prochainDepart == TimeSpan.Zero)
                return double.MaxValue;

            double tempsAttente = (prochainDepart - noeudCourant.HeureArrivee).TotalMinutes;

            return noeudCourant.CoutMinimal + CalculerCoutEtape(parametres, 0, tempsAttente, true);
        }

        /// <summary>
        /// Calcule le coût spécifique d'un trajet normal
        /// </summary>
        /// <param name="parametres">Paramètres de recherche</param>
        /// <param name="noeudCourant">Nœud de départ</param>
        /// <param name="arete">Arête du trajet</param>
        /// <param name="prochainDepart">Heure du prochain départ</param>
        /// <returns>Coût du trajet</returns>
        public static double CalculerCoutTrajet(ParametresRecherche parametres, Noeud noeudCourant, Arete arete, TimeSpan prochainDepart)
        {
            if (prochainDepart == TimeSpan.Zero)
                return double.MaxValue;

            double tempsAttente = (prochainDepart - noeudCourant.HeureArrivee).TotalMinutes;
            double tempsTrajet = arete.Poids;

            return noeudCourant.CoutMinimal + CalculerCoutEtape(parametres, tempsTrajet, tempsAttente, false);
        }

        /// <summary>
        /// Calcule un score de qualité simple pour tri (plus bas = meilleur)
        /// </summary>
        /// <param name="itineraire">Itinéraire à scorer</param>
        /// <param name="heureDepart">Heure de départ souhaitée</param>
        /// <returns>Score de qualité</returns>
        public static double CalculerScoreQualite(Itineraire itineraire, TimeSpan heureDepart)
        {
            if (itineraire?.Etapes == null)
                return double.MaxValue;

            double score = 0;

            // Composante 1 : Temps total (poids 60%)
            score += itineraire.TempsTotal.TotalMinutes * 0.6;

            // Composante 2 : Correspondances (poids 25%)
            score += itineraire.NombreCorrespondances * 15 * 0.25; // 15 min de pénalité par correspondance

            // Composante 3 : Écart par rapport à l'heure souhaitée (poids 15%)
            var ecartHeureDepart = Math.Abs((itineraire.HeureDepart - heureDepart).TotalMinutes);
            score += ecartHeureDepart * 0.15;

            return score;
        }

        /// <summary>
        /// Vérifie si deux itinéraires sont suffisamment différents
        /// </summary>
        /// <param name="itineraire1">Premier itinéraire</param>
        /// <param name="itineraire2">Deuxième itinéraire</param>
        /// <returns>True si les itinéraires sont suffisamment différents</returns>
        public static bool SontSuffissammentDifferents(Itineraire itineraire1, Itineraire itineraire2)
        {
            if (itineraire1?.Etapes == null || itineraire2?.Etapes == null)
                return true;

            // Différence de durée significative (> 10%)
            var duree1 = itineraire1.TempsTotal.TotalMinutes;
            var duree2 = itineraire2.TempsTotal.TotalMinutes;
            var differenceTemps = Math.Abs(duree1 - duree2) / Math.Max(duree1, duree2);

            if (differenceTemps > 0.1) // Plus de 10% de différence
                return true;

            // Différence dans le nombre de correspondances
            if (Math.Abs(itineraire1.NombreCorrespondances - itineraire2.NombreCorrespondances) >= 1)
                return true;

            return false;
        }
    }
}
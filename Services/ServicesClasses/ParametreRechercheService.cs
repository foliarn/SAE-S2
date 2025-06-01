using BiblioSysteme;

namespace Services.ServicesClasses
{
    /// <summary>
    /// Service pour la logique métier des paramètres de recherche
    /// </summary>
    public static class ParametreRechercheService
    {
        /// <summary>
        /// Ajuste automatiquement les coefficients selon le type de recherche
        /// </summary>
        /// <param name="parametres">Paramètres à ajuster</param>
        public static void AjusterCoefficients(ParametresRecherche parametres)
        {
            if (parametres == null)
                throw new ArgumentNullException(nameof(parametres));

            switch (parametres.TypeRecherche)
            {
                case TypeRecherche.PlusRapide:
                    // Minimise le temps total - peu importe les correspondances
                    parametres.CoefficientTempsTrajet = 1.0;
                    parametres.CoefficientTempsAttente = 1.0;
                    parametres.CoefficientCorrespondance = 3.0; // Correspondances acceptées
                    break;

                case TypeRecherche.MoinsCorrespondances:
                    // Privilégie les trajets directs
                    parametres.CoefficientTempsTrajet = 1.0;
                    parametres.CoefficientTempsAttente = 1.5;
                    parametres.CoefficientCorrespondance = 15.0; // Forte pénalité correspondances
                    parametres.NombreMaxCorrespondances = 1; // Maximum 1 correspondance
                    break;

                case TypeRecherche.PlusConfortable:
                    // Minimise les temps d'attente, accepte trajets plus longs
                    parametres.CoefficientTempsTrajet = 1.2; // Temps de trajet moins important
                    parametres.CoefficientTempsAttente = 0.5; // Temps d'attente très pénalisé
                    parametres.CoefficientCorrespondance = 8.0;
                    break;
            }
        }

        /// <summary>
        /// Crée des paramètres prédéfinis pour recherche rapide
        /// </summary>
        /// <param name="heureDepart">Heure de départ souhaitée</param>
        /// <returns>Paramètres optimisés pour la rapidité</returns>
        public static ParametresRecherche PourRechercheRapide(TimeSpan heureDepart)
        {
            var parametres = new ParametresRecherche(heureDepart, TypeRecherche.PlusRapide);
            AjusterCoefficients(parametres);
            return parametres;
        }

        /// <summary>
        /// Crée des paramètres prédéfinis pour recherche confortable
        /// </summary>
        /// <param name="heureDepart">Heure de départ souhaitée</param>
        /// <returns>Paramètres optimisés pour le confort</returns>
        public static ParametresRecherche PourRechercheConfortable(TimeSpan heureDepart)
        {
            var parametres = new ParametresRecherche(heureDepart, TypeRecherche.PlusConfortable);
            AjusterCoefficients(parametres);
            return parametres;
        }

        /// <summary>
        /// Crée des paramètres prédéfinis pour recherche directe
        /// </summary>
        /// <param name="heureDepart">Heure de départ souhaitée</param>
        /// <returns>Paramètres optimisés pour minimiser les correspondances</returns>
        public static ParametresRecherche PourRechercheDirecte(TimeSpan heureDepart)
        {
            var parametres = new ParametresRecherche(heureDepart, TypeRecherche.MoinsCorrespondances);
            AjusterCoefficients(parametres);
            return parametres;
        }
    }
}
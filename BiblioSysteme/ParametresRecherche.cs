using System;

namespace BiblioSysteme
{
    /// <summary>
    /// Types de recherche d'itinéraire selon les préférences utilisateur
    /// </summary>
    public enum TypeRecherche
    {
        PlusRapide,         // Minimise le temps total de trajet
        MoinsCorrespondances, // Privilégie les trajets directs
        PlusConfortable     // Minimise les temps d'attente
    }

    /// <summary>
    /// Paramètres de recherche d'itinéraire - Version simple et pratique
    /// </summary>
    public class ParametresRecherche
    {
        // Paramètres de base
        public TimeSpan HeureDepart { get; set; }
        public TypeRecherche TypeRecherche { get; set; }

        // Limites de recherche
        public TimeSpan TempsMaxRecherche { get; set; }
        public int NombreMaxCorrespondances { get; set; }

        // Coefficients pour le calcul du coût (influencent l'algorithme)
        public double CoefficientTempsTrajet { get; set; }
        public double CoefficientTempsAttente { get; set; }
        public double CoefficientCorrespondance { get; set; }

        /// <summary>
        /// Constructeur avec paramètres par défaut (équilibrés)
        /// </summary>
        public ParametresRecherche()
        {
            HeureDepart = TimeSpan.FromHours(8); // 8h00 par défaut
            TypeRecherche = TypeRecherche.PlusRapide;
            TempsMaxRecherche = TimeSpan.FromHours(2);
            NombreMaxCorrespondances = 3;

            // Coefficients équilibrés par défaut
            CoefficientTempsTrajet = 1.0;
            CoefficientTempsAttente = 1.2; // Légèrement pénalisé
            CoefficientCorrespondance = 5.0; // Pénalité pour chaque correspondance
        }

        /// <summary>
        /// Constructeur simplifié pour usage courant
        /// </summary>
        /// <param name="heureDepart">Heure de départ souhaitée</param>
        /// <param name="typeRecherche">Type de recherche préféré</param>
        public ParametresRecherche(TimeSpan heureDepart, TypeRecherche typeRecherche) : this()
        {
            HeureDepart = heureDepart;
            TypeRecherche = typeRecherche;

            // Ajuster les coefficients selon le type de recherche
            AjusterCoefficients();
        }

        /// <summary>
        /// Ajuste automatiquement les coefficients selon le type de recherche
        /// </summary>
        private void AjusterCoefficients()
        {
            switch (TypeRecherche)
            {
                case TypeRecherche.PlusRapide:
                    // Minimise le temps total - peu importe les correspondances
                    CoefficientTempsTrajet = 1.0;
                    CoefficientTempsAttente = 1.0;
                    CoefficientCorrespondance = 3.0; // Correspondances acceptées
                    break;

                case TypeRecherche.MoinsCorrespondances:
                    // Privilégie les trajets directs
                    CoefficientTempsTrajet = 1.0;
                    CoefficientTempsAttente = 1.5;
                    CoefficientCorrespondance = 15.0; // Forte pénalité correspondances
                    NombreMaxCorrespondances = 1; // Maximum 1 correspondance
                    break;

                case TypeRecherche.PlusConfortable:
                    // Minimise les temps d'attente, accepte trajets plus longs
                    CoefficientTempsTrajet = 1.2; // Temps de trajet moins important
                    CoefficientTempsAttente = 0.5; // Temps d'attente très pénalisé
                    CoefficientCorrespondance = 8.0;
                    break;
            }
        }
/*
        /// <summary>
        /// Crée des paramètres prédéfinis pour recherche rapide
        /// </summary>
        public static ParametresRecherche PourRechercheRapide(TimeSpan heureDepart)
        {
            return new ParametresRecherche(heureDepart, TypeRecherche.PlusRapide);
        }

        /// <summary>
        /// Crée des paramètres prédéfinis pour recherche confortable (moins d'attente)
        /// </summary>
        public static ParametresRecherche PourRechercheConfortable(TimeSpan heureDepart)
        {
            return new ParametresRecherche(heureDepart, TypeRecherche.PlusConfortable);
        }

        /// <summary>
        /// Crée des paramètres prédéfinis pour recherche avec moins de correspondances
        /// </summary>
        public static ParametresRecherche PourRechercheDirecte(TimeSpan heureDepart)
        {
            return new ParametresRecherche(heureDepart, TypeRecherche.MoinsCorrespondances);
        }*/
    }
}
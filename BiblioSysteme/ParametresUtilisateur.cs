using System;

namespace BiblioSysteme
{
    // Paramètres de recherche d'itinéraire - définit les critères souhaités par l'utilisateur
    public class ParametresRecherche
    {
        public const int NOMBRE_ITINERAIRES = 2;
        public int NombreMaxCorrespondances { get; set; }
        public TimeSpan HeureSouhaitee { get; set; }
        public bool EstHeureDepart { get; set; }

        public TimeSpan TempsCorrespondanceMin { get; set; }
        public TimeSpan TempsCorrespondanceMax { get; set; }
        public TimeSpan TempsMaxRecherche { get; set; }
        
        // Préférence utilisateur (à choisir dans les paramètres de l'application)
        public double CoefficientTempsTransport { get; set; }
        public double CoefficientCorrespondance { get; set; }
        public double CoefficientAttente { get; set; }

        //Constructeurs :

        // Paramètres de base (0 changement de l'utilisateur)
        public ParametresRecherche()
        {
            HeureSouhaitee = TimeSpan.FromHours(DateTime.Now.Hour).Add(TimeSpan.FromMinutes(DateTime.Now.Minute));
            EstHeureDepart = true;
            TempsCorrespondanceMin = TimeSpan.FromMinutes(3);
            TempsCorrespondanceMax = TimeSpan.FromMinutes(20);
            TempsMaxRecherche = TimeSpan.FromHours(2);

            // On décourage les temps d'attente et les correspondances par défaut
            CoefficientTempsTransport = 1.0;
            CoefficientCorrespondance = 1.0;
            CoefficientAttente = 1.0;
        }

        // Constructeur simplifié
        public ParametresRecherche(TimeSpan heureSouhaitee, bool estHeureDepart) : this()
        {
            HeureSouhaitee = heureSouhaitee;
            EstHeureDepart = estHeureDepart;

            TempsCorrespondanceMin = TimeSpan.FromMinutes(3);
            TempsCorrespondanceMax = TimeSpan.FromMinutes(15);
            TempsMaxRecherche = TimeSpan.FromHours(2);
            CoefficientTempsTransport = 1.0;
            CoefficientCorrespondance = 1.0;
            CoefficientAttente = 1.0; 
        }

        public ParametresRecherche(TimeSpan heureSouhaitee, bool estHeureDepart,
                                 int nombreMaxCorrespondances, TimeSpan tempsCorrespondanceMin)
        {
            HeureSouhaitee = heureSouhaitee;
            EstHeureDepart = estHeureDepart;
            TempsCorrespondanceMin = tempsCorrespondanceMin;
            TempsCorrespondanceMax = TimeSpan.FromMinutes(20);
            TempsMaxRecherche = TimeSpan.FromHours(2);
        }
    }
}
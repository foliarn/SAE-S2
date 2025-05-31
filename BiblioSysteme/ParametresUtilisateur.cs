using System;

namespace BiblioSysteme
{
    // Paramètres de recherche d'itinéraire - définit les critères souhaités par l'utilisateur
    public class ParametresRecherche
    {
        public const int NombreMaxItineraires = 2;
        public const int NombreMaxCorrespondances = 2; // A voir si on ajoute les pref user
        public TimeSpan HeureSouhaitee { get; set; }
        public bool EstHeureDepart { get; set; }

        public TimeSpan TempsCorrespondanceMin { get; set; }
        public TimeSpan TempsCorrespondanceMax { get; set; }
        public TimeSpan TempsMaxRecherche { get; set; }
        
        // Préférence utilisateur (à choisir dans les paramètres de l'application)
        public bool PreferenceTrajetDirect { get; set; }
        public bool PreferenceVitesse { get; set; }

        //Constructeurs :

        // Paramètres de base (0 changement de l'utilisateur)
        public ParametresRecherche()
        {
            HeureSouhaitee = TimeSpan.FromHours(DateTime.Now.Hour).Add(TimeSpan.FromMinutes(DateTime.Now.Minute));
            EstHeureDepart = true;
            TempsCorrespondanceMin = TimeSpan.FromMinutes(3);
            TempsCorrespondanceMax = TimeSpan.FromMinutes(20);
            TempsMaxRecherche = TimeSpan.FromHours(2);
            PreferenceTrajetDirect = false;
            PreferenceVitesse = true;
        }

        // Constructeur simplifié
        public ParametresRecherche(TimeSpan heureSouhaitee, bool estHeureDepart) : this()
        {
            HeureSouhaitee = heureSouhaitee;
            EstHeureDepart = estHeureDepart;

            TempsCorrespondanceMin = TimeSpan.FromMinutes(3);
            TempsCorrespondanceMax = TimeSpan.FromMinutes(15);
            TempsMaxRecherche = TimeSpan.FromHours(2);
            PreferenceTrajetDirect = false;
            PreferenceVitesse = true;
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
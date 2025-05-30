using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblioSysteme
{
    /// <summary>
    /// Représente une étape d'un itinéraire de transport en commun.
    /// Une étape correspond à un trajet sur une ligne entre deux arrêts.
    /// </summary>
    public class EtapeItineraire
    {
        // Propriétés principales
        public Arret ArretDepart { get; set; }
        public Arret ArretArrivee { get; set; }
        public Ligne LigneUtilisee { get; set; }

        // Horaires
        public TimeSpan HeureDepart { get; set; }
        public TimeSpan HeureArrivee { get; set; }

        // Informations complémentaires
        public bool EstCorrespondance { get; set; }
        public TimeSpan? TempsAttente { get; set; }  // Temps d'attente avant cette étape
        public int NumeroEtape { get; set; }         // Position dans l'itinéraire

        // Propriétés calculées
        public TimeSpan DureeTrajet => HeureArrivee - HeureDepart;
        //public int NombreArrets => CalculerNombreArrets();

        // Constructeurs
        public EtapeItineraire()
        {
            EstCorrespondance = false;
            NumeroEtape = 0;
        }

        public EtapeItineraire(Arret arretDepart, Arret arretArrivee, Ligne ligne,
                              TimeSpan heureDepart, TimeSpan heureArrivee)
        {
            // Validation des paramètres
            ArretDepart = arretDepart ?? throw new ArgumentNullException(nameof(arretDepart));
            ArretArrivee = arretArrivee ?? throw new ArgumentNullException(nameof(arretArrivee));
            LigneUtilisee = ligne ?? throw new ArgumentNullException(nameof(ligne));

            if (arretDepart.IdArret == arretArrivee.IdArret)
                throw new ArgumentException("L'arrêt de départ ne peut pas être identique à l'arrêt d'arrivée");

            if (heureDepart >= heureArrivee)
                throw new ArgumentException("L'heure de départ doit être antérieure à l'heure d'arrivée");

            HeureDepart = heureDepart;
            HeureArrivee = heureArrivee;
            EstCorrespondance = false;
            NumeroEtape = 0;
        }
    }
}

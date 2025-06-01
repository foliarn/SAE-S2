using System;
using System.Collections.Generic;
using System.Linq;

namespace BiblioSysteme
{
    /// <summary>
    /// Représente un itinéraire complet de transport en commun.
    /// Un itinéraire est composé d'une ou plusieurs étapes connectées.
    /// </summary>
    public class Itineraire
    {
        // Propriétés principales
        public List<EtapeItineraire> Etapes { get; set; }
        public Arret ArretDepart { get; set; }
        public Arret ArretDestination { get; set; }

        // Horaires globaux
        public TimeSpan HeureDepart { get; set; }
        public TimeSpan HeureArrivee { get; set; }

        // Informations calculées
        public TimeSpan TempsTotal => HeureArrivee - HeureDepart;
        public int NombreCorrespondances => Etapes?.Count(e => e.EstCorrespondance) ?? 0;
        public List<Ligne> LignesUtilisees => Etapes?.Where(e => e.LigneUtilisee != null)
                                                    .Select(e => e.LigneUtilisee)
                                                    .Distinct()
                                                    .ToList() ?? new List<Ligne>();

        // Métadonnées
        public string TypeItineraire { get; set; }      // Vitesse, moins d'attente ou moins de correspondances

        // Constructeur
        public Itineraire(Arret arretDepart, Arret arretDestination)
        {
            ArretDepart = arretDepart ?? throw new ArgumentNullException(nameof(arretDepart));
            ArretDestination = arretDestination ?? throw new ArgumentNullException(nameof(arretDestination));

            if (arretDepart.IdArret == arretDestination.IdArret)
                throw new ArgumentException("L'arrêt de départ ne peut pas être identique à l'arrêt de destination");

            Etapes = new List<EtapeItineraire>();
            TypeItineraire = "Standard";
        }
    }
}
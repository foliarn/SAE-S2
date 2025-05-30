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

        // Informations calculées (propriétés en lecture seule)
        public TimeSpan TempsTotal => HeureArrivee - HeureDepart;
        public int NombreCorrespondances => Etapes?.Count(e => e.EstCorrespondance) ?? 0;
        public List<Ligne> LignesUtilisees => Etapes?.Where(e => e.LigneUtilisee != null)
                                                    .Select(e => e.LigneUtilisee)
                                                    .Distinct()
                                                    .ToList() ?? new List<Ligne>();

        // Métadonnées
        public DateTime DateCalcul { get; set; }
        public string TypeItineraire { get; set; }      // "Rapide", "Direct", "Économique", etc.

        // Statistiques détaillées
        public TimeSpan TempsTransport { get; set; }    // Temps effectif en transport
        public TimeSpan TempsAttenteTotale { get; set; } // Temps d'attente/correspondances
        public int NombreArrets { get; set; }           // Nombre total d'arrêts traversés

        // Constructeurs
        public Itineraire()
        {
            Etapes = new List<EtapeItineraire>();
            DateCalcul = DateTime.Now;
            TypeItineraire = "Standard";
        }

        public Itineraire(Arret arretDepart, Arret arretDestination)
        {
            ArretDepart = arretDepart ?? throw new ArgumentNullException(nameof(arretDepart));
            ArretDestination = arretDestination ?? throw new ArgumentNullException(nameof(arretDestination));

            if (arretDepart.IdArret == arretDestination.IdArret)
                throw new ArgumentException("L'arrêt de départ ne peut pas être identique à l'arrêt de destination");

            Etapes = new List<EtapeItineraire>();
            DateCalcul = DateTime.Now;
            TypeItineraire = "Standard";
        }

        public Itineraire(Arret arretDepart, Arret arretDestination, List<EtapeItineraire> etapes)
            : this(arretDepart, arretDestination)
        {
            Etapes = etapes ?? new List<EtapeItineraire>();

            // Calculer automatiquement les horaires globaux si des étapes existent
            if (Etapes.Count > 0)
            {
                HeureDepart = Etapes.First().HeureDepart;
                HeureArrivee = Etapes.Last().HeureArrivee;
            }
        }
    }
}
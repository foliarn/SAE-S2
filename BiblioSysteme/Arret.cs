using System;
using System.Collections.Generic;
using System.Linq;
using BiblioSysteme;

namespace BiblioSysteme
{
    public class Arret
    {
        // Propriétés correspondant à la table Arrets
        public int IdArret { get; set; }
        public string NomArret { get; set; }

        // Liste des lignes qui passent par cet arrêt (relation Many-to-Many)
        public List<Ligne> Lignes { get; set; }

        // Constructeurs
        public Arret()
        {
            Lignes = new List<Ligne>();
        }

        public Arret(int idArret, string nomArret)
        {
            // Validation des paramètres
            if (idArret <= 0)
            {
                throw new ArgumentException("L'ID de l'arrêt doit être positif", nameof(idArret));
            }

            if (string.IsNullOrWhiteSpace(nomArret))
            {
                throw new ArgumentException("Le nom de l'arrêt ne peut pas être vide", nameof(nomArret));
            }

            IdArret = idArret;
            NomArret = nomArret.Trim();
            Lignes = new List<Ligne>();
        }
    }

    /// <summary>
    /// Réprésente un arrêt dans une ligne de transport (avec son ordre et le temps de départ).
    /// </summary>
    public class ArretLigne
    {
        public Arret Arret { get; set; }
        public int Ordre { get; set; } // Ordre de l'arrêt dans la ligne (commence à 1 pour le premier arrêt)
        public int TempsDepart { get; set; } // Temps en minutes depuis le départ du premier arrêt de la ligne

        public ArretLigne()
        {
            Arret = new Arret();
            Ordre = 0;
            TempsDepart = 0;
        }

        public ArretLigne(Arret arret, int ordre, int tempsDepart)
        {
            Arret = arret;
            Ordre = ordre;
            TempsDepart = tempsDepart;
        }
    }
}
   
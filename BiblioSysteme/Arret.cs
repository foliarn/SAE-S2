using System;
using System.Collections.Generic;
using System.Linq;
using BiblioSysteme;

namespace BiblioSysteme
{
    public class Arret
    {
        // Colonnes de la table Arrets
        public int IdArret { get; set; }
        public string NomArret { get; set; }

        // Liste des lignes qui passent par cet arrêt
        public List<Ligne> Lignes { get; set; }

        // Constructeurs
        public Arret()
        {
            Lignes = new List<Ligne>();
        }

        public Arret(int idArret, string nomArret)
        {
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
    /// Représente un arrêt dans une ligne de transport (avec son ordre et les temps de départs).
    /// </summary>
    public class ArretLigne
    {
        public Arret Arret { get; set; }
        public int Ordre { get; set; }
        public int TempsDepuisDebut { get; set; } // Temps en minutes depuis le premier arrêt de la ligne
        public int TempsDepuisFin { get; set; }   // idem mais depuis le dernier arrêt de la ligne

        // Constructeurs
        public ArretLigne()
        {
            Arret = new Arret();
            Ordre = 0;
            TempsDepuisDebut = 0;
            TempsDepuisFin = 0;
        }

        public ArretLigne(Arret arret, int ordre, int tempsDepuisDebut)
        {
            Arret = arret;
            Ordre = ordre;
            TempsDepuisDebut = tempsDepuisDebut;
            TempsDepuisFin = 0;
        }

        public ArretLigne(Arret arret, int ordre, int tempsDepuisDebut, int tempsDepuisFin)
        {
            Arret = arret;
            Ordre = ordre;
            TempsDepuisDebut = tempsDepuisDebut;
            TempsDepuisFin = tempsDepuisFin;
        }
    }
}
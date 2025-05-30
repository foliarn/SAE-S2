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
}
   
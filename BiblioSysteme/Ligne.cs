using System;
using System.Collections.Generic;

namespace BiblioSysteme
{
    /// <summary>
    /// Classe de données représentant une ligne de transport
    /// Contient uniquement les propriétés et constructeurs simples
    /// </summary>
    public class Ligne
    {
        // Propriétés correspondant à la table Lignes
        public int IdLigne { get; set; }
        public string NomLigne { get; set; }
        public string Description { get; set; }

        // Propriétés correspondant à la table Horaires_Lignes
        public TimeSpan PremierDepart { get; set; }
        public TimeSpan DernierDepart { get; set; }
        public int IntervalleMinutes { get; set; }

        // Liste des arrêts de cette ligne dans l'ordre (relation Many-to-Many)
        public List<ArretLigne> Arrets { get; set; }

        // Constructeur par défaut
        public Ligne()
        {
            Arrets = new List<ArretLigne>();
            NomLigne = string.Empty;
            Description = string.Empty;
        }

        // Constructeur avec paramètres
        public Ligne(int idLigne, string nomLigne, string description = "") : this()
        {
            IdLigne = idLigne;
            NomLigne = nomLigne?.Trim().ToUpper() ?? string.Empty;
            Description = string.IsNullOrWhiteSpace(description) ? string.Empty : description.Trim();
        }
    }
}
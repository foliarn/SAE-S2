﻿using BiblioSysteme;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblioBDD
{
    public static class RecupDonnees
    {
        // Propriétés statiques pour stocker les données récupérées (pour éviter de les charger plusieurs fois)
        public static List<Arret> tousLesArrets { get; set; }
        public static List<Ligne> toutesLesLignes { get; set; }

        // Déclaration des requêtes multi-lignes afin d'améliorer la lisibilité
        private static string requeteGetToutesLesLignes = @"
                    SELECT l.id_ligne, l.nom_ligne, l.description, 
                        h.premier_depart, h.dernier_depart, h.intervalle_minutes
                    FROM Lignes l
                    LEFT JOIN Horaires_Lignes h ON l.id_ligne = h.id_ligne";

        private static string requeteGetArretsParLigne = @"
                    SELECT a.id_arret, a.nom_arret, la.ordre, la.temps_depart
                    FROM Arrets a
                    INNER JOIN Lignes_Arrets la ON a.id_arret = la.id_arret
                    WHERE la.id_ligne = @idLigne
                    ORDER BY la.ordre";

        private static string requeteGetLigneParId = @"
                    SELECT l.id_ligne, l.nom_ligne, l.description, 
                           h.premier_depart, h.dernier_depart, h.intervalle_minutes
                    FROM Lignes l
                    LEFT JOIN Horaires_Lignes h ON l.id_ligne = h.id_ligne
                    WHERE l.id_ligne = @idLigne";


        /// <summary>
        /// Récupère toutes les lignes de la base de données
        /// </summary>
        /// <returns>Liste des lignes ou liste vide en cas d'erreur</returns>
        public static List<Ligne> GetToutesLesLignes()
        {
            try
            {
                if (BDD.conn == null || BDD.conn.State != ConnectionState.Open)
                {
                    System.Diagnostics.Debug.WriteLine("Connexion à la base de données fermée");
                    return [];
                }

                var lignes = new List<Ligne>();

                // Étape 1 : Charger toutes les lignes sans appeler GetArretsParLigne
                using (var cmd = new MySqlCommand(requeteGetToutesLesLignes, BDD.conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var ligne = new Ligne(
                            reader.GetInt32("id_ligne"),
                            reader.GetString("nom_ligne"),
                            reader.IsDBNull("description") ? "" : reader.GetString("description")
                        );

                        // Ne PAS charger les arrêts ici
                        if (!reader.IsDBNull("premier_depart"))
                        {
                            ligne.PremierDepart = reader.GetTimeSpan("premier_depart");
                            ligne.DernierDepart = reader.GetTimeSpan("dernier_depart");
                            ligne.IntervalleMinutes = reader.GetInt32("intervalle_minutes");
                        }

                        lignes.Add(ligne);
                    }
                }

                // Étape 2 : Charger les arrêts pour chaque ligne, maintenant que le reader est fermé
                foreach (var ligne in lignes)
                {
                    ligne.Arrets = GetArretsParLigne(ligne.IdLigne);
                }

                return lignes;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la récupération des lignes : {ex.Message}");
                return [];
            }
        }

        /// <summary>
        /// Récupère une ligne spécifique par son ID
        /// </summary>
        /// <param name="idLigne">ID de la ligne</param>
        /// <returns>Ligne trouvée ou null</returns>
        public static Ligne GetLigneParId(int idLigne)
        {
            try
            {
                if (BDD.conn == null || BDD.conn.State != System.Data.ConnectionState.Open)
                {
                    System.Diagnostics.Debug.WriteLine("Connexion à la base de données fermée");
                    return new Ligne();
                }

                // On charge d'abord les arrêts de la ligne pour éviter d'avoir deux readers ouverts en même temps
                var listeArrets = GetArretsParLigne(idLigne);

                using (var cmd = new MySqlCommand(requeteGetLigneParId, BDD.conn))
                {
                    cmd.Parameters.AddWithValue("@idLigne", idLigne);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var ligne = new Ligne(
                                reader.GetInt32("id_ligne"),
                                reader.GetString("nom_ligne"),
                                reader.IsDBNull("description") ? "" : reader.GetString("description")
                            );

                            if (!reader.IsDBNull("premier_depart"))
                            {
                                ligne.PremierDepart = reader.GetTimeSpan("premier_depart");
                                ligne.DernierDepart = reader.GetTimeSpan("dernier_depart");
                                ligne.IntervalleMinutes = reader.GetInt32("intervalle_minutes");
                            }

                            // Utiliser la liste d'arrêts chargée précédemment
                            ligne.Arrets = listeArrets;

                            return ligne;
                        }
                    }
                }

                return new Ligne();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la récupération de la ligne : {ex.Message}");
                return new Ligne();
            }
        }

        /// <summary>
        /// Récupère tous les arrêts
        /// </summary>
        /// <returns>Liste des arrêts ou liste vide en cas d'erreur</returns>
        public static List<Arret> GetTousLesArrets()
        {
            try
            {
                if (BDD.conn == null || BDD.conn.State != System.Data.ConnectionState.Open)
                {
                    System.Diagnostics.Debug.WriteLine("Connexion à la base de données fermée");
                    return [];
                }

                var arrets = new List<Arret>();
                string requete = "SELECT id_arret, nom_arret FROM Arrets ORDER BY nom_arret";

                using (var cmd = new MySqlCommand(requete, BDD.conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var arret = new Arret(
                            reader.GetInt32("id_arret"),
                            reader.GetString("nom_arret")
                        );
                        arrets.Add(arret);
                    }
                }

                return arrets;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la récupération des arrêts : {ex.Message}");
                return [];
            }
        }

        /// <summary>
        /// Récupère les arrêts d'une ligne (avec leur ordre et temps de départ)
        /// </summary>
        /// <param name="idLigne">ID de la ligne</param>
        /// <returns>Liste des arrêts (ArretLigne)</returns>
        public static List<ArretLigne> GetArretsParLigne(int idLigne)
        {
            try
            {
                if (BDD.conn == null || BDD.conn.State != System.Data.ConnectionState.Open)
                {
                    System.Diagnostics.Debug.WriteLine("Connexion à la base de données fermée");
                    return [];
                }

                var arretsLigne = new List<ArretLigne>();

                using (var cmd = new MySqlCommand(requeteGetArretsParLigne, BDD.conn))
                {
                    cmd.Parameters.AddWithValue("@idLigne", idLigne);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var arret = new Arret(
                                reader.GetInt32("id_arret"),
                                reader.GetString("nom_arret")
                            );

                            var arretLigne = new ArretLigne(
                                arret,
                                reader.GetInt32("ordre"),
                                reader.GetInt32("temps_depart")
                            );
                            arretsLigne.Add(arretLigne);
                        }
                    }
                }

                return arretsLigne;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la récupération des arrêts de la ligne : {ex.Message}");
                return [];
            }
        }


        /// <summary>
        /// Récupère toutes les lignes avec leurs arrêts
        /// </summary>
        /// <returns>Liste des lignes complètes ou liste vide en cas d'erreur</returns>
        public static List<Ligne> GetToutesLesLignesCompletes()
        {
            try
            {
                var lignes = GetToutesLesLignes();
                if (lignes == null)
                {
                    return [];
                }

                foreach (var ligne in lignes)
                {
                    var arrets = GetArretsParLigne(ligne.IdLigne);
                    if (arrets != null)
                    {
                        foreach (var arret in arrets)
                        {
                            ligne.Arrets.Add(arret);
                        }
                    }
                }

                return lignes;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la récupération des lignes complètes : {ex.Message}");
                return [];
            }
        }

        // Méthodes pour actualiser chaque type de données (au cas où une desynchronisation arriverait)
        public static void ActualiserLignes()
        {
            if (BDD.conn != null && BDD.conn.State == System.Data.ConnectionState.Open)
            {
                toutesLesLignes = RecupDonnees.GetToutesLesLignes() ?? new List<Ligne>();
            }
        }

        public static void ActualiserArrets()
        {
            if (BDD.conn != null && BDD.conn.State == System.Data.ConnectionState.Open)
            {
                tousLesArrets = RecupDonnees.GetTousLesArrets() ?? new List<Arret>();
            }
        }

        public static void ActualiserTout()
        {
            ActualiserLignes();
            ActualiserArrets();
        }
    }
}

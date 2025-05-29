using BiblioSysteme;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblioBDD
{
    public static class RecupDonnees
    {
        // Déclaration des requêtes multi-lignes afin d'améliorer la lisibilité
        private static string requeteGetToutesLesLignes = @"
                    SELECT l.id_ligne, l.nom_ligne, l.description, 
                        h.premier_depart, h.dernier_depart, h.intervalle_minutes
                    FROM Lignes l
                    LEFT JOIN Horaires_Lignes h ON l.id_ligne = h.id_ligne";

        private static string requeteGetArretsParLigne = @"
                    SELECT a.id_arret, a.nom_arret, la.Ordre
                    FROM Arrets a
                    INNER JOIN Lignes_Arrets la ON a.id_arret = la.id_arret
                    WHERE la.id_ligne = @idLigne
                    ORDER BY la.Ordre";

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
                if (BDD.conn == null || BDD.conn.State != System.Data.ConnectionState.Open)
                {
                    System.Diagnostics.Debug.WriteLine("Connexion à la base de données fermée");
                    return [];
                }

                var lignes = new List<Ligne>();


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

                        // Récupération des horaires si disponibles
                        if (!reader.IsDBNull("premier_depart"))
                        {
                            ligne.PremierDepart = reader.GetTimeSpan("premier_depart");
                            ligne.DernierDepart = reader.GetTimeSpan("dernier_depart");
                            ligne.IntervalleMinutes = reader.GetInt32("intervalle_minutes");
                        }

                        lignes.Add(ligne);
                    }
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

                            // Récupération des horaires si disponibles
                            if (!reader.IsDBNull("premier_depart"))
                            {
                                ligne.PremierDepart = reader.GetTimeSpan("premier_depart");
                                ligne.DernierDepart = reader.GetTimeSpan("dernier_depart");
                                ligne.IntervalleMinutes = reader.GetInt32("intervalle_minutes");
                            }

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
        /// Récupère les arrêts d'une ligne dans l'ordre
        /// </summary>
        /// <param name="idLigne">ID de la ligne</param>
        /// <returns>Liste des arrêts ordonnés ou liste vide en cas d'erreur</returns>
        public static List<Arret> GetArretsParLigne(int idLigne)
        {
            try
            {
                if (BDD.conn == null || BDD.conn.State != System.Data.ConnectionState.Open)
                {
                    System.Diagnostics.Debug.WriteLine("Connexion à la base de données fermée");
                    return [];
                }

                var arrets = new List<Arret>();

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
                            arrets.Add(arret);
                        }
                    }
                }

                return arrets;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la récupération des arrêts de la ligne : {ex.Message}");
                return [];
            }
        }

        /// <summary>
        /// Récupère une ligne complète avec tous ses arrêts et horaires
        /// </summary>
        /// <param name="idLigne">ID de la ligne</param>
        /// <returns>Ligne complète ou ligne vide en cas d'erreur</returns>
        public static Ligne GetLigneComplete(int idLigne)
        {
            try
            {
                // Récupérer la ligne de base
                var ligne = GetLigneParId(idLigne);
                if (ligne == null)
                {
                    return new Ligne();
                }

                // Récupérer les arrêts de cette ligne
                var arrets = GetArretsParLigne(idLigne);
                if (arrets != null)
                {
                    foreach (var arret in arrets)
                    {
                        ligne.AjouterArret(arret);
                    }
                }
                ligne.GenererTempsEntreArrets(3);

                return ligne;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la récupération de la ligne complète : {ex.Message}");
                return new Ligne();
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
                            ligne.AjouterArret(arret);
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
    }
}

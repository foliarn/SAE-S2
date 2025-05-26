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
        /// <summary>
        /// Récupère toutes les lignes de la base de données
        /// </summary>
        /// <returns>Liste des lignes ou null en cas d'erreur</returns>
        public static List<Ligne> GetToutesLesLignes()
        {
            try
            {
                if (BDD.conn == null || BDD.conn.State != System.Data.ConnectionState.Open)
                {
                    System.Diagnostics.Debug.WriteLine("Connexion à la base de données fermée");
                    return null;
                }

                var lignes = new List<Ligne>();
                string requete = @"
                    SELECT l.id_ligne, l.nom_ligne, l.description, 
                        h.premier_depart, h.dernier_depart, h.intervalle_minutes
                    FROM Lignes l
                    LEFT JOIN Horaires_Lignes h ON l.id_ligne = h.id_ligne";

                using (var cmd = new MySqlCommand(requete, BDD.conn))
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
                return null;
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
                    return null;
                }

                string requete = @"
                    SELECT l.id_ligne, l.nom_ligne, l.description, 
                           h.premier_depart, h.dernier_depart, h.intervalle_minutes
                    FROM Lignes l
                    LEFT JOIN Horaires_Lignes h ON l.id_ligne = h.id_ligne
                    WHERE l.id_ligne = @idLigne";

                using (var cmd = new MySqlCommand(requete, BDD.conn))
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

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la récupération de la ligne : {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Récupère tous les arrêts
        /// </summary>
        /// <returns>Liste des arrêts ou null en cas d'erreur</returns>
        public static List<Arret> GetTousLesArrets()
        {
            try
            {
                if (BDD.conn == null || BDD.conn.State != System.Data.ConnectionState.Open)
                {
                    System.Diagnostics.Debug.WriteLine("Connexion à la base de données fermée");
                    return new List<Arret>();
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
                return new List<Arret>();
            }
        }

        /// <summary>
        /// Récupère les arrêts d'une ligne dans l'ordre
        /// </summary>
        /// <param name="idLigne">ID de la ligne</param>
        /// <returns>Liste des arrêts ordonnés ou null en cas d'erreur</returns>
        public static List<Arret> GetArretsParLigne(int idLigne)
        {
            try
            {
                if (BDD.conn == null || BDD.conn.State != System.Data.ConnectionState.Open)
                {
                    System.Diagnostics.Debug.WriteLine("Connexion à la base de données fermée");
                    return null;
                }

                var arrets = new List<Arret>();
                string requete = @"
                    SELECT a.id_arret, a.nom_arret, la.Ordre
                    FROM Arrets a
                    INNER JOIN Lignes_Arrets la ON a.id_arret = la.id_arret
                    WHERE la.id_ligne = @idLigne
                    ORDER BY la.Ordre";

                using (var cmd = new MySqlCommand(requete, BDD.conn))
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
                return null;
            }
        }

        /// <summary>
        /// Récupère une ligne complète avec tous ses arrêts et horaires
        /// </summary>
        /// <param name="idLigne">ID de la ligne</param>
        /// <returns>Ligne complète ou null en cas d'erreur</returns>
        public static Ligne GetLigneComplete(int idLigne)
        {
            try
            {
                // Récupérer la ligne de base
                var ligne = GetLigneParId(idLigne);
                if (ligne == null)
                {
                    return null;
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
                return null;
            }
        }

        /// <summary>
        /// Récupère toutes les lignes avec leurs arrêts
        /// </summary>
        /// <returns>Liste des lignes complètes ou null en cas d'erreur</returns>
        public static List<Ligne> GetToutesLesLignesCompletes()
        {
            try
            {
                var lignes = GetToutesLesLignes();
                if (lignes == null)
                {
                    return null;
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
                return null;
            }
        }
    }
}

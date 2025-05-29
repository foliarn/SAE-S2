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
    public static class ModifBDD
    {
        /// <summary>
        /// Exécute une requête SQL avec des paramètres
        /// </summary>
        /// <param name="requete">La requête SQL à exécuter</param>
        /// <param name="parametres">Les paramètres de la requête</param>
        public static void ExecuterRequete(string requete, params MySqlParameter[] parametres)
        {
            using (var cmd = new MySqlCommand(requete, BDD.conn))
            {
                if (parametres != null)
                {
                    foreach (var param in parametres)
                    {
                        cmd.Parameters.Add(param);
                    }
                }
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Ajoute une ligne à la base de données.
        /// </summary>
        /// <param name="ligne">La ligne à ajouter.</param>"
        /// <returns>True si l'opération a réussi, et False sinon</returns>
        public static bool AjouterLigne(Ligne ligne)
        {
            // Requête pour insérer la ligne
            string requete = "INSERT INTO Lignes (nom_ligne, description) VALUES (@nom, @desc);";

            using (var cmd = new MySqlCommand(requete, BDD.conn))
            {
                cmd.Parameters.AddWithValue("@nom", ligne.NomLigne);
                cmd.Parameters.AddWithValue("@desc", ligne.Description ?? "");

                try
                {
                    cmd.ExecuteNonQuery();

                    // Récupérer l'ID de la ligne nouvellement insérée
                    long idLigne = cmd.LastInsertedId;

                    // Requête pour ajouter une entrée dans Horaires_Lignes avec des valeurs par défaut
                    string requeteHoraire = @"
                        INSERT INTO Horaires_Lignes 
                        (id_ligne, premier_depart, dernier_depart, intervalle_minutes) 
                        VALUES (@idLigne, '06:00:00', '22:00:00', 15);";

                    ExecuterRequete(requeteHoraire, new MySqlParameter("@idLigne", idLigne));

                    return true;
                }
                catch (MySqlException ex)
                {
                    System.Diagnostics.Debug.WriteLine("Erreur SQL : " + ex.Message);
                    return false;
                }
            }
        }

        /// <summary>
        /// Retire une ligne de la base de données.
        /// </summary>
        /// <param name="idLigne">L'ID de la ligne à retirer.</param>
        /// <returns>True si l'opération a réussi, False sinon</returns>
        public static bool RetirerLigne(int idLigne)
        {
            try
            {
                var idParam = new MySqlParameter("@idLigne", idLigne);

                // 1. Supprimer les entrées dans Lignes_Arrets
                ExecuterRequete("DELETE FROM Lignes_Arrets WHERE id_ligne = @idLigne", idParam);

                // 2. Supprimer les entrées dans Horaires_Lignes
                ExecuterRequete("DELETE FROM Horaires_Lignes WHERE id_ligne = @idLigne", idParam);

                // 3. Supprimer la ligne dans Lignes
                ExecuterRequete("DELETE FROM Lignes WHERE id_ligne = @idLigne", idParam);

                return true;
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur SQL : " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Ajoute un arrêt à la base de données.
        /// </summary>
        /// <param name="arret">L'arrêt à ajouter.</param>
        /// <returns>True si l'opération a réussi, False sinon</returns>
        public static bool AjouterArret(Arret arret)
        {
            try
            {
                ExecuterRequete("INSERT INTO Arrets (nom_arret) VALUES (@nom)",
                    new MySqlParameter("@nom", arret.NomArret));
                return true;
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur SQL : " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Retire un arrêt de la base de données.
        /// </summary>
        /// <param name="idArret">L'ID de l'arrêt à retirer.</param>
        /// <returns>True si l'opération a réussi, False sinon</returns>
        public static bool RetirerArret(int idArret)
        {
            try
            {
                ExecuterRequete("DELETE FROM Arrets WHERE id_arret = @idArret",
                    new MySqlParameter("@idArret", idArret));
                return true;
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur SQL : " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Change le nom d'un arrêt ou d'une ligne dans la BDD
        /// </summary>
        /// <param name="id">L'ID de l'arrêt ou de la ligne</param>
        /// <param name="nouveauNom">Le nouveau nom</param>
        /// <returns>Vrai si l'opération a réussi, False sinon</returns>
        public static bool ChangerNom(int id, string nouveauNom, bool isLigne)
        {
            try
            {
                if (isLigne)
                {
                    ExecuterRequete("UPDATE Lignes SET nom_ligne = @nom WHERE id_ligne = @id",
                        new MySqlParameter("@nom", nouveauNom),
                        new MySqlParameter("@id", id));
                }
                else
                {
                    ExecuterRequete("UPDATE Arrets SET nom_arret = @nom WHERE id_arret = @id",
                        new MySqlParameter("@nom", nouveauNom),
                        new MySqlParameter("@id", id));
                }
                return true;
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur SQL : " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Ajoute un arrêt d'une ligne dans la BDD
        /// </summary>
        /// <param name="idArret">L'ID de l'arrêt à ajouter</param>
        /// <param name="idLigne">L'ID de la ligne</param>
        /// <param name="ordre">L'index de l'arrêt dans la ligne</param>
        /// <returns>True si l'opération a réussi, False sinon</returns> 
        public static bool AjouterArretALigne(int idArret, int idLigne, int ordre)
        {
            try
            {
                string requeteInsert = @"
                    INSERT INTO Lignes_Arrets (id_ligne, id_arret, ordre, temps_depart)
                    VALUES (@idLigne, @idArret, @ordre, 99);";

                ExecuterRequete(requeteInsert,
                    new MySqlParameter("@idLigne", idLigne),
                    new MySqlParameter("@idArret", idArret),
                    new MySqlParameter("@ordre", ordre));

                return true;
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur SQL : " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Retire un arrêt spécifique d'une ligne, sans affecter les autres.
        /// </summary>
        /// <param name="idArret">L'ID de l'arrêt à retirer</param>
        /// <param name="idLigne">L'ID de la ligne</param>
        /// <returns>True si l'opération a réussi, False sinon</returns>
        public static bool RetirerArretDeLigne(int idArret, int idLigne)
        {
            try
            {
                // Supprimer l'arrêt de la ligne (pas globalement)
                ExecuterRequete("DELETE FROM Lignes_Arrets WHERE id_ligne = @idLigne AND id_arret = @idArret",
                    new MySqlParameter("@idLigne", idLigne),
                    new MySqlParameter("@idArret", idArret));

                // Réordonner les arrêts de cette ligne uniquement
                ReordonnerArretsLigne(idLigne);

                return true;
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur SQL : " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Modifie l'horaire de départ d'une ligne dans la BDD
        /// </summary> 
        /// <param name="idLigne">L'ID de la ligne</param>
        /// <param name="horaireDepart"> L'horaire de départ à modifier</param>
        /// <returns>True si l'opération a réussi, False sinon</returns>
        public static bool ModifierHoraireDepart(int idLigne, TimeSpan horaireDepart)
        {
            try
            {
                ExecuterRequete("UPDATE Horaires_Lignes SET premier_depart = @horaireDepart WHERE id_ligne = @idLigne",
                    new MySqlParameter("@horaireDepart", horaireDepart),
                    new MySqlParameter("@idLigne", idLigne));
                return true;
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur SQL : " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Modifie l'intervalle de départ d'une ligne dans la BDD
        /// </summary>
        /// <param name="idLigne"> L'ID de la ligne</param>
        /// <param name="intervalleDepart"> L'intervalle de départ à modifier</param>
        /// <returns>True si l'opération a réussi, False sinon</returns>    
        public static bool ModifierIntervalleDepart(int idLigne, int intervalleDepart)
        {
            try
            {
                ExecuterRequete("UPDATE Horaires_Lignes SET intervalle_minutes = @intervalleDepart WHERE id_ligne = @idLigne",
                    new MySqlParameter("@intervalleDepart", intervalleDepart),
                    new MySqlParameter("@idLigne", idLigne));
                return true;
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur SQL : " + ex.Message);
                return false;
            }
        }

        public static bool ReordonnerArretsLigne(int idLigne)
        {
            if (BDD.conn.State != ConnectionState.Open)
            {
                System.Diagnostics.Debug.WriteLine("Erreur : Connexion non ouverte.");
                return false;
            }

            try
            {
                // 1. Récupérer les arrêts triés par ordre actuel
                string selectQuery = @"
                    SELECT id_arret 
                    FROM Lignes_Arrets 
                    WHERE id_ligne = @idLigne 
                    ORDER BY ordre;";

                using (var cmdSelect = new MySqlCommand(selectQuery, BDD.conn))
                {
                    cmdSelect.Parameters.AddWithValue("@idLigne", idLigne);

                    using (var reader = cmdSelect.ExecuteReader())
                    {
                        var arrets = new List<int>();

                        while (reader.Read())
                        {
                            arrets.Add(reader.GetInt32("id_arret"));
                        }

                        reader.Close();

                        if (arrets.Count == 0)
                        {
                            System.Diagnostics.Debug.WriteLine("Aucun arrêt trouvé pour cette ligne.");
                            return true; // Pas d'erreur, juste rien à faire
                        }

                        // 2. Réécrire les ordres proprement using ExecuterRequete
                        string updateQuery = @"
                            UPDATE Lignes_Arrets 
                            SET ordre = @ordre 
                            WHERE id_ligne = @idLigne AND id_arret = @idArret;";

                        foreach (var idArret in arrets.Select((value, index) => new { value, index }))
                        {
                            ExecuterRequete(updateQuery,
                                new MySqlParameter("@ordre", idArret.index + 1),
                                new MySqlParameter("@idLigne", idLigne),
                                new MySqlParameter("@idArret", idArret.value));
                        }
                    }
                }

                return true;
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur SQL lors du réordonnancement : " + ex.Message);
                return false;
            }
        }
    }
}
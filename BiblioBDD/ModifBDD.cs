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

                    using (var cmdHoraire = new MySqlCommand(requeteHoraire, BDD.conn))
                    {
                        cmdHoraire.Parameters.AddWithValue("@idLigne", idLigne);
                        cmdHoraire.ExecuteNonQuery();
                    }

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
        // 1. Supprimer les entrées dans Lignes_Arrets
        string requeteArrets = "DELETE FROM Lignes_Arrets WHERE id_ligne = @idLigne";
        using (var cmdArrets = new MySqlCommand(requeteArrets, BDD.conn))
        {
            cmdArrets.Parameters.AddWithValue("@idLigne", idLigne);
            cmdArrets.ExecuteNonQuery();
        }

        // 2. Supprimer les entrées dans Horaires_Lignes
        string requeteHoraires = "DELETE FROM Horaires_Lignes WHERE id_ligne = @idLigne";
        using (var cmdHoraires = new MySqlCommand(requeteHoraires, BDD.conn))
        {
            cmdHoraires.Parameters.AddWithValue("@idLigne", idLigne);
            cmdHoraires.ExecuteNonQuery();
        }

        // 3. Supprimer la ligne dans Lignes
        string requeteLigne = "DELETE FROM Lignes WHERE id_ligne = @idLigne";
        using (var cmdLigne = new MySqlCommand(requeteLigne, BDD.conn))
        {
            cmdLigne.Parameters.AddWithValue("@idLigne", idLigne);
            cmdLigne.ExecuteNonQuery();
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
        /// Ajoute un arrêt à la base de données.
        /// </summary>
        /// <param name="arret">L'arrêt à ajouter.</param>
        /// <returns>True si l'opération a réussi, False sinon</returns>
        public static bool AjouterArret(Arret arret)
        {
            string requete = "INSERT INTO Arrets (nom_arret) VALUES (@nom)";
            using (var cmd = new MySqlCommand(requete, BDD.conn))
            {
                cmd.Parameters.AddWithValue("@nom", arret.NomArret);
                try
                {
                    cmd.ExecuteNonQuery();
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
        /// Retire un arrêt de la base de données.
        /// </summary>
        /// <param name="idArret">L'ID de l'arrêt à retirer.</param>
        /// <returns>True si l'opération a réussi, False sinon</returns>
        public static bool RetirerArret(int idArret)
        {
            string requete = "DELETE FROM Arrets WHERE id_arret = @idArret";
            using (var cmd = new MySqlCommand(requete, BDD.conn))
            {
                cmd.Parameters.AddWithValue("@idArret", idArret);
                try
                {
                    cmd.ExecuteNonQuery();
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
        /// Change le nom d'un arrêt ou d'une ligne dans la BDD
        /// </summary>
        /// <param name="id">L'ID de l'arrêt ou de la ligne</param>
        /// <param name="nouveauNom">Le nouveau nom</param>
        /// <returns>Vrai si l'opération a réussi, False sinon</returns>
        public static bool ChangerNom(int id, string nouveauNom, bool isLigne)
        {
            if (isLigne)
            {
                string requete = "UPDATE Lignes SET nom_ligne = @nom WHERE id_ligne = @id";
                using (var cmd = new MySqlCommand(requete, BDD.conn))
                {
                    cmd.Parameters.AddWithValue("@nom", nouveauNom);
                    cmd.Parameters.AddWithValue("@id", id);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                    catch (MySqlException ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Erreur SQL : " + ex.Message);
                        return false;
                    }
                }
            }
            else
            {
                string requete = "UPDATE Arrets SET nom_arret = @nom WHERE id_arret = @id";
                using (var cmd = new MySqlCommand(requete, BDD.conn))
                {
                    cmd.Parameters.AddWithValue("@nom", nouveauNom);
                    cmd.Parameters.AddWithValue("@id", id);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                    catch (MySqlException ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Erreur SQL : " + ex.Message);
                        return false;
                    }
                }
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
                //Insérer le nouvel arrêt avec l'ordre choisi
                string requeteInsert = @"
                    INSERT INTO Lignes_Arrets (id_ligne, id_arret, ordre, temps_depart)
                    VALUES (@idLigne, @idArret, @ordre, 99);";

                using (var insertCmd = new MySqlCommand(requeteInsert, BDD.conn))
                {
                    insertCmd.Parameters.AddWithValue("@idLigne", idLigne);
                    insertCmd.Parameters.AddWithValue("@idArret", idArret);
                    insertCmd.Parameters.AddWithValue("@ordre", ordre);
                    insertCmd.ExecuteNonQuery();
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
                string requeteDelete = "DELETE FROM Lignes_Arrets WHERE id_ligne = @idLigne AND id_arret = @idArret";
                using (var cmd = new MySqlCommand(requeteDelete, BDD.conn))
                {
                    cmd.Parameters.AddWithValue("@idLigne", idLigne);
                    cmd.Parameters.AddWithValue("@idArret", idArret);
                    cmd.ExecuteNonQuery();
                }

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
            string requete = "UPDATE Horaires_Lignes SET premier_depart = @horaireDepart WHERE id_ligne = @idLigne";
            using (var cmd = new MySqlCommand(requete, BDD.conn))
            {
                cmd.Parameters.AddWithValue("@horaireDepart", horaireDepart);
                cmd.Parameters.AddWithValue("@idLigne", idLigne);
                try
                {
                    cmd.ExecuteNonQuery();
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
        /// Modifie l'intervalle de départ d'une ligne dans la BDD
        /// </summary>
        /// <param name="idLigne"> L'ID de la ligne</param>
        /// <param name="intervalleDepart"> L'intervalle de départ à modifier</param>
        /// <returns>True si l'opération a réussi, False sinon</returns>    
        public static bool ModifierIntervalleDepart(int idLigne, int intervalleDepart)
        {
            string requete = "UPDATE Horaires_Lignes SET intervalle_minutes = @intervalleDepart WHERE id_ligne = @idLigne";
            using (var cmd = new MySqlCommand(requete, BDD.conn))
            {
                cmd.Parameters.AddWithValue("@intervalleDepart", intervalleDepart);
                cmd.Parameters.AddWithValue("@idLigne", idLigne);
                try
                {
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (MySqlException ex)
                {
                    System.Diagnostics.Debug.WriteLine("Erreur SQL : " + ex.Message);
                    return false;
                }
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

                        // 2. Réécrire les ordres proprement
                        string updateQuery = @"
                            UPDATE Lignes_Arrets 
                            SET ordre = @ordre 
                            WHERE id_ligne = @idLigne AND id_arret = @idArret;";

                        using (var cmdUpdate = new MySqlCommand(updateQuery, BDD.conn))
                        {
                            foreach (var idArret in arrets.Select((value, index) => new { value, index }))
                            {
                                cmdUpdate.Parameters.Clear();
                                cmdUpdate.Parameters.AddWithValue("@ordre", idArret.index + 1);
                                cmdUpdate.Parameters.AddWithValue("@idLigne", idLigne);
                                cmdUpdate.Parameters.AddWithValue("@idArret", idArret.value);
                                cmdUpdate.ExecuteNonQuery();
                            }
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

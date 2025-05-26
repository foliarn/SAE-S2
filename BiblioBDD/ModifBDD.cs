using BiblioSysteme;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblioBDD
{
    public static class ModifBDD
    {
        public static MySql.Data.MySqlClient.MySqlConnection conn;
        /// <summary>
        /// Ajoute une ligne à la base de données.
        /// </summary>
        /// <param name="ligne">La ligne à ajouter.</param>"
        /// <returns>True si l'opération a réussi, et False sinon</returns>

        public static bool AjouterLigne(Ligne ligne)
        {
            string requete = "INSERT INTO lignes (nom_ligne, description) VALUES (@nom, @desc)";
            // Éviter d'utiliser des chaînes de caractères directement dans les requêtes SQL pour éviter les injections SQL
            using (var cmd = new MySqlCommand(requete, conn))
            {
                cmd.Parameters.AddWithValue("@nom", ligne.NomLigne);
                cmd.Parameters.AddWithValue("@desc", ligne.Description ?? ""); // Utiliser une chaîne vide si la description est nulle

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
        /// Retire une ligne de la base de données.
        /// </summary>
        /// <param name="idLigne">L'ID de la ligne à retirer.</param>"
        /// <returns>True si l'opération a réussi, et False sinon</returns>
        public static bool RetirerLigne(int idLigne)
        {
            string requete = "DELETE FROM lignes WHERE id_ligne = @idLigne";
            using (var cmd = new MySqlCommand(requete, conn))
            {
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
        /// Ajoute un arrêt à la base de données.
        /// </summary>
        /// <param name="arret">L'arrêt à ajouter.</param>
        /// <returns>True si l'opération a réussi, False sinon</returns>
        public static bool AjouterArret(Arret arret)
        {
            string requete = "INSERT INTO arrets (nom_arret, description) VALUES (@nom, @desc)";
            using (var cmd = new MySqlCommand(requete, conn))
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
            string requete = "DELETE FROM arrets WHERE id_arret = @idArret";
            using (var cmd = new MySqlCommand(requete, conn))
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
    }
}

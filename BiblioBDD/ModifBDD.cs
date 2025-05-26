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
    }
}

﻿using MySql.Data.MySqlClient;

namespace BiblioBDD
{
    public class BDD
    {
        public static MySql.Data.MySqlClient.MySqlConnection conn;

        public static bool OuvrirConnexion()
        {
            string serveur = "10.1.139.236";
            string login = "e3";
            string mdp = "mdp";
            string bd = "basee3";

            string chaineConnexion = $"server={serveur};uid={login};pwd={mdp};database={bd}";
            try
            {
                conn = new MySqlConnection(chaineConnexion);
                conn.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur de connexion à la base de données : {ex.Message}");
                return false;
            }
        }
    }
}

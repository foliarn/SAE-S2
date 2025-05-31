using BiblioSysteme;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BiblioBDD
{
    
    public static class ModifBDD
    {
        // Définition des requetes SQL multiligne pour éviter les répétitions et simplifier l'affichage
        private static string requeteSelectMAJ = @"
            SELECT id_arret, ordre, temps_depart 
            FROM Lignes_Arrets 
            WHERE id_ligne = @idLigne 
            ORDER BY ordre;";

        private static string requeteMAJOrdre = @"
            UPDATE Lignes_Arrets 
            SET temps_depart = @tempsDepart 
            WHERE id_ligne = @idLigne AND id_arret = @idArret;";

        /// <summary>
        /// Exécute une requête SQL avec des paramètres.
        /// </summary>
        /// <param name="requete">La requête SQL à exécuter</param>
        /// <param name="retournerId">Booléen indiquant si on souhaite récupérer l'ID généré (par exemple, après INSERT)</param>
        /// <param name="parametres">Les paramètres de la requête</param>
        /// <returns>Retourne l'ID généré (si applicable), 0 en cas de succès sans ID, -1 en cas d'erreur</returns>
        public static int ExecuterRequete(string requete, bool retournerId = false, params MySqlParameter[] parametres)
        {
            try
            {
                // Vérifie que la connexion est ouverte
                if (BDD.conn == null || BDD.conn.State != ConnectionState.Open)
                {
                    System.Diagnostics.Debug.WriteLine("Erreur : Connexion à la base non établie.");
                    return -1;
                }

                using (var cmd = new MySqlCommand(requete, BDD.conn))
                {
                    // Ajout des paramètres s'ils sont présents
                    if (parametres != null && parametres.Length > 0)
                    {
                        cmd.Parameters.AddRange(parametres);
                    }

                    // Exécution de la commande
                    cmd.ExecuteNonQuery();

                    // Renvoie l'ID inséré si demandé
                    if (retournerId)
                    {
                        return (int)cmd.LastInsertedId;
                    }
                }

                // Retourne 0 si tout s'est bien passé mais qu'on ne veut pas l'ID
                return 0;
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur SQL : " + ex.Message);
                return -1; // Erreur SQL
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur inattendue : " + ex.Message);
                return -1; // Autre erreur
            }
        }

        /// <summary>
        /// Ajoute une ligne à la base de données.
        /// </summary>
        /// <param name="ligne">La ligne à ajouter.</param>
        /// <returns>L'id de la ligne créée, ou -1 si l'opération a échoué</returns>
        public static int AjouterLigne(Ligne ligne)
        {
            // Vérification basique des données avant insertion
            if (ligne == null)
            {
                System.Diagnostics.Debug.WriteLine("Erreur : La ligne à ajouter est null.");
                return -1;
            }

            // Préparation des paramètres
            var parametres = new MySqlParameter[]
            {
                new MySqlParameter("@nom", ligne.NomLigne),
                new MySqlParameter("@desc", ligne.Description ?? "") // Utilisation d'une chaîne vide si la description est null
            };

            return ExecuterRequete("INSERT INTO Lignes (nom_ligne, description) VALUES (@nom, @desc);", true, parametres);
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
                ExecuterRequete("DELETE FROM Lignes_Arrets WHERE id_ligne = @idLigne",false, idParam);

                // 2. Supprimer les entrées dans Horaires_Lignes
                ExecuterRequete("DELETE FROM Horaires_Lignes WHERE id_ligne = @idLigne",false, idParam);

                // 3. Supprimer la ligne dans Lignes
                ExecuterRequete("DELETE FROM Lignes WHERE id_ligne = @idLigne",false, idParam);

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
        /// <returns>L'ID de l'arrêt, et -1 si l'opération échoue</returns>
        public static int AjouterArret(Arret arret)
        {
            // Vérification que l'objet n'est pas null
            if (arret == null)
            {
                System.Diagnostics.Debug.WriteLine("Erreur : L'arrêt à ajouter est null.");
                return -1;
            }

            // Préparation des paramètres
            var parametres = new MySqlParameter[]
            {
                new MySqlParameter("@nom", arret.NomArret)
            };

            return ExecuterRequete("INSERT INTO Arrets (nom_arret) VALUES (@nom);", true, parametres);
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
                    false,
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
                        false,
                        new MySqlParameter("@nom", nouveauNom),
                        new MySqlParameter("@id", id));
                }
                else
                {
                    ExecuterRequete("UPDATE Arrets SET nom_arret = @nom WHERE id_arret = @id",
                        false,
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

                int succes = ExecuterRequete(requeteInsert,
                    false,
                    new MySqlParameter("@idLigne", idLigne),
                    new MySqlParameter("@idArret", idArret),
                    new MySqlParameter("@ordre", ordre));

                if (succes == -1)
                {
                    System.Diagnostics.Debug.WriteLine("Erreur lors de l'ajout de l'arrêt à la ligne.");
                    return false;
                }

                // Méthodes pour réordonner les arrêts et mettre à jour les temps de départ
                ReordonnerArretsLigne(idLigne);
                MAJTempsDepart(idLigne, ordre, true);
                
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
        /// <param name="ordre">L'ordre de l'arrêt dans la ligne (pour la MAJ des temps)</param>
        /// <returns>True si l'opération a réussi, False sinon</returns>
        public static bool RetirerArretDeLigne(int idArret, int idLigne, int ordre)
        {
            try
            {
                // Supprimer l'arrêt de la ligne (pas globalement)
                ExecuterRequete("DELETE FROM Lignes_Arrets WHERE id_ligne = @idLigne AND id_arret = @idArret",
                    false,
                    new MySqlParameter("@idLigne", idLigne),
                    new MySqlParameter("@idArret", idArret));

                // On réordonne les arrêts et on met à jour les temps de départ
                ReordonnerArretsLigne(idLigne);
                MAJTempsDepart(idLigne, ordre , false);

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
                    false,
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
                    false,
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
                // A. On récupère tous les arrêts de la ligne dans l'ordre 
                using (var cmdSelect = new MySqlCommand(requeteSelectMAJ, BDD.conn))
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

                        foreach (var idArret in arrets.Select((value, index) => new { value, index }))
                        {
                            ExecuterRequete(updateQuery,
                                false,
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
        /// <summary>
        /// Met à jour les temps de départ des arrêts d'une ligne en ajoutant ou soustrayant un delta aléatoire (1 à 3 min) par rapport au précédent.
        /// </summary>
        /// <param name="idLigne">L'ID de la ligne à mettre à jour.</param>
        /// <param name="ordre">L'ordre de l'arrêt à partir duquel on commence la mise à jour (1 pour le premier arrêt).</param>
        /// <param name="ajout">Booléen indiquant si on ajoute ou soustrait les minutes</param>
        /// <returns>True si l'opération a réussi, False sinon</returns>
        public static bool MAJTempsDepart(int idLigne, int ordre, bool ajout = true)
        {
            if (BDD.conn.State != ConnectionState.Open)
            {
                System.Diagnostics.Debug.WriteLine("Erreur : Connexion non ouverte.");
                return false;
            }

            var random = new Random();

            try
            {
                // 1. Récupérer les arrêts avec leurs temps_depart existants
                var arrets = RecupDonnees.GetArretsParLigne(idLigne);

                if (arrets.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("Aucun arrêt trouvé pour cette ligne.");
                    return false;
                }

                // 2. Calculer à partir de l'arrêt spécifié (ordre)
                int tempsCourant = arrets[ordre - 1].TempsDepart;

                // 3. Mettre à jour chaque arrêt suivant
                foreach (var item in arrets)
                {
                    // On ajoute ou soustrait un delta aléatoire entre 1 et 3 minutes
                    int delta = random.Next(1, 4); // Entre 1 et 3 inclus

                    if (!ajout) delta = -delta;
                    tempsCourant += delta;
                    // Mise à jour du temps de départ pour l'arrêt courant
                    ExecuterRequete(requeteMAJOrdre,
                        false,
                        new MySqlParameter("@tempsDepart", tempsCourant),
                        new MySqlParameter("@idLigne", idLigne),
                        new MySqlParameter("@idArret", item.Arret.IdArret));
                }
                return true;
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur SQL lors de la mise à jour des temps de départ : " + ex.Message);
                return false;
            }
        }

    }
}
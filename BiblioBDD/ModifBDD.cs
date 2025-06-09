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
            SELECT id_arret, ordre
            FROM Lignes_Arrets 
            WHERE id_ligne = @idLigne 
            ORDER BY ordre;";

        /// <summary>
        /// Exécute une requête SQL avec des paramètres. -- non utilisé pour les SELECT (reader), mais pour INSERT, UPDATE, DELETE 
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
                if (Connexion.conn == null || Connexion.conn.State != ConnectionState.Open)
                {
                    System.Diagnostics.Debug.WriteLine("Erreur : Connexion à la base non établie.");
                    return -1;
                }

                using (var cmd = new MySqlCommand(requete, Connexion.conn))
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

            int idLigne = ExecuterRequete("INSERT INTO Lignes (nom_ligne, description) VALUES (@nom, @desc);", true, parametres);

            // Créer l'entrée dans Horaires_Lignes
            var parametres_horaires = new MySqlParameter[]
        {
            new MySqlParameter("@id_ligne", idLigne),
            new MySqlParameter("@premier_depart", TimeSpan.FromHours(6)),      // 06:00
            new MySqlParameter("@dernier_depart", TimeSpan.FromHours(22)),     // 22:00
            new MySqlParameter("@intervalle_minutes", 15)                      // 15 minutes
        };

            string requete_horaires = @"
                INSERT INTO Horaires_Lignes (id_ligne, premier_depart, dernier_depart, intervalle_minutes)
                VALUES (@id_ligne, @premier_depart, @dernier_depart, @intervalle_minutes);";

            int resultat_horaires = ExecuterRequete(requete_horaires, false, parametres_horaires);

            if (resultat_horaires == -1)
            {
                System.Diagnostics.Debug.WriteLine("⚠️ Erreur lors de la création des horaires, mais ligne créée.");
                // Ne pas échouer complètement, la ligne existe
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"✅ Horaires créés pour la ligne {idLigne}");
            }

            return idLigne;
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
        public static bool AjouterArretALigne(int idLigne, int idArret, int ordre)
        {
            try
            {
                // Récupérer les arrêts existants DANS L'ORDRE
                var arretsExistants = RecupDonnees.GetArretsParLigne(idLigne)
                                                 .OrderBy(a => a.Ordre)
                                                 .ToList();

                int tempsDepuisDebut = 0;
                int tempsDepuisFin = 0;

                // LOGIQUE CORRIGÉE pour les temps bidirectionnels
                if (arretsExistants.Count == 0)
                {
                    // Premier arrêt de la ligne
                    tempsDepuisDebut = 0;
                    tempsDepuisFin = 0;
                }
                else if (ordre == 1)
                {
                    // Insertion en première position
                    tempsDepuisDebut = 0;
                    // Décaler tous les autres arrêts
                    var ancienPremier = arretsExistants.FirstOrDefault();
                    if (ancienPremier != null)
                    {
                        tempsDepuisFin = ancienPremier.TempsDepuisFin + new Random().Next(2, 5);
                    }
                }
                else if (ordre > arretsExistants.Count)
                {
                    // Ajout à la fin
                    var dernierArret = arretsExistants.LastOrDefault();
                    if (dernierArret != null)
                    {
                        tempsDepuisDebut = dernierArret.TempsDepuisDebut + new Random().Next(2, 5);
                        tempsDepuisFin = 0;
                    }
                }
                else
                {
                    // Insertion au milieu - calculer par interpolation
                    var arretPrecedent = arretsExistants.Where(a => a.Ordre < ordre).LastOrDefault();
                    var arretSuivant = arretsExistants.Where(a => a.Ordre >= ordre).FirstOrDefault();

                    if (arretPrecedent != null && arretSuivant != null)
                    {
                        // Interpolation linéaire
                        tempsDepuisDebut = (arretPrecedent.TempsDepuisDebut + arretSuivant.TempsDepuisDebut) / 2;
                        tempsDepuisFin = (arretPrecedent.TempsDepuisFin + arretSuivant.TempsDepuisFin) / 2;
                    }
                    else if (arretPrecedent != null)
                    {
                        tempsDepuisDebut = arretPrecedent.TempsDepuisDebut + new Random().Next(2, 5);
                        tempsDepuisFin = Math.Max(0, arretPrecedent.TempsDepuisFin - new Random().Next(2, 5));
                    }
                }

                // Insérer le nouvel arrêt
                string requeteInsert = @"
                    INSERT INTO Lignes_Arrets (id_ligne, id_arret, ordre, temps_depuis_debut, temps_depuis_fin)
                    VALUES (@idLigne, @idArret, @ordre, @tempsDepuisDebut, @tempsDepuisFin);";

                int succes = ExecuterRequete(requeteInsert, false,
                    new MySqlParameter("@idLigne", idLigne),
                    new MySqlParameter("@idArret", idArret),
                    new MySqlParameter("@ordre", ordre),
                    new MySqlParameter("@tempsDepuisDebut", tempsDepuisDebut),
                    new MySqlParameter("@tempsDepuisFin", tempsDepuisFin));

                if (succes == -1)
                {
                    System.Diagnostics.Debug.WriteLine("Erreur lors de l'ajout de l'arrêt à la ligne.");
                    return false;
                }

                // IMPORTANT : Réordonner et recalculer APRÈS insertion
                ReordonnerArretsLigne(idLigne);
                RecalculerTempsBidirectionnels(idLigne);

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur SQL : " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Retire un arrêt spécifique d'une ligne.
        /// </summary>
        /// <param name="idArret">L'ID de l'arrêt à retirer</param>
        /// <param name="idLigne">L'ID de la ligne</param>
        /// <param name="ordre">L'ordre de l'arrêt dans la ligne</param>
        /// <returns>True si l'opération a réussi, False sinon</returns>
        public static bool RetirerArretDeLigne(int idArret, int idLigne)
        {
            try
            {
                // Supprimer l'arrêt de la ligne (pas globalement)
                ExecuterRequete("DELETE FROM Lignes_Arrets WHERE id_ligne = @idLigne AND id_arret = @idArret",
                    false,
                    new MySqlParameter("@idLigne", idLigne),
                    new MySqlParameter("@idArret", idArret));

                // Réordonner les arrêts restants et recalculer tous les temps bidirectionnels
                ReordonnerArretsLigne(idLigne);
                RecalculerTempsBidirectionnels(idLigne);

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
        /// <summary>
        /// Réordonne les arrêts d'une ligne dans la BDD (utilisé après insertion ou suppression d'un arrêt)
        /// </summary>
        /// <param name="idLigne"></param>
        /// <returns>True si l'opération a réussi, false sinon</returns>
        public static bool ReordonnerArretsLigne(int idLigne)
        {
            if (Connexion.conn.State != ConnectionState.Open)
            {
                System.Diagnostics.Debug.WriteLine("Erreur : Connexion non ouverte.");
                return false;
            }
            try
            {
                //On récupère tous les arrêts de la ligne dans l'ordre 
                using (var cmdSelect = new MySqlCommand(requeteSelectMAJ, Connexion.conn))
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
                            return true;
                        }

                        // Puis on réécrit les ordres proprement depuis le début
                        string updateQuery = @"
                            UPDATE Lignes_Arrets 
                            SET ordre = @ordre 
                            WHERE id_ligne = @idLigne AND id_arret = @idArret;";

                        for (int i = 0; i < arrets.Count; i++)
                        {
                            ExecuterRequete(updateQuery,
                                false,
                                new MySqlParameter("@ordre", i + 1), // Ordre commence à 1
                                new MySqlParameter("@idLigne", idLigne),
                                new MySqlParameter("@idArret", arrets[i]));
                        }
                    }
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
        /// Recalcule tous les temps bidirectionnels d'une ligne après modification
        /// </summary>
        /// <param name="idLigne">ID de la ligne</param>
        /// <returns>True si succès, False sinon</returns>
        private static bool RecalculerTempsBidirectionnels(int idLigne)
        {
            try
            {
                // Récupérer tous les arrêts de la ligne DANS L'ORDRE
                var arretsLigne = RecupDonnees.GetArretsParLigne(idLigne)
                                             .OrderBy(a => a.Ordre)
                                             .ToList();

                if (arretsLigne.Count == 0) return true;

                var random = new Random();

                // ======= SENS NORMAL (temps_depuis_debut) =======
                int tempsAccumule = 0;
                for (int i = 0; i < arretsLigne.Count; i++)
                {
                    var arret = arretsLigne[i];

                    if (i == 0)
                    {
                        tempsAccumule = 0; // Premier arrêt
                    }
                    else
                    {
                        tempsAccumule += random.Next(2, 6); // 2-5 minutes entre arrêts
                    }

                    // Mise à jour en base
                    ExecuterRequete(
                        "UPDATE Lignes_Arrets SET temps_depuis_debut = @temps WHERE id_ligne = @idLigne AND id_arret = @idArret",
                        false,
                        new MySqlParameter("@temps", tempsAccumule),
                        new MySqlParameter("@idLigne", idLigne),
                        new MySqlParameter("@idArret", arret.Arret.IdArret));
                }

                // ======= SENS INVERSE (temps_depuis_fin) =======
                tempsAccumule = 0;
                for (int i = arretsLigne.Count - 1; i >= 0; i--)
                {
                    var arret = arretsLigne[i];

                    if (i == arretsLigne.Count - 1)
                    {
                        tempsAccumule = 0; // Dernier arrêt
                    }
                    else
                    {
                        tempsAccumule += random.Next(2, 6); // 2-5 minutes entre arrêts
                    }

                    // Mise à jour en base
                    ExecuterRequete(
                        "UPDATE Lignes_Arrets SET temps_depuis_fin = @temps WHERE id_ligne = @idLigne AND id_arret = @idArret",
                        false,
                        new MySqlParameter("@temps", tempsAccumule),
                        new MySqlParameter("@idLigne", idLigne),
                        new MySqlParameter("@idArret", arret.Arret.IdArret));
                }

                System.Diagnostics.Debug.WriteLine($"Temps bidirectionnels recalculés pour ligne {idLigne}");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur recalcul temps bidirectionnels : {ex.Message}");
                return false;
            }
        }
    }
}
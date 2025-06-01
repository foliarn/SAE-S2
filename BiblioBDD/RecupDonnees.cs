using BiblioSysteme;
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
        // Déclaration des requêtes multi-lignes afin d'améliorer la lisibilité
        private static string requeteGetToutesLesLignes = @"
                    SELECT l.id_ligne, l.nom_ligne, l.description, 
                        h.premier_depart, h.dernier_depart, h.intervalle_minutes
                    FROM Lignes l
                    LEFT JOIN Horaires_Lignes h ON l.id_ligne = h.id_ligne";

        private static string requeteGetArretsParLigne = @"
                    SELECT a.id_arret, a.nom_arret, la.ordre, 
                           la.temps_depuis_debut, la.temps_depuis_fin
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
                if (Connexion.conn == null || Connexion.conn.State != ConnectionState.Open)
                {
                    System.Diagnostics.Debug.WriteLine("Connexion à la base de données fermée");
                    return [];
                }

                var lignes = new List<Ligne>();

                // On charge d'abord les lignes sans leurs arrêts pour éviter d'avoir deux readers ouverts en même temps
                using (var cmd = new MySqlCommand(requeteGetToutesLesLignes, Connexion.conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
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

                        lignes.Add(ligne);
                    }
                }

                // Puis on charge les arrêts pour chaque ligne (reader séparés)
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
                if (Connexion.conn == null || Connexion.conn.State != System.Data.ConnectionState.Open)
                {
                    System.Diagnostics.Debug.WriteLine("Connexion à la base de données fermée");
                    return new Ligne();
                }

                // On charge d'abord les arrêts de la ligne pour éviter d'avoir deux readers ouverts en même temps
                var listeArrets = GetArretsParLigne(idLigne);

                using (var cmd = new MySqlCommand(requeteGetLigneParId, Connexion.conn))
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
                if (Connexion.conn == null || Connexion.conn.State != System.Data.ConnectionState.Open)
                {
                    System.Diagnostics.Debug.WriteLine("Connexion à la base de données fermée");
                    return [];
                }

                var arrets = new List<Arret>();
                string requete = "SELECT id_arret, nom_arret FROM Arrets ORDER BY nom_arret";

                using (var cmd = new MySqlCommand(requete, Connexion.conn))
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
        /// Récupère les arrêts d'une ligne (avec leur ordre et temps de départs)
        /// </summary>
        /// <param name="idLigne">ID de la ligne</param>
        /// <returns>Liste des arrêts (ArretLigne)</returns>
        public static List<ArretLigne> GetArretsParLigne(int idLigne)
        {
            try
            {
                if (Connexion.conn == null || Connexion.conn.State != System.Data.ConnectionState.Open)
                {
                    System.Diagnostics.Debug.WriteLine("Connexion à la base de données fermée");
                    return [];
                }

                var arretsLigne = new List<ArretLigne>();

                using (var cmd = new MySqlCommand(requeteGetArretsParLigne, Connexion.conn))
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
                                reader.GetInt32("temps_depuis_debut"),
                                reader.GetInt32("temps_depuis_fin")
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
    }
}
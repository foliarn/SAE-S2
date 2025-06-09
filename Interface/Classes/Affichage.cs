using BiblioSysteme;
using System.Data;
using BiblioBDD;
using Services.ServicesClasses;

namespace Interface.Classes
{
    public static class Affichage
    {
        public class ArretHoraire // Classe pour représenter un arrêt et ses horaires (faciliter l'affichage dans DataGridView)
        {
            public string NomArret { get; set; }
            public string Horaires { get; set; } // Format texte pour affichage
        }

        /// <summary>
        /// Affiche une ligne complète avec ses arrêts et horaires
        /// </summary>
        /// <param name="idLigne">ID de la ligne à afficher</param>
        /// <param name="dgv">DataGridView pour l'affichage</param>
        /// <param name="sensNormal">True pour sens normal, False pour sens inverse</param>
        public static void AfficherLigneComplete(int idLigne, DataGridView dgv, bool sensNormal = true)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== AFFICHAGE LIGNE {idLigne} (Sens: {(sensNormal ? "Normal" : "Inverse")}) ===");

                // Récupérer la ligne
                Ligne ligne = RecupDonnees.GetLigneParId(idLigne);
                if (ligne == null)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Ligne {idLigne} non trouvée");
                    dgv.DataSource = null;
                    dgv.Rows.Clear();
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"Ligne trouvée : {ligne.NomLigne}");
                System.Diagnostics.Debug.WriteLine($"Horaires : {ligne.PremierDepart} - {ligne.DernierDepart}, Intervalle : {ligne.IntervalleMinutes}min");
                System.Diagnostics.Debug.WriteLine($"Nombre d'arrêts : {ligne.Arrets?.Count ?? 0}");

                // VÉRIFICATIONS préalables
                if (ligne.Arrets == null || ligne.Arrets.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ Aucun arrêt dans cette ligne");
                    dgv.DataSource = null;
                    dgv.Rows.Clear();
                    return;
                }

                // VÉRIFICATION des horaires
                if (!LigneService.EstLigneValide(ligne))
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ Ligne invalide - affichage des arrêts seulement");

                    // Afficher juste les arrêts sans horaires (dans l'ordre demandé)
                    var arretsOrdonnes = sensNormal ?
                        ligne.Arrets.OrderBy(al => al.Ordre) :
                        ligne.Arrets.OrderByDescending(al => al.Ordre);

                    var listeSansHoraires = arretsOrdonnes.Select(al => new ArretHoraire
                    {
                        NomArret = al.Arret.NomArret,
                        Horaires = "Horaires non configurés"
                    }).ToList();

                    ConfigurerColonnes(dgv);
                    dgv.DataSource = listeSansHoraires;
                    return;
                }

                // Générer la liste avec horaires
                var liste = new List<ArretHoraire>();

                // Ordonner les arrêts selon le sens demandé
                var arretsOrdonnesAvecHoraires = sensNormal ?
                    ligne.Arrets.OrderBy(al => al.Ordre) :
                    ligne.Arrets.OrderByDescending(al => al.Ordre);

                foreach (var arretLigne in arretsOrdonnesAvecHoraires)
                {
                    System.Diagnostics.Debug.WriteLine($"Traitement arrêt : {arretLigne.Arret.NomArret}");

                    Arret arret = arretLigne.Arret;

                    // GÉNÉRATION SÉCURISÉE des horaires selon le sens
                    List<TimeSpan> horaires = ArretService.ObtenirHorairesPassage(ligne, arret, sensNormal);

                    string horairesStr;
                    if (horaires.Count > 0)
                    {
                        horairesStr = string.Join(" - ", horaires.Select(h => h.ToString(@"hh\:mm")));
                    }
                    else
                    {
                        horairesStr = "Aucun horaire";
                    }

                    liste.Add(new ArretHoraire
                    {
                        NomArret = arret.NomArret,
                        Horaires = horairesStr
                    });
                }

                // Configurer le DataGridView
                ConfigurerColonnes(dgv);
                dgv.DataSource = liste;

                System.Diagnostics.Debug.WriteLine($"✅ Affichage terminé : {liste.Count} arrêts (sens {(sensNormal ? "normal" : "inverse")})");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erreur affichage ligne : {ex.Message}");
                dgv.DataSource = null;
                dgv.Rows.Clear();
            }
        }

        /// <summary>
        /// Affiche une ligne dans le sens normal (ordre croissant)
        /// </summary>
        /// <param name="idLigne">ID de la ligne</param>
        /// <param name="dgv">DataGridView pour l'affichage</param>
        public static void AfficherLigneSensNormal(int idLigne, DataGridView dgv)
        {
            AfficherLigneComplete(idLigne, dgv, true);
        }

        /// <summary>
        /// Affiche une ligne dans le sens inverse (ordre décroissant)
        /// </summary>
        /// <param name="idLigne">ID de la ligne</param>
        /// <param name="dgv">DataGridView pour l'affichage</param>
        public static void AfficherLigneSensInverse(int idLigne, DataGridView dgv)
        {
            AfficherLigneComplete(idLigne, dgv, false);
        }

        /// <summary>
        /// Obtient le nom des terminus d'une ligne pour affichage
        /// </summary>
        /// <param name="ligne">La ligne</param>
        /// <returns>Tuple contenant (premier terminus, dernier terminus)</returns>
        public static (string premierTerminus, string dernierTerminus) ObtenirTerminus(Ligne ligne)
        {
            if (ligne?.Arrets == null || ligne.Arrets.Count == 0)
                return ("Inconnu", "Inconnu");

            var arretsOrdonnes = ligne.Arrets.OrderBy(al => al.Ordre).ToList();

            string premierTerminus = arretsOrdonnes.First().Arret.NomArret;
            string dernierTerminus = arretsOrdonnes.Last().Arret.NomArret;

            return (premierTerminus, dernierTerminus);
        }

        //Configuration des colonnes
        private static void ConfigurerColonnes(DataGridView dgv)
        {
            dgv.AutoGenerateColumns = false;

            // Vider les colonnes existantes pour éviter les doublons
            dgv.Columns.Clear();

            // Créer la colonne pour le nom de l'arrêt
            var colNom = new DataGridViewTextBoxColumn
            {
                Name = "colNomArret",
                HeaderText = "Arrêt",
                DataPropertyName = "NomArret",
                Width = 200
            };
            dgv.Columns.Add(colNom);

            // Créer la colonne pour les horaires
            var colHoraire = new DataGridViewTextBoxColumn
            {
                Name = "colHoraires",
                HeaderText = "Horaires",
                DataPropertyName = "Horaires",
                Width = 400
            };
            dgv.Columns.Add(colHoraire);
        }
    }
}
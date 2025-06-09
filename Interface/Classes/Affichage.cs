using BiblioSysteme;
using System.Data;
using BiblioBDD;
using Services.ServicesClasses;

//using Services;

namespace Interface.Classes
{
    public static class Affichage
    {
        public class ArretHoraire // Classe pour représenter un arrêt et ses horaires (faciliter l'affichage dans DataGridView)
        {
            public string NomArret { get; set; }
            public string Horaires { get; set; } // Format texte pour affichage
        }
        public static void AfficherLigneComplete(int idLigne, DataGridView dgv)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== AFFICHAGE LIGNE {idLigne} ===");

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

                    // Afficher juste les arrêts sans horaires
                    var listeSansHoraires = ligne.Arrets.Select(al => new ArretHoraire
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

                foreach (var arretLigne in ligne.Arrets)
                {
                    System.Diagnostics.Debug.WriteLine($"Traitement arrêt : {arretLigne.Arret.NomArret}");

                    Arret arret = arretLigne.Arret;

                    // GÉNÉRATION SÉCURISÉE des horaires
                    List<TimeSpan> horaires = ArretService.ObtenirHorairesPassage(ligne, arret, true);

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

                System.Diagnostics.Debug.WriteLine($"✅ Affichage terminé : {liste.Count} arrêts");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erreur affichage ligne : {ex.Message}");
                dgv.DataSource = null;
                dgv.Rows.Clear();
            }
        }

        //Configuration des colonnes
        private static void ConfigurerColonnes(DataGridView dgv)
        {
            dgv.AutoGenerateColumns = false;

            // Créer les colonnes si elles n'existent pas encore
            if (dgv.Columns["colNomArret"] == null)
            {
                var colNom = new DataGridViewTextBoxColumn
                {
                    Name = "colNomArret",
                    HeaderText = "Arrêt",
                    DataPropertyName = "NomArret",
                    Width = 200
                };
                dgv.Columns.Add(colNom);
            }

            if (dgv.Columns["colHoraires"] == null)
            {
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
}

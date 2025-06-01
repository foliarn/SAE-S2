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
            // Récupérer la ligne
            Ligne ligne = RecupDonnees.GetLigneParId(idLigne);

            if (ligne == null)
            {
                dgv.DataSource = null;
                dgv.Rows.Clear();
                return;
            }

            var liste = new List<ArretHoraire>();

            foreach (var arretLigne in ligne.Arrets)
            {
                Arret arret = arretLigne.Arret;

                // Récupère TOUS les horaires de la journée en passant un horaire bas
                TimeSpan horaireDebutJournee = new TimeSpan(5, 0, 0); // 05:00 (avant le début de service)
                List<TimeSpan> horaires = ArretService.ObtenirHorairesPassage(ligne, arret, true); // true = sens normal

                string horairesStr = string.Join(" - ", horaires.Select(h => h.ToString(@"hh\:mm")));

                liste.Add(new ArretHoraire
                {
                    NomArret = arret.NomArret,
                    Horaires = horairesStr
                });
            }

            dgv.AutoGenerateColumns = false;

            // Créer les colonnes si elles n'existent pas encore
            if (dgv.Columns["colNomArret"] == null)
            {
                var colNom = new DataGridViewTextBoxColumn
                {
                    Name = "colNomArret",
                    HeaderText = "Arrêt",
                    DataPropertyName = "NomArret"
                };
                dgv.Columns.Add(colNom);
            }

            if (dgv.Columns["colHoraires"] == null)
            {
                var colHoraire = new DataGridViewTextBoxColumn
                {
                    Name = "colHoraires",
                    HeaderText = "Horaires",
                    DataPropertyName = "Horaires"
                };
                dgv.Columns.Add(colHoraire);
            }

            dgv.DataSource = liste;
        }
    }
}

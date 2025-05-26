using BiblioSysteme;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiblioBDD;

namespace SAE_S2.Classes
{
    public static class Affichage
    {
        public class ArretHoraireViewModel
        {
            public string NomArret { get; set; }
            public string Horaires { get; set; } // Format texte pour affichage
        }

        public static void AfficherLigneComplete(int idLigne, DataGridView dgv)
        {
            // Récupérer la ligne
            Ligne ligne = RecupDonnees.GetLigneComplete(idLigne);

            if (ligne == null || !ligne.EstValide())
            {
                dgv.DataSource = null;
                dgv.Rows.Clear();
                return;
            }

            var liste = new List<ArretHoraireViewModel>();

            foreach (var arret in ligne.Arrets)
            {
                // Utilise GetHorairesDepart() + TempsEntreArrets via CalculerHorairesPourArret()
                var horaires = ligne.GetHorairesPourArret(arret);
                string horairesStr = string.Join(" - ", horaires.Select(h => h.ToString(@"hh\:mm")));

                liste.Add(new ArretHoraireViewModel
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
                var colH = new DataGridViewTextBoxColumn
                {
                    Name = "colHoraires",
                    HeaderText = "Horaires",
                    DataPropertyName = "Horaires"
                };
                dgv.Columns.Add(colH);
            }

            dgv.DataSource = liste;
        }
    }
}

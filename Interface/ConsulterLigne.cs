using BiblioSysteme;
using Interface.Classes;
using Services;

namespace Interface
{
    public partial class ConsulterLigne : Form
    {
        private Accueil formAccueil; // Référence au formulaire d'accueil pour le retour au menu
        private int idLigne; // ID de la ligne à afficher
        private Ligne ligneSelectionnee; 
        public ConsulterLigne(Accueil accueil)
        {
            InitializeComponent();
            formAccueil = accueil;

            // Initialisation des données et de l'interface
            Utils.RemplirComboBox(cmbChoixLigne, Init.toutesLesLignes, "NomLigne", "IdLigne");
            Utils.CentrerControle(dgvLigne, false, true);
            Utils.CentrerControle(pnlChoixLigne, false, true);

            // Directement afficher une ligne à l'ouverture du formulaire
            if (cmbChoixLigne.Items.Count > 0)
            {
                cmbChoixLigne.SelectedIndex = 0; // Sélectionner le premier élément
                ligneSelectionnee = cmbChoixLigne.SelectedItem as Ligne;
                idLigne = (int)cmbChoixLigne.SelectedValue;
                Affichage.AfficherLigneComplete(idLigne, dgvLigne);
                lblTitreDgv.Text = "Arrêts et horaires de la ligne " + ligneSelectionnee.NomLigne;

                // Centrer le titre du DataGridView
                lblTitreDgv.Left = dgvLigne.Left + (dgvLigne.Width - lblTitreDgv.Width) / 2;
            }
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            this.Dispose();
            formAccueil.Show();
        }

        private void btnValiderChoix_Click(object sender, EventArgs e)
        {
            ligneSelectionnee = cmbChoixLigne.SelectedItem as Ligne;
            idLigne = (int)cmbChoixLigne.SelectedValue;
            Affichage.AfficherLigneComplete(idLigne, dgvLigne);
            lblTitreDgv.Text = "Arrêts et horaires de la ligne " + ligneSelectionnee.NomLigne;
        }
    }
}

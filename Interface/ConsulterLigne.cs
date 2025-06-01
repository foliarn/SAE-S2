using BiblioSysteme;
using Interface.Classes;
using Services;

namespace Interface
{
    public partial class ConsulterLigne : Form
    {
        private Accueil formAccueil;
        private int idLigne;
        private Ligne ligneSelectionnee;
        public ConsulterLigne(Accueil accueil)
        {
            InitializeComponent();
            formAccueil = accueil;
            Utils.RemplirComboBox(cmbChoixLigne, Init.toutesLesLignes, "NomLigne", "IdLigne");
            Utils.CentrerControle(dgvLigne, false, true);
            Utils.CentrerControle(pnlChoixLigne, false, true);

            // Directement afficher une ligne
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

        private void ConsulterLigne_Load(object sender, EventArgs e)
        {

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

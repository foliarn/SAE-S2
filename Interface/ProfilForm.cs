using System;
using System.Windows.Forms;
using BiblioSysteme;

namespace Interface
{
    /// <summary>
    /// C'est la fenêtre de profil utilisateur.
    /// </summary>
    public partial class ProfilForm : Form
    {
        public event EventHandler SeDeconnecter;

        public ParametresRecherche parametresUser { get; set; }

        public ProfilForm()
        {
            InitializeComponent();

            parametresUser = new ParametresRecherche();

            // Charger les données utilisateur existantes
            if (Login.username != null)
            {
                txtUsername.Text = Login.username;
            }
        }

        private void btnFermer_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void btnSauvegarder_Click(object sender, EventArgs e)
        {
            try
            {
                // Logique de sauvegarde des paramètres
                Login.username = txtUsername.Text;
                Login.password = txtPassword.Text; // Si vous avez un champ mot de passe

                if (rdoModeVitesse.Checked)
                {
                    parametresUser.CoefficientTempsTrajet = 3.0;
                    parametresUser.CoefficientTempsAttente = 1.0;
                    parametresUser.CoefficientCorrespondance = 1.0;
                }
                else if (rdoModeNoAttente.Checked)
                {
                    parametresUser.CoefficientTempsTrajet = 1.0;
                    parametresUser.CoefficientTempsAttente = 3.0;
                    parametresUser.CoefficientCorrespondance = 1.0;
                }
                else
                {
                    parametresUser.CoefficientTempsTrajet = 1.0;
                    parametresUser.CoefficientTempsAttente = 1.0;
                    parametresUser.CoefficientCorrespondance = 3.0;
                }

                MessageBox.Show("Paramètres sauvegardés !", "Succès",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la sauvegarde des paramètres : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnDeconnexion_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Êtes-vous sûr de vouloir vous déconnecter ?",
                "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Login.estConnecte = false; // Mettre à jour l'état de connexion
                this.Hide();
                SeDeconnecter?.Invoke(this, EventArgs.Empty);
            }
        }

        private void btnMenuAdmin_Click(object sender, EventArgs e)
        {
            this.Hide(); // Fermer le ProfilForm

            MenuAdmin menuAdmin = new MenuAdmin();
            menuAdmin.FormClosed += (s, args) =>
            {
            };
            menuAdmin.ShowDialog();
        }
    }
}
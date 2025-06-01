using BiblioBDD;
using BiblioSysteme;
using Interface.Classes;
using Services;

namespace Interface
{

    public partial class Accueil : Form
    {
        public ProfilForm profilForm;

        private bool estElargi = false;
        private Size tailleOG;
        private Point LocationOG;
        public Accueil()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Dpi;
            Init.Initialiser();

            // Remplir les comboBox avec les arrêts
            Utils.RemplirComboBox(cmbDepart, Init.tousLesArrets, "NomArret", "IdArret");
            Utils.RemplirComboBox(cmbDest, Init.tousLesArrets, "NomArret", "IdArret");

            // Positionner le plan d'abord
            pnlPlan.Top = (ClientSize.Height - pnlPlan.Height) / 2;
            pnlPlan.Left = ClientSize.Width - pnlPlan.Width - 50;

            // Centrer le panel de recherche dans l'espace disponible à gauche
            pnlRecherche.Left = (pnlPlan.Left - pnlRecherche.Width) / 2;
            pnlRecherche.Top = (ClientSize.Height - pnlRecherche.Height) / 2;

            // Centrer les éléments dans le panel de recherche
            lblTitre.Left = (pnlRecherche.ClientSize.Width - lblTitre.Width) / 2;
            btnTrouver.Left = (pnlRecherche.ClientSize.Width - btnTrouver.Width) / 2;

            // Autres éléments
            btnLigne.Location = new Point(pnlPlan.Left + (pnlPlan.Width - btnLigne.Width) / 2, pnlPlan.Bottom + 10);
            lblPlan.Location = new Point(pnlPlan.Left + (pnlPlan.Width - lblPlan.Width) / 2, pnlPlan.Top - 50);
            picLogo.Location = new Point(pnlRecherche.Left + (pnlRecherche.Width - picLogo.Width) / 2, pnlRecherche.Top - 130);
            picLogin.Location = new Point(this.ClientSize.Width - 56, 8);

            profilForm = new ProfilForm();

        }

        private void chkHeure_CheckedChanged(object sender, EventArgs e)
        {
            if (chkHeure.Checked)
            {
                pnlRecherche.Size = new Size(250, 200);

                rdoDepart.Visible = false;
                rdoArrive.Visible = false;
                dtpHeure.Visible = false;
            }

            else
            {
                pnlRecherche.Size = new Size(250, 275);

                rdoDepart.Top = chkHeure.Bottom + 10;
                rdoArrive.Top = chkHeure.Bottom + 10;
                dtpHeure.Top = rdoDepart.Bottom + 15;

                rdoDepart.Location = new Point(15, chkHeure.Bottom + 10);
                rdoArrive.Location = new Point(130, chkHeure.Bottom + 10);

                rdoDepart.Visible = true;
                rdoArrive.Visible = true;
                dtpHeure.Visible = true;

            }

            Utils.CentrerControle(pnlRecherche, false, true);
        }

        private void btnTrouver_Click(object sender, EventArgs e)
        {
            try
            {
                // Validation des sélections
                if (cmbDepart.SelectedItem == null || cmbDest.SelectedItem == null)
                {
                    MessageBox.Show("Veuillez sélectionner un arrêt de départ et de destination.",
                        "Sélection requise", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // On récupère les arrêts sélectionnés 
                var arretDepart = cmbDepart.SelectedItem as Arret;
                var arretDestination = cmbDest.SelectedItem as Arret;

                if (arretDepart == null || arretDestination == null)
                {
                    MessageBox.Show("Erreur lors de la récupération des arrêts sélectionnés.",
                        "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // On vérifie que les arrêts sont différents
                if (arretDepart.IdArret == arretDestination.IdArret)
                {
                    MessageBox.Show("L'arrêt de départ et de destination doivent être différents.",
                        "Erreur de sélection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // On crée les paramètres de recherche
                var parametres = ParametresHelper.CreerDepuisInterface(chkHeure, dtpHeure, rdoDepart, rdoArrive);

                // On ouvre la page d'itinéraire !
                PageItineraire pageItineraire = new PageItineraire(this, arretDepart, arretDestination, parametres);
                pageItineraire.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la recherche d'itinéraire : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"Erreur btnTrouver_Click (Accueil) : {ex.Message}");
            }
        }

        private void picLogin_Click(object sender, EventArgs e)
        {
  

            // Créer et afficher le formulaire de login
            if (Login.estConnecte == true)
            {
                // Positionner au centre du MenuAdmin
                profilForm.Location = new Point(
                    this.Location.X + (this.Width - profilForm.Width) / 2,
                    this.Location.Y + (this.Height - profilForm.Height) / 2
                );

                profilForm.Show();
            }
            else
            {
                this.Hide();
                Login login = new Login(this);
                login.ShowDialog();
                // Une fois le formulaire de login fermé, on revient à l'accueil
                this.Show();
            }
        }

        private void pnlPlan_Click(object sender, EventArgs e)
        {
            if (estElargi == false)
            {
                // On garde la taille et la position d'origine
                tailleOG = pnlPlan.Size;
                LocationOG = pnlPlan.Location;

                //Agrandir l'image et la centrer
                pnlPlan.Size = new Size(1088, 612);
                pnlPlan.Location = new Point((ClientSize.Width - pnlPlan.Width) / 2, (ClientSize.Height - pnlPlan.Height) / 2);
                pnlPlan.Top = (ClientSize.Height - pnlPlan.Height) / 2;
                pnlPlan.BringToFront();
                picRetrecir.Visible = true;
                picRetrecir.Parent = picPlan;
                picRetrecir.BackColor = Color.Transparent;
                picPlan.Cursor = Cursors.Default;
            }
            estElargi = !estElargi; // Inversion de l'état de l'agrandissement
        }

        private void picRetrecir_Click(object sender, EventArgs e)
        {
            // On remet l'image à sa taille et position d'origine
            pnlPlan.Size = tailleOG;
            pnlPlan.Location = LocationOG;
            picRetrecir.Visible = false;
            picPlan.Cursor = Cursors.Hand;
        }

        private void btnLigne_Click(object sender, EventArgs e)
        {
            this.Hide();
            // Ouvre le formulaire ConsulterLigne
            ConsulterLigne consulterLigne = new ConsulterLigne(this);
            consulterLigne.ShowDialog();
            this.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MenuAdmin menuAdmin = new MenuAdmin(this);
            menuAdmin.ShowDialog();
        }

        
    }
}

using BiblioBDD;
using BiblioSysteme;
using Interface.Classes;
using Services;

namespace Interface
{
    public partial class Accueil : Form
    {

        private void chkHeure_CheckedChanged(object sender, EventArgs e)
        {
            if (chkHeure.Checked)
            {
                pnlRecherche.Size = new Size(500, 500);

                rdoDepart.Visible = false;
                rdoArrive.Visible = false;
                dtpHeure.Visible = false;
            }

            else
            {
                pnlRecherche.Size = new Size(500, 500);

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
        public Accueil()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Dpi;
            BDD.OuvrirConnexion(); // Ouvre la connexion à la base de données
            RecupDonnees.tousLesArrets = RecupDonnees.GetTousLesArrets(); // Charge tous les arrêts depuis la base de données
            RecupDonnees.toutesLesLignes = RecupDonnees.GetToutesLesLignes(); // Charge toutes les lignes depuis la base de données

            // Remplir les comboBox avec les arrêts
            Utils.RemplirComboBox(cmbDepart, RecupDonnees.tousLesArrets, "NomArret", "IdArret");
            Utils.RemplirComboBox(cmbDest, RecupDonnees.tousLesArrets, "NomArret", "IdArret");

            Utils.CentrerControle(pnlRecherche, false, true);
            // Pour centrer les éléments dans le panel de recherche
            lblTitre.Left = (pnlRecherche.ClientSize.Width - lblTitre.Width) / 2;
            btnTrouver.Left = (pnlRecherche.ClientSize.Width - btnTrouver.Width) / 2;
            btnTrouver.Left = (pnlRecherche.ClientSize.Width - btnTrouver.Width) / 2;

            // Pour centrer le plan puis l'ancrer à droite
            pnlPlan.Top = (ClientSize.Height - pnlPlan.Height) / 2;
            pnlPlan.Left = ClientSize.Width - pnlPlan.Width - 50;
            btnLigne.Location = new Point(pnlPlan.Left + (pnlPlan.Width - btnLigne.Width) / 2, pnlPlan.Bottom + 10);
            lblPlan.Location = new Point(pnlPlan.Left + (pnlPlan.Width - lblPlan.Width) / 2, pnlPlan.Top - 50);
            picLogo.Location = new Point(pnlRecherche.Left + (pnlRecherche.Width - picLogo.Width) / 2 + 15, pnlRecherche.Top - 185);


            picLogin.Location = new Point(this.ClientSize.Width - 56, 8);

        }
        private bool estElargi = false;
        private Size tailleOG;
        private Point LocationOG;

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

        private void picLogin_Click(object sender, EventArgs e)
        {
            this.Hide();

            // Créer et afficher le formulaire de login
            if (Login.estConnecte == true)
            {
                // Si l'utilisateur est déjà connecté, on ouvre le menu admin
                MenuAdmin menuAdmin = new MenuAdmin();
                menuAdmin.ShowDialog();
                this.Show(); // On revient à l'accueil après la fermeture du menu admin
                return;
            }
            else
            {
                Login login = new Login();
                login.ShowDialog();
            }


            // Une fois le formulaire de login fermé, on revient à l'accueil
            this.Show();
        }

        private void btnLigne_Click(object sender, EventArgs e)
        {

            // Ouvre le formulaire ConsulterLigne
            ConsulterLigne consulterLigne = new ConsulterLigne(this);
            consulterLigne.Show();

            // Ferme le formulaire actuel
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MenuAdmin menuAdmin = new MenuAdmin();
            menuAdmin.ShowDialog();
        }

        private void btnTrouver_Click(object sender, EventArgs e)
        {
            try
            {
                // === VALIDATION DES SÉLECTIONS ===
                if (cmbDepart.SelectedItem == null || cmbDest.SelectedItem == null)
                {
                    MessageBox.Show("Veuillez sélectionner un arrêt de départ et de destination.",
                        "Sélection requise", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // === RÉCUPÉRATION DES ARRÊTS ===  
                var arretDepart = cmbDepart.SelectedItem as Arret;
                var arretDestination = cmbDest.SelectedItem as Arret;

                if (arretDepart == null || arretDestination == null)
                {
                    MessageBox.Show("Erreur lors de la récupération des arrêts sélectionnés.",
                        "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // === VALIDATION DÉPART ≠ DESTINATION ===
                if (arretDepart.IdArret == arretDestination.IdArret)
                {
                    MessageBox.Show("L'arrêt de départ et de destination doivent être différents.",
                        "Erreur de sélection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // === CRÉATION DES PARAMÈTRES AVEC LA CLASSE UTILITAIRE ===
                // ✅ UTILISATION DE ParametresHelper
                var parametres = ParametresHelper.CreerDepuisInterface(chkHeure, dtpHeure, rdoDepart, rdoArrive);

                // Optionnel : afficher les paramètres créés pour le débogage
                string descriptionParametres = ParametresHelper.DecrireParametres(parametres);
                System.Diagnostics.Debug.WriteLine($"Paramètres créés : {descriptionParametres}");

                // === OUVERTURE DE LA PAGE ITINÉRAIRE ===
                //PageItineraire pageItineraire = new PageItineraire(this, arretDepart, arretDestination, parametres);
                //pageItineraire.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la recherche d'itinéraire : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"Erreur btnTrouver_Click (Accueil) : {ex.Message}");
            }
        }
    }
}

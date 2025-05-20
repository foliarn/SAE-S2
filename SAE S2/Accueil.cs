using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SAE_S2
{
    public partial class Accueil : Form
    {

        private void chkHeure_CheckedChanged(object sender, EventArgs e)
        {
            if (chkHeure.Checked)
            {
                pnlRecherche.Size = new Size(250, 250);

                rdoDepart.Visible = false;
                rdoArrive.Visible = false;
                dtpHeure.Visible = false;
            }

            else
            {
                pnlRecherche.Size = new Size(250, 300);

                rdoDepart.Top = chkHeure.Bottom + 10;
                rdoArrive.Top = chkHeure.Bottom + 10;
                dtpHeure.Top = rdoDepart.Bottom + 15;

                rdoDepart.Location = new Point(15, chkHeure.Bottom + 10);
                rdoArrive.Location = new Point(130, chkHeure.Bottom + 10);

                rdoDepart.Visible = true;
                rdoArrive.Visible = true;
                dtpHeure.Visible = true;

            }

            CentrerPanel();
        }

        private void CentrerPanel()
        {
            int y = (this.ClientSize.Height - pnlRecherche.Height) / 2;

            pnlRecherche.Location = new Point(50, y);

        }
        public Accueil()
        {
            InitializeComponent();

            CentrerPanel();
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
            Login login = new Login();
            login.ShowDialog();

            // Une fois le formulaire de login fermé, on revient à l'accueil
            this.Show();
        }

        private void btnLigne_Click(object sender, EventArgs e)
        {
            this.Hide();

            /*            ChoixLigne choixLigne = new ChoixLigne();
                        choixLigne.ShowDialog();*/


        }

        private void button1_Click(object sender, EventArgs e)
        {
            MenuAdmin menuAdmin = new MenuAdmin();
            menuAdmin.ShowDialog();
        }
    }
}

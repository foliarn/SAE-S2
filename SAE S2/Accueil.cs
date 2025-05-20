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
        private void Accueil_Load(object sender, EventArgs e)
        {


        }

        private void chkHeure_CheckedChanged(object sender, EventArgs e)
        {
            if (chkHeure.Checked)
            {
                pnlRecherche.Size = new Size(350, 335);

                rdoDepart.Visible = false;
                rdoArrive.Visible = false;
                dtpHeure.Visible = false;
            }

            else
            {
                pnlRecherche.Size = new Size(350, 400);

                rdoDepart.Top = chkHeure.Bottom + 10;
                rdoArrive.Top = chkHeure.Bottom + 10;
                dtpHeure.Top = rdoDepart.Bottom + 15;

                rdoDepart.Location = new Point(15, chkHeure.Bottom + 10); 
                rdoArrive.Location = new Point(180, chkHeure.Bottom + 10);

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
            // Pour centrer les éléments dans le panel
            lblTitre.Left = (pnlRecherche.ClientSize.Width - lblTitre.Width) / 2;
            btnTrouver.Left = (pnlRecherche.ClientSize.Width - btnTrouver.Width) / 2;
            btnTrouver.Left = (pnlRecherche.ClientSize.Width - btnTrouver.Width) / 2;

        }
    }
}

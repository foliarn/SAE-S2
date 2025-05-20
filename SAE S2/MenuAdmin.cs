using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SAE_S2
{
    public partial class MenuAdmin : Form
    {
        public MenuAdmin()
        {
            InitializeComponent();
        }
        private void pnlCreerLigne_Click(object sender, EventArgs e)
        {
            pnlMenuCreation.Visible = false;
            pnlMenuModif.Visible = false;
            lblTitre.Text = "Création d'une ligne";

            // Afficher le formulaire de création de ligne
            pnlCreation.Visible = true;
            lblSaisirNomHead.Text = "Saisir le nom de la nouvelle ligne";

        }

        private void pnlCreerArret_Click(object sender, EventArgs e)
        {
            pnlMenuCreation.Visible = false;
            pnlMenuModif.Visible = false;
            lblTitre.Text = "Création d'un arrêt";

            pnlCreation.Visible = true;
            lblSaisirNomHead.Text = "Saisir le nom du nouvel arrêt";


        }

        private void pnlModifLigne_Click(object sender, EventArgs e)
        {
            pnlMenuCreation.Visible = false;
            pnlMenuModif.Visible = false;
            lblTitre.Text = "Modification d'une ligne";
        }

        private void pnlModifArret_Click(object sender, EventArgs e)
        {
            pnlMenuCreation.Visible = false;
            pnlMenuModif.Visible = false;
            lblTitre.Text = "Modification d'un arrêt";
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}

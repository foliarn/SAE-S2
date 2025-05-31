using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BiblioSysteme;
using BiblioBDD;
using Interface.Classes;

namespace Interface
{
    public partial class ConsulterLigne : Form
    {
        private Accueil formAccueil;
        public ConsulterLigne(Accueil accueil)
        {
            InitializeComponent();
            formAccueil = accueil;
            Utils.RemplirComboBox(cmbChoixLigne, RecupDonnees.toutesLesLignes, "NomLigne", "IdLigne");
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
            int idLigne = (int)cmbChoixLigne.SelectedValue;
            Affichage.AfficherLigneComplete(idLigne, dgvLigne);

        }
    }
}

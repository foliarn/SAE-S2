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
        private bool isLigne; // true si on modifie une ligne, false si on modifie un arrêt

        public MenuAdmin()
        {

            InitializeComponent();
            // Centrer les panels dans le formulaire
            Utils.CentrerControle(pnlCreation);
            Utils.CentrerControle(pnlModifChoix);
            Utils.CentrerControle(pnlModifArretChoisi, false, true);
            Utils.CentrerControle(pnlSupprVerif, false, true);
            Utils.CentrerControle(pnlArretChangeNom, false, true);
            Utils.CentrerControle(pnlArretModifLigne, false, true);
            Utils.CentrerControle(pnlModifLigneChoisie, false, true);
            Utils.CentrerControle(pnlLigneAjoutArret, false, true);
            Utils.CentrerControle(pnlRetirerArret, false, true);
            Utils.CentrerControle(pnlModifHoraire, false, true);
            Utils.CentrerControle(pnlMenuCreation, false, true);
            Utils.CentrerControle(pnlMenuModif, false, true);

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
            pnlLigneAjoutArret.Visible = false;
            pnlMenuCreation.Visible = false;
            pnlMenuModif.Visible = false;
            lblTitre.Text = "Création d'un arrêt";

            pnlCreation.Visible = true;
            lblSaisirNomHead.Text = "Saisir le nom du nouvel arrêt";
        }

        private void pnlModifLigne_Click(object sender, EventArgs e)
        {
            isLigne = true; // On modifie une ligne
            pnlMenuCreation.Visible = false;
            pnlMenuModif.Visible = false;
            lblTitre.Text = "Modification d'une ligne";

            lblModifHead.Text = "Choisir une ligne";
            lblModif.Text = "Choisissez une ligne à modifier :";
            pnlModifChoix.Visible = true;
        }

        private void pnlModifArret_Click(object sender, EventArgs e)
        {
            pnlMenuCreation.Visible = false;
            pnlMenuModif.Visible = false;
            lblTitre.Text = "Modification d'un arrêt";

            pnlModifChoix.Visible = true;
        }



        private void btnMenu_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnValiderChoix_Click(object sender, EventArgs e)
        {
            pnlModifChoix.Visible = false;
            if (isLigne)
            {
                pnlModifLigneChoisie.Visible = true;
            }
            else
            {
                pnlModifArretChoisi.Visible = true;
            }
        }

        private void pnlModifAppartenance_Click(object sender, EventArgs e)
        {
            pnlArretModifLigne.Visible = true;
            pnlArretChangeNom.Visible = false;
            pnlSupprVerif.Visible = false;
        }

        private void pnlChangerNom_Click(object sender, EventArgs e)
        {
            pnlArretModifLigne.Visible = false;
            pnlArretChangeNom.Visible = true;
            pnlSupprVerif.Visible = false;
        }

        private void pnlSupprArret_Click(object sender, EventArgs e)
        {
            pnlArretModifLigne.Visible = false;
            pnlArretChangeNom.Visible = false;
            pnlSupprVerif.Visible = true;
        }

        private void pnlMenuModifHoraire_Click(object sender, EventArgs e)
        {
            pnlSupprVerif.Visible = false;
            pnlModifChoix.Visible = false;
            pnlCreation.Visible = false;
            pnlModifHoraire.Visible = true;
            pnlLigneAjoutArret.Visible = false;
            pnlArretChangeNom.Visible = false;
            pnlRetirerArret.Visible = false;

        }

        private void pnlMenuAjoutArret_Click(object sender, EventArgs e)
        {
            pnlSupprVerif.Visible = false;
            pnlModifChoix.Visible = false;
            pnlCreation.Visible = false;
            pnlModifHoraire.Visible = false;
            pnlLigneAjoutArret.Visible = true;
            pnlArretChangeNom.Visible = false;
            pnlRetirerArret.Visible = false;

        }

        private void pnlMenuRetraitArret_Click(object sender, EventArgs e)
        {
            pnlSupprVerif.Visible = false;
            pnlModifChoix.Visible = false;
            pnlCreation.Visible = false;
            pnlModifHoraire.Visible = false;
            pnlLigneAjoutArret.Visible = false;
            pnlArretChangeNom.Visible = false;
            pnlRetirerArret.Visible = true;

        }

        private void pnlMenuLigneChangeNom_Click(object sender, EventArgs e)
        {
            pnlSupprVerif.Visible = false;
            pnlModifChoix.Visible = false;
            pnlCreation.Visible = false;
            pnlModifHoraire.Visible = false;
            pnlLigneAjoutArret.Visible = false;
            pnlArretChangeNom.Visible = true;
            pnlRetirerArret.Visible = false;

        }

        private void pnlMenuSupprLigne_Click(object sender, EventArgs e)
        {

            lblTitreSupprVerif.Text = "Êtes-vous sûr de vouloir supprimer cette ligne ?";
            pnlSupprVerif.Visible = true;
            pnlModifChoix.Visible = false;
            pnlCreation.Visible = false;
            pnlModifHoraire.Visible = false;
            pnlLigneAjoutArret.Visible = false;
            pnlArretChangeNom.Visible = false;
            pnlRetirerArret.Visible = false;


        }
    }
}

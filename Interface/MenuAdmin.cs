using SAE_S2.Classes;
using BiblioSysteme;
using BiblioBDD;
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
        private int idChoixMain; // Id de l'arrêt ou de la ligne choisie pour modification
        private int idChoixAjtOuSupp; // Id de l'arrêt ou de la ligne choisie pour ajout ou suppression d'un arrêt ou d'une ligne

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



            // Remplir les ComboBox avec les arrêts
            Utils.RemplirComboBox(cmbLigneAjoutArret, ChargerDonnees.tousLesArrets, "NomArret", "IdArret");
            Utils.RemplirComboBox(cmbLigneRetraitArret, ChargerDonnees.tousLesArrets, "NomArret", "IdArret");
            Utils.RemplirComboBox(cmbArretModifLigneChoixAdd, ChargerDonnees.toutesLesLignes, "NomLigne", "IdLigne"); //TODO : Ajouter un élément "Aucun" pour éviter les erreurs si aucune ligne n'est sélectionnée
            Utils.RemplirComboBox(cmbArretModifLigneChoixSuppr, ChargerDonnees.toutesLesLignes, "NomLigne", "IdLigne"); //TODO : idem

        }
        private void pnlCreerLigne_Click(object sender, EventArgs e)
        {
            pnlMenuCreation.Visible = false;
            pnlMenuModif.Visible = false;
            pnlLigneAjoutArret.Visible = false;
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

            Utils.RemplirComboBox(cmbChoix, ChargerDonnees.tousLesArrets, "NomArret", "IdArret");
        }

        private void pnlModifLigne_Click(object sender, EventArgs e)
        {
            isLigne = true; // On modifie une ligne
            pnlMenuCreation.Visible = false;
            pnlMenuModif.Visible = false;
            pnlModifChoix.Visible = true;

            lblTitre.Text = "Modification d'une ligne";
            lblModifHead.Text = "Choisir une ligne";
            lblModif.Text = "Choisissez une ligne à modifier :";

            Utils.RemplirComboBox(cmbChoix, ChargerDonnees.toutesLesLignes, "NomLigne", "IdLigne");

        }

        private void pnlModifArret_Click(object sender, EventArgs e)
        {
            pnlMenuCreation.Visible = false;
            pnlMenuModif.Visible = false;
            pnlModifChoix.Visible = true;

            lblTitre.Text = "Modification d'un arrêt";
            lblModifHead.Text = "Choisir un arrêt";
            lblModif.Text = "Choisissez un arrêt à modifier :";
            
            Utils.RemplirComboBox(cmbChoix, ChargerDonnees.tousLesArrets, "NomArret", "IdArret");
        }



        private void btnMenu_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnValiderChoix_Click(object sender, EventArgs e)
        {
            pnlModifChoix.Visible = false;
            idChoixMain = cmbChoix.SelectedIndex;
            if (isLigne)
            {
                //todo : Afficher le nom de la ligne choisie
                pnlModifLigneChoisie.Visible = true;
            }
            else
            {
                // todo : Afficher le nom de l'arrêt choisi
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
        //AjtArret
        private void btnValiderAjoutArret_Click(object sender, EventArgs e)
        {

        }

        private void btnRetirerArret_Click(object sender, EventArgs e)
        {
            ModifBDD.RetirerArret(cmbLigneRetraitArret.SelectedIndex); // Pas sûr de l'index, à vérifier (index de la liste ou index de la ComboBox ?)
        }

        private void nudChoixPlace_ValueChanged(object sender, EventArgs e)
        {
            //Maximum = count + 1

        }
    }
}

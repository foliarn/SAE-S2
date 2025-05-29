using BiblioBDD;
using BiblioSysteme;
using MySql.Data.MySqlClient;
using SAE_S2.Classes;
using System.Data;

namespace SAE_S2
{
    public partial class MenuAdmin : Form
    {
        private bool isLigne; // true si on modifie une ligne, false si on modifie un arrêt
        private bool inMenu; // true si on est dans le menu de création/modification, false si on est sur le menu principal
        private int idChoixMain; // Id de l'arrêt ou de la ligne choisie pour modification

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
            ActualiserComboBoxes();

        }

        public void ActualiserComboBoxes()
        {
            //Actualiser les données depuis la BDD
            ChargerDonnees.ActualiserTout();

            Utils.RemplirComboBox(cmbChoix,
            isLigne ? ChargerDonnees.toutesLesLignes.Cast<object>().ToList() : ChargerDonnees.tousLesArrets.Cast<object>().ToList(),
            isLigne ? "NomLigne" : "NomArret",
            isLigne ? "IdLigne" : "IdArret");

            Utils.RemplirComboBox(cmbLigneAjoutArret, ChargerDonnees.ChargerTousLesArretsSaufLigne(idChoixMain), "NomArret", "IdArret");
            Utils.RemplirComboBox(cmbLigneRetraitArret, ChargerDonnees.ChargerArretsParLigne(idChoixMain), "NomArret", "IdArret");
            Utils.RemplirComboBox(cmbArretModifLigneChoixAdd, ChargerDonnees.toutesLesLignes, "NomLigne", "IdLigne"); //TODO : Ajouter un élément "Aucun" pour éviter les erreurs si aucune ligne n'est sélectionnée
            Utils.RemplirComboBox(cmbArretModifLigneChoixSuppr, ChargerDonnees.toutesLesLignes, "NomLigne", "IdLigne"); //TODO : idem
        }
        private void pnlCreerLigne_Click(object sender, EventArgs e)
        {
            isLigne = true;
            lblTitre.Text = "Création d'une ligne";

            // Afficher le formulaire de création de ligne
            Utils.AfficherUniquement(this, pnlCreation);
            lblSaisirNomHead.Text = "Saisir le nom de la nouvelle ligne";
        }

        private void pnlCreerArret_Click(object sender, EventArgs e)
        {
            isLigne = false; // Mise à jour au cas où l'utilisateur aurait créé une ligne juste avant
            lblTitre.Text = "Création d'un arrêt";

            Utils.AfficherUniquement(this, pnlCreation);

            lblSaisirNomHead.Text = "Saisir le nom du nouvel arrêt";

            Utils.RemplirComboBox(cmbChoix, ChargerDonnees.tousLesArrets, "NomArret", "IdArret");
        }

        private void pnlModifLigne_Click(object sender, EventArgs e)
        {
            isLigne = true; // On modifie une ligne
            Utils.AfficherUniquement(this, pnlModifChoix);

            lblTitre.Text = "Modification d'une ligne";
            lblModifHead.Text = "Choisir une ligne";
            lblModif.Text = "Choisissez une ligne à modifier :";

            Utils.RemplirComboBox(cmbChoix, ChargerDonnees.toutesLesLignes, "NomLigne", "IdLigne");
        }

        private void pnlModifArret_Click(object sender, EventArgs e)
        {
            isLigne = false; // // Mise à jour au cas où l'utilisateur aurait modifié une ligne juste avant
            Utils.AfficherUniquement(this, pnlModifChoix); ;

            lblTitre.Text = "Modification d'un arrêt";
            lblModifHead.Text = "Choisir un arrêt";
            lblModif.Text = "Choisissez un arrêt à modifier :";

            Utils.RemplirComboBox(cmbChoix, ChargerDonnees.tousLesArrets, "NomArret", "IdArret");
        }



        private void btnMenu_Click(object sender, EventArgs e)
        {
            if (!pnlMenuCreation.Visible)
            {
                Utils.AfficherUniquement(this, pnlMenuCreation, pnlMenuModif);
            }

            else
                this.Dispose();
        }

        private void btnValiderChoix_Click(object sender, EventArgs e)
        {
            // Vérifier qu'un élément est sélectionné
            if (cmbChoix.SelectedItem == null)
            {
                MessageBox.Show("Veuillez sélectionner un élément.", "Aucune sélection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            pnlModifChoix.Visible = false;

            // Récupérer l'ID réel de l'élément sélectionné (pas l'index)
            if (cmbChoix.SelectedValue != null)
            {
                idChoixMain = (int)cmbChoix.SelectedValue;
            }
            else
            {
                MessageBox.Show("Erreur lors de la récupération de l'ID.", "Erreur",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (isLigne)
            {
                // Récupérer les informations de la ligne choisie
                var ligneChoisie = ChargerDonnees.toutesLesLignes.FirstOrDefault(l => l.IdLigne == idChoixMain);
                if (ligneChoisie != null)
                {
                    lblTitreModifLigneChoisie.Text = $"Ligne sélectionnée : {ligneChoisie.NomLigne}";
                }
                pnlModifLigneChoisie.Visible = true;
            }
            else
            {
                // Récupérer les informations de l'arrêt choisi
                var arretChoisi = ChargerDonnees.tousLesArrets.FirstOrDefault(a => a.IdArret == idChoixMain);
                if (arretChoisi != null)
                {
                    lblTitreModifArretChoisi.Text = $"Arrêt sélectionné : {arretChoisi.NomArret}";
                }
                pnlModifArretChoisi.Visible = true;
            }
            ActualiserComboBoxes(); // Mettre à jour les ComboBox avec les données actuelles
        }

        private void pnlModifAppartenance_Click(object sender, EventArgs e)
        {
            Utils.AfficherUniquement(this, pnlArretModifLigne, pnlModifArretChoisi);
        }

        private void pnlChangerNom_Click(object sender, EventArgs e)
        {
            if (isLigne)
            {
                Utils.AfficherUniquement(this, pnlArretChangeNom, pnlModifLigneChoisie);
                lblChangeNomHead.Text = "Changer le nom de la ligne";
                lblChangeNom.Text = "Saisir le nouveau nom de la ligne :";
            }
            else
            {
                lblChangeNomHead.Text = "Changer le nom de l'arrêt";
                lblChangeNom.Text = "Saisir le nouveau nom de l'arrêt :";
                Utils.AfficherUniquement(this, pnlArretChangeNom, pnlModifArretChoisi);
            }
        }

        private void pnlSupprArret_Click(object sender, EventArgs e)
        {
            Utils.AfficherUniquement(this, pnlSupprVerif, pnlModifArretChoisi);
        }

        private void pnlMenuModifHoraire_Click(object sender, EventArgs e)
        {
            Utils.AfficherUniquement(this, pnlModifHoraire, pnlModifLigneChoisie);
        }

        private void pnlMenuAjoutArret_Click(object sender, EventArgs e)
        {
            Utils.AfficherUniquement(this, pnlLigneAjoutArret, pnlModifLigneChoisie);
        }

        private void pnlMenuRetraitArret_Click(object sender, EventArgs e)
        {
            // Vérifier que la ligne ait au moins un arrêt
            if (cmbLigneRetraitArret.Items.Count == 0)
            {
                MessageBox.Show("Cette ligne n'a pas d'arrêts à retirer.", "Aucun arrêt",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Utils.AfficherUniquement(this, pnlRetirerArret, pnlModifLigneChoisie);

        }

        private void pnlMenuLigneChangeNom_Click(object sender, EventArgs e)
        {
            if (isLigne)
            {
                Utils.AfficherUniquement(this, pnlArretChangeNom, pnlModifLigneChoisie);
                lblChangeNomHead.Text = "Changer le nom de la ligne";
                lblChangeNom.Text = "Saisir le nouveau nom de la ligne :";
            }
            else
            {
                lblChangeNomHead.Text = "Changer le nom de l'arrêt";
                lblChangeNom.Text = "Saisir le nouveau nom de l'arrêt :";
                Utils.AfficherUniquement(this, pnlArretChangeNom, pnlModifArretChoisi);
            }
        }

        private void pnlMenuSupprLigne_Click(object sender, EventArgs e)
        {

            lblTitreSupprVerif.Text = "Êtes-vous sûr de vouloir supprimer cette ligne ?";
            Utils.AfficherUniquement(this, pnlSupprVerif, pnlModifLigneChoisie);
        }
        //AjtArret
        private void btnValiderAjoutArret_Click(object sender, EventArgs e)
        {
            try
            {
                // Vérifier que l'utilisateur a bien sélectionné une ligne
                if (cmbLigneAjoutArret.SelectedItem == null)
                {
                    MessageBox.Show("Veuillez sélectionner une ligne.", "Aucune sélection",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Récupérer l'ID de la ligne sélectionnée et l'ordre de l'arrêt
                int idArret = (int)cmbLigneAjoutArret.SelectedValue;
                int ordreArret = (int)nudChoixPlace.Value;

                // Ajouter l'arrêt à la ligne
                if (ModifBDD.AjouterArretALigne(idArret, idChoixMain, ordreArret))
                {
                    MessageBox.Show("Arrêt ajouté avec succès à la ligne.", "Succès",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtSaisirNom.Clear(); // Réinitialiser le champ de saisie
                }
                else
                {
                    MessageBox.Show("Erreur lors de l'ajout de l'arrêt à la ligne.", "Erreur",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Erreur MySQL : {ex.Message}", "Erreur SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine("Erreur SQL : " + ex);
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Une erreur inattendue est survenue : {ex.Message}", "Erreur générale",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine("Erreur Générale : " + ex);
            }

            ActualiserComboBoxes();
        }

        private void btnRetirerArret_Click(object sender, EventArgs e)
        {
            try
            {
                int idArret = (int)cmbLigneRetraitArret.SelectedValue;
                ModifBDD.RetirerArretDeLigne(idArret, idChoixMain);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Une erreur inattendue est survenue : {ex.Message}", "Erreur générale",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine("Erreur: " + ex);
            }
            ActualiserComboBoxes();
            MessageBox.Show("Arrêt retiré avec succès de la ligne.");
        }

        private void nudChoixPlace_ValueChanged(object sender, EventArgs e)
        {
            //Maximum = count + 1

        }

        private void btnArretCreationValider_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSaisirNom.Text))
            {
                MessageBox.Show("Veuillez saisir un nom valide.");
                return;
            }
            if (!isLigne)
            {
                // Créer un nouvel arrêt avec le nom saisi

                Arret nouvelArret = new Arret
                {
                    NomArret = txtSaisirNom.Text.Trim()
                };
                ModifBDD.AjouterArret(nouvelArret);
            }
            else
            {
                // Créer une nouvelle ligne avec le nom saisi
                Ligne nouvelleLigne = new Ligne
                {
                    NomLigne = txtSaisirNom.Text.Trim()
                };
                ModifBDD.AjouterLigne(nouvelleLigne);
            }
            // Réinitialiser le champ de saisie
            txtSaisirNom.Clear();

            MessageBox.Show("Création réussie.", "Succès",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            ActualiserComboBoxes();

            Utils.AfficherUniquement(this, pnlMenuModif, pnlMenuCreation);
            inMenu = true; // On est de retour dans le menu principal
        }

        private void btnValidSuppr_Click(object sender, EventArgs e)
        {
            // Vérifier que l'utilisateur a bien écrit "CONFIRMER" dans le champ de texte
            if (txtSupprConfirmer.Text.Trim().ToUpper() != "CONFIRMER")
            {
                MessageBox.Show("Veuillez écrire 'CONFIRMER' pour valider la suppression.", "Erreur de confirmation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!isLigne)
            {
                // Supprimer l'arrêt
                if (ModifBDD.RetirerArret(idChoixMain))
                {
                    MessageBox.Show("Arrêt supprimé avec succès.", "Succès",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Erreur lors de la suppression de l'arrêt.", "Erreur",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // Supprimer la ligne
                if (ModifBDD.RetirerLigne(idChoixMain))
                {
                    MessageBox.Show("Ligne supprimée avec succès.", "Succès",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Erreur lors de la suppression de la ligne.", "Erreur",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            ActualiserComboBoxes();
            // Réinitialiser le champ de saisie
            txtSupprConfirmer.Clear();
            idChoixMain = 0; // Réinitialiser l'ID choisi
            Utils.AfficherUniquement(this, pnlMenuModif, pnlMenuCreation);
        }

        private void btnValiderChoixHoraire_Click(object sender, EventArgs e)
        {
            // Vérifie si aucun changement n'a été effectué
            if (!dtpHoraireDepart.Enabled && nudIntervalle.Value == 0)
            {
                MessageBox.Show("Rien n'a été changé");
                return;
            }

            try
            {
                // Met à jour l'horaire uniquement si le dtp est activé
                if (dtpHoraireDepart.Enabled)
                {
                    TimeSpan heureDepart = dtpHoraireDepart.Value.TimeOfDay;
                    ModifBDD.ModifierHoraireDepart(idChoixMain, heureDepart);
                }

                // Met à jour l'intervalle uniquement si la valeur est différente de 0
                if (nudIntervalle.Value > 0)
                {
                    int intervalle = (int)nudIntervalle.Value;
                    ModifBDD.ModifierIntervalleDepart(idChoixMain, intervalle);
                }
                MessageBox.Show("Changements effectués avec succès.");
                pnlModifHoraire.Visible = false;
            }

            catch (Exception ex)
            {
                // Gestion globale des erreurs
                MessageBox.Show($"Une erreur s'est produite : {ex.Message}");
                // Tu peux aussi logger l'erreur dans la console ou un fichier
                System.Diagnostics.Debug.WriteLine("Erreur : " + ex);
            }
        }

        private void chkNePasChanger_CheckedChanged(object sender, EventArgs e)
        {
            // Si la case est cochée, désactiver le DateTimePicker
            dtpHoraireDepart.Enabled = !chkNePasChanger.Checked;
        }

        private void btnChangeNomValider_Click(object sender, EventArgs e)
        {
            try
            {
                // Vérifier que le champ de texte n'est pas vide
                if (string.IsNullOrWhiteSpace(txtChangeNom.Text))
                {
                    MessageBox.Show("Veuillez saisir un nom valide.", "Nom invalide",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                ModifBDD.ChangerNom(idChoixMain, txtChangeNom.Text, isLigne);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Une erreur s'est produite : {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // Réinitialiser le champ de saisie
            txtChangeNom.Clear();

            ActualiserComboBoxes();
            MessageBox.Show("Nom modifié avec succès.", "Succès",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            pnlArretChangeNom.Visible = false;

        }

        private void btnDeconnecter_Click(object sender, EventArgs e)
        {
            Login.estConnecte = false;
            MessageBox.Show("Vous êtes déconnecté.", "Déconnexion",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Dispose();
        }
    }
}
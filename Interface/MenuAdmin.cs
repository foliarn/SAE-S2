using BiblioBDD;
using BiblioSysteme;
using MySql.Data.MySqlClient;
using Interface.Classes;
using System.Data;
using Services;
using Services.ServicesClasses;

namespace Interface
{

    public partial class MenuAdmin : Form
    {
        private Accueil accueil;

        private bool isLigne; // true si on modifie une ligne, false si on modifie un arrêt
        private bool inMenu; // true si on est dans le menu de création/modification, false si on est sur le menu principal
        private int idChoixMain; // Id de l'arrêt ou de la ligne choisie pour modification

        public MenuAdmin(Accueil formAccueil)
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
            Utils.CentrerControle(pnlTitre, true, false);



            // Remplir les ComboBox avec les arrêts
            Utils.RemplirComboBox(cmbChoix,
            isLigne ? Init.toutesLesLignes.Cast<object>().ToList() : Init.tousLesArrets.Cast<object>().ToList(),
            isLigne ? "NomLigne" : "NomArret",
            isLigne ? "IdLigne" : "IdArret");

            Utils.RemplirComboBox(cmbArretModifLigneChoixAdd, Init.toutesLesLignes, "NomLigne", "IdLigne"); //TODO : Ajouter un élément "Aucun" pour éviter les erreurs si aucune ligne n'est sélectionnée
            Utils.RemplirComboBox(cmbArretModifLigneChoixSuppr, Init.toutesLesLignes, "NomLigne", "IdLigne"); //TODO : idem

            // Initialiser le formulaire de profil
            this.accueil = formAccueil;
            accueil.profilForm = new ProfilForm();
            accueil.profilForm.SeDeconnecter += (s, e) => this.Close();

        }

        public MenuAdmin()
        {
            InitializeComponent();
        }

        private void pnlCreerLigne_Click(object sender, EventArgs e)
        {
            isLigne = true;
            lblTitre.Text = "Création d'une ligne";
            Utils.CentrerControle(lblTitre);

            // Afficher le formulaire de création de ligne
            Utils.AfficherUniquement(this, pnlCreation);
            lblSaisirNomHead.Text = "Saisir le nom de la nouvelle ligne";
        }

        private void pnlCreerArret_Click(object sender, EventArgs e)
        {
            isLigne = false; // Mise à jour au cas où l'utilisateur aurait créé une ligne juste avant
            lblTitre.Text = "Création d'un arrêt";
            Utils.CentrerControle(lblTitre);

            Utils.AfficherUniquement(this, pnlCreation);

            lblSaisirNomHead.Text = "Saisir le nom du nouvel arrêt";

            Utils.RemplirComboBox(cmbChoix, Init.tousLesArrets, "NomArret", "IdArret");
        }

        private void pnlModifLigne_Click(object sender, EventArgs e)
        {
            isLigne = true; // On modifie une ligne
            Utils.AfficherUniquement(this, pnlModifChoix);

            lblTitre.Text = "Modification d'une ligne";
            Utils.CentrerControle(lblTitre);

            lblModifHead.Text = "Choisir une ligne";
            lblModif.Text = "Choisissez une ligne à modifier :";

            Utils.RemplirComboBox(cmbChoix, Init.toutesLesLignes, "NomLigne", "IdLigne");
        }

        private void pnlModifArret_Click(object sender, EventArgs e)
        {
            isLigne = false; // // Mise à jour au cas où l'utilisateur aurait modifié une ligne juste avant
            Utils.AfficherUniquement(this, pnlModifChoix); ;

            lblTitre.Text = "Modification d'un arrêt";
            Utils.CentrerControle(lblTitre);

            lblModifHead.Text = "Choisir un arrêt";
            lblModif.Text = "Choisissez un arrêt à modifier :";

            Utils.RemplirComboBox(cmbChoix, Init.tousLesArrets, "NomArret", "IdArret");
        }



        private void btnMenu_Click(object sender, EventArgs e)
        {
            if (!pnlMenuCreation.Visible)
            {
                Utils.AfficherUniquement(this, pnlMenuCreation, pnlMenuModif);
                lblTitre.Text = "Que souhaitez-vous faire ?";
                Utils.CentrerControle(lblTitre);
                pnlTitre.Visible = true;
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
                var ligneChoisie = Init.toutesLesLignes.FirstOrDefault(l => l.IdLigne == idChoixMain);
                if (ligneChoisie != null)
                {
                    lblTitreModifLigneChoisie.Text = $"Ligne sélectionnée : {ligneChoisie.NomLigne}";
                }
                pnlModifLigneChoisie.Visible = true;

            }
            else
            {
                // Récupérer les informations de l'arrêt choisi
                var arretChoisi = Init.tousLesArrets.FirstOrDefault(a => a.IdArret == idChoixMain);
                if (arretChoisi != null)
                {
                    lblTitreModifArretChoisi.Text = $"Arrêt sélectionné : {arretChoisi.NomArret}";
                }
                pnlModifArretChoisi.Visible = true;
            }
            pnlTitre.Visible = false;
            //ActualiserComboBoxes(); // Mettre à jour les ComboBox avec les données actuelles
        }

        private void pnlModifAppartenance_Click(object sender, EventArgs e)
        {
            Utils.AfficherUniquement(this, pnlArretModifLigne, pnlModifArretChoisi);
            Utils.RemplirComboBoxLignesSelonArret(cmbArretModifLigneChoixAdd, idChoixMain, false);
            Utils.RemplirComboBoxLignesSelonArret(cmbArretModifLigneChoixSuppr, idChoixMain, true);
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
            Utils.RemplirComboBox(cmbLigneAjoutArret, Utils.ChargerTousLesArretsSaufLigne(idChoixMain), "NomArret", "IdArret");
            Utils.AfficherUniquement(this, pnlLigneAjoutArret, pnlModifLigneChoisie);
        }

        private void pnlMenuRetraitArret_Click(object sender, EventArgs e)
        {
            // Vérifier que la ligne ait au moins un arrêt
            Utils.RemplirComboBox(cmbLigneRetraitArret, Utils.GetArretsSeulsParLigne(idChoixMain), "NomArret", "IdArret");
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

            //ActualiserComboBoxes();
        }

        private void btnRetirerArret_Click(object sender, EventArgs e)
        {
            try
            {
                int idArret = (int)cmbLigneRetraitArret.SelectedValue;
                Ligne ligne = Init.toutesLesLignes.FirstOrDefault(l => l.IdLigne == idChoixMain);
                int ordre = ligne.Arrets.FirstOrDefault(a => a.Arret.IdArret == idArret)?.Ordre ?? -1;

                ModifBDD.RetirerArretDeLigne(idArret, idChoixMain, ordre);
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Une erreur inattendue est survenue : {ex.Message}", "Erreur générale",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine("Erreur: " + ex);
            }
            //ActualiserComboBoxes();
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
                MessageBox.Show("Veuillez saisir un nom valide.", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string nom = txtSaisirNom.Text.Trim();

            try
            {
                if (!isLigne)
                {
                    Arret nouvelArret = new Arret { NomArret = nom };

                    if (!ArretService.EstValide(nouvelArret))
                    {
                        MessageBox.Show("L'arrêt n'est pas valide.", "Erreur",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (!ArretService.AjouterArret(nouvelArret))
                    {
                        MessageBox.Show("Erreur lors de la création de l'arrêt.", "Erreur",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    LigneService.ValiderParametresLigne(0, nom.ToUpper());

                    Ligne nouvelleLigne = new Ligne
                    {
                        NomLigne = nom.ToUpper(),
                        Description = "Ligne " + nom.ToUpper()
                    };

                    if (!LigneService.AjouterLigne(nouvelleLigne))
                    {
                        MessageBox.Show("Erreur lors de la création de la ligne.", "Erreur",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                txtSaisirNom.Clear();
                MessageBox.Show("Création réussie.", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Utils.AfficherUniquement(this, pnlMenuModif, pnlMenuCreation);
                inMenu = true;
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
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
                if (ArretService.RetirerArret(idChoixMain))
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
                if (LigneService.RetirerLigne(idChoixMain))
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
            //ActualiserComboBoxes();
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

            //ActualiserComboBoxes();
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

        private void chkNePasAjouterUneLigne_CheckedChanged(object sender, EventArgs e)
        {
            if (chkNePasAjouterUneLigne.Checked)
            {
                // Si la case est cochée, désactiver le ComboBox
                cmbArretModifLigneChoixAdd.Enabled = false;
            }
            else
            {
                // Si la case n'est pas cochée, réactiver le ComboBox
                cmbArretModifLigneChoixAdd.Enabled = true;
            }
        }

        private void chkNePasRetirerLigne_CheckedChanged(object sender, EventArgs e)
        {
            if (chkNePasRetirerLigne.Checked)
            {
                // Si la case est cochée, désactiver le ComboBox
                cmbArretModifLigneChoixSuppr.Enabled = false;
            }
            else
            {
                // Si la case n'est pas cochée, réactiver le ComboBox
                cmbArretModifLigneChoixSuppr.Enabled = true;
            }
        }

        private void btnArretModifLigneValider_Click(object sender, EventArgs e)
        {
            if (chkNePasAjouterUneLigne.Checked && chkNePasRetirerLigne.Checked)
            {
                MessageBox.Show("Veuillez sélectionner au moins une action à effectuer.", "Aucune action sélectionnée",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbArretModifLigneChoixAdd.Enabled)
            {
                // Ajouter l'arrêt à la ligne sélectionnée
                int idLigne = (int)cmbArretModifLigneChoixAdd.SelectedValue;
                if (ModifBDD.AjouterArretALigne(idChoixMain, idLigne, 0)) // 0 pour ajouter à la fin
                {
                    MessageBox.Show("Arrêt ajouté à la ligne avec succès.", "Succès",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Erreur lors de l'ajout de l'arrêt à la ligne.", "Erreur",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (cmbArretModifLigneChoixSuppr.Enabled)
            {
                // Retirer l'arrêt de la ligne sélectionnée
                int idLigne = (int)cmbArretModifLigneChoixSuppr.SelectedValue;
                if (ModifBDD.RetirerArretDeLigne(idChoixMain, idLigne, 0)) // 0 pour retirer de la fin
                {
                    MessageBox.Show("Arrêt retiré de la ligne avec succès.", "Succès",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Erreur lors du retrait de l'arrêt de la ligne.", "Erreur",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAE_S2.Classes;
using BiblioSysteme;
using BiblioBDD;
using BiblioSysteme.ETA;

namespace SAE_S2
{
    public partial class PageItineraire : Form
    {
        private Accueil formAccueil;
        private Graphe grapheTransport;
        private CalculateurItineraire calculateurItineraire;
        private List<Arret> tousLesArrets;

        public PageItineraire(Accueil accueil)
        {
            InitializeComponent();
            formAccueil = accueil;

            InitialiserInterface();
            InitialiserCalculateur();
            ChargerDonneesInterface();
        }

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

            Utils.CentrerControle(pnlRecherche, false, true);
        }

        /// <summary>
        /// Initialise l'interface utilisateur
        /// </summary>
        private void InitialiserInterface()
        {
            // Centrer les panneaux
            Utils.CentrerControle(pnlRecherche, false, true);
            Utils.CentrerControle(pnlItineraire1, false, true);
            Utils.CentrerControle(pnlItineraire2, false, true);

            // Configurer les ComboBox
            cmbDepart.DisplayMember = "NomArret";
            cmbDepart.ValueMember = "IdArret";
            cmbDest.DisplayMember = "NomArret";
            cmbDest.ValueMember = "IdArret";

            // Configurer l'heure par défaut
            dtpHeure.Value = DateTime.Today.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute);

            // Masquer les panneaux d'itinéraires au début
            pnlItineraire1.Visible = false;
            pnlItineraire2.Visible = false;

            // Gérer l'événement de recherche
            btnTrouver.Click += BtnTrouver_Click;

            // Gérer le changement de l'option "partir maintenant"
            chkHeure.CheckedChanged += ChkHeure_CheckedChanged;
        }

        /// <summary>
        /// Initialise le calculateur d'itinéraire
        /// </summary>
        private void InitialiserCalculateur()
        {
            try
            {
                // Charger toutes les lignes depuis la BDD
                var toutesLesLignes = ChargerDonnees.ChargerToutesLesLignes();

                if (toutesLesLignes == null || toutesLesLignes.Count == 0)
                {
                    MessageBox.Show("Erreur : Aucune ligne trouvée dans la base de données.",
                        "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Construire le graphe de transport
                grapheTransport = new Graphe();
                grapheTransport.ConstruireGraphe(toutesLesLignes);

                // Valider le graphe
                if (!grapheTransport.ValiderGraphe())
                {
                    MessageBox.Show("Attention : Le graphe de transport contient des incohérences.",
                        "Avertissement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                // Créer le calculateur
                calculateurItineraire = new CalculateurItineraire(grapheTransport);

                System.Diagnostics.Debug.WriteLine($"Calculateur initialisé : {grapheTransport.GetStatistiques()}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'initialisation du calculateur : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"Erreur initialisation calculateur : {ex.Message}");
            }
        }

        /// <summary>
        /// Charge les données dans l'interface (ComboBox)
        /// </summary>
        private void ChargerDonneesInterface()
        {
            try
            {
                // Charger tous les arrêts
                tousLesArrets = ChargerDonnees.ChargerTousLesArrets();

                if (tousLesArrets == null || tousLesArrets.Count == 0)
                {
                    MessageBox.Show("Aucun arrêt trouvé dans la base de données.",
                        "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Remplir les ComboBox
                Utils.RemplirComboBox(cmbDepart, tousLesArrets, "NomArret", "IdArret");
                Utils.RemplirComboBox(cmbDest, tousLesArrets, "NomArret", "IdArret");

                System.Diagnostics.Debug.WriteLine($"Interface chargée : {tousLesArrets.Count} arrêts disponibles");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des données : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"Erreur chargement données : {ex.Message}");
            }
        }

        /// <summary>
        /// Gère le clic sur le bouton "Trouver un itinéraire"
        /// </summary>
        private void BtnTrouver_Click(object sender, EventArgs e)
        {
            try
            {
                // Valider la saisie
                if (!ValiderSaisie())
                    return;

                // Masquer les anciens résultats
                pnlItineraire1.Visible = false;
                pnlItineraire2.Visible = false;

                // Récupérer les paramètres de recherche
                var arretDepart = (Arret)cmbDepart.SelectedItem;
                var arretDestination = (Arret)cmbDest.SelectedItem;
                var parametres = CreerParametresRecherche();

                // Afficher un indicateur de chargement
                btnTrouver.Text = "Recherche en cours...";
                btnTrouver.Enabled = false;
                Application.DoEvents();

                // Lancer la recherche
                var itineraires = calculateurItineraire.CalculerItineraires(arretDepart, arretDestination, parametres);

                // Afficher les résultats
                AfficherResultats(itineraires);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la recherche d'itinéraire : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"Erreur recherche itinéraire : {ex.Message}");
            }
            finally
            {
                // Restaurer le bouton
                btnTrouver.Text = "Trouver un itinéraire";
                btnTrouver.Enabled = true;
            }
        }

        /// <summary>
        /// Valide la saisie utilisateur
        /// </summary>
        private bool ValiderSaisie()
        {
            if (cmbDepart.SelectedItem == null)
            {
                MessageBox.Show("Veuillez sélectionner un arrêt de départ.",
                    "Saisie incomplète", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbDepart.Focus();
                return false;
            }

            if (cmbDest.SelectedItem == null)
            {
                MessageBox.Show("Veuillez sélectionner un arrêt de destination.",
                    "Saisie incomplète", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbDest.Focus();
                return false;
            }

            var arretDepart = (Arret)cmbDepart.SelectedItem;
            var arretDestination = (Arret)cmbDest.SelectedItem;

            if (arretDepart.IdArret == arretDestination.IdArret)
            {
                MessageBox.Show("Le départ et la destination ne peuvent pas être identiques.",
                    "Saisie invalide", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (calculateurItineraire == null)
            {
                MessageBox.Show("Le système de calcul d'itinéraire n'est pas disponible.",
                    "Erreur système", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Crée les paramètres de recherche basés sur l'interface
        /// </summary>
        private ParametresRecherche CreerParametresRecherche()
        {
            TimeSpan heureRecherche;

            if (chkHeure.Checked)
            {
                // Partir maintenant
                heureRecherche = TimeSpan.FromHours(DateTime.Now.Hour).Add(TimeSpan.FromMinutes(DateTime.Now.Minute));
            }
            else
            {
                // Heure spécifique
                heureRecherche = TimeSpan.FromHours(dtpHeure.Value.Hour).Add(TimeSpan.FromMinutes(dtpHeure.Value.Minute));
            }

            return new ParametresRecherche(heureRecherche, true)
            {
                TempsCorrespondanceMin = TimeSpan.FromMinutes(3),
                NombreMaxCorrespondances = 2,
                TempsMaxRecherche = TimeSpan.FromHours(2),
                NombreMaxItineraires = 3
            };
        }

        /// <summary>
        /// Affiche les résultats de la recherche
        /// </summary>
        private void AfficherResultats(List<Itineraire> itineraires)
        {
            if (itineraires == null || itineraires.Count == 0)
            {
                MessageBox.Show("Aucun itinéraire trouvé pour ce trajet.",
                    "Aucun résultat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Afficher le premier itinéraire (plus rapide)
            if (itineraires.Count >= 1)
            {
                RemplirPanelItineraire(pnlItineraire1, itineraires[0], "Itinéraire le plus rapide");
                pnlItineraire1.Visible = true;
            }

            // Afficher le deuxième itinéraire (alternatif) si disponible
            if (itineraires.Count >= 2)
            {
                RemplirPanelItineraire(pnlItineraire2, itineraires[1], "Itinéraire alternatif");
                pnlItineraire2.Visible = true;
            }
            else
            {
                pnlItineraire2.Visible = false;
            }

            System.Diagnostics.Debug.WriteLine($"Affichage de {itineraires.Count} itinéraire(s)");
        }

        /// <summary>
        /// Remplit un panel d'itinéraire avec les données
        /// </summary>
        private void RemplirPanelItineraire(Panel panel, Itineraire itineraire, string titre)
        {
            try
            {
                var arretDepart = (Arret)cmbDepart.SelectedItem;
                var arretDestination = (Arret)cmbDest.SelectedItem;

                // Identifier les contrôles selon le panel
                string suffixe = panel.Name.EndsWith("1") ? "1" : "2";

                // Titre de l'itinéraire
                var lblItineraire = panel.Controls.Find($"lblItineraire{suffixe}", true).FirstOrDefault() as Label;
                var lblItineraireHead = panel.Controls.Find($"lblItineraire{suffixe}Head", true).FirstOrDefault() as Label;

                if (lblItineraire != null) lblItineraire.Text = titre;
                if (lblItineraireHead != null) lblItineraireHead.Text = $"Vers {arretDestination.NomArret}";

                // Lignes à prendre
                var lblLigneAPrendre = panel.Controls.Find($"lblLigneAPrendre{suffixe}", true).FirstOrDefault() as Label;
                if (lblLigneAPrendre != null)
                {
                    var lignes = itineraire.LignesUtilisees.Select(l => l.NomLigne).ToList();
                    lblLigneAPrendre.Text = lignes.Count > 0 ? string.Join(", ", lignes) : "Aucune ligne";
                }

                // Arrêt de départ
                var lblArretDepart = panel.Controls.Find($"lblArretDepart{suffixe}", true).FirstOrDefault() as Label;
                var lblArret = panel.Controls.Find($"lblArret{suffixe}", true).FirstOrDefault() as Label;

                if (lblArretDepart != null) lblArretDepart.Text = $"Arrêt {arretDepart.NomArret}";
                if (lblArret != null) lblArret.Text = $"Monter dans la {itineraire.LignesUtilisees.FirstOrDefault()?.NomLigne ?? "ligne"}";

                // Correspondance (si applicable)
                var lblChgmtLigneHead = panel.Controls.Find($"lblChgmtLigne{suffixe}Head", true).FirstOrDefault() as Label;
                var lblChgmtLigne = panel.Controls.Find($"lblChgmtLigne{suffixe}", true).FirstOrDefault() as Label;
                var pnlChgmtLigne = panel.Controls.Find($"pnlChgmtLigne{suffixe}", true).FirstOrDefault() as Panel;

                if (itineraire.NombreCorrespondances > 0 && itineraire.Etapes.Count > 1)
                {
                    var etapeCorrespondance = itineraire.Etapes.FirstOrDefault(e => e.EstCorrespondance);
                    if (etapeCorrespondance != null)
                    {
                        if (lblChgmtLigneHead != null) lblChgmtLigneHead.Text = $"Arrêt {etapeCorrespondance.ArretDepart.NomArret}";
                        if (lblChgmtLigne != null) lblChgmtLigne.Text = $"Correspondance - Attendre {etapeCorrespondance.TempsAttente?.TotalMinutes:F0} min";
                        if (pnlChgmtLigne != null) pnlChgmtLigne.Visible = true;
                    }
                }
                else
                {
                    if (lblChgmtLigneHead != null) lblChgmtLigneHead.Text = $"Arrêt {arretDestination.NomArret}";
                    if (lblChgmtLigne != null) lblChgmtLigne.Text = $"Descendre à {arretDestination.NomArret}";
                    if (pnlChgmtLigne != null) pnlChgmtLigne.Visible = true;
                }

                // Temps estimé
                var lblTempsEstimeHead = panel.Controls.Find($"lblTempsEstime{suffixe}Head", true).FirstOrDefault() as Label;
                var lblTempsEstime = panel.Controls.Find($"lblTempsEstime{suffixe}", true).FirstOrDefault() as Label;

                if (lblTempsEstimeHead != null) lblTempsEstimeHead.Text = "Temps estimé :";
                if (lblTempsEstime != null) lblTempsEstime.Text = $"{itineraire.TempsTotal.TotalMinutes:F0} minutes";

                // Horaire de départ
                var lblHoraireDepartHead = panel.Controls.Find($"lblHoraireDepart{suffixe}Head", true).FirstOrDefault() as Label;
                var lblHoraireDepart = panel.Controls.Find($"lblHoraireDepart{suffixe}", true).FirstOrDefault() as Label;

                if (lblHoraireDepartHead != null) lblHoraireDepartHead.Text = "Horaire de départ :";
                if (lblHoraireDepart != null) lblHoraireDepart.Text = $"{itineraire.HeureDepart:hh\\:mm} à {arretDepart.NomArret}";

                System.Diagnostics.Debug.WriteLine($"Panel {suffixe} rempli : {itineraire}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur remplissage panel : {ex.Message}");
                MessageBox.Show($"Erreur lors de l'affichage de l'itinéraire : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Gère le changement de l'option "partir maintenant"
        /// </summary>
        private void ChkHeure_CheckedChanged(object sender, EventArgs e)
        {
            // Afficher/masquer les contrôles d'heure selon l'option
            rdoDepart.Visible = !chkHeure.Checked;
            rdoArrive.Visible = !chkHeure.Checked;
            dtpHeure.Visible = !chkHeure.Checked;

            // Ajuster la taille du panel si nécessaire
            if (chkHeure.Checked)
            {
                pnlRecherche.Height = 250;
            }
            else
            {
                pnlRecherche.Height = 332;
            }

            Utils.CentrerControle(pnlRecherche, false, true);
        }

        private void PageItineraire_Load(object sender, EventArgs e)
        {
            // Méthode existante - peut rester vide ou contenir des initialisations supplémentaires
        }

        private void label11_Click(object sender, EventArgs e)
        {
            // Méthode existante - peut rester vide
        }

        /// <summary>
        /// Méthode pour revenir à la page d'accueil
        /// </summary>
        private void RetournerAccueil()
        {
            try
            {
                this.Hide();
                formAccueil?.Show();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur retour accueil : {ex.Message}");
            }
        }

        /// <summary>
        /// Override de la fermeture pour revenir à l'accueil
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                RetournerAccueil();
            }
            else
            {
                base.OnFormClosing(e);
            }
        }
    }
}
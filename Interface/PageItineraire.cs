//using Services;
using BiblioBDD;
using BiblioSysteme;
using Interface.Classes;
using Services;
using System.Data;

namespace Interface
{
    public partial class PageItineraire : Form
    {
        private Accueil formAccueil;
        private Arret arretDepart;
        private Arret arretDestination;
        private ParametresRecherche parametresRecherche;
        //private CalculateurItineraire calculateur;
        private List<Itineraire> itinerairesCalcules;

        // Constructeur existant (pour compatibilité)
        public PageItineraire(Accueil accueil)
        {
            InitializeComponent();
            formAccueil = accueil;
            InitialiserInterface();
        }

        // Constructeur avec transfert automatique
        public PageItineraire(Accueil accueil, Arret depart, Arret destination, ParametresRecherche parametres)
        {
            InitializeComponent();
            formAccueil = accueil;
            arretDepart = depart;
            arretDestination = destination;
            parametresRecherche = parametres;

            InitialiserInterface();

            // Lancer automatiquement la recherche si tous les paramètres sont fournis
            if (arretDepart != null && arretDestination != null && parametresRecherche != null)
            {
                ChargerItineraires();
            }
        }

        private void InitialiserInterface()
        {
            try
            {
                // Charger les données
                if (RecupDonnees.tousLesArrets == null || RecupDonnees.tousLesArrets.Count == 0)
                {
                    RecupDonnees.tousLesArrets = RecupDonnees.GetTousLesArrets();
                }

                // Remplir les comboBox
                Utils.RemplirComboBox(cmbDepart, RecupDonnees.tousLesArrets, "NomArret", "IdArret");
                Utils.RemplirComboBox(cmbDest, RecupDonnees.tousLesArrets, "NomArret", "IdArret");

                // AJOUTER L'ÉVÉNEMENT ICI
                btnTrouver.Click += BtnTrouver_Click;

                // Préremplir les champs si on a des données (TRANSFERT)
                if (arretDepart != null)
                {
                    cmbDepart.SelectedValue = arretDepart.IdArret;
                }
                if (arretDestination != null)
                {
                    cmbDest.SelectedValue = arretDestination.IdArret;
                }

                // Configurer les heures (TRANSFERT)
                if (parametresRecherche != null)
                {
                    ConfigurerHeures();
                }

                // Interface
                Utils.CentrerControle(pnlRecherche, false, true);
                pnlItineraire1.Visible = false;
                pnlItineraire2.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'initialisation : {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurerHeures()
        {
            try
            {
                if (parametresRecherche.HeureSouhaitee == TimeSpan.FromHours(DateTime.Now.Hour).Add(TimeSpan.FromMinutes(DateTime.Now.Minute)))
                {
                    chkHeure.Checked = true;
                }
                else
                {
                    chkHeure.Checked = false;
                    dtpHeure.Value = DateTime.Today.Add(parametresRecherche.HeureSouhaitee);

                    if (parametresRecherche.EstHeureDepart)
                        rdoDepart.Checked = true;
                    else
                        rdoArrive.Checked = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur ConfigurerHeures : {ex.Message}");
            }
        }

        private void BtnTrouver_Click(object sender, EventArgs e)
        {
            try
            {
                // Validation des sélections
                if (cmbDepart.SelectedItem == null || cmbDest.SelectedItem == null)
                {
                    MessageBox.Show("Veuillez sélectionner un arrêt de départ et de destination.", "Sélection requise",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Récupérer les nouveaux arrêts
                arretDepart = cmbDepart.SelectedItem as Arret;
                arretDestination = cmbDest.SelectedItem as Arret;

                if (arretDepart.IdArret == arretDestination.IdArret)
                {
                    MessageBox.Show("L'arrêt de départ et de destination doivent être différents.", "Erreur de sélection",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Mettre à jour les paramètres avec ParametresHelper
                parametresRecherche = ParametresHelper.CreerDepuisInterface(chkHeure, dtpHeure, rdoDepart, rdoArrive);

                // Lancer la recherche
                ChargerItineraires();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la recherche : {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"Erreur BtnTrouver_Click : {ex.Message}");
            }
        }

        public ParametresRecherche CreerParametresRecherche()
        {
            try
            {
                TimeSpan heureSouhaitee;
                bool estHeureDepart = true;

                if (chkHeure.Checked)
                {
                    heureSouhaitee = TimeSpan.FromHours(DateTime.Now.Hour).Add(TimeSpan.FromMinutes(DateTime.Now.Minute));
                }
                else
                {
                    heureSouhaitee = dtpHeure.Value.TimeOfDay;
                    estHeureDepart = rdoDepart.Checked;
                }

                return new ParametresRecherche(heureSouhaitee, estHeureDepart);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur création paramètres : {ex.Message}");
                return new ParametresRecherche();
            }
        }

        private async void ChargerItineraires()
        {
            try
            {
                // Masquer les panneaux pendant le calcul
                pnlItineraire1.Visible = false;
                pnlItineraire2.Visible = false;

                // Afficher un message de chargement
                btnTrouver.Text = "Calcul en cours...";
                btnTrouver.Enabled = false;

                // Calculer les itinéraires de manière asynchrone
                await Task.Run(() =>
                {
                    itinerairesCalcules = CalculateurItineraire.CalculerItineraires(arretDepart, arretDestination, parametresRecherche);
                });

                // Restaurer le bouton
                btnTrouver.Text = "Trouver un itinéraire";
                btnTrouver.Enabled = true;

                // Afficher les résultats
                AfficherResultats();
            }
            catch (Exception ex)
            {
                btnTrouver.Text = "Trouver un itinéraire";
                btnTrouver.Enabled = true;

                MessageBox.Show($"Erreur lors du calcul des itinéraires : {ex.Message}", "Erreur de calcul",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"Erreur ChargerItineraires : {ex.Message}");
            }
        }

        private void AfficherResultats()
        {
            try
            {
                if (itinerairesCalcules == null || itinerairesCalcules.Count == 0)
                {
                    MessageBox.Show("Aucun itinéraire trouvé entre ces deux arrêts.", "Aucun résultat",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Afficher le premier itinéraire (le plus rapide)
                if (itinerairesCalcules.Count >= 1)
                {
                    AfficherItineraire(itinerairesCalcules[0], 1);
                    pnlItineraire1.Visible = true;
                }

                // Afficher le deuxième itinéraire s'il existe
                if (itinerairesCalcules.Count >= 2)
                {
                    AfficherItineraire(itinerairesCalcules[1], 2);
                    pnlItineraire2.Visible = true;
                }

                System.Diagnostics.Debug.WriteLine($"Affichage de {itinerairesCalcules.Count} itinéraire(s)");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'affichage des résultats : {ex.Message}", "Erreur d'affichage",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"Erreur AfficherResultats : {ex.Message}");
            }
        }

        private void AfficherItineraire(Itineraire itineraire, int numeroItineraire)
        {
            try
            {
                // Déterminer quel panneau utiliser
                Panel panelItineraire = numeroItineraire == 1 ? pnlItineraire1 : pnlItineraire2;

                // Labels principaux
                Label lblItineraireHead = panelItineraire.Controls.Find($"lblItineraire{numeroItineraire}Head", true).FirstOrDefault() as Label;
                Label lblItineraire = panelItineraire.Controls.Find($"lblItineraire{numeroItineraire}", true).FirstOrDefault() as Label;

                // Labels des détails
                Label lblLigneAPrendre = panelItineraire.Controls.Find($"lblLigneAPrendre{numeroItineraire}", true).FirstOrDefault() as Label;
                Label lblArretDepart = panelItineraire.Controls.Find($"lblArretDepart{numeroItineraire}", true).FirstOrDefault() as Label;
                Label lblArret = panelItineraire.Controls.Find($"lblArret{numeroItineraire}", true).FirstOrDefault() as Label;
                Label lblChgmtLigneHead = panelItineraire.Controls.Find($"lblChgmtLigne{numeroItineraire}Head", true).FirstOrDefault() as Label;
                Label lblChgmtLigne = panelItineraire.Controls.Find($"lblChgmtLigne{numeroItineraire}", true).FirstOrDefault() as Label;
                Label lblTempsEstime = panelItineraire.Controls.Find($"lblTempsEstime{numeroItineraire}", true).FirstOrDefault() as Label;
                Label lblHoraireDepart = panelItineraire.Controls.Find($"lblHoraireDepart{numeroItineraire}", true).FirstOrDefault() as Label;

                // Panels des étapes
                Panel pnlPremLigne = panelItineraire.Controls.Find($"pnlPremLigne{numeroItineraire}", true).FirstOrDefault() as Panel;
                Panel pnlChgmtLigne = panelItineraire.Controls.Find($"pnlChgmtLigne{numeroItineraire}", true).FirstOrDefault() as Panel;

                if (itineraire.Etapes.Count == 0)
                {
                    if (lblItineraireHead != null) lblItineraireHead.Text = "Aucun itinéraire";
                    return;
                }

                // Titre principal
                string typeItineraire = numeroItineraire == 1 ? "Itinéraire le plus rapide" : "Itinéraire alternatif";
                if (lblItineraire != null) lblItineraire.Text = typeItineraire;
                if (lblItineraireHead != null) lblItineraireHead.Text = $"Vers {arretDestination.NomArret}";

                // Lignes utilisées
                var lignesUtilisees = itineraire.LignesUtilisees;
                string lignesText = string.Join(", ", lignesUtilisees.Select(l => l.NomLigne));
                if (lblLigneAPrendre != null) lblLigneAPrendre.Text = $"Ligne(s) : {lignesText}";

                // Première étape
                var premiereEtape = itineraire.Etapes.First();
                if (lblArretDepart != null) lblArretDepart.Text = $"Arrêt {premiereEtape.ArretDepart.NomArret}";

                if (itineraire.Etapes.Count == 1)
                {
                    // Trajet direct
                    if (lblArret != null) lblArret.Text = $"Descendre à {premiereEtape.ArretArrivee.NomArret}";
                    if (pnlChgmtLigne != null) pnlChgmtLigne.Visible = false;
                }
                else
                {
                    // Trajet avec correspondance
                    var derniereEtape = itineraire.Etapes.Last();
                    if (lblArret != null) lblArret.Text = $"Changer à {premiereEtape.ArretArrivee.NomArret}";
                    if (lblChgmtLigneHead != null) lblChgmtLigneHead.Text = $"Arrêt {premiereEtape.ArretArrivee.NomArret}";
                    if (lblChgmtLigne != null) lblChgmtLigne.Text = $"Descendre à {derniereEtape.ArretArrivee.NomArret}";
                    if (pnlChgmtLigne != null) pnlChgmtLigne.Visible = true;
                }

                // Temps estimé
                int minutes = (int)itineraire.TempsTotal.TotalMinutes;
                if (lblTempsEstime != null) lblTempsEstime.Text = $"{minutes} minutes";

                // Horaire de départ
                if (lblHoraireDepart != null) lblHoraireDepart.Text = $"{premiereEtape.HeureDepart:hh\\:mm} à {premiereEtape.ArretDepart.NomArret}";

                System.Diagnostics.Debug.WriteLine($"Itinéraire {numeroItineraire} affiché : {itineraire.Etapes.Count} étapes, {minutes} minutes");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur AfficherItineraire {numeroItineraire} : {ex.Message}");
            }
        }

        private void PageItineraire_Load(object sender, EventArgs e)
        {
            // Si on a des données, lancer automatiquement la recherche
            if (arretDepart != null && arretDestination != null && parametresRecherche != null)
            {
                // La recherche est déjà lancée dans le constructeur
            }
        }

        private void label11_Click(object sender, EventArgs e)
        {
            // Événement existant - peut rester vide ou être utilisé pour d'autres fonctionnalités
        }

        // Ajoutez cette méthode pour gérer le retour à l'accueil
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            if (formAccueil != null && !formAccueil.IsDisposed)
            {
                formAccueil.Show();
            }
        }

        // Méthode pour gérer la fermeture avec le bouton de retour (si vous l'ajoutez)
        private void BtnRetour_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void chkHeure_CheckedChanged(object sender, EventArgs e)
        {
            if (chkHeure.Checked)
            {
                pnlRecherche.Size = new Size(250, 300);

                rdoDepart.Visible = false;
                rdoArrive.Visible = false;
                dtpHeure.Visible = false;
            }

            else
            {
                pnlRecherche.Size = new Size(250, 350);

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
    }
}
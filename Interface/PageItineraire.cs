using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Interface.Classes;
using BiblioSysteme;
//using Services;
using BiblioBDD;

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

        // Nouveau constructeur avec paramètres de recherche
        public PageItineraire(Accueil accueil, Arret depart, Arret destination, ParametresRecherche parametres)
        {
            InitializeComponent();
            formAccueil = accueil;
            arretDepart = depart;
            arretDestination = destination;
            parametresRecherche = parametres;

            InitialiserInterface();
            //ChargerItineraires();
        }

        private void InitialiserInterface()
        {
            try
            {
                // Charger les données si pas déjà fait
                if (RecupDonnees.tousLesArrets == null || RecupDonnees.tousLesArrets.Count == 0)
                {
                    RecupDonnees.tousLesArrets = RecupDonnees.GetTousLesArrets();
                }

                // Remplir les comboBox
                Utils.RemplirComboBox(cmbDepart, RecupDonnees.tousLesArrets, "NomArret", "IdArret");
                Utils.RemplirComboBox(cmbDest, RecupDonnees.tousLesArrets, "NomArret", "IdArret");

                // Centrer les contrôles
                Utils.CentrerControle(pnlRecherche, false, true);
                Utils.CentrerControle(pnlItineraire1, false, true);
                Utils.CentrerControle(pnlItineraire2, false, true);

                // Initialiser le calculateur
                //calculateur = new CalculateurItineraire();

                // Préremplir les champs si on a des données
                if (arretDepart != null)
                {
                    cmbDepart.SelectedValue = arretDepart.IdArret;
                }
                if (arretDestination != null)
                {
                    cmbDest.SelectedValue = arretDestination.IdArret;
                }

                // Configurer les heures si nécessaire
                if (parametresRecherche != null)
                {
                    ConfigurerHeures();
                }

                // Ajouter l'événement du bouton recherche
                //btnTrouver.Click += BtnTrouver_Click;

                // Masquer les panneaux d'itinéraires au début
                pnlItineraire1.Visible = false;
                pnlItineraire2.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'initialisation : {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"Erreur InitialiserInterface : {ex.Message}");
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

        /*private void BtnTrouver_Click(object sender, EventArgs e)
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

                // Mettre à jour les paramètres
                parametresRecherche = CreerParametresRecherche();

                // Lancer la recherche
                ChargerItineraires();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la recherche : {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"Erreur BtnTrouver_Click : {ex.Message}");
            }
        }*/

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

        /*private async void ChargerItineraires()
        {
            try
            {
                // Masquer les panneaux pendant le calcul
                pnlItineraire1.Visible = false;
                pnlItineraire2.Visible = false;

                // Afficher un message de chargement (optionnel)
                btnTrouver.Text = "Calcul en cours...";
                btnTrouver.Enabled = false;

                // Calculer les itinéraires de manière asynchrone
                await Task.Run(() =>
                {
                    calculateur.DebugCacheArret(arretDepart.IdArret); // Pour Blum
                    calculateur.DebugCacheArret(109);
                    itinerairesCalcules = calculateur.CalculerItineraires(arretDepart, arretDestination, parametresRecherche);
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
        }*/

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
    }
}
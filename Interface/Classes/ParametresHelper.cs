using System;
using System.Windows.Forms;
using BiblioSysteme;

namespace Interface.Classes
{
    /// <summary>
    /// Classe utilitaire pour créer des ParametresRecherche depuis l'interface utilisateur
    /// Centralise la logique de création des paramètres pour éviter la duplication de code
    /// </summary>
    public static class ParametresHelper
    {
        /// <summary>
        /// Crée un objet ParametresRecherche à partir des contrôles de l'interface
        /// </summary>
        /// <param name="chkHeure">CheckBox "Partir maintenant"</param>
        /// <param name="dtpHeure">DateTimePicker pour l'heure personnalisée</param>
        /// <param name="rdoDepart">RadioButton "Heure de départ"</param>
        /// <param name="rdoArrive">RadioButton "Heure d'arrivée"</param>
        /// <returns>ParametresRecherche configuré selon les choix de l'utilisateur</returns>
        public static ParametresRecherche CreerDepuisInterface(
            CheckBox chkHeure,
            DateTimePicker dtpHeure,
            RadioButton rdoDepart,
            RadioButton rdoArrive)
        {
            try
            {
                TimeSpan heureSouhaitee;
                bool estHeureDepart = true;

                // Déterminer l'heure souhaitée
                if (chkHeure?.Checked == true)
                {
                    // L'utilisateur veut partir maintenant
                    heureSouhaitee = TimeSpan.FromHours(DateTime.Now.Hour)
                                           .Add(TimeSpan.FromMinutes(DateTime.Now.Minute));

                    // Par défaut, "maintenant" = heure de départ
                    estHeureDepart = true;

                    System.Diagnostics.Debug.WriteLine($"Paramètres : Partir maintenant à {heureSouhaitee}");
                }
                else
                {
                    // L'utilisateur a choisi une heure personnalisée
                    heureSouhaitee = dtpHeure?.Value.TimeOfDay ?? TimeSpan.Zero;

                    // Déterminer si c'est une heure de départ ou d'arrivée
                    estHeureDepart = rdoDepart?.Checked ?? true;

                    string typeHeure = estHeureDepart ? "départ" : "arrivée";
                    System.Diagnostics.Debug.WriteLine($"Paramètres : Heure de {typeHeure} à {heureSouhaitee}");
                }

                // Créer et retourner l'objet ParametresRecherche
                var parametres = new ParametresRecherche(heureSouhaitee, estHeureDepart);

                System.Diagnostics.Debug.WriteLine($"ParametresRecherche créés avec succès");
                return parametres;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur création paramètres depuis interface : {ex.Message}");

                // En cas d'erreur, retourner des paramètres par défaut (partir maintenant)
                return CreerMaintenant();
            }
        }

        /// <summary>
        /// Version simplifiée pour partir maintenant (heure actuelle)
        /// Utile pour les cas par défaut ou les raccourcis
        /// </summary>
        /// <returns>ParametresRecherche avec l'heure actuelle comme heure de départ</returns>
        public static ParametresRecherche CreerMaintenant()
        {
            try
            {
                var heureActuelle = TimeSpan.FromHours(DateTime.Now.Hour)
                                          .Add(TimeSpan.FromMinutes(DateTime.Now.Minute));

                System.Diagnostics.Debug.WriteLine($"Paramètres par défaut : Partir maintenant à {heureActuelle}");

                return new ParametresRecherche(heureActuelle, true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur création paramètres 'maintenant' : {ex.Message}");

                // Paramètres de secours
                return new ParametresRecherche();
            }
        }

        /// <summary>
        /// Valide que les contrôles de l'interface sont correctement configurés
        /// Utile pour le débogage
        /// </summary>
        /// <param name="chkHeure">CheckBox à valider</param>
        /// <param name="dtpHeure">DateTimePicker à valider</param>
        /// <param name="rdoDepart">RadioButton départ à valider</param>
        /// <param name="rdoArrive">RadioButton arrivée à valider</param>
        /// <returns>true si tous les contrôles sont valides, false sinon</returns>
        public static bool ValiderControles(
            CheckBox chkHeure,
            DateTimePicker dtpHeure,
            RadioButton rdoDepart,
            RadioButton rdoArrive)
        {
            try
            {
                // Vérifier que les contrôles ne sont pas null
                if (chkHeure == null)
                {
                    System.Diagnostics.Debug.WriteLine("Erreur validation : chkHeure est null");
                    return false;
                }

                if (dtpHeure == null)
                {
                    System.Diagnostics.Debug.WriteLine("Erreur validation : dtpHeure est null");
                    return false;
                }

                if (rdoDepart == null)
                {
                    System.Diagnostics.Debug.WriteLine("Erreur validation : rdoDepart est null");
                    return false;
                }

                if (rdoArrive == null)
                {
                    System.Diagnostics.Debug.WriteLine("Erreur validation : rdoArrive est null");
                    return false;
                }

                // Vérifier la logique des RadioButtons
                if (!chkHeure.Checked && !rdoDepart.Checked && !rdoArrive.Checked)
                {
                    System.Diagnostics.Debug.WriteLine("Avertissement : Aucun RadioButton sélectionné pour l'heure personnalisée");
                    // Pas bloquant, on peut continuer avec rdoDepart par défaut
                }

                System.Diagnostics.Debug.WriteLine("Validation des contrôles : OK");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur validation contrôles : {ex.Message}");
                return false;
            }
        }
    }
}
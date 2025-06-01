using System.ComponentModel;
using System.Data;
using BiblioBDD;
using BiblioSysteme;
using Services;

namespace Interface.Classes
{
    internal class Utils
    {
        /// <summary>
        /// Centre un contrôle horizontalement ou verticalement (ou les deux) dans son parent.
        /// <summary>
        public static void CentrerControle(Control control, bool horizontal = true, bool vertical = true)
        {
            // Permet de trouver le conteneur parent du contrôle
            Control parent = control.Parent ?? control.FindForm();
            if (parent == null) return;

            Point newLocation = control.Location;

            if (horizontal)
            {
                newLocation.X = (parent.ClientSize.Width - control.Width) / 2;
            }
            if (vertical)
            {
                newLocation.Y = (parent.ClientSize.Height - control.Height) / 2;
            }

            control.Location = newLocation;
        }

        /// <summary>
        /// Crée une liste de tous les arrêts de la base sans ceux d'une ligne spécifique. (pour une comboBox d'ajout d'arrêt)
        /// </summary>
        /// <param name="idLigne">La ligne utilisée</param>
        /// <returns>Une liste d'arrêts</returns>
        public static List<Arret> ChargerTousLesArretsSaufLigne(int idLigne)
        {
            // On récupère les arrêts de la ligne spécifiée
            List<ArretLigne> arretsDeLaLigne = RecupDonnees.GetArretsParLigne(idLigne);
            // On filtre les arrêts pour ne garder que ceux qui ne sont pas dans la ligne spécifiée
            List<Arret> arretsFiltres = Init.tousLesArrets
                .Where(a => !arretsDeLaLigne.Any(al => al.Arret.IdArret == a.IdArret))
                .ToList();
            return arretsFiltres;
        }

        /// <summary>
        /// Remplis un ComboBox avec une liste d'objets de type Type.
        /// </summary>
        /// <typeparam name="Type">Le type de source</typeparam>
        /// <param name="comboBox">La comboBox à remplir</param>
        /// <param name="liste">La liste de contenu (source)</param>
        /// <param name="displayMember">Ce qui sera affiché dans la comboBox</param>
        /// <param name="valueMember">L'index de ce qui est affiché</param>
        public static void RemplirComboBox<Type>(ComboBox comboBox, List<Type> liste, string displayMember, string valueMember)
        {
            if (comboBox == null || liste == null) return;

            // Créer une copie indépendante de la liste dans un BindingList
            var bindingList = new BindingList<Type>(new List<Type>(liste));

            // Utiliser un BindingSource pour faciliter la gestion (rafraîchissement automatique, desynchronisation, etc.)
            var bindingSource = new BindingSource();
            bindingSource.DataSource = bindingList;

            // Affecter au ComboBox
            comboBox.DataSource = bindingSource;
            comboBox.DisplayMember = displayMember;
            comboBox.ValueMember = valueMember;
        }

        /// <summary>
        /// Affiche uniquement les panels spécifiés, rend tous les autres invisibles.
        /// Ignore les panels dont le nom commence par "pnlSep".
        /// </summary>
        /// <param name="conteneur">Le conteneur (Form ou Panel) qui contient les panels</param>
        /// <param name="panelsAAfficher">Les panels à garder visibles</param>
        public static void AfficherUniquement(Control conteneur, params Panel[] panelsAAfficher)
        {
            foreach (Control ctrl in conteneur.Controls)
            {
                if (ctrl is Panel panel)
                {
                    // Ignorer les panels de séparation
                    if (panel.Name.StartsWith("pnlSep") || (panel.Name.StartsWith("pnlTitre")))
                        continue;

                    // Si le panel est dans la liste des panels à afficher, on le laisse visible
                    if (panelsAAfficher.Contains(panel))
                    {
                        panel.Visible = true;
                    }
                    else
                    {
                        panel.Visible = false;
                    }
                }
            }
        }
        /// <summary>
        /// Remplit une ComboBox avec les lignes selon qu'elles passent ou non par un arrêt (pour appartenance)
        /// </summary>
        /// <param name="comboBox">La ComboBox à remplir</param>
        /// <param name="idArret">L'arrêt de référence</param>
        /// <param name="inclureArret">True pour inclure les lignes avec cet arrêt, False pour les exclure</param>
        /// <returns>Le nombre de lignes trouvées</returns>
        public static void RemplirComboBoxLignesSelonArret(ComboBox comboBox, int idArret, bool inclureArret = true)
        {
            if (comboBox == null || idArret == 0)
            {
                System.Diagnostics.Debug.WriteLine("Erreur : ComboBox ou idArret = 0");
                return;
            }

            try
            {
                // Vérifier que les données sont chargées
                if (Init.toutesLesLignes == null || Init.toutesLesLignes.Count == 0)
                {
                    Init.toutesLesLignes = RecupDonnees.GetToutesLesLignes();
                }

                List<Ligne> lignesFiltrees;

                if (inclureArret)
                {
                    // Lignes qui passent par cet arrêt
                    lignesFiltrees = Init.toutesLesLignes
                        .Where(ligne => ligne.Arrets != null &&
                                       ligne.Arrets.Any(arretLigne => arretLigne.Arret.IdArret == idArret))
                        .ToList();
                }
                else
                {
                    // Lignes qui ne passent PAS par cet arrêt
                    lignesFiltrees = Init.toutesLesLignes
                        .Where(ligne => ligne.Arrets == null ||
                                       !ligne.Arrets.Any(arretLigne => arretLigne.Arret.IdArret == idArret))
                        .ToList();
                }

                // Utiliser la méthode existante pour remplir la ComboBox
                RemplirComboBox(comboBox, lignesFiltrees, "NomLigne", "IdLigne");
                return;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors du filtrage des lignes : {ex.Message}");
                return;
            }
        }

        /// <summary>
        /// Récupère uniquement les arrêts d'une ligne (sans ordre ni temps)
        /// </summary>
        /// <param name="idLigne">ID de la ligne</param>
        /// <returns>Liste des Arret</returns>
        public static List<Arret> GetArretsSeulsParLigne(int idLigne)
        {
            try
            {
                var arretsLigne = RecupDonnees.GetArretsParLigne(idLigne);
                return arretsLigne.Select(al => al.Arret).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la récupération des arrêts seuls : {ex.Message}");
                return new List<Arret>();
            }
        }

    }
}

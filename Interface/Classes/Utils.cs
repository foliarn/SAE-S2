using System.ComponentModel;
using System.Data;
using BiblioBDD;
using BiblioSysteme;

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
            List<Arret> arretsFiltres = RecupDonnees.tousLesArrets
                .Where(a => !arretsDeLaLigne.Any(al => al.Arret.IdArret == a.IdArret))
                .ToList();
            return arretsFiltres;
        }


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
    }
}

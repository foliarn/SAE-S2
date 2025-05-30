using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        /// Replace un contrôle à une position donnée dans son parent.
        /// <summary>
        
        public static void PositionnerControle(Control control, int x, int y)
        {
            // Permet de trouver le conteneur parent du contrôle
            Control parent = control.Parent ?? control.FindForm();
            if (parent == null) return;

            Point newLocation = control.Location;
            newLocation.X = x;
            newLocation.Y = y;
            control.Location = newLocation;
        }

        public static void RemplirComboBox<Type>(ComboBox comboBox, List<Type> liste, string displayMember, string valueMember)
        {
            if (comboBox == null || liste == null) return;

            // Créer une copie indépendante de la liste
            var bindingSource = new BindingSource();
            bindingSource.DataSource = new BindingList<Type>(new List<Type>(liste));

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

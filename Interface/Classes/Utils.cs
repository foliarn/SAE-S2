using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Data;

namespace SAE_S2.Classes
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
            comboBox.DataSource = liste;
            comboBox.DisplayMember = displayMember;
            comboBox.ValueMember = valueMember;
        }
    }
}

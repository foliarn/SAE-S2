using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Data;

namespace SAE_S2
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

        /// <summary>
        /// Génère le tableau de la ligne de bus (arrêt-horaire).
        /// <summary>
        /// 
        
        public static void genererTableauLigne(List<string> arrets, List<string> horaires, DataGridView dataGridView)
        {
            // Créer une nouvelle DataTable
            DataTable table = new DataTable("Ligne");
            // Ajouter les colonnes
            table.Columns.Add("Arrêt", typeof(string));
            table.Columns.Add("Horaire", typeof(string));
            // Ajouter les lignes (compte le nombre d'arrêts)
            for (int i = 0; i < arrets.Count; i++)
            {
                table.Rows.Add(arrets[i], horaires[i]);
            }
            // Lier la DataTable au DataGridView
            dataGridView.DataSource = table;
        }
    }
}

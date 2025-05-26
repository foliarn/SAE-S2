using BiblioSysteme;
using BiblioBDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAE_S2.Classes
{
    public static class ChargerDonnees
    {
        public static List<Arret> tousLesArrets { get; set; }
        public static List<Ligne> toutesLesLignes { get; set; }
        /// <summary>
        /// Charge tous les arrêts depuis la base de données (permet de les afficher dans les combobox ou autres contrôles).
        /// </summary>
        public static List<Arret> ChargerTousLesArrets()
        {
            // Récupère tous les arrêts depuis la BDD
            List<Arret> tousLesArrets = RecupDonnees.GetTousLesArrets();

            if (tousLesArrets.Count == 0)
            {
                MessageBox.Show("Aucun arrêt trouvé.");
                return new List<Arret>();
            }

            return tousLesArrets;
        }

        /// <summary>
        /// Charge toutes les lignes depuis la base de données (permet de les afficher dans les combobox ou autres contrôles).
        /// </summary>
        public static List<Ligne> ChargerToutesLesLignes()
        {
            // Récupère toutes les lignes depuis la BDD
            List<Ligne> toutesLesLignes = RecupDonnees.GetToutesLesLignes();
            if (toutesLesLignes.Count == 0)
            {
                MessageBox.Show("Aucune ligne trouvée.");
                return new List<Ligne>();
            }
            return toutesLesLignes;

        }
    }
}

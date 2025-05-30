using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiblioBDD;
using BiblioSysteme;

namespace Interface.Classes
{
    public static class ChargerDonnees
    {
        public static List<Arret> tousLesArrets { get; set; }
        public static List<Ligne> toutesLesLignes { get; set; }
        //public static List<Ligne> arretsLigne { get; set; } // Liste mise à jour régulièrement pour les arrêts d'une ligne spécifique


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
            toutesLesLignes = RecupDonnees.GetToutesLesLignes();
            if (toutesLesLignes.Count == 0)
            {
                MessageBox.Show("Aucune ligne trouvée.");
                return new List<Ligne>();
            }
            return toutesLesLignes;

        }

        /// <summary>
        /// Charge les arrêts d'une ligne spécifique par son ID depuis la base de données.
        /// </summary>
        /// <param name="idLigne">ID de la ligne à charger</param>"
        /// <returns>Liste des arrêts de la ligne spécifiée</returns>
        public static List<Arret> ChargerArretsParLigne(int idLigne)
        {
            // Récupère les arrêts de la ligne spécifiée depuis la BDD
            List<Arret> arrets = RecupDonnees.GetArretsParLigne(idLigne);
            return arrets;
        }

        /// <summary>
        /// Charge tous les arrêts de la BDD sauf ceux d'une ligne spécifique. (pour une comboBox d'ajout d'arrêt)
        /// </summary>
        /// <param name="idLigne"></param>
        /// <returns></returns>
        public static List<Arret> ChargerTousLesArretsSaufLigne(int idLigne)
        {
            List<Arret> tousLesArrets = ChargerTousLesArrets();
            List<Arret> arretsDeLaLigne = ChargerArretsParLigne(idLigne);

            // On récupère tous les arrêts dont l'id n'est pas dans les arrêts de la ligne
            List<Arret> arretsFiltres = tousLesArrets
                .Where(a => !arretsDeLaLigne.Any(al => al.IdArret == a.IdArret))
                .ToList();

            if (arretsFiltres.Count == 0)
            {
                MessageBox.Show("Aucun arrêt ne reste après le filtre.");
                return new List<Arret>();
            }

            return arretsFiltres;
        }

        // Méthodes pour actualiser chaque type de données
        public static void ActualiserLignes()
        {
            if (BDD.conn != null && BDD.conn.State == System.Data.ConnectionState.Open)
            {
                toutesLesLignes = RecupDonnees.GetToutesLesLignes() ?? new List<Ligne>();
            }
        }

        public static void ActualiserArrets()
        {
            if (BDD.conn != null && BDD.conn.State == System.Data.ConnectionState.Open)
            {
                tousLesArrets = RecupDonnees.GetTousLesArrets() ?? new List<Arret>();
            }
        }

        // Méthode pour tout actualiser d'un coup
        public static void ActualiserTout()
        {
            ActualiserLignes();
            ActualiserArrets();
        }
    }
}

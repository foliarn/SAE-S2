using BiblioSysteme;
using BiblioBDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class Init
    {
        // Propriétés statiques pour stocker les données récupérées (pour éviter de les charger plusieurs fois)
        public static List<Arret> tousLesArrets { get; set; }
        public static List<Ligne> toutesLesLignes { get; set; }

        public static bool Initialiser()
        {
            try
            {
                Connexion.OuvrirConnexion(); // Ouverture de la connexion à la base de données
                // Chargement des données 
                tousLesArrets = RecupDonnees.GetTousLesArrets();
                toutesLesLignes = RecupDonnees.GetToutesLesLignes();
                // Vérification que les données ont bien été chargées
                if (tousLesArrets == null || toutesLesLignes == null)
                {
                    throw new Exception("Erreur lors du chargement des données.");
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de l'initialisation : {ex.Message}");
                return false;
            }
        }

    }
}

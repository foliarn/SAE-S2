using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiblioSysteme;
using BiblioBDD;

namespace Services
{
    public class ArretService
    {
        /// <summary>
        /// Ajoute une ligne à cet arrêt avec gestion d'erreur
        /// </summary>
        /// <param name="ligne">Ligne à ajouter</param>
        /// <returns>True si l'ajout a réussi, False sinon</returns>
        public static bool AjouterLigne(Ligne ligne, Arret arret)
        {
            try
            {
                // Vérifications des paramètres
                if (ligne == null)
                {
                    throw new ArgumentNullException(nameof(ligne), "La ligne ne peut pas être null");
                }

                // Vérifier si la ligne existe déjà
                if (arret.Lignes.Contains(ligne))
                {
                    throw new InvalidOperationException($"La ligne '{ligne.NomLigne}' dessert déjà l'arrêt '{arret.NomArret}'");
                }

                // Ajout de la ligne
                arret.Lignes.Add(ligne);
                return true;
            }

            catch (ArgumentNullException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur - Paramètre null : {ex.Message}");
                return false;
            }
            catch (InvalidOperationException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur - Opération invalide : {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur inattendue lors de l'ajout de la ligne : {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Retire une ligne de cet arrêt avec gestion d'erreur
        /// </summary>
        /// <param name="ligne">Ligne à retirer</param>
        /// <returns>True si la suppression a réussi, False sinon</returns>
        public static bool RetirerLigne(Ligne ligne, Arret arret)
        {
            try
            {
                if (ligne == null)
                {
                    throw new ArgumentNullException(nameof(ligne), "La ligne ne peut pas être null");
                }

                // Tentative de suppression
                if (!arret.Lignes.Remove(ligne))
                {
                    throw new InvalidOperationException($"La ligne '{ligne.NomLigne}' ne dessert pas l'arrêt '{arret.NomArret}'");
                }

                return true;
            }
            catch (ArgumentNullException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur - Paramètre null : {ex.Message}");
                return false;
            }
            catch (InvalidOperationException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur - Opération invalide : {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur inattendue lors de la suppression de la ligne : {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Retourne les noms de toutes les lignes qui passent par cet arrêt
        /// </summary>
        /// <returns>Liste des noms de lignes, ou liste vide en cas d'erreur</returns>
        public static List<string> GetNomsLignes(Arret arret)
        {
            try
            {
                if (arret.Lignes == null || arret.Lignes.Count == 0)
                {
                    return new List<string>();
                }

                return arret.Lignes.Select(l => l.NomLigne).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la récupération des noms de lignes : {ex.Message}");
                return new List<string>();
            }
        }

        /// <summary>
        /// Retourne tous les horaires de passage à cet arrêt, combinés depuis toutes les lignes associées.
        /// </summary>
        /// <returns>Liste ordonnée des horaires</returns>
        public static List<TimeSpan> GetHorairesPassage(Arret arret)
        {
            try
            {
                var horaires = new List<TimeSpan>();

                if (arret.Lignes == null || arret.Lignes.Count == 0)
                    return horaires; // Aucune ligne => pas d'horaire

                foreach (var ligne in arret.Lignes)
                {
                    var horairesLigne = LigneService.GetHorairesDepart(ligne);
                    horaires.AddRange(horairesLigne);
                }

                // Supprimer les doublons et trier
                horaires = horaires.Distinct().OrderBy(t => t).ToList();

                return horaires;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la récupération des horaires : {ex.Message}");
                return new List<TimeSpan>();
            }
        }

        /// <summary>
        /// Valide les données de l'arrêt
        /// </summary>
        /// <returns>True si valide, False sinon</returns>
        public static bool EstValide(Arret arret)
        {
            try
            {
                return arret.IdArret > 0 &&
                       !string.IsNullOrWhiteSpace(arret.NomArret) &&
                       arret.NomArret.Length <= 30 &&
                       arret.Lignes != null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la validation : {ex.Message}");
                return false;
            }
        }

        ///// <summary>
        ///// Affiche le nom de l'arrêt dans les listes déroulantes
        ///// </summary>
        //public override string ToString(Arret arret)
        //{
        //    try
        //    {
        //        return string.IsNullOrWhiteSpace(arret.NomArret) ? $"Arrêt #{arret.IdArret}" : arret.NomArret;
        //    }
        //    catch (Exception)
        //    {
        //        return "Arrêt invalide";
        //    }
        //}

    }
}

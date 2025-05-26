// BiblioSysteme/Arret.cs
using System;
using System.Collections.Generic;
using System.Linq;
using BiblioSysteme.GestionTransport;

namespace BiblioSysteme
{
    public class Arret
    {
        // Propriétés correspondant à la table Arrets
        public int IdArret { get; set; }
        public string NomArret { get; set; }

        // Liste des lignes qui passent par cet arrêt (relation Many-to-Many)
        public List<Ligne> Lignes { get; set; }

        // Constructeurs
        public Arret()
        {
            Lignes = new List<Ligne>();
        }

        public Arret(int idArret, string nomArret)
        {
            // Validation des paramètres
            if (idArret <= 0)
            {
                throw new ArgumentException("L'ID de l'arrêt doit être positif", nameof(idArret));
            }

            if (string.IsNullOrWhiteSpace(nomArret))
            {
                throw new ArgumentException("Le nom de l'arrêt ne peut pas être vide", nameof(nomArret));
            }

            if (nomArret.Length > 30)
            {
                throw new ArgumentException("Le nom de l'arrêt ne peut pas dépasser 30 caractères", nameof(nomArret));
            }

            IdArret = idArret;
            NomArret = nomArret.Trim();
            Lignes = new List<Ligne>();
        }

        // Méthodes utiles avec gestion d'erreur

        /// <summary>
        /// Ajoute une ligne à cet arrêt avec gestion d'erreur
        /// </summary>
        /// <param name="ligne">Ligne à ajouter</param>
        /// <returns>True si l'ajout a réussi, False sinon</returns>
        public bool AjouterLigne(Ligne ligne)
        {
            try
            {
                // Vérifications des paramètres
                if (ligne == null)
                {
                    throw new ArgumentNullException(nameof(ligne), "La ligne ne peut pas être null");
                }

                // Vérifier si la ligne existe déjà
                if (Lignes.Contains(ligne))
                {
                    throw new InvalidOperationException($"La ligne '{ligne.NomLigne}' dessert déjà l'arrêt '{NomArret}'");
                }

                // Ajout de la ligne
                Lignes.Add(ligne);
                return true;
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine($"Erreur - Paramètre null : {ex.Message}");
                return false;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Erreur - Opération invalide : {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur inattendue lors de l'ajout de la ligne : {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Retire une ligne de cet arrêt avec gestion d'erreur
        /// </summary>
        /// <param name="ligne">Ligne à retirer</param>
        /// <returns>True si la suppression a réussi, False sinon</returns>
        public bool RetirerLigne(Ligne ligne)
        {
            try
            {
                if (ligne == null)
                {
                    throw new ArgumentNullException(nameof(ligne), "La ligne ne peut pas être null");
                }

                // Tentative de suppression
                if (!Lignes.Remove(ligne))
                {
                    throw new InvalidOperationException($"La ligne '{ligne.NomLigne}' ne dessert pas l'arrêt '{NomArret}'");
                }

                return true;
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine($"Erreur - Paramètre null : {ex.Message}");
                return false;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Erreur - Opération invalide : {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur inattendue lors de la suppression de la ligne : {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Retourne les noms de toutes les lignes qui passent par cet arrêt
        /// </summary>
        /// <returns>Liste des noms de lignes, ou liste vide en cas d'erreur</returns>
        public List<string> GetNomsLignes()
        {
            try
            {
                if (Lignes == null || Lignes.Count == 0)
                {
                    return new List<string>();
                }

                return Lignes.Select(l => l.NomLigne).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération des noms de lignes : {ex.Message}");
                return new List<string>();
            }
        }

        /// <summary>
        /// Retourne tous les horaires de passage à cet arrêt, combinés depuis toutes les lignes associées.
        /// </summary>
        /// <returns>Liste ordonnée des horaires</returns>
        public List<TimeSpan> GetHorairesPassage()
        {
            try
            {
                var horaires = new List<TimeSpan>();

                if (Lignes == null || Lignes.Count == 0)
                    return horaires; // Aucune ligne => pas d'horaire

                foreach (var ligne in Lignes)
                {
                    var horairesLigne = ligne.GetHorairesDepart();
                    horaires.AddRange(horairesLigne);
                }

                // Supprimer les doublons et trier
                horaires = horaires.Distinct().OrderBy(t => t).ToList();

                return horaires;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération des horaires : {ex.Message}");
                return new List<TimeSpan>();
            }
        }

        /// <summary>
        /// Valide les données de l'arrêt
        /// </summary>
        /// <returns>True si valide, False sinon</returns>
        public bool EstValide()
        {
            try
            {
                return IdArret > 0 &&
                       !string.IsNullOrWhiteSpace(NomArret) &&
                       NomArret.Length <= 30 &&
                       Lignes != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la validation : {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Affiche le nom de l'arrêt dans les listes déroulantes
        /// </summary>
        public override string ToString()
        {
            try
            {
                return string.IsNullOrWhiteSpace(NomArret) ? $"Arrêt #{IdArret}" : NomArret;
            }
            catch (Exception)
            {
                return "Arrêt invalide";
            }
        }


    }
}
// BiblioSysteme/Ligne.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace BiblioSysteme
{
    public class Ligne
    {
        // Propriétés correspondant à la table Lignes
        public int IdLigne { get; set; }
        public string NomLigne { get; set; }
        public string Description { get; set; }

        // Propriétés correspondant à la table Horaires_Lignes
        public TimeSpan PremierDepart { get; set; }
        public TimeSpan DernierDepart { get; set; }
        public int IntervalleMinutes { get; set; }
        public List<TimeSpan> TempsEntreArrets { get; set; }

        // Liste des arrêts de cette ligne dans l'ordre (relation Many-to-Many)
        public List<Arret> Arrets { get; set; }

        // Constructeurs
        public Ligne()
        {
            Arrets = new List<Arret>();
            TempsEntreArrets = new List<TimeSpan>();
        }

        public Ligne(int idLigne, string nomLigne, string description = "")
        {
            // Validation des paramètres
            if (idLigne <= 0)
            {
                throw new ArgumentException("L'ID de la ligne doit être positif", nameof(idLigne));
            }

            if (string.IsNullOrWhiteSpace(nomLigne))
            {
                throw new ArgumentException("Le nom de la ligne ne peut pas être vide", nameof(nomLigne));
            }

            if (nomLigne.Length > 3)
            {
                throw new ArgumentException("Le nom de la ligne ne peut pas dépasser 3 caractères", nameof(nomLigne));
            }

            if (!string.IsNullOrEmpty(description) && description.Length > 50)
            {
                throw new ArgumentException("La description ne peut pas dépasser 50 caractères", nameof(description));
            }

            IdLigne = idLigne;
            NomLigne = nomLigne.Trim().ToUpper();
            Description = string.IsNullOrWhiteSpace(description) ? "" : description.Trim();
            Arrets = new List<Arret>();
        }

        // Méthodes utiles avec gestion d'erreur

        /// <summary>
        /// Ajoute un arrêt à cette ligne (à la fin) avec gestion d'erreur
        /// </summary>
        /// <param name="arret">Arrêt à ajouter</param>
        /// <returns>True si l'ajout a réussi, False sinon</returns>
        public bool AjouterArret(Arret arret)
        {
            try
            {
                // Vérifications des paramètres
                if (arret == null)
                {
                    throw new ArgumentNullException(nameof(arret), "L'arrêt ne peut pas être null");
                }

                // Vérifier si l'arrêt existe déjà dans cette ligne
                if (Arrets.Contains(arret))
                {
                    throw new InvalidOperationException($"L'arrêt '{arret.NomArret}' est déjà présent dans la ligne '{NomLigne}'");
                }

                // Ajout de l'arrêt
                Arrets.Add(arret);

                // Mise à jour bidirectionnelle
                if (!arret.AjouterLigne(this))
                {
                    // Rollback si l'ajout bidirectionnel a échoué
                    Arrets.Remove(arret);
                    throw new InvalidOperationException("Échec de la mise à jour bidirectionnelle");
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
                System.Diagnostics.Debug.WriteLine($"Erreur inattendue lors de l'ajout de l'arrêt : {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Insère un arrêt à une position spécifique de la ligne
        /// </summary>
        /// <param name="index">Position où insérer l'arrêt (0 = début)</param>
        /// <param name="arret">Arrêt à insérer</param>
        /// <returns>True si l'insertion a réussi, False sinon</returns>
        public bool InsererArret(int index, Arret arret)
        {
            try
            {
                // Vérifications des paramètres
                if (arret == null)
                {
                    throw new ArgumentNullException(nameof(arret), "L'arrêt ne peut pas être null");
                }

                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "L'index ne peut pas être négatif");
                }

                if (index > Arrets.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index),
                        $"L'index ({index}) ne peut pas être supérieur au nombre d'arrêts ({Arrets.Count})");
                }

                // Vérifier si l'arrêt existe déjà dans cette ligne
                if (Arrets.Contains(arret))
                {
                    throw new InvalidOperationException($"L'arrêt '{arret.NomArret}' est déjà présent dans la ligne '{NomLigne}'");
                }

                // Insertion de l'arrêt
                Arrets.Insert(index, arret);

                // Mise à jour bidirectionnelle
                if (!arret.AjouterLigne(this))
                {
                    // Rollback si l'ajout bidirectionnel a échoué
                    Arrets.Remove(arret);
                    throw new InvalidOperationException("Échec de la mise à jour bidirectionnelle");
                }

                return true;
            }
            catch (ArgumentNullException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur - Paramètre null : {ex.Message}");
                return false;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur - Index incorrect : {ex.Message}");
                return false;
            }
            catch (InvalidOperationException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur - Opération invalide : {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur inattendue lors de l'insertion de l'arrêt : {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Retire un arrêt de cette ligne avec gestion d'erreur
        /// </summary>
        /// <param name="arret">Arrêt à retirer</param>
        /// <returns>True si la suppression a réussi, False sinon</returns>
        public bool RetirerArret(Arret arret)
        {
            try
            {
                if (arret == null)
                {
                    throw new ArgumentNullException(nameof(arret), "L'arrêt ne peut pas être null");
                }

                // Tentative de suppression
                if (!Arrets.Remove(arret))
                {
                    throw new InvalidOperationException($"L'arrêt '{arret.NomArret}' n'est pas présent dans la ligne '{NomLigne}'");
                }

                // Mise à jour bidirectionnelle
                arret.RetirerLigne(this);

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
                System.Diagnostics.Debug.WriteLine($"Erreur inattendue lors de la suppression de l'arrêt : {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Retourne la position d'un arrêt dans la ligne (commence à 1)
        /// </summary>
        /// <param name="arret">Arrêt à localiser</param>
        /// <returns>Position de l'arrêt (1-indexé), 0 si non trouvé ou erreur</returns>
        public int GetOrdreArret(Arret arret)
        {
            try
            {
                if (arret == null)
                {
                    throw new ArgumentNullException(nameof(arret), "L'arrêt ne peut pas être null");
                }

                int index = Arrets.IndexOf(arret);
                return index >= 0 ? index + 1 : 0;
            }
            catch (ArgumentNullException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur - Paramètre null : {ex.Message}");
                return 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la recherche de l'ordre : {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Génère tous les horaires de départ pour cette ligne
        /// </summary>
        /// <returns>Liste des horaires de départ, ou liste vide en cas d'erreur</returns>
        public List<TimeSpan> GetHorairesDepart()
        {
            try
            {
                var horaires = new List<TimeSpan>();

                if (IntervalleMinutes <= 0)
                {
                    throw new InvalidOperationException("L'intervalle doit être positif");
                }

                if (PremierDepart > DernierDepart)
                {
                    throw new InvalidOperationException("Le premier départ ne peut pas être après le dernier départ");
                }

                var heure = PremierDepart;
                while (heure <= DernierDepart)
                {
                    horaires.Add(heure);
                    heure = heure.Add(TimeSpan.FromMinutes(IntervalleMinutes));

                    // Sécurité pour éviter les boucles infinies
                    if (horaires.Count > 200)
                    {
                        throw new InvalidOperationException("Trop d'horaires générés, vérifiez les paramètres");
                    }
                }

                return horaires;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la génération des horaires : {ex.Message}");
                return new List<TimeSpan>();
            }
        }

        /// <summary>
        /// Génère automatiquement des temps de trajet entre chaque arrêt,
        /// avec un temps fixe (ex: 5 minutes) entre chaque paire d'arrêts.
        /// </summary>
        /// <param name="tempsParTroncon">Temps en minutes entre chaque arrêt</param>
        public void GenererTempsEntreArrets(int tempsParTroncon = 5)
        {
            if (tempsParTroncon <= 0)
                throw new ArgumentException("Le temps par tronçon doit être positif", nameof(tempsParTroncon));

            if (Arrets == null || Arrets.Count < 2)
                throw new InvalidOperationException("Impossible de générer les temps sans au moins 2 arrêts");

            TempsEntreArrets = new List<TimeSpan>();

            for (int i = 1; i < Arrets.Count; i++)
            {
                TempsEntreArrets.Add(TimeSpan.FromMinutes(tempsParTroncon));
            }
        }


        /// <summary>
        /// Calcule les horaires de passage à un arrêt spécifique de la ligne, en tenant compte du temps de trajet cumulé depuis le départ initial.
        /// </summary>
        /// <param name="arret">L'arrêt pour lequel on souhaite obtenir les horaires.</param>
        /// <returns>Une liste de TimeSpan représentant les horaires de passage à cet arrêt, ou une liste vide en cas d'erreur.</returns>
        /// <exception cref="ArgumentException">Si l'arrêt n'appartient pas à cette ligne.</exception>
        private List<TimeSpan> CalculerHorairesPourArret(Arret arret)
        {
            int index = Arrets.IndexOf(arret);
            if (index == -1)
                throw new ArgumentException("Cet arrêt n'appartient pas à cette ligne");

            // Calcule le décalage cumulé depuis le départ initial
            TimeSpan decalage = TimeSpan.Zero;

            for (int i = 0; i < index; i++)
            {
                decalage += TempsEntreArrets[i];
            }

            // Génère les horaires de base de la ligne
            var horairesBase = GetHorairesDepart();

            // Applique le décalage aux horaires pour cet arrêt
            return horairesBase.Select(h => h.Add(decalage)).ToList();
        }


        // Cache pour les horaires par arrêt afin d'éviter les recalculs ou de tout stocker
        private Dictionary<Arret, List<TimeSpan>> _horairesCache;

        /// <summary>
        /// Obtient la liste des horaires de passage pour un arrêt spécifique de cette ligne, 
        /// en tenant compte du temps de trajet cumulé depuis le départ initial.
        /// </summary>
        /// <param name="arret">L'arrêt pour lequel récupérer les horaires.</param>
        /// <returns>Une liste de TimeSpan représentant les horaires de passage à cet arrêt.</returns>
        /// <exception cref="ArgumentException">Lancée si l'arrêt n'appartient pas à cette ligne.</exception>
        public List<TimeSpan> GetHorairesPourArret(Arret arret)
        {
            // Initialisation du cache si nécessaire
            _horairesCache ??= new Dictionary<Arret, List<TimeSpan>>();

            // Si déjà calculé, on retourne le résultat mis en cache
            if (_horairesCache.TryGetValue(arret, out var horaires))
            {
                return horaires;
            }

            // Sinon, on calcule et on met en cache
            horaires = CalculerHorairesPourArret(arret);
            _horairesCache[arret] = horaires;

            return horaires;
        }

        /// <summary>
        /// Retourne les noms de tous les arrêts de cette ligne
        /// </summary>
        /// <returns>Liste des noms d'arrêts, ou liste vide en cas d'erreur</returns>
        public List<string> GetNomsArrets()
        {
            try
            {
                if (Arrets == null || Arrets.Count == 0)
                {
                    return new List<string>();
                }

                return Arrets.Select(a => a.NomArret).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la récupération des noms d'arrêts : {ex.Message}");
                return new List<string>();
            }
        }

        /// <summary>
        /// Retourne le nombre d'arrêts de cette ligne
        /// </summary>
        /// <returns>Nombre d'arrêts, 0 en cas d'erreur</returns>
        public int GetNombreArrets()
        {
            try
            {
                return Arrets?.Count ?? 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors du comptage des arrêts : {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Valide les données de la ligne
        /// </summary>
        /// <returns>True si valide, False sinon</returns>
        public bool EstValide()
        {
            try
            {
                return IdLigne > 0 &&
                       !string.IsNullOrWhiteSpace(NomLigne) &&
                       NomLigne.Length <= 3 &&
                       (string.IsNullOrEmpty(Description) || Description.Length <= 50) &&
                       IntervalleMinutes > 0 &&
                       PremierDepart <= DernierDepart &&
                       Arrets != null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la validation : {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Affiche le nom de la ligne dans les listes déroulantes
        /// </summary>
        public override string ToString()
        {
            try
            {
                return string.IsNullOrWhiteSpace(NomLigne) ? $"Ligne #{IdLigne}" : NomLigne;
            }
            catch (Exception)
            {
                return "Ligne invalide";
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using BiblioSysteme;
using BiblioBDD;

namespace Services;

/// <summary>
/// Service contenant toute la logique métier pour les lignes de transport
/// </summary>
public class LigneService
{
    /// <summary>
    /// Valide les paramètres de création d'une ligne
    /// </summary>
    /// <param name="idLigne">ID de la ligne</param>
    /// <param name="nomLigne">Nom de la ligne</param>
    /// <param name="description">Description optionnelle</param>
    /// <returns>True si valide, False sinon</returns>
    /// <exception cref="ArgumentException">Lancée si les paramètres sont invalides</exception>
    public static bool ValiderParametresLigne(int idLigne, string nomLigne, string description = "")
    {
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

        return true;
    }

    /// <summary>
    /// Ajoute un arrêt à une ligne (à la fin) avec gestion d'erreur
    /// </summary>
    /// <param name="ligne">Ligne concernée</param>
    /// <param name="arret">Arrêt à ajouter</param>
    /// <returns>True si l'ajout a réussi, False sinon</returns>
    public static bool AjouterArret(Ligne ligne, Arret arret)
    {
        try
        {
            // Vérifications des paramètres
            if (ligne == null)
            {
                throw new ArgumentNullException(nameof(ligne), "La ligne ne peut pas être null");
            }

            if (arret == null)
            {
                throw new ArgumentNullException(nameof(arret), "L'arrêt ne peut pas être null");
            }

            // Vérifier si l'arrêt existe déjà dans cette ligne
            if (ligne.Arrets.Contains(arret))
            {
                throw new InvalidOperationException($"L'arrêt '{arret.NomArret}' est déjà présent dans la ligne '{ligne.NomLigne}'");
            }

            // Ajout de l'arrêt
            ligne.Arrets.Add(arret);

            // Mise à jour bidirectionnelle
            if (!ArretService.AjouterLigne(ligne, arret))
            {
                // Rollback si l'ajout bidirectionnel a échoué
                ligne.Arrets.Remove(arret);
                throw new InvalidOperationException("Échec de la mise à jour bidirectionnelle");
            }

            // Vider le cache des horaires
            ligne.HorairesCache.Clear();

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
    /// <param name="ligne">Ligne concernée</param>
    /// <param name="index">Position où insérer l'arrêt (0 = début)</param>
    /// <param name="arret">Arrêt à insérer</param>
    /// <returns>True si l'insertion a réussi, False sinon</returns>
    public static bool InsererArret(Ligne ligne, int index, Arret arret)
    {
        try
        {
            // Vérifications des paramètres
            if (ligne == null)
            {
                throw new ArgumentNullException(nameof(ligne), "La ligne ne peut pas être null");
            }

            if (arret == null)
            {
                throw new ArgumentNullException(nameof(arret), "L'arrêt ne peut pas être null");
            }

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "L'index ne peut pas être négatif");
            }

            if (index > ligne.Arrets.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index),
                    $"L'index ({index}) ne peut pas être supérieur au nombre d'arrêts ({ligne.Arrets.Count})");
            }

            // Vérifier si l'arrêt existe déjà dans cette ligne
            if (ligne.Arrets.Contains(arret))
            {
                throw new InvalidOperationException($"L'arrêt '{arret.NomArret}' est déjà présent dans la ligne '{ligne.NomLigne}'");
            }

            // Insertion de l'arrêt
            ligne.Arrets.Insert(index, arret);

            // Mise à jour bidirectionnelle
            if (!ArretService.AjouterLigne(ligne, arret))
            {
                // Rollback si l'ajout bidirectionnel a échoué
                ligne.Arrets.Remove(arret);
                throw new InvalidOperationException("Échec de la mise à jour bidirectionnelle");
            }

            // Vider le cache des horaires
            ligne.HorairesCache.Clear();

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
    /// Retire un arrêt d'une ligne avec gestion d'erreur
    /// </summary>
    /// <param name="ligne">Ligne concernée</param>
    /// <param name="arret">Arrêt à retirer</param>
    /// <returns>True si la suppression a réussi, False sinon</returns>
    public static bool RetirerArret(Ligne ligne, Arret arret)
    {
        try
        {
            if (ligne == null)
            {
                throw new ArgumentNullException(nameof(ligne), "La ligne ne peut pas être null");
            }

            if (arret == null)
            {
                throw new ArgumentNullException(nameof(arret), "L'arrêt ne peut pas être null");
            }

            // Tentative de suppression
            if (!ligne.Arrets.Remove(arret))
            {
                throw new InvalidOperationException($"L'arrêt '{arret.NomArret}' n'est pas présent dans la ligne '{ligne.NomLigne}'");
            }

            // Mise à jour bidirectionnelle
            ArretService.RetirerLigne(ligne, arret);

            // Vider le cache des horaires
            ligne.HorairesCache.Clear();

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
    /// <param name="ligne">Ligne concernée</param>
    /// <param name="arret">Arrêt à localiser</param>
    /// <returns>Position de l'arrêt (1-indexé), 0 si non trouvé ou erreur</returns>
    public static int GetOrdreArret(Ligne ligne, Arret arret)
    {
        try
        {
            if (ligne == null)
            {
                throw new ArgumentNullException(nameof(ligne), "La ligne ne peut pas être null");
            }

            if (arret == null)
            {
                throw new ArgumentNullException(nameof(arret), "L'arrêt ne peut pas être null");
            }

            int index = ligne.Arrets.IndexOf(arret);
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
    /// Génère tous les horaires de départ pour une ligne
    /// </summary>
    /// <param name="ligne">Ligne concernée</param>
    /// <returns>Liste des horaires de départ, ou liste vide en cas d'erreur</returns>
    public static List<TimeSpan> GetHorairesDepart(Ligne ligne)
    {
        try
        {
            if (ligne == null)
            {
                throw new ArgumentNullException(nameof(ligne), "La ligne ne peut pas être null");
            }

            var horaires = new List<TimeSpan>();

            if (ligne.IntervalleMinutes <= 0)
            {
                throw new InvalidOperationException("L'intervalle doit être positif");
            }

            if (ligne.PremierDepart > ligne.DernierDepart)
            {
                throw new InvalidOperationException("Le premier départ ne peut pas être après le dernier départ");
            }

            var heure = ligne.PremierDepart;
            while (heure <= ligne.DernierDepart)
            {
                horaires.Add(heure);
                heure = heure.Add(TimeSpan.FromMinutes(ligne.IntervalleMinutes));

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
    /// Génère automatiquement des temps de trajet entre chaque arrêt
    /// </summary>
    /// <param name="ligne">Ligne concernée</param>
    /// <param name="tempsParTroncon">Temps en minutes entre chaque arrêt</param>
    public static void GenererTempsEntreArrets(Ligne ligne, int tempsParTroncon = 5)
    {
        if (ligne == null)
            throw new ArgumentNullException(nameof(ligne), "La ligne ne peut pas être null");

        if (tempsParTroncon <= 0)
            throw new ArgumentException("Le temps par tronçon doit être positif", nameof(tempsParTroncon));

        if (ligne.Arrets == null || ligne.Arrets.Count < 2)
            throw new InvalidOperationException("Impossible de générer les temps sans au moins 2 arrêts");

        ligne.TempsEntreArrets = new List<TimeSpan>();

        for (int i = 1; i < ligne.Arrets.Count; i++)
        {
            ligne.TempsEntreArrets.Add(TimeSpan.FromMinutes(tempsParTroncon));
        }

        // Vider le cache des horaires
        ligne.HorairesCache.Clear();
    }

    /// <summary>
    /// Calcule les horaires de passage à un arrêt spécifique de la ligne
    /// </summary>
    /// <param name="ligne">Ligne concernée</param>
    /// <param name="arret">L'arrêt pour lequel on souhaite obtenir les horaires</param>
    /// <returns>Une liste de TimeSpan représentant les horaires de passage à cet arrêt</returns>
    private static List<TimeSpan> CalculerHorairesPourArret(Ligne ligne, Arret arret)
    {
        int index = ligne.Arrets.IndexOf(arret);
        if (index == -1)
            throw new ArgumentException("Cet arrêt n'appartient pas à cette ligne");

        // Calcule le décalage cumulé depuis le départ initial
        TimeSpan decalage = TimeSpan.Zero;

        for (int i = 0; i < index; i++)
        {
            if (i < ligne.TempsEntreArrets.Count)
            {
                decalage += ligne.TempsEntreArrets[i];
            }
        }

        // Génère les horaires de base de la ligne
        var horairesBase = GetHorairesDepart(ligne);

        // Applique le décalage aux horaires pour cet arrêt
        return horairesBase.Select(h => h.Add(decalage)).ToList();
    }

    /// <summary>
    /// Obtient la liste des horaires de passage pour un arrêt spécifique
    /// </summary>
    /// <param name="ligne">Ligne concernée</param>
    /// <param name="arret">L'arrêt pour lequel récupérer les horaires</param>
    /// <returns>Une liste de TimeSpan représentant les horaires de passage à cet arrêt</returns>
    public static List<TimeSpan> GetHorairesPourArret(Ligne ligne, Arret arret)
    {
        if (ligne == null)
        {
            throw new ArgumentNullException(nameof(ligne), "La ligne ne peut pas être null");
        }

        if (arret == null)
        {
            throw new ArgumentNullException(nameof(arret), "L'arrêt ne peut pas être null");
        }

        // Si déjà calculé, on retourne le résultat mis en cache
        if (ligne.HorairesCache.TryGetValue(arret, out var horaires))
        {
            return horaires;
        }

        // Sinon, on calcule et on met en cache
        horaires = CalculerHorairesPourArret(ligne, arret);
        ligne.HorairesCache[arret] = horaires;

        return horaires;
    }

    /// <summary>
    /// Retourne les noms de tous les arrêts d'une ligne
    /// </summary>
    /// <param name="ligne">Ligne concernée</param>
    /// <returns>Liste des noms d'arrêts, ou liste vide en cas d'erreur</returns>
    public static List<string> GetNomsArrets(Ligne ligne)
    {
        try
        {
            if (ligne == null)
            {
                throw new ArgumentNullException(nameof(ligne), "La ligne ne peut pas être null");
            }

            if (ligne.Arrets == null || ligne.Arrets.Count == 0)
            {
                return new List<string>();
            }

            return ligne.Arrets.Select(a => a.NomArret).ToList();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur lors de la récupération des noms d'arrêts : {ex.Message}");
            return new List<string>();
        }
    }

    /// <summary>
    /// Retourne le nombre d'arrêts d'une ligne
    /// </summary>
    /// <param name="ligne">Ligne concernée</param>
    /// <returns>Nombre d'arrêts, 0 en cas d'erreur</returns>
    public static int GetNombreArrets(Ligne ligne)
    {
        try
        {
            if (ligne == null)
            {
                return 0;
            }

            return ligne.Arrets?.Count ?? 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur lors du comptage des arrêts : {ex.Message}");
            return 0;
        }
    }

    /// <summary>
    /// Valide les données d'une ligne
    /// </summary>
    /// <param name="ligne">Ligne à valider</param>
    /// <returns>True si valide, False sinon</returns>
    public static bool EstValide(Ligne ligne)
    {
        try
        {
            if (ligne == null)
            {
                return false;
            }

            return ligne.IdLigne > 0 &&
                   !string.IsNullOrWhiteSpace(ligne.NomLigne) &&
                   ligne.NomLigne.Length <= 3 &&
                   (string.IsNullOrEmpty(ligne.Description) || ligne.Description.Length <= 50) &&
                   ligne.IntervalleMinutes > 0 &&
                   ligne.PremierDepart <= ligne.DernierDepart &&
                   ligne.Arrets != null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur lors de la validation : {ex.Message}");
            return false;
        }
    }
}
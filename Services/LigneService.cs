using BiblioSysteme;
using BiblioBDD;

namespace Services;

/// <summary>
/// Service contenant toute la logique métier pour les lignes de transport
/// </summary>
public class LigneService
{
    /// <summary>
    /// Ajoute une ligne en mémoire et en base de données
    /// </summary>
    /// <param name="ligne">Ligne à ajouter</param>
    /// <returns>True si la ligne a été ajoutée avec succès, False sinon</returns>
    public static bool AjouterLigne(Ligne ligne)
    {
        if (ligne == null)
        {
            System.Diagnostics.Debug.WriteLine("Erreur : La ligne à ajouter est null.");
            return false;
        }

        int idInsere = ModifBDD.AjouterLigne(ligne);

        if (idInsere == -1)
        {
            System.Diagnostics.Debug.WriteLine("Erreur : Échec de l'ajout de la ligne en base de données.");
            return false;
        }

        ligne.IdLigne = idInsere;
        RecupDonnees.toutesLesLignes.Add(ligne);

        return true;
    }

    /// <summary>
    /// Retire une ligne de la base de données et de la mémoire
    /// </summary>
    /// <param name="idLigne">Ligne à retirer</param>
    /// <returns>True si la ligne a été retirée avec succès, False sinon</returns>
    public static bool RetirerLigne(int idLigne)
    {
        try  
        {
            var ligne = RecupDonnees.toutesLesLignes.FirstOrDefault(l => l.IdLigne == idLigne);
            if (ModifBDD.RetirerLigne(idLigne))
            {
                RecupDonnees.toutesLesLignes.Remove(ligne);
                return true;
            }
            throw new Exception("Échec du retrait de la ligne en base de données.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur lors du retrait de la ligne : {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Ajoute un arrêt à une ligne en mémoire et en base de données
    /// </summary>
    /// <param name="ligne">Ligne choisie</param>
    /// <param name="arret">Arrêt à ajouter</param>
    /// <param name="ordre">Ordre de l'arrêt dans la ligne</param>
    /// <returns>True si ça a fonctionné, false sinon</returns>
    /// <exception cref="ArgumentException"></exception>
    public static bool AjouterArretALigne(Ligne ligne, Arret arret, int ordre)
    {
        if (ligne == null || arret == null)
        {
            throw new ArgumentException("La ligne ou l'arrêt ne peut pas être null.");
        }
        if (ligne.Arrets == null)
        {
            ligne.Arrets = new List<ArretLigne>();
        }
        // Vérifier si l'arrêt existe déjà dans la ligne
        if (ligne.Arrets.Any(al => al.Arret.IdArret == arret.IdArret))
        {
            System.Diagnostics.Debug.WriteLine("Erreur : L'arrêt est déjà présent dans la ligne.");
            return false;
        }
        // Ajouter l'arrêt à la ligne
        ArretLigne arretLigne = new ArretLigne
        {
            Arret = arret,
            TempsDepart = 0 // Initialiser à 0, sera mis à jour plus tard
        };

        ligne.Arrets.Add(arretLigne);
        arretLigne.Ordre = ordre;

        // Mettre à jour la base de données
        if (!ModifBDD.AjouterArretALigne(ligne.IdLigne, arret.IdArret, ordre))
        {
            System.Diagnostics.Debug.WriteLine("Erreur : Échec de l'ajout de l'arrêt en base de données.");
            return false;
        }
        // Mettre à jour l'ordre de l'arrêt
        
        return true;
    }

    public static TimeSpan ObtenirTempsDepuisDepartInitial(Ligne ligne, Arret arret)
    {
        if (ligne?.Arrets == null || arret == null)
            return TimeSpan.Zero;

        ArretLigne arretLigne = ligne.Arrets.FirstOrDefault(al => al.Arret.IdArret == arret.IdArret);

        if (arretLigne == null)
        {
            throw new ArgumentException("L'arrêt spécifié n'appartient pas à la ligne.");
        }

        return TimeSpan.FromMinutes(arretLigne.TempsDepart);
    }

    /// <summary>
    /// Valide une ligne
    /// </summary>
    /// <param name="Ligne">La ligne à vérifier</param>
    /// <returns>True si la ligne est valide, False sinon</returns>
    public static bool EstValide(Ligne ligne)
    {
        if (ligne == null)
        {
            System.Diagnostics.Debug.WriteLine("Erreur : La ligne est null.");
            return false;
        }
        if (string.IsNullOrWhiteSpace(ligne.NomLigne) || ligne.NomLigne.Length > 3)
        {
            System.Diagnostics.Debug.WriteLine("Erreur : Le nom de la ligne est invalide.");
            return false;
        }
        if (ligne.Arrets == null || !ligne.Arrets.Any())
        {
            System.Diagnostics.Debug.WriteLine("Erreur : La ligne n'a pas d'arrêts définis.");
            return false;
        }

        // Vérifier que les horaires sont cohérents
        if (ligne.PremierDepart >= ligne.DernierDepart)
        {
            System.Diagnostics.Debug.WriteLine("Erreur : Le premier départ doit être avant le dernier départ.");
            return false;
        }
        if (ligne.IntervalleMinutes <= 0)
        {
            System.Diagnostics.Debug.WriteLine("Erreur : L'intervalle entre les départs doit être positif.");
            return false;
        }
        return true;
    }


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
}
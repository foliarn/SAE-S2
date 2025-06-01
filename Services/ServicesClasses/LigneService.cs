using BiblioSysteme;
using BiblioBDD;

namespace Services.ServicesClasses;

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
        Init.toutesLesLignes.Add(ligne);

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
            var ligne = Init.toutesLesLignes.FirstOrDefault(l => l.IdLigne == idLigne);

            // Supprimer la ligne principale
            if (!ModifBDD.RetirerLigne(idLigne))
                return false;

            Init.toutesLesLignes.Remove(ligne);
            return true;
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
            TempsDepuisDebut = 0, // Initialiser à 0, sera mis à jour plus tard
            TempsDepuisFin = 0
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

    /// <summary>
    /// Vérifie qu'un arrêt appartient à une ligne
    /// </summary>
    /// <param name="ligne">La ligne</param>
    /// <param name="arret">L'arrêt à vérifier</param>
    /// <returns>True si l'arrêt appartient à la ligne</returns>
    public static bool ContientArret(Ligne ligne, Arret arret)
    {
        if (ligne?.Arrets == null || arret == null)
            return false;

        return ligne.Arrets.Any(a => a.Arret.IdArret == arret.IdArret);
    }

    /// <summary>
    /// Obtient l'ArretLigne pour un arrêt donné dans une ligne
    /// </summary>
    /// <param name="ligne">La ligne</param>
    /// <param name="arret">L'arrêt recherché</param>
    /// <returns>ArretLigne ou null si non trouvé</returns>
    public static ArretLigne ObtenirArretLigne(Ligne ligne, Arret arret)
    {
        if (ligne?.Arrets == null || arret == null)
            return null;

        return ligne.Arrets.FirstOrDefault(a => a.Arret.IdArret == arret.IdArret);
    }

    /// <summary>
    /// Détermine le sens de circulation entre 2 arrêts d'une ligne
    /// </summary>
    /// <param name="ligne">La ligne</param>
    /// <param name="arretDepart">Arrêt de départ</param>
    /// <param name="arretArrivee">Arrêt d'arrivée</param>
    /// <returns>True si sens normal (ordre croissant), False si sens inverse</returns>
    public static bool EstSensNormal(Ligne ligne, Arret arretDepart, Arret arretArrivee)
    {
        if (ligne?.Arrets == null)
            throw new ArgumentNullException(nameof(ligne));

        var arretLigneDepart = ObtenirArretLigne(ligne, arretDepart);
        var arretLigneArrivee = ObtenirArretLigne(ligne, arretArrivee);

        if (arretLigneDepart == null || arretLigneArrivee == null)
            throw new ArgumentException("Un des arrêts n'appartient pas à cette ligne");

        // Sens normal = ordre croissant
        return arretLigneDepart.Ordre < arretLigneArrivee.Ordre;
    }

    /// <summary>
    /// Obtient toutes les lignes qui passent par un arrêt
    /// </summary>
    /// <param name="arret">L'arrêt</param>
    /// <param name="toutesLesLignes">Liste de toutes les lignes disponibles</param>
    /// <returns>Liste des lignes qui passent par cet arrêt</returns>
    public static List<Ligne> ObtenirLignesParArret(Arret arret, List<Ligne> toutesLesLignes)
    {
        if (arret == null || toutesLesLignes == null)
            return new List<Ligne>();

        return toutesLesLignes.Where(ligne => ContientArret(ligne, arret)).ToList();
    }

    /// <summary>
    /// Vérifie qu'une ligne est valide pour la recherche d'itinéraire
    /// </summary>
    /// <param name="ligne">La ligne à valider</param>
    /// <returns>True si la ligne est utilisable</returns>
    public static bool EstLigneValide(Ligne ligne)
    {
        if (ligne == null) return false;
        if (ligne.Arrets == null || ligne.Arrets.Count < 2) return false;
        if (ligne.IntervalleMinutes <= 0) return false;
        if (ligne.PremierDepart >= ligne.DernierDepart) return false;

        return true;
    }

    /// <summary>
    /// Obtient le temps total de parcours d'une ligne dans un sens
    /// </summary>
    /// <param name="ligne">La ligne</param>
    /// <param name="sensNormal">Sens de circulation</param>
    /// <returns>Temps total en minutes</returns>
    public static int ObtenirTempsTotalParcours(Ligne ligne, bool sensNormal)
    {
        if (ligne?.Arrets == null || ligne.Arrets.Count == 0)
            return 0;

        if (sensNormal)
        {
            return ligne.Arrets.Max(a => a.TempsDepuisDebut);
        }
        else
        {
            return ligne.Arrets.Max(a => a.TempsDepuisFin);
        }
    }
}
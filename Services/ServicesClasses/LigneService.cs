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
        System.Diagnostics.Debug.WriteLine(idInsere);

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

        // Mettre à jour la base de données
        if (!ModifBDD.AjouterArretALigne(ligne.IdLigne, arret.IdArret, ordre))
        {
            System.Diagnostics.Debug.WriteLine("Erreur : Échec de l'ajout de l'arrêt en base de données.");
            return false;
        }

        // Recharger la ligne depuis la base pour avoir les temps corrects
        var ligneReloadee = RecupDonnees.GetLigneParId(ligne.IdLigne);
        if (ligneReloadee != null)
        {
            ligne.Arrets = ligneReloadee.Arrets;
            ligne.PremierDepart = ligneReloadee.PremierDepart;
            ligne.DernierDepart = ligneReloadee.DernierDepart;
            ligne.IntervalleMinutes = ligneReloadee.IntervalleMinutes;
            ligne.Description = ligneReloadee.Description;
        }
        Init.toutesLesLignes = RecupDonnees.GetToutesLesLignes();

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
    /// Change le nom d'une ligne en base de données et met à jour l'objet en mémoire
    /// </summary>
    /// <param name="idLigne">ID de la ligne à modifier</param>
    /// <param name="nouveauNom">Nouveau nom de la ligne</param>
    /// <returns>True si la modification a réussi, False sinon</returns>
    public static bool ChangerNom(int idLigne, string nouveauNom)
    {
        try
        {
            // Validation des paramètres
            if (idLigne <= 0)
            {
                System.Diagnostics.Debug.WriteLine("Erreur : ID de ligne invalide");
                return false;
            }

            if (string.IsNullOrWhiteSpace(nouveauNom))
            {
                System.Diagnostics.Debug.WriteLine("Erreur : Le nouveau nom ne peut pas être vide");
                return false;
            }

            nouveauNom = nouveauNom.Trim().ToUpper(); // Appliquer la logique métier (majuscules)

            // Vérifier que la ligne existe en mémoire
            var ligne = Init.toutesLesLignes.FirstOrDefault(l => l.IdLigne == idLigne);
            if (ligne == null)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur : Ligne avec ID {idLigne} non trouvée en mémoire");
                return false;
            }

            // Vérifier qu'aucune autre ligne n'a déjà ce nom
            if (Init.toutesLesLignes.Any(l => l.IdLigne != idLigne &&
                l.NomLigne.Equals(nouveauNom, StringComparison.OrdinalIgnoreCase)))
            {
                System.Diagnostics.Debug.WriteLine("Erreur : Une ligne avec ce nom existe déjà");
                return false;
            }

            // Sauvegarder l'ancien nom pour rollback éventuel
            string ancienNom = ligne.NomLigne;

            // 1. Modifier en base de données
            if (!ModifBDD.ChangerNom(idLigne, nouveauNom, true))
            {
                System.Diagnostics.Debug.WriteLine("Erreur lors de la modification en base de données");
                return false;
            }

            // 2. Modifier en mémoire
            ligne.NomLigne = nouveauNom;

            // 3. Optionnel : Mettre à jour aussi la description si elle suit un pattern
            if (ligne.Description.StartsWith("Ligne ", StringComparison.OrdinalIgnoreCase))
            {
                ligne.Description = $"Ligne {nouveauNom}";
            }

            System.Diagnostics.Debug.WriteLine($"Nom de la ligne ID {idLigne} changé de '{ancienNom}' vers '{nouveauNom}'");
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur lors du changement de nom de la ligne : {ex.Message}");
            return false;
        }
    }

    public static bool EstLigneValide(Ligne ligne)
    {
        if (ligne == null) return false;
        if (ligne.Arrets == null || ligne.Arrets.Count < 2) return false;

        if (ligne.IntervalleMinutes < 0) return false; // Accepter 0

        // Vérifier les horaires seulement s'ils sont définis
        if (ligne.PremierDepart != TimeSpan.Zero && ligne.DernierDepart != TimeSpan.Zero)
        {
            if (ligne.PremierDepart >= ligne.DernierDepart) return false;
        }

        return true;
    }
}
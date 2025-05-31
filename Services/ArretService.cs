using BiblioSysteme;
using BiblioBDD;

namespace Services
{
    public class ArretService
    {

        /// <summary>
        /// Ajoute un arrêt en mémoire et en base de données
        /// </summary>
        /// <param name="arret">Arrêt à ajouter</param>
        /// <returns>True si l'arrêt a été ajouté avec succès, False sinon</returns>
        public static bool AjouterArret(Arret arret)
        {
            if (arret == null || string.IsNullOrWhiteSpace(arret.NomArret))
            {
                System.Diagnostics.Debug.WriteLine("Erreur : L'arrêt ou son nom est invalide.");
                return false;
            }

            int idInsere = ModifBDD.AjouterArret(arret);

            if (idInsere == -1)
            {
                System.Diagnostics.Debug.WriteLine("Erreur : Échec de l'ajout de l'arrêt en base de données.");
                return false;
            }

            arret.IdArret = idInsere;
            RecupDonnees.tousLesArrets.Add(arret);

            return true;
        }

        /// <summary>
        /// Retire un arrêt de la base de données et de la mémoire
        /// </summary>
        /// <param name="idArret">Arrêt à retirer</param>
        /// <returns>True si l'arrêt a été retiré avec succès, False sinon</returns>
        public static bool RetirerArret(int idArret)
        {             
            try
            {
                var arret = RecupDonnees.tousLesArrets.FirstOrDefault(a => a.IdArret == idArret);
                if (arret == null || arret.IdArret <= 0)
                {
                    throw new ArgumentException("L'arrêt à retirer n'est pas valide.");
                }

                if (ModifBDD.RetirerArret(arret.IdArret))
                {
                    RecupDonnees.tousLesArrets.Remove(arret);
                    return true;
                }

                throw new Exception("Échec du retrait de l'arrêt en base de données.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors du retrait de l'arrêt : {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Retourne tous les horaires de passage à cet arrêt d'une ligne spécifique à partir de l'horaire précisé.
        /// Utilise un lazy cache pour éviter de recalculer plusieurs fois les mêmes données.
        /// </summary>
        /// <param name="arret">Arrêt concerné</param>
        /// <param name="idLigne">Identifiant de la ligne</param>
        /// <param name="horaire">Horaire à partir duquel on veut les passages</param>
        /// <returns>Liste ordonnée des horaires à partir de l'horaire spécifié</returns>
        public static List<TimeSpan> GetHorairesPassage(Arret arret, Ligne ligne, TimeSpan horaire)
        {
            try
            {
                if (ligne == null)
                    throw new ArgumentNullException(nameof(ligne), "La ligne ne peut pas être null.");

                // Vérifier si on a déjà calculé les horaires pour cet arrêt
                if (ligne.HorairesCache.TryGetValue(arret, out var horaires))
                {
                    // Filtrer uniquement les horaires supérieurs ou égaux à l'horaire demandé
                    return horaires.Where(h => h >= horaire).ToList();
                }

                // Sinon, on calcule toutes les horaires pour cet arrêt/ligne
                var toutesLesHoraires = new List<TimeSpan>();
                TimeSpan tempsJusquAArret = LigneService.ObtenirTempsDepuisDepartInitial(ligne, arret);

                TimeSpan HoraireActuel = ligne.PremierDepart + tempsJusquAArret;

                while (HoraireActuel <= ligne.DernierDepart + tempsJusquAArret)
                {
                    toutesLesHoraires.Add(HoraireActuel);
                    HoraireActuel = HoraireActuel.Add(TimeSpan.FromMinutes(ligne.IntervalleMinutes));
                }

                // Mets toutes les horaires en cache (une seule fois par ligne)
                ligne.HorairesCache[arret] = toutesLesHoraires;

                // Filtre pour ne retourner que les horaires à partir de l'horaire spécifié en paramètre
                return toutesLesHoraires.Where(h => h >= horaire).ToList();
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la récupération des horaires de passage : {ex.Message}");
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
                       arret.NomArret.Length <= 30;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la validation : {ex.Message}");
                return false;
            }
        }
    }
}

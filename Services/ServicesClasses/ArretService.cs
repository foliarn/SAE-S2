using BiblioSysteme;
using BiblioBDD;

namespace Services.ServicesClasses
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
            Init.tousLesArrets.Add(arret);

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
                var arret = Init.tousLesArrets.FirstOrDefault(a => a.IdArret == idArret);
                if (arret == null || arret.IdArret <= 0)
                {
                    throw new ArgumentException("L'arrêt à retirer n'est pas valide.");
                }

                if (ModifBDD.RetirerArret(arret.IdArret))
                {
                    Init.tousLesArrets.Remove(arret);
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
        /// <param name="ligne">Ligne concernée</param>
        /// <param name="horaire">Horaire à partir duquel on veut les passages</param>
        /// <returns>Liste ordonnée des horaires à partir de l'horaire spécifié</returns>
        public static List<TimeSpan> GetHorairesPassage(Arret arret, Ligne ligne, TimeSpan horaire)
        {
            try
            {
                // Vérifier que les paramètres sont valides
                if (ligne == null)
                {
                    System.Diagnostics.Debug.WriteLine("Erreur GetHorairesPassage : ligne null");
                    return new List<TimeSpan>();
                }

                if (arret == null)
                {
                    System.Diagnostics.Debug.WriteLine("Erreur GetHorairesPassage : arrêt null");
                    return new List<TimeSpan>();
                }

                // Vérifier si on a déjà calculé les horaires pour cet arrêt
                if (ligne.HorairesCache.TryGetValue(arret, out var horaires))
                {
                    // Filtrer uniquement les horaires supérieurs ou égaux à l'horaire demandé
                    var horairesFiltrés = horaires.Where(h => h >= horaire).ToList();
                    //System.Diagnostics.Debug.WriteLine($"Cache hit : {horairesFiltrés.Count} horaires trouvés pour {arret.NomArret} sur ligne {ligne.NomLigne}");
                    return horairesFiltrés;
                }

                // Sinon, on calcule toutes les horaires pour cet arrêt/ligne
                var toutesLesHoraires = new List<TimeSpan>();

                // Vérifier que l'arrêt fait partie de la ligne
                TimeSpan tempsJusquAArret;
                try
                {
                    tempsJusquAArret = LigneService.ObtenirTempsDepuisDepartInitial(ligne, arret);
                }
                catch (ArgumentException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erreur : L'arrêt {arret.NomArret} n'appartient pas à la ligne {ligne.NomLigne}");
                    return new List<TimeSpan>();
                }

                // Calculer tous les horaires de passage
                TimeSpan horaireActuel = ligne.PremierDepart.Add(tempsJusquAArret);
                TimeSpan horaireLimite = ligne.DernierDepart.Add(tempsJusquAArret);

                while (horaireActuel <= horaireLimite)
                {
                    toutesLesHoraires.Add(horaireActuel);
                    horaireActuel = horaireActuel.Add(TimeSpan.FromMinutes(ligne.IntervalleMinutes));
                }

                // Mettre en cache toutes les horaires (une seule fois par ligne)
                ligne.HorairesCache[arret] = toutesLesHoraires;

                // Filtrer pour ne retourner que les horaires à partir de l'horaire spécifié
                var resultats = toutesLesHoraires.Where(h => h >= horaire).ToList();

                // System.Diagnostics.Debug.WriteLine($"Calculé : {resultats.Count} horaires pour {arret.NomArret} sur ligne {ligne.NomLigne} à partir de {horaire}");
                return resultats;
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
            if (arret == null)
            {
                throw new ArgumentException("L'arrêt ne peut pas être null", nameof(arret));
            }

            if (string.IsNullOrWhiteSpace(arret.NomArret))
            {
                throw new ArgumentException("Le nom de l'arrêt ne peut pas être vide", nameof(arret.NomArret));
            }

            if (arret.NomArret.Length > 30)
            {
                throw new ArgumentException("Le nom de l'arrêt ne peut pas dépasser 30 caractères", nameof(arret.NomArret));
            }

            if (Init.tousLesArrets?.Any(a => a.NomArret.Equals(arret.NomArret, StringComparison.OrdinalIgnoreCase)) == true)
            {
                throw new ArgumentException("Un arrêt avec ce nom existe déjà.", nameof(arret.NomArret));
            }

            return true;
        }
    }
}

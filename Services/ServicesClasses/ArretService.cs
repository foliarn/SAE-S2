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
        /// NOUVEAU : Cache bidirectionnel complet
        /// </summary>
        /// <param name="arret">Arrêt concerné</param>
        /// <param name="ligne">Ligne concernée</param>
        /// <param name="horaire">Horaire à partir duquel on veut les passages</param>
        /// <param name="sensNormal">Sens de circulation (true = normal, false = inverse)</param>
        /// <returns>Liste ordonnée des horaires à partir de l'horaire spécifié</returns>
        public static List<TimeSpan> GetHorairesPassage(Arret arret, Ligne ligne, TimeSpan horaire, bool sensNormal = true)
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

                // NOUVEAU : Cache bidirectionnel avec clé incluant le sens
                string cleCache = $"{arret.IdArret}_{sensNormal}";

                // Créer un dictionnaire de cache bidirectionnel si il n'existe pas
                if (ligne.HorairesCache == null)
                {
                    ligne.HorairesCache = new Dictionary<Arret, List<TimeSpan>>();
                }

                // Vérifier si on a déjà calculé les horaires pour cet arrêt dans ce sens
                // On utilise une astuce : stocker dans le cache avec une clé composite
                var arretCacheKey = new Arret(arret.IdArret * 1000 + (sensNormal ? 1 : 0), $"{arret.NomArret}_{(sensNormal ? "N" : "I")}");

                if (ligne.HorairesCache.TryGetValue(arretCacheKey, out var horairesCache))
                {
                    var horairesFiltrés = horairesCache.Where(h => h >= horaire).ToList();
                    return horairesFiltrés;
                }


                // Calculer tous les horaires pour cet arrêt/ligne dans le sens spécifié
                var toutesLesHoraires = CalculerHorairesComplets(arret, ligne, sensNormal);

                // Mettre en cache toutes les horaires avec la clé bidirectionnelle
                ligne.HorairesCache[arretCacheKey] = toutesLesHoraires;

                // Filtrer pour ne retourner que les horaires à partir de l'horaire spécifié
                var resultats = toutesLesHoraires.Where(h => h >= horaire).ToList();

                return resultats;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la récupération des horaires de passage : {ex.Message}");
                return new List<TimeSpan>();
            }
        }

        /// <summary>
        /// NOUVEAU : Calcule tous les horaires de passage pour un arrêt/ligne/sens
        /// </summary>
        /// <param name="arret">Arrêt concerné</param>
        /// <param name="ligne">Ligne concernée</param>
        /// <param name="sensNormal">Sens de circulation</param>
        /// <returns>Liste complète des horaires</returns>
        private static List<TimeSpan> CalculerHorairesComplets(Arret arret, Ligne ligne, bool sensNormal)
        {
            var toutesLesHoraires = new List<TimeSpan>();

            try
            {
                // Vérifier que l'arrêt fait partie de la ligne et obtenir le temps jusqu'à l'arrêt
                TimeSpan tempsJusquAArret;
                try
                {
                    tempsJusquAArret = LigneService.ObtenirTempsDepuisDepartInitial(ligne, arret, sensNormal);
                }
                catch (ArgumentException)
                {
                    System.Diagnostics.Debug.WriteLine($"Erreur : L'arrêt {arret.NomArret} n'appartient pas à la ligne {ligne.NomLigne}");
                    return toutesLesHoraires;
                }

                // Calculer tous les horaires de passage selon le sens
                TimeSpan horaireDepart, horaireLimite;

                if (sensNormal)
                {
                    // Sens normal : utiliser les horaires de la ligne tels quels
                    horaireDepart = ligne.PremierDepart;
                    horaireLimite = ligne.DernierDepart;
                }
                else
                {
                    // Sens inverse : même logique pour l'instant
                    // TODO : Dans une version avancée, on pourrait avoir des horaires différents selon le sens
                    horaireDepart = ligne.PremierDepart;
                    horaireLimite = ligne.DernierDepart;
                }

                TimeSpan horaireActuel = horaireDepart.Add(tempsJusquAArret);
                TimeSpan horaireLimiteArret = horaireLimite.Add(tempsJusquAArret);

                while (horaireActuel <= horaireLimiteArret)
                {
                    toutesLesHoraires.Add(horaireActuel);
                    horaireActuel = horaireActuel.Add(TimeSpan.FromMinutes(ligne.IntervalleMinutes));
                }

                System.Diagnostics.Debug.WriteLine($"Calculé {toutesLesHoraires.Count} horaires pour {arret.NomArret} sur ligne {ligne.NomLigne} (sens: {(sensNormal ? "normal" : "inverse")})");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur calcul horaires complets : {ex.Message}");
            }

            return toutesLesHoraires;
        }

        /// <summary>
        /// NOUVEAU : Vide le cache bidirectionnel pour une ligne
        /// </summary>
        /// <param name="ligne">Ligne dont il faut vider le cache</param>
        public static void ViderCacheLigne(Ligne ligne)
        {
            if (ligne?.HorairesCache != null)
            {
                ligne.HorairesCache.Clear();
                System.Diagnostics.Debug.WriteLine($"Cache vidé pour la ligne {ligne.NomLigne}");
            }
        }

        /// <summary>
        /// NOUVEAU : Vide tous les caches de toutes les lignes
        /// </summary>
        public static void ViderTousLesCaches()
        {
            if (Init.toutesLesLignes != null)
            {
                foreach (var ligne in Init.toutesLesLignes)
                {
                    ViderCacheLigne(ligne);
                }
                System.Diagnostics.Debug.WriteLine("Tous les caches horaires ont été vidés");
            }
        }
        public static int ObtenirTempsSelon(ArretLigne arretLigne, bool sensNormal)
        {
            return sensNormal ? arretLigne.TempsDepuisDebut : arretLigne.TempsDepuisFin;
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
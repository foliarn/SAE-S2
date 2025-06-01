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
        /// Trouve un arrêt par son ID dans les lignes
        /// </summary>
        public static Arret TrouverArretParId(int idArret, List<Ligne> lignes)
        {
            foreach (var ligne in lignes)
            {
                if (ligne.Arrets != null)
                {
                    var arretTrouve = ligne.Arrets.FirstOrDefault(a => a.Arret.IdArret == idArret);
                    if (arretTrouve != null)
                        return arretTrouve.Arret;
                }
            }
            return null;
        }

        /// <summary>
        /// Obtient le temps depuis le début selon le sens de circulation
        /// </summary>
        /// <param name="arretLigne">L'arrêt dans la ligne</param>
        /// <param name="sensNormal">true = sens normal, false = sens inverse</param>
        /// <returns>Temps en minutes depuis le terminus de départ</returns>
        public static int ObtenirTempsDepuisDebut(ArretLigne arretLigne, bool sensNormal)
        {
            if (arretLigne == null)
                throw new ArgumentNullException(nameof(arretLigne));

            return sensNormal ? arretLigne.TempsDepuisDebut : arretLigne.TempsDepuisFin;
        }

        /// <summary>
        /// Calcule le temps de trajet entre 2 arrêts d'une même ligne
        /// </summary>
        /// <param name="ligne">La ligne</param>
        /// <param name="arretDepart">Arrêt de départ</param>
        /// <param name="arretArrivee">Arrêt d'arrivée</param>
        /// <param name="sensNormal">Sens de circulation</param>
        /// <returns>Temps de trajet en minutes</returns>
        public static int CalculerTempsTrajet(Ligne ligne, Arret arretDepart, Arret arretArrivee, bool sensNormal)
        {
            if (ligne?.Arrets == null)
                throw new ArgumentNullException(nameof(ligne));

            var arretLigneDepart = ligne.Arrets.FirstOrDefault(a => a.Arret.IdArret == arretDepart.IdArret);
            var arretLigneArrivee = ligne.Arrets.FirstOrDefault(a => a.Arret.IdArret == arretArrivee.IdArret);

            if (arretLigneDepart == null || arretLigneArrivee == null)
                throw new ArgumentException("Un des arrêts n'appartient pas à cette ligne");

            int tempsDepart = ObtenirTempsDepuisDebut(arretLigneDepart, sensNormal);
            int tempsArrivee = ObtenirTempsDepuisDebut(arretLigneArrivee, sensNormal);

            return Math.Abs(tempsArrivee - tempsDepart);
        }

        /// <summary>
        /// Obtient tous les horaires de passage d'une ligne à un arrêt
        /// </summary>
        /// <param name="ligne">La ligne</param>
        /// <param name="arret">L'arrêt</param>
        /// <param name="sensNormal">Sens de circulation</param>
        /// <returns>Liste de tous les horaires de passage dans la journée</returns>
        public static List<TimeSpan> ObtenirHorairesPassage(Ligne ligne, Arret arret, bool sensNormal)
        {
            var horaires = new List<TimeSpan>();

            if (ligne?.Arrets == null)
                return horaires;

            // Trouver l'arrêt dans la ligne
            var arretLigne = ligne.Arrets.FirstOrDefault(a => a.Arret.IdArret == arret.IdArret);
            if (arretLigne == null)
                return horaires;

            // Temps depuis le début selon le sens
            int tempsDepuisDebut = ObtenirTempsDepuisDebut(arretLigne, sensNormal);

            // Calculer tous les horaires de passage
            TimeSpan horaireActuel = ligne.PremierDepart.Add(TimeSpan.FromMinutes(tempsDepuisDebut));
            TimeSpan horaireLimite = ligne.DernierDepart.Add(TimeSpan.FromMinutes(tempsDepuisDebut));

            while (horaireActuel <= horaireLimite)
            {
                horaires.Add(horaireActuel);
                horaireActuel = horaireActuel.Add(TimeSpan.FromMinutes(ligne.IntervalleMinutes));
            }

            return horaires;
        }

        /// <summary>
        /// Trouve le prochain horaire de passage après une heure donnée
        /// </summary>
        /// <param name="ligne">La ligne</param>
        /// <param name="arret">L'arrêt</param>
        /// <param name="apresHeure">Heure après laquelle chercher</param>
        /// <param name="sensNormal">Sens de circulation</param>
        /// <returns>Prochain horaire, ou TimeSpan.Zero si aucun service</returns>
        public static TimeSpan TrouverProchainHoraire(Ligne ligne, Arret arret, TimeSpan apresHeure, bool sensNormal)
        {
            var tousLesHoraires = ObtenirHorairesPassage(ligne, arret, sensNormal);

            return tousLesHoraires.FirstOrDefault(h => h > apresHeure);
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
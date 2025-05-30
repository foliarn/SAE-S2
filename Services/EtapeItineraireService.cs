using System;
using System.Collections.Generic;
using System.Linq;
using BiblioSysteme;

namespace Services
{
    /// <summary>
    /// Méthodes utilitaires pour la classe EtapeItineraire
    /// </summary>
    public static class EtapeItineraireUtils
    {
        /// <summary>
        /// Calcule le nombre d'arrêts traversés pendant cette étape
        /// </summary>
        public static int CalculerNombreArrets(EtapeItineraire etape)
        {
            try
            {
                if (etape.LigneUtilisee?.Arrets == null)
                    return 0;

                int indexDepart = etape.LigneUtilisee.Arrets.FindIndex(a => a.IdArret == etape.ArretDepart.IdArret);
                int indexArrivee = etape.LigneUtilisee.Arrets.FindIndex(a => a.IdArret == etape.ArretArrivee.IdArret);

                if (indexDepart == -1 || indexArrivee == -1)
                    return 0;

                return Math.Abs(indexArrivee - indexDepart);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur calcul nombre d'arrêts : {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Vérifie si cette étape est valide
        /// </summary>
        public static bool EstValide(EtapeItineraire etape)
        {
            try
            {
                // Vérifications de base
                if (etape.ArretDepart == null || etape.ArretArrivee == null || etape.LigneUtilisee == null)
                    return false;

                if (etape.ArretDepart.IdArret == etape.ArretArrivee.IdArret)
                    return false;

                if (etape.HeureDepart >= etape.HeureArrivee)
                    return false;

                // Vérifier que les arrêts appartiennent à la ligne
                if (!etape.LigneUtilisee.Arrets.Any(a => a.IdArret == etape.ArretDepart.IdArret))
                    return false;

                if (!etape.LigneUtilisee.Arrets.Any(a => a.IdArret == etape.ArretArrivee.IdArret))
                    return false;

                // Vérifier l'ordre des arrêts sur la ligne
                int indexDepart = etape.LigneUtilisee.Arrets.FindIndex(a => a.IdArret == etape.ArretDepart.IdArret);
                int indexArrivee = etape.LigneUtilisee.Arrets.FindIndex(a => a.IdArret == etape.ArretArrivee.IdArret);

                if (indexDepart == -1 || indexArrivee == -1 || indexDepart == indexArrivee)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur validation étape : {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Vérifie si cette étape peut être connectée à une autre étape
        /// </summary>
        public static bool PeutSeConnecterA(EtapeItineraire etape, EtapeItineraire etapeSuivante, TimeSpan tempsCorrespondanceMin)
        {
            try
            {
                if (etapeSuivante == null)
                    return false;

                // L'arrêt d'arrivée de cette étape doit être l'arrêt de départ de la suivante
                if (etape.ArretArrivee.IdArret != etapeSuivante.ArretDepart.IdArret)
                    return false;

                // Il faut assez de temps pour la correspondance
                var tempsDisponible = etapeSuivante.HeureDepart - etape.HeureArrivee;
                return tempsDisponible >= tempsCorrespondanceMin;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur vérification connexion : {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Calcule le temps d'attente nécessaire avant cette étape
        /// TODO : voir si on peut l'enlever (3min entre chaque etape ici)
        /// </summary>
        public static TimeSpan CalculerTempsAttente(EtapeItineraire etape, TimeSpan heureArriveeEtapePrecedente)
        {
            try
            {
                if (etape.HeureDepart <= heureArriveeEtapePrecedente)
                    return TimeSpan.Zero;

                return etape.HeureDepart - heureArriveeEtapePrecedente;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur calcul temps d'attente : {ex.Message}");
                return TimeSpan.Zero;
            }
        }

        /// <summary>
        /// Retourne une description textuelle de l'étape
        /// 
        /// </summary>
        public static string GetDescription(EtapeItineraire etape)
        {
            try
            {
                var description = $"Ligne {etape.LigneUtilisee?.NomLigne ?? "?"} : " +
                                $"{etape.ArretDepart?.NomArret ?? "?"} → {etape.ArretArrivee?.NomArret ?? "?"} " +
                                $"({etape.HeureDepart:hh\\:mm} - {etape.HeureArrivee:hh\\:mm})";

                if (etape.EstCorrespondance && etape.TempsAttente.HasValue)
                {
                    description += $" [Correspondance - Attente: {etape.TempsAttente.Value.TotalMinutes:F0}min]";
                }

                return description;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur génération description : {ex.Message}");
                return "Étape invalide";
            }
        }

        /// <summary>
        /// Retourne une description courte de l'étape
        /// </summary>
        public static string GetDescriptionCourte(EtapeItineraire etape)
        {
            try
            {
                return $"{etape.LigneUtilisee?.NomLigne ?? "?"}: {etape.ArretDepart?.NomArret ?? "?"} → {etape.ArretArrivee?.NomArret ?? "?"}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur génération description courte : {ex.Message}");
                return "Étape invalide";
            }
        }

        /// <summary>
        /// Clone cette étape
        /// </summary>
        public static EtapeItineraire Clone(EtapeItineraire etape)
        {
            try
            {
                return new EtapeItineraire(etape.ArretDepart, etape.ArretArrivee, etape.LigneUtilisee, etape.HeureDepart, etape.HeureArrivee)
                {
                    EstCorrespondance = etape.EstCorrespondance,
                    TempsAttente = etape.TempsAttente,
                    NumeroEtape = etape.NumeroEtape
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur clonage étape : {ex.Message}");
                return new EtapeItineraire();
            }
        }
    }

    /// <summary>
    /// Méthodes d'extension pour les collections d'étapes
    /// </summary>
    public static class EtapeItineraireExtensions
    {
        /// <summary>
        /// Calcule la durée totale d'une liste d'étapes
        /// </summary>
        public static TimeSpan CalculerDureeTotale(this IEnumerable<EtapeItineraire> etapes)
        {
            if (etapes == null || !etapes.Any())
                return TimeSpan.Zero;

            var listeEtapes = etapes.ToList();
            if (listeEtapes.Count == 0)
                return TimeSpan.Zero;

            var premiereEtape = listeEtapes.First();
            var derniereEtape = listeEtapes.Last();

            return derniereEtape.HeureArrivee - premiereEtape.HeureDepart;
        }

        /// <summary>
        /// Compte le nombre de correspondances dans une liste d'étapes
        /// </summary>
        public static int CompterCorrespondances(this IEnumerable<EtapeItineraire> etapes)
        {
            if (etapes == null)
                return 0;

            return etapes.Count(e => e.EstCorrespondance);
        }

        /// <summary>
        /// Retourne les lignes utilisées dans une liste d'étapes
        /// </summary>
        public static List<Ligne> GetLignesUtilisees(this IEnumerable<EtapeItineraire> etapes)
        {
            if (etapes == null)
                return new List<Ligne>();

            return etapes
                .Where(e => e.LigneUtilisee != null)
                .Select(e => e.LigneUtilisee)
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// Valide que toutes les étapes peuvent se connecter entre elles
        /// </summary>
        public static bool EstSequenceValide(this IEnumerable<EtapeItineraire> etapes, TimeSpan tempsCorrespondanceMin)
        {
            if (etapes == null)
                return false;

            var listeEtapes = etapes.ToList();
            if (listeEtapes.Count <= 1)
                return true;

            for (int i = 0; i < listeEtapes.Count - 1; i++)
            {
                if (!EtapeItineraireUtils.PeutSeConnecterA(listeEtapes[i], listeEtapes[i + 1], tempsCorrespondanceMin))
                    return false;
            }

            return true;
        }
    }
}
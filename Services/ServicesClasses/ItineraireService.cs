using BiblioSysteme;

namespace Services.ServicesClasses
{
    /// <summary>
    /// Services pour la gestion des étapes dans les itinéraires
    /// </summary>
    public static class ItineraireServices
    {
        /// <summary>
        /// Ajoute une étape à un itinéraire
        /// </summary>
        public static bool AjouterEtape(Itineraire itineraire, EtapeItineraire etape)
        {
            try
            {
                if (itineraire == null || etape == null)
                    return false;

                itineraire.Etapes.Add(etape);
                etape.NumeroEtape = itineraire.Etapes.Count;

                // Mettre à jour les horaires globaux
                if (itineraire.Etapes.Count == 1)
                {
                    itineraire.HeureDepart = etape.HeureDepart;
                    itineraire.HeureArrivee = etape.HeureArrivee;
                }
                else
                {
                    if (etape.HeureDepart < itineraire.HeureDepart)
                        itineraire.HeureDepart = etape.HeureDepart;
                    if (etape.HeureArrivee > itineraire.HeureArrivee)
                        itineraire.HeureArrivee = etape.HeureArrivee;
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur ajout étape : {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Supprime une étape d'un itinéraire par index
        /// </summary>
        public static bool SupprimerEtape(Itineraire itineraire, int index)
        {
            try
            {
                if (itineraire?.Etapes == null || index < 0 || index >= itineraire.Etapes.Count)
                    return false;

                itineraire.Etapes.RemoveAt(index);

                // Recalculer les numéros d'étapes
                for (int i = 0; i < itineraire.Etapes.Count; i++)
                {
                    itineraire.Etapes[i].NumeroEtape = i + 1;
                }

                // Recalculer les horaires globaux
                if (itineraire.Etapes.Count > 0)
                {
                    itineraire.HeureDepart = itineraire.Etapes.Min(e => e.HeureDepart);
                    itineraire.HeureArrivee = itineraire.Etapes.Max(e => e.HeureArrivee);
                }
                else
                {
                    // Itinéraire vide
                    itineraire.HeureDepart = TimeSpan.Zero;
                    itineraire.HeureArrivee = TimeSpan.Zero;
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur suppression étape : {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Supprime une étape spécifique d'un itinéraire (??)
        /// </summary>
        public static bool SupprimerEtapeSpec(Itineraire itineraire, EtapeItineraire etape)
        {
            try
            {
                if (itineraire?.Etapes == null || etape == null)
                    return false;

                int index = itineraire.Etapes.IndexOf(etape);
                if (index == -1)
                    return false;

                return SupprimerEtape(itineraire, index);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur suppression étape spécifique : {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Insère une étape à une position spécifique dans l'itinéraire
        /// </summary>
        public static bool InsererEtape(Itineraire itineraire, int index, EtapeItineraire etape)
        {
            try
            {
                if (itineraire?.Etapes == null || etape == null)
                    return false;

                if (index < 0 || index > itineraire.Etapes.Count)
                    return false;

                itineraire.Etapes.Insert(index, etape);

                // Recalculer les numéros d'étapes
                for (int i = 0; i < itineraire.Etapes.Count; i++)
                {
                    itineraire.Etapes[i].NumeroEtape = i + 1;
                }

                // Mettre à jour les horaires globaux
                if (etape.HeureDepart < itineraire.HeureDepart)
                    itineraire.HeureDepart = etape.HeureDepart;
                if (etape.HeureArrivee > itineraire.HeureArrivee)
                    itineraire.HeureArrivee = etape.HeureArrivee;

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur insertion étape : {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Remplace une étape par une autre à un index donné
        /// </summary>
        public static bool RemplacerEtape(Itineraire itineraire, int index, EtapeItineraire nouvelleEtape)
        {
            try
            {
                if (itineraire?.Etapes == null || nouvelleEtape == null)
                    return false;

                if (index < 0 || index >= itineraire.Etapes.Count)
                    return false;

                itineraire.Etapes[index] = nouvelleEtape;
                nouvelleEtape.NumeroEtape = index + 1;

                // Recalculer les horaires globaux
                itineraire.HeureDepart = itineraire.Etapes.Min(e => e.HeureDepart);
                itineraire.HeureArrivee = itineraire.Etapes.Max(e => e.HeureArrivee);

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur remplacement étape : {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Vide toutes les étapes d'un itinéraire
        /// </summary>
        public static void ViderEtapes(Itineraire itineraire)
        {
            try
            {
                if (itineraire?.Etapes == null)
                    return;

                itineraire.Etapes.Clear();
                itineraire.HeureDepart = TimeSpan.Zero;
                itineraire.HeureArrivee = TimeSpan.Zero;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur vidage étapes : {ex.Message}");
            }
        }

        /// <summary>
        /// Déplace une étape d'une position à une autre
        /// </summary>
        public static bool DeplacerEtape(Itineraire itineraire, int indexSource, int indexDestination)
        {
            try
            {
                if (itineraire?.Etapes == null)
                    return false;

                if (indexSource < 0 || indexSource >= itineraire.Etapes.Count ||
                    indexDestination < 0 || indexDestination >= itineraire.Etapes.Count ||
                    indexSource == indexDestination)
                    return false;

                var etape = itineraire.Etapes[indexSource];
                itineraire.Etapes.RemoveAt(indexSource);
                itineraire.Etapes.Insert(indexDestination, etape);

                // Recalculer les numéros d'étapes
                for (int i = 0; i < itineraire.Etapes.Count; i++)
                {
                    itineraire.Etapes[i].NumeroEtape = i + 1;
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur déplacement étape : {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Recherche une étape par ses arrêts de départ et d'arrivée
        /// </summary>
        public static EtapeItineraire TrouverEtape(Itineraire itineraire, Arret arretDepart, Arret arretArrivee)
        {
            try
            {
                if (itineraire?.Etapes == null || arretDepart == null || arretArrivee == null)
                    return null;

                return itineraire.Etapes.FirstOrDefault(e =>
                    e.ArretDepart?.IdArret == arretDepart.IdArret &&
                    e.ArretArrivee?.IdArret == arretArrivee.IdArret);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur recherche étape : {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Recherche toutes les étapes utilisant une ligne spécifique
        /// </summary>
        public static List<EtapeItineraire> TrouverEtapesParLigne(Itineraire itineraire, Ligne ligne)
        {
            try
            {
                if (itineraire?.Etapes == null || ligne == null)
                    return new List<EtapeItineraire>();

                return itineraire.Etapes
                    .Where(e => e.LigneUtilisee?.IdLigne == ligne.IdLigne)
                    .ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur recherche étapes par ligne : {ex.Message}");
                return new List<EtapeItineraire>();
            }
        }

        /// <summary>
        /// Compte le nombre d'étapes dans l'itinéraire
        /// </summary>
        public static int CompterEtapes(Itineraire itineraire)
        {
            try
            {
                return itineraire?.Etapes?.Count ?? 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur comptage étapes : {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Vérifie si l'itinéraire contient des étapes
        /// </summary>
        public static bool ADesEtapes(Itineraire itineraire)
        {
            try
            {
                return itineraire?.Etapes?.Count > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur vérification présence étapes : {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Obtient la première étape de l'itinéraire
        /// </summary>
        public static EtapeItineraire ObtenirPremiereEtape(Itineraire itineraire)
        {
            try
            {
                return itineraire?.Etapes?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur obtention première étape : {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Obtient la dernière étape de l'itinéraire
        /// </summary>
        public static EtapeItineraire ObtenirDerniereEtape(Itineraire itineraire)
        {
            try
            {
                return itineraire?.Etapes?.LastOrDefault();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur obtention dernière étape : {ex.Message}");
                return null;
            }
        }
    }
}
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using BiblioSysteme;
//using BiblioBDD;

//namespace Services
//{
//    /// <summary>
//    /// Calculateur d'itinéraires utilisant l'algorithme de Dijkstra adapté aux transports en commun
//    /// </summary>
//    public class CalculateurItineraire
//    {
//        private readonly List<Arret> _tousLesArrets;
//        private readonly List<Ligne> _toutesLesLignes;
//        private readonly Dictionary<int, List<HoraireArret>> _horairesCache;
//        private readonly Dictionary<int, Arret> _arretsParId;
//        private readonly Dictionary<int, Ligne> _lignesParId;

//        public CalculateurItineraire()
//        {
//            try
//            {
//                // Charger les données depuis la BDD
//                _tousLesArrets = RecupDonnees.GetTousLesArrets() ?? new List<Arret>();
//                _toutesLesLignes = RecupDonnees.GetToutesLesLignes() ?? new List<Ligne>();
                
//                // Créer les dictionnaires pour un accès rapide
//                _arretsParId = _tousLesArrets.ToDictionary(a => a.IdArret, a => a);
//                _lignesParId = _toutesLesLignes.ToDictionary(l => l.IdLigne, l => l);
                
//                // Initialiser le cache des horaires
//                _horairesCache = new Dictionary<int, List<HoraireArret>>();
//                InitialiserCache();
                
//                System.Diagnostics.Debug.WriteLine($"CalculateurItineraire initialisé : {_tousLesArrets.Count} arrêts, {_toutesLesLignes.Count} lignes");
//            }
//            catch (Exception ex)
//            {
//                System.Diagnostics.Debug.WriteLine($"Erreur initialisation CalculateurItineraire : {ex.Message}");
//                _tousLesArrets = new List<Arret>();
//                _toutesLesLignes = new List<Ligne>();
//                _arretsParId = new Dictionary<int, Arret>();
//                _lignesParId = new Dictionary<int, Ligne>();
//                _horairesCache = new Dictionary<int, List<HoraireArret>>();
//            }
//        }

//        /// <summary>
//        /// Calcule les meilleurs itinéraires entre deux arrêts
//        /// </summary>
//        public List<Itineraire> CalculerItineraires(Arret arretDepart, Arret arretDestination, ParametresRecherche parametres)
//        {
//            try
//            {
//                if (arretDepart == null || arretDestination == null || parametres == null)
//                    return new List<Itineraire>();

//                if (arretDepart.IdArret == arretDestination.IdArret)
//                    return new List<Itineraire>();

//                var resultats = new List<Itineraire>();

//                // Algorithme de Dijkstra modifié pour les transports en commun
//                var cheminsPossibles = DijkstraTransport(arretDepart, arretDestination, parametres);

//                // Convertir les chemins en itinéraires
//                foreach (var chemin in cheminsPossibles)
//                {
//                    var itineraire = ConstruireItineraire(chemin, arretDepart, arretDestination, parametres);
//                    if (itineraire != null)
//                    {
//                        resultats.Add(itineraire);
//                    }
//                }

//                // Trier et limiter les résultats
//                return resultats
//                    .OrderBy(i => i.TempsTotal)
//                    .ThenBy(i => i.NombreCorrespondances)
//                    .Take(ParametresRecherche.NombreMaxItineraires)
//                    .ToList();
//            }
//            catch (Exception ex)
//            {
//                System.Diagnostics.Debug.WriteLine($"Erreur calcul itinéraires : {ex.Message}");
//                return new List<Itineraire>();
//            }
//        }

//        /// <summary>
//        /// Obtient un arrêt par son ID
//        /// </summary>
//        private Arret ObtenirArret(int idArret)
//        {
//            return _arretsParId.TryGetValue(idArret, out var arret) ? arret : null;
//        }

//        /// <summary>
//        /// Obtient une ligne par son ID
//        /// </summary>
//        private Ligne ObtenirLigne(int idLigne)
//        {
//            return _lignesParId.TryGetValue(idLigne, out var ligne) ? ligne : null;
//        }

//        /// <summary>
//        /// VERSION OPTIMISÉE de l'algorithme de Dijkstra pour les transports en commun
//        /// </summary>
//        private List<CheminTransport> DijkstraTransport(Arret arretDepart, Arret arretDestination, ParametresRecherche parametres)
//        {
//            var chemins = new List<CheminTransport>();
//            var distancesMinimales = new Dictionary<int, TimeSpan>();
//            var noeudsVisites = new HashSet<int>(); // AJOUT : éviter de retraiter les mêmes arrêts
//            var noeudsATraiter = new List<NoeudDijkstra>();

//            // Vérifications initiales
//            if (!_horairesCache.ContainsKey(arretDepart.IdArret))
//            {
//                System.Diagnostics.Debug.WriteLine($"Erreur : Arrêt de départ {arretDepart.NomArret} non trouvé dans le cache");
//                return chemins;
//            }

//            if (!_arretsParId.ContainsKey(arretDestination.IdArret))
//            {
//                System.Diagnostics.Debug.WriteLine($"Erreur : Arrêt de destination {arretDestination.NomArret} non trouvé");
//                return chemins;
//            }

//            // Initialiser avec le nœud de départ
//            var noeudDepart = new NoeudDijkstra
//            {
//                ArretId = arretDepart.IdArret,
//                TempsTotal = TimeSpan.Zero,
//                HeureArrivee = parametres.HeureSouhaitee,
//                CheminParcouru = new List<EtapeTransport>(),
//                NombreCorrespondances = 0
//            };

//            noeudsATraiter.Add(noeudDepart);
//            distancesMinimales[arretDepart.IdArret] = TimeSpan.Zero;

//            System.Diagnostics.Debug.WriteLine($"Démarrage Dijkstra optimisé : {arretDepart.NomArret} → {arretDestination.NomArret} à {parametres.HeureSouhaitee}");

//            // OPTIMISATION : Limites plus strictes
//            int iterationsMax = 1000; // Réduit de 1000 à 50
//            int iteration = 0;
//            var tempsLimite = parametres.HeureSouhaitee.Add(TimeSpan.FromMinutes(90)); // Limite à 90 minutes

//            while (noeudsATraiter.Count > 0 &&
//                   chemins.Count < ParametresRecherche.NombreMaxItineraires &&
//                   iteration < iterationsMax)
//            {
//                iteration++;

//                // OPTIMISATION : Trier seulement quand nécessaire (tous les 5 nœuds)
//                if (iteration % 5 == 1 || noeudsATraiter.Count < 10)
//                {
//                    noeudsATraiter.Sort((a, b) => {
//                        var compareTemps = a.TempsTotal.CompareTo(b.TempsTotal);
//                        if (compareTemps != 0) return compareTemps;
//                        return a.NombreCorrespondances.CompareTo(b.NombreCorrespondances);
//                    });
//                }

//                var noeudActuel = noeudsATraiter[0];
//                noeudsATraiter.RemoveAt(0);

//                // OPTIMISATION : Éviter de retraiter les mêmes arrêts avec un temps plus long
//                string cleNoeud = $"{noeudActuel.ArretId}_{noeudActuel.HeureArrivee.Hours}_{noeudActuel.HeureArrivee.Minutes}";
//                if (noeudsVisites.Contains(noeudActuel.ArretId))
//                {
//                    // Si on a déjà visité cet arrêt avec un meilleur temps, ignorer
//                    if (distancesMinimales.TryGetValue(noeudActuel.ArretId, out var tempsPrecedent) &&
//                        noeudActuel.TempsTotal >= tempsPrecedent.Add(TimeSpan.FromMinutes(5))) // Tolérance de 5 min
//                    {
//                        continue;
//                    }
//                }

//                noeudsVisites.Add(noeudActuel.ArretId);

//                System.Diagnostics.Debug.WriteLine($"Itération {iteration}: {ObtenirArret(noeudActuel.ArretId)?.NomArret} (temps: {noeudActuel.TempsTotal.TotalMinutes:F0}min)");

//                // Si on a atteint la destination
//                if (noeudActuel.ArretId == arretDestination.IdArret)
//                {
//                    var chemin = new CheminTransport
//                    {
//                        Etapes = new List<EtapeTransport>(noeudActuel.CheminParcouru),
//                        TempsTotal = noeudActuel.TempsTotal,
//                        NombreCorrespondances = noeudActuel.NombreCorrespondances
//                    };
//                    chemins.Add(chemin);
//                    System.Diagnostics.Debug.WriteLine($"✅ Chemin {chemins.Count} trouvé ! {chemin.Etapes.Count} étapes, {chemin.TempsTotal.TotalMinutes:F0} min");
//                    continue;
//                }

//                // OPTIMISATION : Arrêt précoce si on dépasse la limite de temps
//                if (noeudActuel.HeureArrivee > tempsLimite)
//                {
//                    System.Diagnostics.Debug.WriteLine($"Arrêt précoce : dépassement limite de temps");
//                    break;
//                }

//                // Explorer les voisins avec contraintes plus strictes
//                ExplorerVoisinsOptimise(noeudActuel, noeudsATraiter, parametres, distancesMinimales, arretDestination.IdArret);
//            }

//            System.Diagnostics.Debug.WriteLine($"Dijkstra terminé après {iteration} itérations, {chemins.Count} chemins trouvés");
//            return chemins;
//        }

//        /// <summary>
//        /// VERSION OPTIMISÉE de l'exploration des voisins
//        /// </summary>
//        private void ExplorerVoisinsOptimise(NoeudDijkstra noeudActuel, List<NoeudDijkstra> noeudsATraiter,
//            ParametresRecherche parametres, Dictionary<int, TimeSpan> distancesMinimales, int arretDestinationId)
//        {
//            var arretActuel = ObtenirArret(noeudActuel.ArretId);

//            if (!_horairesCache.ContainsKey(noeudActuel.ArretId))
//            {
//                return;
//            }

//            // OPTIMISATION : Récupérer moins d'horaires mais de meilleure qualité
//            var horairesDisponibles = ObtenirHorairesDepuisOptimise(noeudActuel.ArretId, noeudActuel.HeureArrivee, parametres, arretDestinationId);

//            System.Diagnostics.Debug.WriteLine($"Exploration depuis {arretActuel?.NomArret}: {horairesDisponibles.Count} horaires sélectionnés");

//            foreach (var horaire in horairesDisponibles)
//            {
//                // Vérifications de base
//                if (!_arretsParId.ContainsKey(horaire.ArretDestinationId))
//                    continue;

//                // OPTIMISATION : Calculer le temps total de manière plus efficace
//                var nouveauTempsTotal = horaire.HeureArrivee - parametres.HeureSouhaitee;

//                // Gérer le passage à minuit (rare)
//                if (nouveauTempsTotal < TimeSpan.Zero)
//                    nouveauTempsTotal = nouveauTempsTotal.Add(TimeSpan.FromDays(1));

//                // OPTIMISATION : Contraintes plus strictes sur le temps
//                if (nouveauTempsTotal > TimeSpan.FromMinutes(90)) // Limite à 90 minutes
//                    continue;

//                // OPTIMISATION : Éviter les détours évidents
//                if (distancesMinimales.TryGetValue(horaire.ArretDestinationId, out var distanceConnue))
//                {
//                    if (nouveauTempsTotal >= distanceConnue.Add(TimeSpan.FromMinutes(10))) // Tolérance réduite
//                        continue;
//                }

//                // OPTIMISATION : Limiter le nombre de correspondances plus strictement
//                var nombreCorrespondances = noeudActuel.NombreCorrespondances;
//                if (noeudActuel.CheminParcouru.Count > 0)
//                {
//                    var derniereEtape = noeudActuel.CheminParcouru.Last();
//                    if (derniereEtape.LigneId != horaire.LigneId)
//                    {
//                        nombreCorrespondances++;

//                        // OPTIMISATION : Maximum 2 correspondances
//                        if (nombreCorrespondances > 2)
//                            continue;

//                        // Vérifier le temps de correspondance
//                        var tempsCorrespondance = horaire.HeureDepart - noeudActuel.HeureArrivee;
//                        if (tempsCorrespondance < parametres.TempsCorrespondanceMin ||
//                            tempsCorrespondance > parametres.TempsCorrespondanceMax)
//                            continue;
//                    }
//                }

//                // Créer le nouveau nœud
//                var nouveauNoeud = new NoeudDijkstra
//                {
//                    ArretId = horaire.ArretDestinationId,
//                    TempsTotal = nouveauTempsTotal,
//                    HeureArrivee = horaire.HeureArrivee,
//                    CheminParcouru = new List<EtapeTransport>(noeudActuel.CheminParcouru),
//                    NombreCorrespondances = nombreCorrespondances
//                };

//                // Ajouter la nouvelle étape
//                nouveauNoeud.CheminParcouru.Add(new EtapeTransport
//                {
//                    ArretDepartId = noeudActuel.ArretId,
//                    ArretDestinationId = horaire.ArretDestinationId,
//                    LigneId = horaire.LigneId,
//                    HeureDepart = horaire.HeureDepart,
//                    HeureArrivee = horaire.HeureArrivee
//                });

//                // Mettre à jour la distance minimale
//                distancesMinimales[horaire.ArretDestinationId] = nouveauTempsTotal;

//                // OPTIMISATION : Limiter la taille de la file d'attente
//                if (noeudsATraiter.Count < 100) // Maximum 100 nœuds en attente
//                {
//                    noeudsATraiter.Add(nouveauNoeud);
//                }
//            }
//        }

//        /// <summary>
//        /// VERSION OPTIMISÉE de la récupération des horaires
//        /// </summary>
//        private List<HoraireArret> ObtenirHorairesDepuisOptimise(int idArret, TimeSpan heureMinimale,
//            ParametresRecherche parametres, int arretDestinationId)
//        {
//            if (!_horairesCache.TryGetValue(idArret, out var tousLesHoraires))
//            {
//                return new List<HoraireArret>();
//            }

//            // OPTIMISATION : Récupérer moins d'horaires mais mieux ciblés
//            var horairesFilters = tousLesHoraires.Where(h =>
//            {
//                // Conditions de base
//                if (h.HeureDepart < heureMinimale || h.HeureDepart >= h.HeureArrivee)
//                    return false;

//                // OPTIMISATION : Privilégier les connexions vers la destination
//                bool versDestination = h.ArretDestinationId == arretDestinationId;

//                // Limite de temps plus stricte
//                var duree = h.HeureArrivee - parametres.HeureSouhaitee;
//                if (duree < TimeSpan.Zero) duree = duree.Add(TimeSpan.FromDays(1));

//                bool dansLesTemps = duree <= TimeSpan.FromMinutes(90);

//                return dansLesTemps;
//            })
//            .OrderBy(h => h.ArretDestinationId == arretDestinationId ? 0 : 1) // Priorité à la destination
//            .ThenBy(h => h.HeureDepart)
//            .Take(10) // OPTIMISATION : Maximum 10 horaires par arrêt
//            .ToList();

//            return horairesFilters;
//        }

//        /// <summary>
//        /// AJOUT : Méthode de debug pour analyser le cache
//        /// </summary>
//        public void DebugCacheArret(int idArret)
//        {
//            var arret = ObtenirArret(idArret);
//            System.Diagnostics.Debug.WriteLine($"\n=== DEBUG CACHE ARRÊT {arret?.NomArret} (ID: {idArret}) ===");

//            if (!_horairesCache.TryGetValue(idArret, out var horaires))
//            {
//                System.Diagnostics.Debug.WriteLine("Aucun horaire en cache pour cet arrêt !");
//                return;
//            }

//            System.Diagnostics.Debug.WriteLine($"Nombre total d'horaires en cache: {horaires.Count}");

//            // Grouper par ligne
//            var horairesParLigne = horaires.GroupBy(h => h.LigneId);

//            foreach (var groupe in horairesParLigne)
//            {
//                var ligne = ObtenirLigne(groupe.Key);
//                System.Diagnostics.Debug.WriteLine($"\nLigne {ligne?.NomLigne} ({groupe.Count()} horaires):");

//                foreach (var h in groupe.Take(5)) // Premiers 5 horaires de cette ligne
//                {
//                    var arretDest = ObtenirArret(h.ArretDestinationId);
//                    System.Diagnostics.Debug.WriteLine($"  {h.HeureDepart} → {h.HeureArrivee} vers {arretDest?.NomArret}");
//                }

//                if (groupe.Count() > 5)
//                    System.Diagnostics.Debug.WriteLine($"  ... et {groupe.Count() - 5} autres horaires");
//            }

//            System.Diagnostics.Debug.WriteLine("=== FIN DEBUG ===\n");
//        }

//        /// <summary>
//        /// Construit un itinéraire complet à partir d'un chemin trouvé
//        /// </summary>
//        private Itineraire ConstruireItineraire(CheminTransport chemin, Arret arretDepart, Arret arretDestination, ParametresRecherche parametres)
//        {
//            try
//            {
//                var itineraire = new Itineraire(arretDepart, arretDestination);

//                foreach (var etapeTransport in chemin.Etapes)
//                {
//                    var arretDep = ObtenirArret(etapeTransport.ArretDepartId);
//                    var arretArr = ObtenirArret(etapeTransport.ArretDestinationId);
//                    var ligne = ObtenirLigne(etapeTransport.LigneId);

//                    if (arretDep == null || arretArr == null || ligne == null)
//                        continue;

//                    var etapeItineraire = new EtapeItineraire(
//                        arretDep, arretArr, ligne,
//                        etapeTransport.HeureDepart, etapeTransport.HeureArrivee
//                    );

//                    ItineraireServices.AjouterEtape(itineraire, etapeItineraire);
//                }

//                // Calculer les statistiques et le score
//                //ItineraireServices.CalculerStatistiques(itineraire);
//                //itineraire.ScoreQualite = ItineraireServices.CalculerScoreQualite(itineraire, parametres);

//                return itineraire;
//            }
//            catch (Exception ex)
//            {
//                System.Diagnostics.Debug.WriteLine($"Erreur construction itinéraire : {ex.Message}");
//                return null;
//            }
//        }

//        /// <summary>
//        /// OPTIMISATION : Nettoyer le cache après initialisation pour supprimer les doublons
//        /// </summary>
//        private void OptimiserCache()
//        {
//            try
//            {
//                int horairesAvant = _horairesCache.Values.Sum(h => h.Count);

//                foreach (var kvp in _horairesCache.ToList())
//                {
//                    var arretId = kvp.Key;
//                    var horaires = kvp.Value;

//                    // Supprimer les doublons
//                    var horairesDedupliques = horaires
//                        .GroupBy(h => new { h.ArretDestinationId, h.LigneId, h.HeureDepart })
//                        .Select(g => g.First())
//                        .OrderBy(h => h.HeureDepart)
//                        .ToList();

//                    _horairesCache[arretId] = horairesDedupliques;
//                }

//                int horairesApres = _horairesCache.Values.Sum(h => h.Count);
//                System.Diagnostics.Debug.WriteLine($"Cache optimisé : {horairesAvant} → {horairesApres} horaires ({horairesAvant - horairesApres} doublons supprimés)");
//            }
//            catch (Exception ex)
//            {
//                System.Diagnostics.Debug.WriteLine($"Erreur optimisation cache : {ex.Message}");
//            }
//        }

//        /// <summary>
//        /// MISE À JOUR de InitialiserCache avec optimisation finale
//        /// </summary>
//        private void InitialiserCache()
//        {
//            try
//            {
//                _horairesCache.Clear();

//                foreach (var ligne in _toutesLesLignes)
//                {
//                    var arretsLigne = ChargerDonnees.ChargerArretsParLigne(ligne.IdLigne);
//                    if (arretsLigne.Count < 2) continue;

//                    ligne.Arrets.Clear();
//                    ligne.Arrets.AddRange(arretsLigne);

//                    if (ligne.TempsEntreArrets == null || ligne.TempsEntreArrets.Count == 0)
//                    {
//                        LigneService.GenererTempsEntreArrets(ligne, 3);
//                    }

//                    // Générer les horaires dans les deux sens
//                    GenererHorairesEntreArrets(ligne, arretsLigne);
//                }

//                // OPTIMISATION : Nettoyer le cache après génération
//                OptimiserCache();

//                var nbArrets = _horairesCache.Count;
//                var nbHoraires = _horairesCache.Values.Sum(h => h.Count);
//                System.Diagnostics.Debug.WriteLine($"Cache initialisé et optimisé : {nbArrets} arrêts, {nbHoraires} horaires");

//                if (nbHoraires == 0)
//                {
//                    System.Diagnostics.Debug.WriteLine("ATTENTION : Le cache des horaires est vide !");
//                }
//            }
//            catch (Exception ex)
//            {
//                System.Diagnostics.Debug.WriteLine($"Erreur initialisation cache optimisé : {ex.Message}");
//            }
//        }

//        /// <summary>
//        /// NOUVELLE MÉTHODE : Génère les horaires dans les deux sens pour une ligne
//        /// </summary>
//        private void GenererHorairesEntreArrets(Ligne ligne, List<Arret> arretsLigne)
//        {
//            try
//            {
//                // 1. SENS NORMAL (A → B → C → D)
//                GenererHorairesSens(ligne, arretsLigne, false);

//                // 2. SENS INVERSE (D → C → B → A)
//                var arretsInverses = new List<Arret>(arretsLigne);
//                arretsInverses.Reverse();
//                GenererHorairesSens(ligne, arretsInverses, true);
//            }
//            catch (Exception ex)
//            {
//                System.Diagnostics.Debug.WriteLine($"Erreur génération horaires bidirectionnels ligne {ligne.NomLigne} : {ex.Message}");
//            }
//        }

//        /// <summary>
//        /// VERSION OPTIMISÉE de la génération des horaires dans un sens
//        /// Génère moins d'horaires mais de meilleure qualité
//        /// </summary>
//        private void GenererHorairesSens(Ligne ligne, List<Arret> arretsOrdonnes, bool estSensInverse)
//        {
//            try
//            {
//                // OPTIMISATION : Décalage plus important pour différencier les sens
//                var decalageSensInverse = estSensInverse ? TimeSpan.FromMinutes(45) : TimeSpan.Zero;

//                // OPTIMISATION : Générer moins d'horaires de base (tous les 15 minutes au lieu de toutes les X minutes)
//                var horairesBase = GenererHorairesOptimises(ligne);

//                // Pour chaque tronçon entre arrêts consécutifs
//                for (int i = 0; i < arretsOrdonnes.Count - 1; i++)
//                {
//                    var arretDepart = arretsOrdonnes[i];
//                    var arretArrivee = arretsOrdonnes[i + 1];

//                    // Initialiser le cache pour cet arrêt de départ s'il n'existe pas
//                    if (!_horairesCache.ContainsKey(arretDepart.IdArret))
//                    {
//                        _horairesCache[arretDepart.IdArret] = new List<HoraireArret>();
//                    }

//                    // Pour chaque horaire de base, calculer l'horaire à ce tronçon
//                    foreach (var horaireBase in horairesBase)
//                    {
//                        // Calculer les temps cumulés
//                        var tempsCumuleDepart = CalculerTempsCumule(ligne, arretsOrdonnes, i, estSensInverse);
//                        var tempsCumuleArrivee = CalculerTempsCumule(ligne, arretsOrdonnes, i + 1, estSensInverse);

//                        // Horaires effectifs
//                        var heureDepart = horaireBase.Add(tempsCumuleDepart).Add(decalageSensInverse);
//                        var heureArrivee = horaireBase.Add(tempsCumuleArrivee).Add(decalageSensInverse);

//                        // OPTIMISATION : Limiter aux heures de service (6h-22h)
//                        if (heureDepart >= TimeSpan.FromHours(6) &&
//                            heureArrivee <= TimeSpan.FromHours(22) &&
//                            heureArrivee > heureDepart)
//                        {
//                            _horairesCache[arretDepart.IdArret].Add(new HoraireArret
//                            {
//                                ArretDestinationId = arretArrivee.IdArret,
//                                LigneId = ligne.IdLigne,
//                                HeureDepart = heureDepart,
//                                HeureArrivee = heureArrivee
//                            });
//                        }
//                    }
//                }

//                System.Diagnostics.Debug.WriteLine($"Ligne {ligne.NomLigne} ({(estSensInverse ? "inverse" : "normal")}) : " +
//                    $"{horairesBase.Count} horaires de base générés");
//            }
//            catch (Exception ex)
//            {
//                System.Diagnostics.Debug.WriteLine($"Erreur génération horaires sens ligne {ligne.NomLigne} : {ex.Message}");
//            }
//        }

//        /// <summary>
//        /// OPTIMISATION : Génère des horaires de base optimisés (moins nombreux)
//        /// </summary>
//        private List<TimeSpan> GenererHorairesOptimises(Ligne ligne)
//        {
//            var horaires = new List<TimeSpan>();

//            // OPTIMISATION : Horaires toutes les 15 minutes au lieu d'utiliser l'intervalle de la ligne
//            var intervalle = TimeSpan.FromMinutes(15);
//            var debut = TimeSpan.FromHours(6);  // 6h00
//            var fin = TimeSpan.FromHours(22);   // 22h00

//            var heure = debut;
//            while (heure <= fin)
//            {
//                horaires.Add(heure);
//                heure = heure.Add(intervalle);
//            }

//            return horaires;
//        }

//        /// <summary>
//        /// NOUVELLE MÉTHODE : Calcule le temps cumulé jusqu'à un arrêt donné dans un sens
//        /// </summary>
//        private TimeSpan CalculerTempsCumule(Ligne ligne, List<Arret> arretsOrdonnes, int indexArret, bool estSensInverse)
//        {
//            try
//            {
//                if (indexArret <= 0) return TimeSpan.Zero;

//                var tempsCumule = TimeSpan.Zero;

//                // Dans le sens normal, on utilise les temps tels quels
//                // Dans le sens inverse, on utilise les mêmes temps mais dans l'ordre inverse
//                if (!estSensInverse)
//                {
//                    // Sens normal : additionner les temps de 0 à indexArret-1
//                    for (int i = 0; i < indexArret && i < ligne.TempsEntreArrets.Count; i++)
//                    {
//                        tempsCumule = tempsCumule.Add(ligne.TempsEntreArrets[i]);
//                    }
//                }
//                else
//                {
//                    // Sens inverse : les temps entre arrêts restent les mêmes, 
//                    // mais l'ordre des arrêts est inversé
//                    int nbArrets = arretsOrdonnes.Count;
//                    for (int i = 0; i < indexArret && i < ligne.TempsEntreArrets.Count; i++)
//                    {
//                        // Utiliser l'index correspondant dans le sens normal
//                        int indexTempsNormal = (nbArrets - 2) - i;
//                        if (indexTempsNormal >= 0 && indexTempsNormal < ligne.TempsEntreArrets.Count)
//                        {
//                            tempsCumule = tempsCumule.Add(ligne.TempsEntreArrets[indexTempsNormal]);
//                        }
//                    }
//                }

//                return tempsCumule;
//            }
//            catch (Exception ex)
//            {
//                System.Diagnostics.Debug.WriteLine($"Erreur calcul temps cumulé : {ex.Message}");
//                return TimeSpan.Zero;
//            }
//        }

//        /// <summary>
//        /// Obtient des statistiques sur le calculateur
//        /// </summary>
//        public string ObtenirStatistiques()
//        {
//            try
//            {
//                var nbArrets = _horairesCache.Count;
//                var nbHoraires = _horairesCache.Values.Sum(h => h.Count);
//                var nbLignes = _toutesLesLignes.Count;

//                return $"Calculateur: {nbArrets} arrêts, {nbLignes} lignes, {nbHoraires} horaires en cache";
//            }
//            catch (Exception ex)
//            {
//                System.Diagnostics.Debug.WriteLine($"Erreur statistiques : {ex.Message}");
//                return "Statistiques non disponibles";
//            }
//        }
//    }

//    // Classes internes pour le calcul
//    internal class NoeudDijkstra
//    {
//        public int ArretId { get; set; }
//        public TimeSpan TempsTotal { get; set; }
//        public TimeSpan HeureArrivee { get; set; }
//        public List<EtapeTransport> CheminParcouru { get; set; } = new List<EtapeTransport>();
//        public int NombreCorrespondances { get; set; }
//    }

//    internal class EtapeTransport
//    {
//        public int ArretDepartId { get; set; }
//        public int ArretDestinationId { get; set; }
//        public int LigneId { get; set; }
//        public TimeSpan HeureDepart { get; set; }
//        public TimeSpan HeureArrivee { get; set; }
//    }

//    internal class HoraireArret
//    {
//        public int ArretDestinationId { get; set; }
//        public int LigneId { get; set; }
//        public TimeSpan HeureDepart { get; set; }
//        public TimeSpan HeureArrivee { get; set; }
//    }

//    internal class CheminTransport
//    {
//        public List<EtapeTransport> Etapes { get; set; } = new List<EtapeTransport>();
//        public TimeSpan TempsTotal { get; set; }
//        public int NombreCorrespondances { get; set; }
//    }

//    internal class ComparateurNoeudDijkstra : IComparer<NoeudDijkstra>
//    {
//        public int Compare(NoeudDijkstra x, NoeudDijkstra y)
//        {
//            if (x == null && y == null) return 0;
//            if (x == null) return 1;
//            if (y == null) return -1;

//            // Priorité : temps total, puis nombre de correspondances, puis ID arrêt, puis heure arrivée
//            var compareTemps = x.TempsTotal.CompareTo(y.TempsTotal);
//            if (compareTemps != 0) return compareTemps;

//            var compareCorresp = x.NombreCorrespondances.CompareTo(y.NombreCorrespondances);
//            if (compareCorresp != 0) return compareCorresp;

//            var compareArret = x.ArretId.CompareTo(y.ArretId);
//            if (compareArret != 0) return compareArret;

//            return x.HeureArrivee.CompareTo(y.HeureArrivee);
//        }
//    }
//}
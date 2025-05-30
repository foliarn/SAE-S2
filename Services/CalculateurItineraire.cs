using System;
using System.Collections.Generic;
using System.Linq;
using BiblioSysteme;
using BiblioBDD;

namespace Services
{
    /// <summary>
    /// Calculateur d'itinéraires utilisant l'algorithme de Dijkstra adapté aux transports en commun
    /// </summary>
    public class CalculateurItineraire
    {
        private readonly List<Arret> _tousLesArrets;
        private readonly List<Ligne> _toutesLesLignes;
        private readonly Dictionary<int, List<HoraireArret>> _horairesCache;
        private readonly Dictionary<int, Arret> _arretsParId;
        private readonly Dictionary<int, Ligne> _lignesParId;

        public CalculateurItineraire()
        {
            try
            {
                // Charger les données depuis la BDD
                _tousLesArrets = ChargerDonnees.ChargerTousLesArrets() ?? new List<Arret>();
                _toutesLesLignes = ChargerDonnees.ChargerToutesLesLignes() ?? new List<Ligne>();
                
                // Créer les dictionnaires pour un accès rapide
                _arretsParId = _tousLesArrets.ToDictionary(a => a.IdArret, a => a);
                _lignesParId = _toutesLesLignes.ToDictionary(l => l.IdLigne, l => l);
                
                // Initialiser le cache des horaires
                _horairesCache = new Dictionary<int, List<HoraireArret>>();
                InitialiserCache();
                
                System.Diagnostics.Debug.WriteLine($"CalculateurItineraire initialisé : {_tousLesArrets.Count} arrêts, {_toutesLesLignes.Count} lignes");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur initialisation CalculateurItineraire : {ex.Message}");
                _tousLesArrets = new List<Arret>();
                _toutesLesLignes = new List<Ligne>();
                _arretsParId = new Dictionary<int, Arret>();
                _lignesParId = new Dictionary<int, Ligne>();
                _horairesCache = new Dictionary<int, List<HoraireArret>>();
            }
        }

        /// <summary>
        /// Calcule les meilleurs itinéraires entre deux arrêts
        /// </summary>
        public List<Itineraire> CalculerItineraires(Arret arretDepart, Arret arretDestination, ParametresRecherche parametres)
        {
            try
            {
                if (arretDepart == null || arretDestination == null || parametres == null)
                    return new List<Itineraire>();

                if (arretDepart.IdArret == arretDestination.IdArret)
                    return new List<Itineraire>();

                var resultats = new List<Itineraire>();

                // Algorithme de Dijkstra modifié pour les transports en commun
                var cheminsPossibles = DijkstraTransport(arretDepart, arretDestination, parametres);

                // Convertir les chemins en itinéraires
                foreach (var chemin in cheminsPossibles)
                {
                    var itineraire = ConstruireItineraire(chemin, arretDepart, arretDestination, parametres);
                    if (itineraire != null)
                    {
                        resultats.Add(itineraire);
                    }
                }

                // Trier et limiter les résultats
                return resultats
                    .OrderBy(i => i.TempsTotal)
                    .ThenBy(i => i.NombreCorrespondances)
                    .Take(ParametresRecherche.NombreMaxItineraires)
                    .ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur calcul itinéraires : {ex.Message}");
                return new List<Itineraire>();
            }
        }

        /// <summary>
        /// Obtient un arrêt par son ID
        /// </summary>
        private Arret ObtenirArret(int idArret)
        {
            return _arretsParId.TryGetValue(idArret, out var arret) ? arret : null;
        }

        /// <summary>
        /// Obtient une ligne par son ID
        /// </summary>
        private Ligne ObtenirLigne(int idLigne)
        {
            return _lignesParId.TryGetValue(idLigne, out var ligne) ? ligne : null;
        }

        /// <summary>
        /// Alternative : Utiliser une List + tri manuel au lieu de SortedSet
        /// Cette méthode est plus robuste et évite complètement le problème des doublons
        /// </summary>
        private List<CheminTransport> DijkstraTransport(Arret arretDepart, Arret arretDestination, ParametresRecherche parametres)
        {
            var chemins = new List<CheminTransport>();  
            var distancesMinimales = new Dictionary<int, TimeSpan>();
            var noeudsATraiter = new List<NoeudDijkstra>(); // Remplace SortedSet

            // Vérifier que les arrêts existent dans notre cache
            if (!_horairesCache.ContainsKey(arretDepart.IdArret))
            {
                System.Diagnostics.Debug.WriteLine($"Erreur : Arrêt de départ {arretDepart.NomArret} non trouvé dans le cache");
                return chemins;
            }

            if (!_arretsParId.ContainsKey(arretDestination.IdArret))
            {
                System.Diagnostics.Debug.WriteLine($"Erreur : Arrêt de destination {arretDestination.NomArret} non trouvé");
                return chemins;
            }

            // Initialiser avec le nœud de départ
            var noeudDepart = new NoeudDijkstra
            {
                ArretId = arretDepart.IdArret,
                TempsTotal = TimeSpan.Zero,
                HeureArrivee = parametres.HeureSouhaitee,
                CheminParcouru = new List<EtapeTransport>(),
                NombreCorrespondances = 0
            };

            noeudsATraiter.Add(noeudDepart);
            distancesMinimales[arretDepart.IdArret] = TimeSpan.Zero;

            System.Diagnostics.Debug.WriteLine($"Démarrage Dijkstra : {arretDepart.NomArret} → {arretDestination.NomArret} à {parametres.HeureSouhaitee}");

            int iterationsMax = 1000;
            int iteration = 0;

            while (noeudsATraiter.Count > 0 && chemins.Count < ParametresRecherche.NombreMaxItineraires && iteration < iterationsMax)
            {
                iteration++;

                // Trier et prendre le meilleur nœud (équivalent de SortedSet.Min)
                noeudsATraiter.Sort((a, b) => {
                    var compareTemps = a.TempsTotal.CompareTo(b.TempsTotal);
                    if (compareTemps != 0) return compareTemps;
                    return a.NombreCorrespondances.CompareTo(b.NombreCorrespondances);
                });

                var noeudActuel = noeudsATraiter[0];
                noeudsATraiter.RemoveAt(0);

                System.Diagnostics.Debug.WriteLine($"Itération {iteration}: Traitement arrêt {ObtenirArret(noeudActuel.ArretId)?.NomArret} (temps: {noeudActuel.TempsTotal.TotalMinutes:F1}min)");

                // Si on a atteint la destination
                if (noeudActuel.ArretId == arretDestination.IdArret)
                {
                    var chemin = new CheminTransport
                    {
                        Etapes = new List<EtapeTransport>(noeudActuel.CheminParcouru),
                        TempsTotal = noeudActuel.TempsTotal,
                        NombreCorrespondances = noeudActuel.NombreCorrespondances
                    };
                    chemins.Add(chemin);
                    System.Diagnostics.Debug.WriteLine($"Chemin trouvé ! {chemin.Etapes.Count} étapes, {chemin.TempsTotal.TotalMinutes:F0} min");
                    continue;
                }

                // Vérifier si on a déjà trouvé un chemin plus court vers cet arrêt
                if (distancesMinimales.TryGetValue(noeudActuel.ArretId, out var distanceConnue))
                {
                    if (noeudActuel.TempsTotal > distanceConnue)
                    {
                        System.Diagnostics.Debug.WriteLine($"Chemin plus long ignoré vers {ObtenirArret(noeudActuel.ArretId)?.NomArret}");
                        continue;
                    }
                }

                // Explorer les voisins
                ExplorerVoisins(noeudActuel, noeudsATraiter, parametres, distancesMinimales);
            }

            System.Diagnostics.Debug.WriteLine($"Dijkstra terminé après {iteration} itérations, {chemins.Count} chemins trouvés");
            return chemins;
        }

        /// <summary>
        /// Version améliorée d'ExplorerVoisinsAlternatif avec plus de debug
        /// </summary>
        private void ExplorerVoisins(NoeudDijkstra noeudActuel, List<NoeudDijkstra> noeudsATraiter,
            ParametresRecherche parametres, Dictionary<int, TimeSpan> distancesMinimales)
        {
            var arretActuel = ObtenirArret(noeudActuel.ArretId);

            // AJOUT : Debug du cache pour cet arrêt si pas d'horaires
            if (!_horairesCache.ContainsKey(noeudActuel.ArretId))
            {
                System.Diagnostics.Debug.WriteLine($"ERREUR: Arrêt {arretActuel?.NomArret} (ID: {noeudActuel.ArretId}) absent du cache !");
                return;
            }

            // Obtenir les horaires disponibles à partir de cet arrêt
            var horairesDisponibles = ObtenirHorairesDepuis(noeudActuel.ArretId, noeudActuel.HeureArrivee, parametres);

            System.Diagnostics.Debug.WriteLine($"Exploration depuis {arretActuel?.NomArret}: {horairesDisponibles.Count} horaires disponibles");

            // Si pas d'horaires, débugger le cache
            if (horairesDisponibles.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine($"PROBLÈME: Aucun horaire disponible depuis {arretActuel?.NomArret} après {noeudActuel.HeureArrivee}");
                DebugCacheArret(noeudActuel.ArretId);
            }

            foreach (var horaire in horairesDisponibles)
            {
                // Vérifier que l'arrêt de destination existe
                if (!_arretsParId.ContainsKey(horaire.ArretDestinationId))
                {
                    System.Diagnostics.Debug.WriteLine($"ERREUR: Arrêt destination ID {horaire.ArretDestinationId} inexistant");
                    continue;
                }

                // CORRECTION : Calcul du temps total plus robuste
                TimeSpan nouveauTempsTotal;

                if (horaire.HeureArrivee >= parametres.HeureSouhaitee)
                {
                    // Cas normal : même jour
                    nouveauTempsTotal = horaire.HeureArrivee - parametres.HeureSouhaitee;
                }
                else
                {
                    // Cas rare : passage à minuit
                    nouveauTempsTotal = horaire.HeureArrivee.Add(TimeSpan.FromDays(1)) - parametres.HeureSouhaitee;
                }

                // Vérifier les contraintes de temps (plus permissif)
                if (nouveauTempsTotal > parametres.TempsMaxRecherche)
                {
                    System.Diagnostics.Debug.WriteLine($"Horaire écarté: temps total {nouveauTempsTotal.TotalMinutes:F0}min > limite {parametres.TempsMaxRecherche.TotalMinutes}min");
                    continue;
                }

                // Vérifier si on a déjà un meilleur chemin vers cette destination
                if (distancesMinimales.TryGetValue(horaire.ArretDestinationId, out var distanceConnue))
                {
                    if (nouveauTempsTotal >= distanceConnue)
                    {
                        System.Diagnostics.Debug.WriteLine($"Chemin vers {ObtenirArret(horaire.ArretDestinationId)?.NomArret} ignoré (temps {nouveauTempsTotal.TotalMinutes:F0}min >= {distanceConnue.TotalMinutes:F0}min)");
                        continue;
                    }
                }

                // Créer le nouveau nœud
                var nouveauNoeud = new NoeudDijkstra
                {
                    ArretId = horaire.ArretDestinationId,
                    TempsTotal = nouveauTempsTotal,
                    HeureArrivee = horaire.HeureArrivee,
                    CheminParcouru = new List<EtapeTransport>(noeudActuel.CheminParcouru),
                    NombreCorrespondances = noeudActuel.NombreCorrespondances
                };

                // Ajouter la nouvelle étape
                var nouvelleEtape = new EtapeTransport
                {
                    ArretDepartId = noeudActuel.ArretId,
                    ArretDestinationId = horaire.ArretDestinationId,
                    LigneId = horaire.LigneId,
                    HeureDepart = horaire.HeureDepart,
                    HeureArrivee = horaire.HeureArrivee
                };

                nouveauNoeud.CheminParcouru.Add(nouvelleEtape);

                // Compter les correspondances
                if (noeudActuel.CheminParcouru.Count > 0)
                {
                    var derniereEtape = noeudActuel.CheminParcouru.Last();
                    if (derniereEtape.LigneId != horaire.LigneId)
                    {
                        nouveauNoeud.NombreCorrespondances++;
                    }
                }

                // Mettre à jour la distance minimale connue
                distancesMinimales[horaire.ArretDestinationId] = nouveauTempsTotal;

                // Ajouter à la liste des nœuds à traiter
                noeudsATraiter.Add(nouveauNoeud);

                var arretDest = ObtenirArret(horaire.ArretDestinationId);
                var ligneNom = ObtenirLigne(horaire.LigneId)?.NomLigne;
                System.Diagnostics.Debug.WriteLine($"  → Ajout connexion vers {arretDest?.NomArret} (ligne {ligneNom}, temps total: {nouveauTempsTotal.TotalMinutes:F0}min)");
            }
        }

        /// <summary>
        /// Explore les arrêts voisins accessibles depuis le nœud actuel
        /// </summary>
        private void ExplorerVoisins2(NoeudDijkstra noeudActuel, SortedSet<NoeudDijkstra> filePriorite,
            ParametresRecherche parametres, Dictionary<int, TimeSpan> distancesMinimales)
        {
            // Obtenir les horaires disponibles à partir de cet arrêt
            var horairesDisponibles = ObtenirHorairesDepuis(noeudActuel.ArretId, noeudActuel.HeureArrivee, parametres);

            foreach (var horaire in horairesDisponibles)
            {
                // Vérifier que l'arrêt de destination existe
                if (!_arretsParId.ContainsKey(horaire.ArretDestinationId))
                    continue;

                //// Vérifier les contraintes de correspondance
                //if (noeudActuel.CheminParcouru.Count > 0)
                //{
                //    var derniereEtape = noeudActuel.CheminParcouru.Last();
                //    var tempsAttente = horaire.HeureDepart - noeudActuel.HeureArrivee;

                //    // Temps de correspondance insuffisant
                //    if (tempsAttente < parametres.TempsCorrespondanceMin)
                //        continue;

                //    // Temps de correspondance trop long
                //    if (tempsAttente > parametres.TempsCorrespondanceMax)
                //        continue;

                //    // Trop de correspondances
                //    if (derniereEtape.LigneId != horaire.LigneId &&
                //        noeudActuel.NombreCorrespondances >= parametres.NombreMaxCorrespondances)
                //        continue;
                //}

                // Calculer le nouveau temps total
                var nouveauTempsTotal = (horaire.HeureArrivee >= noeudActuel.HeureArrivee)
                    ? horaire.HeureArrivee - parametres.HeureSouhaitee  // Temps depuis le début
                    : horaire.HeureArrivee.Add(TimeSpan.FromDays(1)) - parametres.HeureSouhaitee; // Gérer le passage à minuit

                // Vérifier les contraintes de temps
                if (nouveauTempsTotal > parametres.TempsMaxRecherche)
                    continue;

                // Vérifier si on a déjà un meilleur chemin vers cette destination
                if (distancesMinimales.TryGetValue(horaire.ArretDestinationId, out var distanceConnue))
                {
                    if (nouveauTempsTotal >= distanceConnue)
                        continue; // On a déjà un meilleur chemin
                }

                // Créer le nouveau nœud
                var nouveauNoeud = new NoeudDijkstra
                {
                    ArretId = horaire.ArretDestinationId,
                    TempsTotal = nouveauTempsTotal,
                    HeureArrivee = horaire.HeureArrivee,
                    CheminParcouru = new List<EtapeTransport>(noeudActuel.CheminParcouru),
                    NombreCorrespondances = noeudActuel.NombreCorrespondances
                };

                // Ajouter la nouvelle étape
                var nouvelleEtape = new EtapeTransport
                {
                    ArretDepartId = noeudActuel.ArretId,
                    ArretDestinationId = horaire.ArretDestinationId,
                    LigneId = horaire.LigneId,
                    HeureDepart = horaire.HeureDepart,
                    HeureArrivee = horaire.HeureArrivee
                };

                nouveauNoeud.CheminParcouru.Add(nouvelleEtape);

                // Compter les correspondances
                if (noeudActuel.CheminParcouru.Count > 0)
                {
                    var derniereEtape = noeudActuel.CheminParcouru.Last();
                    if (derniereEtape.LigneId != horaire.LigneId)
                    {
                        nouveauNoeud.NombreCorrespondances++;
                    }
                }

                // Mettre à jour la distance minimale connue
                distancesMinimales[horaire.ArretDestinationId] = nouveauTempsTotal;

                // Ajouter à la file de priorité
                filePriorite.Add(nouveauNoeud);
            }
        }

        /// <summary>
        /// CORRIGÉ : Obtient les horaires de départ disponibles depuis un arrêt à partir d'une heure donnée
        /// </summary>
        private List<HoraireArret> ObtenirHorairesDepuis(int idArret, TimeSpan heureMinimale, ParametresRecherche parametres)
        {
            if (!_horairesCache.TryGetValue(idArret, out var tousLesHoraires))
            {
                System.Diagnostics.Debug.WriteLine($"Aucun horaire en cache pour l'arrêt ID {idArret}");
                return new List<HoraireArret>();
            }

            System.Diagnostics.Debug.WriteLine($"Filtrage horaires pour arrêt {idArret}: {tousLesHoraires.Count} horaires en cache, heure min: {heureMinimale}");

            // CORRECTION PRINCIPALE : Logique de filtrage plus permissive
            var horairesFilters = tousLesHoraires.Where(h =>
            {
                // 1. L'horaire de départ doit être >= heure minimale
                bool heureOk = h.HeureDepart >= heureMinimale;

                // 2. Vérification de cohérence : départ < arrivée
                bool coherenceOk = h.HeureDepart < h.HeureArrivee;

                // 3. Pas de limite trop stricte sur l'heure d'arrivée
                // (on enlève le filtre sur parametres.HeureSouhaitee + TempsMaxRecherche qui était trop restrictif)

                if (!heureOk)
                    System.Diagnostics.Debug.WriteLine($"  Horaire {h.HeureDepart} écarté (< {heureMinimale})");
                if (!coherenceOk)
                    System.Diagnostics.Debug.WriteLine($"  Horaire incohérent: départ {h.HeureDepart} >= arrivée {h.HeureArrivee}");

                return heureOk && coherenceOk;
            })
            .OrderBy(h => h.HeureDepart)
            .Take(20) // Augmenté de 50 à 20 pour le debug, mais plus permissif
            .ToList();

            System.Diagnostics.Debug.WriteLine($"Après filtrage: {horairesFilters.Count} horaires disponibles");

            // Debug détaillé des premiers horaires
            foreach (var h in horairesFilters.Take(3))
            {
                var arretDest = ObtenirArret(h.ArretDestinationId);
                var ligne = ObtenirLigne(h.LigneId);
                System.Diagnostics.Debug.WriteLine($"  Horaire: {h.HeureDepart} → {h.HeureArrivee} vers {arretDest?.NomArret} (ligne {ligne?.NomLigne})");
            }

            return horairesFilters;
        }

        /// <summary>
        /// AJOUT : Méthode de debug pour analyser le cache
        /// </summary>
        public void DebugCacheArret(int idArret)
        {
            var arret = ObtenirArret(idArret);
            System.Diagnostics.Debug.WriteLine($"\n=== DEBUG CACHE ARRÊT {arret?.NomArret} (ID: {idArret}) ===");

            if (!_horairesCache.TryGetValue(idArret, out var horaires))
            {
                System.Diagnostics.Debug.WriteLine("Aucun horaire en cache pour cet arrêt !");
                return;
            }

            System.Diagnostics.Debug.WriteLine($"Nombre total d'horaires en cache: {horaires.Count}");

            // Grouper par ligne
            var horairesParLigne = horaires.GroupBy(h => h.LigneId);

            foreach (var groupe in horairesParLigne)
            {
                var ligne = ObtenirLigne(groupe.Key);
                System.Diagnostics.Debug.WriteLine($"\nLigne {ligne?.NomLigne} ({groupe.Count()} horaires):");

                foreach (var h in groupe.Take(5)) // Premiers 5 horaires de cette ligne
                {
                    var arretDest = ObtenirArret(h.ArretDestinationId);
                    System.Diagnostics.Debug.WriteLine($"  {h.HeureDepart} → {h.HeureArrivee} vers {arretDest?.NomArret}");
                }

                if (groupe.Count() > 5)
                    System.Diagnostics.Debug.WriteLine($"  ... et {groupe.Count() - 5} autres horaires");
            }

            System.Diagnostics.Debug.WriteLine("=== FIN DEBUG ===\n");
        }

        /// <summary>
        /// Construit un itinéraire complet à partir d'un chemin trouvé
        /// </summary>
        private Itineraire ConstruireItineraire(CheminTransport chemin, Arret arretDepart, Arret arretDestination, ParametresRecherche parametres)
        {
            try
            {
                var itineraire = new Itineraire(arretDepart, arretDestination);

                foreach (var etapeTransport in chemin.Etapes)
                {
                    var arretDep = ObtenirArret(etapeTransport.ArretDepartId);
                    var arretArr = ObtenirArret(etapeTransport.ArretDestinationId);
                    var ligne = ObtenirLigne(etapeTransport.LigneId);

                    if (arretDep == null || arretArr == null || ligne == null)
                        continue;

                    var etapeItineraire = new EtapeItineraire(
                        arretDep, arretArr, ligne,
                        etapeTransport.HeureDepart, etapeTransport.HeureArrivee
                    );

                    ItineraireServices.AjouterEtape(itineraire, etapeItineraire);
                }

                // Calculer les statistiques et le score
                //ItineraireServices.CalculerStatistiques(itineraire);
                //itineraire.ScoreQualite = ItineraireServices.CalculerScoreQualite(itineraire, parametres);

                return itineraire;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur construction itinéraire : {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Initialise le cache des horaires pour optimiser les recherches
        /// CORRIGÉ : Génère les connexions dans les deux sens
        /// </summary>
        private void InitialiserCache()
        {
            try
            {
                _horairesCache.Clear();

                foreach (var ligne in _toutesLesLignes)
                {
                    // Charger les arrêts de la ligne depuis la BDD
                    var arretsLigne = ChargerDonnees.ChargerArretsParLigne(ligne.IdLigne);
                    if (arretsLigne.Count < 2) continue;

                    // CORRECTION : Assigner les arrêts à la ligne
                    ligne.Arrets.Clear();
                    ligne.Arrets.AddRange(arretsLigne);

                    // Générer les temps entre arrêts si pas déjà fait (3 minutes fixes)
                    if (ligne.TempsEntreArrets == null || ligne.TempsEntreArrets.Count == 0)
                    {
                        LigneService.GenererTempsEntreArrets(ligne, 3);
                    }

                    // CORRECTION : Générer les connexions dans les DEUX sens
                    // 1. Sens normal : arrêt i → arrêt i+1
                    for (int i = 0; i < arretsLigne.Count - 1; i++)
                    {
                        var arretDepart = arretsLigne[i];
                        var arretArrivee = arretsLigne[i + 1];

                        GenererHorairesEntreArrets(arretDepart, arretArrivee, ligne, i);
                    }

                    // 2. Sens inverse : arrêt i+1 → arrêt i  
                    for (int i = arretsLigne.Count - 1; i > 0; i--)
                    {
                        var arretDepart = arretsLigne[i];
                        var arretArrivee = arretsLigne[i - 1];

                        // Index pour les temps = même que le sens normal
                        int indexTemps = i - 1;
                        GenererHorairesEntreArrets(arretDepart, arretArrivee, ligne, indexTemps);
                    }

                    System.Diagnostics.Debug.WriteLine($"Ligne {ligne.NomLigne} : {arretsLigne.Count} arrêts traités (bidirectionnel)");
                }

                // Statistiques finales
                var nbArrets = _horairesCache.Count;
                var nbHoraires = _horairesCache.Values.Sum(h => h.Count);
                System.Diagnostics.Debug.WriteLine($"Cache initialisé : {nbArrets} arrêts, {nbHoraires} horaires");

                // Vérification que le cache n'est pas vide
                if (nbHoraires == 0)
                {
                    System.Diagnostics.Debug.WriteLine("ATTENTION : Le cache des horaires est vide ! Vérifier les données.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur initialisation cache : {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace : {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Méthode helper pour générer les horaires entre deux arrêts
        /// </summary>
        private void GenererHorairesEntreArrets(Arret arretDepart, Arret arretArrivee, Ligne ligne, int indexTemps)
        {
            try
            {
                // Initialiser le cache pour cet arrêt de départ s'il n'existe pas
                if (!_horairesCache.ContainsKey(arretDepart.IdArret))
                {
                    _horairesCache[arretDepart.IdArret] = new List<HoraireArret>();
                }

                // Calculer les horaires pour chacun de ces arrêts
                var horairesArretDepart = LigneService.GetHorairesPourArret(ligne, arretDepart);
                var horairesArretArrivee = LigneService.GetHorairesPourArret(ligne, arretArrivee);

                // Créer les horaires pour ce tronçon
                for (int j = 0; j < horairesArretDepart.Count && j < horairesArretArrivee.Count; j++)
                {
                    var heureDepart = horairesArretDepart[j];
                    var heureArrivee = horairesArretArrivee[j];

                    // Vérification de cohérence
                    if (heureArrivee > heureDepart)
                    {
                        _horairesCache[arretDepart.IdArret].Add(new HoraireArret
                        {
                            ArretDestinationId = arretArrivee.IdArret,
                            LigneId = ligne.IdLigne,
                            HeureDepart = heureDepart,
                            HeureArrivee = heureArrivee
                        });
                    }
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur calcul horaire ligne {ligne.NomLigne} tronçon {arretDepart.NomArret}→{arretArrivee.NomArret} : {ex.Message}");
            }
        }

        /// <summary>
        /// Obtient des statistiques sur le calculateur
        /// </summary>
        public string ObtenirStatistiques()
        {
            try
            {
                var nbArrets = _horairesCache.Count;
                var nbHoraires = _horairesCache.Values.Sum(h => h.Count);
                var nbLignes = _toutesLesLignes.Count;

                return $"Calculateur: {nbArrets} arrêts, {nbLignes} lignes, {nbHoraires} horaires en cache";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur statistiques : {ex.Message}");
                return "Statistiques non disponibles";
            }
        }
    }

    // Classes internes pour le calcul
    internal class NoeudDijkstra
    {
        public int ArretId { get; set; }
        public TimeSpan TempsTotal { get; set; }
        public TimeSpan HeureArrivee { get; set; }
        public List<EtapeTransport> CheminParcouru { get; set; } = new List<EtapeTransport>();
        public int NombreCorrespondances { get; set; }
    }

    internal class EtapeTransport
    {
        public int ArretDepartId { get; set; }
        public int ArretDestinationId { get; set; }
        public int LigneId { get; set; }
        public TimeSpan HeureDepart { get; set; }
        public TimeSpan HeureArrivee { get; set; }
    }

    internal class HoraireArret
    {
        public int ArretDestinationId { get; set; }
        public int LigneId { get; set; }
        public TimeSpan HeureDepart { get; set; }
        public TimeSpan HeureArrivee { get; set; }
    }

    internal class CheminTransport
    {
        public List<EtapeTransport> Etapes { get; set; } = new List<EtapeTransport>();
        public TimeSpan TempsTotal { get; set; }
        public int NombreCorrespondances { get; set; }
    }

    internal class ComparateurNoeudDijkstra : IComparer<NoeudDijkstra>
    {
        public int Compare(NoeudDijkstra x, NoeudDijkstra y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return 1;
            if (y == null) return -1;

            // Priorité : temps total, puis nombre de correspondances, puis ID arrêt, puis heure arrivée
            var compareTemps = x.TempsTotal.CompareTo(y.TempsTotal);
            if (compareTemps != 0) return compareTemps;

            var compareCorresp = x.NombreCorrespondances.CompareTo(y.NombreCorrespondances);
            if (compareCorresp != 0) return compareCorresp;

            var compareArret = x.ArretId.CompareTo(y.ArretId);
            if (compareArret != 0) return compareArret;

            return x.HeureArrivee.CompareTo(y.HeureArrivee);
        }
    }
}
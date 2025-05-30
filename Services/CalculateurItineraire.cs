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
        /// Algorithme de Dijkstra adapté aux transports en commun
        /// </summary>
        private List<CheminTransport> DijkstraTransport(Arret arretDepart, Arret arretDestination, ParametresRecherche parametres)
        {
            var chemins = new List<CheminTransport>();
            var noeudsVisites = new HashSet<string>();
            var filePriorite = new SortedSet<NoeudDijkstra>(new ComparateurNoeudDijkstra());

            // Initialiser avec le nœud de départ
            var noeudDepart = new NoeudDijkstra
            {
                ArretId = arretDepart.IdArret,
                TempsTotal = TimeSpan.Zero,
                HeureArrivee = parametres.HeureSouhaitee,
                CheminParcouru = new List<EtapeTransport>(),
                NombreCorrespondances = 0
            };

            filePriorite.Add(noeudDepart);

            while (filePriorite.Count > 0 && chemins.Count < ParametresRecherche.NombreMaxItineraires)
            {
                var noeudActuel = filePriorite.Min;
                filePriorite.Remove(noeudActuel);

                // Si on a atteint la destination
                if (noeudActuel.ArretId == arretDestination.IdArret)
                {
                    var chemin = new CheminTransport
                    {
                        Etapes = noeudActuel.CheminParcouru,
                        TempsTotal = noeudActuel.TempsTotal,
                        NombreCorrespondances = noeudActuel.NombreCorrespondances
                    };
                    chemins.Add(chemin);
                    continue;
                }

                // Éviter de revisiter le même état
                string cleNoeud = $"{noeudActuel.ArretId}_{noeudActuel.HeureArrivee.Ticks}_{noeudActuel.NombreCorrespondances}";
                if (noeudsVisites.Contains(cleNoeud))
                    continue;

                noeudsVisites.Add(cleNoeud);

                // Explorer les voisins
                ExplorerVoisins(noeudActuel, filePriorite, parametres);
            }

            return chemins;
        }

        /// <summary>
        /// Explore les arrêts voisins accessibles depuis le nœud actuel
        /// </summary>
        private void ExplorerVoisins(NoeudDijkstra noeudActuel, SortedSet<NoeudDijkstra> filePriorite, ParametresRecherche parametres)
        {
            // Obtenir les horaires disponibles à partir de cet arrêt
            var horairesDisponibles = ObtenirHorairesDepuis(noeudActuel.ArretId, noeudActuel.HeureArrivee, parametres);

            foreach (var horaire in horairesDisponibles)
            {
                // Vérifier les contraintes de correspondance
                if (noeudActuel.CheminParcouru.Count > 0)
                {
                    var derniereEtape = noeudActuel.CheminParcouru.Last();
                    var tempsAttente = horaire.HeureDepart - noeudActuel.HeureArrivee;

                    // Temps de correspondance insuffisant
                    if (tempsAttente < parametres.TempsCorrespondanceMin)
                        continue;

                    // Temps de correspondance trop long
                    if (tempsAttente > parametres.TempsCorrespondanceMax)
                        continue;

                    // Trop de correspondances
                    if (derniereEtape.LigneId != horaire.LigneId && 
                        noeudActuel.NombreCorrespondances >= parametres.NombreMaxCorrespondances)
                        continue;
                }

                // Créer le nouveau nœud
                var nouveauNoeud = new NoeudDijkstra
                {
                    ArretId = horaire.ArretDestinationId,
                    TempsTotal = noeudActuel.TempsTotal + (horaire.HeureArrivee - noeudActuel.HeureArrivee),
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

                // Vérifier les contraintes de temps
                if (nouveauNoeud.TempsTotal <= parametres.TempsMaxRecherche)
                {
                    filePriorite.Add(nouveauNoeud);
                }
            }
        }

        /// <summary>
        /// Obtient les horaires de départ disponibles depuis un arrêt à partir d'une heure donnée
        /// </summary>
        private List<HoraireArret> ObtenirHorairesDepuis(int idArret, TimeSpan heureMinimale, ParametresRecherche parametres)
        {
            if (!_horairesCache.TryGetValue(idArret, out var tousLesHoraires))
                return new List<HoraireArret>();

            return tousLesHoraires
                .Where(h => h.HeureDepart >= heureMinimale)
                .Where(h => h.HeureArrivee <= parametres.HeureSouhaitee.Add(parametres.TempsMaxRecherche))
                .OrderBy(h => h.HeureDepart)
                .Take(50) // Limiter pour éviter l'explosion combinatoire
                .ToList();
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

                    // Générer les temps entre arrêts si pas déjà fait
                    if (ligne.TempsEntreArrets == null || ligne.TempsEntreArrets.Count == 0)
                    {
                        LigneService.GenererTempsEntreArrets(ligne, 3); // 3 minutes par défaut entre arrêts
                    }

                    var horairesLigne = LigneService.GetHorairesDepart(ligne);
                    
                    for (int i = 0; i < arretsLigne.Count - 1; i++)
                    {
                        var arretDepart = arretsLigne[i];
                        var arretArrivee = arretsLigne[i + 1];

                        if (!_horairesCache.ContainsKey(arretDepart.IdArret))
                        {
                            _horairesCache[arretDepart.IdArret] = new List<HoraireArret>();
                        }

                        // Calculer les horaires pour ce tronçon
                        foreach (var heureDepart in horairesLigne)
                        {
                            try
                            {
                                var horairesArretDepart = LigneService.GetHorairesPourArret(ligne, arretDepart);
                                var horairesArretArrivee = LigneService.GetHorairesPourArret(ligne, arretArrivee);

                                var heureArrivee = horairesArretArrivee.FirstOrDefault(h => h > heureDepart);

                                if (heureArrivee != TimeSpan.Zero)
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
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Erreur calcul horaire ligne {ligne.NomLigne} : {ex.Message}");
                            }
                        }
                    }
                }

                var nbArrets = _horairesCache.Count;
                var nbHoraires = _horairesCache.Values.Sum(h => h.Count);
                System.Diagnostics.Debug.WriteLine($"Cache initialisé : {nbArrets} arrêts, {nbHoraires} horaires");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur initialisation cache : {ex.Message}");
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
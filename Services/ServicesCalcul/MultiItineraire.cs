using BiblioSysteme;
using Services.ServicesClasses;
using Services.ServicesCalcul;

namespace Services.CalculClasses
{
    /// <summary>
    /// Recherche multi-itinéraires - Trouve deux alternatives co   mplémentaires
    /// </summary>
    public static class RechercheMultiItineraires
    {
        /// <summary>
        /// Trouve 2 itinéraires alternatifs pour un trajet
        /// </summary>
        /// <param name="arretDepart">Arrêt de départ</param>
        /// <param name="arretDestination">Arrêt de destination</param>
        /// <param name="heureDepart">Heure de départ souhaitée</param>
        /// <returns>Liste de 2 itinéraires (ou moins si pas trouvés)</returns>
        public static List<Itineraire> TrouverDeuxItineraires(Arret arretDepart, Arret arretDestination, TimeSpan heureDepart)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== RECHERCHE MULTI-ITINÉRAIRES ===");
                System.Diagnostics.Debug.WriteLine($"De : {arretDepart.NomArret} vers {arretDestination.NomArret}");
                System.Diagnostics.Debug.WriteLine($"Départ souhaité : {heureDepart}");

                var itineraires = new List<Itineraire>();

                // Stratégie 1 : Recherche rapide (priorité vitesse)
                var parametresRapide = ParametreRechercheService.PourRechercheRapide(heureDepart);
                var itineraireRapide = RechercheItineraire.TrouverItineraire(arretDepart, arretDestination, parametresRapide);

                if (itineraireRapide != null)
                {
                    itineraireRapide.TypeItineraire = "Rapide";
                    itineraires.Add(itineraireRapide);
                    System.Diagnostics.Debug.WriteLine($"Itinéraire rapide trouvé : {itineraireRapide.TempsTotal}");
                }

                // Stratégie 2 : Recherche alternative (différente de la première)
                var itineraireAlternatif = TrouverItineraireAlternatif(arretDepart, arretDestination, heureDepart, itineraireRapide);

                if (itineraireAlternatif != null)
                {
                    itineraires.Add(itineraireAlternatif);
                    System.Diagnostics.Debug.WriteLine($"Itinéraire alternatif trouvé : {itineraireAlternatif.TempsTotal}");
                }

                // Trier par qualité (meilleur en premier)
                itineraires = TrierParQualite(itineraires, heureDepart);

                System.Diagnostics.Debug.WriteLine($"{itineraires.Count} itinéraire(s) trouvé(s)");
                return itineraires;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erreur recherche multi : {ex.Message}");
                return new List<Itineraire>();
            }
        }

        /// <summary>
        /// Trouve un itinéraire alternatif différent du premier
        /// </summary>
        private static Itineraire TrouverItineraireAlternatif(Arret arretDepart, Arret arretDestination, TimeSpan heureDepart, Itineraire itinerairePrincipal)
        {
            // Si pas d'itinéraire principal, essayer recherche confortable
            if (itinerairePrincipal == null)
            {
                var parametresConfort = ParametreRechercheService.PourRechercheConfortable(heureDepart);
                var itineraire = RechercheItineraire.TrouverItineraire(arretDepart, arretDestination, parametresConfort);
                if (itineraire != null)
                    itineraire.TypeItineraire = "Confortable";
                return itineraire;
            }

            // Analyser l'itinéraire principal pour choisir la meilleure alternative
            var typeAlternatif = DeterminerTypeAlternatif(itinerairePrincipal);

            System.Diagnostics.Debug.WriteLine($"Type alternatif choisi : {typeAlternatif}");

            ParametresRecherche parametresAlternatifs = typeAlternatif switch
            {
                TypeRecherche.MoinsCorrespondances => ParametreRechercheService.PourRechercheDirecte(heureDepart),
                TypeRecherche.PlusConfortable => ParametreRechercheService.PourRechercheConfortable(heureDepart),
                _ => ParametreRechercheService.PourRechercheDirecte(heureDepart)
            };

            var itineraireAlternatif = RechercheItineraire.TrouverItineraire(arretDepart, arretDestination, parametresAlternatifs);

            if (itineraireAlternatif != null)
            {
                // Vérifier que l'alternative est suffisamment différente
                if (CalculCout.SontSuffissammentDifferents(itinerairePrincipal, itineraireAlternatif))
                {
                    itineraireAlternatif.TypeItineraire = typeAlternatif switch
                    {
                        TypeRecherche.MoinsCorrespondances => "Direct",
                        TypeRecherche.PlusConfortable => "Confortable",
                        _ => "Alternatif"
                    };
                    return itineraireAlternatif;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ Itinéraire alternatif trop similaire, ignoré");

                    // Essayer une 3ème stratégie : décaler l'heure de départ (ce qui est fait généralement...)
                    return TrouverAvecDecalageHoraire(arretDepart, arretDestination, heureDepart, itinerairePrincipal);
                }
            }

            return null;
        }

        /// <summary>
        /// Détermine le meilleur type d'alternative selon l'itinéraire principal
        /// </summary>
        private static TypeRecherche DeterminerTypeAlternatif(Itineraire itinerairePrincipal)
        {
            if (itinerairePrincipal?.Etapes == null)
                return TypeRecherche.MoinsCorrespondances;

            // Si l'itinéraire principal a beaucoup de correspondances, proposer un direct
            if (itinerairePrincipal.NombreCorrespondances >= 2)
            {
                return TypeRecherche.MoinsCorrespondances;
            }

            // Si l'itinéraire principal est déjà direct, proposer un confortable
            if (itinerairePrincipal.NombreCorrespondances <= 1)
            {
                return TypeRecherche.PlusConfortable;
            }

            // Par défaut, recherche directe
            return TypeRecherche.MoinsCorrespondances;
        }



        /// <summary>
        /// Trouve un itinéraire en décalant l'heure de départ
        /// </summary>
        private static Itineraire TrouverAvecDecalageHoraire(Arret arretDepart, Arret arretDestination, TimeSpan heureDepart, Itineraire itinerairePrincipal)
        {
            // Essayer avec +15 minutes
            var heureDecalee = heureDepart.Add(TimeSpan.FromMinutes(15));
            var parametresDecales = ParametreRechercheService.PourRechercheRapide(heureDecalee);

            var itineraireDecale = RechercheItineraire.TrouverItineraire(arretDepart, arretDestination, parametresDecales);

            if (itineraireDecale != null && CalculCout.SontSuffissammentDifferents(itinerairePrincipal, itineraireDecale))
            {
                itineraireDecale.TypeItineraire = "Départ décalé";
                System.Diagnostics.Debug.WriteLine($"✅ Itinéraire avec départ décalé trouvé : +15min");
                return itineraireDecale;
            }

            // Essayer avec -10 minutes si possible
            if (heureDepart.TotalMinutes >= 10)
            {
                heureDecalee = heureDepart.Subtract(TimeSpan.FromMinutes(10));
                parametresDecales = ParametreRechercheService.PourRechercheRapide(heureDecalee);

                itineraireDecale = RechercheItineraire.TrouverItineraire(arretDepart, arretDestination, parametresDecales);

                if (itineraireDecale != null && CalculCout.SontSuffissammentDifferents(itinerairePrincipal, itineraireDecale))
                {
                    itineraireDecale.TypeItineraire = "Départ anticipé";
                    System.Diagnostics.Debug.WriteLine($"✅ Itinéraire avec départ anticipé trouvé : -10min");
                    return itineraireDecale;
                }
            }

            return null;
        }

        /// <summary>
        /// Trie les itinéraires par qualité (meilleur en premier)
        /// </summary>
        private static List<Itineraire> TrierParQualite(List<Itineraire> itineraires, TimeSpan heureDepart)
        {
            if (itineraires.Count <= 1)
                return itineraires;

            // Critères de qualité par priorité :
            // 1. Temps total de trajet
            // 2. Nombre de correspondances (moins = mieux)
            // 3. Proximité avec l'heure souhaitée

            return itineraires.OrderBy(i => CalculCout.CalculerScoreQualite(i, heureDepart)).ToList();
        }
    }
}
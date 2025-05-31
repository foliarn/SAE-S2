using System;
using System.Collections.Generic;
using System.Linq;

namespace BiblioSysteme
{
    /// <summary>
    /// Représente un nœud (arrêt) dans le graphe de transport
    /// </summary>
    public class Noeud
    {
        public Arret ArretNoeud { get; set; }
        public List<Arete> AretesSortantes { get; set; }

        // Propriétés pour l'algorithme de Dijkstra
        public double CoutMinimal { get; set; }
        public Noeud Precedent { get; set; }
        public bool Visite { get; set; }
        public Arete AretePrecedente { get; set; } // Pour connaître la ligne utilisée
        public TimeSpan HeureArrivee { get; set; }

        public Noeud(int idArret, string nomArret)
        {
            ArretNoeud = new Arret(idArret, nomArret);
            ArretNoeud.NomArret = nomArret;
            AretesSortantes = new List<Arete>();
            ReinitialiserDijkstra();
        }

        /// <summary>
        /// Remet à zéro les propriétés de Dijkstra pour une nouvelle recherche
        /// </summary>
        public void ReinitialiserDijkstra()
        {
            CoutMinimal = double.MaxValue;
            Precedent = null;
            Visite = false;
            AretePrecedente = null;
        }
    }

    /// <summary>
    /// Représente une arête (connexion) entre deux arrêts
    /// </summary>
    public class Arete
    {
        public Noeud NoeudDepart { get; set; }
        public Noeud NoeudArrivee { get; set; }
        public Ligne LigneUtilisee { get; set; }
        public int Poids { get; set; } // Temps en minutes
        public bool EstCorrespondance { get; set; }

        public Arete(Noeud depart, Noeud arrivee, Ligne ligne, int poids, bool estCorrespondance = false)
        {
            NoeudDepart = depart;
            NoeudArrivee = arrivee;
            LigneUtilisee = ligne;
            Poids = poids;
            EstCorrespondance = estCorrespondance;
        }
    }

    /// <summary>
    /// Graphe de transport en commun - Structure de données pure
    /// </summary>
    public class Graphe
    {
        public Dictionary<int, Noeud> Noeuds { get; set; }
        public List<Arete> Aretes { get; set; }

        public Graphe()
        {
            Noeuds = new Dictionary<int, Noeud>();
            Aretes = new List<Arete>();
        }
    }
}
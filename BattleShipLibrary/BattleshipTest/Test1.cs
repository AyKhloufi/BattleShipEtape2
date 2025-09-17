using BattleShipLibrary;
using System.Reflection;

namespace BattleshipTest
{
    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        public void PlacerBateau_DoîtStockerLesBonnesPositions()
        {
           
            //  Création une grille 4x4 et 1 bateau
            var parametres = new Settings(4, 4, 1);
            var jeu = new Battleship(parametres);
            var positionsAttendue = new List<(int, int)> { (1, 1), (1, 2) };

            
            // FR : Affectation manuelle des positions du bateau
            var champ = typeof(Battleship)
                .GetField("positionsBateau", BindingFlags.NonPublic | BindingFlags.Instance);
            champ.SetValue(jeu, positionsAttendue);

            // Récupération des positions 
            var positionsReelles = jeu.PositionsBateau;

            //  Vérification que les positions correspondent
            CollectionAssert.AreEqual(positionsAttendue, positionsReelles.ToList());
        }

        [TestMethod]
        public void EncoderDecoderPositions_DoîtDonnerLesMêmesPositions()
        {
            //  Création d'une liste de coordonnées à encoder
            var positionsOriginales = new List<(int, int)> { (1, 2), (3, 4) };

            // Encodage puis décodage
            string encode = BattleshipSerializer.EncodePositions(positionsOriginales);
            var decode = BattleshipSerializer.DecodePositions(encode);

            // Vérification que les valeurs originales et décodées sont identiques
            CollectionAssert.AreEqual(positionsOriginales, decode);
        }

        [TestMethod]
        public void AttaquerPosition_DoîtRetournerVrai_SiTouche()
        {
            // Préparer le jeu et placer un bateau 
            var parametres = new Settings(4, 4, 1);
            var jeu = new Battleship(parametres);
            var position = (1, 1);
            var positionsAttendue = new List<(int, int)> { position };

            var champPositions = typeof(Battleship)
                .GetField("positionsBateau", BindingFlags.NonPublic | BindingFlags.Instance);
            champPositions.SetValue(jeu, positionsAttendue);

            //  Définir manuellement le symbole du bateau 
            var champGrille = typeof(Battleship)
                .GetField("grille", BindingFlags.NonPublic | BindingFlags.Instance);
            var grille = (char[,])champGrille.GetValue(jeu);
            grille[1, 1] = '■';

            //  Attaquer la position du bateau
            var resultat = jeu.AttaquerPosition(1, 1);

            //  Vérifier que l'attaque a réussi (true)
            Assert.IsTrue(resultat);
        }

        [TestMethod]
        public void AttaquerPosition_DoîtRetournerFaux_SiManqué()
        {
            // Le joueur attaque une case vide
            var parametres = new Settings(4, 4, 1);
            var jeu = new Battleship(parametres);

            var resultat = jeu.AttaquerPosition(0, 0);

            //  Doit retourner false car il n'y a pas de bateau
            Assert.IsFalse(resultat);
        }

        [TestMethod]
        public void EstCoule_DoîtRetournerVrai_SiToutesPartiesTouchees()
        {
            // Définir deux parties d’un bateau
            var parametres = new Settings(4, 4, 1);
            var jeu = new Battleship(parametres);
            var positions = new List<(int, int)> { (0, 0), (0, 1) };

            var champPositions = typeof(Battleship)
                .GetField("positionsBateau", BindingFlags.NonPublic | BindingFlags.Instance);
            champPositions.SetValue(jeu, positions);

            //Toutes les parties du bateau sont marquées comme "touchées"
            var champGrille = typeof(Battleship)
                .GetField("grille", BindingFlags.NonPublic | BindingFlags.Instance);
            var grille = (char[,])champGrille.GetValue(jeu);
            grille[0, 0] = '#';
            grille[0, 1] = '#';

            //Vérifier que le bateau est considéré comme coulé
            Assert.IsTrue(jeu.EstCoule());
        }

        [TestMethod]
        public void MettreAJourResultatAttaque_DoîtModifierCorrectementGrilleAdverse()
        {
            //Simuler une attaque réussie contre l’adversaire
            var parametres = new Settings(4, 4, 1);
            var jeu = new Battleship(parametres);

            jeu.MettreAJourResultatAttaque(0, 0, true);

            //  Accéder à la grille de l'adversaire 
            var champGrilleAdverse = typeof(Battleship)
                .GetField("grilleAdversaire", BindingFlags.NonPublic | BindingFlags.Instance);
            var grilleAdverse = (char[,])champGrilleAdverse.GetValue(jeu);

            // Vérifier que la case est marquée comme touchée
            Assert.AreEqual('#', grilleAdverse[0, 0]);
        }
    }
}

using BattleShipLibrary;
using System;

namespace test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //var game1 = new Battleship(4, 4);
            //var game2 = new Battleship(4, 4);
            //Console.WriteLine("=== Joueur 1 place son bateau ===");
            //game1.AfficherGrilleJoueur();
            //var pos1 = game1.PlacerBateau();

            //Console.WriteLine("=== Joueur 2 place son bateau ===");
            //game2.AfficherGrilleJoueur();
            //var pos2 = game2.PlacerBateau();

           

            //Console.Clear();
            //Console.WriteLine("=== Début du jeu (10 coups chacun) ===");

            //for (int turn = 1; turn <= 10; turn++)
            //{
            //    Console.WriteLine($"\n----- Tour {turn} -----");

            //    Console.WriteLine("\nJoueur 1 attaque :");
            //    var attaque1 = game1.Attaquer();
            //    string msg1 = BattleshipSerializer.EncodeAttaque(attaque1);
            //    var cible1 = BattleshipSerializer.DecodeAttaque(msg1);

            //    bool resultat1 = game2.AttaquerPosition(cible1.Item1, cible1.Item2);
            //    game1.MettreAJourResultatAttaque(cible1.Item1, cible1.Item2, resultat1);

            //    Console.WriteLine("\nJoueur 2 attaque :");
            //    var attaque2 = game2.Attaquer();
            //    string msg2 = BattleshipSerializer.EncodeAttaque(attaque2);
            //    var cible2 = BattleshipSerializer.DecodeAttaque(msg2);

            //    bool resultat2 = game1.AttaquerPosition(cible2.Item1, cible2.Item2);
            //    game2.MettreAJourResultatAttaque(cible2.Item1, cible2.Item2, resultat2);

            //    Console.WriteLine("\n=== État des plateaux après ce tour ===");
            //    Console.WriteLine("\nPlateau Joueur 1 :");
            //    game1.AfficherGrilleJoueur();
            //    game1.AfficherGrilleAdversaire();

            //    Console.WriteLine("\nPlateau Joueur 2 :");
            //    game2.AfficherGrilleJoueur();
            //    game2.AfficherGrilleAdversaire();

            //    // Stop if someone sunk
            //    if (game1.EstCoule())
            //    {
            //        Console.ForegroundColor = ConsoleColor.Red;
            //        Console.WriteLine("\n>>> Joueur 1 a perdu, bateau coulé !");
            //        break;
            //    }
            //    if (game2.EstCoule())
            //    {
            //        Console.ForegroundColor = ConsoleColor.Red;
            //        Console.WriteLine("\n>>> Joueur 2 a perdu, bateau coulé !");
            //        break;
            //    }
            //    Console.ResetColor();
            //}

            //Console.WriteLine("\n=== Fin de la partie ===");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
namespace BattleShipLibrary
{
    public class Battleship
    {

        private char[,] grille, grilleAdversaire;

        private List<(int, int)> positionsBateau;
        private List<(int, int)> positionsBateauAdversaire;
        private const char TOUCHER = '#', MANQUER = '*', BATEAU = '■', VIDE = '~';
        public Settings Setting { get; set; }

        public Battleship(Settings settings)
        {
            this.Setting = settings;

            grille = new char[Setting.HauteurTableau, Setting.LargeurTableau];
            grilleAdversaire = new char[Setting.HauteurTableau, Setting.LargeurTableau];

            for (int i = 0; i < Setting.HauteurTableau; i++)
                for (int j = 0; j < Setting.LargeurTableau; j++)
                {
                    grille[i, j] = VIDE;
                    grilleAdversaire[i, j] = VIDE;
                }

            positionsBateau = new List<(int, int)>();
            positionsBateauAdversaire = new List<(int, int)>();
        }

        public IReadOnlyList<(int, int)> PositionsBateau => positionsBateau;
        public IReadOnlyList<(int, int)> PositionsBateauAdversaire => positionsBateauAdversaire;

        private void AfficherGrille(char[,] plateau, bool isOpponent)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(" y\\x");
            for (int j = 1; j <= Setting.LargeurTableau; j++)
                Console.Write($"[{j,2}]");
            Console.WriteLine();

            for (int i = 0; i < Setting.HauteurTableau; i++)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"[{i + 1,2}]");
                for (int j = 0; j < Setting.LargeurTableau; j++)
                {
                    char c = plateau[i, j];
                    switch (c)
                    {
                        case VIDE: Console.ForegroundColor = ConsoleColor.Blue; break;
                        case BATEAU: Console.ForegroundColor = ConsoleColor.Green; break;
                        case TOUCHER: Console.ForegroundColor = ConsoleColor.DarkRed; break;
                        case MANQUER: Console.ForegroundColor = ConsoleColor.Red; break;
                        default: Console.ForegroundColor = ConsoleColor.White; break;
                    }
                    if (isOpponent && plateau[i, j] == BATEAU)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write($"[ {VIDE}]");
                    }
                    else { Console.Write($"[ {c}]"); }
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine();
            }
        }


        public void AfficherGrilleJoueur()
        {
            Console.WriteLine("\nTon plateau : ");
            AfficherGrille(grille, false);
        }


        public void AfficherGrilleAdversaire()
        {
            Console.WriteLine("\nPlateau de l'adversaire : ");
            AfficherGrille(grilleAdversaire, true);
        }
        private (int, int) DemanderPosition()
        {
            int x, y;
            while (true)
            {
                Console.Write("Entrez une position (format x.y) : ");
                string input = Console.ReadLine() ?? "";

                try
                {
                    string[] parts = input.Split('.');
                    if (parts.Length != 2)
                        throw new Exception();


                    x = int.Parse(parts[0]) - 1;
                    y = int.Parse(parts[1]) - 1;


                    if (x < 0 || x >= Setting.LargeurTableau || y < 0 || y >= Setting.HauteurTableau)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Position hors de la grille, réessayez.");
                        Console.ResetColor();
                    }
                    else
                    {
                        return (y, x);
                    }
                }
                catch
                {

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Format invalide, utilisez x.y.");
                    Console.ResetColor();
                }
            }
        }
       
    
        public List<(int, int)> PlacerBateau()
        {
            Console.WriteLine("Choisir la première case :");
            AfficherGrilleJoueur();
            var (ax, ay) = DemanderPosition();

            int bx = -1, by = -1;
            while (true)
            {
                Console.WriteLine("Choisir la deuxième case:");
                var pos = DemanderPosition();
                bx = pos.Item1;
                by = pos.Item2;


                bool estAdjacent =
                (Math.Abs(ax - bx) == 1 && ay == by) ||
                (Math.Abs(ay - by) == 1 && ax == bx);

                if (estAdjacent)
                {
                    break;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("La deuxième case doit être voisine horizontalement ou verticalement de la première.");
                    Console.ResetColor();
                }
            }

            grille[ax, ay] = BATEAU;
            grille[bx, by] = BATEAU;
            positionsBateau = new List<(int, int)> { (ax, ay), (bx, by) };

            Console.Clear();
            Console.WriteLine("Bateau placé");
            AfficherGrilleJoueur();


            return new List<(int, int)> { (ax, ay), (bx, by) };
        }

        public void PositionsBateauAdversaireEnBoard(List<(int, int)> positions)
        {
            positionsBateauAdversaire = positions;
            foreach (var (x, y) in positions)
                grilleAdversaire[x, y] = BATEAU;
        }

        public (int, int) Attaquer()
        {
            Console.WriteLine("Choisissez une case à attaquer : ");
            (int, int) coords = DemanderPosition();
            if (grilleAdversaire[coords.Item1, coords.Item2] != VIDE && grilleAdversaire[coords.Item1, coords.Item2] != BATEAU)
            {
                do
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Position {coords.Item1 + 1}.{coords.Item2 + 1} déjà attaquée ! Choisissez une autre case.");
                    Console.ResetColor();
                    coords = DemanderPosition();

                } while (grilleAdversaire[coords.Item1, coords.Item2] != VIDE && grilleAdversaire[coords.Item1, coords.Item2] != BATEAU);
            }
            
            return coords;
        }

        public bool AttaquerPosition(int x, int y)
        {
            while (true)
            {
                if (grille[x, y] == BATEAU)
                {
                    grille[x, y] = TOUCHER;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Touché!");
                    Console.ResetColor();
                    return true;
                }
                else if (grille[x, y] == VIDE)
                {
                    grille[x, y] = MANQUER;
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"Manqué");
                    Console.ResetColor();
                    return false;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Position {x + 1}.{y + 1} déjà attaquée ! Choisissez une autre case.");
                    Console.ResetColor();

                    var (ax, ay) = DemanderPosition();
                    x = ax;
                    y = ay;
                }
            }
        }

        public bool AttaquerPositionAdverse(int x, int y)
        {
            while (true)
            {
                if (grilleAdversaire[x, y] == BATEAU)
                {
                    grilleAdversaire[x, y] = TOUCHER;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Touché!");
                    Console.ResetColor();
                    return true;
                }
                else if (grilleAdversaire[x, y] == VIDE)
                {
                    grilleAdversaire[x, y] = MANQUER;
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"Manqué");
                    Console.ResetColor();
                    return false;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Position {x + 1}.{y + 1} déjà attaquée ! Choisissez une autre case.");
                    Console.ResetColor();

                    var (ax, ay) = DemanderPosition();
                    x = ax;
                    y = ay;
                }
            }
        }

        public void MettreAJourResultatAttaque(int x, int y, bool estTouche)
        {
            if (estTouche)
            {
                grilleAdversaire[x, y] = TOUCHER;
            }
            else
            {
                grilleAdversaire[x, y] = MANQUER;
            }
            Console.ResetColor();
        }


        public bool EstCoule()
        {
            foreach (var (x, y) in positionsBateau)
            {
                if (grille[x, y] != TOUCHER)
                    return false;
            }
            return true;
        }


        public void ClearBattleShip()
        {
            positionsBateau = new List<(int, int)>();
            positionsBateauAdversaire = new List<(int, int)>();


            for (int i = 0; i < Setting.HauteurTableau; i++)
                for (int j = 0; j < Setting.LargeurTableau; j++)
                {
                    grille[i, j] = VIDE;
                    grilleAdversaire[i, j] = VIDE;
                }
        }

        public static Settings DemanderSetting()
        {
            Settings settings = new Settings();
            bool valide = false;
            int largeur = 0;
            int hauteur = 0;
            int nbBateaux;

            Console.Clear();
            Console.WriteLine("Quel est la largeur du tableau : ");

            while(!int.TryParse(Console.ReadLine(), out largeur) || largeur < 4 || largeur > 12)
            {
                Console.Clear();
                Console.WriteLine("Erreur : La largeur doit etre comprise entre 4 et 12 !");
                Console.WriteLine("Quel est la largeur du tableau : ");
            }

            Console.Clear();
            Console.WriteLine("Quel est la hauteur du tableau : ");

            while (!int.TryParse(Console.ReadLine(), out hauteur) || hauteur < 4 || hauteur > 12)
            {
                Console.Clear();
                Console.WriteLine("Erreur : La hauteur doit etre comprise entre 4 et 12 !");
                Console.WriteLine("Quel est la hauteur du tableau : ");
            } 

            //valide = false;
            //while (!valide)
            //{
            //    Console.Clear();
            //    Console.WriteLine("Quel est le nombre de bateaux : ");
            //    if (int.TryParse(Console.ReadLine(), out nbBateaux))
            //    {
            //        valide = true;
            //    }
            //}

            settings.HauteurTableau = hauteur;
            settings.LargeurTableau = largeur;
            return settings;
        }

    }


    public static class BattleshipSerializer
    {
        public static string EncodePositions(List<(int, int)> positions)
        {
            return string.Join(";", positions.Select(p => $"{p.Item1}.{p.Item2}"));
        }

        public static List<(int, int)> DecodePositions(string data)
        {
            var positions = new List<(int, int)>();
            var parts = data.Split(';');
            foreach (var part in parts)
            {
                var coords = part.Split('.');
                if (coords.Length == 2 &&
                    int.TryParse(coords[0], out int x) &&
                    int.TryParse(coords[1], out int y))
                {
                    positions.Add((x, y));
                }
            }
            return positions;
        }



       public static string EncodeAttaque((int, int) positions)
       {
            return $"{positions.Item1}.{positions.Item2}";
       }

        public static (int, int) DecodeAttaque(string data)
        {
            var coords = data.Split('.');
            if (coords.Length == 2 &&
                int.TryParse(coords[0], out int x) &&
                int.TryParse(coords[1], out int y))
            {
                return (x, y);
            }
            throw new FormatException("Format d'attaque invalide");
        }
    }
}

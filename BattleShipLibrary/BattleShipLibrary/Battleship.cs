using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
namespace BattleShipLibrary
{
    public class Battleship
    {

        private char[,] grille, grilleAdversaire;

        private List<(int, int)> positionsBateau;
        private List<(int, int)> positionsBateauAdversaire;
        private const char TOUCHER = '#', MANQUER = '*', BATEAU = '■', VIDE = '~';
        public Settings settings { get; set; }

        public Battleship(Settings settings)
        {
            this.settings = settings;

            grille = new char[settings.HauteurTableau, settings.LargeurTableau];
            grilleAdversaire = new char[settings.HauteurTableau, settings.LargeurTableau];

            for (int i = 0; i < settings.HauteurTableau; i++)
                for (int j = 0; j < settings.LargeurTableau; j++)
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
            for (int j = 1; j <= settings.LargeurTableau; j++)
                Console.Write($"[{j,2}]");
            Console.WriteLine();

            for (int i = 0; i < settings.HauteurTableau; i++)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"[{i + 1,2}]");
                for (int j = 0; j < settings.LargeurTableau; j++)
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


                    if (x < 0 || x >= settings.LargeurTableau || y < 0 || y >= settings.HauteurTableau)
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
            List<(int, int)> toutesLesPositions = new List<(int, int)>();

            for (int i = 0; i < settings.NumberOfShip; i++)
            {
                FormShipEnum formShip = settings.FormShips[i];

                switch (formShip)
                {
                    case FormShipEnum.L:
                        positionsBateau.AddRange(PlacerBateauL());
                        break;
                    case FormShipEnum.I:
                        positionsBateau.AddRange(PlacerBateauI());
                        break;
                    case FormShipEnum.X:
                        positionsBateau.AddRange(PlacerBateauX());
                        break;
                    case FormShipEnum.O:
                        positionsBateau.AddRange(PlacerBateauO());
                        break;
                    case FormShipEnum.T:
                        positionsBateau.AddRange(PlacerBateauP());
                        break;
                }

                // Ajouter les positions à la grille
                foreach (var pos in positionsBateau)
                {
                    grille[pos.Item1, pos.Item2] = BATEAU;
                }

                toutesLesPositions.AddRange(positionsBateau);

                Console.Clear();
                Console.WriteLine($"Bateau {formShip} placé");
                AfficherGrilleJoueur();
            }

            return toutesLesPositions;
        }

        private List<(int, int)> PlacerBateauL()
        {
            Console.WriteLine("Placement du bateau en forme de L (3 cases)");
            AfficherGrilleJoueur();

            while (true)
            {
                Console.WriteLine("Choisir la position du coin du L:");
                var (x, y) = DemanderPosition();

                Console.WriteLine("Choisir l'orientation du L:");
                Console.WriteLine("1. L normal (coin en haut-gauche)");
                Console.WriteLine("2. L tourné 90° (coin en bas-gauche)");
                Console.WriteLine("3. L tourné 180° (coin en bas-droite)");
                Console.WriteLine("4. L tourné 270° (coin en haut-droite)");

                if (int.TryParse(Console.ReadLine(), out int orientation) && orientation >= 1 && orientation <= 4)
                {
                    List<(int, int)> positions = new List<(int, int)>();

                    switch (orientation)
                    {
                        case 1: // L normal
                            positions = new List<(int, int)> { (x, y), (x + 1, y), (x, y + 1) };
                            break;
                        case 2: // L tourné 90°
                            positions = new List<(int, int)> { (x, y), (x - 1, y), (x, y + 1) };
                            break;
                        case 3: // L tourné 180°
                            positions = new List<(int, int)> { (x, y), (x - 1, y), (x, y - 1) };
                            break;
                        case 4: // L tourné 270°
                            positions = new List<(int, int)> { (x, y), (x + 1, y), (x, y - 1) };
                            break;
                    }

                    if (PositionsValides(positions))
                    {
                        return positions;
                    }
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Position ou orientation invalide. Réessayez.");
                Console.ResetColor();
            }
        }

        private List<(int, int)> PlacerBateauI()
        {
            Console.WriteLine("Placement du bateau en forme de I (ligne droite de 4 cases)");
            AfficherGrilleJoueur();

            while (true)
            {
                Console.WriteLine("Choisir la première extrémité du bateau:");
                var (x, y) = DemanderPosition();

                Console.WriteLine("Choisir l'orientation:");
                Console.WriteLine("1. Horizontal (vers la droite)");
                Console.WriteLine("2. Vertical (vers le bas)");

                if (int.TryParse(Console.ReadLine(), out int orientation) && (orientation == 1 || orientation == 2))
                {
                    List<(int, int)> positions = new List<(int, int)>();

                    if (orientation == 1) // Horizontal
                    {
                        positions = new List<(int, int)> { (x, y), (x, y + 1), (x, y + 2), (x, y + 3) };
                    }
                    else // Vertical
                    {
                        positions = new List<(int, int)> { (x, y), (x + 1, y), (x + 2, y), (x + 3, y) };
                    }

                    if (PositionsValides(positions))
                    {
                        return positions;
                    }
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Position ou orientation invalide. Réessayez.");
                Console.ResetColor();
            }
        }

        private List<(int, int)> PlacerBateauX()
        {
            Console.WriteLine("Placement du bateau en forme de X (5 cases en croix)");
            AfficherGrilleJoueur();

            while (true)
            {
                Console.WriteLine("Choisir la position du centre du X:");
                var (x, y) = DemanderPosition();

                List<(int, int)> positions = new List<(int, int)>
        {
            (x, y),         // Centre
            (x - 1, y),     // Gauche
            (x + 1, y),     // Droite
            (x, y - 1),     // Haut
            (x, y + 1)      // Bas
        };

                if (PositionsValides(positions))
                {
                    return positions;
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Position invalide. Le X doit tenir entièrement dans la grille. Réessayez.");
                Console.ResetColor();
            }
        }

        private List<(int, int)> PlacerBateauO()
        {
            Console.WriteLine("Placement du bateau en forme de O (carré de 4 cases)");
            AfficherGrilleJoueur();

            while (true)
            {
                Console.WriteLine("Choisir la position du coin supérieur gauche du carré:");
                var (x, y) = DemanderPosition();

                List<(int, int)> positions = new List<(int, int)>
                {
                    (x, y),         // Coin supérieur gauche
                    (x + 1, y),     // Coin supérieur droit
                    (x, y + 1),     // Coin inférieur gauche
                    (x + 1, y + 1)  // Coin inférieur droit
                };

                if (PositionsValides(positions))
                {
                    return positions;
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Position invalide. Le carré doit tenir entièrement dans la grille. Réessayez.");
                Console.ResetColor();
            }
        }

        private List<(int, int)> PlacerBateauP()
        {
            Console.WriteLine("Placement du bateau en forme de T (4 cases)");
            AfficherGrilleJoueur();

            while (true)
            {
                Console.WriteLine("Choisir la position du coin inférieur gauche du T:");
                var (x, y) = DemanderPosition();

                Console.WriteLine("Choisir l'orientation du T:");
                Console.WriteLine("1. T normal (ouverture vers la haut)");
                Console.WriteLine("2. T tourné 90° (ouverture vers le gauche)");
                Console.WriteLine("3. T tourné 180° (ouverture vers la bas)");
                Console.WriteLine("4. T tourné 270° (ouverture vers le droite)");

                if (int.TryParse(Console.ReadLine(), out int orientation) && orientation >= 1 && orientation <= 4)
                {
                    List<(int, int)> positions = new List<(int, int)>();

                    switch (orientation)
                    {
                        case 1: // P normal
                            positions = new List<(int, int)> { (x, y), (x, y - 1), (x, y - 2), (x + 1, y - 1) };
                            break;
                        case 2: // P tourné 90°
                            positions = new List<(int, int)> { (x, y), (x + 1, y), (x + 2, y), (x + 1, y + 1) };
                            break;
                        case 3: // P tourné 180°
                            positions = new List<(int, int)> { (x, y), (x, y + 1), (x, y + 2), (x - 1, y + 1) };
                            break;
                        case 4: // P tourné 270°
                            positions = new List<(int, int)> { (x, y), (x - 1, y), (x - 2, y), (x - 1, y - 1) };
                            break;
                    }

                    if (PositionsValides(positions))
                    {
                        return positions;
                    }
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Position ou orientation invalide. Réessayez.");
                Console.ResetColor();
            }
        }

        private bool PositionsValides(List<(int, int)> positions)
        {
            foreach (var (x, y) in positions)
            {
                // Vérifier que la position est dans la grille
                if (x < 0 || x >= grille.GetLength(0) || y < 0 || y >= grille.GetLength(1))
                {
                    return false;
                }

                // Vérifier que la case n'est pas déjà occupée
                if (grille[x, y] == BATEAU)
                {
                    return false;
                }
            }

            return true;
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


            for (int i = 0; i < settings.HauteurTableau; i++)
                for (int j = 0; j < settings.LargeurTableau; j++)
                {
                    grille[i, j] = VIDE;
                    grilleAdversaire[i, j] = VIDE;
                }
            settings.InitFormsShipEnums();
        }

        public static Settings DemanderSetting()
        {
            int largeur = 0;
            int hauteur = 0;
            FormShipEnum formShip;

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
            Settings settings = new Settings(hauteur, largeur);



            for (int i = 1; i <= settings.NumberOfShip; i++)
            {
                Console.Clear();
                Console.WriteLine($"Quelle est la forme du bateau numéro {i} ({string.Join(", ", settings.FormShipEnums)}): ");
                while (!FormShipEnum.TryParse(Console.ReadLine(), out formShip) || !settings.FormShipEnums.Contains(formShip))
                {
                    Console.Clear();
                    Console.WriteLine($"Erreur : Le bateau doit etre d'une forme {string.Join(", ", settings.FormShipEnums)} !");
                    Console.WriteLine($"Quelle est la forme du bateau numéro {i} ({string.Join(", ", settings.FormShipEnums)}): ");
                }
                settings.AddFormShip(formShip);
            }
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

using Newtonsoft.Json;

namespace BattleShipLibrary
{
    public enum FormShipEnum
    {
        L,
        I,
        O, // carré
        X,
        T
    }
    public class Settings
    {
        public int HauteurTableau { get; set; }
        public int LargeurTableau { get; set; }
        public int NumberOfShip { get; set; }
        public List<FormShipEnum> FormShips { get; set; }
        public List<FormShipEnum> FormShipEnums { get; set; }
        public Dictionary<char, ConsoleColor> ColorsOfConsole { get; set; }
        public List<ConsoleColor> ConsoleColors { get; set; }
        private static readonly string ConfigPath = "battleship_config.json";

        public Settings() { }

        public Settings(int hauteurTableau, int largeurTableau)
        {
            FormShips = new List<FormShipEnum>();
            FormShipEnums = new List<FormShipEnum>();
            InitFormsShipEnums();
            HauteurTableau = hauteurTableau;
            LargeurTableau = largeurTableau;
            NumberOfShip = 3;
            ColorsOfConsole = new Dictionary<char, ConsoleColor>();
            // Si un fichier de configuration est ajouté, il faudra initialiser ColorsOfConsole avec les valeurs du fichier


            ConsoleColors = new List<ConsoleColor>();
            InitConsoleColors();
        }

        public bool AddFormShip(FormShipEnum formShip)
        {
            if (FormShips.Count < NumberOfShip && FormShipEnums.Contains(formShip))
            {
                FormShips.Add(formShip);
                FormShipEnums.Remove(formShip);
                return true;
            }
            return false;
        }

        public void InitFormsShipEnums()
        {
            FormShipEnums = Enum.GetValues(typeof(FormShipEnum)).Cast<FormShipEnum>().ToList();
        }

        public void InitConsoleColors()
        {
            ConsoleColors.Clear();
            ConsoleColors.Add(ConsoleColor.Green);
            ConsoleColors.Add(ConsoleColor.Yellow);
            ConsoleColors.Add(ConsoleColor.Blue);
            ConsoleColors.Add(ConsoleColor.Gray);
            ConsoleColors.Add(ConsoleColor.White);
            ConsoleColors.Add(ConsoleColor.DarkRed);
            ConsoleColors.Add(ConsoleColor.Magenta);
            ConsoleColors.Add(ConsoleColor.Cyan);
            ConsoleColors.Add (ConsoleColor.DarkYellow);
        }

        public void AddColorOfConsole(char key, ConsoleColor value)
        {
            ColorsOfConsole.Add(key, value);
            ConsoleColors.Remove(value);
        }

        public static void SaveSettings(Dictionary<char, ConsoleColor> colors)
        {
            try
            {
                string json = JsonConvert.SerializeObject(colors, Formatting.Indented);
                File.WriteAllText(ConfigPath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur de sauvegarde : {ex.Message}");
            }
        }

        public static Dictionary<char, ConsoleColor> LoadSettings()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    string json = File.ReadAllText(ConfigPath);
                    return JsonConvert.DeserializeObject<Dictionary<char, ConsoleColor>>(json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur de chargement : {ex.Message}");
            }
            return null;
        }

        public static bool Exists() => File.Exists(ConfigPath);

    }
}

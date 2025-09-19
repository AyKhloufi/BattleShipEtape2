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

    }
}

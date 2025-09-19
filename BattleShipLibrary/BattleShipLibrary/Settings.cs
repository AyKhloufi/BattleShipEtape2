using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public int HauteurTableau {  get; set; }
        public int LargeurTableau { get; set; }
        public int NumberOfShip { get; set; }
        public List<FormShipEnum> FormShips { get; set; }

        public List<FormShipEnum> FormShipEnums { get; set; }

        public Settings() { }

        public Settings(int hauteurTableau, int largeurTableau)
        {
            FormShips = new List<FormShipEnum>();
            FormShipEnums = new List<FormShipEnum>();
            InitFormsShipEnums();
            HauteurTableau = hauteurTableau;
            LargeurTableau = largeurTableau;
            NumberOfShip = 3;
        }

        public bool AddFormShip(FormShipEnum formShip)
        {
            if(FormShips.Count < NumberOfShip && FormShipEnums.Contains(formShip))
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

    }
}

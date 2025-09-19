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

        public Settings() { }

        public Settings(int hauteurTableau, int largeurTableau)
        {
            FormShips = new List<FormShipEnum>();
            HauteurTableau = hauteurTableau;
            LargeurTableau = largeurTableau;
            NumberOfShip = 3;
        }

        public bool AddFormShip(FormShipEnum formShip)
        {
            if(FormShips.Count < NumberOfShip)
            {
                FormShips.Add(formShip);
                return true;
            }
            return false;
        }
    }
}

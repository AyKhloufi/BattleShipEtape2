using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShipLibrary
{
    public class Settings
    {
        public int HauteurTableau {  get; set; }
        public int LargeurTableau { get; set; }
        public int NombreBateaux { get; set; }

        public Settings() { }

        public Settings(int hauteurTableau, int largeurTableau, int nombreBateaux)
        {
            HauteurTableau = hauteurTableau;
            LargeurTableau = largeurTableau;
            NombreBateaux = nombreBateaux;
        }
    }
}

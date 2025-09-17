using BattleShipLibrary;
using Serveur_Battleship.models;

namespace Serveur_Battleship
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                ServerSocket.StartServer();
                Thread.Sleep(1000);
            }                
        }
    }
}

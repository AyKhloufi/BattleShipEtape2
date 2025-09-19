using BattleShipLibrary;
using Client_Battleship.Model;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Text;

// C : coords du bateau : List<(int, int)>
// A : Attaquer : (int, int)
// R : Rejouer : bool
// V : Valider le coup adverse : bool + coords
// O : OK : string vide
// W : Tu as gagné : string vide 
// S : reception des settings

namespace Serveur_Battleship.models
{
    internal class ServerSocket
    {
        public static string data = null;
        public static Socket handler;
        public static Battleship battleship;
        public static bool replay = true;
        private static Socket listener;

        public static void StartServer()
        {
            replay = true;
            byte[] bytes = new byte[32];
            IPEndPoint ipClient = new IPEndPoint(IPAddress.Any, 443);

            if (listener != null)
            {
                try
                {
                    listener.Shutdown(SocketShutdown.Both);
                    listener.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la fermeture du listener: {ex.Message}");
                }
                listener = null;
            }

            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            try
            {
                listener.Bind(ipClient);
                listener.Listen(1);
                Console.Clear();
                Console.WriteLine("Serveur démarré et en attente de connexions...");

                while (replay)
                {
                    byte[] msg;
                    Console.WriteLine("Waiting for player...");

                    handler = listener.Accept();
                    Console.WriteLine("Player Found.");

                    // Traitement de la connexion
                    HandleClientConnection(bytes);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur serveur: {ex.Message}");
                Disconnect(handler, listener);
            }
            finally
            {
                // Nettoyer les ressources
                CleanupServer();
            }
        }

        private static void HandleClientConnection(byte[] bytes)
        {
            try
            {
                data = "";
                while (!data.Contains('|'))
                {
                    int bytesRec = handler.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                }
                data = data.Substring(0, data.IndexOf('|'));

                Message m = JsonConvert.DeserializeObject<Message>(data);
                Settings settings = JsonConvert.DeserializeObject<Settings>(m.LeMessage);

                m = new Message('O', string.Empty);
                byte[] msg = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(m) + "|");
                handler.Send(msg);

                battleship = new Battleship(settings);

                msg = PlacerBateau(battleship);
                handler.Send(msg);

                bool endConnection = false;
                while (!endConnection && replay)
                {
                    data = "";
                    while (!data.Contains('|'))
                    {
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    }
                    data = data.Substring(0, data.IndexOf('|'));

                    m = JsonConvert.DeserializeObject<Message>(data);

                    switch (m.Code)
                    {
                        case 'C':
                            Console.Clear();
                            battleship.AfficherGrilleAdversaire();
                            battleship.AfficherGrilleJoueur();
                            PlacerBateauAdverse(m.LeMessage);
                            break;
                        case 'A':
                            AttaquerAdverse(m.LeMessage);
                            if (!replay)
                            {
                                endConnection = true;
                            }
                            break;
                        case 'R':
                            Console.Clear();
                            battleship.ClearBattleShip();
                            msg = PlacerBateau(battleship);
                            handler.Send(msg);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du traitement du client: {ex.Message}");
            }
            finally
            {
                if (handler != null)
                {
                    try
                    {
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                    }
                    catch { }
                }
            }
        }

        public static void Disconnect(Socket handler, Socket listener)
        {
            try
            {
                if (listener != null)
                {
                    listener.Shutdown(SocketShutdown.Both);
                    listener.Close();
                }
            }
            catch { }

            try
            {
                if (handler != null)
                {
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch { }
        }

        private static void CleanupServer()
        {
            try
            {
                if (listener != null)
                {
                    listener.Close();
                    listener = null;
                }
            }
            catch { }
        }


        public static byte[] PlacerBateau(Battleship battleship)
        {
            List<(int, int)> coords = battleship.PlacerBateau();
            Message m = new Message('C', JsonConvert.SerializeObject(coords));
            return Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(m) + "|");
        }
        public static void PlacerBateauAdverse(string leMessage)
        {
            List<(int, int)> coords = JsonConvert.DeserializeObject<List<(int, int)>>(leMessage);
            battleship.PositionsBateauAdversaireEnBoard(coords);
        }

        public static string RecieveData(byte[] bytes)
        {
            data = "";
            while (!data.Contains('|'))
            {
                int bytesRec = handler.Receive(bytes);
                data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
            }
            return data.Substring(0, data.IndexOf('|'));
        }

        public static bool CoupEstValide((int, int) coordsAttack)
        {
            //Si oui out of bounds
            if (coordsAttack.Item1 < 0 || coordsAttack.Item2 < 0 ||
                coordsAttack.Item1 > battleship.settings.LargeurTableau || coordsAttack.Item2 > battleship.settings.HauteurTableau)
            {
                return false;
            }
            return true;
            
        }

        public static void AttaquerAdverse(string leMessage)
        {
            (int, int) coordsAttack = JsonConvert.DeserializeObject<(int, int)>(leMessage);
            bool valide = CoupEstValide(coordsAttack);

            Message m = new Message('V', JsonConvert.SerializeObject(false));
            bool touche= false;
            if (valide)
            {
                touche = battleship.AttaquerPosition(coordsAttack.Item1, coordsAttack.Item2);
                m = new Message('V', JsonConvert.SerializeObject(true));
            }
            byte[] msg = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(m) + "|");
            handler.Send(msg);

            byte[] bytes = new byte[32];
            data = "";
            while (!data.Contains('|'))
            {
                int bytesRec = handler.Receive(bytes);
                data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
            }
            data = data.Substring(0, data.IndexOf('|'));
            m = JsonConvert.DeserializeObject<Message>(data);


            if (battleship.EstCoule())
            {
                Console.WriteLine("Tu as perdu ...");
                m = new Message('W', "");
                msg = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(m) + "|");
                handler.Send(msg);
                data = "";
                while (!data.Contains('|'))
                {
                    int bytesRec = handler.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                }
                data = data.Substring(0, data.IndexOf('|'));
                m = JsonConvert.DeserializeObject<Message>(data);
                if (m.Code == 'R')
                {
                    bool rejouer = JsonConvert.DeserializeObject<bool>(m.LeMessage);
                    //le serveur a gagné et on sait si le client veut rejouer
                    if (rejouer)
                    {
                        Console.Clear();
                        battleship.ClearBattleShip();
                        m = new Message('R', "");
                        msg = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(m) + "|");
                        handler.Send(msg);

                        data = "";
                        while (!data.Contains('|'))
                        {
                            int bytesRec = handler.Receive(bytes);
                            data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        }
                        data = data.Substring(0, data.IndexOf('|'));
                        m = JsonConvert.DeserializeObject<Message>(data);

                        PlacerBateauAdverse(m.LeMessage);

                        msg = PlacerBateau(battleship);
                        handler.Send(msg);
                    }
                    else
                    {
                        replay = false;
                        return;
                    }

                }
            }
            else if (touche)
            {
                Console.Clear();
                battleship.AfficherGrilleAdversaire();
                battleship.AfficherGrilleJoueur();
                AttaquerAdverse(m.LeMessage);

            }
            else
            {
                Console.Clear();
                battleship.AfficherGrilleAdversaire();
                battleship.AfficherGrilleJoueur();
                Attaquer();
            }
        }

        public static void Attaquer()
        {
            (int, int) coordsAttack = battleship.Attaquer();

            Message m = new Message('A', JsonConvert.SerializeObject(coordsAttack));
            byte[] msg = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(m) + "|");
            int bytesSent = handler.Send(msg);

            byte[] bytes = new byte[32];
            data = "";
            while (!data.Contains('|'))
            {
                int bytesRec = handler.Receive(bytes);
                data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
            }
            data = data.Substring(0, data.IndexOf('|'));
            m = JsonConvert.DeserializeObject<Message>(data);

            if (m.Code == 'V')
            {
                if (JsonConvert.DeserializeObject<bool>(m.LeMessage))
                {

                    bool touche = battleship.AttaquerPositionAdverse(coordsAttack.Item1, coordsAttack.Item2);
                    if (touche)
                    {
                        Console.Clear();
                        battleship.AfficherGrilleAdversaire();
                        battleship.AfficherGrilleJoueur();
                        Attaquer();
                    }
                    else
                    {
                        Console.Clear();
                        battleship.AfficherGrilleAdversaire();
                        battleship.AfficherGrilleJoueur();
                        m = new Message('O', string.Empty);
                        msg = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(m) + "|");
                        handler.Send(msg);
                    }
                    
                }
                else
                {
                    //TODO tu as triché
                }
            }
            if (m.Code == 'R')
            {
                bool rejouer = JsonConvert.DeserializeObject<bool>(m.LeMessage);
                if (rejouer)
                {
                    Console.Clear();
                    battleship.ClearBattleShip();
                    m = new Message('R', "");
                    msg = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(m) + "|");
                    handler.Send(msg);

                    data = "";
                    while (!data.Contains('|'))
                    {
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    }
                    data = data.Substring(0, data.IndexOf('|'));
                    m = JsonConvert.DeserializeObject<Message>(data);

                    PlacerBateauAdverse(m.LeMessage);

                    msg = PlacerBateau(battleship);
                    handler.Send(msg);
                }
                else
                {
                    replay = false;
                    return;
                }

            }
        }

    }
}

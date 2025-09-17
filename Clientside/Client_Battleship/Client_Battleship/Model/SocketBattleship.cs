using BattleShipLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Newtonsoft.Json;
// 'C' envoie de coordonnées du bateau placé : List<(int, int)>
// 'A' Attaquer : (int, int)
// 'R' Rejouer seulement le client qui peut l'envoyer si le serveur l'envoie ca veut dire que la demande est accepté : bool
// 'V' Validation d'un coup : bool
// 'O' OK : string vide
// 'W' Le serveur nous dit que ses bateaux sont coulés : string vide
// 'S' Envoie des settings vers le server : Settings

//Ismael Harvey
namespace Client_Battleship.Model
{
    public static class SocketBattleship
    {
        static Battleship battleship = null;
    
        public static string data = null;
        public static (int, int) lastAttack;
        public static void StartClient()
        {
            byte[] bytes = new byte[16];
            while (true)
            {
                try
                {
                    Console.Clear();
                    Console.Write("Entrer l'adresse IP de l'hôte : ");
                    string ip = Console.ReadLine() ?? "";
                    IPHostEntry iPHostInfo = Dns.GetHostEntry(ip);
                    IPAddress iPAddress = iPHostInfo.AddressList.First(a => a.AddressFamily == AddressFamily.InterNetwork);
                    IPEndPoint endPoint = new(iPAddress, 443);

                    Socket sender = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                
                    try
                    {
                        sender.Connect(endPoint);

                        Console.WriteLine("La connexion à l'hôte est bien établie");

                        Message message = Setting();
                        byte[] msg = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(message) + "|");
                        sender.Send(msg);
                        data = "";
                        while (!data.Contains('|'))
                        {
                            int bytesRec = sender.Receive(bytes);
                            data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        }

                        message = PlaceShipMessage();
                        msg = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(message) + "|");
                        sender.Send(msg);
                        while (true)
                        {
                            data = "";

                            while (!data.Contains('|'))
                            {
                                int bytesRec = sender.Receive(bytes);
                                data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                            }
                            data = data.Substring(0, data.IndexOf("|"));
                            Message messageRec = JsonConvert.DeserializeObject<Message>(data);
                            Message sendMessage = new();

                            switch (messageRec.Code)
                            {
                                case 'C':
                                    Console.Clear();
                                    battleship.AfficherGrilleAdversaire();
                                    battleship.AfficherGrilleJoueur();
                                    PlaceEnnemyShip(messageRec);
                                    sendMessage = AttackMessage();
                                    break;
                                case 'A':
                                    sendMessage = ValidateAttackMessage(messageRec);
                                    if (battleship.EstCoule())
                                    {
                                        sendMessage = ReplayMessage();
                                        break;
                                    }
                                    Console.Clear();
                                    battleship.AfficherGrilleAdversaire();
                                    battleship.AfficherGrilleJoueur();
                                    break;
                                case 'V':
                                    sendMessage = OKMessage();
                                    break;
                                case 'O':
                                    Console.Clear();
                                    battleship.AfficherGrilleAdversaire();
                                    battleship.AfficherGrilleJoueur();
                                    sendMessage = AttackMessage();
                                    break;
                                case 'W':
                                    sendMessage = ReplayMessage();
                                    break;
                                case 'R':
                                    sendMessage = PlaceShipMessage();
                                    break;
                                default:
                                    break;
                            }
                            msg = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(sendMessage) + "|");
                            sender.Send(msg);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                } 
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        private static Message OKMessage()
        {
            battleship.AttaquerPositionAdverse(lastAttack.Item1, lastAttack.Item2);

            Console.Clear();
            battleship.AfficherGrilleAdversaire();
            battleship.AfficherGrilleJoueur();
            return new Message('O', string.Empty);
        }

        private static Message ReplayMessage()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Voulez-vous rejouer? [y/n]");
                string str = Console.ReadLine().ToLower();
                if (str == "y") 
                {
                    battleship.ClearBattleShip();
                    return new Message('R', JsonConvert.SerializeObject(true));
                }
                if (str == "n")
                {
                    return new Message('R', JsonConvert.SerializeObject(false));
                }
            }
        }

        private static Message AttackMessage()
        {
            (int, int) coords = battleship.Attaquer();
            lastAttack = coords;
            return new Message('A', JsonConvert.SerializeObject(coords));
        }
        private static Message PlaceShipMessage()
        {
            List<(int, int)> coords = battleship.PlacerBateau();
            return new Message('C', JsonConvert.SerializeObject(coords));
        }

        private static void PlaceEnnemyShip(Message message)
        {
            List<(int, int)> coords = JsonConvert.DeserializeObject<List<(int, int)>>(message.LeMessage);

            battleship.PositionsBateauAdversaireEnBoard(coords);
        }

        private static Message ValidateAttackMessage(Message message)
        {
            (int, int) coords = JsonConvert.DeserializeObject<(int, int)>(message.LeMessage);
            //Si oui out of bounds
            if(coords.Item1 < 0 || coords.Item2 < 0 ||
                coords.Item1 > battleship.Setting.LargeurTableau || coords.Item2 > battleship.Setting.HauteurTableau)
            {
                return new Message('V', JsonConvert.SerializeObject(false));
            }

            battleship.AttaquerPosition(coords.Item1, coords.Item2);
            
            return new Message('V', JsonConvert.SerializeObject(true));
        }

        private static Message Setting()
        {
            Settings setting = Battleship.DemanderSetting();
            battleship = new Battleship(setting);
            return new Message('S', JsonConvert.SerializeObject(battleship.Setting));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Battleship.Model
{
    public class Message
    {
        public char Code {  get; set; }
        public string LeMessage { get; set; } = string.Empty;

        public Message() { }

        public Message(char code, string leMessage)
        {
            this.Code = code;
            this.LeMessage = leMessage;
        }
    }
}

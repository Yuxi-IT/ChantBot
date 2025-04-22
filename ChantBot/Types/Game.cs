using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChantBot.Types
{
    public class Game
    {
        public long GameID { get; set; }
        public string StartDate { get; set; }
        public string[] Players { get; set; }
        public int GameType { get; set; }
        public int MinAmount { get; set; }
        public string GameResult { get; set; }

    }

}

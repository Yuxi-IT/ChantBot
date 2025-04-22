using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChantBot.Types
{
    public class LogEntry
    {
        public string Date { get; set; }
        public string Chat { get; set; }
        public string From { get; set; }
        public string Message { get; set; }
    }

    public class ErrorEntry
    {
        public string Date { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
    }

}

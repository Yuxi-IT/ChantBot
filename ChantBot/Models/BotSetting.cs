using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChantBot.Models
{
    public class BotSetting
    {
        public Own? Own { get; set; }
        public List<OfficalUser>? Admins { get; set; }
        public List<OfficalUser>? Customer { get; set; }
        public List<string>? Groups { get; set; }
        public string? WebApp { get; set; }
    }

    public class Own
    {
        public string? ID { get; set; }
        public string? Name { get; set; }
    }

    public class OfficalUser
    {
        public string? ID { get; set; }
        public string? Name { get; set; }
    }
}
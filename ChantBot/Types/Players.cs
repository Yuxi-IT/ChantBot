using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChantBot.Types
{
    public class Player
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string ID { get; set; }
        public long Amount { get; set; }
        public int Level { get; set; }
        public int VIPLevel { get; set; }
        public string[] Invites { get; set; }
        public string PaymentAddress { get; set; }
        public string LastCheckIn { get; set; }
        public string[] AmountChange { get; set; }
        public string RegDate { get; set; }
        public string InviteCode { get; set; }
        public string RegCode { get; set; } = "";
    }

}

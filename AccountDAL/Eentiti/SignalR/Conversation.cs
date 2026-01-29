using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Eentiti.SignalR
{
    public class Conversation: BaseEntity
    {
        public string ClientUserId { get; set; }
        public string AdminUserId { get; set; }
        public DateTime LastActivity { get; set; }
    }
}

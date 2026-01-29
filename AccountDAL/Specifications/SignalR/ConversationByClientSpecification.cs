using AccountDAL.Eentiti.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Specifications.SignalR
{
    public class ConversationByClientSpecification: BaseSpecification<Conversation>
    {
        public ConversationByClientSpecification(string clientId)
        : base(c => c.ClientUserId == clientId)
        {
          
        }
    }
}

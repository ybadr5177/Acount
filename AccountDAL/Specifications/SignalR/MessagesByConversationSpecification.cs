using AccountDAL.Eentiti.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Specifications.SignalR
{
    public class MessagesByConversationSpecification : BaseSpecification<Message>
    {
        public MessagesByConversationSpecification(int conversationId)
       : base(m => m.ConversationId == conversationId)
        {
            // الترتيب الزمني ضروري لعرض الشات بشكل صحيح
            AddOrderBy(m => m.Timestamp);
        }
    }
}

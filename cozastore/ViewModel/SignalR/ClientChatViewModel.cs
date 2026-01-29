using AccountDAL.Eentiti.SignalR;

namespace cozastore.ViewModel.SignalR
{
    public class ClientChatViewModel
    {
        public int ConversationId { get; set; }
        public string SenderId { get; set; }
        public IReadOnlyList<Message> Messages { get; set; }
    }
}

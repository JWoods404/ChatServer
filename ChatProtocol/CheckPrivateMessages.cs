using System;
using System.Collections.Generic;
using System.Text;

namespace ChatProtocol
{
    public class CheckPrivateMessages : IMessage
    {
        public List<DirectChatMessage> SavedMessages { get; set; }
        public DirectChatMessage SavedMessage { get; set; }
        public string Username { get; set; }
        public int MessageId
        {
            get { return 11; }
            set { }
        }
    }
}

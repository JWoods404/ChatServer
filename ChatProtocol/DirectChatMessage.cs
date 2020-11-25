using System;
using System.Collections.Generic;
using System.Text;

namespace ChatProtocol
{
    public class DirectChatMessage : IMessage
    {
        public string Content { get; set; }
        public string SessionId { get; set; }
        //public string Username { get; set; }
        public int UserId { get; set; }
        public int ToUserId { get; set; }

        private DateTime currentTime;
        public DateTime Now 
        { 
            get { return currentTime; }
            set { currentTime = value; }
        }

        public int MessageId
        {
            get { return 10; }
            set { }
        }
    }
}

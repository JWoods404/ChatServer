using System;
using System.Collections.Generic;
using System.Text;

namespace ChatProtocol
{
    public class RegisterNewUserMessage : IMessage
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Success { get; set; }
        public int Id { get; set; }
        public int MessageId 
        { 
            get { return 8; }
            set { }
        }
    }
}

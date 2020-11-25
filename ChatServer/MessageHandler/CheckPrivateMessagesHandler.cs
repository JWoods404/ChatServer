using ChatProtocol;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ChatServer.MessageHandler
{
    public class CheckPrivateMessagesHandler : IMessageHandler
    {
        public void Execute(Server server, TcpClient client, IMessage message)
        {
            var privateMessages = message as CheckPrivateMessages;
            privateMessages.SavedMessages = server.GetMessages();

            var user = server.GetUsers().Find(u => u.Username == privateMessages.Username);

            List<DirectChatMessage> messageList = server.GetMessages();
            if (messageList != null)
            {
                foreach (var savedMessage in messageList)
                {
                    if (savedMessage.ToUserId == user.Id)
                    {
                        privateMessages.SavedMessage = savedMessage;
                        var directChatJson = JsonSerializer.Serialize(privateMessages);
                        var chatMsg = System.Text.Encoding.UTF8.GetBytes(directChatJson);
                        client.GetStream().Write(chatMsg, 0, chatMsg.Length);
                        server.RemoveMessages(savedMessage);
                    }
                }
            }
        }
    }
}

using ChatProtocol;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;

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
                for (int i = 0; i < messageList.Count; i++)
                {
                    if (messageList[i].ToUserId == user.Id)
                    {
                        privateMessages.SavedMessage = messageList[i];
                        var directChatJson = JsonSerializer.Serialize(privateMessages);
                        var chatMsg = System.Text.Encoding.UTF8.GetBytes(directChatJson);
                        client.GetStream().Write(chatMsg, 0, chatMsg.Length);
                        server.RemoveMessages(messageList[i]);
                        i--;
                    }
                }
            }
        }
    }
}

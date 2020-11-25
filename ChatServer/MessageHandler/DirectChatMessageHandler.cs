using ChatProtocol;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ChatServer.MessageHandler
{
    public class DirectChatMessageHandler : IMessageHandler
    {
        public void Execute(Server server, TcpClient client, IMessage message)
        {
            var directChatMessage = message as DirectChatMessage;
            var fromUser = server.GetUsers().Find(u => u.SessionIds.Contains(directChatMessage.SessionId));
            var toUser = server.GetUsers().Find(u => u.Id.Equals(directChatMessage.ToUserId));

            if (fromUser != null)
            {
                directChatMessage.SessionId = string.Empty;
                directChatMessage.UserId = fromUser.Id;
                var json = JsonSerializer.Serialize(directChatMessage);
                var msg = System.Text.Encoding.UTF8.GetBytes(json);

                if (toUser.Clients.Count != 0)
                {
                    foreach (var toClient in toUser.Clients)
                    {
                        toClient.GetStream().Write(msg, 0, msg.Length);
                    }
                }
                else
                {
                    directChatMessage.Now = DateTime.Now;
                    server.SaveMessage(directChatMessage);
                }

            }
        }
    }
}

using ChatProtocol;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading;

namespace ChatServer.MessageHandler
{
    public class ConnectMessageHandler : IMessageHandler
    {
        public void Execute(Server server, TcpClient client, IMessage message)
        {
            var connectMessage = message as ConnectMessage;

            var authenticatedServerPassword = true;
            if (server.HasPassword())
            {
                authenticatedServerPassword = server.CheckPassword(connectMessage.ServerPassword);
            }

            var user = server.GetUsers().Find(u => u.Username == connectMessage.ClientUsername && u.Password == connectMessage.ClientPassword);
            var authenticatedUser = (user != null);

            var authenticated = authenticatedServerPassword && authenticatedUser;
            var connectResponseMessage = new ConnectResponseMessage();
            if (authenticated)
            {
                var sessionId = Guid.NewGuid().ToString();
                user.SessionIds.Add(sessionId);
                user.Clients.Add(client);
                connectResponseMessage.SessionId = sessionId;
                server.AddClient(client);
                Console.WriteLine("Client connected.");
                if (user.SessionIds.Count == 1)
                {
                    // Send user count to all clients (broadcast)
                    var userCountMessage = new UserCountMessage
                    {
                        UserCount = server.GetUsers().Count,
                        UserOnlineCount = server.GetUsers().Count(u => u.SessionIds.Count > 0),
                    };

                    var userCountMessageJson = JsonSerializer.Serialize(userCountMessage);
                    var userCountMessageBytes = System.Text.Encoding.UTF8.GetBytes(userCountMessageJson);

                    foreach (TcpClient remoteClient in server.GetClients())
                    {
                        remoteClient.GetStream().Write(userCountMessageBytes, 0, userCountMessageBytes.Length);
                    }
                    Thread.Sleep(50);
                }
            }

            connectResponseMessage.Success = authenticated;

            var json = JsonSerializer.Serialize(connectResponseMessage);
            var msg = System.Text.Encoding.UTF8.GetBytes(json);
            client.GetStream().Write(msg, 0, msg.Length);

            /*List<DirectChatMessage> messageList = server.GetMessages();
            if (messageList != null)
            {
                foreach (var savedMessage in messageList)
                {
                    if (savedMessage.ToUserId == user.Id)
                    {
                        var directChatMessage = savedMessage;
                        directChatMessage.SessionId = null;
                        var directChatJson = JsonSerializer.Serialize(directChatMessage);
                        var chatMsg = System.Text.Encoding.UTF8.GetBytes(directChatJson);
                        client.GetStream().Write(chatMsg, 0, chatMsg.Length);
                    }
                }
            }*/
        }
    }
}
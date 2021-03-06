﻿using ChatProtocol;
using ChatServer.Extension;
using ChatServer.MessageHandler;
using System;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;

namespace ChatServer
{
    class Program
    {
        static void HandleClient(Server server, TcpClient client)
        {
            var bytes = new byte[1024];
            string data;

            while (true)
            {
                int i;
                try
                {
                    NetworkStream stream = client.GetStream();

                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        data = System.Text.Encoding.UTF8.GetString(bytes, 0, i);

                        // Verschlüsselung: data entschlüsseln

                        var genericMessage = JsonSerializer.Deserialize<GenericMessage>(data);

                        var message = MessageFactory.GetMessage(genericMessage.MessageId, data);

                        var handler = MessageHandlerFactory.GetMessageHandler(genericMessage.MessageId);
                        handler.Execute(server, client, message);
                    }
                }
                catch (System.IO.IOException)
                {

                }
                catch (System.InvalidOperationException)
                { }
            }
        }

        static void Main()
        {
            var server = new Server(13000, "127.0.0.1");

            try
            {
                server.SetPassword("test123");
                server.Start();
                Console.WriteLine($"{server.UserCount()} users registered.");

                while (true)
                {
                    Console.Write("Waiting for a connection... ");
                    try
                    {
                        TcpClient client = server.AcceptTcpClient();
                        Console.WriteLine("Connected!");
                        var thread = new Thread(() => HandleClient(server, client));
                        thread.Start();
                    }
                    catch (System.IO.IOException)
                    {

                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                server.Stop();
            }

            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }
    }
}
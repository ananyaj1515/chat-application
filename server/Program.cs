using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class Program
    {
        private static readonly ConcurrentDictionary<string, TcpClient> clients = new();
        private static readonly ConcurrentDictionary<string, string> clientNames = new();
        
        static async Task Main(string[] args)
        {
            int port = 9000;
            TcpListener server = new TcpListener(IPAddress.Any, port);
            server.Start();
            Console.WriteLine("[Server] Server started and listening on port " + port);

            while(true)
            {
                TcpClient client = await server.AcceptTcpClientAsync();
                string clientId = Guid.NewGuid().ToString();
                clients[clientId] = client;
                Console.WriteLine("[Server] New connection added: "+ clientId);
                _ = Task.Run(() => HandleClientAsync(clientId, client)); // we use Task.Run instead of await to prevent blocking the loop, so it can go service other clients also, fire and forget
            }
        }

        private static async Task HandleClientAsync(string clientId, TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            try
            {
                // first message should be the name, implemented the same way in client code
                string? name = await reader.ReadLineAsync();          
                if (string.IsNullOrWhiteSpace(name)) return;
                clientNames[clientId] = name;

                Console.WriteLine("[Server] " + name + " joined");
                await BroadcastAsync("[Server] " + name + " has joined");

                string? message;
                while ((message = await reader.ReadLineAsync()) != null) // becomes null when client disconnects
                {
                    if (message == "/exit")
                    {
                        break;
                    }

                    string formatted = name + ": " + message;
                    Console.WriteLine("[Client] " + formatted);
                    await BroadcastAsync(formatted);
                }


            } catch (Exception e)
            {
                Console.WriteLine("[Server] client " + clientId + "ran into error: " + e.Message);
            } finally
            {
                clients.TryRemove(clientId, out _);
                clientNames.TryRemove(clientId, out _);
                client.Close();
                Console.WriteLine("[Server] " + clientId + " removed.");
            }
        }

        private static async Task BroadcastAsync(string message)
        {
            foreach (var(id, client) in clients)
            {
                try
                {
                    StreamWriter writer = new StreamWriter(client.GetStream(), Encoding.UTF8) { AutoFlush = true };
                    await writer.WriteLineAsync(message);
                } catch (Exception e)
                {
                    Console.WriteLine("[Server] client " + clientNames[id] + " disconnected");
                    Console.WriteLine("ERROR: " + e.Message);
                }
            }
        } 
    }
}
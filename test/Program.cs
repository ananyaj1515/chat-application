using System.Net.Sockets;
using System.Text;

Console.WriteLine("Enter your username: ");
string username = Console.ReadLine() ?? "Anonymous";

TcpClient client = new TcpClient();
await client.ConnectAsync("127.0.0.1", 9000);
Console.WriteLine("[Client] connected to server 127.0.0.1 on port ", 9000); // this is my own machine, loopback address

NetworkStream stream = client.GetStream();
StreamReader reader = new StreamReader(stream, Encoding.UTF8);
StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) {AutoFlush = true};


await writer.WriteLineAsync(username);

_ = Task.Run(async () =>
{
    string? line;
    while ((line = await reader.ReadLineAsync()) != null)
    {
        Console.WriteLine("[Received] ", line);
    }

});

Console.WriteLine("[Client] Type message and press enter. /exit to quit");

string? message;
while(( message = Console.ReadLine()) != null)
{
    await writer.WriteLineAsync(message);
    if (message == "/exit")
    {
        break;
    }
}

client.Close();
Console.WriteLine("[Client] disconnected");
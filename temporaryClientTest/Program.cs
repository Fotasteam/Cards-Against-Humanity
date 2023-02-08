using System.Net.Sockets;
using System.Text;
using WatsonTcp;

WatsonTcpClient tcpClient = new WatsonTcpClient("83.29.103.194", 8001); //192.168.1.81
tcpClient.Events.ServerConnected += ServerConnected;
tcpClient.Events.ServerDisconnected += ServerDisconnected;
tcpClient.Events.MessageReceived += MessageReceived;
tcpClient.Callbacks.SyncRequestReceived = SyncRequestReceived;
tcpClient.Connect();

tcpClient.Send("Hello!");

bool deactivate = false;

while (!deactivate)
{
    if (Console.ReadKey().Key == ConsoleKey.Escape) deactivate = true;
}

void MessageReceived(object sender, MessageReceivedEventArgs args)
{
    Console.WriteLine("Message from server: " + Encoding.UTF8.GetString(args.Data));
}

void ServerConnected(object sender, ConnectionEventArgs args)
{
    //Console.WriteLine("Server connected");
}

void ServerDisconnected(object sender, DisconnectionEventArgs args)
{
    //Console.WriteLine("Server disconnected");
}

SyncResponse SyncRequestReceived(SyncRequest req)
{
    return new SyncResponse(req, "Hello back at you!");
}
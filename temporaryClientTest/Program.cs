using System.Net.Sockets;
using System.Text;
using WatsonTcp;

//WatsonTcpClient tcpClient = new WatsonTcpClient("83.29.103.194", 8001); //192.168.1.81
//tcpClient.Events.ServerConnected += ServerConnected;
//tcpClient.Events.ServerDisconnected += ServerDisconnected;
//tcpClient.Events.MessageReceived += MessageReceived;
//tcpClient.Callbacks.SyncRequestReceived = SyncRequestReceived;
//tcpClient.Connect();

//tcpClient.Send("Hello!");

//bool deactivate = false;

//while (!deactivate)
//{
//    if (Console.ReadKey().Key == ConsoleKey.Escape) deactivate = true;
//}

//void MessageReceived(object sender, MessageReceivedEventArgs args)
//{
//    Console.WriteLine("Message from server: " + Encoding.UTF8.GetString(args.Data));
//}

//void ServerConnected(object sender, ConnectionEventArgs args)
//{

//}

//void ServerDisconnected(object sender, DisconnectionEventArgs args)
//{

//}

//SyncResponse SyncRequestReceived(SyncRequest req)
//{
//    return new SyncResponse(req, "Hello back at you!");
//}

string output;
using (HttpClient client = new HttpClient())
{
    output = await client.GetStringAsync("https://raw.githubusercontent.com/Fotasteam/Cards-Against-Humanity/master/Karty%20Przeciwko%20Ludzko%C5%9Bci/Cards/BlackCards1.ini");
}

List<string> strings = new List<string>();

string aLine = null;
StringReader strReader = new StringReader(output);
while (true)
{
    aLine = strReader.ReadLine();
    if (aLine != null)
    {
        strings.Add(aLine);
    }
    else
    {
        break;
    }
}
Console.WriteLine("Modified text:\n\n{0}");

for (int i = 0; i < strings.Count; i++)
{
    Console.WriteLine(strings[i]);
}
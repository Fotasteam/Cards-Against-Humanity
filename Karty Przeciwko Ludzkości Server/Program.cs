using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

using WatsonTcp;

// -----------------------

//matos - 192.168.1.74

//ja - 192.168.1.81 // 192.168.1.81:8001

// ----------------------
List<Guid> guids = new List<Guid>(); // guid kazdego klienta
List<string> nicknames = new List<string>();


WatsonTcpServer server = new WatsonTcpServer(null, 8001);

bool deactivate = false;
int gameState = 0;
int headPlayer = 0;
bool didServerReceiveID = false;

server.Events.ClientConnected += ClientConnected;
server.Events.ClientDisconnected += ClientDisconnected;
server.Events.MessageReceived += MessageReceived;
server.Callbacks.SyncRequestReceived = SyncRequestReceived;

IEnumerable<ClientMetadata> clients = server.ListClients();

server.Start();

while ( !deactivate )
{
    if (Console.ReadKey().Key != ConsoleKey.Enter) deactivate = true;

    bool roundActive = true;
    headPlayer = 0;
    gameState = 1;

    while (roundActive)
    {
        switch (gameState)
        {              
            case 1:
                Random randomPlayer = new Random();
                int rand = randomPlayer.Next(0, nicknames.Count);
                headPlayer = rand;

                foreach (Guid guid in guids)
                {
                    server.Send(guid, nicknames.Count().ToString());
                    foreach (string nick in nicknames)
                    {
                        server.Send(guid, nick);
                    }
                    server.Send(guid, headPlayer.ToString());
                }

                gameState = 2;
                break;
        }
    }
}

//server.Send(guid, "Hello, client!");

//EVENTS:
void ClientConnected(object sender, ConnectionEventArgs args)
{
    Console.WriteLine("Client connected: " + args.Client.ToString());
    guids.Add(args.Client.Guid);
}

void ClientDisconnected(object sender, DisconnectionEventArgs args)
{
    Console.WriteLine("Client disconnected: " + args.Client.ToString() + ": " + args.Reason.ToString());
}

void MessageReceived(object sender, MessageReceivedEventArgs args)
{
    switch (gameState)
    {
        case 0:
            nicknames.Add(Encoding.UTF8.GetString(args.Data)); //mozliwy bug, guid szybciej dodawany od nickname'u,
            break; // przez co gdy 2 osoby naraz sie polacza id dla guids i nicknames bedzie inne
        case 2:
            if (!didServerReceiveID)
            {
                int id = int.Parse(Encoding.UTF8.GetString(args.Data));

                foreach (Guid guid in guids)
                {
                    if (guid != guids[headPlayer])
                    {
                        server.Send(guid, id.ToString());
                    }
                }
            }
            else
            {
                int type = int.Parse(Encoding.UTF8.GetString(args.Data));

                foreach (Guid guid in guids)
                {
                    if (guid != guids[headPlayer])
                    {
                        server.Send(guid, type.ToString());
                    }
                }
            }
            break;
    }

    //Console.WriteLine("Message from " + args.Client.ToString() + ": " + Encoding.UTF8.GetString(args.Data));
}

SyncResponse SyncRequestReceived(SyncRequest req)
{
    return new SyncResponse(req, "Hello back at you!");
}
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Transactions;
using WatsonTcp;

List<Guid> guids = new List<Guid>(); // guid kazdego klienta
List<string> nicknames = new List<string>();

WatsonTcpServer server = new WatsonTcpServer(null, 8001);

bool startGame = false;
bool gameActive = true;
int gameState = 0;
int headPlayer = 0;
bool didServerReceiveIDBlack = false;
int receivedWhiteCards = 0;
List<int> listOfEverybodysWhiteCardID = new List<int>();
bool newRound = true;

server.Events.ClientConnected += ClientConnected;
server.Events.ClientDisconnected += ClientDisconnected;
server.Events.MessageReceived += MessageReceived;
server.Callbacks.SyncRequestReceived = SyncRequestReceived;

IEnumerable<ClientMetadata> clients = server.ListClients();

server.Start();

while (gameActive && newRound)
{
    newRound = false;
    gameState = 1;
    listOfEverybodysWhiteCardID.Clear();
    headPlayer = 0;
    didServerReceiveIDBlack = false;
    receivedWhiteCards = 0;

    while (!startGame)
    {
        if (Console.ReadKey().Key != ConsoleKey.Enter) startGame = true;

        bool roundActive = true;
        headPlayer = 0;
        gameState = 1;

        if (Console.ReadKey().Key == ConsoleKey.Escape)
        {
            gameActive = false;
            roundActive = false;
        }

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

            if (newRound) break;
        }
    }
}

Console.WriteLine("Shutting down...");

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
            if (!didServerReceiveIDBlack)
            {
                int id = int.Parse(Encoding.UTF8.GetString(args.Data));

                foreach (Guid guid in guids)
                {
                    if (guid != guids[headPlayer])
                    {
                        server.Send(guid, id.ToString());
                        Console.WriteLine(guid + " ID SENT: " + id);
                    }
                }
                didServerReceiveIDBlack = true;
            }
            else
            {
                int type = int.Parse(Encoding.UTF8.GetString(args.Data));

                foreach (Guid guid in guids)
                {
                    if (guid != guids[headPlayer])
                    {
                        server.Send(guid, type.ToString());
                        Console.WriteLine(guid + " TYPE SENT: " + type);
                    }
                }
                gameState = 3;
            }
            break;
        case 3:
            listOfEverybodysWhiteCardID.Add(int.Parse(Encoding.UTF8.GetString(args.Data)));

            receivedWhiteCards++;
            if (receivedWhiteCards == guids.Count - 1)
            {
                foreach (Guid guid in guids)
                {
                    foreach (int id in listOfEverybodysWhiteCardID)
                    {
                        server.Send(guid, id.ToString());
                        Console.WriteLine(guid + " WHITE ID SENT: " + id);
                    }
                }
            }

            Console.WriteLine("[ENTER] - NEW ROUND");
            Console.WriteLine("Awaiting for further tasks...");
            if (Console.ReadKey().Key == ConsoleKey.Enter)
            {
                newRound = true;

                foreach (Guid guid in guids)
                {
                    server.Send(guid, "6");
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
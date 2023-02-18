using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Transactions;
using WatsonTcp;

bool shouldAnotherRoundBegin = true;
bool hasAtLeastOneRoundHappened = false;

Console.ForegroundColor = ConsoleColor.Red;
Console.WriteLine("This server version is still experimental, expect some minor issues to occur");

List<Guid> guids = new List<Guid>(); // guid kazdego klienta
List<string> nicknames = new List<string>();

WatsonTcpServer server = new WatsonTcpServer(null, 8001);

int gameState = 0;
int headPlayer = 0;
bool didServerReceiveIDBlack = false;
int receivedWhiteCards = 0;
List<int> listOfEverybodysWhiteCardID = new List<int>();
bool deactivate = false;
bool roundActive = true;
int receivedVotes = 0;

int[] votingCardsIDs = new int[1500];

server.Events.ClientConnected += ClientConnected;
server.Events.ClientDisconnected += ClientDisconnected;
server.Events.MessageReceived += MessageReceived;
server.Callbacks.SyncRequestReceived = SyncRequestReceived;

IEnumerable<ClientMetadata> clients = server.ListClients();

server.Start();

while (shouldAnotherRoundBegin)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine(Environment.NewLine + "[GAME] - NEW ROUND BEGINS:" + Environment.NewLine);
    shouldAnotherRoundBegin = false;
    gameState = 0;
    headPlayer = 0;
    didServerReceiveIDBlack = false;
    receivedWhiteCards = 0;
    listOfEverybodysWhiteCardID.Clear();
    deactivate = false;
    roundActive = true;
    votingCardsIDs = new int[1500];
    receivedVotes = 0;

    while (!deactivate)
    {
        if (Console.ReadKey().Key != ConsoleKey.Enter || hasAtLeastOneRoundHappened) deactivate = true;

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
}

Console.ForegroundColor = ConsoleColor.Red;
Console.WriteLine("[SERVER] - SHUTTING DOWN...");

void ClientConnected(object sender, ConnectionEventArgs args)
{
    sendServerMessage("CLIENT CONNECTED: " + args.Client.ToString());
    guids.Add(args.Client.Guid);
}

void ClientDisconnected(object sender, DisconnectionEventArgs args)
{
    sendServerMessage("CLIENT CONNECTED: " + args.Client.ToString() + " REASON: " + args.Reason.ToString());
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
                        sendServerMessage("RECEIVED HEADPLAYER'S BLACK CARD ID, SENDING IT TO EVERYONE " + guid + " ID SENT: " + id);    
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
                        sendServerMessage("RECEIVED HEADPLAYER'S BLACK CARD TYPE, SENDING IT TO EVERYONE " + guid + " TYPE SENT: " + type);
                    }
                }
                gameState = 3;
            }
            break;
        case 3:
            listOfEverybodysWhiteCardID.Add(int.Parse(Encoding.UTF8.GetString(args.Data)));
            sendServerMessage("RECEIVED " + args.Client + " WHITE CARD ID, AWAITING EVERYBODYS CHOICE");

            receivedWhiteCards++;
            if (receivedWhiteCards == guids.Count - 1)
            {
                foreach (Guid guid in guids)
                {
                    foreach (int id in listOfEverybodysWhiteCardID)
                    {
                        server.Send(guid, id.ToString());
                        sendServerMessage("SENDING WHITE CARD ID TO: " + guid + " WHITE ID SENT: " + id);
                    }
                }
            }

            gameState = 4;
            break;

        case 4:
            votingCardsIDs[int.Parse(Encoding.UTF8.GetString(args.Data))]++;

            receivedVotes++;
            sendGameMessage("MESSAGE ISSUED BY " + args.Client + " RECEIVED " + receivedVotes + " OUT OF " + guids.Count + " VOTES");
            if (receivedVotes == guids.Count)
            {
                int elementWithMostVotes = 0;
                int indexOfTheElementWithMostVotes = 0;
                for (int i = 0; i < votingCardsIDs.Length; ++i)
                {
                    if (votingCardsIDs[i] > elementWithMostVotes)
                    {
                        elementWithMostVotes = votingCardsIDs[i];
                        indexOfTheElementWithMostVotes = i;
                    }
                }

                foreach (Guid guid in guids)
                {
                    server.Send(guid ,indexOfTheElementWithMostVotes.ToString());
                }
                sendServerMessage("SENDING VOTING RESULTS TO EVERYONE");
                Console.WriteLine();
                sendGameMessage("TO START ANOTHER ROUND PRESS ENTER TWICE");
                sendServerMessage("AWAITING FURTHER TASKS...");
                hasAtLeastOneRoundHappened = true;
                if (Console.ReadKey().Key == ConsoleKey.Enter)
                {
                    shouldAnotherRoundBegin = true;

                    foreach (Guid guid in guids)
                    {
                        server.Send(guid, "6");
                        sendServerMessage("TELLING CLIENTS TO RESET AND PREPARE FOR A NEW ROUND");
                    }
                }
                roundActive = false;
            }
            break;
    }

    //Console.WriteLine("Message from " + args.Client.ToString() + ": " + Encoding.UTF8.GetString(args.Data));
}

SyncResponse SyncRequestReceived(SyncRequest req)
{
    return new SyncResponse(req, "Hello back at you!");
}

void sendGameMessage(string message)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("[GAME] - " + message);
}

void sendServerMessage(string message)
{
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("[SERVER] - " + message);
}
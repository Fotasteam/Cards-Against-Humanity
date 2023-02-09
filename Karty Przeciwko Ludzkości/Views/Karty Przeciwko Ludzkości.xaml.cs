﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using WatsonTcp;
using System.Text;
using Windows.Media.Protection.PlayReady;
using Windows.UI.Popups;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Karty_Przeciwko_Ludzkości;
using Windows.UI.Xaml.Documents;
using System.Net.Sockets;
using static System.Net.Mime.MediaTypeNames;
using System.Threading;
using Karty_Przeciwko_Ludzkości.Scripts;
using Windows.UI.Composition;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace Karty_Przeciwko_Ludzkości.Views
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class Karty_Przeciwko_Ludzkości : Page
    {
        private List<Card> Cards;


        string nickname = ((App)Windows.UI.Xaml.Application.Current).playerNick;
        List<string> playerNicknames = new List<string>();
        int whoIsHeadPlayer = 0;
        int gameState = 0;
        string serverIp;
        WatsonTcpClient tcpClient;
        public Karty_Przeciwko_Ludzkości()
        {
            // ---TEMPORARY

            CardManager cardManager = new CardManager();

            List<Card> cards = new List<Card>();
            cards = cardManager.getRandomBlackCard();

            string test = "";
            foreach (string line in ((App)Windows.UI.Xaml.Application.Current).test)
            {
                test += line + " ";
            }

            var messageDialog = new MessageDialog(test);
            messageDialog.Title = "Error";
            messageDialog.ShowAsync();

            // ---TEMPORARY

            this.InitializeComponent();
            if (((App)Windows.UI.Xaml.Application.Current).ipAddress != null)
            {
                serverIp = ((App)Windows.UI.Xaml.Application.Current).ipAddress;
                tcpClient = new WatsonTcpClient(serverIp, 8001);
                tcpClient.Events.ServerConnected += ServerConnected;
                tcpClient.Events.ServerDisconnected += ServerDisconnected;
                tcpClient.Events.MessageReceived += MessageReceived;
                tcpClient.Callbacks.SyncRequestReceived = SyncRequestReceived;

                tcpClient.Connect();
                tcpClient.Send(nickname);
            }
            else
            {
                //var messageDialog = new MessageDialog("Brak adresu IP");
                //messageDialog.Title = "Error";
                //messageDialog.ShowAsync();
            }
            //tcpClient.Connect();

            CardManager manager = new CardManager();
            Cards = manager.GetCards();
        }

        int playerAmmount = 0; //zresetowac to przy restarcie rundy!!!

        async void MessageReceived(object sender, MessageReceivedEventArgs args)
        {
            string message = Encoding.UTF8.GetString(args.Data);
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                switch (gameState)
                {
                    case 0:
                        if (playerAmmount == 0)
                        {
                            if (int.TryParse(message, out playerAmmount))
                            {
                                //nic tu
                            }
                            else
                            {
                                var messageDialog = new MessageDialog(nickname);
                                messageDialog.Title = "Failed at Parsing: Input string was not in a correct format, message: " + message + ".";
                                messageDialog.ShowAsync();
                            }

                        }
                        else if (playerNicknames.Count() < playerAmmount)
                            playerNicknames.Add(message);
                        else
                        {
                            whoIsHeadPlayer = int.Parse(message);
                            gameState = 1;
                            if (playerNicknames[whoIsHeadPlayer] == nickname)
                            {
                                //wybor karty czarnej i wyslanie info do serwera


                                break;
                            }
                            
                            //wybor karty bialej nie tutaj, po dostaniu info od serwera o karcie czarnej
                            //bedziemy wybierac karty biale

                            //serwer: oczekiwanie info o karcie czarnej i wyslanie jej do clientow!!!
                        }
                        break;
                }
            });
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
    }
}

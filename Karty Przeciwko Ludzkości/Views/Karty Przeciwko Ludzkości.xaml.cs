using System;
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
using System.ServiceModel.Channels;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace Karty_Przeciwko_Ludzkości.Views
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class Karty_Przeciwko_Ludzkości : Page
    {
        private List<Card> Cards;

        List<Card> selectedWhiteCards = new List<Card>();
        Card blackCard = new Card();
        string nickname = ((App)Windows.UI.Xaml.Application.Current).playerNick;
        List<string> playerNicknames = new List<string>();
        int whoIsHeadPlayer = 0;
        int gameState = 0;
        string serverIp;
        WatsonTcpClient tcpClient;
        public Karty_Przeciwko_Ludzkości()
        {
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
                var messageDialog = new MessageDialog("Brak adresu IP");
                messageDialog.Title = "Error";
                messageDialog.ShowAsync();
            }
        }

        int playerAmmount = 0; //zresetowac to przy restarcie rundy!!!
        bool didPlayerReceiveID = false; //zresetowac -||-
        List<int> idOfWhiteCards = new List<int>();

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
                                CardManager manager = new CardManager();
                                Cards = manager.getRandomBlackCard();
                                gridView.Items.Clear();

                                for (int i = 0; i < Cards.Count; ++i)
                                {
                                    gridView.Items.Add(Cards[i]);
                                }
                                gameState = 2;
                                break;
                            }
                            else
                            {
                                gameState = 4;
                            }
                        }
                        break;
                    case 4:
                        CardManager cardManager = new CardManager();

                        if (!didPlayerReceiveID)
                        {
                            blackCard.CardID = int.Parse(message);
                            didPlayerReceiveID = true;
                        }
                        else
                        {
                            blackCard.CardType = int.Parse(message);
                            blackCard = cardManager.getBlackCardFromType(blackCard.CardType, blackCard.CardID);

                            gridBlackCardTextBlockCardContent.Text = blackCard.CardContent;
                            gridBlackCard.Visibility = Visibility.Visible;

                            List<Card> whiteCardGeneratedList = cardManager.getWhiteCards();
                            gridView.Items.Clear();

                            foreach (Card card in whiteCardGeneratedList)
                            {
                                gridView.Items.Add(card);
                            }
                            gameState = 5;
                        }
                        break;
                    case 3: 
                        
                        idOfWhiteCards.Add(int.Parse(message));

                        if (idOfWhiteCards.Count == playerAmmount-1)
                        {
                            CardManager managerCard = new CardManager();
                            selectedWhiteCards = managerCard.getWhiteCardsFromID(idOfWhiteCards);

                            gridView.Items.Clear();
                            foreach (Card card in selectedWhiteCards)
                            {
                                gridView.Items.Add(card);
                            }

                            gridBlackCardTextBlockCardContent.Text = blackCard.CardContent;
                            gridBlackCard.Visibility = Visibility.Visible;
                        }

                        gameState = 6;
                        break;
                    case 6:
                        blackCard = new Card();
                        gameState = 0;
                        whoIsHeadPlayer = 0;

                        gridBlackCard.Visibility = Visibility.Collapsed;
                        gridView.Items.Clear();
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

        private void gridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (playerNicknames[whoIsHeadPlayer] == nickname && gameState == 2)
            {
                Card selectedCard = gridView.SelectedItem as Card;

                tcpClient.Send(selectedCard.CardID.ToString());
                tcpClient.Send(selectedCard.CardType.ToString());
                gameState = 3;
                gridView.Items.Clear();
                blackCard = selectedCard;
            }
            else if (gameState == 5)
            {
                Card selectedCard = gridView.SelectedItem as Card;

                tcpClient.Send(selectedCard.CardID.ToString());
                gameState = 3;
                gridView.Items.Clear();
            }
        }
    }
}

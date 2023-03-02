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
using Windows.UI.Popups;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Karty_Przeciwko_Ludzkości;
using Karty_Przeciwko_Ludzkości.Scripts;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using System.Threading.Tasks;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace Karty_Przeciwko_Ludzkości.Views
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class Karty_Przeciwko_Ludzkości : Page
    {
        bool isInfobarEnabled = true;

        private List<Card> Cards;

        List<Card> selectedWhiteCards = new List<Card>();
        bool receivedWhiteID = false;
        Card blackCard = new Card();
        string nickname;
        List<string> playerNicknames = new List<string>();
        int whoIsHeadPlayer = 0;
        int gameState = 0;
        string serverIp;
        WatsonTcpClient tcpClient;
        public Karty_Przeciwko_Ludzkości()
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            nickname = localSettings.Values["nick"].ToString();
            isInfobarEnabled = (bool)localSettings.Values["toggleHints"];

            this.InitializeComponent();
            if (((App)Windows.UI.Xaml.Application.Current).ipAddress != null)
            {
                try
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
                catch (Exception ex)
                {
                    new ToastContentBuilder()
                    .AddArgument("conversationId", 9813)
                    
                    .AddText("Failed connecting to the server")
                    .AddText("Timed out. Go to settings and check if the IP address is correct. You can also check the help page for more information.")
                    .AddText("Exception message copied to clipboard")

                    .AddButton(new ToastButton()
                        .SetContent("Close")
                        .AddArgument("action", "dismiss"))

                    .AddAudio(new Uri("ms-appx:///Audio/NotificationSound.mp3"))
                    .Show();

                    DataPackage dp = new DataPackage();
                    dp.SetText(ex.Message);
                    Clipboard.SetContent(dp);

                    InfoBar.Visibility = Visibility.Visible;
                    InfoBar.Title = "Error";
                    InfoBar.Severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
                    InfoBar.Message = "Failed connecting to the server. Check notifications for further details.";
                }
            }
            else
            {
                new ToastContentBuilder()
                    .AddArgument("conversationId", 9813)

                    .AddText("Failed connecting to the server")
                    .AddText("No IP address selected. Go to settings and paste the hosts IP address.")

                    .AddButton(new ToastButton()
                        .SetContent("Close")
                        .AddArgument("action", "dismiss"))

                    .AddAudio(new Uri("ms-appx:///Audio/NotificationSound.mp3"))
                    .Show();

                InfoBar.Visibility = Visibility.Visible;
                InfoBar.Title = "Error";
                InfoBar.Severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
                InfoBar.Message = "Failed connecting to the server. Check notifications for further details.";
            }
        }

        int playerAmmount = 0; //zresetowac to przy restarcie rundy!!!
        bool didPlayerReceiveID = false; //zresetowac -||-
        List<int> idOfWhiteCards = new List<int>();
        List<string> nicknameOfWhiteCards = new List<string>();

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
                                //nic tutaj
                            }
                            else
                            {
                                var messageDialog = new MessageDialog("Failed at Parsing: Input string was not in a correct format, message: " + message + ".");
                                messageDialog.Title = "Error";
                                messageDialog.ShowAsync();

                                InfoBar.Visibility = Visibility.Visible;
                                InfoBar.Title = "Error";
                                InfoBar.Severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
                                InfoBar.Message = "Failed connecting to the server. Report this issue to the developer.";
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

                                gridView.Visibility = Visibility.Visible;
                                for (int i = 0; i < Cards.Count; ++i)
                                {
                                    gridView.Items.Add(Cards[i]);
                                }
                                gameState = 2;

                                InfoBar.Title = "You are the Card Czar";
                                InfoBar.Severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Informational;
                                InfoBar.Message = "As the Card Czar you need to select one of the cards available. Everyone else will have to select the funniest response available.";
                                LoadingProgressRing.Visibility = Visibility.Collapsed;
                                break;
                            }
                            else
                            {
                                InfoBar.Title = "Awaiting " + playerNicknames[whoIsHeadPlayer];
                                InfoBar.Severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Informational;
                                InfoBar.Message = "The Card Czar ( " + playerNicknames[whoIsHeadPlayer] + " ) is currently chosing a card. You need to wait until he finishes.";
                                LoadingProgressRing.Visibility = Visibility.Visible;
                                gameState = 4;
                            }
                        }

                        LoadingProgressRing.IsActive = false;
                        break;
                    case 4:
                        LoadingProgressRing.Visibility = Visibility.Collapsed;
                        InfoBar.Title = "Awaiting your choice";
                        InfoBar.Severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Informational;
                        InfoBar.Message = "The Card Czar chose his card, finally! Now, you need to select an 'apropriate' response.";

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
                            gridView.Visibility = Visibility.Visible;

                            foreach (Card card in whiteCardGeneratedList)
                            {
                                gridView.Items.Add(card);
                            }
                            gameState = 5;
                        }
                        break;
                    case 3:
                        LoadingProgressRing.Visibility = Visibility.Collapsed;
                        InfoBar.Title = "The results are here!";
                        InfoBar.Severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Informational;
                        InfoBar.Message = "Everyone finished chosing their cards, eventually. Now all you need to do is select the answer you like the most.";

                        if (!receivedWhiteID)
                        {
                            idOfWhiteCards.Add(int.Parse(message));
                            receivedWhiteID = true;
                        }
                        else
                        {
                            nicknameOfWhiteCards.Add(message);
                            receivedWhiteID = false;
                        }

                        if (nicknameOfWhiteCards.Count == playerAmmount-1)
                        {
                            CardManager managerCard = new CardManager();
                            selectedWhiteCards = managerCard.getWhiteCardsFromID(idOfWhiteCards, nicknameOfWhiteCards);

                            gridView.Items.Clear();
                            gridView.Visibility = Visibility.Visible;
                            foreach (Card card in selectedWhiteCards)
                            {
                                gridView.Items.Add(card);
                            }

                            gridBlackCardTextBlockCardContent.Text = blackCard.CardContent;
                            gridBlackCard.Visibility = Visibility.Visible;

                            gameState = 7;
                        }
                        break;
                    case 6:
                        blackCard = new Card();
                        gameState = 0;
                        whoIsHeadPlayer = 0;
                        playerAmmount = 0;
                        didPlayerReceiveID = false;
                        idOfWhiteCards.Clear();
                        playerNicknames.Clear();
                        selectedWhiteCards.Clear();
                        receivedWhiteID = false;
                        nicknameOfWhiteCards.Clear();
                        gridWinCardTextBlockNickname.Text = "Cards Against Humanity";

                        gridBlackCard.Visibility = Visibility.Collapsed;
                        gridView.Visibility = Visibility.Collapsed;
                        gridView.Items.Clear();
                        break;
                    case 8:
                        LoadingProgressRing.Visibility = Visibility.Collapsed;
                        InfoBar.Title = "Whoa, the results are here!";
                        InfoBar.Severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Informational;
                        InfoBar.Message = "This round is done. If you are the host, start a new round in the console. If you are not, the only thing you can do is wait.";

                        int id;
                        if (int.TryParse(message, out id))
                        {
                            CardManager cm = new CardManager();
                            id = int.Parse(message);
                            Card winnerCard = cm.getWhiteCardFromID(id);

                            gridWinCardTextBlockCardContent.Text = winnerCard.CardContent;
                        }
                        else
                        {
                            gridWinCardTextBlockNickname.Text = message;
                            gridWinCard.Visibility = Visibility.Visible;

                            gameState = 6;
                        }
                        break;
                }
            });
        }

        void ServerConnected(object sender, ConnectionEventArgs args)
        {
            //new ToastContentBuilder()
            //    .AddArgument("conversationId", 9813)

            //    .AddText("Succesfully connected to the server")
            //    .AddText("Awaiting message from host")

            //    .AddButton(new ToastButton()
            //        .SetContent("Close")
            //        .AddArgument("action", "dismiss"))

            //    .AddAudio(new Uri("ms-appx:///Audio/NotificationSound.mp3"))
            //    .Show();
        }

        async void ServerDisconnected(object sender, DisconnectionEventArgs args)
        {
            new ToastContentBuilder()
                .AddArgument("conversationId", 9813)

                .AddText("Connection to server lost")
                .AddText("Reason: " + args.Reason.ToString())

                .AddButton(new ToastButton()
                    .SetContent("Close")
                    .AddArgument("action", "dismiss"))

                .AddAudio(new Uri("ms-appx:///Audio/NotificationSound.mp3"))
                .Show();

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                InfoBar.Title = "Connection lost";
                InfoBar.Severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
                InfoBar.Message = "Read the notification to get more information about the issue.";
            });
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

                LoadingProgressRing.Visibility = Visibility.Visible;
                InfoBar.Title = "Waiting for players to finish deciding.";
                InfoBar.Severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Informational;
                InfoBar.Message = "Your job is done. Now people will select their most-suiting answer. After everyone does so you will be able to vote for your favorite.";
            }
            else if (gameState == 5)
            {
                Card selectedCard = gridView.SelectedItem as Card;

                tcpClient.Send(selectedCard.CardID.ToString());
                tcpClient.Send(nickname);
                gameState = 3;
                gridView.Items.Clear();

                LoadingProgressRing.Visibility = Visibility.Visible;
                InfoBar.Title = "Waiting for players to finish deciding.";
                InfoBar.Severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Informational;
                InfoBar.Message = "Great! Now you need to wait for everyone else to select their card.";
            }
            else if (gameState == 7)
            {
                LoadingProgressRing.Visibility = Visibility.Visible;
                InfoBar.Title = "Great choice!";
                InfoBar.Severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Informational;
                InfoBar.Message = "Once again, you need to wait for everyone to finish chosing their cards. Once they do, the best card will be revealed!";

                Card selectedCard = gridView.SelectedItem as Card;

                tcpClient.Send(selectedCard.CardID.ToString());
                tcpClient.Send(selectedCard.CardNickname);
                gameState = 8;
                gridView.Items.Clear();
            }

            gridView.Visibility = Visibility.Collapsed;
        }
    }
}

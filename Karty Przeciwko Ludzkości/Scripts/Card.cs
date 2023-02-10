using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarSymbols;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;
using static System.Net.Mime.MediaTypeNames;
using Karty_Przeciwko_Ludzkości;

namespace Karty_Przeciwko_Ludzkości.Scripts
{
    public class Card
    {
        public int CardID { get; set; }
        public int CardType { get; set; } //1 - biale, 2 - czarne
        public string CardContent { get; set; }
        public Windows.UI.Xaml.Controls.Symbol CardSymbol { get; set; }
    }

    public class CardManager
    {
        public List<Card> getRandomBlackCard()
        {
            HttpClient client = new HttpClient();
            Task<string> task = client.GetStringAsync("https://raw.githubusercontent.com/Fotasteam/Cards-Against-Humanity/master/Karty%20Przeciwko%20Ludzko%C5%9Bci/Cards/BlackCards1.ini");
            string result = task.Result;

            List<Card> BlackCards = new List<Card>();

            var sr = new StringReader(result);
            int id = -1;
            string line = null;
            while (true)
            {
                ++id;
                line = sr.ReadLine();
                if (line != null)
                {
                    BlackCards.Add(new Card { CardID = id, CardType = 2, CardSymbol= Windows.UI.Xaml.Controls.Symbol.Admin, CardContent = line });
                }
                else
                    break;
            }

            var SelectedCards = new List<Card>();
            Random rand = new Random();

            for (int i = 0; i < 5; i++)
            {
                int r = rand.Next(1, 331 + 1);
                SelectedCards.Add(BlackCards[r]);
            }

            client.Dispose();
            return SelectedCards;
        }

        public Card getBlackCardFromType(int Type, int ID)
        {
            List<Card> BlackCards = new List<Card>();
            Card blackCard = new Card();

            string fileName = "";
            HttpClient client = new HttpClient();

            switch (Type)
            {
                case 2:
                    fileName = "BlackCards1.ini";
                    break;
            }

            Task<string> task = client.GetStringAsync("https://raw.githubusercontent.com/Fotasteam/Cards-Against-Humanity/master/Karty%20Przeciwko%20Ludzko%C5%9Bci/Cards/" + fileName);
            string result = task.Result;

            var sr = new StringReader(result);
            int i = -1;
            string line = null;
            while (true)
            {
                ++i;
                line = sr.ReadLine();
                if (line != null)
                {
                    BlackCards.Add(new Card { CardID = i, CardType = 2, CardSymbol = Windows.UI.Xaml.Controls.Symbol.Admin, CardContent = line });
                }
                else
                    break;
            }

            for (int j = 0; j < BlackCards.Count; ++j)
            {
                if (BlackCards[j].CardID == ID)
                {
                    blackCard = BlackCards[j];
                }
            }

            return blackCard;
        }
    }

    //karty Uzytkownika wylosowac i zapisac do listy!!!
}

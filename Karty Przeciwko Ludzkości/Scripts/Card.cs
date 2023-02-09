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
    }

    public class CardManager
    {
        List<Card> BlackCards1 = new List<Card>() { };
        List<Card> BlackCards2 = new List<Card>() { };
        List<Card> BlackCards3 = new List<Card>() { };
        List<Card> WhiteCards = new List<Card>() { };

        public List<Card> GetCards()
        {
            var Cards = new List<Card>();

            Cards.Add(new Card { CardID = 1, CardType = 1, CardContent = "Taking a man's eyes and balls out and putting his eyes where his balls go and then his balls in the eye holes.\t" });
            Cards.Add(new Card { CardID = 2, CardType = 1, CardContent = "karta2", });
            Cards.Add(new Card { CardID = 3, CardType = 1, CardContent = "karta3", });
            Cards.Add(new Card { CardID = 3, CardType = 1, CardContent = "karta3", });
            Cards.Add(new Card { CardID = 3, CardType = 1, CardContent = "karta3" });
            Cards.Add(new Card { CardID = 1, CardType = 1, CardContent = "Taking a man's eyes and balls out and putting his eyes where his balls go and then his balls in the eye holes.\t" });
            Cards.Add(new Card { CardID = 2, CardType = 1, CardContent = "karta2" });
            Cards.Add(new Card { CardID = 3, CardType = 1, CardContent = "karta3" });
            Cards.Add(new Card { CardID = 3, CardType = 1, CardContent = "karta3" });
            Cards.Add(new Card { CardID = 3, CardType = 1, CardContent = "karta3" });

            return Cards;
        }

        public List<Card> getRandomBlackCard()
        {
            readFromFile("BlackCards1.ini");

            var Cards = new List<Card>();
            Random rand = new Random();

            for (int i = 0; i < 5; i++)
            {
                int r = rand.Next(1, 331 + 1);
                //Cards.Add(BlackCards1[0]);
            }

            return Cards;
        }

        private async void readFromFile(string file)
        {
            string output;
            using (HttpClient client = new HttpClient())
            {
                output = await client.GetStringAsync("https://raw.githubusercontent.com/Fotasteam/Cards-Against-Humanity/master/Karty%20Przeciwko%20Ludzko%C5%9Bci/Cards/"+file);
            }

            switch (file)
            {
                case "BlackCards1.ini":
                    var sr = new StringReader(output);
                    int id = -1;
                    string line = null;
                    while (true)
                    {
                        ++id;
                        line = sr.ReadLine();
                        if (line != null)
                        {
                            BlackCards1.Add(new Card { CardID = id, CardType = 2, CardContent = line });
                            ((App)Windows.UI.Xaml.Application.Current).test.Add(line);
                        }
                        else
                            break;
                    }

                    break;
            }
        }
        
    }

    //karty Uzytkownika wylosowac i zapisac do listy!!!
}

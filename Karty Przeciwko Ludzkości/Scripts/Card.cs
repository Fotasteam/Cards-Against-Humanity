using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarSymbols;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

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
        public List<Card> BlackCards1;
        public List<Card> BlackCards2;
        public List<Card> BlackCards3;
        public List<Card> WhiteCards;

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

            List<Card> listOfBlackCards = new List<Card>();
            for (int i = 1; i<= 331; ++i)
            {
                //listOfBlackCards.Add(new Card{ CardID = i, CardType = 2, CardContent = sr.ReadLine()});
            }
            //sr.Close();

            for (int i = 0; i < 5; i++)
            {
                int r = rand.Next(1, 331 + 1);


            }

            return Cards;
        }

        private async void readFromFile(string file)
        {
            StorageFile storageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(@"ms-appx:///" + file));

            StreamReader sr = new StreamReader(await storageFile.OpenStreamForReadAsync());

            switch (file)
            {
                case "BlackCards1.ini":
                    for (int id = 0; !sr.EndOfStream; ++id)
                    {
                        BlackCards1.Add(new Card { CardID = id, CardType = 2, CardContent = sr.ReadLine()});
                    }
                    break;
            }
        }
        
    }

    //karty Uzytkownika wylosowac i zapisac do listy!!!
}

using Microsoft.Web.WebView2.Core;
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

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace Karty_Przeciwko_Ludzkości.Views
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class InfoPage : Page
    {
        string textOfInformation = "Created using C# Universal Windows Platform with WinUI 2.8.2\r\n "+
            "This project is using the WatsonTCP API. It's a really simple API to get started with TCP/IP connections.\r\n " + 
            "This project is using the Microsoft UWP Toolkit (7.13)\r\n " + Environment.NewLine +
            "This is just a fun pet project, please don't sue me. It's just a blatant copy of Cards Against Humanity. " +
            "Check out the real game and buy it, it's really fun.\r\n ";

        string textOfGameRules = "The game is simple:\r\n" + Environment.NewLine +
            "One player is chosen as the Card Czar. He has to:\r\n" + Environment.NewLine +
            "1. Chose the question or fill-in-the-blank phrase.\r\n" +
            "2. At the end vote for the best answer.\r\n" + Environment.NewLine +
            "Everyone else is just a generic player. They have to:\r\n" + Environment.NewLine +
            "1. Choose the funniest answer to the Czars' Card.\r\n" +
            "2. At the end vote for the best asnwer.\r\n" + Environment.NewLine +
            "The game is really simple. Just have fun!";

        public InfoPage()
        {
            this.InitializeComponent();
        }
    }
}

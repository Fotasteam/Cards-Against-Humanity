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
using Windows.UI.Xaml.Media.Animation;

using WatsonTcp;
using System.Text;
using Windows.Media.Protection.PlayReady;
using Windows.UI.Popups;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Karty_Przeciwko_Ludzkości.Views;
using Windows.UI.Xaml.Documents;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x415

namespace Karty_Przeciwko_Ludzkości
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                ContentFrame.Navigate(typeof(Settings));
            }
            else
            {
                var selectedItem = args.SelectedItem as NavigationViewItem;
                string selectedItemTag = ((string)selectedItem.Tag);
                string pageName = "Karty_Przeciwko_Ludzkości.Views." + selectedItemTag;
                Type pageType = Type.GetType(pageName);
                ContentFrame.Navigate(pageType);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(Settings));
        }
    }
}

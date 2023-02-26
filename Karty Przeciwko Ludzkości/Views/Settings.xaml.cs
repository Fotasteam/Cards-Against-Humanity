using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Karty_Przeciwko_Ludzkości;
using Windows.Storage;
using Windows.UI.Popups;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace Karty_Przeciwko_Ludzkości.Views
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class Settings : Page
    {
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        object hintsToggled = false;
        bool isToggled = false;
        object nickname = "";
        public Settings()
        {
            if (localSettings.Values["toggleHints"] == null)
                localSettings.Values["toggleHints"] = true;

            if (localSettings.Values["nick"] == null)
                localSettings.Values["nick"] = "nickExample";

            nickname = localSettings.Values["nick"];
            hintsToggled = localSettings.Values["toggleHints"];
            isToggled = (bool)hintsToggled;

            this.InitializeComponent();
        }

        private void textBoxIpAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            var ip = textBoxIpAddress.Text;
            ((App)Application.Current).ipAddress = ip;
        }

        private void textBoxIpAddress_Loaded(object sender, RoutedEventArgs e)
        {
            if (((App)Windows.UI.Xaml.Application.Current).ipAddress != null)
                textBoxIpAddress.Text = ((App)Windows.UI.Xaml.Application.Current).ipAddress;
        }

        private void textBoxNick_TextChanged(object sender, TextChangedEventArgs e)
        {
            nickname = textBoxNick.Text;
            localSettings.Values["nick"] = nickname;
        }

        private void toggleSwitchHints_Toggled(object sender, RoutedEventArgs e)
        {
            if (toggleSwitchHints.IsOn)
                localSettings.Values["toggleHints"] = true;
            else
                localSettings.Values["toggleHints"] = false;

            hintsToggled = localSettings.Values["toggleHints"];
            isToggled = (bool)hintsToggled;
        }
    }
}

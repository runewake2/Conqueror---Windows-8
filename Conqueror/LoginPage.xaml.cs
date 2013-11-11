using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Conqueror
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
            if(App.Dispatcher == null)
                App.Dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
        }

        private void OnDonate(object sender, TappedRoutedEventArgs e)
        {
            //Windows.System.Launcher.LaunchUriAsync(new Uri("https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=UB5ACWZVSHMUA&lc=US&item_name=Conqueror%20for%20Windows%208&item_number=ConquerorForWindows8&currency_code=USD&bn=PP%2dDonationsBF%3abtn_donateCC_LG%2egif%3aNonHosted"));
            Windows.UI.ApplicationSettings.SettingsPane.Show();
        }

        private void OnGetAccount(object sender, RoutedEventArgs e)
        {
            Windows.System.Launcher.LaunchUriAsync(new Uri("http://www.conquerclub.com/?ref=427135"));
        }

        private async void OnNavigate(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(Conqueror.ViewModel.CurrentPlayerViewModel.Instance.Username))
            {
                MessageDialog dialog = new MessageDialog("You must enter your username before you can view your games. If you don't have a Conquer Club account you can sign up using the provided link.", "No Username");
                await dialog.ShowAsync();
            }
            else
            {
                Conqueror.ViewModel.CurrentPlayerViewModel.Instance.Save();
                this.Frame.Navigate(typeof(MainPage));
            }
        }
    }
}

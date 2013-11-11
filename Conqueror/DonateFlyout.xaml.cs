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

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace Conqueror
{
    public sealed partial class DonateFlyout : SettingsFlyout
    {
        public DonateFlyout()
        {
            this.InitializeComponent();
        }

        private void OnDonate(object sender, RoutedEventArgs e)
        {
            Windows.System.Launcher.LaunchUriAsync(new Uri("https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=UB5ACWZVSHMUA&lc=US&item_name=Conqueror%20for%20Windows%208&item_number=ConquerorForWindows8&currency_code=USD&bn=PP%2dDonationsBF%3abtn_donateCC_LG%2egif%3aNonHosted"));
        }

        private void OnReview(object sender, RoutedEventArgs e)
        {
            Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:REVIEW?PFN=35457SamuelWronski.Conqueror_rst4xp8zcmeb4"));
        }
    }
}

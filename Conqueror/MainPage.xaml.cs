using Conqueror.Common;
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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Conqueror
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public MainPage()
        {
            this.InitializeComponent();
            //App.Dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
            if (App.Dispatcher == null)
                App.Dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void ShowFlyoutHold(object sender, HoldingRoutedEventArgs e)
        {
            Flyout.ShowAttachedFlyout(sender as FrameworkElement);
            e.Handled = true;
        }

        private void ShowFlyoutRightTap(object sender, RightTappedRoutedEventArgs e)
        {
            Flyout.ShowAttachedFlyout(sender as FrameworkElement);
            e.Handled = true;
        }

        private void NavigateNewGame(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(BrowserPage));
        }

        private void OnOpenGame(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(BrowserPage), Conqueror.ViewModel.CurrentGamesViewModel.Instance.SelectedGame.GameNumber);
        }

        private async void OnReportBug(object sender, RoutedEventArgs e)
        {
            var mailto = new Uri("mailto:?to=runewake2@outlook.com&subject=Report a Bug (Conqueror 8)&body=Please describe your bug in detail. If I can not reproduce the issue you are experiencing I can not fix it. Thanks for taking the time to make Conqueror better!");
            await Windows.System.Launcher.LaunchUriAsync(mailto);
        }

        private async void OnGotoWOZ(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("http://www.worldofzero.com"));
        }

        private async void OnRequestFeature(object sender, RoutedEventArgs e)
        {
            var mailto = new Uri("mailto:?to=runewake2@outlook.com&subject=Request a Feature (Conqueror 8)&body=Thanks for taking the time to request a feature. Please describe what you would like in as much detail as you can. I may contact you to further develop upon your idea. I can not guarentee the addition of any particular feature into Conqueror. Thanks for your suggestion!");
            await Windows.System.Launcher.LaunchUriAsync(mailto);
        }

        private async void OnOpenConquerorWindowsPhone(object sender, TappedRoutedEventArgs e)
        {
            var open = new Uri("http://www.windowsphone.com/s?appid=8a197727-fa33-4583-9d75-e58c5f86f914");
            await Windows.System.Launcher.LaunchUriAsync(open);
        }

        private void NavigateSignOut(object sender, RoutedEventArgs e)
        {
            ViewModel.CurrentPlayerViewModel.Instance.Username = "";
            ViewModel.CurrentPlayerViewModel.Instance.Password = "";
            this.Frame.Navigate(typeof(LoginPage));
        }

        private void RefreshGames(object sender, RoutedEventArgs e)
        {
            ViewModel.CurrentGamesViewModel.Instance.Refresh();
        }
    }
}

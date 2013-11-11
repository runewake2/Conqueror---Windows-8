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
    public sealed partial class BrowserPage : Page
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


        public BrowserPage()
        {
            this.InitializeComponent();
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
            if (e.Parameter != null && String.IsNullOrWhiteSpace(e.Parameter.ToString()))
                browser.Navigate(new Uri("http://www.conquerclub.com/game.php?game=" + e.Parameter));
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void OpenFlyout(object sender, RoutedEventArgs e)
        {
            Flyout.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private void OnNavigationComp(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            try
            {
                string script = string.Format("document.getElementById(\"{0}\").value=\"{1}\"; document.getElementById(\"{2}\").value=\"{3}\";", "username", Conqueror.ViewModel.CurrentPlayerViewModel.Instance.Username, "password", Conqueror.ViewModel.CurrentPlayerViewModel.Instance.Password);
                browser.InvokeScript("eval", new string[] { script });
            }
            catch { }
        }

        private void NewGame(object sender, RoutedEventArgs e)
        {
            browser.Navigate(new Uri("http://www.conquerclub.com/player.php?mode=find"));
        }

        private void RefreshBrowser(object sender, RoutedEventArgs e)
        {
            browser.Refresh();
        }

        private void BackGame(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private TextBox focus;
        private void SetTarget(object sender, RoutedEventArgs e)
        {
            focus = sender as TextBox;
        }

        private void Add7(object sender, RoutedEventArgs e)
        {
            if (focus != null)
                focus.Text += "7";
        }
    }
}

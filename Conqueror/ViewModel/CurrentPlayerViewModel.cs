using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Conqueror.ViewModel
{
    public class CurrentPlayerViewModel : INotifyPropertyChanged
    {
        #region Singleton

        private static CurrentPlayerViewModel _instance;

        public static CurrentPlayerViewModel Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CurrentPlayerViewModel();
                return _instance;
            }
        }

        #endregion

        #region Properties
        private string username;
        private string password;

        public string Username
        {
            get { return username; }
            set
            {
                if (username != value)
                {
                    username = value;
                    NotifyChanged("Username");
                }
            }
        }

        public string Password
        {
            get { return password; }
            set
            {
                if (password != value)
                {
                    password = value;
                    NotifyChanged("Password");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region Data Handler and Initialization

        public CurrentPlayerViewModel()
        {
            _instance = this;
            Load();
        }

        private static string savePath = @"player_data.dat";

        public async void Load()
        {
            try
            {
                var appData = Windows.Storage.ApplicationData.Current;
                if(appData.RoamingSettings.Values["username"] != null)
                    Username = appData.RoamingSettings.Values["username"].ToString();
                if (appData.RoamingSettings.Values["password"] != null)
                    Password = appData.RoamingSettings.Values["password"].ToString();
            }
            catch (Exception e)
            {
            }
        }

        public async void Save()
        {
            try
            {
                var appData = Windows.Storage.ApplicationData.Current;
                appData.RoamingSettings.Values["username"] = this.username;
                appData.RoamingSettings.Values["password"] = this.password;
            }
            catch(Exception e) {
            }
        }

        #endregion
    }
}

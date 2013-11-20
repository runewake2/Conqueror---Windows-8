using ConquerorClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Popups;

namespace Conqueror.ViewModel
{
    public class CurrentPlayer : INotifyPropertyChanged
    {
        private static Color[] stateColors= new Color[] {
            new Color() { A = 255, R = 255, G = 215, B = 0},
            new Color() { A = 255, R = 200, G = 0, B = 0},
            new Color() { A = 255, R = 128, G = 128, B = 128},
            new Color() { A = 255, R = 0, G = 200, B = 0},
            new Color() { A = 255, R = 255, G = 215, B = 0},
        };

        private string name;
        private string rank;
        private string score;
        private Color playerState;

        private PlayerClient client;

        public string Name { get { return name; } }
        public string Rank { get { return rank; }
            set
            {
                if (rank != value)
                {
                    rank = value;
                    NotifyChanged("Rank");
                }
            }
        }

        public string Score
        {
            get { return score ; }
            set
            {
                if (score != value)
                {
                    score = value;
                    NotifyChanged("Score");
                }
            }
        }

        public Color State { get { return playerState; } }

        public CurrentPlayer(GamePlayer player)
        {
            name = player.Text;
            switch(player.State.ToLower())
            {
                case "won": playerState = stateColors[0]; break;
                case "lost": playerState = stateColors[1]; break;
                case "waiting": playerState = stateColors[2]; break;
                case "ready": playerState = stateColors[3]; break;
                case "playing": playerState = stateColors[4]; break;
                default: playerState = stateColors[2]; break;
            }

            client = new PlayerClient();
            client.OnGetPlayerComplete += (o, e) =>
            {
                App.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    Rank = e.Users.Rank;
                    Score = e.Users.Score;
                });
            };
            client.GetPlayer(name);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }

    public class CurrentGame : INotifyPropertyChanged
    {

        #region Properties
        private string map;
        private TimeSpan time;
        private string mapLink = @"Assets/Maps/Classic.jpg";
        private string type;
        private string spoils;
        private string reinforcements;
        private string gameNumber;
        private bool trench;
        private bool speed;
        private bool fog;
        private List<CurrentPlayer> player;

        public string MapName
        {
            get { return map; }
        }

        public string Spoils
        {
            get { return spoils; }
        }

        public string Fortifications
        {
            get { return reinforcements; }
        }

        public string GameMode
        {
            get { return type; }
        }

        public string GameNumber
        {
            get { return gameNumber; }
        }

        public float Trench
        {
            get { return trench ? 1 : 0.25f; }
        }

        public float SpeedGame
        {
            get { return speed ? 1 : 0.25f; }
        }

        public float FogOfWar
        {
            get { return fog ? 1 : 0.25f; }
        }

        private string timeString = "00:00:00";
        public string Time
        {
            get { return timeString; }
            set
            {
                if (timeString != value)
                {
                    timeString = value;
                    NotifyChanged("Time");
                }
            }
        }

        public string MapImage
        {
            get { return mapLink; }
            set
            {
                if (mapLink != value)
                {
                    mapLink = value;
                    NotifyChanged("MapImage");
                }
            }
        }
        
        public Color GameState
        {
            get {
                CurrentPlayer p = null;
                foreach(CurrentPlayer player in Players)
                {
                    if (player.Name.ToLower() == CurrentPlayerViewModel.Instance.Username.ToLower())
                    {
                        p = player;
                        break;
                    }
                }

                if (p != null)
                    return p.State;
                else return Colors.Black;
            }
        }

        public List<CurrentPlayer> Players
        {
            get { return player; }
        }

        #endregion

        private MapFetcher fetcher;
        private Timer timer;

        public CurrentGame(Game g)
        {
            map = g.Map.ToUpper();
            if (!TimeSpan.TryParse(g.TimeRemaining, out time))
                time = new TimeSpan(0, 0, 0);
            spoils = g.SpoilsString + " Spoils";
            switch(g.Fortifications)
            {
                case "C":
                    reinforcements = "Chained Reinforcements";
                    break;
                case "O":
                    reinforcements = "Adjacent Reinforcements";
                    break;
                case "M":
                    reinforcements = "Unlimited Reinforcements";
                    break;
                default:
                    reinforcements = "Unknown Reinforcements";
                    break;
            }
            switch (g.GameType)
            {
                case "S":
                    type = "Standard";
                    break;
                case "C":
                    type = "Terminator";
                    break;
                case "A":
                    type = "Assassin";
                    break;
                case "D":
                    type = "Doubles";
                    break;
                case "T":
                    type = "Triples";
                    break;
                case "Q":
                    type = "Quadruples";
                    break;
                case "P":
                    type = "Polymorphic";
                    break;
                default:
                    type = "Unknown Game Mode";
                    break;
            }
            gameNumber = g.GameNumber;
            trench = g.TrenchWarfare[0] == 'Y' ? true : false;
            speed = g.SpeedGame[0] == 'N' ? false : true;
            fog = g.FogOfWar[0] == 'Y' ? true : false;

            fetcher = new MapFetcher();
            fetcher.OnGetMapComplete += (o, e) =>
                {
                    App.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        MapImage = e.Data.LargeImageLink;
                    });
                };
            fetcher.GetMap(g.Map);

            timer = new Timer((o) =>
            {
                App.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    time = time.Subtract(TimeSpan.FromSeconds(1));
                    Time = time.ToString("hh\\:mm\\:ss");
                });
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

            player = new List<CurrentPlayer>();
            foreach (GamePlayer p in g.BuildPlayers)
                player.Add(new CurrentPlayer(p));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }

    public class CurrentGamesViewModel : INotifyPropertyChanged
    {
        #region Singleton

        private static CurrentGamesViewModel _instance;

        public static CurrentGamesViewModel Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CurrentGamesViewModel();
                return _instance;
            }
        }

        #endregion

        #region Properties
        private List<CurrentGame> games;

        private bool _crash = false;
        private bool crashed
        {
            get { return _crash; }
            set {
                if (_crash != value)
                {
                    _crash = value;
                    NotifyChanged("crashed");
                    NotifyChanged("HasCrashed");
                }
            }
        }

        public Windows.UI.Xaml.Visibility HasCrashed { get { return crashed ? Windows.UI.Xaml.Visibility.Visible : Windows.UI.Xaml.Visibility.Collapsed; } }

        public List<CurrentGame> Games
        {
            get { return games; }
            set
            {
                if (games != value)
                {
                    games = value;
                    NotifyChanged("Games");
                    NotifyChanged("SelectedGame");
                }
            }
        }

        private int index = 0;

        public int Index
        {
            get {
                if (Games == null || Games.Count <= 0)
                    return -1;
                else if (index < 0) return 0;
                return index;
            }
            set
            {
                if (index != value)
                {
                    index = value;
                    NotifyChanged("Index");
                    NotifyChanged("SelectedGame");
                }
            }
        }
        public CurrentGame SelectedGame
        {
            get
            {
                if (Games == null || Games.Count < index + 1)
                    return null;
                else if (index < 0) return Games[0];
                else return Games[index];
            }
        }

        private bool showCat = false;
        public bool ShowCat { get { return showCat; }
            set
            {
                if (showCat == value) return;
                showCat = value;
                NotifyChanged("ShowCat");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        private void SetGames(List<Game> games)
        {
            GameTimeUserComparator compare = new GameTimeUserComparator(CurrentPlayerViewModel.Instance.Username);
            games.Sort(compare);
            List<CurrentGame> currentGames = new List<CurrentGame>();
            foreach(Game g in games)
            {
                currentGames.Add(new CurrentGame(g));
            }
            Games = currentGames;
        }

        private GamesList client;

        #region Data Handler and Initialization

        public CurrentGamesViewModel()
        {
            _instance = this;

            crashed = false;
            client = new ConquerorClient.GamesList();
            client.OnGetGamesComplete += GamesCompleted;
            client.OnAPIUnavailable += APIUnavailable;
            client.OnGetGamesError += GamesError;
            client.FindPlayersCurrentGames(CurrentPlayerViewModel.Instance.Username);
        }

        public void Refresh()
        {
            client.FindPlayersCurrentGames(CurrentPlayerViewModel.Instance.Username);
        }

        protected void GamesCompleted(object sender, GameListArgs args)
        {
            App.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                crashed = false;
                try
                {
                    SetGames(args.Games.Data.GameInfo.Games);
                }
                catch { crashed = true; }
            });
        }

        protected async void APIUnavailable(object sender, EventArgs args)
        {
            App.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                crashed = true;
                Games = null;
                MessageDialog message = new MessageDialog("The Conquer Club API is not available right now. This is what Conqueror uses to run, therefore Conqueror will not function until this is fixed. You can still access your games by ConquerClub.com through Internet Explorer. Sorry for the inconvenience.", "Conquer Club API is Unavailable");
                message.Commands.Add(new UICommand("OK", null));

                await message.ShowAsync();
                App.Current.Exit();
            });
        }

        protected void GamesError(object sender, EventArgs args)
        {
            App.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                crashed = true;
                Games = null;
            });
        }

        #endregion
    }
}

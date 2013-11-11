using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.ComponentModel;

namespace ConquerorClient
{
    public class GameListArgs : EventArgs
    {
        private GameList gl;

        public GameList Games { get { return gl; } set { gl = value; } }

        public GameListArgs(GameList gl) { this.gl = gl; }
    }

    public delegate void GamesListEventHandler(object sender, GameListArgs e);

    /// <summary>
    /// The GamesList is used to retrieve a list of games from the Conquer Club API
    /// </summary>
    public class GamesList
    {
        //The base URL used to get the list of games
        private static readonly string _url = @"http://www.conquerclub.com/api.php?mode=gamelist&names=Y&chat=Y";


        public event GamesListEventHandler OnGetGamesComplete;
        public event EventHandler OnGetGamesError;
        public event EventHandler OnAPIUnavailable;

        /// <summary>
        /// This is the master function for getting a game list.
        /// Used for searching for games.
        /// </summary>
        /// <param name="gn">game number, comma separated</param>
        /// <param name="gs">game state</param>
        /// <param name="np">number of players, comma seperated</param>
        /// <param name="ty">game type, defined by </param>
        /// <param name="mp">map name, comma seperated</param>
        /// <param name="it">initial troops</param>
        /// <param name="po">play order</param>
        /// <param name="bc">spoils</param>
        /// <param name="ft">reinforcements</param>
        /// <param name="wf">fog of war</param>
        /// <param name="tw">trench warfare</param>
        /// <param name="rl">round limit, comma seperated, may be 0, 20, 50 or 100</param>
        /// <param name="sg">speed game, comma seperated, may be N, 5, 4, 3, 2, 1</param>
        /// <param name="pt">private/tournament game</param>
        /// <param name="to">tournament name</param>
        /// <param name="lb">tournament label</param>
        /// <param name="p1">player 1's username</param>
        /// <param name="p2">player 2's username</param>
        /// <param name="p3">player 3's username</param>
        /// <param name="p4">player 4's username</param>
        /// <param name="p1id">player 1's user id</param>
        /// <param name="p2id">player 2's user id</param>
        /// <param name="p3id">player 3's user id</param>
        /// <param name="p4id">player 4's user id</param>
        /// <param name="page">the page number to return</param>
        public void GetGamesList(string gn = null, GameState? gs = null, string np = null, GameType? ty = null,
            string mp = null, InitialTroops? it = null, PlayOrder? po = null, Spoils? bc = null, Reinforcements? ft = null, bool? wf = null,
            bool? tw = null, string rl = null, string sg = null, PrivateGame? pt = null, string to = null, string lb = null,
            string p1 = null, string p2 = null, string p3 = null, string p4 = null,
            string p1id = null, string p2id = null, string p3id = null, string p4id = null ,int? page = null)
        {
            //Build request string
            string request = _url;

            //continue to build request by adding parameters that exist to request string
            if (gn != null) request   += "&gn="   + gn;
            if (gs != null) request   += "&gs="   + (char)gs;
            if (np != null) request   += "&np="   + np;
            if (ty != null) request   += "&ty="   + (char)ty;
            if (mp != null) request   += "&mp="   + mp;
            if (it != null) request   += "&it="   + (char)it;
            if (po != null) request   += "&po="   + (char)po;
            if (bc != null) request   += "&bc="   + (char)bc;
            if (ft != null) request   += "&ft="   + (char)ft;
            if (wf != null) request   += "&wf="   + (wf == true ? "Y" : "N");
            if (tw != null) request   += "&tw="   + (tw == true ? "Y" : "N");
            if (rl != null) request   += "&rl="   + rl;
            if (sg != null) request   += "&sg="   + sg;
            if (pt != null) request   += "&pt="   + (char)pt;
            if (to != null) request   += "&to="   + to;
            if (lb != null) request   += "&lb="   + lb;
            if (p1 != null) request   += "&p1un=" + p1;
            if (p2 != null) request   += "&p2un=" + p2;
            if (p3 != null) request   += "&p3un=" + p3;
            if (p4 != null) request   += "&p4un=" + p4;
            if (p1id != null) request += "&p1="   + p1id;
            if (p2id != null) request += "&p2="   + p2id;
            if (p3id != null) request += "&p3="   + p3id;
            if (p4id != null) request += "&p4="   + p4id;
            if (page != null) request += "&page=" + page;

            //Pass request string to web accessor
            BeginFetchResult(request);
        }

        public void FindPlayersCurrentGames(string username = null, string userid = null)
        {
            if (username == null && userid == null)
                throw new ArgumentException("Both username and userid may not be null");
            GetGamesList(null,GameState.Active,null,null,null,null,null,null,null,null,null,null,null,null,null,null,username,null,null,null,userid,null,null,null,null);
        }

        public void FindPlayersFinishedGames(string username = null, string userid = null)
        {
            if (username == null && userid == null)
                throw new ArgumentException("Both username and userid may not be null");
            GetGamesList(null, GameState.Finished, null, null, null, null, null, null, null, null, null, null, null, null, null, null, username, null, null, null, userid, null, null, null, null);
        }

        private void BeginFetchResult(string url)
        {
            NetworkAccess.StartConnection();
            
            //TODO: Web API Call
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            if (request.Headers == null)
            {
                request.Headers = new WebHeaderCollection();
            }
            //request.Headers[HttpRequestHeader.IfModifiedSince] = DateTime.UtcNow.ToString();
            request.BeginGetResponse(EndFetchResult, request);
        }

        private void EndFetchResult(IAsyncResult result)
        {
            NetworkAccess.FinishConnection();

            if (result.IsCompleted)
            {
                HttpWebRequest request = (HttpWebRequest)result.AsyncState;

                //Deserialize the data and call the event
                string data;
                try
                {
                    StreamReader stream = new StreamReader(request.EndGetResponse(result).GetResponseStream());
                    data = stream.ReadToEnd();
                    stream.Dispose();
                    //stream.Close();
                }
                catch (WebException we) { CallCompleted(null); return; }

                data = data.Replace("<api>", "<api xmlns:json='http://james.newtonking.com/projects/json'> id='1'");
                data = data.Replace("<game>", "<game json:Array='true'>");

                XDocument doc = null;
                try
                {
                    doc = XDocument.Parse(data);
                }
                catch { APIUnavailable(); }

                string todeserialize = JsonConvert.SerializeXNode(doc);

                if (todeserialize.ToLower().Contains("the api is temporarily closed for maintenance"))
                {
                    APIUnavailable();
                }
                else
                {
                    GameList res = JsonConvert.DeserializeObject<GameList>(todeserialize);
                    CallCompleted(res);
                }
            }
        }

        private void APIUnavailable()
        {
            if (OnAPIUnavailable != null)
                OnAPIUnavailable(this, new EventArgs());
        }

        private void CallCompleted(GameList result)
        {
            if (result != null)
            {
                if (OnGetGamesComplete != null)
                    OnGetGamesComplete(result, new GameListArgs(result));
            }
            else
            {
                if (OnGetGamesError != null)
                    OnGetGamesError(result, new EventArgs());
            }
        }
    }

    [JsonObject()]
    public class GameList
    {
        [JsonProperty("api")]
        public GamesListResponse Data { get; set; }
    }

    [JsonObject()]
    public class GamesListResponse
    {
        [JsonProperty("page")]
        public string Page { get; set; }
        [JsonProperty("games")]
        public GameInformation GameInfo { get; set; }

        /// <summary>
        /// Gets the current page
        /// </summary>
        /// <returns>the current page OR -1 if it failed to parse the page</returns>
        public int GetCurrentPage()
        {
            string p = Page.Split(' ')[0].Trim();
            int ret;
            if (!Int32.TryParse(p, out ret))
                return -1; //Failed
            return ret;
        }

        /// <summary>
        /// Gets the last page
        /// </summary>
        /// <returns>the last page OR -1 if it failed to parse the page</returns>
        public int GetLastPage()
        {
            string p = Page.Split(' ')[2].Trim();
            int ret;
            if (!Int32.TryParse(p, out ret))
                return -1; //Failed
            return ret;
        }
    }

    [JsonObject()]
    public class GameInformation
    {
        [JsonProperty("@total")]
        public string Total { get; set; }
        [JsonProperty("game")]
        public List<Game> Games { get; set; }
    }

    [JsonObject()]
    public class Game : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [JsonProperty("game_number")]
        public string GameNumber { get; set; }
        [JsonProperty("game_state")]
        public string GameState { get; set; }
        [JsonProperty("tournament")]
        public string Tournament { get; set; }
        public bool IsTournament { get { return Tournament != null && Tournament.ToLower()[0] == 't'; } }
        [JsonProperty("private")]
        public string Private { get; set; }
        public bool IsPrivate { get { return Private != null && Private.ToLower()[0] == 't'; } }
        [JsonProperty("speed_game")]
        public string SpeedGame { get; set; }
        [JsonProperty("map")]
        public string Map { get; set; }
        [JsonProperty("game_type")]
        public string GameType { get; set; }
        [JsonProperty("initial_troops")]
        public string InitialTroops { get; set; }
        [JsonProperty("play_order")]
        public string PlayOrder { get; set; }
        [JsonProperty("bonus_cards")]
        public string BonusCards { get; set; }
        [JsonProperty("fortifications")]
        public string Fortifications { get; set; }
        [JsonProperty("war_fog")]
        public string FogOfWar { get; set; }
        [JsonProperty("trench_warfare")]
        public string TrenchWarfare { get; set; }
        [JsonProperty("round_limit")]
        public string RoundLimit { get; set; }
        [JsonProperty("round")]
        public string Round { get; set; }
        [JsonProperty("time_remaining")]
        public string TimeRemaining { get; set; }
        [JsonProperty("chat")]
        public string Chat { get; set; }
        [JsonProperty("players")]
        public GamePlayers Players { get; set; }
        [JsonProperty("events")]
        public string Events { get; set; }

        public bool Finished { get { return GameState.ToLower()[0] == 'f'; }}
        public float GameStateMeter { get { return (GameState.ToLower()[0] == 'f' ? 0 : (GameState.ToLower()[0] == 'w' ? 0.5f : 1)); } }

        public string CurrentPlayer
        {
            get
            {
                foreach (GamePlayer player in Players.Players)
                {
                    if (player.State.ToLower().Equals("ready") || player.State.ToLower().Equals("playing"))
                    {
                        return player.Text;
                    }
                    else if (player.State.ToLower().Equals("won"))
                    {
                        string s = "";
                        List<string> winners = new List<string>();
                        foreach (GamePlayer p in Players.Players)
                        {
                            if (p.State.ToLower().Equals("won"))
                            {
                                winners.Add(p.Text);
                                //s += p.Text + ", ";
                            }
                        }

                        winners.Sort();
                        foreach (string str in winners)
                            s += str + ", ";

                        return s.Substring(0, s.Length - 2);
                    }
                }
                return "";
            }
        }

        public int WinningPlayers
        {
            get
            {
                int i = 0;
                foreach (GamePlayer p in Players.Players)
                {
                    if (p.State.ToLower().Equals("won"))
                    {
                        i++;
                    }
                }
                return i;
            }
        }

        public string TurnOrderString
        {
            get
            {
                switch (PlayOrder.ToUpper()[0])
                {
                    case 'F':
                        return "Freestyle";
                    case 'S':
                        return "Sequential";
                    default:
                        return "Unknown";
                }
            }
        }

        public string SpoilsString
        {
            get
            {
                switch (BonusCards.ToUpper()[0])
                {
                    case '1':
                        return "No";
                    case '2':
                        return "Escalating";
                    case '3':
                        return "Flat Rate";
                    case '4':
                        return "Nuclear";
                    default:
                        return "Unknown";
                }
            }
        }

        public GamePlayer[] BuildPlayers
        {
            get
            {
                List<GamePlayer> _players = new List<GamePlayer>();
                int current = -1;
                foreach (GamePlayer p in Players.Players)
                    if (p.State.Equals("Won") || p.State.Equals("Ready") || p.State.Equals("Playing"))
                    {
                        current = Players.Players.IndexOf(p);
                        break;
                    }

                //Two states here
                //First is if current is -1, then just copy current players to List
                //Second is if current is not, then do a wraparound adding players on the way
                if (current <= -1) //STATE 1
                {
                    foreach (GamePlayer p in Players.Players)
                        _players.Add(p);
                }
                else //STATE 0
                {
                    bool wrapped = false;
                    for (int i = current; !wrapped || i != current; i = (i + 1) % Players.Players.Count)
                    {
                        wrapped = true;
                        _players.Add(Players.Players[i]);
                    }
                }

                return _players.ToArray();
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Game)
            {
                return (obj as Game).GameNumber.Equals(this.GameNumber);
            }
            return false;
        }
    }

    [JsonObject()]
    public class GamePlayers
    {
        [JsonProperty("player")]
        public List<GamePlayer> Players { get; set; }
    }

    [JsonObject()]
    public class GamePlayer
    {
        /// <summary>
        /// Defines the players current state
        /// May be:
        /// Won
        /// Lost
        /// Waiting
        /// Ready
        /// Playing
        /// </summary>
        [JsonProperty("@state")]
        public string State { get; set; }

        /// <summary>
        /// Depending on how the data was fetched this will either include the users name or id
        /// </summary>
        [JsonProperty("#text")]
        public string Text { get; set; }
    }

    public class GameWinnerComparator : Comparer<Game>
    {
        private string _pn;

        public GameWinnerComparator(string name)
        {
            _pn = name.ToLower();
        }

        public override int Compare(Game x, Game y)
        {
            int found = 0;
            foreach (GamePlayer p in x.Players.Players)
            {
                if (p.State.ToLower().Equals("won") && p.Text.ToLower().Equals(_pn))
                {
                    found -= 100;
                    break;
                }
            }
            foreach (GamePlayer p in y.Players.Players)
            {
                if (p.State.ToLower().Equals("won") && p.Text.ToLower().Equals(_pn))
                {
                    found += 100;
                    break;
                }
            }

            found += x.WinningPlayers * 10;
            found -= y.WinningPlayers * 10;

            found += x.Players.Players.Count;
            found -= y.Players.Players.Count;

            return found;
        }
    }

    public class GameTimeComparator : Comparer<Game>
    {
        public GameTimeComparator()
        {
        }

        public override int Compare(Game x, Game y)
        {
            TimeSpan xspan, yspan;

            if (TimeSpan.TryParse(x.TimeRemaining, out xspan) && TimeSpan.TryParse(y.TimeRemaining, out yspan))
            {
                return xspan.CompareTo(yspan);
            }

            return 0;
        }
    }

    public class GameTimeUserComparator : Comparer<Game>
    {
        string user;

        public GameTimeUserComparator(string u)
        {
            user = u;
        }

        public override int Compare(Game x, Game y)
        {
            TimeSpan xspan, yspan;
            if (x.CurrentPlayer.Equals(user) && !y.CurrentPlayer.Equals(user))
                return -1;
            else if (!x.CurrentPlayer.Equals(user) && y.CurrentPlayer.Equals(user))
                return 1;

            if (TimeSpan.TryParse(x.TimeRemaining, out xspan) && TimeSpan.TryParse(y.TimeRemaining, out yspan))
            {
                return xspan.CompareTo(yspan);
            }

            return 0;
        }
    }
}

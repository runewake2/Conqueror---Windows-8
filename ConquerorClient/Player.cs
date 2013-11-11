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
    public class PlayerArgs : EventArgs
    {
        private Player gl;

        public Player Users { get { return gl; } set { gl = value; } }

        public PlayerArgs(Player gl) { this.gl = gl; }
    }

    public delegate void PlayerEventHandler(object sender, PlayerArgs e);

    /// <summary>
    /// The GamesList is used to retrieve a list of games from the Conquer Club API
    /// </summary>
    public class PlayerClient
    {
        //The base URL used to get the list of games
        private static readonly string _url = @"http://www.conquerclub.com/api.php?mode=player";


        public event PlayerEventHandler OnGetPlayerComplete;
        public event EventHandler OnGetPlayerError;

        /// <summary>
        /// This is the master function for getting a game list.
        /// Used for searching for games.
        /// </summary>
        /// <param name="pn">the name of the player to get data of</param>
        public void GetPlayer(string pn)
        {
            //Player player = DataHandler.ShouldUpdatePlayer(pn);
            //if (player == null)
            {
                //Build request string
                string request = _url;

                //continue to build request by adding parameters that exist to request string
                if (pn == null) throw new ArgumentNullException("You must provide a player name");
                if (pn.Equals("")) throw new ArgumentException("You may not find a player with a blank username");

                //Pass request string to web accessor
                BeginFetchResult(request + "&un=" + pn);
            }
            //else CallCompleted(player);
        }

        private void BeginFetchResult(string url)
        {
            NetworkAccess.StartConnection();

            //TODO: Web API Call
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
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
                    data = new StreamReader(request.EndGetResponse(result).GetResponseStream()).ReadToEnd();
                }
                catch (WebException we) { CallCompleted(null); return; }

                XDocument doc = XDocument.Parse(data);

                string todeserialize = JsonConvert.SerializeXNode(doc);

                try
                {
                    PlayerData res = JsonConvert.DeserializeObject<PlayerData>(todeserialize);
                    //DataHandler.UpdatePlayer(res.Data.User.Username, res.Data.User);
                    CallCompleted(res.Data.User);
                }
                catch (JsonSerializationException jse)
                {
                    CallCompleted(null);
                }
            }
        }

        private void CallCompleted(Player result)
        {
            if (result != null)
            {
                if (OnGetPlayerComplete != null)
                    OnGetPlayerComplete(result, new PlayerArgs(result));
            }
            else
            {
                if (OnGetPlayerError != null)
                    OnGetPlayerError(result, new EventArgs());
            }
        }
    }

    [JsonObject()]
    public class PlayerData
    {
        [JsonProperty("api")]
        public PlayerResponse Data { get; set; }
    }

    [JsonObject()]
    public class PlayerResponse
    {
        [JsonProperty("player")]
        public Player User { get; set; }
    }

    [JsonObject()]
    public class Player
    {
        [JsonProperty("userid")]
        public string ID { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("membership")]
        public string Membership { get; set; }
        [JsonProperty("score")]
        public string Score { get; set; }
        [JsonProperty("games_completed")]
        public string GamesCompleted { get; set; }
        [JsonProperty("games_won")]
        public string GamesWon { get; set; }
        [JsonProperty("rank")]
        public string Rank { get; set; }
        [JsonProperty("rating")]
        public string Rating { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("attendance")]
        public string Attendance { get; set; }
        [JsonProperty("medals")]
        public string Medals { get; set; }
    }
}

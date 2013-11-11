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
    public enum TournamentState
    {
        Open = 'W',
        Ongoing = 'A',
        Completed = 'F',
        Abandoned = 'X'
    }

    public class TournamentArgs : EventArgs
    {
        private List<Tournament> gl;

        public List<Tournament> Users { get { return gl; } set { gl = value; } }

        public TournamentArgs(List<Tournament> gl) { this.gl = gl; }
    }

    public delegate void TournamentsEventHandler(object sender, TournamentArgs e);

    /// <summary>
    /// The GamesList is used to retrieve a list of games from the Conquer Club API
    /// </summary>
    public class TournamentClient
    {
        //The base URL used to get the list of games
        private static readonly string _url = @"http://www.conquerclub.com/api.php?mode=tournamentlist&names=Y";


        public event TournamentsEventHandler OnGetTournamentComplete;
        public event EventHandler OnGetTournamentError;

        /// <summary>
        /// This is the master function for getting a game list.
        /// Used for searching for games.
        /// </summary>
        /// <param name="pn">the name of the player to get data of</param>
        public void GetTournaments(TournamentState state)
        {
            //Build request string
            string request = _url;

            //Pass request string to web accessor
            BeginFetchResult(request + "&status=" + (char)state);
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

                data = data.Replace("<api>", "<api xmlns:json='http://james.newtonking.com/projects/json'> id='1'");
                data = data.Replace("<winner>", "<winner json:Array='true'>");
                data = data.Replace("<organizer ", "<organizer json:Array='true' ");

                XDocument doc = XDocument.Parse(data);

                string todeserialize = JsonConvert.SerializeXNode(doc);

                try
                {
                    TournamentData res = JsonConvert.DeserializeObject<TournamentData>(todeserialize);

                    //List<Tournament> ret = new List<Tournament>();
                    //foreach (TournamentInfo ti in res.Data.Data)
                    //    ret.Add(ti.Data);

                    CallCompleted(res.Data.Data.Data);
                }
                catch (JsonSerializationException jse)
                {
                    CallCompleted(null);
                }
            }
        }

        private void CallCompleted(List<Tournament> result)
        {
            if (result != null)
            {
                if (OnGetTournamentComplete != null)
                    OnGetTournamentComplete(result, new TournamentArgs(result));
            }
            else
            {
                if (OnGetTournamentError != null)
                    OnGetTournamentError(result, new EventArgs());
            }
        }
    }

    [JsonObject()]
    public class TournamentData
    {
        [JsonProperty("api")]
        public TournamentResponse Data { get; set; }
    }

    [JsonObject()]
    public class TournamentResponse
    {
        [JsonProperty("tournaments")]
        public TournamentInfo Data { get; set; }
    }

    [JsonObject()]
    public class TournamentInfo
    {
        [JsonProperty("tournament")]
        public List<Tournament> Data { get; set; }
    }

    [JsonObject()]
    public class TournamentOrganizers
    {
        [JsonProperty("organizer")]
        public List<TournamentOrganizer> Organizers { get; set; }
    }

    [JsonObject()]
    public class TournamentOrganizer
    {
        [JsonProperty("#text")]
        public string Name { get; set; }

        [JsonProperty("@removed")]
        public string Removed { get; set; }
    }

    [JsonObject()]
    public class TournamentWinners
    {
        [JsonProperty("winner")]
        public List<TournamentWinner> Winners { get; set; }
    }

    [JsonObject()]
    public class TournamentWinner
    {
        [JsonProperty("#text")]
        public string Name { get; set; }
    }

    [JsonObject()]
    public class Tournament
    {
        [JsonProperty("tourney_number")]
        public string ID { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("topic")]
        public string topic { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("organizers")]
        public TournamentOrganizers Organizers { get; set; }
        [JsonProperty("winners")]
        public TournamentWinners Winners { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ConquerorClient
{
    public class MapListArgs : EventArgs
    {
        private Map mp;

        public Map Data { get { return mp; } set { mp = value; } }

        public MapListArgs(Map mp) { this.mp = mp; }
    }

    public delegate void MapListEventHandler(object sender, MapListArgs e);

    /// <summary>
    /// The GamesList is used to retrieve a list of games from the Conquer Club API
    /// </summary>
    public class MapFetcher
    {
        //The base URL used to get the list of games
        private static readonly string _url = @"http://www.conquerclub.com/api.php?mode=maplist";


        public event MapListEventHandler OnGetMapComplete;
        public event EventHandler OnGetMapError;

        /// <summary>
        /// This begins the request for data about a specific map
        /// </summary>
        /// <param name="mp"></param>
        public void GetMap(string mp)
        {
            //Map m = DataHandler.ShouldUpdateMap(mp);
            //if (m == null)
                BeginFetchResult(_url + "&mp=" + mp);
            //else CallCompleted(m);
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
                    StreamReader stream = new StreamReader(request.EndGetResponse(result).GetResponseStream());
                    data = stream.ReadToEnd();
                    stream.Dispose();
                }
                catch (WebException we) { CallCompleted(null); return; }

                XDocument doc = XDocument.Parse(data);
                //Set Games to parse as an array always:

                string todeserialize = JsonConvert.SerializeXNode(doc);

                try
                {
                    MapData res = JsonConvert.DeserializeObject<MapData>(todeserialize);
                    //DataHandler.UpdateMap(res.Data.Map.Data.Title, res.Data.Map.Data);
                    CallCompleted(res.Data.Map.Data);
                }
                catch (JsonSerializationException jse)
                {
                    CallCompleted(null);
                }
            }
        }

        private void CallCompleted(Map result)
        {
            if (result != null)
            {
                if (OnGetMapComplete != null)
                    OnGetMapComplete(result, new MapListArgs(result));
            }
            else
            {
                if (OnGetMapError != null)
                    OnGetMapError(result, new EventArgs());
            }
        }
    }

    [JsonObject()]
    public class MapData
    {
        [JsonProperty("api")]
        public MapsContents Data { get; set; }
    }

    [JsonObject()]
    public class MapsContents
    {
        [JsonProperty("maps")]
        public MapContainer Map { get; set; }
    }

    [JsonObject()]
    public class MapContainer
    {
        [JsonProperty("map")]
        public Map Data { get; set; }
    }

    [JsonObject()]
    public class Map
    {
        private static readonly string _forumURL = @"http://www.conquerclub.com/forum/viewtopic.php?f=358&t="; //Just add the Topic for link to forum topic
        private static readonly string _imageURL = @"http://maps.conquerclub.com/"; //Add Small, Large or Thumb to this to get a link to an image of the map

        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("xml")]
        public string XML { get; set; }
        [JsonProperty("small")]
        public string Small { get; set; }
        [JsonProperty("large")]
        public string Large { get; set; }
        [JsonProperty("thumbnail")]
        public string Thumbnail { get; set; }
        [JsonProperty("topic")]
        public string Topic { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("small_width")]
        public string SmallWidth { get; set; }
        [JsonProperty("small_height")]
        public string SmallHeight { get; set; }
        [JsonProperty("large_width")]
        public string LargeWidth { get; set; }
        [JsonProperty("large_height")]
        public string LargeHeight { get; set; }
        [JsonProperty("territories")]
        public string Territories { get; set; }

        public string StatusString
        {
            get
            {
                switch (Status.ToUpper()[0])
                {
                    case 'N':
                        return "Normal";
                    case 'B':
                        return "Beta";
                    case 'C':
                        return "Closed";
                    case 'R':
                        return "Random";
                    default:
                        return "Unknown";
                }
            }
        }

        public string TopicLink { get { return _forumURL + Topic; } }

        public string SmallImageLink { get { return _imageURL + Small; } }

        public string LargeImageLink { get { return _imageURL + Large; } }

        public string ThumbImageLink { get { return _imageURL + Thumbnail; } }
    }
}

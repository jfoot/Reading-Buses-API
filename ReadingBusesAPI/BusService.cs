using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReadingBusesAPI
{
    public class BusService
    {
        [JsonProperty("route_code")]
        public string ServiceId { get; set; }

        [JsonProperty("group_name")]
        public string BrandName { get; set; }

        private List<string> stops;
        internal List<string> Stops
        {
            get {
                if (stops == null)
                {
                    stops = JsonConvert.DeserializeObject<List<BusStop>>(
                         new System.Net.WebClient().DownloadString("https://rtl2.ods-live.co.uk/api/linePatterns?key=" + ReadingBuses.APIKey + "&service=" + ServiceId))
                             .Select(p => p.ActoCode).ToList();
                }
                return stops;
            }  
        }

        internal BusService() {}


        public string[] getLocationsActo()
        {
            return Stops.ToArray();
        }

        public BusStop[] getLocations()
        {
            BusStop[] temp = new BusStop[stops.Count];
            for (int i = 0; i < temp.Length; i++)
                temp[i] = ReadingBuses.getInstance().getLocation(Stops[i]);
            return temp;
        }

        public void printLocationsActo()
        {
            foreach(var stop in Stops)
                Console.WriteLine(stop);
        }

        public void printLocationNames()
        {
            foreach(var stop in getLocations())
                Console.WriteLine(stop.CommonName);
        }
    }
}

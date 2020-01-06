using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReadingBusesAPI
{
    public sealed class BusService
    {
        [JsonProperty("route_code")]
        public string ServiceId { get; internal set; }

        [JsonProperty("group_name")]
        public string BrandName { get; internal set; }

        private BusStop[] stopsObjects;

        /// <summary>
        /// Gets a list of bus stops acto codes, if this is the first time it's asked for call upon the API
        /// This is delayed so only to call the API when needed.
        /// </summary>
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

        /// <summary>
        /// Gets an array of stops the bus service travels too as an array of ActoCode
        /// </summary>
        /// <returns>An array of Acto-Codes for the stops visited by this services.</returns>
        public string[] getLocationsActo()
        {
            return Stops.ToArray();
        }

        /// <summary>
        /// Gets an array of stops the bus service travels too as an array of BusStop objects. 
        /// </summary>
        /// <returns>An array of BusStop objects for the stops visited by this service.</returns>
        public BusStop[] getLocations()
        {
            if(stopsObjects == null)
            {
                BusStop[] temp = new BusStop[stops.Count];
                for (int i = 0; i < temp.Length; i++)
                    if (ReadingBuses.getInstance().isLocation(Stops[i]))
                        temp[i] = ReadingBuses.getInstance().getLocation(Stops[i]);
                stopsObjects = temp;
            } 
            return stopsObjects;
        }

        /// <summary>
        /// Gets the Live GPS positions for all Vehicles operating on this service.
        /// </summary>
        /// <returns>An array of GPS data points for all vehicles currently operating on this service.</returns>
        public LivePosition[] GetLivePositions()
        {
            return ReadingBuses.getInstance().getLiveVehiclePositions().Where(o => o.ServiceId.ToUpper() == ServiceId.ToUpper()).ToArray();
        }

        /// <summary>
        /// Prints off all the Acto-codes for bus stops visited by the service.
        /// </summary>
        public void printLocationsActo()
        {
            foreach(var stop in Stops)
                Console.WriteLine(stop);
        }

        /// <summary>
        /// Prints off all the names for the bus stops visited by the service.
        /// </summary>
        public void printLocationNames()
        {
            foreach(var stop in getLocations())
                Console.WriteLine(stop.CommonName);
        }
    }
}

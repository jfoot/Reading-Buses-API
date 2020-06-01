using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReadingBusesAPI
{
    /// <summary>
    /// Stores information about an individual bus services
    /// </summary>
    public sealed class BusService
    {
        /// <value>The service number for the bus service, this is only guaranteed to be unique per operator, not in the API as a whole. For example Reading Buses and Newbury And District both operate a number '2' service. </value>
        [JsonProperty("route_code")]
        public string ServiceId { get; internal set; }
        /// <value>The brand name for the service, used mainly for Reading Buses services, such as Lion, Purple or Orange.</value>
        [JsonProperty("group_name")]
        public string BrandName { get; internal set; }
        /// <value>The operator short code.</value>
        [JsonProperty("operator_code")]
        internal string OperatorCodeS { get; set; }
        /// <value>The operator enum value.</value>
        public Operators OperatorCode => ReadingBuses.GetOperatorE(OperatorCodeS);

        /// <value>Stores a list of 'BusStops' objects the service visits.</value>
        private BusStop[] _stopsObjects;

        /// <value>Stores a list of bus stops acto-code/IDs the service visits.</value>
        private List<string> _stops;

        /// <summary>
        /// Gets a list of bus stops acto codes, if this is the first time it's asked for call upon the API
        /// This is delayed so only to call the API when needed.
        /// </summary>
        internal async Task<List<string>> GetStops()
        {
            if (_stops == null)
            {
                _stops = new List<string>();
                _stops = JsonConvert.DeserializeObject<List<BusStop>>(
                      await new System.Net.WebClient().DownloadStringTaskAsync("https://rtl2.ods-live.co.uk/api/linePatterns?key=" + ReadingBuses.APIKey + "&service=" + ServiceId))
                         .Select(p => p.ActoCode).ToList();
            }
            return _stops;
        }

        /// <summary>
        /// The default constructor, used for automatic phrasing of data.
        /// </summary>
        internal BusService() {}

        /// <summary>
        /// Gets an array of stops the bus service travels too as an array of ActoCode
        /// </summary>
        /// <returns>An array of Acto-Codes for the stops visited by this services.</returns>
        public async Task<string[]> GetLocationsActo() => (await GetStops()).ToArray();

        /// <summary>
        /// Gets an array of 'BusStop' objects the bus service travels too as an array of BusStop objects.
        /// If the API is invalid and links to a Bus Stop not in the list of locations it will simply be ignored.
        /// </summary>
        /// <returns>An array of BusStop objects for the stops visited by this service.</returns>
        public async Task<BusStop[]> GetLocations()
        {
            if(_stopsObjects == null)
            {
                List<string> actoCodes = await GetStops();

                BusStop[] temp = new BusStop[actoCodes.Count];
                for (int i = 0; i < temp.Length; i++)
                    if (ReadingBuses.GetInstance().IsLocation(actoCodes[i]))
                        temp[i] = ReadingBuses.GetInstance().GetLocation(actoCodes[i]);
                _stopsObjects = temp;
            } 
            return _stopsObjects;
        }

        /// <summary>
        /// Gets the Live GPS positions for all Vehicles operating on this service.
        /// </summary>
        /// <returns>An array of GPS data points for all vehicles currently operating on this service.</returns>
        public async Task<LivePosition[]> GetLivePositions() => (await ReadingBuses.GetInstance().GetLiveVehiclePositions()).Where(o => String.Equals(o.ServiceId, ServiceId, StringComparison.CurrentCultureIgnoreCase)).ToArray();

        /// <summary>
        /// Prints off all the Acto-codes for bus stops visited by the service.
        /// </summary>
        public void PrintLocationsActo()
        {
            foreach(var stop in GetStops().Result)
                Console.WriteLine(stop);
        }

        /// <summary>
        /// Prints off all the names for the bus stops visited by the service.
        /// </summary>
        public void PrintLocationNames()
        {
            foreach(var stop in GetLocations().Result)
                Console.WriteLine(stop.CommonName);
        }
    }
}

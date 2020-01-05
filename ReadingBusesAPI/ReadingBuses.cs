using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ReadingBusesAPI
{
    public sealed class ReadingBuses
    {
        internal static string APIKey { get; private set; }
        internal static Dictionary<string, BusStop> Locations { get; set; }
        internal List<BusService> Services { get; set; }

        private static ReadingBuses instance;


        /// <summary>
        /// Create a new Reading Buses libary object, this is the main control. 
        /// </summary>
        /// <param name="APIKey">The Reading Buses API Key, get your own from http://rtl2.ods-live.co.uk/cms/apiservice </param>
        private ReadingBuses(string APIKey)
        {
            ReadingBuses.APIKey = APIKey;

            try
            {
                findServices();
                findLocations();
            }
            catch (Exception)
            {
                throw new Exception("The API Key Entered is incorrect, please check you have a valid Reading Buses API Key.");
            }
        }

        /// <summary>
        /// Used to initially initialise the ReadingBuses Object 
        /// </summary>
        /// <param name="APIKey">The Reading Buses API Key, get your own from http://rtl2.ods-live.co.uk/cms/apiservice </param>
        /// <returns></returns>
        public static ReadingBuses initialise(string APIKey)
        {
            if (instance == null)
                instance = new ReadingBuses(APIKey);
            return instance;
        }

        /// <summary>
        /// You will never need more than one ReadingBuses object, a singelton is used to ensure you always get the same instance.
        /// </summary>
        /// <returns>Returns the ReadingBuses object to be used throughout your program.</returns>
        public static ReadingBuses getInstance()
        {
            if(instance == null)
                throw new Exception("You must first initialise the object before usage, call the 'initialise' function passing your API Key.");
            return instance;
        }


        /// <summary>
        /// Finds all the services operated by Reading Buses.
        /// </summary>
        private void findServices()
        {
            Services = JsonConvert.DeserializeObject<List<BusService>>(
                   new System.Net.WebClient().DownloadString("https://rtl2.ods-live.co.uk/api/services?key=" + APIKey))
                       .OrderBy(p => Convert.ToInt32(Regex.Replace(p.ServiceId, "[^0-9.]", ""))).ToList();
        }

        /// <summary>
        /// Finds all the bus stops visted by Reading Buses.
        /// </summary>
        private void findLocations()
        {
            var locations = JsonConvert.DeserializeObject<List<BusStop>>(
                new System.Net.WebClient().DownloadString("https://rtl2.ods-live.co.uk/api/busstops?key=" + APIKey));
            Locations = new Dictionary<string, BusStop>();

            foreach (var location in locations)
                if (!Locations.ContainsKey(location.ActoCode))
                    Locations.Add(location.ActoCode, location);
        }

        #region Locations
        /// <summary>
        /// Get a bus stop location based upon a bus stops location code
        /// </summary>
        /// <param name="actoCode">The code of the bus stop</param>
        /// <returns>A Bus Stop object for the Acto Code specifed.</returns>
        public BusStop getLocation(string actoCode)
        {
            if (Locations.ContainsKey(actoCode))
               return Locations[actoCode];
            else
                throw new Exception("A bus stop of that Acto Code can not be found, please make sure you have a valid Bus Stop Code.");
        }

        /// <summary>
        /// All the bus stop locations that Reading Buses Visits
        /// </summary>
        /// <returns>All the bus stops Reading Buses visits</returns>
        public BusStop[] getLocations()
        {
            return Locations.Values.ToArray();
        }
        #endregion

        #region Services
        /// <summary>
        /// All the Services Reading Buses Operates 
        /// </summary>
        /// <returns>All the Services Reading Buses Operates</returns>
        public BusService[] getServices()
        {
            return Services.ToArray();
        }

        /// <summary>
        /// Returns all services Reading Buses Operates under a brand name, for example "pink" would return "22,25,27,29" services.
        /// </summary>
        /// <param name="BrandName">The brand name for the services you wish to find, eg "pink" or "sky blue".</param>
        /// <returns>An array of Bus Services which are of the brand name specifed.</returns>
        public BusService[] getServices(string BrandName)
        {
            return Services.Where(o => o.BrandName.ToUpper() == BrandName.ToUpper()).ToArray();
        }

        /// <summary>
        /// Returns a single service which matches the Service Number passed,
        /// </summary>
        /// <param name="ServiceNumber">The service number/ID for the service you wish to be returned eg: 17 or 22.</param>
        /// <returns>The service matching the ID.</returns>
        public BusService getService(string ServiceNumber)
        {
            return Services.Single(o => o.ServiceId.ToUpper() == ServiceNumber.ToUpper());
        }

        /// <summary>
        /// Prints off all the services found by the API which Reading Buses Operates
        /// </summary>
        public void printServices()
        {
            foreach (var service in Services)
                Console.WriteLine(service.BrandName + " " + service.ServiceId);
        }

        #endregion
    }
}

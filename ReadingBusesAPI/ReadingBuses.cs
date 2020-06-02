// Copyright (c) Jonathan Foot. All Rights Reserved. 
// Licensed under the GNU Affero General Public License, Version 3.0 
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ReadingBusesAPI
{
    /// <summary>
    ///     This is the main class for the library, here you can initialise a singleton instance and then query and use the
    ///     Reading Buses API.
    /// </summary>
    /// <example>
    ///     <code>
    /// //Optional
    /// ReadingBuses.SetCache(true);
    /// ReadingBuses Controller =  await ReadingBuses.Initialise("API KEY HERE");
    /// 
    /// BusService service = Controller.GetService("17"); or ReadingBuses.GetInstance().GetService("17");
    /// </code>
    /// </example>
    /// <remarks>
    ///     Cached Data is data stored locally in JSON and XML files, stored in a hidden folder called "cache", in the same
    ///     directory the program is executed from.
    ///     This is a copy of the results from an API call, such as the bus services and bus stops, because it is unlikely for
    ///     this data to change regularly.
    ///     By default the cached data will be updated every 7 days, but you can request new data or disable cache if you wish.
    ///     Caching data is however faster
    ///     as you do not need to keep making API requests for data likely to be the same.
    /// </remarks>
    public sealed class ReadingBuses
    {
        /// <value>The singleton instance</value>
        private static ReadingBuses _instance;

        /// <value>Keeps track of if cache data is being used or not</value>
        private static bool _cache = true;

        /// <value>Keeps track of if warnings are being outputted to console or not.</value>
        private static bool _warning = true;

        /// <value>Keeps track of if full error logs are being outputted to console or not.</value>
        private static bool _fullError = false;

        /// <value>Stores how many days cache data is valid for in days before being regenerated</value>
        private static int _cacheValidityLength = 7;

        /// <value>Holds the cache data for live GPS of vehicles.</value>
        private LivePosition[] _livePositionCache;


        /// <summary>
        ///     Create a new Reading Buses library object, this is the main control.
        /// </summary>
        /// <param name="APIkey">The Reading Buses API Key, get your own from http://rtl2.ods-live.co.uk/cms/apiservice </param>
        private ReadingBuses(string APIkey) => APIKey = APIkey;

        /// <value>Holds the users API Key.</value>
        internal static string APIKey { get; private set; }

        /// <value>Holds information on all the bus stops/locations visited by Reading Buses</value>
        internal static Dictionary<string, BusStop> Locations { get; set; }

        /// <value>Holds information on all the services operated by Reading Buses</value>
        internal List<BusService> Services { get; set; }


        /// <summary>
        ///     Creates cache data and retrieves bus services and bus stop data.
        /// </summary>
        /// <returns></returns>
        private async Task SetUp()
        {
            try
            {
                if (!Directory.Exists("cache"))
                {
                    Directory.CreateDirectory("cache");
                    DirectoryInfo ch = new DirectoryInfo("cache")
                    {
                        Attributes = FileAttributes.Hidden
                    };
                }

                Task<List<BusService>> servicesTask = FindServices();
                Task<Dictionary<string, BusStop>> locationsTask = FindLocations();

                Locations = await locationsTask;
                Services = await servicesTask;
            }
            catch (Exception ex)
            {
                _instance = null;
                PrintFullErrorLogs(ex.Message);
                throw new InvalidOperationException(
                    "The API Key Entered is incorrect, please check you have a valid Reading Buses API Key.");
            }
        }

        /// <summary>
        ///     Sets if you want to cache data into local files or always get new data from the API, which will take longer.
        /// </summary>
        /// <param name="value">True or False for if you want to get Cache or live data.</param>
        /// <exception cref="InvalidOperationException">
        ///     Thrown if you attempt to change the cache options after the library has
        ///     been instantiated
        /// </exception>
        public static void SetCache(bool value)
        {
            if (_instance == null)
                _cache = value;
            else
                throw new InvalidOperationException(
                    "Cache Storage Setting can not be changed once ReadingBuses Object is initialized.");
        }


        /// <summary>
        ///     Sets if you want to print out warning messages to the console screen or not.
        /// </summary>
        /// <param name="value">True or False for printing warning messages.</param>
        public static void SetWarning(bool value) => _warning = value;

        /// <summary>
        ///     Sets if you want to print out the full error logs to console, only needed for debugging library errors.
        /// </summary>
        /// <param name="value">True or False for printing full error logs to console.</param>
        public static void SetFullError(bool value) => _fullError = value;

        /// <summary>
        ///     Sets how long to keep Cache data for before invalidating it and getting new data.
        /// </summary>
        /// <param name="days">The number of days to store the cache data for before getting new data.</param>
        public static void SetCacheVadilityLength(int days) => _cacheValidityLength = days;

        /// <summary>
        ///     Deletes any Cache data stored, Cache data is deleted automatically after a number of days, use this only if you
        ///     need to force new data early.
        /// </summary>
        public static void InvalidateCache() => Directory.Delete("cache", true);

        /// <summary>
        ///     Internal method for printing warning messages to the console screen.
        /// </summary>
        /// <param name="message">The message to print off to console.</param>
        internal static void PrintWarning(string message)
        {
            if (_warning)
                Console.WriteLine(message);
        }

        /// <summary>
        ///     Internal method for printing warning messages to the console screen.
        /// </summary>
        /// <param name="message">The message to print off to console.</param>
        internal static void PrintFullErrorLogs(string message)
        {
            if (_fullError)
                Console.WriteLine(message);
        }


        /// <summary>
        ///     Used to initially initialise the ReadingBuses Object, it is recommended you do this in your programs start up.
        /// </summary>
        /// <param name="APIKey">The Reading Buses API Key, get your own from http://rtl2.ods-live.co.uk/cms/apiservice </param>
        /// <returns>An instance of the library controller. This same instance can be got by calling the "GetInstance" method.</returns>
        /// <exception cref="InvalidOperationException">Can throw an exception if you pass an invalid or expired API Key.</exception>
        /// See
        /// <see cref="ReadingBuses.GetInstance()" />
        /// to get any future instances afterwards.
        public static async Task<ReadingBuses> Initialise(string APIKey)
        {
            if (_instance == null)
            {
                _instance = new ReadingBuses(APIKey);
                await _instance.SetUp();
            }

            return _instance;
        }

        /// <summary>
        ///     You will never need more than one ReadingBuses object, a singleton is used to ensure you always get the same
        ///     instance.
        /// </summary>
        /// <returns>Returns the ReadingBuses object to be used throughout your program.</returns>
        /// <exception cref="InvalidOperationException">
        ///     Thrown if you attempt to get an instance before you have called the
        ///     "Initialise" function.
        /// </exception>
        /// See
        /// <see cref="ReadingBuses.Initialise(string)" />
        /// to initially initialise the ReadingBuses Object singelton.
        public static ReadingBuses GetInstance()
        {
            if (_instance == null)
                throw new InvalidOperationException(
                    "You must first initialise the object before usage, call the 'Initialise' function passing your API Key.");
            return _instance;
        }


        /// <summary>
        ///     Finds all the services operated by Reading Buses.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if an invalid or expired API Key is used.</exception>
        private async Task<List<BusService>> FindServices()
        {
            if (!File.Exists("cache\\Services.cache") || !_cache)
            {
                var newServicesData = JsonConvert.DeserializeObject<List<BusService>>(
                        await new WebClient().DownloadStringTaskAsync(
                            "https://rtl2.ods-live.co.uk/api/services?key=" + APIKey))
                    .OrderBy(p => Convert.ToInt32(Regex.Replace(p.ServiceId, "[^0-9.]", ""))).ToList();

                // Save the JSON file for later use. 
                if (_cache)
                    await File.WriteAllTextAsync("cache\\Services.cache",
                        JsonConvert.SerializeObject(newServicesData, Formatting.Indented));

                return newServicesData;
            }
            else
            {
                DirectoryInfo ch = new DirectoryInfo("cache\\Services.cache");
                if ((DateTime.Now - ch.CreationTime).TotalDays > _cacheValidityLength)
                {
                    File.Delete("cache\\Services.cache");
                    PrintWarning("Warning: Cache data expired, downloading latest Services Data.");
                    return await FindServices();
                }
                else
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<List<BusService>>(
                            await File.ReadAllTextAsync("cache\\Services.cache"));
                    }
                    catch (Exception)
                    {
                        File.Delete("cache\\Services.cache");
                        PrintWarning("Warning: Unable to read Services Cache File, deleting and regenerating cache.");
                        return await FindServices();
                    }
                }
            }
        }

        /// <summary>
        ///     Finds all the bus stops visited by Reading Buses.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if an invalid or expired API Key is used.</exception>
        private async Task<Dictionary<string, BusStop>> FindLocations()
        {
            if (!File.Exists("cache\\Locations.cache") || !_cache)
            {
                var locations = JsonConvert.DeserializeObject<List<BusStop>>(
                    await new WebClient().DownloadStringTaskAsync(
                        "https://rtl2.ods-live.co.uk/api/busstops?key=" + APIKey));

                var locationsFiltered = new Dictionary<string, BusStop>();

                foreach (var location in locations)
                    if (!locationsFiltered.ContainsKey(location.ActoCode))
                        locationsFiltered.Add(location.ActoCode, location);

                if (_cache)
                    await File.WriteAllTextAsync("cache\\Locations.cache",
                        JsonConvert.SerializeObject(locationsFiltered,
                            Formatting.Indented)); // Save the JSON file for later use.       

                return locationsFiltered;
            }
            else
            {
                DirectoryInfo ch = new DirectoryInfo("cache\\Locations.cache");
                if ((DateTime.Now - ch.CreationTime).TotalDays > _cacheValidityLength)
                {
                    File.Delete("cache\\Locations.cache");
                    PrintWarning("Warning: Cache data expired, downloading latest Locations Data.");
                    return await FindLocations();
                }
                else
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<Dictionary<string, BusStop>>(
                            await File.ReadAllTextAsync("cache\\Locations.cache"));
                    }
                    catch (Exception)
                    {
                        File.Delete("cache\\Locations.cache");
                        PrintWarning("Warning: Unable to read Locations Cache File, deleting and regenerating cache.");
                        return await FindLocations();
                    }
                }
            }
        }


        #region Operators

        /// <summary>
        ///     Converts the short hand code for an operator into its Enum value, for example RGB stands for Reading Buses.
        /// </summary>
        /// <param name="operatorCodeS"></param>
        /// <returns>The Enum equivalent of the bus operator short code.</returns>
        internal static Operators GetOperatorE(string operatorCodeS)
        {
            return operatorCodeS switch
            {
                "RGB" => Operators.ReadingBuses,
                "KC" => Operators.Kennections,
                "N&D" => Operators.NewburyAndDistrict,
                _ => Operators.Other
            };
        }

        #endregion

        #region Locations

        /// <summary>
        ///     Get a bus stop location based upon a bus stops location code
        /// </summary>
        /// <param name="actoCode">The code of the bus stop</param>
        /// <returns>A Bus Stop object for the Acto Code specified.</returns>
        /// <exception cref="InvalidOperationException">
        ///     Thrown if the bus stop does not exist. You should first check with
        ///     'IsLocation' If there is any uncertainty.
        /// </exception>
        /// See
        /// <see cref="ReadingBuses.IsLocation(string)" />
        /// to check if it is a location.
        public BusStop GetLocation(string actoCode)
        {
            if (IsLocation(actoCode))
                return Locations[actoCode];

            throw new InvalidOperationException(
                "A bus stop of that Acto Code can not be found, please make sure you have a valid Bus Stop Code. You can use, the 'IsLocation' function to check beforehand.");
        }

        /// <summary>
        ///     All the bus stop locations that Reading Buses Visits
        /// </summary>
        /// <returns>All the bus stops Reading Buses visits</returns>
        public BusStop[] GetLocations() => Locations.Values.ToArray();

        /// <summary>
        ///     Checks to see if the acto code for the bus stop exists in the API feed or not.
        /// </summary>
        /// <param name="actoCode">The ID Code for a bus stop.</param>
        /// <returns>True or False depending on if the stop is in the API feed or not.</returns>
        public bool IsLocation(string actoCode) => Locations.ContainsKey(actoCode);

        #endregion


        #region Services

        /// <summary>
        ///     All the Services Reading Buses Operates
        /// </summary>
        /// <returns>All the Services Reading Buses Operates</returns>
        public BusService[] GetServices() => Services.ToArray();


        /// <summary>
        ///     Returns all services Reading Buses Operates under a brand name, for example "pink" would return "22,25,27,29"
        ///     services.
        /// </summary>
        /// <param name="brandName">The brand name for the services you wish to find, eg "pink" or "sky blue".</param>
        /// <returns>An array of Bus Services which are of the brand name specified.</returns>
        public BusService[] GetServices(string brandName) => Services
            .Where(o => string.Equals(o.BrandName, brandName, StringComparison.CurrentCultureIgnoreCase)).ToArray();

        /// <summary>
        ///     Returns a service which matches the Service Number passed,
        ///     because the Reading Buses API now supports, Kinnections and Newbury and District a service number can no longer be
        ///     considered unique.
        /// </summary>
        /// <param name="serviceNumber">The service number/ID for the service you wish to be returned eg: 17 or 22.</param>
        /// <returns>The services matching the ID.</returns>
        /// <exception cref="InvalidOperationException">
        ///     Thrown if the bus services does not exist. You should first check with
        ///     'IsService' If there is any uncertainty.
        /// </exception>
        /// See
        /// <see cref="ReadingBuses.IsService(string)" />
        /// to check if it is a service.
        public BusService[] GetService(string serviceNumber)
        {
            if (IsService(serviceNumber))
                return Services.Where(o =>
                    string.Equals(o.ServiceId, serviceNumber, StringComparison.CurrentCultureIgnoreCase)).ToArray();

            throw new InvalidOperationException(
                "The service number provided does not exist. You can check if it exists by calling 'IsService' first.");
        }


        /// <summary>
        ///     Returns a service which matches the Service Number passed and the bus operator.
        /// </summary>
        /// <param name="serviceNumber">The service number/ID for the service you wish to be returned eg: 17 or 22.</param>
        /// <param name="operators">The bus operator to search in, for example "ReadingBuses"</param>
        /// <returns>The services matching the ID.</returns>
        /// <exception cref="InvalidOperationException">
        ///     Thrown if the bus services does not exist. You should first check with
        ///     'IsService' If there is any uncertainty.
        /// </exception>
        /// See
        /// <see cref="ReadingBuses.IsService(string)" />
        /// to check if it is a service.
        public BusService GetService(string serviceNumber, Operators operators)
        {
            if (IsService(serviceNumber, operators))
                return Services.Single(o =>
                    string.Equals(o.ServiceId, serviceNumber, StringComparison.CurrentCultureIgnoreCase) &&
                    o.OperatorCode.Equals(operators));

            throw new InvalidOperationException(
                "The service number provided does not exist. You can check if it exists by calling 'IsService' first.");
        }


        /// <summary>
        ///     Checks to see if a service of that number exists or not in the API feed.
        /// </summary>
        /// <param name="serviceNumber">The service number to find.</param>
        /// <returns>True or False for if a service is the API feed or not.</returns>
        public bool IsService(string serviceNumber) => Services.Any(o =>
            string.Equals(o.ServiceId, serviceNumber, StringComparison.CurrentCultureIgnoreCase));


        /// <summary>
        ///     Checks to see if a service of that number exists or not in the API feed, for a specific bus operator.
        /// </summary>
        /// <param name="serviceNumber">The service number to find.</param>
        /// <param name="operators">The specific bus operator you want to search in.</param>
        /// <returns>True or False for if a service is the API feed or not.</returns>
        public bool IsService(string serviceNumber, Operators operators) => Services.Any(o =>
            string.Equals(o.ServiceId, serviceNumber, StringComparison.CurrentCultureIgnoreCase) &&
            o.OperatorCode.Equals(operators));


        /// <summary>
        ///     Prints off all the services found by the API which Reading Buses Operates
        /// </summary>
        public void PrintServices()
        {
            foreach (var service in Services)
                Console.WriteLine(service.BrandName + " " + service.ServiceId);
        }

        /// <summary>
        ///     Gets live GPS data for all buses currently operating.
        /// </summary>
        /// <returns>An array of GPS locations for all buses operating by Reading Buses currently</returns>
        /// <exception cref="InvalidOperationException">Thrown if the API key is invalid or expired.</exception>
        public async Task<LivePosition[]> GetLiveVehiclePositions()
        {
            if (LivePosition.RefreshCache() || _livePositionCache == null)
            {
                var download =
                    await new WebClient().DownloadStringTaskAsync(
                        new Uri("https://rtl2.ods-live.co.uk/api/vehiclePositions?key=" + APIKey));
                _livePositionCache = JsonConvert.DeserializeObject<LivePosition[]>(download).ToArray();
            }

            return _livePositionCache;
        }

        /// <summary>
        ///     Gets live GPS data for a single buses matching Vehicle ID number.
        /// </summary>
        /// <returns>The GPS point of Vehicle matching your ID provided.</returns>
        /// <exception cref="InvalidOperationException">
        ///     Thrown if a vehicle of the ID does not exist or is not currently active.
        ///     You can check by using the 'IsVehicle' function.
        /// </exception>
        public async Task<LivePosition> GetLiveVehiclePosition(string vehicle)
        {
            if (await IsVehicle(vehicle))
                return (await GetLiveVehiclePositions()).Single(o =>
                    string.Equals(o.Vehicle, vehicle, StringComparison.CurrentCultureIgnoreCase));

            throw new InvalidOperationException(
                "A Vehicle of that ID can not be found currently operating. You can first check with the 'IsVehicle' function.");
        }

        /// <summary>
        ///     Checks if the Vehicle ID Number is currently in service right now.
        /// </summary>
        /// <param name="vehicle">Vehicle ID Number eg 414</param>
        /// <returns>True or False for if the buses GPS can be found or not currently.</returns>
        public async Task<bool> IsVehicle(string vehicle) => (await GetLiveVehiclePositions()).Any(o =>
            string.Equals(o.Vehicle, vehicle, StringComparison.CurrentCultureIgnoreCase));

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ReadingBusesAPI
{
    /// <summary>
    ///     This classes simply gets all the buses stops visited by Reading Buses, by interfacing with the "List Of Bus Stops"
    ///     API.
    /// </summary>
    internal class Locations
    {

        /// <value>the location for the service cache file.</value>
        private readonly string _cacheLocation = "cache\\Locations.cache";


        /// <summary>
        ///     Finds all the bus stops visited by Reading Buses.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if an invalid or expired API Key is used.</exception>
        internal async Task<Dictionary<string, BusStop>> FindLocations()
        {
            if (!File.Exists(_cacheLocation) || !ReadingBuses.Cache)
            {
                var locations = JsonConvert.DeserializeObject<List<BusStop>>(
                    await new WebClient().DownloadStringTaskAsync(
                        "https://rtl2.ods-live.co.uk/api/busstops?key=" + ReadingBuses.APIKey));

                var locationsFiltered = new Dictionary<string, BusStop>();

                foreach (var location in locations)
                    if (!locationsFiltered.ContainsKey(location.ActoCode))
                        locationsFiltered.Add(location.ActoCode, location);

                if (ReadingBuses.Cache)
                    await File.WriteAllTextAsync(_cacheLocation,
                        JsonConvert.SerializeObject(locationsFiltered,
                            Formatting.Indented)); // Save the JSON file for later use.       

                return locationsFiltered;
            }
            else
            {
                DirectoryInfo ch = new DirectoryInfo(_cacheLocation);
                if ((DateTime.Now - ch.CreationTime).TotalDays > ReadingBuses.CacheValidityLength)
                {
                    File.Delete(_cacheLocation);
                    ReadingBuses.PrintWarning("Warning: Cache data expired, downloading latest Locations Data.");
                    return await FindLocations();
                }
                
                
                try
                {
                    return JsonConvert.DeserializeObject<Dictionary<string, BusStop>>(
                        await File.ReadAllTextAsync(_cacheLocation));
                }
                catch (JsonException)
                {
                    File.Delete(_cacheLocation);
                    ReadingBuses.PrintWarning(
                        "Warning: Unable to read Locations Cache File, deleting and regenerating cache.");
                    return await FindLocations();
                }
            }
        }
    }
}
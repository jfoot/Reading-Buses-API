// Copyright (c) Jonathan Foot. All Rights Reserved. 
// Licensed under the GNU Affero General Public License, Version 3.0 
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ReadingBusesAPI.Error_Management;
using ReadingBusesAPI.Shared;

namespace ReadingBusesAPI.Bus_Stops
{
    /// <summary>
    ///     This classes simply gets all the buses stops visited by Reading Buses, by interfacing with the "List Of Bus Stops"
    ///     API.
    /// </summary>
    internal class Locations
    {
        /// <value>the location for the service cache file.</value>
        private const string CacheLocation = "cache\\Locations.cache";


        /// <summary>
        ///     Finds all the bus stops visited by Reading Buses.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if an invalid or expired API Key is used.</exception>
        internal async Task<Dictionary<string, BusStop>> FindLocations()
        {
            if (!File.Exists(CacheLocation) || !ReadingBuses.Cache)
            {
                string json = await new WebClient().DownloadStringTaskAsync(UrlConstructor.ListOfBusStops())
                    .ConfigureAwait(false);
                var locationsFiltered = new Dictionary<string, BusStop>();

                try
                {
                    List<BusStop> locations = JsonConvert.DeserializeObject<List<BusStop>>(json);

                    foreach (var location in locations)
                        if (!locationsFiltered.ContainsKey(location.ActoCode))
                            locationsFiltered.Add(location.ActoCode, location);

                    if (ReadingBuses.Cache)
                        await File.WriteAllTextAsync(CacheLocation,
                            JsonConvert.SerializeObject(locationsFiltered,
                                Formatting.Indented)).ConfigureAwait(false); // Save the JSON file for later use.  
                }
                catch (JsonSerializationException)
                {
                    ErrorManagement.TryErrorMessageRetrieval(json);
                }

                return locationsFiltered;
            }
            else
            {
                DirectoryInfo ch = new DirectoryInfo(CacheLocation);
                if ((DateTime.Now - ch.CreationTime).TotalDays > ReadingBuses.CacheValidityLength)
                {
                    File.Delete(CacheLocation);
                    ReadingBuses.PrintWarning("Warning: Cache data expired, downloading latest Locations Data.");
                    return await FindLocations().ConfigureAwait(false);
                }


                try
                {
                    return JsonConvert.DeserializeObject<Dictionary<string, BusStop>>(
                        await File.ReadAllTextAsync(CacheLocation).ConfigureAwait(false));
                }
                catch (JsonSerializationException)
                {
                    File.Delete(CacheLocation);
                    ReadingBuses.PrintWarning(
                        "Warning: Unable to read Locations Cache File, deleting and regenerating cache.");
                    return await FindLocations().ConfigureAwait(false);
                }
            }
        }
    }
}
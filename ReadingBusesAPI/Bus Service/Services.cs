// Copyright (c) Jonathan Foot. All Rights Reserved. 
// Licensed under the GNU Affero General Public License, Version 3.0 
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ReadingBusesAPI
{
    /// <summary>
    ///     This classes simply gets all the bus services operated by Reading Buses, by interfacing with the "List Of Services"
    ///     API.
    /// </summary>
    internal class Services
    {
        /// <value>the location for the service cache file.</value>
        private readonly string _cacheLocation = "cache\\Services.cache";

        /// <summary>
        ///     Finds all the services operated by Reading Buses.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if an invalid or expired API Key is used.</exception>
        internal async Task<List<BusService>> FindServices()
        {
            if (!File.Exists(_cacheLocation) || !ReadingBuses.Cache)
            {
                var newServicesData = JsonConvert.DeserializeObject<List<BusService>>(
                        await new WebClient().DownloadStringTaskAsync(
                            "https://rtl2.ods-live.co.uk/api/services?key=" + ReadingBuses.APIKey))
                    .OrderBy(p => Convert.ToInt32(Regex.Replace(p.ServiceId, "[^0-9.]", ""))).ToList();

                // Save the JSON file for later use. 
                if (ReadingBuses.Cache)
                    await File.WriteAllTextAsync(_cacheLocation,
                        JsonConvert.SerializeObject(newServicesData, Formatting.Indented));

                return newServicesData;
            }
            else
            {
                DirectoryInfo ch = new DirectoryInfo(_cacheLocation);
                if ((DateTime.Now - ch.CreationTime).TotalDays > ReadingBuses.CacheValidityLength)
                {
                    File.Delete(_cacheLocation);
                    ReadingBuses.PrintWarning("Warning: Cache data expired, downloading latest Services Data.");
                    return await FindServices();
                }

                try
                {
                    return JsonConvert.DeserializeObject<List<BusService>>(
                        await File.ReadAllTextAsync(_cacheLocation));
                }
                catch (JsonException)
                {
                    File.Delete(_cacheLocation);
                    ReadingBuses.PrintWarning(
                        "Warning: Unable to read Services Cache File, deleting and regenerating cache.");
                    return await FindServices();
                }
            }
        }
    }
}
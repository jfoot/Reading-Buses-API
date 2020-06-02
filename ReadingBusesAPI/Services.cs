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
        /// <summary>
        ///     Finds all the services operated by Reading Buses.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if an invalid or expired API Key is used.</exception>
        internal async Task<List<BusService>> FindServices()
        {
            if (!File.Exists("cache\\Services.cache") || !ReadingBuses.Cache)
            {
                var newServicesData = JsonConvert.DeserializeObject<List<BusService>>(
                        await new WebClient().DownloadStringTaskAsync(
                            "https://rtl2.ods-live.co.uk/api/services?key=" + ReadingBuses.APIKey))
                    .OrderBy(p => Convert.ToInt32(Regex.Replace(p.ServiceId, "[^0-9.]", ""))).ToList();

                // Save the JSON file for later use. 
                if (ReadingBuses.Cache)
                    await File.WriteAllTextAsync("cache\\Services.cache",
                        JsonConvert.SerializeObject(newServicesData, Formatting.Indented));

                return newServicesData;
            }
            else
            {
                DirectoryInfo ch = new DirectoryInfo("cache\\Services.cache");
                if ((DateTime.Now - ch.CreationTime).TotalDays > ReadingBuses.CacheValidityLength)
                {
                    File.Delete("cache\\Services.cache");
                    ReadingBuses.PrintWarning("Warning: Cache data expired, downloading latest Services Data.");
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
                        ReadingBuses.PrintWarning(
                            "Warning: Unable to read Services Cache File, deleting and regenerating cache.");
                        return await FindServices();
                    }
                }
            }
        }
    }
}
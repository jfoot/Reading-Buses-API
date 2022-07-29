// Copyright (c) Jonathan Foot. All Rights Reserved. 
// Licensed under the GNU Affero General Public License, Version 3.0 
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using ReadingBusesAPI.Common;
using ReadingBusesAPI.ErrorManagement;

namespace ReadingBusesAPI.BusStops
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
					List<BusStopIntermediary> locations = JsonSerializer.Deserialize<List<BusStopIntermediary>>(json);

					foreach (var location in locations)
					{
						if (!locationsFiltered.ContainsKey(location.ActoCode))
						{
							locationsFiltered.Add(location.ActoCode, new BusStop(location));
						}
						else
						{
							locationsFiltered[location.ActoCode].Merge(location);
						}
					}


					if (ReadingBuses.Cache)
					{
						File.WriteAllText(CacheLocation,
						JsonSerializer.Serialize(locationsFiltered, new JsonSerializerOptions { WriteIndented = true })); // Save the JSON file for later use.  
					}
				}
				catch (JsonException)
				{
					ErrorManager.TryErrorMessageRetrieval(json);
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
					return JsonSerializer.Deserialize<Dictionary<string, BusStop>>(
						File.ReadAllText(CacheLocation));
				}
				catch (JsonException)
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

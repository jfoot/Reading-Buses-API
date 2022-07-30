// Copyright (c) Jonathan Foot. All Rights Reserved. 
// Licensed under the GNU Affero General Public License, Version 3.0 
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using ReadingBusesAPI.BusServices;
using ReadingBusesAPI.BusStops;
using ReadingBusesAPI.ErrorManagement;

namespace ReadingBusesAPI.Common
{

	/// <summary>
	/// Used to write a cache files to the disk.
	/// </summary>
	internal static class CacheWriter
	{

		public static readonly string CACHE_FOLDER = "cache";
		public static readonly string ARCHIVED_CACHE_FOLDER = "cache-archive";

		/// <summary>
		/// Checks is the data stored within cache, if so read it, else make a request to the live API and cache this result for next time.
		/// </summary>
		/// <typeparam name="T">The type of data we are wanting.</typeparam>
		/// <param name="cacheLocation">The location of cache on disk if it exists.</param>
		/// <param name="liveURL">The live url to find the data within the API.</param>
		/// <param name="useCache">States if we should check and use cache or not.</param>
		/// <returns></returns>
		public static async Task<T> ReadOrCreateCache<T>(string cacheLocation, string liveURL, bool useCache)
		{
			if (useCache && File.Exists(cacheLocation))
			{
				string jsonCache = File.ReadAllText(cacheLocation);

				try
				{
					return JsonSerializer.Deserialize<T>(jsonCache);
				}
				catch (JsonException)
				{
					File.Delete(cacheLocation);
					ReadingBuses.PrintWarning($"Cache read failed - Fetching from Live. : {liveURL}");
					return await ReadOrCreateCache<T>(cacheLocation, liveURL, false);
				}
			}
			else
			{
				string json = await new WebClient().DownloadStringTaskAsync(liveURL).ConfigureAwait(false);

				try
				{
					var result = JsonSerializer.Deserialize<T>(json);
					File.WriteAllText(cacheLocation, json);
					return result;
				}
				catch (JsonException)
				{
					ErrorManager.TryErrorMessageRetrieval(json);
					return default(T);
				}
			}
		}


		/// <summary>
		/// Creates the two hidden cache folders.
		/// </summary>
		public static void CreateCacheDirectory()
		{
			if (!Directory.Exists(CACHE_FOLDER))
			{
				Directory.CreateDirectory(CACHE_FOLDER);
				new DirectoryInfo(CACHE_FOLDER) { Attributes = FileAttributes.Hidden };
			}

			if (!Directory.Exists(ARCHIVED_CACHE_FOLDER))
			{
				Directory.CreateDirectory(ARCHIVED_CACHE_FOLDER);
				new DirectoryInfo(ARCHIVED_CACHE_FOLDER) { Attributes = FileAttributes.Hidden };
			}
		}


		/// <summary>
		/// The file name for a tracking history cache file.
		/// </summary>
		/// <param name="service">The bus service/</param>
		/// <param name="location">The bus stop</param>
		/// <param name="date">The date of the request.</param>
		/// <param name="vehicle">The vehicle id</param>
		/// <returns>The cache location.</returns>
		public static string TrackingHistory(BusService service, BusStop location, DateTime date, string vehicle)
		{
			return $"{ARCHIVED_CACHE_FOLDER}/TH_{service?.ServiceId}_{location?.ActoCode}_{date.ToShortDateString().Replace('/','-')}_{vehicle}.json";
		}

		/// <summary>
		/// The file name for a Timetabled Journey cache file.
		/// </summary>
		/// <param name="service">The bus service/</param>
		/// <param name="location">The bus stop</param>
		/// <param name="date">The date of the request.</param>
		/// <returns>The cache location.</returns>
		public static string TimetabledJourneys(BusService service, BusStop location, DateTime date)
		{
			return $"{ARCHIVED_CACHE_FOLDER}/TJ_{service?.ServiceId}_{location?.ActoCode}_{date.ToShortDateString().Replace('/', '-')}.json";
		}

		/// <summary>
		/// The file name for a the vehicle position history cache file.
		/// </summary>
		/// <param name="dateStartTime">The date of interest</param>
		/// <param name="timeSpan">The time length of interest.</param>
		/// <param name="vehicle">The vehicle id.</param>
		/// <returns>The cache location.</returns>
		internal static string VehiclePositionHistory(DateTime dateStartTime, TimeSpan timeSpan, string vehicle)
		{
			return $"{ARCHIVED_CACHE_FOLDER}/VPH_{vehicle}_{dateStartTime.ToShortDateString().Replace('/', '-')}_{dateStartTime.ToLongTimeString().Replace(':', '-')}_{timeSpan.TotalSeconds}.json";
		}
	}
}

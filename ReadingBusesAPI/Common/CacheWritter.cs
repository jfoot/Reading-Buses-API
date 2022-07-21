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
	internal class CacheWritter
	{

		public static readonly string CACHE_FOLDER = "cache";
		public static readonly string ARCHIVED_CACHE_FOLDER = "cache-archive";


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



		public static string TrackingHistory(BusService service, BusStop location, DateTime date, string vehicle)
		{
			return $"{ARCHIVED_CACHE_FOLDER}/{service?.ServiceId}_{location?.ActoCode}_{date.ToShortDateString().Replace('/','-')}_{vehicle}.json";
		}
	}
}

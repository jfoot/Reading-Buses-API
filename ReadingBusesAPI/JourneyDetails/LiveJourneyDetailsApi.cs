// Copyright (c) Jonathan Foot. All Rights Reserved. 
// Licensed under the GNU Affero General Public License, Version 3.0
// See the LICENSE file in the project root for more information.

using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using ReadingBusesAPI.BusServices;
using ReadingBusesAPI.Common;
using ReadingBusesAPI.TimeTable;

namespace ReadingBusesAPI.JourneyDetails
{
	/// <summary>
	///		Contains the logic to call upon the Live Journey Details API. 
	/// </summary>
	internal static class LiveJourneyDetailsApi
	{
		/// <summary>
		///		Calls upon the live journey api, all parameters are optional, null if unknown. 
		/// </summary>
		/// <param name="service">The bus service to filter by.</param>
		/// <param name="vehicle">The vehicle id to filter by.</param>
		/// <returns></returns>
		internal static async Task<HistoricJourney[]> GetLiveJourney(BusService service, string vehicle)
		{
			string liveURL = UrlConstructor.LiveJourneyDetails(service, vehicle);
			string json = await new WebClient().DownloadStringTaskAsync(liveURL).ConfigureAwait(false);

			try
			{
				return JsonSerializer.Deserialize<HistoricJourney[]>(json);
			}
			catch (Exception ex)
			{
				ReadingBuses.PrintFullErrorLogs(ex.Message);
				return null;
			}
		}
	}
}

// Copyright (c) Jonathan Foot. All Rights Reserved. 
// Licensed under the GNU Affero General Public License, Version 3.0 
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using ReadingBusesAPI.BusServices;
using ReadingBusesAPI.BusStops;
using ReadingBusesAPI.Common;
using ReadingBusesAPI.ErrorManagement;

namespace ReadingBusesAPI.TimeTable
{
	/// <summary>
	///		Contains the logic to call upon the Tracking History API.
	/// </summary>
	internal static class TrackingHistoryApi
	{
	
		/// <summary>
		///     Gets the actual arrival and departure times of a bus, by service, date, location and/or vehicle ID.
		/// </summary>
		/// <param name="service">The bus services you wish to view.</param>
		/// <param name="date">The date of the time table.</param>
		/// <param name="location">The location to get timetable data from.</param>
		/// <param name="vehicle">A bus/Vehicle ID number.</param>
		/// <returns>An array of time table records for the service or location or both</returns>
		/// <exception cref="ReadingBusesApiExceptionMalformedQuery">
		///     If you have tried to get data for a date in the future. Or if you have not provided any date, and/or you have not
		///     provided at least either the service or location or vehicle.
		/// </exception>
		/// <exception cref="ReadingBusesApiExceptionBadQuery">Thrown if the API responds with an error message.</exception>
		/// <exception cref="ReadingBusesApiExceptionCritical">Thrown if the API fails, but provides no reason.</exception>
		internal static async Task<HistoricJourney[]> GetTimeTable(BusService service, DateTime date,
			BusStop location, string vehicle)
		{
			if (date == null || date > DateTime.Now)
			{
				throw new ReadingBusesApiExceptionMalformedQuery(
					"You can not get past data for a date in the future, if you want time table data use the 'BusTimeTable' objects and functions instead.");
			}

			if (service == null && location == null && string.IsNullOrEmpty(vehicle))
			{
				throw new ReadingBusesApiExceptionMalformedQuery(
					"You must provide a date and a service and/or location for a valid query.");
			}


			string cacheLocation = CacheWriter.TrackingHistory(service, location, date, vehicle);
			string liveURL = UrlConstructor.TrackingHistory(service, location, date, vehicle);

			return await CacheWriter.ReadOrCreateCache<HistoricJourney[]>(cacheLocation, liveURL, ReadingBuses.ArchiveCache);
		}
	}
}

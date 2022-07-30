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
	///		Contains the logic to call upon the Scheduled Journeys API.
	/// </summary>
	internal static class ScheduledJourneysApi 
	{

		/// <summary>
		///     Gets the time table of a service or a location as one array of 'BusTimeTable' objects.
		/// </summary>
		/// <param name="service">The bus services you wish to view.</param>
		/// <param name="date">The date of the time table.</param>
		/// <param name="location">The location to get timetable data from.</param>
		/// <returns>An array of time table records for the service or location or both</returns>
		/// <exception cref="ReadingBusesApiExceptionMalformedQuery">
		///     If you have not provided any date, and/or you have not provided at least
		///     either the service or location.
		/// </exception>
		/// <exception cref="ReadingBusesApiExceptionBadQuery">Thrown if the API responds with an error message.</exception>
		/// <exception cref="ReadingBusesApiExceptionCritical">Thrown if the API fails, but provides no reason.</exception>
		internal static async Task<Journey[]> GetTimeTable(BusService service, DateTime date,
			BusStop location)
		{
			if (date == null || (service == null && location == null))
			{
				throw new ReadingBusesApiExceptionMalformedQuery(
					"You must provide a date and a service and/or location for a valid query.");
			}


			string cacheLocation = CacheWriter.TimetabledJourneys(service, location, date);
			string liveURL = UrlConstructor.TimetabledJourneys(service, location, date);

			return await CacheWriter.ReadOrCreateCache<Journey[]>(cacheLocation, liveURL, ReadingBuses.ArchiveCache);
		}
	}
}

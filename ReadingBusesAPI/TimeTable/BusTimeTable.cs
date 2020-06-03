﻿// Copyright (c) Jonathan Foot. All Rights Reserved. 
// Licensed under the GNU Affero General Public License, Version 3.0 
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ReadingBusesAPI.Bus_Service;
using ReadingBusesAPI.Bus_Stops;

namespace ReadingBusesAPI.TimeTable
{
    /// <summary>
    ///     Represents and retrieves information  about a scheduled/predicted single time table record, which means information
    ///     on one bus at one location. Related
    ///     to the "Timetabled Journeys" API.
    /// </summary>
    public class BusTimeTable : TimeTableRecord
    {
        /// <summary>
        ///     Default constructor to prevent creating an object directly outside the API.
        /// </summary>
        internal BusTimeTable()
        {
        }


        /// <summary>
        ///     Gets the time table of a service or a location as one array of 'BusTimeTable' objects.
        /// </summary>
        /// <param name="service">The bus services you wish to view.</param>
        /// <param name="date">The date of the time table.</param>
        /// <param name="location">The location to get timetable data from.</param>
        /// <returns>An array of time table records for the service or location or both</returns>
        /// <exception cref="InvalidOperationException">
        ///     If you have not provided any date, and/or you have not provided at least
        ///     either the service or location.
        /// </exception>
        internal static async Task<BusTimeTable[]> GetTimeTable(BusService service, DateTime date,
            BusStop location)
        {
            if (date == null || service == null && location == null)
                throw new InvalidOperationException(
                    "You must provide a date and a service and/or location for a valid query.");


            var timeTable = JsonConvert.DeserializeObject<List<BusTimeTable>>(
                await new WebClient().DownloadStringTaskAsync("https://rtl2.ods-live.co.uk/api/scheduledJourneys?key=" +
                                                              ReadingBuses.APIKey +
                                                              "&service=" + (service ?? new BusService("")).ServiceId +
                                                              "&date=" +
                                                              date.ToString("yyyy-MM-dd") + "&location=" +
                                                              (location ?? new BusStop("")).ActoCode));

            return timeTable.ToArray();
        }

        /// <summary>
        ///     Gets the time table for a service and groups it by a journey code instead of one continuous array of time table
        ///     entries.
        /// </summary>
        /// <param name="service">The bus services you wish to view.</param>
        /// <param name="date">The date of the time table.</param>
        /// <param name="location">The location to get timetable data from.</param>
        /// <returns>Returns an IGroupings of Arrays of 'BusTimeTable' records grouped by journey codes.</returns>
        /// <exception cref="InvalidOperationException">
        ///     If you have not provided any date, and/or you have not provided at least
        ///     either the service or location.
        /// </exception>
        internal static async Task<IGrouping<string, BusTimeTable>[]> GetGroupedTimeTable(BusService service,
            DateTime date,
            BusStop location)
        {
            return (await GetTimeTable(service, date, location))
                .GroupBy(x => x.JourneyCode).ToArray();
        }
    }
}
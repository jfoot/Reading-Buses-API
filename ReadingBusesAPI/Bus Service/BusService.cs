// Copyright (c) Jonathan Foot. All Rights Reserved. 
// Licensed under the GNU Affero General Public License, Version 3.0 
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ReadingBusesAPI.Bus_Stops;
using ReadingBusesAPI.Error_Management;
using ReadingBusesAPI.Shared;
using ReadingBusesAPI.TimeTable;
using ReadingBusesAPI.Vehicle_Positions;

namespace ReadingBusesAPI.Bus_Service
{
    /// <summary>
    ///     Stores information about an individual bus services. Related to the "List Of Services" API.
    /// </summary>
    public sealed class BusService
    {
        /// <value>Stores a list of bus stops acto-code/IDs the service visits.</value>
        private List<string> _stops;

        /// <value>Stores a list of 'BusStops' objects the service visits.</value>
        private BusStop[] _stopsObjects;

        /// <summary>
        ///     The default constructor, used for automatic phrasing of data.
        /// </summary>
        internal BusService()
        {
        }

        /// <summary>
        ///     Used to create a snub/ fake object for passing to function calls, if all you need to pass is an service number to
        ///     the function.
        /// </summary>
        /// <param name="serviceNumber">ID of the bus service.</param>
        /// <remarks>
        ///     Unless you are doing something very strange, you probably should not need to use this, it is more for testing
        ///     purposes.
        /// </remarks>
        public BusService(string serviceNumber)
        {
            ServiceId = serviceNumber;
            OperatorCode = Operators.Other;
        }

        /// <summary>
        ///     Used to create a snub/ fake object for passing to function calls, if all you need to pass is an service number to
        ///     the function.
        /// </summary>
        /// <param name="serviceNumber">ID of the bus service.</param>
        /// <param name="operators">The operator who runs the service.</param>
        /// <remarks>
        ///     Unless you are doing something very strange, you probably should not need to use this, it is more for testing
        ///     purposes.
        /// </remarks>
        public BusService(string serviceNumber, Operators operators)
        {
            ServiceId = serviceNumber;
            OperatorCode = operators;
        }


        /// <value>
        ///     The service number for the bus service, this is only guaranteed to be unique per operator, not in the API as a
        ///     whole. For example Reading Buses and Newbury And District both operate a number '2' service.
        /// </value>
        [JsonProperty("route_code")]
        public string ServiceId { get; internal set; }

        /// <value>The brand name for the service, used mainly for Reading Buses services, such as Lion, Purple or Orange.</value>
        [JsonProperty("group_name")]
        public string BrandName { get; internal set; }


        /// <value>The operator enum value.</value>
        [JsonProperty("operator_code")]
        [JsonConverter(typeof(ParseOperatorConverter))]
        public Operators OperatorCode { get; internal set; }


        /// <summary>
        ///     Gets a list of bus stops acto codes, if this is the first time it's asked for call upon the API
        ///     This is delayed so only to call the API when needed.
        /// </summary>
        /// <exception cref="ReadingBusesApiException">Thrown if you have an invalid or expired API key.</exception>
        private async Task<List<string>> GetStops()
        {
            if (_stops == null)
            {
                string json = await new WebClient().DownloadStringTaskAsync(
                    UrlConstructor.LinePatterns(this));
                _stops = new List<string>();

                try
                {
                    _stops = JsonConvert.DeserializeObject<List<BusStop>>(json)
                        .Select(p => p.ActoCode).ToList();
                }
                catch (JsonSerializationException)
                {
                    ErrorManagement.TryErrorMessageRetrieval(json);
                }
            }

            return _stops;
        }

        /// <summary>
        ///     Gets an array of stops the bus service travels too as an array of ActoCode
        /// </summary>
        /// <returns>An array of Acto-Codes for the stops visited by this services.</returns>
        public async Task<string[]> GetLocationsActo() => (await GetStops()).ToArray();

        /// <summary>
        ///     Gets an array of 'BusStop' objects the bus service travels too as an array of BusStop objects.
        ///     If the API is invalid and links to a Bus Stop not in the list of locations it will simply be ignored.
        /// </summary>
        /// <returns>An array of BusStop objects for the stops visited by this service.</returns>
        public async Task<BusStop[]> GetLocations()
        {
            if (_stopsObjects == null)
            {
                List<string> actoCodes = await GetStops();

                BusStop[] temp = new BusStop[actoCodes.Count];
                for (int i = 0; i < temp.Length; i++)
                    if (ReadingBuses.GetInstance().IsLocation(actoCodes[i]))
                        temp[i] = ReadingBuses.GetInstance().GetLocation(actoCodes[i]);
                _stopsObjects = temp;
            }

            return _stopsObjects;
        }

        /// <summary>
        ///     Gets the Live GPS positions for all Vehicles operating on this service.
        /// </summary>
        /// <returns>An array of GPS data points for all vehicles currently operating on this service.</returns>
        public async Task<LivePosition[]> GetLivePositions() =>
            (await ReadingBuses.GetInstance().GpsController.GetLiveVehiclePositions()).Where(o =>
                string.Equals(o.ServiceId, ServiceId, StringComparison.CurrentCultureIgnoreCase)).ToArray();

        /// <summary>
        ///     Prints off all the Acto-codes for bus stops visited by the service.
        /// </summary>
        public void PrintLocationsActo()
        {
            foreach (var stop in GetStops().Result)
                Console.WriteLine(stop);
        }

        /// <summary>
        ///     Prints off all the names for the bus stops visited by the service.
        /// </summary>
        public void PrintLocationNames()
        {
            foreach (var stop in GetLocations().Result)
                Console.WriteLine(stop.CommonName);
        }

        #region BusTimeTable

        /// <summary>
        ///     Gets the full bus time table, for a specific date.
        /// </summary>
        /// <param name="date">the date on which you want a timetable for.</param>
        /// <param name="location">
        ///     (optional) a specific bus stop you want timetables for, if null it will get a timetable for
        ///     every bus stop on route.
        /// </param>
        /// <returns>An array for the time table at a particular bus stop.</returns>
        /// <exception cref="ReadingBusesApiExceptionMalformedQuery">
        ///     If you have not provided any date.
        /// </exception>
        /// <exception cref="ReadingBusesApiExceptionBadQuery">Thrown if the API responds with an error message.</exception>
        /// <exception cref="ReadingBusesApiExceptionCritical">Thrown if the API fails, but provides no reason.</exception>
        public Task<BusTimeTable[]> GetTimeTable(DateTime date, BusStop location = null)
        {
            return BusTimeTable.GetTimeTable(this, date, location);
        }


        /// <summary>
        ///     Gets the time table for this specific bus service, split into groups by the journey code.
        /// </summary>
        /// <param name="date">The date on which you want the time table for.</param>
        /// <param name="location">
        ///     (optional) The specific bus stop you want time table data for. Leave as null if you want the
        ///     whole routes timetable.
        /// </param>
        /// <returns>A grouping of arrays of time table records based upon journey code.</returns>
        public Task<IGrouping<string, BusTimeTable>[]> GetGroupedTimeTable(DateTime date, BusStop location = null)
        {
            return BusTimeTable.GetGroupedTimeTable(this, date, location);
        }

        #endregion

        #region ArchivedBusTimeTable

        /// <summary>
        ///     Gets the archived real bus departure and arrival times along with their time table history for this service on a
        ///     specific date.
        /// </summary>
        /// <param name="date">the date on which you want a archived timetable data for. This should be a date in the past.</param>
        /// <param name="location">
        ///     (optional) a specific bus stop you want archived timetables for, if null it will get a timetable for
        ///     every bus stop on route.
        /// </param>
        /// <returns>An array of time table records, containing the scheduled and actual arrival and departure times of buses. </returns>
        /// <exception cref="ReadingBusesApiExceptionMalformedQuery">
        ///     If you have tried to get data for a date in the future. Or if you have not provided any date, and/or you have not
        ///     provided at least either the service or location or vehicle.
        /// </exception>
        /// <exception cref="ReadingBusesApiExceptionBadQuery">Thrown if the API responds with an error message.</exception>
        /// <exception cref="ReadingBusesApiExceptionCritical">Thrown if the API fails, but provides no reason.</exception>
        public Task<ArchivedBusTimeTable[]> GetArchivedTimeTable(DateTime date, BusStop location = null)
        {
            return ArchivedBusTimeTable.GetTimeTable(this, date, location, null);
        }


        /// <summary>
        ///     Gets the archived real bus departure and arrival times along with their time table history for this service on a
        ///     specific date, split into groups by the journey code.
        /// </summary>
        /// <param name="date">The date on which you want the time table for.  This should be a date in the past.</param>
        /// <param name="location">
        ///     (optional) The specific bus stop you want time table data for. Leave as null if you want the
        ///     whole routes timetable.
        /// </param>
        /// <returns>A grouping of arrays of time table records based upon journey code.</returns>
        /// <exception cref="ReadingBusesApiExceptionMalformedQuery">
        ///     If you have tried to get data for a date in the future. Or if you have not provided any date, and/or you have not
        ///     provided at least either the service or location or vehicle.
        /// </exception>
        /// <exception cref="ReadingBusesApiExceptionBadQuery">Thrown if the API responds with an error message.</exception>
        /// <exception cref="ReadingBusesApiExceptionCritical">Thrown if the API fails, but provides no reason.</exception>
        public Task<IGrouping<string, ArchivedBusTimeTable>[]> GetGroupedArchivedTimeTable(DateTime date,
            BusStop location = null)
        {
            return ArchivedBusTimeTable.GetGroupedTimeTable(this, date, location, null);
        }

        #endregion
    }
}
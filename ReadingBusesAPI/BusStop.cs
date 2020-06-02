// Copyright (c) Jonathan Foot. All Rights Reserved. 
// Licensed under the GNU Affero General Public License, Version 3.0 
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ReadingBusesAPI
{
    /// <summary>
    ///     Stores information about a single bus stop. Related to the "List Of Bus Stops" API.
    /// </summary>
    public sealed class BusStop
    {
        /// <summary>
        ///     The default constructor used for parsing data automatically.
        /// </summary>
        internal BusStop()
        {
        }

        /// <summary>
        ///     Used to create a snub/ fake object for passing to function calls, if all you need to pass is an acto-code to the
        ///     function.
        /// </summary>
        /// <param name="actoCode">ID of the bus stop.</param>
        /// <remarks>
        ///     Unless you are doing something very strange, you probably should not need to use this, it is more for testing
        ///     purposes.
        /// </remarks>
        public BusStop(string actoCode)
        {
            ActoCode = actoCode;
        }

        /// <value>The unique identifier for a bus stop.</value>
        [JsonProperty("location_code")]
        public string ActoCode { get; internal set; }

        /// <value>The public, easy to understand stop name.</value>
        [JsonProperty("description")]
        public string CommonName { get; internal set; }

        /// <value>The latitude of the bus stop</value>
        [JsonProperty("latitude")]
        public string Latitude { get; internal set; }

        /// <value>The longitude of the bus stop</value>
        [JsonProperty("longitude")]
        public string Longitude { get; internal set; }

        /// <value>The bearing of the bus stop</value>
        [JsonProperty("bearing")]
        public string Bearing { get; internal set; }

        /// <value>The services that travel to this stop, separated by '/'</value>
        /// See
        /// <see cref="BusStop.GetServices(Operators)" />
        /// to get a list of Service Objects.
        [JsonProperty("routes")]
        public string Services { get; internal set; }

        /// <value>The Brand/Group of buses that most frequently visit this stop. Such as Purple, for the Purple 17s.</value>
        [JsonProperty("group_name")]
        public string GroupName { get; internal set; }

        /// <summary>
        ///     Gets live data from a bus stop.
        /// </summary>
        /// <returns>Returns a list of Live Records, which are individual buses due to arrive at the bus stop.</returns>
        public List<LiveRecord> GetLiveData() => LiveRecord.GetLiveData(ActoCode);

        /// <summary>
        ///     Finds the 'BusService' object for all of the bus services which visit this stop.
        /// </summary>
        /// <param name="busOperator"></param>
        /// <returns>A list of BusService Objects for services which visit this bus stop.</returns>
        public List<BusService> GetServices(Operators busOperator)
        {
            string[] services = Services.Split('/');
            List<BusService> serviceObjects = new List<BusService>();

            foreach (var service in services)
                serviceObjects.Add(ReadingBuses.GetInstance().GetService(service, busOperator));

            return serviceObjects;
        }

        /// <summary>
        ///     Gets the geographical position of the bus stop.
        /// </summary>
        /// <returns>A Point Object for the position of the bus stop.</returns>
        public Point GetPoint() => new Point(double.Parse(Longitude), double.Parse(Latitude));

        /// <summary>
        ///     Gets time table data at this specific bus stop.
        /// </summary>
        /// <param name="date">The date you want time table data for.</param>
        /// <param name="service">
        ///     (optional) the service you want time table data for specifically. If null, you get time table
        ///     data for all services at this stop.
        /// </param>
        /// <returns></returns>
        public Task<BusTimeTable[]> GetTimeTable(DateTime date, BusService service = null)
        {
            return BusTimeTable.GetAggregateTimeTable(service ?? new BusService(""), date, this);
        }
    }
}
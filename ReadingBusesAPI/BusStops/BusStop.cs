// Copyright (c) Jonathan Foot. All Rights Reserved. 
// Licensed under the GNU Affero General Public License, Version 3.0 
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ReadingBusesAPI.BusServices;
using ReadingBusesAPI.Common;
using ReadingBusesAPI.ErrorManagement;
using ReadingBusesAPI.JourneyDetails;
using ReadingBusesAPI.TimeTable;

namespace ReadingBusesAPI.BusStops
{
	/// <summary>
	///     Stores information about a single bus stop. Related to the "List Of Bus Stops" API.
	/// </summary>
	public sealed class BusStop
	{

		/// <summary>
		///     The default constructor, used only for JSON Parsing.
		///     Will be made internal when System.Text.Json add support for internal constructors in a future update.
		/// </summary>
		[JsonConstructor]
		[Obsolete("Do not use, will be made internal when system.text.json supports parsing in future updates.")]
		public BusStop()
		{
		}

		/// <summary>
		/// Constructs a BusStop object from an intermediary bus stop object.
		/// </summary>
		/// <param name="intermediary">Intermediary bus stop.</param>
		internal BusStop(BusStopIntermediary intermediary)
		{
			ActoCode = intermediary.ActoCode;
			CommonName = intermediary.CommonName;
			Latitude = intermediary.Latitude;
			Longitude = intermediary.Longitude;
			Bearing = intermediary.Bearing;
			if(intermediary.GetService() != null)
				ServiceObjects.Add(intermediary.GetService());
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
		[JsonPropertyName("location_code")]
		[JsonInclude]
		public string ActoCode { get; internal set; }

		/// <value>The public, easy to understand stop name.</value>
		[JsonPropertyName("description")]
		[JsonInclude]
		public string CommonName { get; internal set; }

		/// <value>The latitude of the bus stop</value>
		[JsonPropertyName("latitude")]
		[JsonInclude]
		public string Latitude { get; internal set; }

		/// <value>The longitude of the bus stop</value>
		[JsonPropertyName("longitude")]
		[JsonInclude]
		public string Longitude { get; internal set; }

		/// <value>The bearing of the bus stop</value>
		[JsonPropertyName("bearing")]
		[JsonInclude]
		public string Bearing { get; internal set; }

		/// <value>A reference to the bus services at this stop.</value>
		[JsonInclude]
		[JsonConverter(typeof(ParseServiceObjects))]
		public List<BusService> ServiceObjects { get; internal set; } = new List<BusService>();

		/// <summary>
		/// Combines two bus stops together that are the same, but report different services that stop at the them.
		/// </summary>
		/// <param name="otherStop">The other stop to merge with this one.</param>
		internal void Merge(BusStopIntermediary otherStop)
		{
			ServiceObjects.Add(otherStop.GetService());
		}


		/// <summary>
		///     Gets live data from a bus stop.
		/// </summary>
		/// <returns>Returns a list of Live Records, which are individual buses due to arrive at the bus stop.</returns>
		public async Task<LiveRecord[]> GetLiveData()
		{
			return await Task.Run(() => LiveRecord.GetLiveData(ActoCode)).ConfigureAwait(false);
		}

		/// <summary>
		///     Finds the 'BusService' object for all of the bus services which visit this stop.
		/// </summary>
		/// <param name="busOperator"></param>
		/// <returns>A list of BusService Objects for services which visit this bus stop.</returns>
		public BusService[] GetServices(Company busOperator)
		{
			return ServiceObjects.ToArray();
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
		/// <returns>An array of time table records for a particular bus stop.</returns>
		/// <exception cref="ReadingBusesApiExceptionMalformedQuery">
		///     If you have not provided any date.
		/// </exception>
		/// <exception cref="ReadingBusesApiExceptionBadQuery">Thrown if the API responds with an error message.</exception>
		/// <exception cref="ReadingBusesApiExceptionCritical">Thrown if the API fails, but provides no reason.</exception>
		public Task<BusTimeTable[]> GetTimeTable(DateTime date)
		{
			return BusTimeTable.GetTimeTable(null, date, this);
		}


		/// <summary>
		///     Gets time table data at this specific bus stop.
		/// </summary>
		/// <param name="date">The date you want time table data for.</param>
		/// <param name="service">
		///     (optional) the service you want time table data for specifically. If null, you get time table
		///     data for all services at this stop.
		/// </param>
		/// <returns>An array of time table records for a particular bus stop.</returns>
		/// <exception cref="ReadingBusesApiExceptionMalformedQuery">
		///     If you have not provided any date.
		/// </exception>
		/// <exception cref="ReadingBusesApiExceptionBadQuery">Thrown if the API responds with an error message.</exception>
		/// <exception cref="ReadingBusesApiExceptionCritical">Thrown if the API fails, but provides no reason.</exception>
		public Task<BusTimeTable[]> GetTimeTable(DateTime date, BusService service)
		{
			return BusTimeTable.GetTimeTable(service, date, this);
		}


		/// <summary>
		///     Gets the archived real bus departure and arrival times along with their time table history at this specific bus
		///     stop.
		/// </summary>
		/// <param name="date">The date you want time table data for. This should be a date in the past.</param>
		/// <returns></returns>
		public Task<ArchivedBusTimeTable[]> GetArchivedTimeTable(DateTime date)
		{
			return ArchivedBusTimeTable.GetTimeTable(null, date, this, null);
		}


		/// <summary>
		///     Gets the archived real bus departure and arrival times along with their time table history at this specific bus
		///     stop.
		/// </summary>
		/// <param name="date">The date you want time table data for. This should be a date in the past.</param>
		/// <param name="service">
		///     (optional) the service you want time table data for specifically. If null, you get time table
		///     data for all services at this stop.
		/// </param>
		/// <returns></returns>
		public Task<ArchivedBusTimeTable[]> GetArchivedTimeTable(DateTime date, BusService service)
		{
			return ArchivedBusTimeTable.GetTimeTable(service, date, this, null);
		}
	}
}

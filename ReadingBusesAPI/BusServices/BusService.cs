// Copyright (c) Jonathan Foot. All Rights Reserved. 
// Licensed under the GNU Affero General Public License, Version 3.0 
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ReadingBusesAPI.BusStops;
using ReadingBusesAPI.Common;
using ReadingBusesAPI.ErrorManagement;
using ReadingBusesAPI.JourneyDetails;
using ReadingBusesAPI.TimeTable;
using ReadingBusesAPI.VehiclePositions;

namespace ReadingBusesAPI.BusServices
{
	/// <summary>
	///     Stores information about an individual bus services. Related to the "List Of Lines" API.
	/// </summary>
	public sealed class BusService
	{
		/// <value>
		/// Stores a list of bus stops acto-code/IDs the service visits.
		/// String is the acto code
		/// Bool store is out an OutBound Service or not.
		/// </value>
		private List<(string, bool)> _stops;

		/// <value>Stores a list of 'BusStops' objects the service visits going outbound.</value>
		private BusStop[] _stopsObjectsOutBound;

		/// <value>Stores a list of 'BusStops' objects the service visits going inbound.</value>
		private BusStop[] _stopsObjectsInBound;

		/// <summary>
		///     The default constructor, used only for JSON Parsing.
		///     Will be made internal when System.Text.Json add support for internal constructors in a future update.
		/// </summary>
		[JsonConstructor]
		[Obsolete("Do not use, will be made internal when system.text.json supports parsing in future updates.")]
		public BusService()
		{
		}

		/// <summary>
		///     Used to create a snub/ fake object for passing to function calls, if all you need to pass is an service number to
		///     the function. Makes operator code, "other" by default.
		/// </summary>
		/// <param name="serviceNumber">ID of the bus service.</param>
		/// <remarks>
		///     Unless you are doing something very strange, you probably should not need to use this, it is more for testing
		///     purposes.
		/// </remarks>
		public BusService(string serviceNumber)
		{
			ServiceId = serviceNumber;
			Company = Company.Other;
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
		public BusService(string serviceNumber, Company operators)
		{
			ServiceId = serviceNumber;
			Company = operators;
		}

		/// <value>
		///     The service number for the bus service, this is only guaranteed to be unique per operator, not in the API as a
		///     whole. For example Reading Buses and Newbury And District both operate a number '2' service.
		/// </value>
		[JsonPropertyName("route_code")]
		[JsonInclude]
		public string ServiceId { get; internal set; }

		/// <value>The brand name for the service, used mainly for Reading Buses services, such as Lion, Purple or Orange.</value>
		[JsonPropertyName("group_name")]
		[JsonInclude]
		public string BrandName { get; internal set; }


		/// <value>The operator of the service enum value.</value>
		[JsonPropertyName("operator_code")]
		[JsonConverter(typeof(ParseOperatorConverter))]
		[JsonInclude]
		public Company Company { get; internal set; }



		/// <summary>
		///     Gets a list of bus stops acto codes, if this is the first time it's asked for call upon the API
		///     This is delayed so only to call the API when needed.
		/// </summary>
		/// <exception cref="ReadingBusesApiException">Thrown if you have an invalid or expired API key.</exception>
		private async Task<List<(string, bool)>> GetStops()
		{
			if (_stops == null)
			{

				string json = await new WebClient().DownloadStringTaskAsync(
					UrlConstructor.LinePatterns(this)).ConfigureAwait(false);
				_stops = new List<(string, bool)>();

				try
				{
					//Goes through the results, filters out anything which isn't from the same operator
					//Then order it by the display order.
					//Then map it into a tuple, of Acto-code along with if it is outbound or not.
					_stops = JsonSerializer.Deserialize<List<StopPattern>>(json)
						.Where(b => b.OperatorCode.Equals(Company)).OrderBy(b => b.Order).Select(b => (b.ActoCode, b.IsOutbound())).ToList();
				}
				catch (JsonException)
				{
					ErrorManager.TryErrorMessageRetrieval(json);
				}
			}

			return _stops;
		}

		/// <summary>
		///     Gets an array of acto-codes for the bus stops that the services visits.
		///     The first set of results are the outbound, the final set are the inbound.
		/// </summary>
		/// <returns>An array of Acto-Codes for the stops visited by this services.</returns>
		public async Task<string[]> GetLocationsActo()
		{
			List<string> outbound = (await GetLocationsActo(Direction.Outbound)).ToList();
			List<string> inbound = (await GetLocationsActo(Direction.Inbound)).ToList();
			//Adds the inbound onto the end of the outbound.      
			outbound.AddRange(inbound);

			return outbound.ToArray();
		}

		/// <summary>
		///  Gets an array of acto-codes for the bus stops that the services visits.
		/// </summary>
		/// <param name="direction">Do you want outbound acto-codes or inbound.</param>
		/// <returns></returns>
		public async Task<string[]> GetLocationsActo(Direction direction)
		{
			List<(string, bool)> locations = (await GetStops().ConfigureAwait(false));
			//Filter out for only stops in the direction of travel requested and get the string into an array.
			return locations.Where(location => location.Item2 == direction.Equals(Direction.Outbound)).Select(id => id.Item1).ToArray();
		}


		/// <summary>
		///     Gets an array of 'BusStop' objects the bus service travels too as an array of BusStop objects.
		///     If the API is invalid and links to a Bus Stop not in the list of locations it will simply be ignored.
		///     Ordered on all the outbound stops first and then all the inbound stops.
		/// </summary>
		/// <returns>An array of BusStop objects for the stops visited by this service.</returns>
		public async Task<BusStop[]> GetLocations()
		{
			List<BusStop> outbound = (await GetLocations(Direction.Outbound)).ToList();
			List<BusStop> inbound = (await GetLocations(Direction.Inbound)).ToList();
			//Adds the inbound stops onto the outbound.
			outbound.AddRange(inbound);
			//Converts to an array and returns results.
			return outbound.ToArray();
		}

		/// <summary>
		///     Gets an array of 'BusStop' objects the bus service travels too as an array of BusStop objects.
		///     If the API is invalid and links to a Bus Stop not in the list of locations it will simply be ignored.
		/// </summary>
		/// <param name="direction">The direction for stops, outbound or inbound</param>
		/// <returns>returns back all the bus stop objects visited by the service, for the direction of travel specified.</returns>
		public async Task<BusStop[]> GetLocations(Direction direction)
		{
			if (direction.Equals(Direction.Outbound))
			{
				return _stopsObjectsOutBound ?? (_stopsObjectsOutBound = await GetBusStops(direction));
			}

			return _stopsObjectsInBound ?? (_stopsObjectsInBound = await GetBusStops(direction));
		}


		/// <summary>
		/// Gets the bus stop object associated with the acto-code for the bus stop.
		/// </summary>
		/// <param name="direction">do you want out bound or inbound stops.</param>
		/// <returns>An array of Bus Stop objects that the service visits.</returns>
		private async Task<BusStop[]> GetBusStops(Direction direction)
		{
			string[] actoCodes = await GetLocationsActo(direction);

			BusStop[] temp = new BusStop[actoCodes.Length];
			for (int i = 0; i < temp.Length; i++)
			{
				if (ReadingBuses.GetInstance().IsLocation(actoCodes[i]))
				{
					temp[i] = ReadingBuses.GetInstance().GetLocation(actoCodes[i]);
				}
			}

			return temp;
		}



		/// <summary>
		///     Gets the Live GPS positions for all Vehicles operating on this service.
		/// </summary>
		/// <returns>An array of GPS data points for all vehicles currently operating on this service.</returns>
		public async Task<LiveVehiclePosition[]> GetLivePositions() =>
			(await ReadingBuses.GetInstance().GpsController.GetLiveVehiclePositions().ConfigureAwait(false)).Where(o =>
				string.Equals(o.ServiceId, ServiceId, StringComparison.CurrentCultureIgnoreCase) && o.Company.Equals(Company)).ToArray();


		/// <summary>
		/// Gets live journey tracking information for this service.
		/// </summary>
		/// <returns>The live journey tracing information for this service.</returns>
		public async Task<HistoricJourney[]> GetLiveJourneyData()
		{
			return (await LiveJourneyDetailsApi.GetLiveJourney(this, null))
				.Where(ser => ser.Company.Equals(Company)).ToArray();
		}


		#region BusTimeTable

		/// <summary>
		///     Gets the full bus time table, for a specific date.
		/// </summary>
		/// <param name="date">the date on which you want a timetable for.</param>
		/// <returns>An array for the time table at a particular bus stop.</returns>
		/// <exception cref="ReadingBusesApiExceptionMalformedQuery">
		///     If you have not provided any date.
		/// </exception>
		/// <exception cref="ReadingBusesApiExceptionBadQuery">Thrown if the API responds with an error message.</exception>
		/// <exception cref="ReadingBusesApiExceptionCritical">Thrown if the API fails, but provides no reason.</exception>
		public async Task<Journey[]> GetTimeTable(DateTime date)
		{
			return (await ScheduledJourneysApi.GetTimeTable(this, date, null))
				.Where(ser => ser.Company.Equals(Company)).ToArray();
		}


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
		public async Task<Journey[]> GetTimeTable(DateTime date, BusStop location)
		{
			return (await ScheduledJourneysApi.GetTimeTable(this, date, location))
				.Where(ser => ser.Company.Equals(Company)).ToArray();
		}



		#endregion

		#region ArchivedBusTimeTable

		/// <summary>
		///     Gets the archived real bus departure and arrival times along with their time table history for this service on a
		///     specific date.
		/// </summary>
		/// <param name="date">the date on which you want a archived timetable data for. This should be a date in the past.</param>
		/// <returns>An array of time table records, containing the scheduled and actual arrival and departure times of buses. </returns>
		/// <exception cref="ReadingBusesApiExceptionMalformedQuery">
		///     If you have tried to get data for a date in the future. Or if you have not provided any date, and/or you have not
		///     provided at least either the service or location or vehicle.
		/// </exception>
		/// <exception cref="ReadingBusesApiExceptionBadQuery">Thrown if the API responds with an error message.</exception>
		/// <exception cref="ReadingBusesApiExceptionCritical">Thrown if the API fails, but provides no reason.</exception>
		public async Task<HistoricJourney[]> GetArchivedTimeTable(DateTime date)
		{
			return (await TrackingHistoryApi.GetTimeTable(this, date, null, null))
				.Where(ser => ser.Company.Equals(Company)).ToArray();
		}


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
		public async Task<HistoricJourney[]> GetArchivedTimeTable(DateTime date, BusStop location)
		{
			return (await TrackingHistoryApi.GetTimeTable(this, date, location, null))
				.Where(ser => ser.Company.Equals(Company)).ToArray();
		}

		#endregion
	}
}

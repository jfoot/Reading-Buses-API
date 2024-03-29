﻿// Copyright (c) Jonathan Foot. All Rights Reserved. 
// Licensed under the GNU Affero General Public License, Version 3.0 
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using ReadingBusesAPI.BusServices;
using ReadingBusesAPI.BusStops;
using ReadingBusesAPI.Common;
using ReadingBusesAPI.ErrorManagement;
using ReadingBusesAPI.TimeTable;

namespace ReadingBusesAPI.JourneyDetails
{
	/// <summary>
	///     Used to store information about a buses arrival at a bus stop. Mainly related to the "Stop Predictions" API.
	/// </summary>
	public sealed class LiveRecord
	{
		/// <summary>
		///     The default constructor, used for XML parsing.
		/// </summary>
		internal LiveRecord()
		{
		}

		/// <value>Holds the Service Number for the bus route.</value>
		public string ServiceNumber { get; internal set; }

		/// <value>Holds the destination for the bus.</value>
		public string DestinationName { get; internal set; }

		/// <value>Holds scheduled departure time of the bus at the location.</value>
		public DateTime? ScheduledDeparture { get; internal set; }

		/// <value>Holds the estimated/ expected departure time of the bus, if Null no estimated time exists yet.</value>
		public DateTime? ExpectedDeparture { get; internal set; }

		/// <value>Holds the estimated/ expected arrival time of the bus, if Null no estimated time exists yet.</value>
		public DateTime? ExpectedArrival { get; internal set; }

		/// <value>Holds the estimated/ expected arrival time of the bus, if Null no scheduled time exists.</value>
		public DateTime? ScheduledArrival { get; internal set; }

		/// <value>Holds the operator of the service.</value>
		public Company OperatorCode { get; internal set; }

		/// <value>Holds the Vehicles reference ID or number to identify it.</value>
		public string VehicleRef { get; internal set; }

		/// <value>The acto-code for the destination stop.</value>
		internal string _destination;

		/// <value>The acto-code for the origin stop.</value>
		internal string _origin;


		/// <summary>
		/// Gets the origin bus stop object. Null if unknown.
		/// </summary>
		/// <returns>Bus Stop for where this vehicle originated from.</returns>
		public BusStop GetOriginStop()
		{
			if (ReadingBuses.GetInstance().IsLocation(_origin))
				return ReadingBuses.GetInstance().GetLocation(_origin);

			return null;
		}

		/// <summary>
		/// Gets the destination bus stop object. Null if unknown.
		/// </summary>
		/// <returns>Bus Stop for where this vehicle is destining.</returns>
		public BusStop GetDestinationStop()
		{
			if (ReadingBuses.GetInstance().IsLocation(_destination))
				return ReadingBuses.GetInstance().GetLocation(_destination);

			return null;
		}


		/// <summary>
		///     Returns the related BusService Object for the Bus LiveRecord.
		/// </summary>
		/// <returns>Information about the current bus service object.</returns>
		/// <exception cref="InvalidOperationException">
		///     Can throw an exception if the service does not exists. This is however very
		///     unlikely, if this occurs there is an error in the API, not with your code.
		/// </exception>
		public BusService Service()
		{
			return ReadingBuses.GetInstance().GetService(ServiceNumber, OperatorCode);
		}

		/// <summary>
		/// Gets live journey tracking information for this vehicle.
		/// </summary>
		/// <returns>The live journey tracing information for this vehicle.</returns>
		public async Task<HistoricJourney[]> GetLiveJourneyData()
		{
			return (await LiveJourneyDetailsApi.GetLiveJourney(null, new Regex("[^0-9]").Replace(VehicleRef, "")))
				.Where(journey => journey.Company.Equals(OperatorCode)).ToArray();
		}
		

		/// <summary>
		///     Returns the number of min till bus is due in a min format.
		/// </summary>
		/// <returns>The number of min until the bus is due to arrive in string format.</returns>
		public string DisplayTime()
		{
			if (ScheduledDeparture != null) 
				return ((ExpectedDeparture ?? (DateTime)ScheduledDeparture) - DateTime.Now).TotalMinutes.ToString("0") + " mins";
			if(ScheduledArrival != null) 
				return ((ExpectedArrival ?? (DateTime)ScheduledArrival) - DateTime.Now).TotalMinutes.ToString("0") + " mins";

			return "";
		}

		/// <summary>
		///     Returns the number of min till the bus is due to arrive.
		/// </summary>
		/// <returns>The number of min till the bus is due to arrive.</returns>
		public double ArrivalMin()
		{
			if (ScheduledDeparture != null)
					return ((ExpectedDeparture ?? (DateTime)ScheduledDeparture) - DateTime.Now).TotalMinutes;
			if (ScheduledArrival != null)
				return ((ExpectedArrival ?? (DateTime)ScheduledArrival) - DateTime.Now).TotalMinutes;
			return 0;
		}

		/// <summary>
		///     Gets a list of upcoming arrivals at a specific bus stop. Can throw an exception.
		/// </summary>
		/// <param name="actoCode">The Acto-code ID for a specific bus stop.</param>
		/// <returns>A list of Live Records containing details about upcoming buses.</returns>
		/// <exception cref="ReadingBusesApiExceptionMalformedQuery">Thrown if no data is returned from the API.</exception>
		/// <exception cref="ReadingBusesApiExceptionBadQuery">
		///     Thrown if you have used an invalid or expired API key or an invalid
		///     acto-code
		/// </exception>
		/// <exception cref="ReadingBusesApiExceptionCritical">Thrown if no error message or reasoning for fault is detectable.</exception>
		internal static LiveRecord[] GetLiveData(string actoCode)
		{
			try
			{
				var d = UrlConstructor.StopPredictions(actoCode);
				XDocument doc = XDocument.Load(UrlConstructor.StopPredictions(actoCode));
				XNamespace ns = doc.Root.GetDefaultNamespace();
				var arrivals = doc.Descendants(ns + "MonitoredStopVisit").Select(x => new LiveRecord()
				{
					ServiceNumber = (string)x.Descendants(ns + "LineRef").FirstOrDefault(),
					DestinationName = (string)x.Descendants(ns + "DestinationName").FirstOrDefault(),
					ScheduledDeparture = (DateTime?)x.Descendants(ns + "AimedDepartureTime").FirstOrDefault(),
					ExpectedDeparture = (DateTime?)x.Descendants(ns + "ExpectedDepartureTime").FirstOrDefault(),
					ScheduledArrival = (DateTime?)x.Descendants(ns + "AimedArrivalTime").FirstOrDefault(),
					ExpectedArrival = (DateTime?)x.Descendants(ns + "ExpectedArrivalTime").FirstOrDefault(),
					OperatorCode = ReadingBuses.GetOperatorE((string)x.Descendants(ns + "OperatorRef").FirstOrDefault()),
					VehicleRef = (string)x.Descendants(ns + "VehicleRef").FirstOrDefault(),
					_destination = (string)x.Descendants(ns + "DestinationRef").FirstOrDefault(),
					_origin = (string)x.Descendants(ns + "DestinationRef").FirstOrDefault()
				}).ToList();
				return arrivals.ToArray();
			}
			catch (NullReferenceException)
			{
				throw new ReadingBusesApiExceptionMalformedQuery("No data received.");
			}
			catch (WebException ex)
			{
				throw new ReadingBusesApiExceptionBadQuery(ex.Message);
			}
			catch (Exception ex)
			{
				ReadingBuses.PrintFullErrorLogs(ex.Message);
				throw new ReadingBusesApiExceptionCritical();
			}
		}
	}
}

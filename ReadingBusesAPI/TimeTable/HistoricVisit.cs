// Copyright (c) Jonathan Foot. All Rights Reserved. 
// Licensed under the GNU Affero General Public License, Version 3.0 
// See the LICENSE file in the project root for more information.


using System;
using ReadingBusesAPI.Common;
using System.Text.Json.Serialization;

namespace ReadingBusesAPI.TimeTable
{
	public class HistoricVisit : Visit
	{
		/// <summary>
		///     The default constructor, used only for JSON Parsing.
		///     Will be made internal when System.Text.Json add support for internal constructors in a future update.
		/// </summary>
		[JsonConstructor]
		[Obsolete("Do not use, will be made internal when system.text.json supports parsing in future updates.")]
		public HistoricVisit()
		{
		}

		/// <value>
		/// Unsure what these values represent, if you know, please let me know.
		/// The Siri Schema states the following classifiers should be present: "onTime" | "early" | "delayed" | "cancelled" | "arrived" | "departed" | "missed" | "noReport" | "notExpected"
		/// </value>
		[JsonPropertyName("ArrivalStatus")]
		[JsonInclude]
		public string ArrivalStatus { get; internal set; }

		/// <value>
		/// Unsure what these values represent, if you know, please let me know.
		/// The Siri Schema states the following classifiers should be present: "onTime" | "early" | "delayed" | "cancelled" | "arrived" | "departed" | "missed" | "noReport" | "notExpected"
		/// </value>
		[JsonPropertyName("DepartureStatus")]
		[JsonInclude]
		public string DepartureStatus { get; internal set; }


		/// <value>The Actual Arrival time of the bus at the stop.</value>
		[JsonPropertyName("ArrivalTime")]
		[JsonConverter(typeof(DateTimeOffsetConverter))]
		[JsonInclude]
		public DateTime? ActualArrival { get; internal set; }

		/// <value>The Actual Departure time of the bus at the stop.</value>
		[JsonPropertyName("DepartureTime")]
		[JsonConverter(typeof(DateTimeOffsetConverter))]
		[JsonInclude]
		public DateTime? ActualDeparture { get; internal set; }


		/// <summary>
		///     How late the bus was to arrive at a bus stop.
		/// </summary>
		/// <returns>
		///     The number of seconds the bus was late to arrive by.
		///     If no arrival time can be found, 0 is returned.
		/// </returns>
		public double ArrivalLateness()
		{
			if (ActualArrival != null)
			{
				return ((DateTime)ActualArrival - ScheduledArrival).TotalSeconds;
			}

			return 0;
		}

		/// <summary>
		///     How late the bus was to departure at a bus stop.
		/// </summary>
		/// <returns>
		///     The number of seconds the bus was late to departure by.
		///     If no departure time can be found, 0 is returned.
		/// </returns>
		public double DepartureLateness()
		{
			if (ActualDeparture != null)
			{
				return ((DateTime)ActualDeparture - ScheduledDeparture).TotalSeconds;
			}

			return 0;
		}


	}
}

// Copyright (c) Jonathan Foot. All Rights Reserved. 
// Licensed under the GNU Affero General Public License, Version 3.0 
// See the LICENSE file in the project root for more information.


using System;
using System.Text.Json.Serialization;
using ReadingBusesAPI.Common;

namespace ReadingBusesAPI.TimeTable
{
	public class Visit
	{
		/// <summary>
		///     The default constructor, used only for JSON Parsing.
		///     Will be made internal when System.Text.Json add support for internal constructors in a future update.
		/// </summary>
		[JsonConstructor]
		[Obsolete("Do not use, will be made internal when system.text.json supports parsing in future updates.")]
		public Visit()
		{
		}

		/// <value>The unique identifier for the bus stop.</value>
		[JsonPropertyName("Location")]
		[JsonInclude]
		public string AtcoCode { get; internal set; }

		/// <value>The sequence index value for which it is visited in.</value>
		[JsonPropertyName("Sequence")]
		[JsonInclude]
		public int Sequence { get; internal set; }

		/// <value>The name of the location.</value>
		[JsonPropertyName("LocationName")]
		[JsonInclude]
		public string LocationName { get; internal set; }

		/// <value>The Scheduled Arrivial time of the bus at the stop.</value>
		[JsonPropertyName("ScheduledArrivalTime")]
		[JsonConverter(typeof(DateTimeOffsetConverter))]
		[JsonInclude]
		public DateTime ScheduledArrival { get; internal set; }

		/// <value>The Scheduled Departure time of the bus at the stop.</value>
		[JsonPropertyName("ScheduledDepartureTime")]
		[JsonConverter(typeof(DateTimeOffsetConverter))]
		[JsonInclude]
		public DateTime ScheduledDeparture { get; internal set; }

		/// <value>Is this visit at a timming point stop or not.</value>
		[JsonPropertyName("TimingPoint")]
		[JsonConverter(typeof(ParseBoolConverter))]
		[JsonInclude]
		public bool TimingPoint { get; internal set; }

	}
}

// Copyright (c) Jonathan Foot. All Rights Reserved. 
// Licensed under the GNU Affero General Public License, Version 3.0 
// See the LICENSE file in the project root for more information.


using System;
using System.Text.Json.Serialization;
using ReadingBusesAPI.BusServices;
using ReadingBusesAPI.Common;

namespace ReadingBusesAPI.TimeTable
{
	public class Journey
	{
		/// <summary>
		///     The default constructor, used only for JSON Parsing.
		///     Will be made internal when System.Text.Json add support for internal constructors in a future update.
		/// </summary>
		[JsonConstructor]
		[Obsolete("Do not use, will be made internal when system.text.json supports parsing in future updates.")]
		public Journey()
		{
		}


		/// <value>The unique identifier for a journey.</value>
		[JsonPropertyName("Id")]
		[JsonInclude]
		public int Id { get; internal set; }

		/// <value>The bus operator of the service.</value>
		[JsonPropertyName("Operator")]
		[JsonConverter(typeof(ParseOperatorTimetableConverter))]
		[JsonInclude]
		public Company Company { get; internal set; }

		/// <value>The name/id for the bus service.</value>
		[JsonPropertyName("LineRef")]
		[JsonInclude]
		public string ServiceId { get; internal set; }

		/// <value>The JourneyPattern value. This seems unused within the API.</value>
		[JsonPropertyName("JourneyPattern")]
		[JsonInclude]
		public string JourneyPattern { get; internal set; }

		/// <value>The Running Board ID. A running board refers to the trips that the vehicle will execute that day</value>
		[JsonPropertyName("RunningBoard")]
		[JsonInclude]
		public string RunningBoard { get; internal set; }

		/// <value>The Duty ID, which is a timetable of trips a driver is expected to execute that day.</value>
		[JsonPropertyName("Duty")]
		[JsonInclude]
		public string Duty { get; internal set; }

		/// <value>The Journey Code. Seems to server same purpose as the Id value? </value>
		[JsonPropertyName("JourneyCode")]
		[JsonInclude]
		public string JourneyCode { get; internal set; }

		/// <value>The Scheduled Start time of the journey. This is normally the time of the first stop visit.</value>
		[JsonPropertyName("ScheduledStart")]
		[JsonConverter(typeof(DateTimeOffsetConverter))]
		[JsonInclude]
		public DateTime ScheduledStart { get; internal set; }


		/// <value>An array of timetabled visits contained within this journey.</value>
		[JsonPropertyName("visits")]
		[JsonInclude]
		public Visit[] Visits { get; internal set; }


		/// <summary>
		/// Gets a reference to the service that is assoicated with this journey.
		/// </summary>
		/// <returns>Returns a Bus Service object for the assoicated service. Null if unknown within the API.</returns>
		public BusService GetService()
		{
			if(ReadingBuses.GetInstance().IsService(ServiceId,Company))
				return ReadingBuses.GetInstance().GetService(ServiceId,Company);

			return null;
		}
	}
}

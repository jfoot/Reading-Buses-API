// Copyright (c) Jonathan Foot. All Rights Reserved. 
// Licensed under the GNU Affero General Public License, Version 3.0 
// See the LICENSE file in the project root for more information.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using ReadingBusesAPI.BusServices;
using ReadingBusesAPI.BusStops;
using ReadingBusesAPI.Common;

namespace ReadingBusesAPI.TimeTable
{
	/// <summary>
	///     Represents the Raw timetable object data you get from the Timetabled Journeys and Tracking History APIs.
	/// </summary>
	public abstract class TimeTableRecord
	{
		/// <summary>
		///     The default constructor, used only for JSON Parsing.
		///     Will be made internal when System.Text.Json add support for internal constructors in a future update.
		/// </summary>
		[JsonConstructor]
		[Obsolete("Do not use, will be made internal when system.text.json supports parsing in future updates.")]
		public TimeTableRecord()
		{
		}

		/// <value>The service number of the bus.</value>
		[JsonPropertyName("LineRef")]
		internal string ServiceNumber { get; set; }


		/// <value>The operator of the bus services</value>
		[JsonPropertyName("Operator")]
		[JsonConverter(typeof(ParseOperatorConverter))]
		public Company Operator { get; internal set; }

		/// <value>The 'BusStop' object for the stop relating to the time table record..</value>
		[JsonPropertyName("LocationCode")]
		[JsonConverter(typeof(ParseBusStopConverter))]
		public BusStop Location { get; internal set; }

		/// <value>What number bus stop is this in the buses route, ie 1, is the first stop to visit.</value>
		[JsonPropertyName("Sequence")]
		[JsonConverter(typeof(ParseStringConverter))]
		public long Sequence { get; internal set; }

		/// <value>Is this bus heading inbound or outbound.</value>
		[JsonPropertyName("Direction")]
		[JsonConverter(typeof(ParseDirectionConverter))]
		public Direction Direction { get; internal set; }

		/// <value>
		///     A unique value that groups a selection of time table records across different bus stops to show one loop/ cycle
		///     of a bus services route.
		/// </value>
		[JsonPropertyName("JourneyCode")]
		public string JourneyCode { get; internal set; }


		/// <value>Is this bus stop a timing point or not.</value>
		/// <remarks>
		///     A timing point is a major bus stop, where the buses is expected to wait if its early and should actually arrive on
		///     the scheduled time.
		///     All non-timing points times are only estimated scheduled times. A timing point is much more accurate and strict
		///     timings.
		/// </remarks>
		[JsonPropertyName("TimingPoint")]
		[JsonConverter(typeof(ParseTimingPointConverter))]
		public bool IsTimingPoint { get; internal set; }

		/// <value>The scheduled arrival time for the bus. </value>
		[JsonPropertyName("ScheduledArrivalTime")]
		public DateTime SchArrivalTime { get; internal set; }

		/// <value>The scheduled departure time for the bus. </value>
		[JsonPropertyName("ScheduledDepartureTime")]
		public DateTime SchDepartureTime { get; internal set; }


		/// <summary>
		///     Gets the related 'BusService' object relating to the time table record.
		/// </summary>
		/// <returns>A 'BusService' object for this time table record.</returns>
		public BusService GetService()
		{
			return ReadingBuses.GetInstance().GetService(ServiceNumber, Operator);
		}


		#region JSONConverters


		/// <summary>
		///     Converts a bus stop acto-code into a 'BusStop' Object and back again for the JSON converter.
		/// </summary>
		private class ParseBusStopConverter : JsonConverter<BusStop>
		{
			public override BusStop Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				return ReadingBuses.GetInstance().GetLocation(reader.GetString());
			}

			public override void Write(Utf8JsonWriter writer, BusStop value, JsonSerializerOptions options)
			{
				writer.WriteStringValue(value.ActoCode);
			}
		}



		/// <summary>
		///     Converts a string into a Direction Enum and back again for the JSON converter.
		/// </summary>
		private class ParseDirectionConverter : JsonConverter<Direction>
		{
			public override Direction Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				var value = reader.GetString();

				if (value.Equals("Inbound", StringComparison.OrdinalIgnoreCase))
				{
					return Direction.Inbound;
				}

				if (value.Equals("Outbound", StringComparison.OrdinalIgnoreCase))
				{
					return Direction.Outbound;
				}

				throw new JsonException("Cannot unmarshal type Direction");
			}

			public override void Write(Utf8JsonWriter writer, Direction value, JsonSerializerOptions options)
			{
				switch (value)
				{
					case Direction.Inbound:
						writer.WriteStringValue("Inbound");
						return;
					case Direction.Outbound:
						writer.WriteStringValue("Outbound");
						return;
				}

				throw new JsonException("Cannot marshal type Direction");
			}
		}


		/// <summary>
		///     Converts a string into a boolean and back again for the JSON converter.
		/// </summary>
		private class ParseTimingPointConverter : JsonConverter<bool>
		{
			public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				var value = reader.GetString();

				if (value.Equals("NonTimingPoint", StringComparison.OrdinalIgnoreCase))
				{
					return false;
				}

				if (value.Equals("TimingPoint", StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}

				throw new JsonException("Cannot unmarshal type TimingPoint");
			}

			public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
			{
				switch (value)
				{
					case true:
						writer.WriteStringValue("TimingPoint");
						break;
					case false:
						writer.WriteStringValue("NonTimingPoint");
						break;
				}
			}
		}


		#endregion
	}
}

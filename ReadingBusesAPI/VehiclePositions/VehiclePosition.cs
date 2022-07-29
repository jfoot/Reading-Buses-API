// Copyright (c) Jonathan Foot. All Rights Reserved. 
// Licensed under the GNU Affero General Public License, Version 3.0 
// See the LICENSE file in the project root for more information.

using System;
using System.Text.Json.Serialization;
using ReadingBusesAPI.Common;

namespace ReadingBusesAPI.VehiclePositions
{
	/// <summary>
	///     Stores information about GPS data on a vehicle.
	/// </summary>
	public class VehiclePosition
	{
		/// <summary>
		///     The default constructor, used only for JSON Parsing.
		///     Will be made internal when System.Text.Json add support for internal constructors in a future update.
		/// </summary>
		[JsonConstructor]
		[Obsolete("Do not use, will be made internal when system.text.json supports parsing in future updates.")]
		public VehiclePosition()
		{
		}


		/// <value>Holds the operators enum value.</value>
		[JsonPropertyName("operator")]
		[JsonInclude]
		[JsonConverter(typeof(ParseOperatorConverter))]
		public Company Company { get; internal set; }

		/// <value>Holds the reference/identifier for the vehicle</value>
		[JsonPropertyName("vehicle")]
		[JsonInclude]
		public string Vehicle { get; internal set; }

		/// <value>Holds the time it was last seen/ new data was retrieved.</value>
		[JsonPropertyName("observed")]
		[JsonInclude]
		[JsonConverter(typeof(DateTimeOffsetConverter))]
		public DateTime Observed { get; internal set; }

		/// <value>Latitude position of the bus</value>
		[JsonPropertyName("latitude")]
		[JsonInclude]
		public string Latitude { get; internal set; }

		/// <value>longitude position of the bus</value>
		[JsonPropertyName("longitude")]
		[JsonInclude]
		public string Longitude { get; internal set; }


		/// <summary>
		///     Gets the geographical position of the bus.
		/// </summary>
		/// <returns>A Point Object for the position of the bus.</returns>
		public Point GetPoint() => new Point(double.Parse(Longitude), double.Parse(Latitude));

	}
}

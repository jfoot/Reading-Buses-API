// Copyright (c) Jonathan Foot. All Rights Reserved. 
// Licensed under the GNU Affero General Public License, Version 3.0 
// See the LICENSE file in the project root for more information.

using System.Text.Json.Serialization;
using ReadingBusesAPI.Common;

namespace ReadingBusesAPI.BusStops
{
	/// <summary>
	/// An intermediary class used for the "Line Pattern" API, to get the route of a service.
	/// </summary>
	internal class StopPattern
	{
		/// <value>The unique identifier for a bus stop.</value>
		[JsonPropertyName("location_code")]
		[JsonInclude]
		public string ActoCode { get; internal set; }

		/// <value>The public, easy to understand stop name.</value>
		[JsonPropertyName("location_name")]
		[JsonInclude]
		public string CommonName { get; internal set; }

		/// <value>The operator associated to the record, this is needed so you can filter it out.</value>
		[JsonConverter(typeof(ParseOperatorConverter))]
		[JsonPropertyName("operator_code")]
		[JsonInclude]
		public Company OperatorCode { get; internal set; }


		/// <value>The order in which a stop is visited.</value>
		[JsonPropertyName("display_order")]
		[JsonInclude]
		public int Order { get; internal set; }

		/// <value>The direction of travel.</value>
		/// todo make this private once system.text.json supports it.
		[JsonPropertyName("direction")]
		[JsonInclude]
		public int _direction { get; internal set; }

		/// <summary>
		/// Is an outbound stop pattern.
		/// </summary>
		/// <returns>True if outbound, false if inbound.</returns>
		public bool IsOutbound()
		{
			return _direction == 0;
		}
	}
}

// Copyright (c) Jonathan Foot. All Rights Reserved. 
// Licensed under the GNU Affero General Public License, Version 3.0 
// See the LICENSE file in the project root for more information.

using System;
using System.Text.Json.Serialization;
using ReadingBusesAPI.BusServices;
using ReadingBusesAPI.Common;

namespace ReadingBusesAPI.BusStops
{
	/// <summary>
	///     Stores information about a single bus stop and single service. Related to the raw output of the "List Of Bus Stops" API.
	///     This API returns the same stop multiple times with each service that visites it treated as a new stop?
	/// </summary>
	internal sealed class BusStopIntermediary
	{

		/// <summary>
		///     The default constructor, used only for JSON Parsing.
		///     Will be made internal when System.Text.Json add support for internal constructors in a future update.
		/// </summary>
		[JsonConstructor]
		[Obsolete("Do not use, will be made internal when system.text.json supports parsing in future updates.")]
		public BusStopIntermediary()
		{
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

		/// <value>The Brand/Group of buses that most frequently visit this stop. Such as Purple, for the Purple 17s.</value>
		[JsonPropertyName("operator_code")]
		[JsonConverter(typeof(ParseOperatorConverter))]
		[JsonInclude]
		public Company OperatorCode { get; internal set; }

		/// <value>The service ID for a service stopping at the stop.</value>
		[JsonPropertyName("route_code")]
		[JsonInclude]
		public string RouteCode { get; internal set; }

		/// <summary>
		/// Gets the services mentioned at this stop.
		/// </summary>
		/// <returns>Bus Service object connected to the stop. Returns null if invalid API data.</returns>
		public BusService GetService()
		{
			if (!ReadingBuses.GetInstance().IsService(RouteCode, OperatorCode))
				return null;

			return ReadingBuses.GetInstance().GetService(RouteCode, OperatorCode);
		}

	}
}

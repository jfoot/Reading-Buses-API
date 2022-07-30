// Copyright (c) Jonathan Foot. All Rights Reserved. 
// Licensed under the GNU Affero General Public License, Version 3.0 
// See the LICENSE file in the project root for more information.


using System;
using System.Text.Json.Serialization;

namespace ReadingBusesAPI.TimeTable
{
	/// <summary>
	/// A historical journey, one that has happened in the past.
	/// </summary>
	public class HistoricJourney : Journey
	{
		/// <summary>
		///     The default constructor, used only for JSON Parsing.
		///     Will be made internal when System.Text.Json add support for internal constructors in a future update.
		/// </summary>
		[JsonConstructor]
		[Obsolete("Do not use, will be made internal when system.text.json supports parsing in future updates.")]
		public HistoricJourney()
		{
		}


		/// <value>The vehicle code/id of the bus that operated on this journey.</value>
		[JsonPropertyName("VehicleCode")]
		[JsonInclude]
		public string VehicleCode { get; internal set; }


		/// <value>An array of timetabled visits contained within this journey.</value>
		[JsonPropertyName("visits")]
		[JsonInclude]
		public new HistoricVisit[] Visits { get; internal set; }
	}
}

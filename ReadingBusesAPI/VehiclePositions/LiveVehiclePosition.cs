// Copyright (c) Joanthan Foot. All Rights Reserved. 
// Licensed under the GNU Affero General Public License, Version 3.0
// See the LICENSE file in the project root for more information.

using System;
using System.Text.Json.Serialization;
using ReadingBusesAPI.BusServices;

namespace ReadingBusesAPI.VehiclePositions
{
	public class LiveVehiclePosition : VehiclePosition
	{
		/// <summary>
		///     The default constructor, used only for JSON Parsing.
		///     Will be made internal when System.Text.Json add support for internal constructors in a future update.
		/// </summary>
		[JsonConstructor]
		[Obsolete("Do not use, will be made internal when system.text.json supports parsing in future updates.")]
		public LiveVehiclePosition()
		{
		}

		/// <value>Holds the Service Number for the bus route.</value>
		[JsonPropertyName("service")]
		[JsonInclude]
		public string ServiceId { get; internal set; }


		/// <summary>
		/// Gets a reference to the service that is assoicated with this vehicle.
		/// </summary>
		/// <returns>Returns a Bus Service object for the assoicated service. Null if unknown within the API.</returns>
		public BusService GetService()
		{
			if (ReadingBuses.GetInstance().IsService(ServiceId, Company))
				return ReadingBuses.GetInstance().GetService(ServiceId, Company);

			return null;
		}
	}
}

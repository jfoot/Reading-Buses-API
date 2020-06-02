// Copyright (c) Jonathan Foot. All Rights Reserved. 
// Licensed under the GNU Affero General Public License, Version 3.0 
// See the LICENSE file in the project root for more information.

using System;
using Newtonsoft.Json;

namespace ReadingBusesAPI
{
    /// <summary>
    ///     Used to store information about a buses GPS position. Related to the "Live Vehicle Positions" API.
    /// </summary>
    public sealed class LivePosition
    {
        /// <summary>
        ///     The default constructor, which sets the 'LastRetrieval' to current time.
        /// </summary>
        internal LivePosition()
        {
        }

        /// <value>Holds the Operator short code string value.</value>
        [JsonProperty("operator")]
        internal string OperatorCodeS { get; set; }

        /// <value>Holds the operators enum value.</value>
        public Operators OperatorCode => ReadingBuses.GetOperatorE(OperatorCodeS);

        /// <value>Holds the reference/identifier for the vehicle</value>
        [JsonProperty("vehicle")]
        public string Vehicle { get; internal set; }

        /// <value>Holds the Service Number for the bus route.</value>
        [JsonProperty("service")]
        public string ServiceId { get; internal set; }

        /// <value>Holds the time it was last seen/ new data was retrieved.</value>
        [JsonProperty("observed")]
        public DateTimeOffset Observed { get; internal set; }

        /// <value>Latitude position of the bus</value>
        [JsonProperty("latitude")]
        public string Latitude { get; internal set; }

        /// <value>longitude position of the bus</value>
        [JsonProperty("longitude")]
        public string Longitude { get; internal set; }

        /// <value>bearing direction of the bus</value>
        [JsonProperty("bearing")]
        public string Bearing { get; internal set; }


        /// <summary>
        ///     Returns the coordinate for a vehicle.
        /// </summary>
        /// <returns> Returns the coordinate for a vehicle. </returns>
        public string Point() => Latitude + ", " + Longitude;

        /// <summary>
        ///     Finds the Brand name of the service, eg "pink".
        /// </summary>
        /// <returns>The brand name for the service.</returns>
        public string BrandName() => ReadingBuses.GetInstance().GetService(ServiceId, OperatorCode).BrandName;

        /// <summary>
        ///     Prints off the coordinate of the vehicle.
        /// </summary>
        public void PrintPoint() => Console.WriteLine(Point());


        /// <summary>
        ///     Gets the geographical position of the bus.
        /// </summary>
        /// <returns>A Point Object for the position of the bus.</returns>
        public Point GetPoint() => new Point(double.Parse(Longitude), double.Parse(Latitude));
    }
}
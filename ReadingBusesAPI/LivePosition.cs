using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReadingBusesAPI
{
    public sealed class LivePosition
    {
        [JsonProperty("operator")]
        public string Operator { get; internal set; }

        [JsonProperty("vehicle")]
        public string Vehicle { get; internal set; }

        [JsonProperty("service")]
        public string ServiceId { get; internal set; }

        [JsonProperty("observed")]
        public DateTimeOffset Observed { get; internal set; }

        [JsonProperty("latitude")]
        public string Latitude { get; internal set; }

        [JsonProperty("longitude")]
        public string Longitude { get; internal set; }

        [JsonProperty("bearing")]
        public string Bearing { get; internal set; }

        internal static DateTime lastReterival;

        internal LivePosition() => lastReterival = DateTime.Now;

        /// <summary>
        /// GPS data only updates every 30 seconds, so on average you will need to wait 15s for new data.
        /// This is used to check how long it was since last requesting GPS data. If it was recently
        /// there  is no point making another request to the API as you will get the same data and take longer.
        /// </summary>
        /// <returns>Returns if it has been less than 15 seconds from last asking for GPS data.</returns>
        internal static bool RefreshCahce() => (DateTime.Now - lastReterival).TotalSeconds > 15;

        /// <summary>
        /// Returns the coordinate for a vehicle. 
        /// </summary>
        /// <returns> Returns the coordinate for a vehicle. </returns>
        public string Point() => Latitude + ", " + Longitude;

        /// <summary>
        /// Finds the Brand name of the service, eg "pink".
        /// </summary>
        /// <returns>The brand name for the service.</returns>
        public string BrandName() => ReadingBuses.GetInstance().GetService(ServiceId).BrandName;

        /// <summary>
        /// Prints off the coordinate of the vehicle.
        /// </summary>
        public void PrintPoint() => Console.WriteLine(Point());
    }
}

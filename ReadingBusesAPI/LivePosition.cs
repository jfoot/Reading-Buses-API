using Newtonsoft.Json;
using System;

namespace ReadingBusesAPI
{
    /// <summary>
    /// Used to store information about a buses GPS position.
    /// </summary>
    public sealed class LivePosition
    {
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

        /// <value>The last time a GPS request was made. This is used to prevent unnecessary API calls.</value>
        internal static DateTime LastRetrieval;

        /// <summary>
        /// The default constructor, which sets the 'LastRetrieval' to current time.
        /// </summary>
        internal LivePosition() => LastRetrieval = DateTime.Now;

        /// <summary>
        /// GPS data only updates every 30 seconds, so on average you will need to wait 15s for new data.
        /// This is used to check how long it was since last requesting GPS data. If it was recently
        /// there  is no point making another request to the API as you will get the same data and take longer.
        /// </summary>
        /// <returns>Returns if it has been less than 15 seconds from last asking for GPS data.</returns>
        internal static bool RefreshCache() => (DateTime.Now - LastRetrieval).TotalSeconds > 15;

        /// <summary>
        /// Returns the coordinate for a vehicle. 
        /// </summary>
        /// <returns> Returns the coordinate for a vehicle. </returns>
        public string Point() => Latitude + ", " + Longitude;

        /// <summary>
        /// Finds the Brand name of the service, eg "pink".
        /// </summary>
        /// <returns>The brand name for the service.</returns>
        public string BrandName() => ReadingBuses.GetInstance().GetService(ServiceId, OperatorCode).BrandName;

        /// <summary>
        /// Prints off the coordinate of the vehicle.
        /// </summary>
        public void PrintPoint() => Console.WriteLine(Point());


        /// <summary>
        /// Gets the geographical position of the bus.
        /// </summary>
        /// <returns>A Point Object for the position of the bus.</returns>
        public Point GetPoint() => new Point(Double.Parse(Longitude), Double.Parse(Latitude));
    }
}

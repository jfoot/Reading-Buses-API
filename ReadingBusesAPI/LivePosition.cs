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

        internal LivePosition() {
            lastReterival = DateTime.Now;
        }

        internal static bool refreshCahce()
        {
            return (DateTime.Now- lastReterival).TotalSeconds > 15;
        }

        public string Point()
        {
            return Latitude + ", " + Longitude;
        }

        public string BrandName()
        {
            return ReadingBuses.getInstance().getService(ServiceId).BrandName;
        }

        public void printPoint()
        {
            Console.WriteLine(Point());
        }
    }
}

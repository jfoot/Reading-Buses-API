using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReadingBusesAPI
{
    public class BusStop
    {
        [JsonProperty("location_code")]
        public string ActoCode { get; set; }

        [JsonProperty("description")]
        public string CommonName { get; set; }

        [JsonProperty("latitude")]
        public string Latitude { get; set; }

        [JsonProperty("longitude")]
        public string longitude { get; set; }

        [JsonProperty("bearing")]
        public string bearing { get; set; }

        [JsonProperty("routes")]
        public string Services { get; set; }

        [JsonProperty("group_name")]
        public string group_name { get; set; }

        internal BusStop() { }
    }
}

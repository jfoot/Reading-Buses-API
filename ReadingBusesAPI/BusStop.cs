using Newtonsoft.Json;
using System.Collections.Generic;


namespace ReadingBusesAPI
{
    public sealed class BusStop
    {
        [JsonProperty("location_code")]
        public string ActoCode { get; internal set; }

        [JsonProperty("description")]
        public string CommonName { get; internal set; }

        [JsonProperty("latitude")]
        public string Latitude { get; internal set; }

        [JsonProperty("longitude")]
        public string Longitude { get; internal set; }

        [JsonProperty("bearing")]
        public string Bearing { get; internal set; }

        [JsonProperty("routes")]
        public string Services { get; internal set; }

        [JsonProperty("group_name")]
        public string GroupName { get; internal set; }

        internal BusStop() { }

        /// <summary>
        /// Gets live data from a bus stop.
        /// </summary>
        /// <returns>Returns a list of Live Records, which are individual buses due to arrive at the bus stop.</returns>
        public List<LiveRecord> GetLiveData() => LiveRecord.GetLiveData(ActoCode);
    }
}

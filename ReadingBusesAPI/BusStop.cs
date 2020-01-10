using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
        public string longitude { get; internal set; }

        [JsonProperty("bearing")]
        public string bearing { get; internal set; }

        [JsonProperty("routes")]
        public string Services { get; internal set; }

        [JsonProperty("group_name")]
        public string group_name { get; internal set; }

        internal BusStop() { }

        public async Task<List<LiveRecord>> getLiveData()
        {
            return await LiveRecord.GetLiveData(ActoCode);
        }
    }
}

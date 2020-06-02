// Copyright (c) Jonathan Foot. All Rights Reserved. 
// Licensed under the GNU Affero General Public License, Version 3.0 
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ReadingBusesAPI
{
    /// <summary>
    ///     Contains information about a single time table record, which means information on one bus at one location. Related
    ///     to the "Timetabled Journeys" API.
    /// </summary>
    public sealed class BusTimeTable
    {
        /// <value>The service number of the bus.</value>
        [JsonProperty("LineRef")]
        internal string ServiceNumber { get; set; }


        /// <value>The operator of the bus services</value>
        [JsonProperty("Operator")]
        [JsonConverter(typeof(ParseOperatorConverter))]
        internal Operators Operator { get; set; }

        /// <value>The 'BusStop' object for the stop relating to the time table record..</value>
        [JsonProperty("LocationCode")]
        [JsonConverter(typeof(ParseBusStopConverter))]
        public BusStop Location { get; internal set; }

        /// <value>What number bus stop is this in the buses route, ie 1, is the first stop to visit.</value>
        [JsonProperty("Sequence")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Sequence { get; internal set; }

        /// <value>Is this bus heading inbound or outbound.</value>
        [JsonProperty("Direction")]
        [JsonConverter(typeof(ParseDirectionConverter))]
        public Direction Direction { get; internal set; }

        /// <value>
        ///     A unique value that groups a selection of time table records across different bus stops to show one loop/ cycle
        ///     of a bus services route.
        /// </value>
        [JsonProperty("JourneyCode")]
        public string JourneyCode { get; set; }


        /// <value>Is this bus stop a timing point or not.</value>
        /// <remarks>
        ///     A timing point is a major bus stop, where the buses is expected to wait if its early and should actually arrive on
        ///     the scheduled time.
        ///     All non-timing points times are only estimated scheduled times. A timing point is much more accurate and strict
        ///     timings.
        /// </remarks>
        [JsonProperty("TimingPoint")]
        [JsonConverter(typeof(ParseTimingPointConverter))]
        public bool IsTimingPoint { get; set; }

        /// <value>The scheduled arrival time for the bus. </value>
        [JsonProperty("ScheduledArrivalTime")]
        public DateTime ScheduledArrivalTime { get; set; }

        /// <value>The scheduled departure time for the bus. </value>
        [JsonProperty("ScheduledDepartureTime")]
        public DateTime ScheduledDepartureTime { get; set; }


        /// <summary>
        ///     Gets the related 'BusService' object relating to the time table record.
        /// </summary>
        /// <returns>A 'BusSerivce' object for this time table record.</returns>
        public BusService GetService()
        {
            return ReadingBuses.GetInstance().GetService(ServiceNumber, Operator);
        }


        /// <summary>
        ///     Gets the time table of a service or a location as one array of 'BusTimeTable' objects.
        /// </summary>
        /// <param name="service">The bus services you wish to view.</param>
        /// <param name="date">The date of the time table.</param>
        /// <param name="location">The location to get timetable data from.</param>
        /// <returns>An array of time table records for the service or location or both</returns>
        /// <exception cref="InvalidOperationException">
        ///     If you have not provided any date, and/or you have not provided at least
        ///     either the service or location.
        /// </exception>
        internal static async Task<BusTimeTable[]> GetAggregateTimeTable(BusService service, DateTime date,
            BusStop location)
        {
            if (date == null || service == null && location == null)
                throw new InvalidOperationException(
                    "You must provide a date and a service and/or location for a valid query.");


            var timeTable = JsonConvert.DeserializeObject<List<BusTimeTable>>(
                await new WebClient().DownloadStringTaskAsync("https://rtl2.ods-live.co.uk/api/scheduledJourneys?key=" +
                                                              ReadingBuses.APIKey +
                                                              "&service=" + service.ServiceId + "&date=" +
                                                              date.ToString("yyyy-MM-dd") + "&location=" +
                                                              location.ActoCode));

            return timeTable.ToArray();
        }

        /// <summary>
        ///     Gets the time table for a service and groups it by a journey code instead of one continuous array of time table
        ///     entries.
        /// </summary>
        /// <param name="service">The bus services you wish to view.</param>
        /// <param name="date">The date of the time table.</param>
        /// <param name="location">The location to get timetable data from.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">
        ///     If you have not provided any date, and/or you have not provided at least
        ///     either the service or location.
        /// </exception>
        internal static async Task<IGrouping<string, BusTimeTable>[]> GetTimeTable(BusService service, DateTime date,
            BusStop location)
        {
            return (IGrouping<string, BusTimeTable>[]) (await GetAggregateTimeTable(service, date, location))
                .GroupBy(x => x.JourneyCode).ToArray();
        }


        #region JSONConverters

        /// <summary>
        ///     Converts a bus stop acto-code into a 'BusStop' Object and back again for the JSON converter.
        /// </summary>
        internal class ParseBusStopConverter : JsonConverter
        {
            public static readonly ParseBusStopConverter Singleton = new ParseBusStopConverter();
            public override bool CanConvert(Type t) => t == typeof(BusStop);

            public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) return null;
                var value = serializer.Deserialize<string>(reader);

                return ReadingBuses.GetInstance().GetLocation(value);
            }

            public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
            {
                if (untypedValue == null)
                {
                    serializer.Serialize(writer, null);
                    return;
                }

                var value = (BusStop) untypedValue;

                serializer.Serialize(writer, value.ActoCode);
                return;
            }
        }


        /// <summary>
        ///     Converts a string into a long and back again for the JSON converter.
        /// </summary>
        internal class ParseStringConverter : JsonConverter
        {
            public static readonly ParseStringConverter Singleton = new ParseStringConverter();
            public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

            public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) return null;
                var value = serializer.Deserialize<string>(reader);
                if (long.TryParse(value, out var l)) return l;
                throw new Exception("Cannot unmarshal type long");
            }

            public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
            {
                if (untypedValue == null)
                {
                    serializer.Serialize(writer, null);
                    return;
                }

                var value = (long) untypedValue;
                serializer.Serialize(writer, value.ToString());
                return;
            }
        }

        /// <summary>
        ///     Converts a string into a Direction Enum and back again for the JSON converter.
        /// </summary>
        internal class ParseDirectionConverter : JsonConverter
        {
            public static readonly ParseDirectionConverter Singleton = new ParseDirectionConverter();
            public override bool CanConvert(Type t) => t == typeof(Direction) || t == typeof(Direction?);

            public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) return null;
                var value = serializer.Deserialize<string>(reader);
                return value switch
                {
                    "Inbound" => Direction.Inbound,
                    "Outbound" => Direction.Outbound,
                    _ => throw new Exception("Cannot unmarshal type Direction")
                };
            }

            public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
            {
                if (untypedValue == null)
                {
                    serializer.Serialize(writer, null);
                    return;
                }

                var value = (Direction) untypedValue;
                switch (value)
                {
                    case Direction.Inbound:
                        serializer.Serialize(writer, "Inbound");
                        return;
                    case Direction.Outbound:
                        serializer.Serialize(writer, "Outbound");
                        return;
                }

                throw new Exception("Cannot marshal type Direction");
            }
        }

        /// <summary>
        ///     Converts a string into a boolean and back again for the JSON converter.
        /// </summary>
        internal class ParseTimingPointConverter : JsonConverter
        {
            public static readonly ParseTimingPointConverter Singleton = new ParseTimingPointConverter();
            public override bool CanConvert(Type t) => t == typeof(bool) || t == typeof(bool?);

            public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) return null;
                var value = serializer.Deserialize<string>(reader);
                return value switch
                {
                    "NonTimingPoint" => false,
                    "TimingPoint" => true,
                    _ => throw new Exception("Cannot unmarshal type TimingPoint")
                };
            }

            public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
            {
                if (untypedValue == null)
                {
                    serializer.Serialize(writer, null);
                    return;
                }

                var value = (bool) untypedValue;
                switch (value)
                {
                    case false:
                        serializer.Serialize(writer, "NonTimingPoint");
                        return;
                    case true:
                        serializer.Serialize(writer, "TimingPoint");
                        return;
                }

                throw new Exception("Cannot marshal type TimingPoint");
            }
        }

        #endregion
    }
}
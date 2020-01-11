using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ReadingBusesAPI
{
    public sealed class LiveRecord
    {
        public string ServiceNumber { get; internal set; }
        public string Destination { get; internal set; }
        public DateTime SchArrival { get; internal set; }
        public DateTime? ExptArrival { get; internal set; }

        /// <summary>
        /// Returns the related BusService Object for the Bus LiveRecord.
        /// </summary>
        /// <returns></returns>
        public BusService Service()
        {
            return ReadingBuses.GetInstance().GetService(ServiceNumber);
        }

        /// <summary>
        /// Returns the number of min till bus is due in a min format.
        /// </summary>
        /// <returns>The number of min until the bus is due to arrive in string format.</returns>
        public string DisplayTime()
        {
            return ((ExptArrival != null ? (DateTime)ExptArrival : SchArrival) - DateTime.Now).TotalMinutes.ToString("0") + " mins";
        }

        /// <summary>
        /// Returns the number of min till the bus is due to arrive.
        /// </summary>
        /// <returns>The number of min till the bus is due to arrive.</returns>
        public double ArrivalMin()
        {
            return ((ExptArrival != null ? (DateTime)ExptArrival : SchArrival) - DateTime.Now).TotalMinutes;
        }

        internal LiveRecord() { }

        /// <summary>
        /// Gets a list of upcoming arrivals at a specific bus stop.
        /// </summary>
        /// <param name="actoCode">The Acto-code ID for a specific bus stop.</param>
        /// <returns>A list of Live Records containing details about upcoming buses.</returns>
        public static List<LiveRecord> GetLiveData(string actoCode)
        {
            XDocument doc = XDocument.Load("https://rtl2.ods-live.co.uk/api/siri/sm?key=" + ReadingBuses.APIKey + "&location=" + actoCode);
            XNamespace ns = doc.Root.GetDefaultNamespace();
            List<LiveRecord> Arrivals = new List<LiveRecord>();
            Arrivals = doc.Descendants(ns + "MonitoredStopVisit").Select(x => new LiveRecord()
            {
                ServiceNumber = (string)x.Descendants(ns + "LineRef").FirstOrDefault(),
                Destination = (string)x.Descendants(ns + "DestinationName").FirstOrDefault(),
                SchArrival = (DateTime)x.Descendants(ns + "AimedArrivalTime").FirstOrDefault(),
                ExptArrival = (DateTime?)x.Descendants(ns + "ExpectedArrivalTime").FirstOrDefault(),
            }).ToList();

            return Arrivals;
        }
    }
}

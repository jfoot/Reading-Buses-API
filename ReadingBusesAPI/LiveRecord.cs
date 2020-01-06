using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ReadingBusesAPI
{
    public sealed class LiveRecord
    {
        public string ServiceNumber { get; internal set; }
        public string Destination { get; internal set; }
        public DateTime SchArrival { get; internal set; }
        public DateTime? ExptArrival { get; internal set; }

        public BusService Service()
        {
            return ReadingBuses.getInstance().getService(ServiceNumber);
        }

        internal LiveRecord() { }

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

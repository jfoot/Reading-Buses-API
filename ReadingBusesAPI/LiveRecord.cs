using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ReadingBusesAPI
{
    public class LiveRecord
    {
        public string ServiceNumber { get; set; }
        public string Destination { get; set; }
        public DateTime SchArrival { get; set; }
        public DateTime? ExptArrival { get; set; }

        internal LiveRecord() { }

        public static List<LiveRecord> GetLiveData(string actoCode, string APIKEY)
        {
            XDocument doc = XDocument.Load("https://rtl2.ods-live.co.uk/api/siri/sm?key=" + APIKEY + "&location=" + actoCode);
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

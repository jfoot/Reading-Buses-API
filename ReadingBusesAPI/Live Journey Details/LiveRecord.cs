// Copyright (c) Jonathan Foot. All Rights Reserved. 
// Licensed under the GNU Affero General Public License, Version 3.0 
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ReadingBusesAPI
{
    /// <summary>
    ///     Used to store information about a buses arrival at a bus stop. Mainly related to the "Stop Predictions" API.
    /// </summary>
    public sealed class LiveRecord
    {
        /// <summary>
        ///     The default constructor, used for XML parsing.
        /// </summary>
        internal LiveRecord()
        {
        }

        /// <value>Holds the Service Number for the bus route.</value>
        public string ServiceNumber { get; internal set; }

        /// <value>Holds the destination for the bus.</value>
        public string Destination { get; internal set; }

        /// <value>Holds scheduled arrival time of the bus at the location.</value>
        public DateTime SchArrival { get; internal set; }

        /// <value>Holds the estimated/ expected arrival time of the bus, if Null no estimated time exists yet.</value>
        public DateTime? ExptArrival { get; internal set; }

        /// <value>Holds the operator of the service.</value>
        public Operators OperatorCode { get; internal set; }

        /// <value>Holds the Vehicles reference ID or number to identify it.</value>
        public string VehicleRef { get; internal set; }

        /// <value>
        ///     Holds the 'Via' message, which explains where the bus is traveling past on route. Can be null or a place holder
        ///     value if none exists.
        /// </value>
        public string ViaMessage { get; internal set; }


        /// <summary>
        ///     Returns the related BusService Object for the Bus LiveRecord.
        /// </summary>
        /// <returns>Information about the current bus service object.</returns>
        /// <exception cref="InvalidOperationException">
        ///     Can throw an exception if the service does not exists. This is however very
        ///     unlikely, if this occurs there is an error in the API, not with your code.
        /// </exception>
        public BusService Service()
        {
            return ReadingBuses.GetInstance().GetService(ServiceNumber, OperatorCode);
        }

        /// <summary>
        ///     Returns the number of min till bus is due in a min format.
        /// </summary>
        /// <returns>The number of min until the bus is due to arrive in string format.</returns>
        public string DisplayTime()
        {
            return ((ExptArrival ?? SchArrival) - DateTime.Now).TotalMinutes.ToString("0") + " mins";
        }

        /// <summary>
        ///     Returns the number of min till the bus is due to arrive.
        /// </summary>
        /// <returns>The number of min till the bus is due to arrive.</returns>
        public double ArrivalMin()
        {
            return ((ExptArrival ?? SchArrival) - DateTime.Now).TotalMinutes;
        }

        /// <summary>
        ///     Gets a list of upcoming arrivals at a specific bus stop. Can throw an exception.
        /// </summary>
        /// <param name="actoCode">The Acto-code ID for a specific bus stop.</param>
        /// <returns>A list of Live Records containing details about upcoming buses.</returns>
        /// <exception cref="NullReferenceException">Thrown if there is an error with the API.</exception>
        /// <exception cref="Exception">Thrown if you have used an invalid or expired API key.</exception>
        public static List<LiveRecord> GetLiveData(string actoCode)
        {
            XDocument doc = XDocument.Load("https://rtl2.ods-live.co.uk/api/siri/sm?key=" + ReadingBuses.APIKey +
                                           "&location=" + actoCode);
            XNamespace ns = doc.Root.GetDefaultNamespace();
            var arrivals = doc.Descendants(ns + "MonitoredStopVisit").Select(x => new LiveRecord()
            {
                ServiceNumber = (string) x.Descendants(ns + "LineRef").FirstOrDefault(),
                Destination = (string) x.Descendants(ns + "DestinationName").FirstOrDefault(),
                SchArrival = (DateTime) x.Descendants(ns + "AimedArrivalTime").FirstOrDefault(),
                ExptArrival = (DateTime?) x.Descendants(ns + "ExpectedArrivalTime").FirstOrDefault(),
                OperatorCode = ReadingBuses.GetOperatorE((string) x.Descendants(ns + "OperatorRef").FirstOrDefault()),
                VehicleRef = (string) x.Descendants(ns + "VehicleRef").FirstOrDefault(),
                ViaMessage = (string) x.Descendants(ns + "Via").FirstOrDefault()
            }).ToList();

            return arrivals;
        }
    }
}
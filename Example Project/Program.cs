using System;
using System.Collections.Generic;
using ReadingBusesAPI;

namespace Example_Project
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            ReadingBuses.setCache(true);
            await ReadingBuses.initialise("");
       
            ReadingBuses rb = ReadingBuses.getInstance();
            LivePosition[] t = await rb.getLiveVehiclePositions();
            t[1].BrandName();
            BusService Services = rb.getService("17");
            List<LiveRecord> s = await Services.getLocations()[0].getLiveData();

            Console.WriteLine(s[0].DisplayTime());
            BusService[] getServices = rb.getServices();
            BusService[] pink = rb.getServices("pink");
            rb.printServices();
            Services.printLocationsActo();
            Console.WriteLine(Services.BrandName);
            BusStop[] n = Services.getLocations();
            Services.printLocationNames();
        }
    }
}

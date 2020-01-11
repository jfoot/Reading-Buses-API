using System;
using System.Collections.Generic;
using ReadingBusesAPI;

namespace Example_Project
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            ReadingBuses.SetCache(true);
            await ReadingBuses.Initialise("");
            ReadingBuses.DeleteCahce();
            
            ReadingBuses rb = ReadingBuses.GetInstance();
            
            LivePosition[] t = await rb.GetLiveVehiclePositions();
            t[1].BrandName();
            BusService Services = rb.GetService("17");
            List<LiveRecord> s = Services.GetLocations()[0].GetLiveData();

            Console.WriteLine(s[0].DisplayTime());
            BusService[] getServices = rb.GetServices();
            BusService[] pink = rb.GetServices("pink");
            rb.PrintServices();
            Services.PrintLocationsActo();
            Console.WriteLine(Services.BrandName);
            BusStop[] n = Services.GetLocations();
            Services.PrintLocationNames();

        }
    }
}

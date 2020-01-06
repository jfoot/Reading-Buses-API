using System;
using System.Collections.Generic;
using ReadingBusesAPI;

namespace Example_Project
{
    class Program
    {
        static void Main(string[] args)
        {

            ReadingBuses.initialise("XXXX");
            ReadingBuses rb = ReadingBuses.getInstance();
            LivePosition[] t = rb.getLiveVehiclePositions();
            t[1].BrandName();
            BusService Services = rb.getService("22");
            List<LiveRecord> s = Services.getLocations()[0].getLiveData();
            
            Console.WriteLine(s[0].DisplayTime());
            BusService[] getServices = rb.getServices();
            BusService[] pink = rb.getServices("pinkerer");
            rb.printServices();
            Services.printLocationsActo();
            Console.WriteLine(Services.BrandName); 
            BusStop[] n = Services.getLocations();
            Services.printLocationNames();
        }
    }
}

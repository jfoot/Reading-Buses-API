﻿using System;
using System.Collections.Generic;
using ReadingBusesAPI;

namespace Example_Project
{
    class Program
    {
        static void Main(string[] args)
        {

            ReadingBuses.initialise(Console.ReadLine());
            ReadingBuses rb = ReadingBuses.getInstance();

            BusService Services = rb.getService("22");
            BusService[] getServices = rb.getServices();
            BusService[] pink = rb.getServices("pink");
            rb.printServices();
            Services.printLocationsActo();
            BusStop[] n = Services.getLocations();
            Services.printLocationNames();
        }
    }
}

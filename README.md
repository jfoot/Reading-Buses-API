[![.NET Core](https://github.com/jfoot/Reading-Buses-API/workflows/.NET%20Core/badge.svg)](https://github.com/jfoot/Reading-Buses-API/actions)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/654ef87688234627bd523c1db8318090)](https://www.codacy.com/manual/jfoot/Reading-Buses-API?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=jfoot/Reading-Buses-API&amp;utm_campaign=Badge_Grade)
[![https://img.shields.io/badge/Documentation-View-blue](https://img.shields.io/badge/Documentation-View-blue)](https://jonathanfoot.com/Projects/RBAPI/docs/)
# Reading Buses API
A C#, .net Standard Library for the new [Reading Buses API](https://reading-opendata.r2p.com/api-service), available to use in your C# console, UWP, WPF or Win Form Applications.

This includes bus services operated by Reading Buses and subsidiaries such as, Thames Valley Buses and Newbury & District Buses.

The library supports the List of Bus Stops, Live Vehicle Positions, Live Journey Details, Stop Predictions, List of Lines, Line Patterns, Timetabled Journeys, Tracking History and the Vehicle Position History API..

Get your own API Keys from: https://reading-opendata.r2p.com/api-service

## Examples
Examples code can be found on the [Examples Repository here](https://github.com/jfoot/Reading-Buses-API-Examples/blob/master/ReadingBusesNewAPIWithLibrary/Program.cs)

## Documentation
Documentation can be found by [online here](https://jonathanfoot.com/Projects/RBAPI/docs/index.html) or the "docs" folder of this repository.


## Download & Installation
Get the package from the [nuget store here](https://www.nuget.org/packages/ReadingBusesAPI/)

# Very Quick Start
First you need to initialise the library by providing your API key:

```c#
ReadingBuses Controller = await ReadingBuses.Initialise("APIKEY");
```
Once Initialised all future reference can be got using:
```c#
ReadingBuses Controller = ReadingBuses.GetInstance();
```
### Get a List of Bus Services 
To get a list of bus services operated by Reading Buses and the information about them you can do any of the following:

***To get all Services:***
```c#
BusService[] Services = Controller.GetServices();
```
***To get all Services from a company:***
```c#
BusService[] RBServices = Controller.GetServices(Company.ReadingBuses);
BusService[] TVServices = Controller.GetServices(Company.ThamesValley);
```

***To get specific Services based on their brand:***
```c#
BusService[] Services = Controller.GetServices("pink");
```
***To get specific Service by Service Number and Company Operator:***

*Note that the API has data for Reading Buses, Thames Valley Buses and Newbury & District Buses, as such, service id might not be unique. For example, Reading Buses and Newbury & District both operate a service number "1", so operator is needed to specify which one.*

```c#
BusService purple17 = Controller.GetService("17", Company.ReadingBuses);
```

***To get a service's timetable***
```c#
Journey[] timetable = await purple17.GetTimeTable(DateTime.Now);
```

***To get a service's historical journeys***
```c#
HistoricJourney[] timetable = await purple17.GetArchivedTimeTable(DateTime.Now.AddDays(-5));
```

**To get a service's route***
```c#
BusStop[] outbound = await purple17.GetLocations(Direction.Outbound);
BusStop[] inbound = await purple17.GetLocations(Direction.Inbound);
```

### Get a List of Locations (Bus Stops)
***To get all locations***
```c#
BusStop[] Locations = Controller.GetLocations();
```
***To get location based on Acto-Code (Bus Stop ID)***
```c#
BusStop stop = Controller.GetLocation("039028150002");
```

***To get live stop departure information***
```c#
LiveRecord[] departures = await stop.GetLiveData();
```


### Get GPS Data
***To get Live GPS Data***
```c#
LiveVehiclePosition[] Positions = await Controller.GpsController.GetLiveVehiclePositions();
```

***To get Archived GPS Data***
```c#
VehiclePosition[] Positions = await Controller.GpsController.GetArchivedVehiclePositions(DateTime.Now.AddDays(-1), new TimeSpan(3, 0, 0));
```


Once you have the bus service or location you want to inspect, they have various properties to let you get further data about them. Such as live bus stop data, GPS data, bus service routes. For more examples, please see the repository linked above.

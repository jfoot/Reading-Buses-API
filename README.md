![.NET Core](https://github.com/jfoot/Reading-Buses-API/workflows/.NET%20Core/badge.svg)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/654ef87688234627bd523c1db8318090)](https://www.codacy.com/manual/jfoot/Reading-Buses-API?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=jfoot/Reading-Buses-API&amp;utm_campaign=Badge_Grade)
[![https://img.shields.io/badge/Documentation-View-blue](https://img.shields.io/badge/Documentation-View-blue)](https://jonathanfoot.com/Projects/RBAPI/docs/)
# Reading Buses API
A C# Library for the [Reading Buses API](http://rtl2.ods-live.co.uk/cms/apiservice), available to use in your C# console, UWP, WPF or Win Form Applications.

The library supports the List of Bus Stops, Live Vehicle Positions, Stop Predictions, List of Services, Line Patterns, Timetabled Journeys, Tracking History and the Vehicle Position History API..

Get your own API Keys from: http://rtl2.ods-live.co.uk/cms/apiservice

## Examples
Examples code can be found on the [Examples Repository here](https://github.com/jfoot/Reading-Buses-API-Examples/blob/master/ReadingBusesNewAPIWithLibrary/Program.cs)

## Documentation
Documentation can be found by [online here](https://jonathanfoot.com/Projects/RBAPI/docs/index.html) or the "docs" folder of this repository.


## Download & Installation
Get the package from the [nuget store here](https://www.nuget.org/packages/ReadingBusesAPI/)

# Very Quick Start
First you need to initialise the Library by providing your API key:

```c#
ReadingBuses Controller = await ReadingBuses.Initialise("APIKEY");
```
Once Initlised all future reference can be got using:
```c#
ReadingBuses Controller = ReadingBuses.GetInstance();
```
### Get a List of Bus Services 
To get a list of bus services operated by Reading Buses and the information about them you can do any of the following:

***To get all Services:***
```c#
BusService[] Services = Controller.GetServices();
```
***To get specific Services based on their brand:***
```c#
BusService[] Services = Controller.GetServices("pink");
```
***To get specific Service by Service Number and Operator:***
```c#
BusService Services = Controller.GetService("17", Operators.ReadingBuses);
```

### Get a List of Locations (Bus Stops)
***To get all locations***
```c#
BusStop[] Locations = Controller.GetLocations();
```
***To get location based on Acto-Code (Bus Stop ID)***
```c#
BusStop Locations = Controller.GetLocation("33245365434");
```


### Get GPS Data
***To get Live GPS Data***
```c#
LivePosition[] Positions = await Controller.GpsController.GetLiveVehiclePositions();
```

***To get Archived GPS Data***
```c#
LivePosition[] Positions = await Controller.GpsController.GetArchivedVehiclePositions(DateTime.Now.AddDays(-1), new TimeSpan(3, 0, 0));
```


Once you have the bus service or location you want to inspect, they have various properties to let you get further data about them. Such as live bus stop data, GPS data, bus service routes. For more examples please see the repository linked above.

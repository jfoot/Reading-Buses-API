![.NET Core](https://github.com/jfoot/Reading-Buses-API/workflows/.NET%20Core/badge.svg)
# Reading-Buses-API
A C# Library for the Reading Buses API (currently only for .net Core, but can be converted to .net Framework with ease.) 
The library supports the List of Bus Stops, Live Vehicle Positions, Stop Predictions, List of Services and Line Patterns API.

Get your own API Keys from: http://rtl2.ods-live.co.uk/cms/apiservice

## Example
Examples code can be found on the [Examples Repository here](https://github.com/jfoot/Reading-Buses-API-Examples/blob/master/ReadingBusesNewAPIWithLibrary/Program.cs)

## Download
Get the package from the [nuget store here](https://www.nuget.org/packages/ReadingBusesAPI/)

# Quick Start
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
***To get specific Service by Service Number:***
```c#
BusService Services = Controller.GetServices("17");
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

Once you have the bus service or location you want to inspect, they have various properties to let you get further data about them. Such as live bus stop data, GPS data, bus service routes.

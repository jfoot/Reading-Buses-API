using ReadingBusesAPI;
using ReadingBusesAPI.BusServices;
using ReadingBusesAPI.BusStops;

Console.WriteLine("Starting...");
ReadingBuses.SetCache(true);

ReadingBuses controller = await ReadingBuses.Initialise("");

BusService service = controller.GetService("22", ReadingBusesAPI.Common.Company.ReadingBuses);

BusStop[] stops = await service.GetLocations();

BusStop[] outbound = await service.GetLocations(ReadingBusesAPI.Common.Direction.Outbound);
BusStop[] inbound = await service.GetLocations(ReadingBusesAPI.Common.Direction.Inbound);

Console.WriteLine("hello");

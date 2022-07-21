using ReadingBusesAPI;
using ReadingBusesAPI.BusServices;
using ReadingBusesAPI.BusStops;

Console.WriteLine("Starting...");
ReadingBuses.SetCache(false);

ReadingBuses controller = await ReadingBuses.Initialise("");

BusService service = controller.GetService("22", ReadingBusesAPI.Common.Company.ReadingBuses);


var temp = await service.GetArchivedTimeTable(DateTime.Now.AddDays(-5));


BusStop[] stops = await service.GetLocations();

BusStop[] outbound = await service.GetLocations(ReadingBusesAPI.Common.Direction.Outbound);
BusStop[] inbound = await service.GetLocations(ReadingBusesAPI.Common.Direction.Inbound);

Console.WriteLine("hello");

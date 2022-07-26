using ReadingBusesAPI;
using ReadingBusesAPI.BusServices;
using ReadingBusesAPI.BusStops;
using ReadingBusesAPI.TimeTable;
using ReadingBusesAPI.VehiclePositions;

Console.WriteLine("Starting...");
ReadingBuses.SetCache(false);
ReadingBuses.SetArchiveCache(false);

ReadingBuses controller = await ReadingBuses.Initialise("");

BusService service = controller.GetService("17", ReadingBusesAPI.Common.Company.ReadingBuses);

Journey[] temp = await service.GetTimeTable(DateTime.Now.AddDays(-5));

HistoricJourney[] temp2 = await service.GetArchivedTimeTable(DateTime.Now.AddDays(-5));

LivePosition[] s = await service.GetLivePositions();


BusStop[] stops = await service.GetLocations();

BusStop[] outbound = await service.GetLocations(ReadingBusesAPI.Common.Direction.Outbound);
BusStop[] inbound = await service.GetLocations(ReadingBusesAPI.Common.Direction.Inbound);

Console.WriteLine("hello");

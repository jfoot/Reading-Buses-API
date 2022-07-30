using ReadingBusesAPI;
using ReadingBusesAPI.BusServices;
using ReadingBusesAPI.BusStops;
using ReadingBusesAPI.JourneyDetails;
using ReadingBusesAPI.TimeTable;
using ReadingBusesAPI.VehiclePositions;

Console.WriteLine("Starting...");
ReadingBuses.SetCache(false);
ReadingBuses.SetArchiveCache(false);

ReadingBuses controller = await ReadingBuses.Initialise("");

BusService service = controller.GetService("17", ReadingBusesAPI.Common.Company.ReadingBuses);


var sdfsf = await service.GetLiveJourneyData();

//Journey[] temp = await service.GetTimeTable(DateTime.Now.AddDays(-5));

//HistoricJourney[] temp2 = await service.GetArchivedTimeTable(DateTime.Now.AddDays(-5));

//VehiclePosition[] s = await service.GetLivePositions();

LiveVehiclePosition[] s2 = await controller.GpsController.GetLiveVehiclePositions();

HistoricJourney[] deat = await s2[0].GetLiveJourneyData();

//VehiclePosition[] s3 = await controller.GpsController.GetArchivedVehiclePositions(DateTime.Now.AddDays(-5), new TimeSpan(5,0,0));

BusStop[] stops = controller.GetLocations();

LiveRecord[] fs = await stops[2].GetLiveData();

var sfsfs = await fs[2].GetLiveJourneyData();


BusStop[] outbound = await service.GetLocations(ReadingBusesAPI.Common.Direction.Outbound);
BusStop[] inbound = await service.GetLocations(ReadingBusesAPI.Common.Direction.Inbound);

Console.WriteLine("hello");

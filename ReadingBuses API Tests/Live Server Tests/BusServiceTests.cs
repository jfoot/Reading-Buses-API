// Copyright (c) Jonathan Foot. All Rights Reserved. 
// Licensed under the GNU Affero General Public License, Version 3.0 
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using ReadingBusesAPI;
using ReadingBusesAPI.BusServices;
using ReadingBusesAPI.BusStops;
using ReadingBusesAPI.Common;
using ReadingBusesAPI.ErrorManagement;
using ReadingBusesAPI.TimeTable;
using ReadingBusesAPI.VehiclePositions;

namespace ReadingBuses_API_Tests.Live_Server_Tests
{
	/// <summary>
	///     Tests for the 'BusService' class on the live server.
	/// </summary>
	[TestFixture]
	internal class BusServiceTests
	{
		private BusService _testService;

		/// <summary>
		///     Directs traffic to dummy server.
		/// </summary>
		[OneTimeSetUp]
		public void Setup()
		{
			//Use the Live server connection. 
			ReadingBuses.SetDebugging(false);
			_testService = ReadingBuses.GetInstance().GetService("17", Company.ReadingBuses);
		}


		/// <summary>
		///     Check the default constructor
		/// </summary>
		[Test]
		public void CheckDefaultConstructor()
		{
			BusService service = new BusService("22");

			Assert.AreEqual("22", service.ServiceId);
			Assert.AreEqual(Company.Other, service.Company);
		}


		/// <summary>
		///     Check that an array of archived time table records is returned.
		/// </summary>
		[Test]
		public async Task CheckGetArchivedTimeTableAsync()
		{
			HistoricJourney[] timeTable = await _testService.GetArchivedTimeTable(DateTime.Now.AddDays(-1));


			if (timeTable.Length == 0)
			{
				Assert.Fail("No time table records were returned.");
			}


			Assert.Pass();
		}


		/// <summary>
		///     Check that an error is thrown when trying to get future data.
		/// </summary>
		[Test]
		public void CheckGetArchivedTimeTableErrorAsync()
		{
			Assert.ThrowsAsync<ReadingBusesApiExceptionMalformedQuery>(async () =>
				await _testService.GetArchivedTimeTable(DateTime.Now.AddDays(10)));
		}


		/// <summary>
		///     Check that an array of Live GPS positions is returned is returned.
		/// </summary>
		[Test]
		public async Task CheckGetLivePositionsAsync()
		{
			LiveVehiclePosition[] livePositions = await _testService.GetLivePositions();


			if (livePositions.Length != 0)
			{
				Assert.Pass();
			}
			else
			{
				Assert.Fail("No live positions were returned.");
			}
		}


		/// <summary>
		///     Check that an array of string of acto codes is returned.
		/// </summary>
		[Test]
		public async Task CheckGetLocationsActoCodesAsync()
		{
			string[] actoCodes = await _testService.GetLocationsActo();

			foreach (var actoCode in actoCodes)
			{
				if (!ReadingBuses.GetInstance().IsLocation(actoCode))
				{
					Assert.Fail("Not a real location.");
				}
			}


			if (actoCodes.Length != 0)
			{
				Assert.Pass();
			}
			else
			{
				Assert.Fail("No acto-codes were returned.");
			}
		}


		/// <summary>
		///     Check that an array of locations is returned.
		/// </summary>
		[Test]
		public async Task CheckGetLocationsAsync()
		{
			BusStop[] locations = await _testService.GetLocations();


			if (locations.Length != 0)
			{
				Assert.Pass();
			}
			else
			{
				Assert.Fail("No locations were returned.");
			}
		}


		/// <summary>
		///     Check that an array of time table records is returned is returned.
		/// </summary>
		[Test]
		public async Task CheckGetTimeTableAsync()
		{
			const string actoCode = "039027540001";
			Journey[] timeTable = await _testService.GetTimeTable(DateTime.Now.AddDays(-1));


			if (timeTable.Length == 0)
			{
				Assert.Fail("No time table records were returned.");
			}

			Journey[] timeTableAtLocation =
				await _testService.GetTimeTable(DateTime.Now.AddDays(-1), new BusStop(actoCode));

			foreach (var journey in timeTableAtLocation)
			{
				foreach (var record in journey.Visits)
				{
					if (!record.AtcoCode.Equals(actoCode))
					{
						Assert.Fail("The time table record was not for the stop asked for.");
					}
				}
			}

			Assert.Pass();
		}



		/// <summary>
		///     Check the second constructor
		/// </summary>
		[Test]
		public void CheckSecondConstructor()
		{
			BusService service = new BusService("22", Company.ReadingBuses);

			Assert.AreEqual("22", service.ServiceId);
			Assert.AreEqual(Company.ReadingBuses, service.Company);
		}
	}
}

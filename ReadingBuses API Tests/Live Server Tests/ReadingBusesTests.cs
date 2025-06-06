﻿// Copyright (c) Jonathan Foot. All Rights Reserved. 
// Licensed under the GNU Affero General Public License, Version 3.0 
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using NUnit.Framework;
using ReadingBusesAPI;
using ReadingBusesAPI.BusServices;
using ReadingBusesAPI.BusStops;
using ReadingBusesAPI.Common;
using ReadingBusesAPI.ErrorManagement;

namespace ReadingBuses_API_Tests.Live_Server_Tests
{
	/// <summary>
	///     Tests for the 'ReadingBuses' class on a live server.
	/// </summary>
	[TestFixture]
	public class Tests
	{
		/// <summary>
		///     Directs traffic to live server.
		/// </summary>
		[OneTimeSetUp]
		public void Setup()
		{
			//Do not use the dummy server connection. Actually connect to the real ones for these tests.
			ReadingBuses.SetDebugging(false);
		}


		/// <summary>
		///     Check that an error is thrown if you attempt to change the cache settings after setting up.
		/// </summary>
		[Test]
		public void CheckChangeCacheError()
		{
			Assert.Throws(typeof(ReadingBusesApiExceptionMalformedQuery),
				new TestDelegate(delegate { ReadingBuses.SetCache(false); }));

			Assert.Throws(typeof(ReadingBusesApiExceptionMalformedQuery),
				new TestDelegate(delegate { ReadingBuses.SetCache(true); }));
		}

		/// <summary>
		///     Check that the check location exists function is working correctly.
		/// </summary>
		[Test]
		public void CheckLocationExists()
		{
			Assert.That(ReadingBuses.GetInstance().IsLocation("039028160001"), Is.True);
			Assert.That(ReadingBuses.GetInstance().IsLocation(""), Is.False);
		}
		

		/// <summary>
		///     Check that the check location exists function is working correctly.
		/// </summary>
		[Test]
		public void CheckServicesExists()
		{
			Assert.That(ReadingBuses.GetInstance().IsService("17"), Is.True);
			Assert.That(ReadingBuses.GetInstance().IsService(""), Is.False);

		}

		/// <summary>
		///     Check that a Malformed Query Error is returned if you do not pass a valid date.
		/// </summary>
		[Test]
		[TestCase("708")]
		[TestCase("709")]
		[TestCase("414")]
		public void GetHistoricGpsDataInvalidDate(string vehicleId)
		{
			// Using a method as a delegate
			Assert.ThrowsAsync<ReadingBusesApiExceptionMalformedQuery>(async () =>
				await ReadingBuses.GetInstance().GetVehicleTrackingHistory(DateTime.Now.AddDays(10), vehicleId));
		}


		/// <summary>
		///     Check that a Malformed Query Error is returned if you do not pass a valid vehicle ID.
		/// </summary>
		[Test]
		public void GetHistoricGpsDataInvalidVehicle()
		{
			// Using a method as a delegate
			Assert.ThrowsAsync<ReadingBusesApiExceptionMalformedQuery>(async () =>
				await ReadingBuses.GetInstance().GetVehicleTrackingHistory(DateTime.Now.AddDays(-1), ""));
		}

		/// <summary>
		///     Check that we can get an individual bus stop.
		/// </summary>
		[Test]
		[TestCase("039028160001")]
		[TestCase("039026100006")]
		[TestCase("039025530002")]
		public void GetLocation(string actoCode)
		{
			BusStop stop = ReadingBuses.GetInstance().GetLocation(actoCode);
			Assert.That(stop.ActoCode, Is.EqualTo(actoCode));
		}


		/// <summary>
		///     Check that a Malformed Query Error is returned if we attempt to get a non-existent stop.
		/// </summary>
		[Test]
		public void GetLocationError()
		{
			Assert.Throws(typeof(ReadingBusesApiExceptionMalformedQuery),
				new TestDelegate(delegate { ReadingBuses.GetInstance().GetLocation(""); }));
		}

		/// <summary>
		///     Check that bus stop locations have been found.
		/// </summary>
		[Test]
		public void GetLocations()
		{
			BusStop[] stops = ReadingBuses.GetInstance().GetLocations();
			if (stops.Length != 0)
			{
				Assert.Pass();
			}
			else
			{
				Assert.Fail("No Bus Stop locations have been found.");
			}
		}

		/// <summary>
		///     Check that we can get an individual bus service.
		/// </summary>
		[Test]
		[TestCase("17")]
		[TestCase("1")]
		[TestCase("4a")]
		public void GetService(string serviceNumber)
		{
			BusService service = ReadingBuses.GetInstance().GetService(serviceNumber, Company.ReadingBuses);
			Assert.That(service.ServiceId, Is.EqualTo(serviceNumber));
		}


		/// <summary>
		///     Check that we can get a bus service by ID.
		/// </summary>
		[Test]
		public void GetServiceAll()
		{
			const string serviceNumber = "17";
			BusService[] service = ReadingBuses.GetInstance().GetService(serviceNumber);


			if (service.Length != 0 &&
			    service.All(x => x.ServiceId.Equals(serviceNumber, StringComparison.OrdinalIgnoreCase)))
			{
				Assert.Pass();
			}
			else
			{
				Assert.Fail("The retrieved bus services does not match the service asked for.");
			}
		}


		/// <summary>
		///     Check that a Malformed Query Error is returned if we attempt to get a non-existent bus service.
		/// </summary>
		[Test]
		public void GetServiceError()
		{
			Assert.Throws(typeof(ReadingBusesApiExceptionMalformedQuery),
				new TestDelegate(delegate { ReadingBuses.GetInstance().GetService("", Company.Other); }));
		}

		/// <summary>
		///     Check that bus services have been found.
		/// </summary>
		[Test]
		public void GetServices()
		{
			BusService[] services = ReadingBuses.GetInstance().GetServices();
			if (services.Length != 0)
			{
				Assert.Pass();
			}
			else
			{
				Assert.Fail("No Bus services have been found.");
			}
		}
	}
}

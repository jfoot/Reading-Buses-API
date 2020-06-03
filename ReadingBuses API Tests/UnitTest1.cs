﻿using System;
using NUnit.Framework;
using ReadingBusesAPI;

namespace ReadingBuses_API_Tests
{
	[TestFixture]
	public class Tests
	{
		[SetUp]
		public void Setup()
		{
			ReadingBuses.SetCache(false);
			ReadingBuses.SetDebugging(false);
			ReadingBuses.SetFullError(true);
			ReadingBuses.SetWarning(true);
			Console.WriteLine("Starting Unit Tests...");
			ReadingBuses controller = ReadingBuses.Initialise(Environment.GetEnvironmentVariable("API_KEY")).Result;
		}

		[Test]
		public void Test1()
		{
			Assert.Pass();
		}
	}
}

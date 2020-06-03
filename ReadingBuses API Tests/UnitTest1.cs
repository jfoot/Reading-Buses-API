using System;
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
			ReadingBuses.SetDebugging(true);
			ReadingBuses.SetFullError(true);
			ReadingBuses.SetWarning(true);
			ReadingBuses controller = ReadingBuses.Initialise("").Result;
			Console.WriteLine("Starting Unit Tests...");
		}

		[Test]
		public void Test1()
		{
			Assert.Pass();
		}
	}
}

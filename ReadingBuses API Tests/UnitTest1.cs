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
            //Initializes the controller, enter in your Reading Buses API - Get your own from http://rtl2.ods-live.co.uk/cms/apiservice
            //Once Instantiated you can also use, "ReadingBuses.GetInstance();" to get future instances.
            ReadingBuses controller = ReadingBuses.Initialise("").Result;
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}
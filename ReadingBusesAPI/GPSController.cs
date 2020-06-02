using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ReadingBusesAPI
{
    /// <summary>
    ///     Helps get live and historical GPS data on vehicles by accessing the "Live Vehicle Positions" API.
    /// </summary>
    public class GPSController
    {
        /// <value>The last time a GPS request was made. This is used to prevent unnecessary API calls.</value>
        private static DateTime _lastRetrieval;

        /// <value>Holds the cache data for live GPS of vehicles.</value>
        private LivePosition[] _livePositionCache;

        /// <summary>
        ///     Creates a GPS Controller, you should not need to make your own GPS controller, you can get an instance of one via
        ///     the main 'ReadingBuses' object.
        /// </summary>
        internal GPSController()
        {
            _lastRetrieval = DateTime.Now.AddHours(-1);
        }

        /// <summary>
        ///     GPS data only updates every 30 seconds, so on average you will need to wait 15s for new data.
        ///     This is used to check how long it was since last requesting GPS data. If it was recently
        ///     there  is no point making another request to the API as you will get the same data and take longer.
        /// </summary>
        /// <returns>Returns if it has been less than 15 seconds from last asking for GPS data.</returns>
        private bool RefreshCache() => (DateTime.Now - _lastRetrieval).TotalSeconds > 15;


        /// <summary>
        ///     Gets live GPS data for all buses currently operating.
        /// </summary>
        /// <returns>An array of GPS locations for all buses operating by Reading Buses currently</returns>
        /// <exception cref="InvalidOperationException">Thrown if the API key is invalid or expired.</exception>
        public async Task<LivePosition[]> GetLiveVehiclePositions()
        {
            if (RefreshCache() || _livePositionCache == null)
            {
                var download =
                    await new WebClient().DownloadStringTaskAsync(
                        new Uri("https://rtl2.ods-live.co.uk/api/vehiclePositions?key=" + ReadingBuses.APIKey));
                _livePositionCache = JsonConvert.DeserializeObject<LivePosition[]>(download).ToArray();
                _lastRetrieval = DateTime.Now;
            }

            return _livePositionCache;
        }

        /// <summary>
        ///     Gets live GPS data for a single buses matching Vehicle ID number.
        /// </summary>
        /// <returns>The GPS point of Vehicle matching your ID provided.</returns>
        /// <exception cref="InvalidOperationException">
        ///     Thrown if a vehicle of the ID does not exist or is not currently active.
        ///     You can check by using the 'IsVehicle' function.
        /// </exception>
        public async Task<LivePosition> GetLiveVehiclePosition(string vehicle)
        {
            if (await IsVehicle(vehicle))
                return (await GetLiveVehiclePositions()).Single(o =>
                    string.Equals(o.Vehicle, vehicle, StringComparison.CurrentCultureIgnoreCase));

            throw new InvalidOperationException(
                "A Vehicle of that ID can not be found currently operating. You can first check with the 'IsVehicle' function.");
        }

        /// <summary>
        ///     Checks if the Vehicle ID Number is currently in service right now.
        /// </summary>
        /// <param name="vehicle">Vehicle ID Number eg 414</param>
        /// <returns>True or False for if the buses GPS can be found or not currently.</returns>
        public async Task<bool> IsVehicle(string vehicle) => (await GetLiveVehiclePositions()).Any(o =>
            string.Equals(o.Vehicle, vehicle, StringComparison.CurrentCultureIgnoreCase));
    }
}
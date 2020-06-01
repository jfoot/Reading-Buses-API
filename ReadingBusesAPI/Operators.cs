namespace ReadingBusesAPI
{
    /// <summary>
    ///     An Enum of the Operators Reading Buses owns or manages in their API.
    /// </summary>
    public enum Operators
    {
        /// <summary>
        ///     For Reading Buses services
        /// </summary>
        ReadingBuses,

        /// <summary>
        ///     For Kennections services
        /// </summary>
        Kennections,

        /// <summary>
        ///     For Newbury And District services
        /// </summary>
        NewburyAndDistrict,

        /// <summary>
        ///     For any other operator which is new in the API and has not yet been officially supported in this library.
        /// </summary>
        Other
    }
}
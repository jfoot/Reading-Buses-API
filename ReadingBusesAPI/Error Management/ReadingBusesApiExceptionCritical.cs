using System;

namespace ReadingBusesAPI.Error_Management
{
    /// <summary>
    ///     An exception type which is thrown when the cause of the error is unknown.
    /// </summary>
    public class ReadingBusesApiExceptionCritical : ReadingBusesApiException
    {
        internal ReadingBusesApiExceptionCritical() : base(
            "An unexplainable critical error has occurred, this is most likely due to an error in the Reading Buses API. For full details turn on 'Full Error Logs'. If this keeps happening please report it on Git hub for investigation.")
        {
        }

        internal ReadingBusesApiExceptionCritical(string message) : base(message)
        {
        }

        internal ReadingBusesApiExceptionCritical(string message, Exception innerException) : base(message,
            innerException)
        {
        }
    }
}
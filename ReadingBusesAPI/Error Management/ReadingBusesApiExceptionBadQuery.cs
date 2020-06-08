using System;

namespace ReadingBusesAPI.Error_Management
{
    /// <summary>
    ///     An exception type which is used when the API returns back an error message.
    ///     Most likely due to an invalid request such as asking for data that does not exist.
    /// </summary>
#pragma warning disable CA1032 // Implement standard exception constructors
    public class ReadingBusesApiExceptionBadQuery : ReadingBusesApiException
#pragma warning restore CA1032 // Implement standard exception constructors
    {
        internal ReadingBusesApiExceptionBadQuery(ErrorFormat content) : base(content.Message)
        {
        }

        internal ReadingBusesApiExceptionBadQuery(string content) : base(content)
        {
        }

        internal ReadingBusesApiExceptionBadQuery(string message, Exception innerException) : base(message,
            innerException)
        {
        }
    }
}
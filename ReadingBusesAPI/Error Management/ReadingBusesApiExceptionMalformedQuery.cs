using System;

namespace ReadingBusesAPI.Error_Management
{
    /// <summary>
    ///     An exception type which is used when the user asks to make a invalid API call
    ///     This is would be thrown during checks done before even directly calling upon the web API.
    ///     For example if you have not filtered by at least one property when required too.
    /// </summary>
    public class ReadingBusesApiExceptionMalformedQuery : ReadingBusesApiException
    {
        internal ReadingBusesApiExceptionMalformedQuery() : base()
        {
        }


        internal ReadingBusesApiExceptionMalformedQuery(string content) : base(content)
        {
        }

        internal ReadingBusesApiExceptionMalformedQuery(string message, Exception innerException) : base(message,
            innerException)
        {
        }
    }
}
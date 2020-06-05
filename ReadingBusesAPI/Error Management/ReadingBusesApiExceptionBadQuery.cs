using System;

namespace ReadingBusesAPI.Error_Management
{
    /// <summary>
    /// An exception type which is used when the API returns back an error message.
    /// Most likely due to an invalid request such as asking for data that does not exist. 
    /// </summary>
    public class ReadingBusesApiExceptionBadQuery : ReadingBusesApiException
    {
        internal ReadingBusesApiExceptionBadQuery() : base(){}


        internal ReadingBusesApiExceptionBadQuery(ErrorFormat content) : base(content.Message)
        {
        }

        internal ReadingBusesApiExceptionBadQuery(string content) : base(content)
        {
        }

        internal ReadingBusesApiExceptionBadQuery(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
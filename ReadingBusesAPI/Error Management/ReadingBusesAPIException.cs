// Copyright (c) Jonathan Foot. All Rights Reserved. 
// Licensed under the GNU Affero General Public License, Version 3.0 
// See the LICENSE file in the project root for more information.

using System;

namespace ReadingBusesAPI.Error_Management
{
    /// <summary>
    /// Stores the basic/base type of Exception which can be thrown by the API.
    /// </summary>
    public class ReadingBusesApiException : Exception
    {
        private ReadingBusesApiException()
        {
        }

        internal ReadingBusesApiException(string content) : base(content)
        {
        }
    }

    /// <summary>
    /// An exception type which is thrown when the cause of the error is unknown.
    /// </summary>
    public class ReadingBusesApiExceptionCritical : ReadingBusesApiException
    {
        internal ReadingBusesApiExceptionCritical() : base("An unexplainable critical error has occurred, this is most likely due to an error in the Reading Buses API. For full details turn on 'Full Error Logs'. If this keeps happening please report it on Git hub for investigation.")
        {
        }
    }

    /// <summary>
    /// An exception type which is used when the user asks to make a invalid API call
    /// This is would be thrown during checks done before even directly calling upon the web API.
    /// For example if you have not filtered by at least one property when required too.
    /// </summary>
    public class ReadingBusesApiExceptionMalformedQuery : ReadingBusesApiException
    {
        internal ReadingBusesApiExceptionMalformedQuery(string content) : base(content)
        {
        }
    }

    /// <summary>
    /// An exception type which is used when the API returns back an error message.
    /// Most likely due to an invalid request such as asking for data that does not exist. 
    /// </summary>
    public class ReadingBusesApiExceptionBadQuery : ReadingBusesApiException
    {
        internal ReadingBusesApiExceptionBadQuery(ErrorFormat content) : base(content.Message)
        {
        }

        internal ReadingBusesApiExceptionBadQuery(string content) : base(content)
        {
        }
    }
}

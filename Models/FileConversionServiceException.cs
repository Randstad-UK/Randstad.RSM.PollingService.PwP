using System;
using System.Runtime.Serialization;

namespace Randstad.RSM.PollingService.PwP.Models
{
    public class FileConversionServiceException : Exception
    {
        public FileConversionServiceException()
        {
        }

        protected FileConversionServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public FileConversionServiceException(string message) : base(message)
        {
        }

        public FileConversionServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
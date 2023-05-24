using System;
using System.Runtime.Serialization;

namespace Randstad.RSM.PollingService.PwP.Models
{
    public class TnaImportException : Exception
    {
        public TnaImportException()
        {
        }

        protected TnaImportException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public TnaImportException(string message) : base(message)
        {
        }

        public TnaImportException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

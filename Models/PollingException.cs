using System;
using System.Runtime.Serialization;

namespace Randstad.RSM.PollingService.PwP.Models
{
    public class PollingException : Exception
    {
        public PollingException()
        {
        }

        protected PollingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public PollingException(string message) : base(message)
        {
        }

        public PollingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

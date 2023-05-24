using System;

namespace Randstad.RSM.PollingService.PwP.Models
{
    public class ClientRate
    {
        public Guid RateId { get; set; }
        public Guid ClientId { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string ImportRateIdentifier { get; set; }
    }
}

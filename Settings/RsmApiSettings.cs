using System;
using System.Collections.Generic;
using System.Text;

namespace Randstad.RSM.PollingService.PwP.Settings
{
    public class RsmApiSettings
    {
        public string Name { get; set; }
        public string GetUnpaidInvoiceRefCodesEndpoint { get; set; }
        public string GetInvoicesByRefCodesEndpoint { get; set; }
        public string UpdateInvoiceToBePaidEndpoint { get; set; }
    }
}

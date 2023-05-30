
using Dapper;
using Randstad.Logging.Core;
using Randstad.RSM.PollingService.PwP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Randstad.RSM.PollingService.PwP.Services.DataAccess
{
    public class DreamDAL : IDreamDAL
    {
        private IDataAccess _sqlDataAccess;
        private string _query;

        private readonly string _selectInvoice = "SELECT DOCNUM,Account,Value,DETAILSTATUS,UNALLOCATED "
                                                + "from ranlive.SQSDBA.D_DETAILS with (nolock) "
                                                + "where ledger  = 'DEBTORS' and "
                                                + "NOMINAL in ('6000','6001') and "
                                                + "DOCNUM in (@invoiceNumber) ";

        public DreamDAL(IDataAccess dataAccess)
        {
            _sqlDataAccess = dataAccess;
        }

        public async Task<DreamInvoice> GetInvoiceByInvoiceNumber(string invoiceNumber, Guid correlationId)
        {
            _query = _selectInvoice.Replace("@invoiceNumber", invoiceNumber);

            var result = await _sqlDataAccess.QueryAsync<DreamInvoice>(_query, correlationId);

            return result.FirstOrDefault();
        }
    }
}

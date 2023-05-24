using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Randstad.RSM.PollingService.PwP.Services.DataAccess
{
    public interface IDataAccess
    {
        Task<int> ExecuteAsync(string sqlCommand, Guid correlationId);
        Task<IEnumerable<T>> QueryAsync<T>(string sqlSelect, Guid correlationId);
    }
}

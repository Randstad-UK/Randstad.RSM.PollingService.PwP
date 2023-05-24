using System;
using System.Threading.Tasks;

namespace Randstad.RSM.PollingService.PwP.Services.FileConversion
{
    /// <summary>Provides a mechanism for data conversion.</summary>
    public interface IFileConversionService<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename">Source data filename</param>
        /// <param name="correlationId">Process flow tracking Id</param>
        /// <param name="requiredColumnNames">Optional comma separated list of required fields, if supplied will be validated against the file, erroring if missing any.</param>
        /// <returns>T type</returns>
        Task<T> ConvertAsync(string filename, Guid correlationId, string requiredColumnNames = null);
    }
}

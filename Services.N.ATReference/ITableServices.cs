using System.Collections.Generic;
using System.Threading.Tasks;

namespace BGBA.Services.N.ATReference
{
    public interface ITableServices : BGBA.Models.N.Core.Trace.ITraceService
    {
        Task<T> GetTableByGet<T>(string tableName, Dictionary<string, string> filters);
        Task<T> GetTableByPost<T>(string tableName, Dictionary<string, string> filters);
    }
}

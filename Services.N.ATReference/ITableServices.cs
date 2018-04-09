using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.N.Consulta.ATReference
{
    public interface ITableServices
    {
        Task<T> GetTableByGet<T>(string tableName, Dictionary<string, string> filters);
        Task<T> GetTableByPost<T>(string tableName, Dictionary<string, string> filters);
    }
}

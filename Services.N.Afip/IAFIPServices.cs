using System.Threading.Tasks;
using BGBA.Models.N.Client;
using BGBA.Models.N.Afip;

namespace BGBA.Services.N.Afip
{
    public interface IAfipServices : BGBA.Models.N.Core.Trace.ITraceService
    {
        Task<Credentials> GetCredentials();
        Task<ClientData> GetClient(string Cuix);
    }
}

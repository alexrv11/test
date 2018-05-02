using System.Threading.Tasks;
using Models.N.Client;
using Models.N.Afip;

namespace Services.N.Afip
{
    public interface IAfipServices : Models.N.Core.Trace.ITraceService
    {
        Task<Credentials> GetCredentials();
        Task<ClientData> GetClient(string Cuix);
    }
}

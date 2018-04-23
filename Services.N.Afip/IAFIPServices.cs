using System.Threading.Tasks;
using Models.N.Client;
using Models.N.Afip;
using Services.N.Trace;

namespace Services.N.Afip
{
    public interface IAFIPServices : ITraceable
    {
        Task<Credentials> GetCredentials();
        Task<ClientData> GetClient(string Cuix);
    }
}

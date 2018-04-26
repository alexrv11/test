using System.Threading.Tasks;
using Models.N.Client;
using Models.N.Afip;

namespace Services.N.Afip
{
    public interface IAFIPServices
    {
        Task<Credentials> GetCredentials();
        Task<ClientData> GetClient(string Cuix);
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.N.Client
{
    public interface IClientServices
    {
        Task<string> GetCuix(string du, string sexo);
        Task<string> AddClient(Models.N.Client.MinimumClientData client);
        Task<string> GetClientNV(Models.N.Client.ClientData client);
        Task<bool> UpdateAddress(string idHost, Models.N.Location.Address address);
    }
}

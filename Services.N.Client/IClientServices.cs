using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.N.Client
{
    public interface IClientServices
    {
        Task<string> GetCuix(string du, string sexo);
        Task<bool> AddClient(Models.N.Client.ClientData client);
        Task<List<Models.N.Client.ClientData>> GetClientNV(Models.N.Client.ClientData client);
    }
}

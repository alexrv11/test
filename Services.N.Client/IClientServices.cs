using System.Threading.Tasks;

namespace BGBA.Services.N.Client
{
    public interface IClientServices : Models.N.Core.Trace.ITraceService
    {
        Task<string> GetCuix(string du, string sexo);
        Task<bool> AddClientNV(Models.N.Client.ClientData client);
        Task<Models.N.Client.ClientData> GetClientNV(Models.N.Client.ClientData client);
        Task<Models.N.Client.ClientData> GetClientAfip(string cuix);
        Task<bool> UpdateClientNV(string idHost, Models.N.Location.Address address, string email, Models.N.Client.Phone phone);
        Task<Models.N.Location.Address> NormalizeAddress(Models.N.Location.MapOptions address);
    }
}

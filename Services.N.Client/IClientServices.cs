using System.Threading.Tasks;

namespace BGBA.Services.N.Client
{
    public interface IClientServices : Models.N.Core.Trace.ITraceService
    {
        Task<string> GetCuix(string du, string sexo);
        Task<bool> AddClient(Models.N.Client.MinimumClientData client);
        Task<string> GetClientNV(Models.N.Client.MinimumClientData client);
        Task<bool> UpdateAddress(string idHost, Models.N.Location.Address address, string email);
        Task<Models.N.Location.Address> NormalizeAddress(Models.N.Location.MapOptions address);
    }
}

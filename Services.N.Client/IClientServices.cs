using System.Threading.Tasks;

namespace Services.N.Client
{
    public interface IClientServices : Models.N.Core.Trace.ITraceService
    {
        Task<string> GetCuix(string du, string sexo);
        Task<bool> AddClient(Models.N.Client.MinimumClientData client);
        Task<string> GetClientNV(Models.N.Client.MinimumClientData client);
        Task<bool> UpdateAddress(string idHost, Models.N.Location.Address address, string email);
    }
}

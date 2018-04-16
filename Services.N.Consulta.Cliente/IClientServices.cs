using System.Threading.Tasks;

namespace Services.N.Client
{
    public interface IClientServices
    {
        Task<string> GetCuix(string du, string sexo);
        Task<bool> AddClient(Models.N.Client.PadronData client);
    }
}

using System.Threading.Tasks;
using Models.N.Afip.AutenticacionAfip;
using Models.N.Afip.ConsultaClienteAfip;

namespace Services.N.Afip
{
    public interface IAFIPServices
    {
        Task<AutenticarYAutorizarConsumoWebserviceResponseDatosCredenciales> GetCredentials();
        Task<persona> GetClientAFIP(string Cuix);
    }
}

using System.Threading.Tasks;

namespace Services.N.Afip
{
    public interface IAFIPServices
    {
        Task<Models.AutenticacionAfip.AutenticarYAutorizarConsumoWebserviceResponseDatosCredenciales> GetCredentials();
        Task<Models.ConsultaClienteAfip.persona> GetClientAFIP(string Cuix);
    }
}

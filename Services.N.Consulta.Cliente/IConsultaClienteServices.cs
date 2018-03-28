using System.Threading.Tasks;
using Core.N.Models;
using Models.N.Consulta.Padron;

namespace Services.N.ConsultaCliente
{
    public interface IConsultaClienteServices
    {
        Task<string> GetCuix(string du, string sexo);
        Task<DatosPadron> GetDatosPadron(string cuix);
        Task<DatosPadron> GetDatosPadronAfip(string cuix);
    }
}

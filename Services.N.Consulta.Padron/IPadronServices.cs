using System.Threading.Tasks;
using Models.N.Consulta.Padron;

namespace Services.N.Consulta.Padron
{
    public interface IPadronServices
    {
        Task<DatosPadron> ConsultaPadron(string cuix);
    }
}
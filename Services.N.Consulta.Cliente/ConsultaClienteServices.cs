using System;
using System.Threading.Tasks;
using Models.N.Consulta.Padron;
using Microsoft.Extensions.Configuration;
using Services.N.ConsultaCliente;
using Core.N.Models;

namespace Services.N.Consulta.Cliente
{
    public class ConsultaClienteServices : IConsultaClienteServices
    {
        private IConfiguration _configuration;
        public ConsultaClienteServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<string> GetCuix(string du, string sexo)
        {
            var services = new Core.N.Rest.RestServices
            {
                Method = "GET",
                Url = $"{_configuration["GetCuix:Url"]}?du={du}&cuixType={sexo}",
                ContentType = "application/json",
                TimeoutMilliseconds = Convert.ToInt32(_configuration["GetCuix:TimeoutMilliseconds"])
            };

            return await services.ExecuteAsync<string>();
        }

        public async Task<MicroserviceModel<DatosPadron>> GetDatosPadron(string cuix)
        {
            var services = new Core.N.Rest.RestServices
            {
                Method = "GET",
                Url = $"{_configuration["GetDatosPadron:Url"]}?cuix={cuix}",
                ContentType = "application/json",
                TimeoutMilliseconds = Convert.ToInt32(_configuration["GetDatosPadron:TimeoutMilliseconds"])
            };

            return await services.ExecuteAsync<MicroserviceModel<DatosPadron>>();
        }
    }
}

using System;
using System.Threading.Tasks;
using Models.N.Consulta.Padron;
using Microsoft.Extensions.Configuration;
using Services.N.ConsultaCliente;
using System.Linq;

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

        public async Task<DatosPadron> GetDatosPadron(string cuix)
        {
            var services = new Core.N.Rest.RestServices
            {
                Method = "GET",
                Url = $"{_configuration["GetDatosPadron:Url"]}?cuix={cuix}",
                ContentType = "application/json",
                TimeoutMilliseconds = Convert.ToInt32(_configuration["GetDatosPadron:TimeoutMilliseconds"])
            };

            return await services.ExecuteAsync<DatosPadron>();
        }

        public async Task<DatosPadron> GetDatosPadronAfip(string cuix)
        {
            var services = new Core.N.Rest.RestServices
            {
                Method = "POST",
                Url = $"{_configuration["GetDatosPadronAfip:Url"]}",
                PayLoad = $"{cuix}",
                ContentType = "application/json",
                TimeoutMilliseconds = Convert.ToInt32(_configuration["GetDatosPadronAfip:TimeoutMilliseconds"])
            };

            var result = await services.ExecuteAsync<Models.N.Afip.ConsultaClienteAfip.persona>();

            return new DatosPadron
            {
                Apellido = result.apellido,
                Nombre = result.nombre,
                Cuix = result.idPersona.ToString(),
                TipoCuix = result.tipoClave.ToString(),
                FechaDeNacimiento = result.fechaNacimiento,
                Sexo = result.sexo,
                Domicilios = result.domicilio.Select(d => new Models.N.Location.Address
                {
                    City = d.localidad,
                    Country = "ARGENTINA",
                    PostalCode = d.codPostal,
                    Province = d.descripcionProvincia,
                    StreetAndNumber = d.direccion
                }).ToList()
            };

        }
    }
}

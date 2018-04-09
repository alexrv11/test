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
            var services = new Services.N.Core.Rest.RestServices
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
            var services = new Services.N.Core.Rest.RestServices
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
            var services = new Services.N.Core.Rest.RestServices
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
                    LocalityDescription = d.localidad,
                    PostalCode = d.codPostal,
                    Province = new Models.N.Location.Province {
                        Name = d.descripcionProvincia
                    },
                    Street = d.direccion,
                    AddressType = d.tipoDomicilio
                }).ToList()
            };
        }

        public async Task<bool> NormalizeAddress(Models.N.Location.Address address)
        {
            var services = new Services.N.Core.Rest.RestServices
            {
                Method = "POST",
                Url = $"{_configuration["GetLocation:Url"]}",
                ContentType = "application/json",
                TimeoutMilliseconds = Convert.ToInt32(_configuration["GetLocation:TimeoutMilliseconds"])
            };

            var response = await services.ExecuteAsync<Models.N.Location.GoogleMapsAddress,Models.N.Location.Address>(address);

            if (response.Status != "OK")
                throw new Exception(response.Status);

            var mapsAddress = response.Results.FirstOrDefault().AddressComponents;

            address.LocalityDescription = mapsAddress.FirstOrDefault(a => a.Types.Any(t => Models.N.Location.GoogleMapsAddress.LOCALITY_SUBLOCALITY.Contains(t))).ShortName;
            address.Country = new Models.N.Location.Country {
                Description = mapsAddress.FirstOrDefault(a => a.Types.Any(t => Models.N.Location.GoogleMapsAddress.COUNTRY.Contains(t))).LongName
            };
            address.Number = mapsAddress.FirstOrDefault(a => a.Types.Any(t => Models.N.Location.GoogleMapsAddress.STREET_NUMBER.Contains(t))).LongName;
            address.Street = mapsAddress.FirstOrDefault(a => a.Types.Any(t => Models.N.Location.GoogleMapsAddress.STREET.Contains(t))).LongName;
            address.PostalCode = mapsAddress.FirstOrDefault(a => a.Types.Any(t => Models.N.Location.GoogleMapsAddress.POSTAL_CODE.Contains(t))).LongName;
            address.Location = response.Results.FirstOrDefault().Geometry.Location;

            return true;
        }
    }
}

using System;
using System.Threading.Tasks;
using AutoMapper;
using Core.N.Utils.ObjectFactory;
using Microsoft.Extensions.Configuration;
using Services.N.Core.HttpClient;

namespace Services.N.Client
{
    public class ClientServices : IClientServices
    {
        private readonly IConfiguration _configuration;
        private readonly IObjectFactory _objectFactory;
        private readonly IMapper _mapper;

        public ClientServices(IConfiguration configuration, IObjectFactory objectFactory, IMapper mapper) {
            _configuration = configuration;
            _objectFactory = objectFactory;
            _mapper = mapper;
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

        public async Task<bool> AddClient(Models.N.Client.PadronData client)
        {
            try
            {
                var request = new BUS.CrearClienteRequest
                {
                    BGBAHeader = await _objectFactory.InstantiateFromJsonFile<BUS.BGBAHeader>(_configuration["AddClient:BGBAHeader"]),
                    Datos = new BUS.CrearClienteRequestDatos { Item = _mapper.Map<Models.N.Client.PadronData, BUS.CrearPersonaFisica>(client) }
                };

                var response = await HttpRequestFactory.Post(_configuration["AddClient:Url"], new SoapJsonContent(request, _configuration["AddClient:Operation"]));
                var soapResponse = response.SoapContentAsType<BUS.CrearClienteResponse>();

                if (soapResponse.BGBAResultadoOperacion.Severidad == BUS.severidad.ERROR)
                    throw new Exception($"{soapResponse.BGBAResultadoOperacion.Codigo} {soapResponse.BGBAResultadoOperacion.Descripcion}");

                return true;
            }
            catch (Exception e)
            {
                throw new Exception("Error adding the client", e);
            }
        }
    }
}

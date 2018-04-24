using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using AutoMapper;
using Core.N.Utils.ObjectFactory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        public async Task<string> AddClient(Models.N.Client.MinimumClientData client)
        {
            try
            {
                var request = new BUS.AdministracionCliente.CrearClienteDatosBasicosRequest
                {
                    BGBAHeader = await _objectFactory.InstantiateFromJsonFile<BUS.AdministracionCliente.BGBAHeader> (_configuration["AddClient:BGBAHeader"]),
                    Datos = _mapper.Map<Models.N.Client.MinimumClientData, BUS.AdministracionCliente.CrearClienteDatosBasicosRequestDatos>(client)
                };

                var response = await HttpRequestFactory.Post(_configuration["AddClient:Url"], new SoapJsonContent(request, _configuration["AddClient:Operation"]));
                dynamic soapResponse = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(JObject.Parse(response.ContentAsString()).SelectToken($"..{typeof(BUS.AdministracionCliente.CrearClienteDatosBasicosResponse).Name}")));

                if (soapResponse.BGBAResultadoOperacion.Severidad == BUS.AdministracionCliente.severidad.ERROR)
                    throw new Exception($"{soapResponse.BGBAResultadoOperacion.Codigo} {soapResponse.BGBAResultadoOperacion.Descripcion}");

                return soapResponse.Datos.IdPersona;
            }
            catch (Exception e)
            {
                throw new Exception("Error adding the client", e);
            }
        }

        public async Task<string> GetClientNV(Models.N.Client.ClientData client)
        {
            try
            {
                var request = new BUS.ConsultaCliente.BuscarClientePorDatosBasicosRequest
                {
                    BGBAHeader = await _objectFactory.InstantiateFromJsonFile<BUS.ConsultaCliente.BGBAHeader>(_configuration["GetClient:BGBAHeader"]),
                    Datos = _mapper.Map<Models.N.Client.ClientData, BUS.ConsultaCliente.Datos>(client)
                };

                var response = await HttpRequestFactory.Post(_configuration["GetClient:Url"], new SoapJsonContent(request, _configuration["GetClient:Operation"]));


                //try
                //{
                //    var xmlTypeResponse = response.SoapContentAsXmlType<BUS.ConsultaCliente.BuscarClientePorDatosBasicosResponse>();

                //}
                //catch (Exception e)
                //{
                //    System.Console.WriteLine(e.ToString());
                //}

                dynamic soapResponse = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(JObject.Parse(response.ContentAsString()).SelectToken("..BuscarClientePorDatosBasicosResponse")));
                if (soapResponse.BGBAResultadoOperacion.Severidad == BUS.ConsultaCliente.severidad.ERROR)
                    throw new Exception($"{soapResponse.BGBAResultadoOperacion.Codigo} {soapResponse.BGBAResultadoOperacion.Descripcion}");

                if (soapResponse.Datos.Personas == null)
                    return "";

                if ((soapResponse as dynamic).Datos.Personas.Persona.Type == JTokenType.Array)
                {
                    return soapResponse.Datos.Personas.Persona[0].PersonaFisica.DatosPersonaComunes.IdPersona;
                }

                return soapResponse.Datos.Personas.Persona.PersonaFisica.DatosPersonaComunes.IdPersona;
            }
            catch (Exception e)
            {
                throw new Exception("Error getting the client", e);
            }
        }

        public async Task<bool> UpdateAddress(string idHost, Models.N.Location.Address address)
        {
            try
            {
                var request = new BUS.AdministracionCliente.ModificarClienteRequest()
                {
                    BGBAHeader = await _objectFactory.InstantiateFromJsonFile<BUS.AdministracionCliente.BGBAHeader>(_configuration["UpdateClient:BGBAHeader"]),
                    Datos = new BUS.AdministracionCliente.ModificarClienteRequestDatos
                    {
                        Item = new BUS.AdministracionCliente.ModificarPersonaFisica
                        {
                            Domicilio = _mapper.Map<Models.N.Location.Address, BUS.AdministracionCliente.Domicilio1>(address),
                            IdPersona = Convert.ToUInt64(idHost),
                            ParametrizacionFisica = new BUS.AdministracionCliente.ParametrizacionFisica
                            {
                                ActualizarFisicaDatosDomicilio = true,
                                ActualizarFisicaDatosDomicilioSpecified = true
                            }
                        }
                    }
                };

                var response = await HttpRequestFactory.Post(_configuration["UpdateClient:Url"], new SoapJsonContent(request, _configuration["UpdateClient:Operation"]));

                dynamic soapResponse = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(JObject.Parse(response.ContentAsString()).SelectToken($"..{typeof(BUS.AdministracionCliente.ModificarClienteResponse).Name}")));

                if (soapResponse.BGBAResultadoOperacion.Severidad == BUS.ConsultaCliente.severidad.ERROR)
                    throw new Exception($"{soapResponse.BGBAResultadoOperacion.Codigo} {soapResponse.BGBAResultadoOperacion.Descripcion}");

                return true;
            }
            catch (Exception e)
            {
                throw new Exception("Error getting the client", e);
            }

        }
    }
}

using System;
using System.Threading.Tasks;
using AutoMapper;
using Core.N.Utils.ObjectFactory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Services.N.Core.HttpClient;
using Models.N.Core.Trace;
using System.Security.Cryptography.X509Certificates;
using Models.N.Location;

namespace Services.N.Client
{
    public class ClientServices : TraceServiceBase, IClientServices
    {
        private readonly IConfiguration _configuration;
        private readonly IObjectFactory _objectFactory;
        private readonly IMapper _mapper;
        private readonly X509Certificate2 _cert;

        public ClientServices(IConfiguration configuration, IObjectFactory objectFactory, IMapper mapper, X509Certificate2 cert)
        {
            _configuration = configuration;
            _objectFactory = objectFactory;
            _mapper = mapper;
            _cert = cert;
        }

        public async Task<string> GetCuix(string du, string sexo)
        {
            var service = new HttpRequestFactory();
            var isError = false;
            var url = $"{_configuration["GetCuix:Url"]}?du={du}&cuixType={sexo}";
            try
            {
                var response = await service.Get(url);
                return response.ContentAsType<string>();
            }
            catch (Exception e)
            {
                isError = true;
                throw new Exception("Error getting cuix.", e);
            }
            finally
            {
                this.Communicator_TraceHandler(this,
                    new TraceEventArgs
                    {
                        Description = "Get cuix.",
                        ElapsedTime = service.ElapsedTime,
                        ForceDebug = false,
                        IsError = isError,
                        Request = service.Request,
                        Response = service.Response,
                        URL = url
                    });
            }
        }

        public async Task<bool> AddClient(Models.N.Client.MinimumClientData client)
        {
            var service = new HttpRequestFactory();
            var isError = false;
            var url = _configuration["AddClient:Url"];

            try
            {
                var request = new BUS.AdministracionCliente.CrearClienteDatosBasicosRequest
                {
                    BGBAHeader = await _objectFactory.InstantiateFromJsonFile<BUS.AdministracionCliente.BGBAHeader>(_configuration["AddClient:BGBAHeader"]),
                    Datos = _mapper.Map<Models.N.Client.MinimumClientData, BUS.AdministracionCliente.CrearClienteDatosBasicosRequestDatos>(client)
                };

                var content = new SoapJsonContent(request, _configuration["AddClient:Operation"]);

                var response = await service.Post(url, content,_cert);
                dynamic soapResponse = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(JObject.Parse(response.ContentAsString()).SelectToken($"..{typeof(BUS.AdministracionCliente.CrearClienteDatosBasicosResponse).Name}")));

                if (soapResponse.BGBAResultadoOperacion.Severidad == BUS.AdministracionCliente.severidad.ERROR)
                    throw new Exception($"{soapResponse.BGBAResultadoOperacion.Codigo} {soapResponse.BGBAResultadoOperacion.Descripcion}");

                return true;
            }
            catch (Exception e)
            {
                throw new Exception("Error adding the client", e);
            }
            finally
            {
                this.Communicator_TraceHandler(this,
                    new TraceEventArgs
                    {
                        Description = "Add client in NV.",
                        ElapsedTime = service.ElapsedTime,
                        ForceDebug = false,
                        IsError = isError,
                        Request = service.Request,
                        Response = service.Response,
                        URL = url
                    });
            }
        }

        public async Task<string> GetClientNV(Models.N.Client.MinimumClientData client)
        {
            var service = new HttpRequestFactory();
            var isError = false;
            var url = _configuration["GetClient:Url"];

            BUS.ConsultaCliente.BuscarClientePorDatosBasicosRequest request = null;
            try
            {
                request = new BUS.ConsultaCliente.BuscarClientePorDatosBasicosRequest
                {
                    BGBAHeader = await _objectFactory.InstantiateFromJsonFile<BUS.ConsultaCliente.BGBAHeader>(_configuration["GetClient:BGBAHeader"]),
                    Datos = _mapper.Map<Models.N.Client.MinimumClientData, BUS.ConsultaCliente.Datos>(client)
                };
            }
            catch (Exception e)
            {
                throw new Exception("Error generating the request", e);
            }

            try
            {
                var response = await service.Post(url, new SoapJsonContent(request, _configuration["GetClient:Operation"]),_cert);

                dynamic soapResponse = JsonConvert.DeserializeObject<dynamic>(
                    JsonConvert.SerializeObject(
                        JObject.Parse(response.ContentAsString())
                        .SelectToken("..BuscarClientePorDatosBasicosResponse")));
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
                isError = true;
                throw new Exception("Error getting the client", e);
            }
            finally
            {
                this.Communicator_TraceHandler(this,
                    new TraceEventArgs
                    {
                        Description = "Get client from NV.",
                        ElapsedTime = service.ElapsedTime,
                        ForceDebug = false,
                        IsError = isError,
                        Request = service.Request,
                        Response = service.Response,
                        URL = url
                    });
            }
        }

        public async Task<bool> UpdateAddress(string idHost, Models.N.Location.Address address, string email)
        {
            var service = new HttpRequestFactory();
            var isError = false;
            var url = _configuration["UpdateClient:Url"];

            try
            {
                var request = new BUS.AdministracionCliente.ModificarClienteRequest()
                {
                    BGBAHeader = await _objectFactory.InstantiateFromJsonFile<BUS.AdministracionCliente.BGBAHeader>(_configuration["UpdateClient:BGBAHeader"]),
                    Datos = new BUS.AdministracionCliente.ModificarClienteRequestDatos(),
                };

                var item = new BUS.AdministracionCliente.ModificarPersonaFisica
                {
                    ParametrizacionFisica = new BUS.AdministracionCliente.ParametrizacionFisica(),
                    IdPersona = Convert.ToUInt64(idHost),
                };


                if (address != null)
                {
                    item.Domicilio = _mapper.Map<Models.N.Location.Address, BUS.AdministracionCliente.Domicilio1>(address);
                    item.ParametrizacionFisica.ActualizarFisicaDatosDomicilio = true;
                    item.ParametrizacionFisica.ActualizarFisicaDatosDomicilioSpecified = true;
                }

                if (!String.IsNullOrEmpty(email))
                {
                    item.Email = email;
                    item.ParametrizacionFisica.ActualizarFisicaDatosEmail = true;
                    item.ParametrizacionFisica.ActualizarFisicaDatosEmailSpecified = true;
                }


                request.Datos.Item = item;

                var response = await service.Post(url, new SoapJsonContent(request, _configuration["UpdateClient:Operation"]), _cert);

                dynamic soapResponse = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(JObject.Parse(response.ContentAsString()).SelectToken($"..{typeof(BUS.AdministracionCliente.ModificarClienteResponse).Name}")));

                if (soapResponse.BGBAResultadoOperacion.Severidad == BUS.ConsultaCliente.severidad.ERROR)
                    throw new Exception($"{soapResponse.BGBAResultadoOperacion.Codigo} {soapResponse.BGBAResultadoOperacion.Descripcion}");

                return true;
            }
            catch (Exception e)
            {
                isError = true;
                throw new Exception("Error getting the client", e);
            }
            finally
            {
                this.Communicator_TraceHandler(this,
                    new TraceEventArgs
                    {
                        Description = "Update client in NV.",
                        ElapsedTime = service.ElapsedTime,
                        ForceDebug = false,
                        IsError = isError,
                        Request = service.Request,
                        Response = service.Response,
                        URL = url
                    });
            }
        }

        public async Task<Address> NormalizeAddress(MapOptions mapOptions)
        {
            var service = new HttpRequestFactory();
            var isError = false;
            var url = _configuration["NormalizeAddress:Url"];

            try
            {
                var result = await service.Post(url,mapOptions);

                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                    return result.ContentAsType<Address>();

                if (result.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                throw new Exception(result.ContentAsString());

            }
            catch (Exception e)
            {
                isError = true;
                throw new Exception("Error normalizing the address", e);
            }
            finally
            {
                this.Communicator_TraceHandler(this,
                    new TraceEventArgs
                    {
                        Description = "Normalize the address.",
                        ElapsedTime = service.ElapsedTime,
                        ForceDebug = false,
                        IsError = isError,
                        Request = service.Request,
                        Response = service.Response,
                        URL = url
                    });
            }
        }
    }
}

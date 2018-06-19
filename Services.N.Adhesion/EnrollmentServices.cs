using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System;
using BGBA.Models.N.Core.Trace;
using BGBA.Models.N.Core.Utils.ObjectFactory;
using BGBA.Models.N.Client.Enrollment;
using System.Security.Cryptography.X509Certificates;
using BGBA.Services.N.Core.HttpClient;
using Models.N.Core.Exceptions;
using AutoMapper;
using System.Collections.Generic;
using BGBA.Services.N.Enrollment.SCS;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace BGBA.Services.N.Enrollment
{
    public class EnrollmentServices : TraceServiceBase, IEnrollmentServices
    {
        public string ERROR_PIN_SCS { get { return "LINK-SCS"; } }
        public string ERROR_ALREDY_REGISTERED { get { return "CLIEADHE"; } }

        public string NOT_INFORMED { get { return "USSINOOK"; } }
        public string CONSECUTIVE_CHARACTERS { get { return "CARACONS"; } }
        public string INCORRECT_CHARACTERS { get { return "CARAINCO"; } }

        private readonly IConfiguration _configuration;
        private readonly IObjectFactory _objectFactory;
        private readonly X509Certificate2 _certificate;
        private readonly IMapper _mapper;

        public EnrollmentServices(IConfiguration configuration, IObjectFactory objectFactory, X509Certificate2 cert, IMapper mapper)
        {
            _configuration = configuration;
            _objectFactory = objectFactory;
            _certificate = cert;
            _mapper = mapper;
        }

        public async Task<string> EnrollClientAsync(EnrollmentData data)
        {
            var service = new HttpRequestFactory();
            var url = _configuration["EnrollClient:Url"];
            var isError = false;

            var request = new Models.AccionesAdhesionBancaAutomatica.AdherirClienteFisicoProductoBancaAutomaticaRequest
            {
                BGBAHeader = await _objectFactory.InstantiateFromJsonFile<Models.AccionesAdhesionBancaAutomatica.BGBAHeader>(_configuration["EnrollClient:BGBAHeader"]),
                Datos = _mapper.Map<EnrollmentData, Models.AccionesAdhesionBancaAutomatica.AdherirClienteFisicoProductoBancaAutomaticaRequestDatos>(data)
            };

            try
            {
                var response = (await service.Post(url, new SoapJsonContent(request, _configuration["EnrollClient:Operation"]), _certificate)).SoapContentAsJsonType<Models.AccionesAdhesionBancaAutomatica.AdherirClienteFisicoProductoBancaAutomaticaResponse>();

                if (response.BGBAResultadoOperacion.Severidad == Models.AccionesAdhesionBancaAutomatica.severidad.OK)
                    return response.Datos.NumeroAdhesionClienteCanalesAlternativos.ToString();

                throw new TechnicalException(response.BGBAResultadoOperacion.Descripcion, response.BGBAResultadoOperacion.Codigo);
            }
            catch (Exception e)
            {
                isError = true;

                if (e.GetType() == typeof(TechnicalException))
                    throw;

                throw new InvalidOperationException("Error al realizar el servicio.", e);
            }
            finally
            {
                this.Communicator_TraceHandler(this, new TraceEventArgs() { ElapsedTime = service.ElapsedTime, URL = url, Request = service.Request, Response = service.Response, IsError = isError });
            }
        }

        public async Task<string> EnrollAlphanumericAsync(EnrollmentData data)
        {
            var service = new HttpRequestFactory();
            var url = _configuration["EnrollAlphanumeric:Url"];
            var isError = false;

            var request = new Models.AdministracionUsuarioHomebanking.CrearUsuarioRequest
            {
                BGBAHeader = await _objectFactory.InstantiateFromJsonFile<Models.AdministracionUsuarioHomebanking.BGBAHeader>(_configuration["EnrollAlphanumeric:BGBAHeader"]),
                Datos = _mapper.Map<EnrollmentData, Models.AdministracionUsuarioHomebanking.CrearUsuarioRequestDatos>(data)
            };

            try
            {
                var response = (await service.Post(url, new SoapJsonContent(request, _configuration["EnrollAlphanumeric:Operation"]), _certificate)).SoapContentAsJsonType<Models.AdministracionUsuarioHomebanking.CrearUsuarioResponse>();
                if (response.BGBAResultadoOperacion.Severidad == Models.AdministracionUsuarioHomebanking.severidad.OK)
                    return response.BGBAResultadoOperacion.Codigo;

                throw new TechnicalException(response.BGBAResultadoOperacion.Descripcion, response.BGBAResultadoOperacion.Codigo);
            }
            catch (Exception e)
            {
                isError = true;

                if (e.GetType() == typeof(TechnicalException))
                    throw;

                throw new InvalidOperationException("Error realizar el llamado al servicio.", e);
            }
            finally
            {
                this.Communicator_TraceHandler(this, new TraceEventArgs() { ElapsedTime = service.ElapsedTime, URL = url, Request = service.Request, Response = service.Response, IsError = isError });
            }
        }

        public async Task<List<EnrolledClient>> GetEnrolledClientsAsync(string documentNumber)
        {
            var service = new HttpRequestFactory();
            var url = _configuration["GetEnrolledClients:Url"];
            var isError = false;

            var request = new Models.ConsultaClienteCanalesAlternativos.BuscarClienteFisicoCanalesAlternativosPorIdentificacionRequest
            {
                BGBAHeader = await _objectFactory.InstantiateFromJsonFile<Models.ConsultaClienteCanalesAlternativos.BGBAHeader>(_configuration["GetEnrolledClients:BGBAHeader"]),
                Datos = _mapper.Map<string, Models.ConsultaClienteCanalesAlternativos.BuscarClienteFisicoCanalesAlternativosPorIdentificacionRequestDatos>(documentNumber)
            };

            try
            {
                var response = await service.Post(url, new SoapJsonContent(request, _configuration["GetEnrolledClients:Operation"]), _certificate);


                dynamic soapResponse = JsonConvert.DeserializeObject<dynamic>(
                   JsonConvert.SerializeObject(
                       JObject.Parse(response.ContentAsString())
                       .SelectToken("..BuscarClienteFisicoCanalesAlternativosPorIdentificacionResponse")));



                if (soapResponse.BGBAResultadoOperacion.Severidad == Models.ConsultaClienteCanalesAlternativos.severidad.ERROR)
                    throw new TechnicalException(soapResponse.BGBAResultadoOperacion.Descripcion, soapResponse.BGBAResultadoOperacion.Codigo);


                var list = new List<EnrolledClient>();


                if ((soapResponse as dynamic).Datos.DetallesClientes.DetalleCliente.Type == JTokenType.Array)
                {
                    var array = ((JArray)soapResponse.Datos.DetallesClientes.DetalleCliente);

                    foreach (dynamic item in array)
                    {
                        var client = new EnrolledClient();
                        client.HostId = item.IdPersona.ToString();
                        client.BirthDate = Convert.ToDateTime(item.FechaNacimiento);
                        client.DocumentNumber = item.Documento.Numero;
                        client.DocumentType = item.Documento.Tipo;
                        client.Sex = item.sexo;

                        if (item.AdhesionCanalesAlternativos != null)
                        {
                            client.State = (EnrollState)Enum.Parse(typeof(EnrollState), item.AdhesionCanalesAlternativos.Estado.ToString().Replace(" ", ""));
                            client.EnrollNumber = item.AdhesionCanalesAlternativos.Numero.ToString();
                            client.CentralSystemSecurityCodeId = item.AdhesionCanalesAlternativos.IdClaveSistemaCentralSeguridad.ToString();
                        }
                        else
                        {
                            client.State = EnrollState.NOADHERIDO;
                            client.EnrollNumber = "0";
                            client.CentralSystemSecurityCodeId = "0";
                        }

                        list.Add(client);
                    }
                }
                else
                {
                    var item = soapResponse.Datos.DetallesClientes.DetalleCliente;
                    var client = new EnrolledClient();
                    client.HostId = item.IdPersona.ToString();
                    client.BirthDate = Convert.ToDateTime(item.FechaNacimiento);
                    client.DocumentNumber = item.Documento.Numero;
                    client.DocumentType = item.Documento.Tipo;
                    client.Sex = item.sexo;

                    if (item.AdhesionCanalesAlternativos != null)
                    {
                        client.State = (EnrollState)Enum.Parse(typeof(EnrollState), item.AdhesionCanalesAlternativos.Estado.ToString().Replace(" ", ""));
                        client.EnrollNumber = item.AdhesionCanalesAlternativos.Numero.ToString();
                        client.CentralSystemSecurityCodeId = item.AdhesionCanalesAlternativos.IdClaveSistemaCentralSeguridad.ToString();
                    }
                    else
                    {
                        client.State = EnrollState.NOADHERIDO;
                        client.EnrollNumber = "0";
                        client.CentralSystemSecurityCodeId = "0";
                    }
                    list.Add(client);
                }

                return list;
            }
            catch (Exception e)
            {
                isError = true;

                if (e.GetType() == typeof(TechnicalException))
                    throw;

                throw new InvalidOperationException("Error realizar el llamado al servicio.", e);
            }
            finally
            {
                this.Communicator_TraceHandler(this, new TraceEventArgs() { ElapsedTime = service.ElapsedTime, URL = url, Request = service.Request, Response = service.Response, IsError = isError });
            }
        }

        private async Task<AuthenticationSeed> GetSeedAsync()
        {
            var service = new HttpRequestFactory();
            var url = _configuration["GenerateSeed:Url"];
            var isError = false;

            var request = new Models.AccionesSeguridadOmnichannel.GenerarSemillaRequest
            {
                BGBAHeader = await _objectFactory.InstantiateFromJsonFile<Models.AccionesSeguridadOmnichannel.BGBAHeader>(_configuration["GenerateSeed:BGBAHeader"]),
                Datos = new Models.AccionesSeguridadOmnichannel.GenerarSemillaRequestDatos
                {
                    idCliente = new Models.AccionesSeguridadOmnichannel.id { Value = _configuration["GenerateSeed:KEY"] }
                }
            };

            try
            {
                var response = (await service.Post(url, new SoapJsonContent(request, _configuration["GenerateSeed:Operation"]), _certificate)).SoapContentAsJsonType<Models.AccionesSeguridadOmnichannel.GenerarSemillaResponse>();

                if (response.BGBAResultadoOperacion.Severidad == Models.AccionesSeguridadOmnichannel.severidad.ERROR)
                    throw new TechnicalException(response.BGBAResultadoOperacion.Descripcion, response.BGBAResultadoOperacion.Codigo);

                return new AuthenticationSeed
                {
                    Id = response.Datos.IdSesion,
                    Key = response.Datos.Semilla
                };
            }
            catch (Exception e)
            {
                isError = true;
                throw new TechnicalException("Error realizar el llamado al servicio.", e);
            }
            finally
            {
                this.Communicator_TraceHandler(this, new TraceEventArgs() { ElapsedTime = service.ElapsedTime, URL = url, Request = service.Request, Response = service.Response, IsError = isError });
            }
        }

        public async Task<string> GetSCSCipherPasswordAsync(String userId, String password)
        {
            var semilla = await GetSeedAsync();

            User user = new User(userId, password);
            DESCipher des = new DESCipher("HGFEDCBA");

            DESCipher desCipher = new DESCipher(des.decryptString(_configuration["GenerateSeed:KEY"]));

            String decryptedKey = desCipher.decryptString(semilla.Key);
            var sKey = new AuthenticationSeed(decryptedKey, semilla.Id);

            SCSDESCipher dCipher = new SCSDESCipher(sKey.Id, sKey.Key);
            Scrambler scram = new Scrambler();
            String scramblerUser = scram.scrambler(user);
            String toReturn = dCipher.encrypt(scramblerUser);
            return toReturn;
        }
    }
}

using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography.X509Certificates;
using BGBA.Models.N.Location;
using BGBA.Models.N.Core.Trace;
using BGBA.Models.N.Core.Utils.ObjectFactory;
using BGBA.Services.N.Core.HttpClient;
using System.Linq;
using Microsoft.CSharp.RuntimeBinder;
using BGBA.Models.N.Client;
using System.Net.Mail;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;

namespace BGBA.Services.N.Client
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

        public async Task<bool> AddClientNV(Models.N.Client.ClientData client)
        {
            var service = new HttpRequestFactory();
            var isError = false;
            var url = _configuration["AddClient:Url"];

            try
            {
                var request = new BUS.AdministracionCliente.CrearClienteDatosBasicosRequest
                {
                    BGBAHeader = await _objectFactory.InstantiateFromJsonFile<BUS.AdministracionCliente.BGBAHeader>(_configuration["AddClient:BGBAHeader"]),
                    Datos = _mapper.Map<Models.N.Client.ClientData, BUS.AdministracionCliente.CrearClienteDatosBasicosRequestDatos>(client)
                };

                var content = new SoapJsonContent(request, _configuration["AddClient:Operation"]);

                var response = await service.Post(url, content, _cert);
                dynamic soapResponse = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(JObject.Parse(response.ContentAsString()).SelectToken($"..{typeof(BUS.AdministracionCliente.CrearClienteDatosBasicosResponse).Name}")));

                if (soapResponse.BGBAResultadoOperacion.Severidad == BUS.AdministracionCliente.severidad.ERROR)
                    throw new Exception($"{soapResponse.BGBAResultadoOperacion.Codigo} {soapResponse.BGBAResultadoOperacion.Descripcion}");

                return true;
            }
            catch (Exception e)
            {
                isError = true;
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

        public async Task<ClientData> GetClientAfip(string cuix)
        {
            var service = new HttpRequestFactory();
            var isError = false;
            var url = $"{_configuration["GetClientAfip:Url"]}/{cuix}";
            try
            {
                var response = await service.Get(url);
                return response.ContentAsType<ClientData>();
            }
            catch (Exception e)
            {
                isError = true;
                throw new Exception("Error getting client from afip.", e);
            }
            finally
            {
                this.Communicator_TraceHandler(this,
                    new TraceEventArgs
                    {
                        Description = "Get client afip.",
                        ElapsedTime = service.ElapsedTime,
                        ForceDebug = false,
                        IsError = isError,
                        Request = service.Request,
                        Response = service.Response,
                        URL = url
                    });
            }
        }

        public async Task<Address> GetAddressNV(string idHost)
        {
            var service = new HttpRequestFactory();
            var isError = false;
            var url = _configuration["GetClientByIdHost:Url"];

            BUS.ConsultaCliente.ObtenerClienteRequest request = null;
            try
            {
                request = new BUS.ConsultaCliente.ObtenerClienteRequest
                {
                    BGBAHeader = await _objectFactory.InstantiateFromJsonFile<BUS.ConsultaCliente.BGBAHeader>(_configuration["GetClientByIdHost:BGBAHeader"]),
                    Datos = new BUS.ConsultaCliente.ObtenerClienteRequestDatos
                    {
                        IdPersona = Convert.ToUInt64(idHost),
                        DatosSolicitados = new BUS.ConsultaCliente.ObtenerClienteRequestDatosDatosSolicitados
                        {
                            SolicitudDatosPersona = new BUS.ConsultaCliente.SolicitudDatosPersona
                            {
                                SolicitaDatosDomicilio = true,
                                SolicitaDatosEmail = true,
                                SolicitaDatosPosesionEmail = true,
                                SolicitaDatosTelefono = true
                            }
                        }
                    }
                };
            }
            catch (Exception e)
            {
                throw new Exception("Error generating the request", e);
            }

            try
            {
                var response = await service.Post(url, new SoapJsonContent(request, _configuration["GetClientByIdHost:Operation"]), _cert);

                dynamic soapResponse = JsonConvert.DeserializeObject<dynamic>(
                    JsonConvert.SerializeObject(
                        JObject.Parse(response.ContentAsString())
                        .SelectToken("..ObtenerClienteResponse")));

                if (soapResponse.BGBAResultadoOperacion.Severidad == BUS.ConsultaCliente.severidad.ERROR)
                    throw new Exception($"{soapResponse.BGBAResultadoOperacion.Codigo} {soapResponse.BGBAResultadoOperacion.Descripcion}");

                dynamic s = soapResponse.PersonaFisica;

                var result = new Models.N.Location.Address
                {
                    Default = true,
                    Country = new Models.N.Location.Country
                    {
                        Code = s.Domicilio.Direccion.CodigoPais,
                        Description = s.Domicilio.Direccion.DescripcionPais
                    },
                    FlatNumber = s.Domicilio.Direccion.Departamento,
                    Floor = s.Domicilio.Direccion.Piso,
                    Location = new Models.N.Location.Location
                    {
                        Latitude = s.Domicilio.Direccion.Latitud,
                        Longitude = s.Domicilio.Direccion.Longitud
                    },
                    LocalityDescription = s.Domicilio.Direccion.NombreLocalidad,
                    Number = s.Domicilio.Direccion.NumeroPuerta,
                    PostalCode = s.Domicilio.Direccion.CodigoPostal.ToString(),
                    Province = new Models.N.Location.Province
                    {
                        Code = s.Domicilio.Direccion.CodigoProvincia,
                        Name = s.Domicilio.Direccion.DescripcionProvincia
                    },
                    Street = s.Domicilio.Direccion.Calle,
                    AddressType = BGBA.Models.N.Afip.AfipProfiler.RealAddress
                };

                return result;
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

        public async Task<bool> GetClientNV(Models.N.Client.ClientData client)
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
                    Datos = _mapper.Map<Models.N.Client.ClientData, BUS.ConsultaCliente.Datos>(client)
                };
            }
            catch (Exception e)
            {
                throw new Exception("Error generating the request", e);
            }

            try
            {
                var response = await service.Post(url, new SoapJsonContent(request, _configuration["GetClient:Operation"]), _cert);

                dynamic soapResponse = JsonConvert.DeserializeObject<dynamic>(
                    JsonConvert.SerializeObject(
                        JObject.Parse(response.ContentAsString())
                        .SelectToken("..BuscarClientePorDatosBasicosResponse")));

                if (soapResponse.BGBAResultadoOperacion.Severidad == BUS.ConsultaCliente.severidad.ERROR)
                    throw new Exception($"{soapResponse.BGBAResultadoOperacion.Codigo} {soapResponse.BGBAResultadoOperacion.Descripcion}");

                try
                {
                    if (soapResponse.Datos.Personas == null)
                        return false;
                }
                catch (RuntimeBinderException)
                {
                    return false;
                }

                dynamic person;

                if ((soapResponse as dynamic).Datos.Personas.Persona.Type == JTokenType.Array)
                {
                    JArray array = ((JArray)soapResponse.Datos.Personas.Persona);

                    person = array.FirstOrDefault(p => client.Sex.ToUpper().StartsWith(p["PersonaFisica"]["Sexo"].ToString().ToUpper()) &&
                         (client.LastName.ToUpper().Contains(p["PersonaFisica"]["NombrePersona"]["Apellido"].ToString().ToUpper())
                          || p["PersonaFisica"]["NombrePersona"]["Apellido"].ToString().ToUpper().Contains(client.LastName.ToUpper())));

                    if (person == null)
                        return false;
                }
                else
                {
                    person = soapResponse.Datos.Personas.Persona;
                }

                client.HostId = person.PersonaFisica.DatosPersonaComunes.IdPersona.ToString();

                if (person.PersonaFisica.DatosPersonaComunes.Domicilio != null)
                {
                    var address = new Address
                    {
                        PostalCode = person.PersonaFisica.DatosPersonaComunes.Domicilio.CodigoPostal.ToString(),
                        LocalityDescription = person.PersonaFisica.DatosPersonaComunes.Domicilio.NombreLocalidad.ToString(),
                        Street = person.PersonaFisica.DatosPersonaComunes.Domicilio.Calle.ToString(),
                        Number = person.PersonaFisica.DatosPersonaComunes.Domicilio.NumeroPuerta.ToString()
                    };

                    client.Addresses.Add(address);
                }

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

        public async Task<bool> UpdateClientNV(string idHost, Models.N.Location.Address address, string email, Models.N.Client.Phone phone)
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

                if (phone != null)
                {
                    item.Telefonos = new BUS.AdministracionCliente.telefonoBasico1[]
                    {
                        new BUS.AdministracionCliente.telefonoBasico1
                        {
                            Basico = new BUS.AdministracionCliente.telefonoBasicoRespuestaNV1 {
                                celular = phone.IsCellphone.ToString().ToLower(),
                                CodigoArea = phone.AreaNumber,
                                Numero = phone.Number
                            }
                        }
                    };
                    item.ParametrizacionFisica.ActualizarFisicaDatosTelefono = true;
                    item.ParametrizacionFisica.ActualizarFisicaDatosTelefonoSpecified = true;
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
                var result = await service.Post(url, mapOptions);

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

        public async Task<bool> SendEmail(Dictionary<string,string> data, string email, string attachmentNameWithExtension)
        {
            var body = (await System.IO.File.ReadAllTextAsync(_configuration["WelcomeEmail:Body"]));
            var attachment = new Attachment(new MemoryStream(await System.IO.File.ReadAllBytesAsync(_configuration["WelcomeEmail:AttachmentPath"])), attachmentNameWithExtension, MediaTypeNames.Application.Pdf);


            if (data != null)
                foreach (var item in data)
                    body = body.Replace(item.Key, item.Value);


            var msg = CrearMensaje(_configuration["WelcomeEmail:Sender"], email, _configuration["WelcomeEmail:Subject"], body, new[] { attachment }, true);

            EnviarMail(msg);

            return true;
        }

        private MailMessage CrearMensaje(string origen, string destino, string asunto, string body, ICollection<Attachment> attachments, bool isBodyHtml = false)
        {
            MailMessage message = new MailMessage();

            message.From = new MailAddress(origen);
            message.To.Add(new MailAddress(destino));
            message.Subject = asunto;
            message.Body = body;
            message.IsBodyHtml = isBodyHtml;

            if (attachments != null)
            {
                foreach (var item in attachments)
                {
                    message.Attachments.Add(item);
                }
            }

            return message;
        }

        private void EnviarMail(MailMessage mensaje)
        {
            SmtpClient smtpClient;
            string host;
            bool enabledSSl;

            host = _configuration["Smtp:Url"];
            enabledSSl = bool.Parse(_configuration["Smtp:EnabledSSL"]);
            smtpClient = new SmtpClient(host)
            {
                EnableSsl = enabledSSl
            };

            smtpClient.Send(mensaje);
        }
    }
}

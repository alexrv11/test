using System;
using System.Threading.Tasks;
using BGBA.Models.N.Core.Utils.ObjectFactory;
using BGBA.Services.N.Autenticacion.SCS;
using Microsoft.Extensions.Configuration;
using Services.N.Core.Rest;
using BGBA.Models.N.Core.Trace;
using System.Security.Cryptography.X509Certificates;

namespace BGBA.Services.N.Autenticacion
{
    public class AutenticacionServices : TraceServiceBase
    {

        private IConfiguration _configuration;
        private IObjectFactory _objectFactory;
        private X509Certificate2 _certificate;

        public AutenticacionServices(IConfiguration configuration, IObjectFactory objectFactory, X509Certificate2 cert)
        {
            _configuration = configuration;
            _objectFactory = objectFactory;
            _certificate = cert;
        }
        private async Task<SemillaAutenticacion> GetSemilla()
        {
            var service = new RestServices();
            Models.SoapCallRequest.Request request;
            Models.SoapCallResponse.Response response;
            try
            {
                service.ContentType = "application/json";
                service.TimeoutMilliseconds = Convert.ToInt32(_configuration["GenerarSemilla:TimeoutMilliseconds"]);
                service.Method = "POST";
                service.Url = _configuration["GenerarSemilla:Url"];
                service.Certificate = _certificate;
            }
            catch (Exception e)
            {
                throw new Exception("Error al instanciar el servicio", e);
            }

            try
            {
                //request = new Models.AccionesSeguridadOmnichannel.GenerarSemillaRequest
                //{
                //    BGBAHeader = await _objectFactory.InstantiateFromFile<Models.AccionesSeguridadOmnichannel.BGBAHeader>(_configuration["BGBAHeader:Path"]),
                //    Datos = new Models.AccionesSeguridadOmnichannel.GenerarSemillaRequestDatos
                //    {
                //        idCliente = new Models.AccionesSeguridadOmnichannel.id { Value = _configuration["GenerarSemilla:KEY"] }
                //    }
                //};

                request = new Models.SoapCallRequest.Request
                {
                    Envelope = new Models.SoapCallRequest.Envelope
                    {
                        Body = new Models.SoapCallRequest.Body
                        {
                            GenerarSemilla = new Models.SoapCallRequest.GenerarSemilla
                            {
                                GenerarSemillaRequest = new Models.AccionesSeguridadOmnichannel.GenerarSemillaRequest
                                {
                                    BGBAHeader = await _objectFactory.InstantiateFromJsonFile<Models.AccionesSeguridadOmnichannel.BGBAHeader>(_configuration["BGBAHeaderAlias:Path"]),
                                    Datos = new Models.AccionesSeguridadOmnichannel.GenerarSemillaRequestDatos
                                    {
                                        idCliente = new Models.AccionesSeguridadOmnichannel.id { Value = _configuration["GenerarSemilla:KEY"] }
                                    }
                                }
                            }
                        }
                    }
                };
            }
            catch (Exception e)
            {
                throw new Exception("Error al generar el request.", e);
            }

            try
            {
                response = await service.ExecuteAsync<Models.SoapCallResponse.Response, Models.SoapCallRequest.Request>(request);
                this.Communicator_TraceHandler(this, new TraceEventArgs() { ElapsedTime = service.ElapsedTime, URL = service.Url, Request = service.Request, Response = service.Response });


                if (response.Envelope.Body.GenerarSemillaResult.GenerarSemillaResponse.BGBAResultadoOperacion.Severidad == Models.AccionesSeguridadOmnichannel.severidad.ERROR)
                    throw new Exception($"Error en la respuesta del servicio: Codigo={response.Envelope.Body.GenerarSemillaResult.GenerarSemillaResponse.BGBAResultadoOperacion.Codigo}, Descripcion={response.Envelope.Body.GenerarSemillaResult.GenerarSemillaResponse.BGBAResultadoOperacion.Descripcion}");

                return new SemillaAutenticacion
                {
                    Id = response.Envelope.Body.GenerarSemillaResult.GenerarSemillaResponse.Datos.IdSesion,
                    Key = response.Envelope.Body.GenerarSemillaResult.GenerarSemillaResponse.Datos.Semilla
                };
            }
            catch (Exception e)
            {
                this.Communicator_TraceHandler(this, new TraceEventArgs() { ElapsedTime = service.ElapsedTime, URL = service.Url, Request = service.Request, Response = service.Response, IsError = true });
                throw new Exception("Error realizar el llamado al servicio.", e);
            }
        }

        public async Task<string> GetSCSCipherPassword(String userId, String password)
        {
            var semilla = await GetSemilla();

            try
            {
                User user = new User(userId, password);
                DESCipher des = new DESCipher("HGFEDCBA");

                DESCipher desCipher = new DESCipher(des.decryptString(_configuration["GenerarSemilla:KEY"]));

                String decryptedKey = desCipher.decryptString(semilla.Key);
                var sKey = new SemillaAutenticacion(decryptedKey, semilla.Id);

                SCSDESCipher dCipher = new SCSDESCipher(sKey.Id, sKey.Key);
                Scrambler scram = new Scrambler();
                String scramblerUser = scram.scrambler(user);
                String toReturn = dCipher.encrypt(scramblerUser);
                return toReturn;
            }
            catch (Exception e)
            {
                throw new Exception("Error al encriptar con SCS", e);
            }
        }

    }
}

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Services.N.Core.HttpClient;
using Core.N.Utils.ObjectFactory;
using AutoMapper;
using Models.N.Client;
using Models.N.Afip;
using Newtonsoft.Json;
using Microsoft.CSharp.RuntimeBinder;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using Models.N.Core.Trace;

namespace Services.N.Afip
{
    public class AfipServices : Models.N.Core.Trace.TraceServiceBase, IAfipServices
    {
        private readonly IConfiguration _configuration;
        private readonly IObjectFactory _objectFactory;
        private readonly IMapper _mapper;
        private readonly X509Certificate2 _cert;
        private static Credentials _credentials;
        private static DateTime _endOfValidCredentials;

        public AfipServices(IConfiguration configuration, IObjectFactory objectFactory, IMapper mapper, X509Certificate2 cert)
        {
            _configuration = configuration;
            _objectFactory = objectFactory;
            _mapper = mapper;
            _cert = cert;
        }

        public async Task<Credentials> GetCredentials()
        {

            var service = new HttpRequestFactory();
            var isError = false;
            var url = _configuration["GetCredentials:Url"];
            if (IsValidCredentials())
                return _credentials;

            try
            {
                var now = DateTime.Now;

                var request = new AutenticarYAutorizarConsumoWebserviceRequest
                {
                    BGBAHeader = await _objectFactory.InstantiateFromJsonFile<BGBAHeader>(_configuration["GetCredentials:BGBAHeader"]),
                    Datos = new AutenticarYAutorizarConsumoWebserviceRequestDatos
                    {
                        IdRequerimiento = new Random(10000).Next(),
                        HoraDesde = now.ToString("yyyy-MM-ddTHH:mm:sss.fff"),
                        HoraHasta = now.AddMilliseconds(Convert.ToInt32(_configuration["GetCredentials:MillisecondsForValidToken"])).ToString("yyyy-MM-ddTHH:mm:sss.fff"),
                        ServicioAConsumir = _configuration["GetCredentials:ServicioAConsumir"]
                    }
                };

                var response = (await service.Post(url, new SoapJsonContent(request, _configuration["GetCredentials:Operation"]), _cert))
                    .SoapContentAsJsonType<AutenticarYAutorizarConsumoWebserviceResponse>();


                if (response.BGBAResultadoOperacion.Severidad == severidad.ERROR)
                    throw new Exception($"{response.BGBAResultadoOperacion.Codigo},{response.BGBAResultadoOperacion.Descripcion}");

                _endOfValidCredentials = now.AddMilliseconds(Convert.ToInt32(_configuration["GetCredentials:MillisecondsForValidToken"]));
                _credentials = _mapper.Map<AutenticarYAutorizarConsumoWebserviceResponseDatosCredenciales, Credentials>(response.Datos.Credenciales);

                return _credentials;
            }
            catch (Exception e)
            {
                isError = true;
                throw new Exception("Error getting credentials", e);
            }
            finally
            {
                this.Communicator_TraceHandler(this,
                    new TraceEventArgs
                    {
                        Description = "Get credentials afip.",
                        ElapsedTime = service.ElapsedTime,
                        ForceDebug = false,
                        IsError = isError,
                        Request = service.Request,
                        Response = service.Response,
                        URL = url
                    });
            }
        }

        public async Task<ClientData> GetClient(string cuix)
        {
            var service = new HttpRequestFactory();
            var isError = false;
            var url = _configuration["GetClientAfip:Url"];
            try
            {
                var credentials = await GetCredentials();
                var request = new getPersona
                {
                    cuitRepresentada = Convert.ToInt64(_configuration["GetClientAfip:BankCuit"]),
                    idPersona = Convert.ToInt64(cuix),
                    sign = credentials.Sign,
                    token = credentials.Token
                };

                var response = await service.Post(url, new SoapJsonContent(request, _configuration["GetClientAfip:Operation"]), _cert);
                dynamic dynamicResponse = JsonConvert.DeserializeObject<dynamic>(response.ContentAsString());

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    throw new Exception(response.ContentAsString());

                try
                {
                    if (dynamicResponse.Envelope.Body.Fault != null)
                        return null;
                }
                catch (RuntimeBinderException)
                {
                }

                return _mapper.Map<persona, ClientData>(response.SoapContentAsJsonType<getPersonaResponse>().personaReturn.persona);
            }
            catch (Exception e)
            {
                isError = true;
                throw new Exception("Error getting client", e);
            }
            finally
            {
                this.Communicator_TraceHandler(this,
                    new TraceEventArgs
                    {
                        Description = "Get credentials afip.",
                        ElapsedTime = service.ElapsedTime,
                        ForceDebug = false,
                        IsError = isError,
                        Request = service.Request,
                        Response = service.Response,
                        URL = url
                    });
            }
        }

        public bool IsValidCredentials()
        {
            return (_endOfValidCredentials - DateTime.Now).TotalMilliseconds > 0 && _credentials != null;
        }
    }
}

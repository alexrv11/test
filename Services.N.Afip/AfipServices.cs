using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Models.N.Afip.AutenticacionAfip;
using Models.N.Afip.ConsultaClienteAfip;
using Services.N.Core.HttpClient;
using Newtonsoft.Json;
using Core.N.Utils.ObjectFactory;

namespace Services.N.Afip
{
    public class AfipServices : IAFIPServices
    {
        private readonly IConfiguration _configuration;
        private readonly IObjectFactory _objectFactory;
        private AutenticarYAutorizarConsumoWebserviceResponseDatosCredenciales _credentials;
        private DateTime _endOfValidCredentials;

        public AfipServices(IConfiguration configuration, IObjectFactory objectFactory)
        {
            _configuration = configuration;
            _objectFactory = objectFactory;
        }

        public async Task<AutenticarYAutorizarConsumoWebserviceResponseDatosCredenciales> GetCredentials()
        {
            if (IsValidCredentials())
                return _credentials;

            try
            {
                var now = DateTime.Now;

                var request = new AutenticarYAutorizarConsumoWebserviceRequest
                {
                    BGBAHeader = await _objectFactory.InstantiateFromJsonFile<BGBAHeader>(_configuration["Autenticar:BGBAHeader"]),
                    Datos = new AutenticarYAutorizarConsumoWebserviceRequestDatos
                    {
                        IdRequerimiento = new Random(10000).Next(),
                        HoraDesde = now.ToString("yyyy-MM-ddTHH:mm:sss.fff"),
                        HoraHasta = now.AddMilliseconds(Convert.ToInt32(_configuration["Autenticar:MillisecondsForValidToken"])).ToString("yyyy-MM-ddTHH:mm:sss.fff"),
                        ServicioAConsumir = _configuration["Autenticar:ServicioAConsumir"]
                    }
                };

                var response = await HttpRequestFactory.Post(_configuration["Autenticar:Url"], new SoapJsonContent(request, "AutenticarYAutorizarConsumoWebservice"));
                
                dynamic result = JsonConvert.DeserializeObject<dynamic>(response.ContentAsString());
                
                var webResponse = result.Envelope.Body.AutenticarYAutorizarConsumoWebserviceResult.AutenticarYAutorizarConsumoWebserviceResponse;

                var w = response.ContentAsTypeFromSoap<AutenticarYAutorizarConsumoWebserviceResponse>("AutenticarYAutorizarConsumoWebserviceResponse", "http://ws.bancogalicia.com.ar/webservices/accionesautenticacionafip/autenticaryautorizarconsumowebserviceresponse/1_0_0");
                if (webResponse.BGBAResultadoOperacion.Severidad == severidad.ERROR)
                    throw new Exception($"Error en la respuesta del servicio: Codigo={webResponse.BGBAResultadoOperacion.Codigo}, Descripcion={webResponse.BGBAResultadoOperacion.Descripcion}");

                _endOfValidCredentials = now.AddMilliseconds(Convert.ToInt32(_configuration["Autenticar:MillisecondsForValidToken"]));
                _credentials = new AutenticarYAutorizarConsumoWebserviceResponseDatosCredenciales
                {
                    Firma = webResponse.Datos.Credenciales.Firma,
                    Token = webResponse.Datos.Credenciales.Token
                };

                return _credentials;
            }
            catch (Exception e)
            {
                throw new Exception("Error al generar el request", e);
            }
        }

        public async Task<persona> GetClientAFIP(string Cuix)
        {
            var service = new Services.N.Core.Rest.RestServices();
            Models.SoapCallConsultarClienteAfipResponse.Response response = null;


            try
            {
                service.ContentType = "application/json";
                service.TimeoutMilliseconds = Convert.ToInt32(_configuration["ConsultaCliente:TimeoutMilliseconds"]);
                service.Method = "POST";
                service.Url = _configuration["ConsultaCliente:Url"];
            }
            catch (Exception e)
            {
                throw new Exception("Error al instanciar el servicio", e);
            }

            try
            {
                var credentials = await GetCredentials();

                var obj = JObject.Parse(await File.ReadAllTextAsync(_configuration["ConsultaCliente:Request"]));
                obj["Envelope"]["Body"]["getPersona"]["token"] = credentials.Token;
                obj["Envelope"]["Body"]["getPersona"]["sign"] = credentials.Firma;
                obj["Envelope"]["Body"]["getPersona"]["idPersona"] = Cuix;
                service.PayLoad = obj.ToString();
            }
            catch (Exception e)
            {
                throw new Exception("Error al generar el request", e);
            }

            try
            {
                response = await service.ExecuteAsync<Models.SoapCallConsultarClienteAfipResponse.Response>();

                return response.Envelope.Body.getPersonaResponse.personaReturn.persona;
            }
            catch (Exception e)
            {
                throw new Exception("Error al realizar el servicio.", e);
            }
        }

        public bool IsValidCredentials()
        {
            return (_endOfValidCredentials - DateTime.Now).TotalMilliseconds > 0 && _credentials != null;
        }
    }
}

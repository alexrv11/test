using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Models.N.Afip.AutenticacionAfip;
using Models.N.Afip.ConsultaClienteAfip;

namespace Services.N.Afip
{
    public class AfipServices : IAFIPServices
    {
        private readonly IConfiguration _configuration;
        private AutenticarYAutorizarConsumoWebserviceResponseDatosCredenciales _credentials;
        private DateTime _endOfValidCredentials;

        public AfipServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<AutenticarYAutorizarConsumoWebserviceResponseDatosCredenciales> GetCredentials()
        {


            if (IsValidCredentials())
                return _credentials;

            var service = new Core.N.Rest.RestServices();
            Models.SoapCallAutenticarAfipResponse.Response response = null;

            try
            {
                service.ContentType = "application/json";
                service.TimeoutMilliseconds = Convert.ToInt32(_configuration["Autenticar:TimeoutMilliseconds"]);
                service.Method = "POST";
                service.Url = _configuration["Autenticar:Url"];
            }
            catch (Exception e)
            {
                throw new Exception("Error al instanciar el servicio", e);
            }

            try
            {
                var now = DateTime.Now;

                _endOfValidCredentials = now.AddMilliseconds(Convert.ToInt32(_configuration["Autenticar:MillisecondsForValidToken"]));

                var obj = JObject.Parse(await File.ReadAllTextAsync(_configuration["Autenticar:Request"]));
                obj["Envelope"]["Body"]["AutenticarYAutorizarConsumoWebservice"]["AutenticarYAutorizarConsumoWebserviceRequest"]["Datos"]["IdRequerimiento"] = new Random(10000).Next().ToString();
                obj["Envelope"]["Body"]["AutenticarYAutorizarConsumoWebservice"]["AutenticarYAutorizarConsumoWebserviceRequest"]["Datos"]["HoraDesde"] = now.ToString("yyyy-MM-ddTHH:mm:sss.fff");
                obj["Envelope"]["Body"]["AutenticarYAutorizarConsumoWebservice"]["AutenticarYAutorizarConsumoWebserviceRequest"]["Datos"]["HoraHasta"] = _endOfValidCredentials.ToString("yyyy-MM-ddTHH:mm:sss.fff");
                service.PayLoad = obj.ToString();
            }
            catch (Exception e)
            {
                throw new Exception("Error al generar el request", e);
            }

            try
            {
                response = await service.ExecuteAsync<Models.SoapCallAutenticarAfipResponse.Response>();
                
                if (response.Envelope.Body.AutenticarYAutorizarConsumoWebserviceResult.AutenticarYAutorizarConsumoWebserviceResponse.BGBAResultadoOperacion.Severidad == severidad.ERROR)
                    throw new Exception($"Error en la respuesta del servicio: Codigo={response.Envelope.Body.AutenticarYAutorizarConsumoWebserviceResult.AutenticarYAutorizarConsumoWebserviceResponse.BGBAResultadoOperacion.Codigo}, " +
                        $"Descripcion={response.Envelope.Body.AutenticarYAutorizarConsumoWebserviceResult.AutenticarYAutorizarConsumoWebserviceResponse.BGBAResultadoOperacion.Descripcion}");


                _credentials = response.Envelope.Body.AutenticarYAutorizarConsumoWebserviceResult.AutenticarYAutorizarConsumoWebserviceResponse.Datos.Credenciales;

                return _credentials;
            }
            catch (Exception e)
            {
                throw new Exception("Error al realizar el servicio.", e);
            }


        }

        public async Task<persona> GetClientAFIP(string Cuix)
        {
            var service = new Core.N.Rest.RestServices();
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

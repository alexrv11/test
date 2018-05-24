using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using Newtonsoft.Json.Linq;
using BGBA.Models.N.Core.Trace;
using BGBA.Models.N.Core.Utils.ObjectFactory;
using BGBA.Models.N.Adhesion;
using System.Security.Cryptography.X509Certificates;
using BGBA.Services.N.Core.HttpClient;
using System.Dynamic;
using System.Linq;
using Newtonsoft.Json;
using Models.N.Core.Exceptions;

namespace BGBA.Services.N.Adhesion
{
    public class AdhesionServices : TraceServiceBase
    {
        public const string ERROR_PIN_SCS = "LINK-SCS";
        public const string ERROR_ALREDY_REGISTERED = "CLIEADHE";

        public const string NOT_INFORMED = "USSINOOK";
        public const string CONSECUTIVE_CHARACTERS = "CARACONS";
        public const string INCORRECT_CHARACTERS = "CARAINCO";

        private readonly IConfiguration _configuration;
        private readonly IObjectFactory _objectFactory;
        private readonly X509Certificate2 _certificate;

        public AdhesionServices(IConfiguration configuration, IObjectFactory objectFactory, X509Certificate2 cert)
        {
            _configuration = configuration;
            _objectFactory = objectFactory;
            _certificate = cert;
        }

        public async Task<string> AdherirUsuario(DatosAdhesion datos)
        {
            var service = new HttpRequestFactory();
            var url = _configuration["AdhesionPin:Url"];
            bool isError = false;
            Models.SoapCallAdhesionBancaAutomaticaResponse.Response response = null;

            var obj = JObject.Parse(await File.ReadAllTextAsync(_configuration["AdhesionPin:Request"]));
            obj["Envelope"]["Body"]["AdherirClienteFisicoProductoBancaAutomatica"]["AdherirClienteFisicoProductoBancaAutomaticaRequest"]["Datos"]["Claves"]["SistemaCentralSeguridad"] = datos.PinEncriptado;
            obj["Envelope"]["Body"]["AdherirClienteFisicoProductoBancaAutomatica"]["AdherirClienteFisicoProductoBancaAutomaticaRequest"]["Datos"]["AdhesionCliente"]["IdPersona"] = datos.IdHost;
            obj["Envelope"]["Body"]["AdherirClienteFisicoProductoBancaAutomatica"]["AdherirClienteFisicoProductoBancaAutomaticaRequest"]["Datos"]["AdhesionCliente"]["Documentos"]["Documento"]["Tipo"]["$"] = datos.TipoDocumento;
            obj["Envelope"]["Body"]["AdherirClienteFisicoProductoBancaAutomatica"]["AdherirClienteFisicoProductoBancaAutomaticaRequest"]["Datos"]["AdhesionCliente"]["Documentos"]["Documento"]["Numero"]["$"] = datos.NroDocumento;

            if (datos.ProductosAdheribles != null && datos.ProductosAdheribles.Count > 0)
            {
                dynamic prod = new ExpandoObject();

                prod.AdhesionProducto = new ExpandoObject();
                prod.AdhesionProducto.Productos = new ExpandoObject();
                prod.AdhesionProducto.Productos.Producto = JArray.FromObject(datos.ProductosAdheribles);

                obj["Envelope"]["Body"]["AdherirClienteFisicoProductoBancaAutomatica"]["AdherirClienteFisicoProductoBancaAutomaticaRequest"]["Datos"]["AdhesionProducto"] = JObject.FromObject(prod.AdhesionProducto);
            }

            try
            {

                response = (await service.Post(url, obj, _certificate)).ContentAsType<Models.SoapCallAdhesionBancaAutomaticaResponse.Response>();

                if (response.Envelope.Body.AdherirClienteFisicoProductoBancaAutomaticaResult.AdherirClienteFisicoProductoBancaAutomaticaResponse.BGBAResultadoOperacion.Severidad == Models.AccionesAdhesionBancaAutomatica.severidad.OK)
                    return response.Envelope.Body.AdherirClienteFisicoProductoBancaAutomaticaResult.AdherirClienteFisicoProductoBancaAutomaticaResponse.Datos.NumeroAdhesionClienteCanalesAlternativos.ToString();

                throw new TechnicalException(response.Envelope.Body.AdherirClienteFisicoProductoBancaAutomaticaResult.AdherirClienteFisicoProductoBancaAutomaticaResponse.BGBAResultadoOperacion.Descripcion, response.Envelope.Body.AdherirClienteFisicoProductoBancaAutomaticaResult.AdherirClienteFisicoProductoBancaAutomaticaResponse.BGBAResultadoOperacion.Codigo);
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

        public async Task<string> AltaAlfanumerico(DatosAdhesion datos)
        {
            var service = new HttpRequestFactory();
            Models.SoapCallAdministracionUsuarioHomebankingResponse.Response response = null;
            var url = _configuration["AdhesionAlfanumerico:Url"];
            var isError = false;

            var obj = JObject.Parse(await File.ReadAllTextAsync(_configuration["AdhesionAlfanumerico:Request"]));
            obj["Envelope"]["Body"]["CrearUsuario"]["CrearUsuarioRequest"]["Datos"]["IdUsuario"] = datos.UsuarioAlfanumerico;
            obj["Envelope"]["Body"]["CrearUsuario"]["CrearUsuarioRequest"]["Datos"]["NumeroAdhesionClienteCanalesAlternativos"] = datos.IdAdhesion;

            try
            {
                response = (await service.Post(url, obj, _certificate)).ContentAsType<Models.SoapCallAdministracionUsuarioHomebankingResponse.Response>();
                if (response.Envelope.Body.CrearUsuarioResult.CrearUsuarioResponse.BGBAResultadoOperacion.Severidad == Models.AdministracionUsuarioHomebanking.severidad.OK)
                    return response.Envelope.Body.CrearUsuarioResult.CrearUsuarioResponse.BGBAResultadoOperacion.Codigo;

                throw new TechnicalException(response.Envelope.Body.CrearUsuarioResult.CrearUsuarioResponse.BGBAResultadoOperacion.Descripcion, response.Envelope.Body.CrearUsuarioResult.CrearUsuarioResponse.BGBAResultadoOperacion.Codigo);
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
    }
}

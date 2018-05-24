using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BGBA.Models.N.Adhesion;
using BGBA.Models.N.Core.Utils.ObjectFactory;
using BGBA.Services.N.Autenticacion;
using BGBA.Services.N.Adhesion;
using System.Security.Cryptography.X509Certificates;
using Models.N.Core.Exceptions;

namespace BGBA.MS.N.Adhesion.Controllers
{
    [Route("api/adherir")]
    public class AdhesionController : BGBA.Models.N.Core.Microservices.MicroserviceController
    {
        public const string ErrorPrefix = "MS_Adherir";

        private readonly IConfiguration _configuration;
        private readonly IObjectFactory _objectFactory;
        private readonly ILogger<AdhesionController> _logger;
        private readonly X509Certificate2 _certificate;

        public AdhesionController(IConfiguration configuration, IObjectFactory objectFactory, ILogger<AdhesionController> logger, X509Certificate2 cert)
            : base(logger, configuration)
        {
            _configuration = configuration;
            _objectFactory = objectFactory;
            _logger = logger;
            _certificate = cert;
        }

        [HttpPost("cliente")]
        public async Task<IActionResult> Cliente([FromBody]DatosAdhesion datos)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var trace = new BGBA.Models.N.Core.Trace.TraceEventHandler(delegate (object sender, BGBA.Models.N.Core.Trace.TraceEventArgs e)
                {
                    base.Communicator_TraceHandler(sender, e);
                });

                var serviceAutenticacion = new AutenticacionServices(_configuration, _objectFactory, _certificate);
                serviceAutenticacion.TraceHandler += trace;
                datos.PinEncriptado = await serviceAutenticacion.GetSCSCipherPassword(datos.IdHost, datos.Pin);

                _logger.LogTrace("Encripto pin.");

                var result = new Models.AdhesionResult();
                var serviceAdhesion = new AdhesionServices(_configuration, _objectFactory, _certificate);

                if (string.IsNullOrEmpty(datos.IdAdhesion))
                {
                    try
                    {

                        serviceAdhesion.TraceHandler += trace;
                        datos.IdAdhesion = result.IdAdhesion = await serviceAdhesion.AdherirUsuario(datos);
                        result.AdhesionState = Models.AdhesionState.OK;
                        _logger.LogTrace("Adhirio usuario.");
                    }
                    catch (TechnicalException e)
                    {
                        switch (e.TechnicalCode)
                        {
                            case AdhesionServices.ERROR_ALREDY_REGISTERED:
                                result.AdhesionState = Models.AdhesionState.ALREDY_REGISTERED;
                                break;
                            case AdhesionServices.ERROR_PIN_SCS:
                                result.AdhesionState = Models.AdhesionState.INVALID_PIN;
                                break;
                            default:
                                throw;
                        }
                        return new ObjectResult(result);
                    }
                }
                else
                {
                    result.IdAdhesion = datos.IdAdhesion;
                }

                try
                {
                    await serviceAdhesion.AltaAlfanumerico(datos);
                    result.AlphanumericState = Models.AlphanumericState.OK;
                }
                catch (TechnicalException e)
                {
                    switch (e.TechnicalCode)
                    {
                        case AdhesionServices.NOT_INFORMED:
                            result.AlphanumericState = Models.AlphanumericState.NOT_INFORMED;
                            break;
                        case AdhesionServices.CONSECUTIVE_CHARACTERS:
                            result.AlphanumericState = Models.AlphanumericState.CONSECUTIVE_CHARACTERS;
                            break;
                        case AdhesionServices.INCORRECT_CHARACTERS:
                            result.AlphanumericState = Models.AlphanumericState.INCORRECT_CHARACTERS;
                            break;
                        default:
                            throw;
                    }
                }

                return new ObjectResult(result);
            }
            catch (System.Exception e)
            {
                _logger.LogError(e.ToString());
                return new ObjectResult("Error al adherir cliente.") { StatusCode = 500 };
            }
        }
    }
}

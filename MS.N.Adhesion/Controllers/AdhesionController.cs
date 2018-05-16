using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BGBA.Models.N.Adhesion;
using BGBA.Models.N.Core.Utils.ObjectFactory;
using BGBA.Services.N.Autenticacion;
using BGBA.Services.N.Adhesion;
using System.Security.Cryptography.X509Certificates;

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
            :base(logger,configuration)
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

                var trace = new Models.N.Core.Trace.TraceEventHandler(delegate (object sender, Models.N.Core.Trace.TraceEventArgs e)
                {
                    base.Communicator_TraceHandler(sender, e);
                });

                var serviceAutenticacion = new AutenticacionServices(_configuration, _objectFactory, _certificate);
                serviceAutenticacion.TraceHandler += trace;
                datos.PinEncriptado = await serviceAutenticacion.GetSCSCipherPassword(datos.IdHost, datos.Pin);

                _logger.LogTrace("Encripto pin.");

                var serviceAdhesion = new AdhesionServices(_configuration, _objectFactory, _certificate);
                serviceAdhesion.TraceHandler += trace;
                datos.IdAdhesion = await serviceAdhesion.AdherirUsuario(datos);

                _logger.LogTrace("Adhirio usuario.");

                var estadoAlfanumerico = await serviceAdhesion.AltaAlfanumerico(datos);

                return new ObjectResult(new { datos.IdAdhesion, estadoAlfanumerico});
            }
            catch (System.Exception e)
            {
                _logger.LogError(e.ToString());
                return new ObjectResult("Error al adherir cliente.") { StatusCode = 500 };
            }
        }
    }
}

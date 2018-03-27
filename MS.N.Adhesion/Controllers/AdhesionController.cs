using System.Threading.Tasks;
using Core.N.Utils.ObjectFactory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.N.Adhesion;

namespace MS.N.Adhesion.Controllers
{
    [Route("api/adherir")]
    public class AdhesionController : Controller
    {
        public const string ErrorPrefix = "MS_Adherir";

        private IConfiguration _configuration;
        private IObjectFactory _objectFactory;
        private ILogger<AdhesionController> _logger;

        public AdhesionController(IConfiguration configuration, IObjectFactory objectFactory, ILogger<AdhesionController> logger)
        {
            _configuration = configuration;
            _objectFactory = objectFactory;
            _logger = logger;
        }

        [HttpPost("cliente")]
        public async Task<IActionResult> Cliente(DatosAdhesion datos)
        {
            try
            {
                var serviceAutenticacion = new Services.N.Autenticacion.AutenticacionServices(_configuration, _objectFactory);
                datos.PinEncriptado = await serviceAutenticacion.GetSCSCipherPassword(datos.IdHost, datos.Pin);

                _logger.LogTrace("Encripto pin.");

                var serviceAdhesion = new Services.N.Adhesion.AdhesionServices(_configuration, _objectFactory);
                datos.IdAdhesion = await serviceAdhesion.AdherirUsuario(datos);

                _logger.LogTrace("Adhirio usuario.");

                var estadoAlfanumerico = serviceAdhesion.AltaAlfanumerico(datos);

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

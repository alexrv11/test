using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BGBA.Models.N.Adhesion;
using BGBA.Models.N.Core.Utils.ObjectFactory;
using BGBA.Services.N.Autenticacion;
using BGBA.Services.N.Adhesion;

namespace BGBA.MS.N.Adhesion.Controllers
{
    [Route("api/adherir")]
    public class AdhesionController : Controller
    {
        public const string ErrorPrefix = "MS_Adherir";

        private readonly IConfiguration _configuration;
        private readonly IObjectFactory _objectFactory;
        private readonly ILogger<AdhesionController> _logger;

        public AdhesionController(IConfiguration configuration, IObjectFactory objectFactory, ILogger<AdhesionController> logger)
        {
            _configuration = configuration;
            _objectFactory = objectFactory;
            _logger = logger;
        }

        [HttpPost("cliente")]
        public async Task<IActionResult> Cliente([FromBody]DatosAdhesion datos)
        {
            try
            {
                var serviceAutenticacion = new AutenticacionServices(_configuration, _objectFactory);
                datos.PinEncriptado = await serviceAutenticacion.GetSCSCipherPassword(datos.IdHost, datos.Pin);

                _logger.LogTrace("Encripto pin.");

                var serviceAdhesion = new AdhesionServices(_configuration, _objectFactory);
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

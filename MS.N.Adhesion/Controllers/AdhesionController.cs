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

            var result = new Core.N.Models.MicroserviceModel<DatosAdhesion> {
                Header = new Core.N.Models.Header {
                    Status = Core.N.Models.Status.Ok
                }
            };

            try
            {
                var serviceAutenticacion = new Services.N.Autenticacion.AutenticacionServices(_configuration, _objectFactory);
                datos.PinEncriptado = await serviceAutenticacion.GetSCSCipherPassword(datos.IdHost, datos.Pin);

                var serviceAdhesion = new Services.N.Adhesion.AdhesionServices(_configuration, _objectFactory);
                datos.IdAdhesion = await serviceAdhesion.AdherirUsuario(datos);

                var asd = serviceAdhesion.AltaAlfanumerico(datos);

                return new ObjectResult(datos);
            }
            catch (System.Exception e)
            {
                result.Header.Status = Core.N.Models.Status.Error;
                result.Header.ErrorCode = $"{ErrorPrefix}_Cliente";
                result.Header.Description = e.ToString();

                return new ObjectResult(result) { StatusCode = 500 };
            }


        }
    }
}

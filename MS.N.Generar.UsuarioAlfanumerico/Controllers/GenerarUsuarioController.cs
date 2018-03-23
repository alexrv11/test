using System;
using System.Threading.Tasks;
using Core.N.Utils.ObjectFactory;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.N.Adhesion;

namespace MS.N.Generar.UsuarioAlfanumerico.Controllers
{
    [Route("api/generar")]
    public class GenerarUsuarioController : Controller
    {
        public const string ErrorPrefix = "MS_Generar_Usuario_Alfanumerico";

        private IConfiguration _configuration;
        private IObjectFactory _objectFactory;
        private ILogger _logger;

        public GenerarUsuarioController(IConfiguration configuration, IObjectFactory objectFactory, ILogger logger)
        {
            _configuration = configuration;
            _objectFactory = objectFactory;
            _logger = logger;
        }

        [Route("usuario-alfanumerico")]
        public async Task<IActionResult> UsuarioAlfanumerico(DatosAdhesion datos)
        {
            try
            {
                var services = new Services.N.Adhesion.AdhesionServices(_configuration, _objectFactory);
                var model = new Core.N.Models.MicroserviceModel<string>
                {
                    Header = new Core.N.Models.Header
                    {
                        Status = Core.N.Models.Status.Ok
                    },
                    Model = await services.AltaAlfanumerico(datos)
                };


                return new ObjectResult(model);

            }
            catch (Exception e)
            {
                var model = new Core.N.Models.MicroserviceModel<string>
                {
                    Header = new Core.N.Models.Header
                    {
                        Status = Core.N.Models.Status.Error,
                        Description = e.ToString(),
                        ErrorCode = ErrorPrefix
                    }
                };

                return new ObjectResult(model) { StatusCode = 500 };
            }
        }
    }
}
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Services.N.Afip;

namespace MS.N.Afip.Controllers
{
    [Route("api/[controller]")]
    public class AfipController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IAFIPServices _afipServices;

        public AfipController(IConfiguration configuration, ILogger<AfipController> logger, IAFIPServices afipServices)
        {
            _configuration = configuration;
            _logger = logger;
            _afipServices = afipServices;
        }

        [HttpPost("credenciales")]
        public async Task<IActionResult> Credenciales()
        {
            try
            {
                _logger.LogTrace("Consulta credenciales AFIP.");   
                return new ObjectResult(await _afipServices.GetCredentials());
            }
            catch (System.Exception e)
            {
                _logger.LogError(e.ToString());
                return new ObjectResult("Error al consultar las credenciales de AFIP.") { StatusCode = 500 };
            }

        }

        [HttpPost("consulta-cliente")]
        public async Task<IActionResult> Consulta_Cliente([FromBody]string Cuix) {
            try
            {
                _logger.LogTrace("Consulta cliente en AFIP.");
                return new ObjectResult(await _afipServices.GetClientAFIP(Cuix));
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return new ObjectResult("Error al consultar cliente.") { StatusCode = 500 };
            }
        }

    }
}

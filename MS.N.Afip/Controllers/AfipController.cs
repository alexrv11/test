using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Services.N.Afip;

namespace MS.N.Afip.Controllers
{
    [Route("api/afip")]
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

        [HttpGet("credentials")]
        public async Task<IActionResult> Credentials()
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

        [HttpGet("client/{cuix}")]
        public async Task<IActionResult> GetClient(string cuix) {
            try
            {
                _logger.LogTrace("Consulta cliente en AFIP.");
                return new ObjectResult(await _afipServices.GetClient(cuix));
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return new ObjectResult("Error al consultar cliente.") { StatusCode = 500 };
            }
        }

        [HttpGet("config")]
        public IActionResult Config()
        {
            try
            {
                return new ObjectResult(_configuration.AsEnumerable());
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError);
            }
        }

    }
}

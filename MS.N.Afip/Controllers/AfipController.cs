using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BGBA.Models.N.Core.Microservices;
using BGBA.Services.N.Afip;

namespace BGBA.MS.N.Afip.Controllers
{
    [Route("api/afip")]
    public class AfipController : MicroserviceController
    {
        private readonly ILogger _logger;
        private readonly IAfipServices _afipServices;

        public AfipController(IConfiguration configuration, ILogger<AfipController> logger, IAfipServices afipServices) : base(logger)
        {
            _logger = logger;
            _afipServices = afipServices;

            var trace = new Models.N.Core.Trace.TraceEventHandler(delegate (object sender, Models.N.Core.Trace.TraceEventArgs e)
            {
                base.Communicator_TraceHandler(sender, e);
            });
            _afipServices.TraceHandler += trace;
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
    }
}

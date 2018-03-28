using System;
using System.Threading.Tasks;
using Models.N.Consulta.Padron;
using Microsoft.AspNetCore.Mvc;
using Services.N.ConsultaCliente;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MS.N.Consulta.Cliente.Controllers
{
    [Route("api/consulta-cliente")]
    public class ConsultaClienteController : Controller
    {
        public static string ErrorPrefix = "MS_ConsultaCliente";

        private IConsultaClienteServices _service;
        private readonly ILogger _logger;

        public ConsultaClienteController(IConsultaClienteServices service, ILogger logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("")]
        public async Task<IActionResult> Index([FromBody]string du, string sexo)
        {

            var cuix = String.Empty;

            try
            {
                cuix = await _service.GetCuix(du, sexo);
                _logger.LogTrace("Consulto cuix.");
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return new ObjectResult("Error al consultar cuil."){ StatusCode = 500 };
            }

            try
            {
                var datosPadron = await _service.GetDatosPadronAfip(cuix);
                _logger.LogTrace("Consulto datos padron.");

                return new ObjectResult(datosPadron);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());

                return new ObjectResult("Error al consultar los datos padron.")
                { StatusCode = 500 };
            }

        }
    }
}
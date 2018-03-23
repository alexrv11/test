using System;
using System.Threading.Tasks;
using Models.N.Consulta.Padron;
using Microsoft.AspNetCore.Mvc;
using Services.N.ConsultaCliente;

namespace MS.N.Consulta.Cliente.Controllers
{
    [Route("api/consulta-cliente")]
    public class ConsultaClienteController : Controller
    {
        public static string ErrorPrefix = "MS_ConsultaCliente";

        private IConsultaClienteServices _service;

        public ConsultaClienteController(IConsultaClienteServices service)
        {
            _service = service;
        }

        [HttpGet()]
        public async Task<IActionResult> Index(string du, string sexo)
        {

            var cuix = String.Empty;

            try
            {
                cuix = await _service.GetCuix(du, sexo);
            }
            catch (Exception e)
            {
                return new ObjectResult(new Core.N.Models.MicroserviceModel<DatosPadron>
                {
                    Header = new Core.N.Models.Header
                    {
                        Status = Core.N.Models.Status.Error,
                        Description = e.ToString(),
                        ErrorCode = $"{ErrorPrefix}_ConsCuix"
                    }
                }){ StatusCode = 500 };
            }

            try
            {
                return new ObjectResult(await _service.GetDatosPadron(cuix));
            }
            catch (Exception e)
            {
                return new ObjectResult(new Core.N.Models.MicroserviceModel<DatosPadron>
                {
                    Header = new Core.N.Models.Header
                    {
                        Status = Core.N.Models.Status.Error,
                        Description = e.ToString(),
                        ErrorCode = $"{ErrorPrefix}_ConsPadron"
                    }
                })
                { StatusCode = 500 };
            }

        }
    }
}
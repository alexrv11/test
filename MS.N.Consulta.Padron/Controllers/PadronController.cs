using System.Threading.Tasks;
using Core.N.Models;
using Microsoft.AspNetCore.Mvc;
using Models.N.Consulta.Padron;
using Services.N.Consulta.Padron;

namespace MS.N.Consulta.Padron.Controllers
{
    [Route("api/padron")]
    public class PadronController : Controller
    {
        public static string ErrorPrefix = "MS_Padron";
        private IPadronServices _padronService;

        public PadronController(IPadronServices padronService)
        {
            _padronService = padronService;
        }

        [HttpGet()]
        public async Task<IActionResult> Get(string cuix)
        {
            var vm = new MicroserviceModel<DatosPadron>();

            try
            {
                vm.Model = await _padronService.ConsultaPadron(cuix);
                vm.Header = new Header
                {
                    Status = Status.Ok
                };

                return new ObjectResult(vm);
            }
            catch (System.Exception e)
            {
                vm.Header = new Header
                {
                    Status = Status.Error,
                    Description = e.ToString(),
                    ErrorCode = $"{ErrorPrefix}_ERR_CONS_PADRON"
                };

                return new ObjectResult(vm) { StatusCode = 500 };
            }
        }
    }
}

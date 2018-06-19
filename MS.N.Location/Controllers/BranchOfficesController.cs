using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BGBA.Services.N.Location;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MS.N.Location.Controllers
{
    [Route("api/branchoffices")]
    public class BranchOfficesController : BGBA.Models.N.Core.Microservices.MicroserviceController
    {
        private readonly ISucursalServices _sucursalServices;
        private readonly IConfiguration _configuration;
        private readonly ILogger<BranchOfficesController> _logger;

        public BranchOfficesController(IConfiguration configuration, ISucursalServices sucursalServices, 
            ILogger<BranchOfficesController> logger)
            : base(logger)
        {
            _sucursalServices = sucursalServices;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Sucursales()
        {
            try
            {
                return new ObjectResult(await _sucursalServices.GetSucursales());
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, "Error al consultar las sucursales.");
            }
        }

        [HttpPost("{number}")]
        public async Task<IActionResult> MapSucursal(string number)
        {

            try
            {
                var map = await _sucursalServices.GetMapSucursal(number);
                
                if (String.IsNullOrEmpty(map))
                    return NotFound("No se encontró la sucursal.");

                return new ObjectResult(map);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, "Error al generar la URL de mapa para sucursal.");
            }
        }
    }
}
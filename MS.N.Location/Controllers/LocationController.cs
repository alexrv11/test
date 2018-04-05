using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Services.N.Location;

namespace MS.N.Location.Controllers
{
    [Route("api/location")]
    public class LocationController : Controller
    {
        public const string ErrorPrefix = "MS_Location";

        private readonly IConfiguration _configuration;
        private readonly ISucursalServices _sucursalServices;
        private readonly IMapServices _mapServices;
        private readonly ILogger _logger;

        public LocationController(IConfiguration configuration, ISucursalServices sucursalServices, IMapServices mapServices, ILogger<LocationController> logger)
        {
            _configuration = configuration;
            _sucursalServices = sucursalServices;
            _mapServices = mapServices;
            _logger = logger;
        }

        [HttpPost("georeference")]
        public async Task<IActionResult> Georeference([FromBody]Models.N.Location.Address address)
        {
            try
            {
                var fullAddress = await _mapServices.GetFullAddress(address);
                
                if (fullAddress.Status == "ZERO_RESULTS")
                    return NotFound();
                
                return new ObjectResult(fullAddress.Results.FirstOrDefault().Geometry.Location);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, "Error al georeferencias la direccion.");
            }
        }

        [HttpPost("map")]
        public async Task<IActionResult> Map([FromBody]Models.N.Location.MapOptions options)
        {
            try
            {
                if (options.LocationGetCoord)
                {
                    var fullAddress = (await _mapServices.GetFullAddress(options.Address));
                    
                    if (fullAddress.Status == "ZERO_RESULTS")
                        return NotFound();

                    options.Location = fullAddress.Results.FirstOrDefault().Geometry.Location;
                    options.LocationIsCoord = true;
                }

                return new ObjectResult($"{_configuration["GoogleMaps:UrlMap"].Replace("{key}", _configuration["GoogleMaps:Key"])}&{options.ToString()}");
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, "Error al generarl la referencia de ");
            }
        }

        [HttpPost("sucursales")]
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

        [HttpPost("map-sucursal")]
        public async Task<IActionResult> MapSucursal([FromBody]string numeroSucursal)
        {

            try
            {
                if (String.IsNullOrWhiteSpace(numeroSucursal))
                {
                    return BadRequest("Parametro sucursal vacio.");
                }

                var sucursal = await _sucursalServices.GetSucursal(numeroSucursal);

                if (sucursal == null)
                    return NotFound("No se encontró la sucursal.");

                var mapOptions = new Models.N.Location.MapOptions
                {
                    Location = new Models.N.Location.Location
                    {
                        Latitude = sucursal.Latitud,
                        Longitude = sucursal.Longitud
                    },
                    DefaultMarker = true,
                    LocationIsCoord = true,
                };

                return new ObjectResult($"{_configuration["GoogleMaps:UrlMap"].Replace("{key}", _configuration["GoogleMaps:Key"])}&{mapOptions.ToString()}");
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError,"Error al generar la URL de mapa para sucursal.");
            }
        }

        [HttpPost("address-google-maps")]
        public async Task<IActionResult> Address_Google_Maps([FromBody]Models.N.Location.Address address)
        {
            try
            {
                var result = await _mapServices.GetFullAddress(address);

                return new ObjectResult(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, "Error al consultar la direccion.");
            }
        }
    }
}
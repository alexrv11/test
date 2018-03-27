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
        public async Task<IActionResult> Georeference(Models.N.Location.Address address)
        {
            try
            {
                var result = await _mapServices.GetLocation(address);

                return new ObjectResult(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return new ObjectResult("Error al georeferencias la direccion."){ StatusCode = 500 };
            }
        }

        [HttpPost("map")]
        public async Task<IActionResult> Map(Models.N.Location.MapOptions options)
        {
            try
            {
                if (options.LocationGetCoord)
                {
                    options.Location = await _mapServices.GetLocation(options.Address);
                    options.LocationIsCoord = true;
                }

                return new ObjectResult($"{_configuration["GoogleMaps:UrlMap"].Replace("{key}", _configuration["GoogleMaps:Key"])}&{options.ToString()}");
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return new ObjectResult("Error al generarl la referencia de ") { StatusCode = 500 };
            }
        }

        [HttpPost("sucursales")]
        public async Task<IActionResult> MapSucursal()
        {
            try
            { 
                return new ObjectResult(await _sucursalServices.GetSucursales());
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return new ObjectResult("Error al consultar las sucursales.") { StatusCode = 500 };
            }
        }

        [HttpPost("map-sucursal")]
        public async Task<IActionResult> MapSucursal(string numeroSucursal)
        {

            try
            {
                if (String.IsNullOrWhiteSpace(numeroSucursal))
                {
                    return NotFound("Parametro sucursal vacio.");
                }

                var sucursal = await _sucursalServices.GetSucursal(numeroSucursal);

                if (sucursal == null)
                    return NotFound("No se encontró la sucursal.");

                var mapOptions = new Models.N.Location.MapOptions {
                    Location = new Models.N.Location.Location {
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
                return new ObjectResult("Error al generar la URL de mapa para sucursal.") { StatusCode = 500 };
            }
        }
    }
}
﻿using System;
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

                return new ObjectResult("") { StatusCode = 500; }
            }
        }

        [HttpPost("map")]
        public async Task<IActionResult> Map(Models.N.Location.MapOptions options)
        {
            var result = new Core.N.Models.MicroserviceModel<string>
            {
                Header = new Core.N.Models.Header
                {
                    Status = Core.N.Models.Status.Ok
                }
            };

            try
            {
                if (options.LocationGetCoord)
                {
                    options.Location = await _mapServices.GetLocation(options.Address);
                    options.LocationIsCoord = true;
                }

                result.Model = $"{_configuration["GoogleMaps:UrlMap"].Replace("{key}", _configuration["GoogleMaps:Key"])}&{options.ToString()}";

                return new ObjectResult(result);
            }
            catch (Exception e)
            {
                result.Header.ErrorCode = $"{ErrorPrefix}_Map";
                result.Header.Description = e.ToString();
                result.Header.Status = Core.N.Models.Status.Error;

                return new ObjectResult(result) { StatusCode = 500 };
            }
        }

        [HttpPost("map-sucursal")]
        public async Task<IActionResult> MapSucursal(string numeroSucursal)
        {
            var result = new Core.N.Models.MicroserviceModel<string>
            {
                Header = new Core.N.Models.Header
                {
                    Status = Core.N.Models.Status.Ok
                }
            };

            try
            {
                if (String.IsNullOrWhiteSpace(numeroSucursal))
                {
                    return NotFound(result);
                }

                var sucursal = await _sucursalServices.GetSucursal(numeroSucursal);

                if (sucursal == null)
                    return NotFound(result);

                var mapOptions = new Models.N.Location.MapOptions {
                    Location = new Models.N.Location.Location {
                        Latitude = sucursal.Latitud,
                        Longitude = sucursal.Longitud
                    },
                    DefaultMarker = true,
                    LocationIsCoord = true,
                };

                result.Model = $"{_configuration["GoogleMaps:UrlMap"].Replace("{key}", _configuration["GoogleMaps:Key"])}&{mapOptions.ToString()}";

                return new ObjectResult(result);
            }
            catch (Exception e)
            {
                result.Header.ErrorCode = $"{ErrorPrefix}_MapSucursal";
                result.Header.Description = e.ToString();
                result.Header.Status = Core.N.Models.Status.Error;

                return new ObjectResult(result) { StatusCode = 500 };
            }
        }
    }
}
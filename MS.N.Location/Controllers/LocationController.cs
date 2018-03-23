using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace MS.N.Location.Controllers
{
    [Route("api/location")]
    public class LocationController : Controller
    {
        public const string ErrorPrefix = "MS_Location";

        private readonly IConfiguration _configuration;

        public LocationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("georeference")]
        public async Task<IActionResult> Georeference(Models.N.Location.Address address)
        {
            var service = new Services.N.Location.GoogleMapsServices(_configuration);

            var result = new Core.N.Models.MicroserviceModel<Models.N.Location.Location>
            {
                Header = new Core.N.Models.Header
                {
                    Status = Core.N.Models.Status.Ok
                }
            };


            try
            {
                result.Model = await service.GetLocation(address);

                return new ObjectResult(result);
            }
            catch (Exception e)
            {
                result.Header.ErrorCode = $"{ErrorPrefix}_Address";
                result.Header.Description = e.ToString();
                result.Header.Status = Core.N.Models.Status.Error;

                return new ObjectResult(result) { StatusCode = 500 };
            }
        }

        [HttpPost("map")]
        public async Task<IActionResult> Map(Models.N.Location.MapOptions options)
        {
            var service = new Services.N.Location.GoogleMapsServices(_configuration);

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
                    options.Location = await service.GetLocation(options.Address);
                    options.LocationIsCoord = true;
                }

                result.Model = $"{_configuration["GoogleMaps:UrlMap"].Replace("{key}", _configuration["GoogleMaps:Key"])}&{options.ToString()}";

                return new ObjectResult(result);
            }
            catch (Exception e)
            {
                result.Header.ErrorCode = $"{ErrorPrefix}_Address";
                result.Header.Description = e.ToString();
                result.Header.Status = Core.N.Models.Status.Error;

                return new ObjectResult(result) { StatusCode = 500 };
            }
        }
    }
}
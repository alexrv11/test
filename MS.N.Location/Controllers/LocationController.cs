using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BGBA.Models.N.Location;
using BGBA.Services.N.Location;
using BGBA.Services.N.ATReference;
using BGBA.Models.N.Core.Utils.Extensions;
using AutoMapper;
using System.Text.RegularExpressions;

namespace MS.N.Location.Controllers
{
    [Route("api/location")]
    public class LocationController : BGBA.Models.N.Core.Microservices.MicroserviceController
    {
        private readonly IMapServices _mapServices;
        private readonly ILogger _logger;
        private readonly TableHelper _tableHelper;

        public LocationController(IMapServices mapServices, 
            ILogger<LocationController> logger,
            ITableServices tableServices,
            IMapper mapper)
            : base(logger)
        {
            _mapServices = mapServices;
            _logger = logger;

            var trace = new BGBA.Models.N.Core.Trace.TraceEventHandler(delegate (object sender, BGBA.Models.N.Core.Trace.TraceEventArgs e)
            {
                base.Communicator_TraceHandler(sender, e);
            });

            _mapServices.TraceHandler += trace;
            tableServices.TraceHandler += trace;

            _tableHelper = new TableHelper(tableServices, mapper);
        }

        [HttpPost("georeference")]
        public async Task<IActionResult> Georeference([FromBody]BGBA.Models.N.Location.Address address)
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
        public async Task<IActionResult> Map([FromBody]MapOptions options)
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

                return new ObjectResult(_mapServices.GetUrlMap(options));
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, "Error al generarl la referencia de ");
            }
        }

        [HttpPost("address-google-maps")]
        public async Task<IActionResult> Address_Google_Maps([FromBody]Address address)
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

        [HttpPost("normalize-address")]
        public async Task<IActionResult> NormalizeAddress([FromBody]MapOptions mapOptions)
        {
            try
            {
                var mapAddress = await _mapServices.GetFullAddress(mapOptions.Address);
                _logger.LogInformation("Consulto google maps api");

                if (mapAddress.Status == "ZERO_RESULTS")
                    return NotFound();

                await _mapServices.NormalizeAddress(mapOptions, mapAddress);
                
                return new ObjectResult(mapOptions.Address);

            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, "Error al normalizar la direccion.");
            }
        }

        [HttpPost("normalize-address/{placeId}")]
        public async Task<IActionResult> NormalizeAddress(string placeId, [FromBody]MapOptions options)
        {
            try
            {
                var mapAddress = await _mapServices.GetFullAddress(placeId);
                _logger.LogInformation("Consulto google maps api");

                if (mapAddress.Status == "ZERO_RESULTS")
                    return NotFound();

                var result  = await _mapServices.NormalizeAddressByPlaceId(mapAddress, options);

                return new ObjectResult(result);

            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, "Error al normalizar la direccion.");
            }
        }

        [HttpPost("predictive")]
        public async Task<IActionResult> Predictive([FromBody]BGBA.Models.N.Location.MapOptions options)
        {
            try
            {
                var fullAddress = await _mapServices.GetPrediction(options);

                if (fullAddress.Status == "ZERO_RESULTS")
                    return NotFound();

                return new ObjectResult(fullAddress.Results);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, "Error al predecir la direccion.");
            }
        }
    }
}
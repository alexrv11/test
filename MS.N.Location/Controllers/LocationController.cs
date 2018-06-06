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
        public const string ErrorPrefix = "MS_Location";

        private readonly IConfiguration _configuration;
        private readonly ISucursalServices _sucursalServices;
        private readonly IMapServices _mapServices;
        private readonly ILogger _logger;
        private readonly TableHelper _tableHelper;

        public LocationController(IConfiguration configuration, ISucursalServices sucursalServices,
            IMapServices mapServices, ILogger<LocationController> logger, ITableServices tableServices,
            IMapper mapper)
            : base(logger)
        {
            _configuration = configuration;
            _sucursalServices = sucursalServices;
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

                var mapOptions = new BGBA.Models.N.Location.MapOptions
                {
                    Location = new BGBA.Models.N.Location.Location
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
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, "Error al generar la URL de mapa para sucursal.");
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

                var firstCoincidence = mapAddress.Results.FirstOrDefault().AddressComponents;

                mapOptions.Address.LocalityDescription = firstCoincidence.FirstOrDefault(a => a.Types.Any(t => BGBA.Models.N.Location.GoogleMapsAddress.LOCALITY_SUBLOCALITY.Contains(t)))?.ShortName ?? mapOptions.Address.LocalityDescription;
                mapOptions.Address.Number = firstCoincidence.FirstOrDefault(a => a.Types.Any(t => BGBA.Models.N.Location.GoogleMapsAddress.STREET_NUMBER.Contains(t)))?.LongName ?? mapOptions.Address.Number;
                mapOptions.Address.Street = firstCoincidence.FirstOrDefault(a => a.Types.Any(t => BGBA.Models.N.Location.GoogleMapsAddress.STREET.Contains(t)))?.LongName ?? mapOptions.Address.Street;

                var provinces = await _tableHelper.GetProvincesAsync();
                var provinceName = firstCoincidence.FirstOrDefault(a => a.Types.Any(t => BGBA.Models.N.Location.GoogleMapsAddress.PROVINCE.Contains(t)))?.ShortName.RemoveDiacritics();

                if (provinceName == "CABA")
                {
                    provinceName = "CAPITAL FEDERAL";
                    mapOptions.Address.LocalityDescription = "CIUDAD AUTONOMA BUENOS AI";
                }

                mapOptions.Address.Province = provinces.FirstOrDefault(p => p.Name.ToLower() == provinceName.ToLower()) ?? mapOptions.Address.Province;

                var country = firstCoincidence.FirstOrDefault(a => a.Types.Any(t => BGBA.Models.N.Location.GoogleMapsAddress.COUNTRY.Contains(t)))?.LongName;
                mapOptions.Address.Country = (await _tableHelper.GetCountriesAsync()).FirstOrDefault(c => c.Description.ToLower() == country.ToLower()) ?? mapOptions.Address.Country;



                var cpGoogle = $"{firstCoincidence.FirstOrDefault(a => a.Types.Any(t => BGBA.Models.N.Location.GoogleMapsAddress.POSTAL_CODE.Contains(t)))?.LongName}{firstCoincidence.FirstOrDefault(a => a.Types.Any(t => BGBA.Models.N.Location.GoogleMapsAddress.POSTAL_CODE_SUFFIX.Contains(t)))?.LongName}";

                if (!string.IsNullOrEmpty(cpGoogle) && cpGoogle.Length < 8)
                {
                    cpGoogle = Regex.Replace(cpGoogle, "[a-z]*", "",RegexOptions.IgnoreCase).Trim();

                    if (cpGoogle.Length == 4)
                        mapOptions.Address.PostalCode = cpGoogle;
                }
                else if (!string.IsNullOrEmpty(cpGoogle) && cpGoogle.Length == 8)
                    mapOptions.Address.PostalCode = cpGoogle;
                else if (string.IsNullOrWhiteSpace(mapOptions.Address.PostalCode))
                {
                    var cps = await _tableHelper.GetLocalitiesByProvinceWithCPAsync(mapOptions.Address.Province);
                    var localityCP = cps.Where(l => l.Name == mapOptions.Address.LocalityDescription || l.Name.Contains(mapOptions.Address.LocalityDescription) || mapOptions.Address.LocalityDescription.Contains(l.Name)).ToList();
                    if (localityCP.Count > 0)
                        mapOptions.Address.PostalCodeOcurrencies = localityCP.Select(c => c.PostalCode).ToList();
                    else
                        mapOptions.Address.PostalCodeOcurrencies = cps.Select(c => c.PostalCode).ToList();
                }

                mapOptions.Address.Location = mapAddress.Results.FirstOrDefault()?.Geometry.Location;
                mapOptions.Location = mapOptions.Address.Location;
                mapOptions.LocationIsCoord = true;

                mapOptions.Address.UrlMap = $"{_configuration["GoogleMaps:UrlMap"].Replace("{key}", _configuration["GoogleMaps:Key"])}&{mapOptions.ToString()}";


                return new ObjectResult(mapOptions.Address);

            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, "Error al normalizar la direccion.");
            }

        }
    }
}
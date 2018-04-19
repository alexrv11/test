using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Services.N.Location;
using System.Linq;
using Models.N.Location;
using Services.N.Consulta.ATReference;
using Services.N.Afip;
using Services.N.Client;
using Models.N.Client;
using MS.N.Client.ViewModels;

namespace MS.N.Client.Controllers
{
    [Route("api/client")]
    public class ClientController : Controller
    {
        public static string ErrorPrefix = "MS_ConsultaCliente";
        private readonly IConfiguration _configuration;
        private readonly IMapServices _mapServices;
        private readonly IClientServices _clientServices;
        private readonly ILogger _logger;
        private readonly TableHelper _tableHelper;
        private IAFIPServices _afipServices;

        public ClientController(IClientServices clientServices, IAFIPServices afipServices,
            ILogger<ClientController> logger, IMapServices mapServices, IConfiguration configuration,
            TableHelper tableHelper)
        {
            _configuration = configuration;
            _mapServices = mapServices;
            _clientServices = clientServices;
            _logger = logger;
            _tableHelper = tableHelper;
            _afipServices = afipServices;
        }

        [HttpPost("{du}/{sex}")]
        public async Task<IActionResult> GetClient(string du, Sex sex, [FromBody]MapOptions mapOptions)
        {

            //var data = await _clientServices.GetClientNV(new ClientData
            //{
            //    DocumentType = "DU",
            //    DocumentNumber = "123456789"
            //});

            if (!ModelState.IsValid)
                return BadRequest();

            var cuix = String.Empty;

            try
            {
                cuix = await _clientServices.GetCuix(du, sex.ToString());
                _logger.LogTrace("Cuix OK.");
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, "Error getting CUIX");
            }

            try
            {
                var dataPadron = await _afipServices.GetClient(cuix);

                if (dataPadron == null)
                    return NotFound();

                _logger.LogTrace("Afip services OK.");


                foreach (var address in dataPadron.Addresses)
                {
                    try
                    {
                        var mapAddress = await _mapServices.GetFullAddress(address);
                        _logger.LogTrace("Google maps ok");

                        if (mapAddress.Status != "ZERO_RESULTS")
                        {
                            NormalizeAddress(mapOptions, mapAddress, address, _configuration["GoogleMaps:UrlMap"].Replace("{key}", _configuration["GoogleMaps:Key"]));
                            _logger.LogTrace("Address normalized.");
                        }
                        else
                        {
                            _logger.LogTrace("Address not found");
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e.ToString());
                        _logger.LogTrace("Error normalizing address.");
                    }
                }
                
                try
                {
                    dataPadron.HostId = await _clientServices.GetClientNV(dataPadron);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.ToString());
                    _logger.LogTrace("Error getting client from NV.");
                }

                return new ObjectResult(dataPadron);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, "Error al consultar los datos padron.");
            }

        }

        [HttpPost]
        public async Task<IActionResult> AddClient([FromBody]ClientData client)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            try
            {
                var dataPadron = await _clientServices.AddClient(client);
                _logger.LogTrace("add client.");

                return new ObjectResult(dataPadron);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, "error adding client.");
            }

        }

        [HttpPatch]
        public async Task<IActionResult> UpdateClient([FromBody]ClientData client)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            try
            {
                var dataPadron = await _clientServices.UpdateAddress(client);
                _logger.LogTrace("update client.");

                return new ObjectResult(dataPadron);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, "error adding client.");
            }

        }

        private async void NormalizeAddress(MapOptions mapOptions, GoogleMapsAddress mapAddress, Address realAddress, string urlMap)
        {
            var firstCoincidence = mapAddress.Results.FirstOrDefault().AddressComponents;

            realAddress.LocalityDescription = firstCoincidence.FirstOrDefault(a => a.Types.Any(t => Models.N.Location.GoogleMapsAddress.LOCALITY_SUBLOCALITY.Contains(t)))?.ShortName ?? realAddress.LocalityDescription;
            realAddress.Number = firstCoincidence.FirstOrDefault(a => a.Types.Any(t => Models.N.Location.GoogleMapsAddress.STREET_NUMBER.Contains(t)))?.LongName ?? realAddress.Number;
            realAddress.Street = firstCoincidence.FirstOrDefault(a => a.Types.Any(t => Models.N.Location.GoogleMapsAddress.STREET.Contains(t)))?.LongName ?? realAddress.Street;

            var cpGoogle = $"{firstCoincidence.FirstOrDefault(a => a.Types.Any(t => Models.N.Location.GoogleMapsAddress.POSTAL_CODE.Contains(t)))?.LongName}{firstCoincidence.FirstOrDefault(a => a.Types.Any(t => Models.N.Location.GoogleMapsAddress.POSTAL_CODE_SUFFIX.Contains(t)))?.LongName}";
            if (!string.IsNullOrEmpty(cpGoogle))
                realAddress.PostalCode = cpGoogle;

            realAddress.Location = mapAddress.Results.FirstOrDefault()?.Geometry.Location;

            if (mapOptions == null)
                mapOptions = new MapOptions
                {
                    DefaultMarker = true
                };

            mapOptions.Location = realAddress.Location;
            mapOptions.LocationIsCoord = true;

            realAddress.UrlMap = $"{urlMap}&{mapOptions.ToString()}";


            var provinces = await _tableHelper.GetProvincesAsync();
            var provinceName = firstCoincidence.FirstOrDefault(a => a.Types.Any(t => Models.N.Location.GoogleMapsAddress.PROVINCE.Contains(t)))?.ShortName;

            if (provinceName == "CABA")
            {
                provinceName = "CAPITAL FEDERAL";
                realAddress.LocalityDescription = "CIUDAD AUTONOMA BUENOS AI";
            }
            realAddress.Province = provinces.FirstOrDefault(p => p.Name.ToLower() == provinceName.ToLower()) ?? realAddress.Province;

            var country = firstCoincidence.FirstOrDefault(a => a.Types.Any(t => Models.N.Location.GoogleMapsAddress.COUNTRY.Contains(t)))?.LongName;
            realAddress.Country = (await _tableHelper.GetCountriesAsync()).FirstOrDefault(c => c.Description.ToLower() == country.ToLower()) ?? realAddress.Country;

        }
    }
}
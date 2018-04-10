using System;
using System.Threading.Tasks;
using Models.N.Consulta.Padron;
using Microsoft.AspNetCore.Mvc;
using Services.N.ConsultaCliente;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Services.N.Location;
using System.Linq;
using Models.N.Location;
using Services.N.Consulta.ATReference;
using MS.N.Consulta.Cliente.ViewModels;

namespace MS.N.Consulta.Cliente.Controllers
{
    [Route("api/consulta-cliente")]
    public class ConsultaClienteController : Controller
    {
        public static string ErrorPrefix = "MS_ConsultaCliente";
        private readonly IConfiguration _configuration;
        private readonly IMapServices _mapServices;
        private readonly IConsultaClienteServices _consultaClienteServices;
        private readonly ILogger _logger;
        private readonly TableHelper _tableServices;
        public static string RealAddress = "LEGAL/REAL";
        public static string FiscalAddress = "FISCAL";

        public ConsultaClienteController(IConsultaClienteServices consultaClienteServices,
            ILogger<ConsultaClienteController> logger, IMapServices mapServices, IConfiguration configuration,
            TableHelper tableHelper)
        {
            _configuration = configuration;
            _mapServices = mapServices;
            _consultaClienteServices = consultaClienteServices;
            _logger = logger;
            _tableServices = tableHelper;
        }

        [HttpPost()]
        public async Task<IActionResult> Index([FromBody]ConsultaClienteVM cliente)
        {


            if (!ModelState.IsValid)
                return BadRequest();

            var cuix = String.Empty;

            try
            {
                cuix = await _consultaClienteServices.GetCuix(cliente.DU, cliente.Sexo.ToString());
                _logger.LogTrace("Consulto cuix.");
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return new ObjectResult("Error al consultar cuil.") { StatusCode = 500 };
            }

            try
            {
                var dataPadron = await _consultaClienteServices.GetDatosPadronAfip(cuix);
                _logger.LogTrace("Consulto datos padron.");

                foreach (var address in dataPadron.Domicilios)
                {
                    var mapAddress = await _mapServices.GetFullAddress(address);
                    _logger.LogTrace("Consulto datos Maps.");

                    if (mapAddress.Status != "ZERO_RESULTS")
                    {
                        NormalizeAddress(cliente.MapOptions, mapAddress, address, _configuration["GoogleMaps:UrlMap"].Replace("{key}", _configuration["GoogleMaps:Key"]));
                        _logger.LogTrace("Direccion normalizada");
                    }
                    else
                    {
                        _logger.LogTrace("No se encontro direccion.");
                    }
                }

                return new ObjectResult(dataPadron);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());

                return new ObjectResult("Error al consultar los datos padron.")
                { StatusCode = 500 };
            }

        }

        private async void NormalizeAddress(MapOptions mapOptions, GoogleMapsAddress mapAddress, Address realAddress, string urlMap)
        {
            var firstCoincidence = mapAddress.Results.FirstOrDefault().AddressComponents;

            realAddress.LocalityDescription = firstCoincidence.FirstOrDefault(a => a.Types.Any(t => Models.N.Location.GoogleMapsAddress.LOCALITY_SUBLOCALITY.Contains(t)))?.ShortName ?? realAddress.LocalityDescription;
            realAddress.Number = firstCoincidence.FirstOrDefault(a => a.Types.Any(t => Models.N.Location.GoogleMapsAddress.STREET_NUMBER.Contains(t)))?.LongName ?? realAddress.Number;
            realAddress.Street = firstCoincidence.FirstOrDefault(a => a.Types.Any(t => Models.N.Location.GoogleMapsAddress.STREET.Contains(t)))?.LongName ?? realAddress.Street;
            realAddress.PostalCode = firstCoincidence.FirstOrDefault(a => a.Types.Any(t => Models.N.Location.GoogleMapsAddress.POSTAL_CODE.Contains(t)))?.LongName ?? realAddress.PostalCode;
            realAddress.Location = mapAddress.Results.FirstOrDefault()?.Geometry.Location;


            if (mapOptions == null)
                mapOptions = new MapOptions
                {
                    DefaultMarker = true
                };

            mapOptions.Location = realAddress.Location;
            mapOptions.LocationIsCoord = true;

            realAddress.UrlMap = $"{urlMap}&{mapOptions.ToString()}";


            var provinces = await _tableServices.GetProvincesAsync();
            var provinceName = firstCoincidence.FirstOrDefault(a => a.Types.Any(t => Models.N.Location.GoogleMapsAddress.PROVINCE.Contains(t)))?.ShortName;

            if (provinceName == "CABA")
            {
                provinceName = "CAPITAL FEDERAL";
                realAddress.LocalityDescription = "CIUDAD AUTONOMA BUENOS AI";
            }

            realAddress.Province = provinces.FirstOrDefault(p => p.Name.ToLower() == provinceName.ToLower()) ?? realAddress.Province;
            var country = firstCoincidence.FirstOrDefault(a => a.Types.Any(t => Models.N.Location.GoogleMapsAddress.COUNTRY.Contains(t)))?.LongName;
            realAddress.Country = (await _tableServices.GetCountriesAsync()).FirstOrDefault(c => c.Description.ToLower() == country.ToLower()) ?? realAddress.Country;

        }
    }
}
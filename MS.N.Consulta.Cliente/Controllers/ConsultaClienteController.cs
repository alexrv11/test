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

        [HttpGet()]
        public async Task<IActionResult> Index(string du, string sexo)
        {
            var cuix = String.Empty;

            try
            {
                cuix = await _consultaClienteServices.GetCuix(du, sexo);
                _logger.LogTrace("Consulto cuix.");
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return new ObjectResult("Error al consultar cuil."){ StatusCode = 500 };
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
                        NormalizeAddress(mapAddress, address, _configuration["GoogleMaps:UrlMap"].Replace("{key}", _configuration["GoogleMaps:Key"]));
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

        private async void NormalizeAddress(GoogleMapsAddress mapAddress, Address realAddress, string urlMap)
        {
            var firstCoincidence = mapAddress.Results.FirstOrDefault().AddressComponents;

            realAddress.LocalityDescription = firstCoincidence.FirstOrDefault(a => a.Types.Any(t => Models.N.Location.GoogleMapsAddress.LOCALITY_SUBLOCALITY.Contains(t))).ShortName;
            realAddress.Number = firstCoincidence.FirstOrDefault(a => a.Types.Any(t => Models.N.Location.GoogleMapsAddress.STREET_NUMBER.Contains(t))).LongName;
            realAddress.Street = firstCoincidence.FirstOrDefault(a => a.Types.Any(t => Models.N.Location.GoogleMapsAddress.STREET.Contains(t))).LongName;
            realAddress.PostalCode = firstCoincidence.FirstOrDefault(a => a.Types.Any(t => Models.N.Location.GoogleMapsAddress.POSTAL_CODE.Contains(t))).LongName;
            realAddress.Location = mapAddress.Results.FirstOrDefault().Geometry.Location;

            var mapOptions = new Models.N.Location.MapOptions
            {
                Location = realAddress.Location,
                DefaultMarker = true,
                LocationIsCoord = true,
            };

            realAddress.UrlMap =  $"{urlMap}&{mapOptions.ToString()}";


            var provinces = await _tableServices.GetProvincesAsync();
            var provinceName = firstCoincidence.FirstOrDefault(a => a.Types.Any(t => Models.N.Location.GoogleMapsAddress.PROVINCE.Contains(t))).ShortName;

            if (provinceName == "CABA")
            { 
                provinceName = "CAPITAL FEDERAL";
                realAddress.LocalityDescription = "CIUDAD AUTONOMA BUENOS AI";   
            }

            realAddress.Province = provinces.FirstOrDefault(p => p.Name.ToLower() == provinceName.ToLower());
            var country = firstCoincidence.FirstOrDefault(a => a.Types.Any(t => Models.N.Location.GoogleMapsAddress.COUNTRY.Contains(t))).LongName;
            realAddress.Country = (await _tableServices.GetCountriesAsync()).FirstOrDefault(c => c.Description.ToLower() == country.ToLower());

        }
    }
}
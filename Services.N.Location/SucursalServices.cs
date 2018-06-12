using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BGBA.Models.N.Core.Utils.ObjectFactory;
using BGBA.Models.N.Location;
using Microsoft.Extensions.Configuration;

namespace BGBA.Services.N.Location
{
    public class SucursalServices : ISucursalServices
    {
        private readonly IObjectFactory _objectFactory;
        private readonly IConfiguration _configuration;

        public SucursalServices(IObjectFactory objectFactory, IConfiguration configuration)
        {
            _objectFactory = objectFactory;
            _configuration = configuration;
        }

        public async Task<Sucursal> GetSucursal(string numeroSucursal)
        {
            var sucursales = await _objectFactory.InstantiateFromJsonFile<List<Sucursal>>(_configuration["Sucursales:Path"]);

            return sucursales.FirstOrDefault(s => s.Numero == numeroSucursal);
        }

        public async Task<string> GetMapSucursal(string numeroSucursal)
        {
            var sucursales = await _objectFactory.InstantiateFromJsonFile<List<Sucursal>>(_configuration["Sucursales:Path"]);

            var sucu = sucursales.FirstOrDefault(s => s.Numero == numeroSucursal);

            if (sucu == null)
                return null;

            var mapOptions = new BGBA.Models.N.Location.MapOptions
            {
                Location = new BGBA.Models.N.Location.Location
                {
                    Latitude = sucu.Latitud,
                    Longitude = sucu.Longitud
                },
                DefaultMarker = true,
                LocationIsCoord = true,
            };

            return $"{_configuration["GoogleMaps:UrlMap"].Replace("{key}", _configuration["GoogleMaps:Key"])}&{mapOptions.ToString()}";
        }

        public async Task<List<Sucursal>> GetSucursales()
        {
            return await _objectFactory.InstantiateFromJsonFile<List<Sucursal>>(_configuration["Sucursales:Path"]);

        }
    }
}

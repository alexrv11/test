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

        public async Task<List<Sucursal>> GetSucursales()
        {
            return await _objectFactory.InstantiateFromJsonFile<List<Sucursal>>(_configuration["Sucursales:Path"]);

        }
    }
}

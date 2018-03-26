using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.N.Utils.ObjectFactory;
using Microsoft.Extensions.Configuration;
using Models.N.Location;

namespace Services.N.Location
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
            var sucursales = await _objectFactory.InstantiateFromFile<List<Models.N.Location.Sucursal>>(_configuration["Sucursales:Path"]);

            return sucursales.FirstOrDefault(s => s.Numero == numeroSucursal);
        }

        public async Task<List<Sucursal>> GetSucursales()
        {
            return await _objectFactory.InstantiateFromFile<List<Models.N.Location.Sucursal>>(_configuration["Sucursales:Path"]);

        }
    }
}

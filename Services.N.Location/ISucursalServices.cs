using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.N.Location
{
    public interface ISucursalServices
    {
        Task<Models.N.Location.Sucursal> GetSucursal(string numeroSucursal);

        Task<List<Models.N.Location.Sucursal>> GetSucursales();
    }
}

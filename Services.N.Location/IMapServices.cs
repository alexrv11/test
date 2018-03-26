using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.N.Location
{
    public interface IMapServices
    {
        Task<Models.N.Location.Location> GetLocation(Models.N.Location.Address address);
        Task<string> GetMap(Models.N.Location.MapOptions options);
    }
}

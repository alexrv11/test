using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.N.Location
{
    public interface IMapServices
    {
        Task<Models.N.Location.GoogleMapsAddress> GetFullAddress(Models.N.Location.Address address);
        Task<string> GetUrlMap(Models.N.Location.MapOptions options);
    }
}

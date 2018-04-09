using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Models.N.Location;


namespace Services.N.Location
{
    public class GoogleMapsServices : IMapServices
    {
        private readonly IConfiguration _configuration;

        public GoogleMapsServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<GoogleMapsAddress> GetFullAddress(Address address)
        {
            var service = new Services.N.Core.Rest.RestServices
            {
                Url = $"{_configuration["GoogleMaps:Url"]}address={address.ToString()}&key={_configuration["GoogleMaps:Key"]}",
                TimeoutMilliseconds = Convert.ToInt32(_configuration["GoogleMaps:TimeoutMilliseconds"]),
                Method = "GET"
            };

            var result =await service.ExecuteAsync<GoogleMapsAddress>();
            
            return result;
        }

        public async Task<string> GetUrlMap(Models.N.Location.MapOptions options)
        {
            var service = new Services.N.Core.Rest.RestServices
            {
                Url = $"{_configuration["GoogleMaps:UrlMap"].Replace("{key}", _configuration["GoogleMaps:Key"])}&{options.ToString()}",
                TimeoutMilliseconds = Convert.ToInt32(_configuration["GoogleMaps:TimeoutMilliseconds"]),
                Method = "GET"
            };


            await service.ExecuteAsync();

            return service.Response;
        }

    }
}

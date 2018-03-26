using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Services.N.Location
{
    public class GoogleMapsServices : IMapServices
    {
        private readonly IConfiguration _configuration;

        public GoogleMapsServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<Models.N.Location.Location> GetLocation(Models.N.Location.Address address)
        {
            var service = new Core.N.Rest.RestServices
            {
                Url = $"{_configuration["GoogleMaps:Url"]}address={address.ToString()}&key={_configuration["GoogleMaps:Key"]}",
                TimeoutMilliseconds = Convert.ToInt32(_configuration["GoogleMaps:TimeoutMilliseconds"]),
                Method = "GET"
            };


            await service.ExecuteAsync();

            var response = JObject.Parse(service.Response);

            return new Models.N.Location.Location
            {
                Latitude = response["results"][0]["geometry"]["location"]["lat"].ToString(),
                Longitude = response["results"][0]["geometry"]["location"]["lng"].ToString()
            };
        }

        public async Task<string> GetMap(Models.N.Location.MapOptions options)
        {
            var service = new Core.N.Rest.RestServices
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

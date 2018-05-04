using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using BGBA.Models.N.Core.Trace;
using BGBA.Models.N.Location;
using BGBA.Services.N.Core.HttpClient;

namespace BGBA.Services.N.Location
{
    public class GoogleMapsServices : Models.N.Core.Trace.TraceServiceBase, IMapServices
    {
        private readonly IConfiguration _configuration;

        public GoogleMapsServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<GoogleMapsAddress> GetFullAddress(Address address)
        {
            var service = new HttpRequestFactory();
            var url = $"{_configuration["GoogleMaps:Url"]}address={address.ToString()}&key={_configuration["GoogleMaps:Key"]}";
            var isError = false;

            try
            {
                return (await service.Get(url)).ContentAsType<GoogleMapsAddress>();
            }
            catch (Exception)
            {
                isError = true;
                throw;
            }
            finally
            {
                this.Communicator_TraceHandler(this,
                    new TraceEventArgs
                    {
                        Description = "Get full address.",
                        ElapsedTime = service.ElapsedTime,
                        ForceDebug = false,
                        IsError = isError,
                        Request = service.Request,
                        Response = service.Response,
                        URL = url
                    });
            }
        }

        public async Task<string> GetUrlMap(Models.N.Location.MapOptions options)
        {
            var service = new HttpRequestFactory();
            var url = $"{_configuration["GoogleMaps:UrlMap"].Replace("{key}", _configuration["GoogleMaps:Key"])}&{options.ToString()}";
            var isError = false;

            try
            {
                return (await service.Get(url)).ContentAsString();
            }
            catch (Exception)
            {
                isError = true;
                throw;
            }
            finally
            {
                this.Communicator_TraceHandler(this,
                    new TraceEventArgs
                    {
                        Description = "Get url map address.",
                        ElapsedTime = service.ElapsedTime,
                        ForceDebug = false,
                        IsError = isError,
                        Request = service.Request,
                        Response = service.Response,
                        URL = url
                    });
            }
        }
    }
}

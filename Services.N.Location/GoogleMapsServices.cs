using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using BGBA.Models.N.Core.Trace;
using BGBA.Models.N.Location;
using BGBA.Services.N.Core.HttpClient;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BGBA.Services.N.ATReference;
using BGBA.Models.N.Core.Utils.Extensions;

namespace BGBA.Services.N.Location
{
    public class GoogleMapsServices : Models.N.Core.Trace.TraceServiceBase, IMapServices
    {
        private readonly TableHelper _tableHelper;
        private readonly IConfiguration _configuration;

        public GoogleMapsServices(IConfiguration configuration, TableHelper tableHelper)
        {
            _tableHelper = tableHelper;
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

        public async Task<GoogleMapsAddress> GetFullAddress(string placeId)
        {
            var service = new HttpRequestFactory();
            var url = $"{_configuration["GoogleMaps:Url"]}place_id={placeId}&key={_configuration["GoogleMaps:Key"]}";
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

        public string GetUrlMap(Models.N.Location.MapOptions options)
        {
            return $"{_configuration["GoogleMaps:UrlMap"].Replace("{key}", _configuration["GoogleMaps:Key"])}&{options.ToString()}";
        }

        public async Task<Models.N.Location.PredictionsResult> GetPrediction(Models.N.Location.MapOptions options)
        {
            var service = new HttpRequestFactory();
            var url = _configuration["GoogleMaps:Places:Url"]
                .Replace("{key}",_configuration["GoogleMaps:Key"])
                .Replace("{types}", _configuration["GoogleMaps:Places:Types"])
                .Replace("{input}",options.Address.ToString());
            
            if (options.Location != null)
            {
                url += $"&location={options.Location.ToString()}&radius={_configuration["GoogleMaps:Places:Radius"]}&strictbounds";
            }


            var isError = false;

            try
            {
                return (await service.Get(url)).ContentAsType<PredictionsResult>();
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
                        Description = "Get predictions.",
                        ElapsedTime = service.ElapsedTime,
                        ForceDebug = false,
                        IsError = isError,
                        Request = service.Request,
                        Response = service.Response,
                        URL = url
                    });
            }
        }

        public async Task<bool> NormalizeAddress(BGBA.Models.N.Location.MapOptions mapOptions, GoogleMapsAddress mapAddress)
        {

            var firstCoincidence = mapAddress.Results.FirstOrDefault().AddressComponents;

            mapOptions.Address.LocalityDescription = firstCoincidence.FirstOrDefault(a => a.Types.Any(t => BGBA.Models.N.Location.GoogleMapsAddress.LOCALITY_SUBLOCALITY.Contains(t)))?.ShortName ?? mapOptions.Address.LocalityDescription;
            mapOptions.Address.Number = firstCoincidence.FirstOrDefault(a => a.Types.Any(t => BGBA.Models.N.Location.GoogleMapsAddress.STREET_NUMBER.Contains(t)))?.LongName ?? mapOptions.Address.Number;
            mapOptions.Address.Street = firstCoincidence.FirstOrDefault(a => a.Types.Any(t => BGBA.Models.N.Location.GoogleMapsAddress.STREET.Contains(t)))?.LongName ?? mapOptions.Address.Street;

            var provinces = await _tableHelper.GetProvincesAsync();
            var provinceName = firstCoincidence.FirstOrDefault(a => a.Types.Any(t => BGBA.Models.N.Location.GoogleMapsAddress.PROVINCE.Contains(t)))?.ShortName.RemoveDiacritics();

            if (provinceName == "CABA")
            {
                provinceName = "CAPITAL FEDERAL";
                mapOptions.Address.LocalityDescription = "CIUDAD AUTONOMA BUENOS AI";
            }

            mapOptions.Address.Province = provinces.FirstOrDefault(p => p.Name.ToLower() == provinceName.ToLower()) ?? mapOptions.Address.Province;

            var country = firstCoincidence.FirstOrDefault(a => a.Types.Any(t => BGBA.Models.N.Location.GoogleMapsAddress.COUNTRY.Contains(t)))?.LongName;
            mapOptions.Address.Country = (await _tableHelper.GetCountriesAsync()).FirstOrDefault(c => c.Description.ToLower() == country.ToLower()) ?? mapOptions.Address.Country;



            var cpGoogle = $"{firstCoincidence.FirstOrDefault(a => a.Types.Any(t => BGBA.Models.N.Location.GoogleMapsAddress.POSTAL_CODE.Contains(t)))?.LongName}{firstCoincidence.FirstOrDefault(a => a.Types.Any(t => BGBA.Models.N.Location.GoogleMapsAddress.POSTAL_CODE_SUFFIX.Contains(t)))?.LongName}";

            if (!string.IsNullOrEmpty(cpGoogle) && cpGoogle.Length < 8)
            {
                cpGoogle = Regex.Replace(cpGoogle, "[a-z]*", "", RegexOptions.IgnoreCase).Trim();

                if (cpGoogle.Length == 4)
                    mapOptions.Address.PostalCode = cpGoogle;
            }
            else if (!string.IsNullOrEmpty(cpGoogle) && cpGoogle.Length == 8)
                mapOptions.Address.PostalCode = cpGoogle;
            else if (string.IsNullOrWhiteSpace(mapOptions.Address.PostalCode))
            {
                var cps = await _tableHelper.GetLocalitiesByProvinceWithCPAsync(mapOptions.Address.Province);
                var localityCP = cps.Where(l => l.Name == mapOptions.Address.LocalityDescription || l.Name.Contains(mapOptions.Address.LocalityDescription) || mapOptions.Address.LocalityDescription.Contains(l.Name)).ToList();
                if (localityCP.Count > 0)
                    mapOptions.Address.PostalCodeOcurrencies = localityCP.Select(c => c.PostalCode).ToList();
                else
                    mapOptions.Address.PostalCodeOcurrencies = cps.Select(c => c.PostalCode).ToList();
            }

            mapOptions.Address.Location = mapAddress.Results.FirstOrDefault()?.Geometry.Location;
            mapOptions.Location = mapOptions.Address.Location;
            mapOptions.LocationIsCoord = true;

            mapOptions.Address.UrlMap = GetUrlMap(mapOptions);

            return true;
        }

        public async Task<Models.N.Location.Address> NormalizeAddress(GoogleMapsAddress mapAddress)
        {

            var firstCoincidence = mapAddress.Results.FirstOrDefault().AddressComponents;
            var address = new Models.N.Location.Address();

            address.LocalityDescription = firstCoincidence.FirstOrDefault(a => a.Types.Any(t => BGBA.Models.N.Location.GoogleMapsAddress.LOCALITY_SUBLOCALITY.Contains(t)))?.ShortName;
            address.Number = firstCoincidence.FirstOrDefault(a => a.Types.Any(t => BGBA.Models.N.Location.GoogleMapsAddress.STREET_NUMBER.Contains(t)))?.LongName;
            address.Street = firstCoincidence.FirstOrDefault(a => a.Types.Any(t => BGBA.Models.N.Location.GoogleMapsAddress.STREET.Contains(t)))?.LongName;

            var provinces = await _tableHelper.GetProvincesAsync();
            var provinceName = firstCoincidence.FirstOrDefault(a => a.Types.Any(t => BGBA.Models.N.Location.GoogleMapsAddress.PROVINCE.Contains(t)))?.ShortName.RemoveDiacritics();

            if (provinceName == "CABA")
            {
                provinceName = "CAPITAL FEDERAL";
                address.LocalityDescription = "CIUDAD AUTONOMA BUENOS AI";
            }

            address.Province = provinces.FirstOrDefault(p => p.Name.ToLower() == provinceName.ToLower());

            var country = firstCoincidence.FirstOrDefault(a => a.Types.Any(t => BGBA.Models.N.Location.GoogleMapsAddress.COUNTRY.Contains(t)))?.LongName;
            address.Country = (await _tableHelper.GetCountriesAsync()).FirstOrDefault(c => c.Description.ToLower() == country.ToLower());

            var cpGoogle = $"{firstCoincidence.FirstOrDefault(a => a.Types.Any(t => BGBA.Models.N.Location.GoogleMapsAddress.POSTAL_CODE.Contains(t)))?.LongName}{firstCoincidence.FirstOrDefault(a => a.Types.Any(t => BGBA.Models.N.Location.GoogleMapsAddress.POSTAL_CODE_SUFFIX.Contains(t)))?.LongName}";

            if (!string.IsNullOrEmpty(cpGoogle) && cpGoogle.Length < 8)
            {
                cpGoogle = Regex.Replace(cpGoogle, "[a-z]*", "", RegexOptions.IgnoreCase).Trim();

                if (cpGoogle.Length == 4)
                    address.PostalCode = cpGoogle;
            }
            else if (!string.IsNullOrEmpty(cpGoogle) && cpGoogle.Length == 8)
                address.PostalCode = cpGoogle;
            else if (string.IsNullOrWhiteSpace(address.PostalCode))
            {
                var cps = await _tableHelper.GetLocalitiesByProvinceWithCPAsync(address.Province);
                var localityCP = cps.Where(l => l.Name == address.LocalityDescription || l.Name.Contains(address.LocalityDescription) || address.LocalityDescription.Contains(l.Name)).ToList();
                if (localityCP.Count > 0)
                    address.PostalCodeOcurrencies = localityCP.Select(c => c.PostalCode).ToList();
                else
                    address.PostalCodeOcurrencies = cps.Select(c => c.PostalCode).ToList();
            }

            address.Location = mapAddress.Results.FirstOrDefault()?.Geometry.Location;

            var mapOptions = new MapOptions
            {
                Address = address
            };

            address.UrlMap = GetUrlMap(mapOptions);

            return address;
        }
    }
}

using System.Collections.Generic;
using Newtonsoft.Json;

namespace BGBA.Models.N.Location
{
    public class GoogleMapsAddress
    {

        public static readonly string[] STREET_NUMBER = { "street_number" };
        public static readonly string[] STREET = { "route" };
        public static readonly string[] PROVINCE = { "administrative_area_level_1" };
        public static readonly string[] LOCALITY_SUBLOCALITY = { "locality", "sublocality" }; //PROVINCIA, CAPITAL
        public static readonly string[] POSTAL_CODE = { "postal_code" };
        public static readonly string[] POSTAL_CODE_SUFFIX = { "postal_code_suffix" };
        public static readonly string[] COUNTRY = { "country" };

        [JsonProperty(PropertyName = "results")]
        public IEnumerable<GoogleMapsAddressResult> Results { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

    }
    public class GoogleMapsAddressResult
    {
        [JsonProperty(PropertyName = "address_components")]
        public IEnumerable<AddressComponent> AddressComponents { get; set; }
        [JsonProperty(PropertyName = "formatted_address")]
        public string FormattedAddress { get; set; }
        [JsonProperty(PropertyName = "geometry")]
        public Geometry Geometry { get; set; }
        [JsonProperty(PropertyName = "place_id")]
        public string PlaceId { get; set; }
        [JsonProperty(PropertyName = "types")]
        public IEnumerable<string> Types { get; set; }

    }

    public class Geometry
    {
        [JsonProperty(PropertyName = "location")]
        public Location Location { get; set; }
        [JsonProperty(PropertyName = "location_type")]
        public string LocationType { get; set; }
    }
}

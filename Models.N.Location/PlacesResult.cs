using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace BGBA.Models.N.Location
{
    public class PlacesResult
    {
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
        [JsonProperty(PropertyName = "place_id")]
        public string PlaceId { get; set; }
    }
}

using System;
using Newtonsoft.Json;

namespace Models.N.Location
{
    public class Location
    {
        [JsonProperty(PropertyName = "lat")]
        public string Latitude { get; set; }
        [JsonProperty(PropertyName = "lng")]
        public string Longitude { get; set; }

        public override string ToString()
        {
            return $"{Latitude},{Longitude}";
        }
    }
}

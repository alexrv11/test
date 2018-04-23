using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Models.N.Location
{
    public class Location
    {
        [Required]
        [JsonProperty(PropertyName = "lat")]
        public string Latitude { get; set; }

        [Required]
        [JsonProperty(PropertyName = "lng")]
        public string Longitude { get; set; }

        public override string ToString()
        {
            return $"{Latitude},{Longitude}";
        }
    }
}

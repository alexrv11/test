﻿using Newtonsoft.Json;

namespace BGBA.Models.N.Location
{
    public class CountryATReference
    {
        [JsonProperty(PropertyName = "COD_PAIS")]
        public string Code { get; set; }
        [JsonProperty(PropertyName = "NOM_PAIS")]
        public string Description { get; set; }
        [JsonProperty(PropertyName = "COD_ESTADO")]
        public string State { get; set; }
    }
}
    
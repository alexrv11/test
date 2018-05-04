﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace BGBA.Models.N.Location
{
    public class AddressComponent
    {
        [JsonProperty(PropertyName = "long_name")]
        public string LongName { get; set; }
        [JsonProperty(PropertyName = "short_name")]
        public string ShortName { get; set; }
        [JsonProperty(PropertyName = "types")]
        public IEnumerable<string> Types { get; set; }
    }
}

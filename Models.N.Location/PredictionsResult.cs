using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace BGBA.Models.N.Location
{
    public class PredictionsResult
    {
        [JsonProperty(PropertyName = "predictions")]
        public List<PlacesResult> Results { get; set; }
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
    }
}

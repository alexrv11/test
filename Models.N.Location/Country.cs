using Newtonsoft.Json;

namespace Models.N.Location
{
    public class Country
    {
        [JsonProperty(PropertyName = "COD_PAIS")]
        public string Code { get; set; }
        [JsonProperty(PropertyName = "NOM_PAIS")]
        public string Description { get; set; }
        [JsonProperty(PropertyName = "COD_ESTADO")]
        public string State { get; set; }
    }
}
    
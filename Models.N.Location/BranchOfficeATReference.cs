using Newtonsoft.Json;

namespace Models.N.Location
{
    public class BranchOfficeATReference
    {
        [JsonProperty(PropertyName = "SUCURSAL")]
        public string Code { get; set; }
        [JsonProperty(PropertyName = "NOMBRE")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "DOMICILIO")]
        public string Address { get; set; }
        [JsonProperty(PropertyName = "COD_LOCALIDAD")]
        public string LocalityCode { get; set; }
    }
}
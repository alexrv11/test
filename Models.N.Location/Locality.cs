using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Models.N.Location
{
    public class Locality : ILocality
    {
        [JsonProperty(PropertyName = "COD_LOCALIDAD")]
        public string LocalityCode { get; set; }
        [JsonProperty(PropertyName = "COD_PROVINCIA")]
        public string ProvinceCode{ get; set; }
        [JsonProperty(PropertyName = "COD_ESTADO")]
        public string State { get; set; }
        [JsonProperty(PropertyName = "NOM_LOCALIDAD")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "COD_POSTAL_VIEJO")]
        public string PostalCode { get; set; }
        public List<BranchOffice> BranchOffices { get; set; }
    }
}
    
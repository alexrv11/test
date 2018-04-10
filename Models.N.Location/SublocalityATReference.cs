using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Models.N.Location
{
    public class SublocalityATReference : ILocality
    {
        [JsonProperty(PropertyName = "TXT_BARRIO")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "COD_SUCURSAL")] 
        public string BranchCode {get; set;}
        public List<BranchOffice> BranchOffices { get; set; }
    }
}
    
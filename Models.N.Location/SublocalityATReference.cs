using System.Collections.Generic;
using Newtonsoft.Json;

namespace BGBA.Models.N.Location
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
    
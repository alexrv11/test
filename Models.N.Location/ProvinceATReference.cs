using System;
using Newtonsoft.Json;

namespace Models.N.Location
{
    public class ProvinceATReference
    {
        [JsonProperty(PropertyName = "COD_PROVINCIA")]
        public string Code { get; set; }//Numeric
        [JsonProperty(PropertyName = "COD_IDENT_PROV")]
        public string Id { get; set; }//Alphabetic
        [JsonProperty(PropertyName = "NOM_PROVINCIA")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "FE_ALTA")]
        public DateTime EntryDate { get; set; }
        [JsonProperty(PropertyName = "FE_ULT_ACT")]
        public DateTime LastUpdate { get; set; }
        [JsonProperty(PropertyName = "COD_ESTADO")]
        public string State { get; set; }
        [JsonProperty(PropertyName = "GRPO_SCORING")]
        public int ScoringGroup { get; set; }
    }
}

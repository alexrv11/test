using Newtonsoft.Json;

namespace BGBA.Models.N.Location
{
    public class Sucursal
    {
        [JsonProperty("Suc_N_")]
        public string Numero { get; set; }
        [JsonProperty("Denominacion")]
        public string Denominacion { get; set; }
        [JsonProperty("NUEVA_ZONA__Octubre_2017_")]
        public string NuevaZona { get; set; }
        [JsonProperty("REGION_DR")]
        public string Region { get; set; }
        [JsonProperty("Direccion")]
        public string Direccion { get; set; }
        [JsonProperty("Localidad")]
        public string Localidad { get; set; }
        [JsonProperty("Provincia")]
        public string Provincia { get; set; }
        [JsonProperty("Y")]
        public string Latitud { get; set; }
        [JsonProperty("X")]
        public string Longitud { get; set; }
        [JsonProperty("CP")]
        public string CP { get; set; }
    }
}

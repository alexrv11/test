using Newtonsoft.Json;

namespace BGBA.Services.N.Adhesion.Models.SoapCallAdhesionBancaAutomaticaRequest
{
    public class Request
    {
        [JsonProperty("soapenv:Envelope")]
        public Envelope Envelope { get; set; }
    }

    public partial class Envelope
    {
        [JsonProperty("@xmlns:soapenv")]
        public const string SoapEnv = "http://schemas.xmlsoap.org/soap/envelope/";
        [JsonProperty("@xmlns:xsi")]
        public const string xsi = "http://www.w3.org/2001/XMLSchema-instance";
        [JsonProperty("@xmlns:xsd")]
        public const string xsd = "http://www.w3.org/2001/XMLSchema";
        [JsonProperty("soapenv:Header")]
        public const string Header = null;
        [JsonProperty("soapenv:Body")]
        public Body Body { get; set; }
    }

    public partial class Body
    {
        [JsonProperty("AdherirClienteFisicoProductoBancaAutomatica")]
        public AdherirClienteFisicoProductoBancaAutomatica AdherirClienteFisicoProductoBancaAutomatica { get; set; }
    }

    public partial class AdherirClienteFisicoProductoBancaAutomatica
    {
        [JsonProperty("@xmlns")]
        public const string xmlns = "http://ws.bancogalicia.com.ar/webservices/canalesexternos/accionesadhesionbancaautomatica/adherirclientefisicoproductobancaautomaticarequest/1_0_0";
        [JsonProperty("AdherirClienteFisicoProductoBancaAutomaticaRequest")]

        public AccionesAdhesionBancaAutomatica.AdherirClienteFisicoProductoBancaAutomaticaRequest AdherirClienteFisicoProductoBancaAutomaticaRequest { get; set; }
    }
}

namespace BGBA.Services.N.Adhesion.Models.SoapCallAdhesionBancaAutomaticaResponse
{
    public partial class Response
    {
        public Envelope Envelope { get; set; }
    }

    public partial class Envelope
    {
        public Body Body { get; set; }
    }

    public partial class Body
    {
        public AdherirClienteFisicoProductoBancaAutomaticaResult AdherirClienteFisicoProductoBancaAutomaticaResult { get; set; }
    }

    public partial class AdherirClienteFisicoProductoBancaAutomaticaResult
    {
        public AccionesAdhesionBancaAutomatica.AdherirClienteFisicoProductoBancaAutomaticaResponse AdherirClienteFisicoProductoBancaAutomaticaResponse { get; set; }
    }
}

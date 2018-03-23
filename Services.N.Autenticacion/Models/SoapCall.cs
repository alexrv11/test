using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Services.N.Autenticacion.Models.SoapCallRequest
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
        [JsonProperty("@xmlns:_0")]
        public const string _0 = "http://ws.bancogalicia.com.ar/webservices/accionesseguridadomnichannel/generarsemillarequest/1_0_0";
        [JsonProperty("@xmlns:_01")]
        public const string _01 = "http://ws.bancogalicia.com.ar/webservices/globales/bgbaheader/2_0_0";
        [JsonProperty("soapenv:Header")]
        public const string Header = null;
        [JsonProperty("soapenv:Body")]
        public Body Body { get; set; }
    }

    public partial class Body
    {
        [JsonProperty("_0:GenerarSemilla")]
        public GenerarSemilla GenerarSemilla { get; set; }
    }

    public partial class GenerarSemilla
    {
        [JsonProperty("_0:GenerarSemillaRequest")]
        public Models.AccionesSeguridadOmnichannel.GenerarSemillaRequest GenerarSemillaRequest { get; set; }
    }
}

namespace Services.N.Autenticacion.Models.SoapCallResponse
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
        public GenerarSemillaResult GenerarSemillaResult { get; set; }
    }

    public partial class GenerarSemillaResult
    {
        public Models.AccionesSeguridadOmnichannel.GenerarSemillaResponse GenerarSemillaResponse { get; set; }
    }
}

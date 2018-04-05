using Models.N.Afip.AutenticacionAfip;

namespace Services.N.Afip.Models.SoapCallAutenticarAfipRequest
{
    public class Request
    {
    }

    public partial class Envelope
    {
    }

    public partial class Body
    {
    }
}

namespace Services.N.Afip.Models.SoapCallAutenticarAfipResponse
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
        public AutenticarYAutorizarConsumoWebserviceResult AutenticarYAutorizarConsumoWebserviceResult { get; set; }
    }

    public partial class AutenticarYAutorizarConsumoWebserviceResult
    {
        public AutenticarYAutorizarConsumoWebserviceResponse AutenticarYAutorizarConsumoWebserviceResponse { get; set; }
    }
}

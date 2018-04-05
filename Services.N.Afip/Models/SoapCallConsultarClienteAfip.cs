using Models.N.Afip.ConsultaClienteAfip;

namespace Services.N.Afip.Models.SoapCallConsultarClienteAfipRequest
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

namespace Services.N.Afip.Models.SoapCallConsultarClienteAfipResponse
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
        public getPersonaResponse getPersonaResponse { get; set; }
    }
}

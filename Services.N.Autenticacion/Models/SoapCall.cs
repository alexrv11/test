using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace BGBA.Services.N.Autenticacion.Models.SoapCallRequest
{
    public class Request
    {
        public Envelope Envelope { get; set; }
    }

    public partial class Envelope
    {
        public Body Body { get; set; }
    }

    public partial class Body
    {
        public GenerarSemilla GenerarSemilla { get; set; }
    }

    public partial class GenerarSemilla
    {
        public Models.AccionesSeguridadOmnichannel.GenerarSemillaRequest GenerarSemillaRequest { get; set; }
    }
}

namespace BGBA.Services.N.Autenticacion.Models.SoapCallResponse
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

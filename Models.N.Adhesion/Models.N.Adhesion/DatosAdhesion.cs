using System;
using System.Collections.Generic;

namespace Models.N.Adhesion
{
    public class DatosAdhesion
    {
        public string IdHost { get; set; }
        public string IdAdhesion { get; set; }
        public string UsuarioAlfanumerico { get; set; }
        public string TipoDocumento { get; set; }
        public string NroDocumento { get; set; }
        public string Pin { get; set; }
        public string PinEncriptado { get; set; }
        public List<ProductoAdherible> ProductosAdheribles { get; set; }
    }
}

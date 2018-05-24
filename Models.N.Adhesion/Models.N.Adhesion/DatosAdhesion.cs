using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Models.N.Core.Attibutes;

namespace BGBA.Models.N.Adhesion
{
    public class DatosAdhesion
    {
        [Required]
        public string IdHost { get; set; }
        public string IdAdhesion { get; set; }
        [Required]
        public string UsuarioAlfanumerico { get; set; }
        [Required]
        public string TipoDocumento { get; set; }
        [Required]
        public string NroDocumento { get; set; }
        [Required]
        public string Pin { get; set; }
        public string PinEncriptado { get; set; }
        public List<ProductoAdherible> ProductosAdheribles { get; set; }
    }
}

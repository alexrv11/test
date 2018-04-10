using System.ComponentModel.DataAnnotations;
using Models.N.Location;

namespace MS.N.Consulta.Cliente.ViewModels
{
    public class ConsultaClienteVM
    {
        [Required]
        public string DU { get; set; }
        [Required]
        public Sexo Sexo { get; set; }
        public MapOptions MapOptions { get; set; }
    }

    public enum Sexo {
        Male,
        Female
    }
}

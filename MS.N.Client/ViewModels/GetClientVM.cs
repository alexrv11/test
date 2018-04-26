using System.ComponentModel.DataAnnotations;
using Models.N.Location;

namespace MS.N.Client.ViewModels
{
    public class GetClientVM
    {
        [Required]
        public string DU { get; set; }
        [Required]
        public Sex Sexo { get; set; }
        public MapOptions MapOptions { get; set; }
    }

    public enum Sex {
        Male,
        Female
    }
}

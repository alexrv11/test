using System.ComponentModel.DataAnnotations;
using BGBA.Models.N.Location;

namespace BGBA.MS.N.Client.ViewModels
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

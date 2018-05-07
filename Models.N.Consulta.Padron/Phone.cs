using System.ComponentModel.DataAnnotations;

namespace BGBA.Models.N.Client
{
    public class Phone
    {
        [Required]
        public string AreaNumber { get; set; }
        [Required]
        public string Number { get; set; }
        public bool IsCellphone { get; set; }
        public string PhoneType { get; set; }
    }
}
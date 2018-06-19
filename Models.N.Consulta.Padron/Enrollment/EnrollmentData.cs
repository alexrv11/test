using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BGBA.Models.N.Client.Enrollment
{
    public class EnrollmentData
    {
        [Required]
        public string IdHost { get; set; }
        public string EnrollNumber { get; set; }
        [Required]
        public string AlphanumericUser { get; set; }
        [Required]
        public string DocumentType { get; set; }
        [Required]
        public string DocumentNumber { get; set; }
        [Required]
        public string Pin { get; set; }
        public string EncryptedPin { get; set; }
        public List<EnrollmentProduct> EnrollmentProducts { get; set; }
    }
}

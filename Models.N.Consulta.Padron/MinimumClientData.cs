using System;
using System.ComponentModel.DataAnnotations;

namespace Models.N.Client
{
    public class MinimumClientData
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Sex { get; set; }
        [Required]
        public DateTime Birthdate { get; set; }
        [Required]
        public string DocumentNumber { get; set; }
        [Required]
        public string DocumentType { get; set; }
    }
}

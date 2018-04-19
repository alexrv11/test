using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Models.N.Location
{
    public class Country
    {
        [Required]
        public string Code { get; set; }
        public string Description { get; set; }
        public string State { get; set; }
    }
}
    
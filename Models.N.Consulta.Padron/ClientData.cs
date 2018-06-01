using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BGBA.Models.N.Client
{
    public class ClientData : MinimumClientData
    {
        public string CivilState { get; set; }
        public string PersonType {get;set;}
        [Required]
        public string CuixNumber { get; set; }
        [Required]
        public string CuixType { get; set; }
        [Required]
        public string CuixCode { get; set; }
        public string HostId { get; set; }
        public List<Location.Address> Addresses { get; set; }
        public List<Email> Emails { get; set; }
        public List<Phone> Phones { get; set; }
    }
}

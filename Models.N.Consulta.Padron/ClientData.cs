using System.Collections.Generic;

namespace BGBA.Models.N.Client
{
    public class ClientData : MinimumClientData
    {
        public string CivilState { get; set; }
        public string PersonType {get;set;}
        public string CuixNumber { get; set; }
        public string CuixType { get; set; }
        public string CuixCode { get; set; }
        public string HostId { get; set; }
        public List<Location.Address> Addresses { get; set; }
        public List<Email> Emails { get; set; }
        public List<Phone> Phones { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace Models.N.Client
{
    public class ClientData
    {
        public string PersonType {get;set;}
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Sex { get; set; }
        public string CivilState { get; set; }
        public DateTime Birthdate { get; set; }
        public string DocumentNumber { get; set; }
        public string DocumentType { get; set; }
        public string CuixNumber { get; set; }
        public string CuixType { get; set; }
        public string CuixCode { get; set; }
        public string HostId { get; set; }
        public List<Location.Address> Addresses { get; set; }
        public List<Email> Emails { get; set; }
        public List<Phone> Phones { get; set; }
    }
}

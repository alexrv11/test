using System;

namespace BGBA.Models.N.Client.Enrollment
{
    public class EnrolledClient
    {
        public string EnrollNumber { get; set; }
        public string HostId { get; set; }
        public DateTime BirthDate { get; set; }
        public string DocumentNumber { get; set; }
        public string DocumentType { get; set; }
        public string Sex { get; set; }
        public string CentralSystemSecurityCodeId { get; set; } 
        public EnrollState? State { get; set; }

    }

    public enum EnrollState
    {
        ADHERIDO,
        NOADHERIDO,
        BLOQUEADOCLAVEINEXISTENTESCS,
        BLOQUEADOINTENTOSFALLIDOS,
        ADHERIDOBLANQUEADO
    }
}

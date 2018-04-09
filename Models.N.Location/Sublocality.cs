using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Models.N.Location
{
    [DataContract]
    public class Sublocality : ILocality
    {
        [DataMember(Name = "TXT_BARRIO")]
        public string Name { get; set; }
        [DataMember(Name = "COD_SUCURSAL")] 
        public string BranchCode {get; set;}
        public List<BranchOffice> BranchOffices { get; set; }
    }
}
    
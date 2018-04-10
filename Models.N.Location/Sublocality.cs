using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Models.N.Location
{
    public class Sublocality : ILocality
    {
        public string Name { get; set; } 
        public string BranchCode {get; set;}
        public List<BranchOffice> BranchOffices { get; set; }
    }
}
    
using System.Collections.Generic;

namespace BGBA.Models.N.Location
{
    public class Sublocality : ILocality
    {
        public string Name { get; set; } 
        public string BranchCode {get; set;}
        public List<BranchOffice> BranchOffices { get; set; }
    }
}
    
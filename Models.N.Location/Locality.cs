using System.Collections.Generic;

namespace BGBA.Models.N.Location
{
    public class Locality : ILocality
    {
        public string LocalityCode { get; set; }
        public string ProvinceCode{ get; set; }
        public string State { get; set; }
        public string Name { get; set; }
        public string PostalCode { get; set; }
        public List<BranchOffice> BranchOffices { get; set; }
    }
}
    
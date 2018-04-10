﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Models.N.Location
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
    
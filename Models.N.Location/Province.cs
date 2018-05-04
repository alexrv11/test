using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace BGBA.Models.N.Location
{
    public class Province
    {
        [Required]
        public string Code { get; set; }//Numeric
        public string Id { get; set; }//Alphabetic
        public string Name { get; set; }
        public DateTime EntryDate { get; set; }
        public DateTime LastUpdate { get; set; }
        public string State { get; set; }
        public int ScoringGroup { get; set; }
    }
}

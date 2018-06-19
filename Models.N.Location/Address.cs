using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BGBA.Models.N.Location
{
    public class Address
    {
        [Required]
        public string Street { get; set; }
        [Required]
        public string Number { get; set; }
        public string Floor { get; set; }
        public string FlatNumber { get; set; }
        [Required]
        public string LocalityDescription { get; set; }
        public Province Province { get; set; }
        public Country Country { get; set; }
        public string PostalCode { get; set; }
        public string AddressType { get; set; }
        public string UrlMap { get; set; }
        [Required]
        public Location Location { get; set; }
        public string AditionalData { get; set; }
        public string AditionalDataType { get; set; }
        public bool Default { get; set; }
        public List<string>  PostalCodeOcurrencies { get; set; }
        public override string ToString()
        {
            return $"{this.Street ?? "null"} {this.Number ?? "null"} {this.LocalityDescription ?? "null"} {this.Province?.Name ?? "null"} {this.Country?.Description ?? "null"}".Replace(" null", "").Replace(" ", "+");
        }
    }
}
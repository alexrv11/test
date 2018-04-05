namespace Models.N.Location
{
    public class Address
    {
        public string Street { get; set; }
        public string Number { get; set; }
        public string Locality { get; set; }
        public string ProvinceDescription { get; set; }
        public string ProvinceCode { get; set; }
        public string CountryDescription { get; set; }
        public string CoutryCode { get; set; }
        public string PostalCode { get; set; }
        public string AddressType { get; set; }
        public string UrlMap { get; set; }
        public Location Location { get; set; }
        public override string ToString()
        {
            return $"{this.Street?.Replace(" ","+")}+{this.Number?.Replace(" ", "+")}+{this.PostalCode?.Replace(" ", "+")}+{this.Locality?.Replace(" ", "+")}+{this.ProvinceDescription?.Replace(" ", "+")}+{this.CountryDescription?.Replace(" ", "+")}";
        }
    }
}
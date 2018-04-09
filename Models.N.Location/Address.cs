namespace Models.N.Location
{
    public class Address
    {
        public string Street { get; set; }
        public string Number { get; set; }
        public string LocalityDescription { get; set; }
        public Province Province { get; set; }
        public Country Country { get; set; }
        public string CoutryCode { get; set; }
        public string PostalCode { get; set; }
        public string AddressType { get; set; }
        public string UrlMap { get; set; }
        public Location Location { get; set; }
        public override string ToString()
        {
            return $"{this.Street?.Replace(" ","+")}+{this.Number?.Replace(" ", "+")}+{this.LocalityDescription?.Replace(" ", "+")}+{this.Province?.Name?.Replace(" ", "+")}+{this.Country?.Description?.Replace(" ", "+")}";
        }
    }
}
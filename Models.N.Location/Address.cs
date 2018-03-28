namespace Models.N.Location
{
    public class Address
    {
        public string StreetAndNumber { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public override string ToString()
        {
            return $"{this.StreetAndNumber}+{this.PostalCode}+{this.City}+{this.Province}+{this.Country}";
        }
    }
}
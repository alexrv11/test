namespace Models.N.Location
{
    public class Address
    {
        public string Street { get; set; }
        public string Number { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public override string ToString()
        {
            return $"{this.Street}+{this.Number}+{this.City}+{this.Province}+{this.Country}";
        }
    }
}
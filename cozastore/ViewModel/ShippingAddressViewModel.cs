namespace cozastore.ViewModel
{
    public class ShippingAddressViewModel
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsDefault { get; set; }
    }
}

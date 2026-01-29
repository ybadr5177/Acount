using System.ComponentModel.DataAnnotations;

namespace cozastore.ViewModel.Ordes
{
    public class AddressViewModel
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }
        [Required]

        public string LastName { get; set; }
        [Required]

        public string Country { get; set; }
        [Required]

        public string City { get; set; }
        [Required]

        public string Street { get; set; }
      

        public string PhoneNumber { get; set; }
    }
}

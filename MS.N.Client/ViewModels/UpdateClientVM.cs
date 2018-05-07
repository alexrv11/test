using BGBA.Models.N.Location;
using BGBA.Models.N.Client;

namespace BGBA.MS.N.Client.ViewModels
{
    public class UpdateClientVM
    {
        public Address Address { get; set; }
        public string Email { get; set; }
        public MinimumClientData Client { get; set; }
        public Phone Phone { get; set; }
    }
}

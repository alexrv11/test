using System.ComponentModel.DataAnnotations;
using Models.N.Location;
using Models.N.Client;

namespace MS.N.Client.ViewModels
{
    public class UpdateClientVM
    {
        public Address Address { get; set; }
        public string Email { get; set; }
        public MinimumClientData Client { get; set; }
    }
}

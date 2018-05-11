using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BGBA.MS.N.Client.ViewModels
{
    public class SendEmailVM
    {
        [Required]
        public string Email { get; set; }
        public string AttachmentNameWithExtension { get; set; }
        public Dictionary<string, string> Data { get; set; }
    }
}

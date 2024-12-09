using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace InventarioPro.Server.Models
{
    public class ApplicationUser:IdentityUser
    {

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}

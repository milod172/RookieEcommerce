using Microsoft.AspNetCore.Identity;

namespace NovaFashion.API.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime CreatedDate { get; set; } 
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; } = true;
        public virtual ICollection<Orders> Orders { get; set; } = [];
    }
}

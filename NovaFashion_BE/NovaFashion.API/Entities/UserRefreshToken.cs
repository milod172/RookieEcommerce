using Microsoft.AspNetCore.Identity;

namespace NovaFashion.API.Entities
{
    public class UserRefreshToken : IdentityUserToken<string>
    {
        public DateTime ExpiresAt { get; set; }
      
    }
}

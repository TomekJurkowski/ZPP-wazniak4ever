using Microsoft.AspNet.Identity.EntityFramework;

namespace WazniakWebsite.Models.Authentication
{
    public class ApplicationUser : IdentityUser
    {
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("GenuineSampleContext")
        {
        }
    }
}

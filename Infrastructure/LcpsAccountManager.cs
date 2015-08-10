using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Lcps.Division.Directory.Repository;

namespace Lcps.Division.Directory.Infrastructure
{
    public class LcpsAccountManager : UserManager<DirectoryMember>
    {
        public LcpsAccountManager(IUserStore<DirectoryMember> store)
            : base(store)
        {
        }



        public static LcpsAccountManager Create(IdentityFactoryOptions<LcpsAccountManager> options, IOwinContext context)
        {
            var manager = new LcpsAccountManager(new UserStore<DirectoryMember>(context.Get<LcpsRepositoryContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<DirectoryMember>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<DirectoryMember>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }
}

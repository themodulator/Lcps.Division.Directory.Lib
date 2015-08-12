using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Lcps.Division.Directory.Repository;

namespace Lcps.Division.Directory.Infrastructure
{
    public class LcpsAccountManager : UserManager<DirectoryMemberInfo>
    {
        public LcpsAccountManager(IUserStore<DirectoryMemberInfo> store)
            : base(store)
        {
        }



        public static LcpsAccountManager Create(IdentityFactoryOptions<LcpsAccountManager> options, IOwinContext context)
        {
            var manager = new LcpsAccountManager(new UserStore<DirectoryMemberInfo>(context.Get<LcpsRepositoryContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<DirectoryMemberInfo>(manager)
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
                RequireLowercase = false,
                RequireUppercase = false,
            };
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<DirectoryMemberInfo>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }
}

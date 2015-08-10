using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;

using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

using Lcps.Division.Directory.Repository;

namespace Lcps.Division.Directory.Infrastructure
{
    public class LcpsRepositoryContext : IdentityDbContext<DirectoryMember>
    {
        public LcpsRepositoryContext()
            : base(Anvil.Repository.Properties.Settings.Default._repositoryConnectionString)
        {

        }

        public LcpsRepositoryContext(string connectionNameOrString)
            : base(connectionNameOrString)
        {
            Anvil.Repository.Properties.Settings.Default.RepositoryConnectionString = connectionNameOrString;
        }

        public static LcpsRepositoryContext Create()
        {
            return new LcpsRepositoryContext();
        }

        public DbSet<MembershipScope> MembershipScopes { get; set; }

        
    }
}

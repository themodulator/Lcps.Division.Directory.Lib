using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Lcps.Division.Directory.Infrastructure;
using Lcps.Division.Directory.Repository;

using Anvil.Repository;

namespace Lcps.Division.Directory.Repository
{
    public class MembershipFilterRepository : GenericRepository<MembershipFilter>
    {
        public MembershipFilterRepository()
            : base(new LcpsRepositoryContext())
        { }


        public static List<DirectoryMemberInfo> GetMembers(Guid filterId)
        {
            DirectoryContext context = new DirectoryContext();

            MembershipFilter filter = context.MembershipFilters.First(x => x.MembershipFilterKey.Equals(filterId));

            return DirectoryMember.GetByFilter(filter.MembershipScope);
        }



    }
}

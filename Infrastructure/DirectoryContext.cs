using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Anvil.Repository;
using Lcps.Division.Directory.Infrastructure;
using Lcps.Division.Directory.Repository;

namespace Lcps.Division.Directory.Infrastructure
{
    public class DirectoryContext
    {
        #region Fields

        private LcpsRepositoryContext _dbConext = new LcpsRepositoryContext();

        private MembershipScopeRepository _membershipScopes;

        private GenericRepository<DirectoryMemberInfo> _directoryMembers;

        private MembershipFilterRepository _membershipFilters;

        #endregion

        #region Properties

        public MembershipScopeRepository MembershipScopes
        {
            get
            {
                if (_membershipScopes == null)
                    _membershipScopes = new MembershipScopeRepository();

                return _membershipScopes;
            }
        }



        public GenericRepository<DirectoryMemberInfo> DirectoryMembers
        {
            get
            {
                if (_directoryMembers == null)
                    _directoryMembers = new GenericRepository<DirectoryMemberInfo>(_dbConext);

                return _directoryMembers;
            }

        }

        public MembershipFilterRepository MembershipFilters
        {
            get
            {
                if (_membershipFilters == null)
                    _membershipFilters = new MembershipFilterRepository();

                return _membershipFilters;
            }
        }

        #endregion

    }
}

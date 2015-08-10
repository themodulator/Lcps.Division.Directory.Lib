using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lcps.Division.Directory.Repository.Ldap
{
    public enum LdapMapProviders
    {
        None = 0,
        OU = 1,
        Group = 2,
        Home = 3
    }
}

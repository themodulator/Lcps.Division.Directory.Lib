using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lcps.Division.Directory.Repository
{
    public enum MembershipScopeQualifier
    {
        None = 0,
        Location = 1,
        Type = 2,
        Position = 4,
        Grade = 8
    }
}

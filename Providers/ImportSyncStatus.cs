using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lcps.Division.Directory.Providers
{
    public enum ImportSyncStatus
    {
        None = 0,
        Current = 1,
        Insert = 2,
        Update = 3,
        Error = 4
    }
}

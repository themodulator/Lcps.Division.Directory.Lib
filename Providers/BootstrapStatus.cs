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
    public enum BootstrapStatus
    {
        @default = 0,
        success = 1,
        info = 2,
        warning = 4,
        danger = 5
    }
}

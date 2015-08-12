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
    public enum DirectoryMemberClass
    {
        None = 0,
        External = 1,
        Staff = 2,
        Student = 4
    }
}

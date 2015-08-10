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
    public class MvcRouteDefinition
    {
        public string Area { get; set; }
        public string Controller { get; set; }
        public string Action { get; set;}
    }
}

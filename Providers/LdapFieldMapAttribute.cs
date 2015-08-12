﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lcps.Division.Directory.Providers
{
    public class LdapFieldMapAttribute : Attribute
    {
        public LdapFieldMapAttribute(string adsiFieldName)
        {
            ADSIFIeldName = ADSIFIeldName;
        }

        public string ADSIFIeldName { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lcps.Division.Directory.Repository
{
    [Table("MembershipFilter", Schema = "Lcps.Division")]
    public class MembershipFilter
    {
        [Key]
        public Guid MembershipFilterKey { get; set; }

        [Display(Name = "Schema")]
        public MembershipFilterSchema Schema { get; set; }

        [Display(Name = "Filter")]
        public long MembershipScope { get; set; }

        [Display(Name = "Caption")]
        [Index("IX_MembershipFilter_Caption", IsUnique = true)]
        public string Caption { get; set; }
    }
}

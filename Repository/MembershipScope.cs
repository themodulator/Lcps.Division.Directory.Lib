using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;


namespace Lcps.Division.Directory.Repository
{
    [Table("MembershipScope", Schema = "Lcps.Division")]
    public class MembershipScope
    {
        [Key]
        [Display(Name = "Key")]
        public Guid MembershipScopeId { get; set; }

        [Display(Name = "Value")]
        [Index("IX_MembershipScope_ID", IsUnique = true)]
        public long BinaryValue { get; set; }

        [Display(Name = "Caption")]
        [MaxLength(128)]
        [Index("IX_MembershipScope_Caption", IsUnique = true)]
        [Remote("CaptionExists", "MembershipScopes", "DirectorySetup", ErrorMessage="Either the caption or it's literal translation already exists")]
        public string Caption { get; set; }


        [Display(Name = "Qualifier")]
        public MembershipScopeQualifier ScopeQualifier { get; set; }

        [Display(Name = "Literal")]
        public string LiteralName { get; set; }
    }
}

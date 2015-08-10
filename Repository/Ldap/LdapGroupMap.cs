using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lcps.Division.Directory.Repository.Ldap
{
    [Table("LdapGroupMap", Schema = "Ldap")]
    public class LdapGroupMap
    {
        [Key]
        public Guid LdapGroupMapKey { get; set; }
        
        [Required]
        public long MembershipScope { get; set; }

        [Required]
        [MinLength(1024)]
        [Display(Name = "Maps To")]
        public string MapsTo { get; set; }

        [Display(Name = "Provider")]
        public LdapMapProviders Provider { get; set; }

    }
}

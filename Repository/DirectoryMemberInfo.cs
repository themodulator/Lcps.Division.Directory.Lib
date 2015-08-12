using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Anvil.Repository;
using Lcps.Division.Directory.Providers;
using System.Reflection;

namespace Lcps.Division.Directory.Repository
{
    [Table("DirectoryMember",Schema = "Lcps.Division")]
    public class DirectoryMemberInfo : IdentityUser, IDirectoryMember
    {
        public DirectoryMemberInfo()
        { }

        public DirectoryMemberInfo(IDirectoryMember directoryMember)
        {
            foreach (PropertyInfo p in typeof(IDirectoryMember).GetProperties())
            {
                object v = null;

                try
                {
                    v = p.GetValue(directoryMember, null);
                }
                catch(Exception ex)
                {
                    throw new Exception(String.Format("Could not get value for property {0}", p.Name), ex);
                }

                try
                {
                    p.SetValue(this, v, null);
                }
                catch(Exception ex)
                {
                    throw new Exception(String.Format("Could not set value for property {0}", p.Name), ex);
                }
            }
        }

        [Display(Name = "Division ID")]
        [MaxLength(128)]
        [Required]
        [Index("IX_User_InternalId", IsUnique = true)]
        public string InternalId { get; set; }

        [Index("IX_User_ID", IsUnique = true, Order = 1)]
        [Required]
        [MaxLength(75)]
        [Display(Name = "First")]
        public string GivenName { get; set; }

        [Index("IX_User_ID", IsUnique = true, Order = 2)]
        [MaxLength(75)]
        [Display(Name = "Middle")]
        public string MiddleName { get; set; }

        [Index("IX_User_ID", IsUnique = true, Order = 3)]
        [Required]
        [MaxLength(75)]
        [Display(Name = "Last")]
        public string SurName { get; set; }

        [Index("IX_User_ID", IsUnique = true, Order = 4)]
        [DataType(DataType.Date)]
        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DOB { get; set; }

        [Required]
        public Directory.Repository.DiurectoryMemberGender Gender { get; set; }

        [Index("IX_MembershipScope", IsUnique = false)]
        [Display(Name = "Scope")]
        public long MembershipScope { get; set; }

        [Display(Name = "Member Class")]
        public DirectoryMemberClass MemberClass { get; set; }

        [Display(Name = "Title")]
        [MaxLength(256)]
        public string Title { get; set; }

        [MaxLength(256)]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [Required]
        public string InitialPassword { get; set; }

        [MaxLength(256)]
        [Display(Name = "Confirm")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<DirectoryMemberInfo> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }
}

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

namespace Lcps.Division.Directory.Repository
{
    public class DirectoryMember : IdentityUser, IDirectoryMember
    {
        //<Guid("6B32BFFF-7A74-4036-AEA0-EE6A27E88FC2")>
        public const string GuidKey = "6B32BFFF-7A74-4036-AEA0-EE6A27E88FC2";

        [Display(Name = "Division ID")]
        [Index("IX_User_InternalId", IsUnique = true)]
        [MaxLength(128)]
        [Required]
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
        public long MembershipScope { get; set; }

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

        public string DecryptPassword()
        {
            try
            {
                Anvil.RijndaelEnhanced enc = new Anvil.RijndaelEnhanced(GuidKey);
                string pwd = enc.Decrypt(this.InitialPassword);
                return pwd;
            }
            catch (Exception ex)
            {
                Anvil.Repository.ExceptionCollector ec = new Anvil.Repository.ExceptionCollector(ex);
                return ec.ToUL();

            }
        }




        public List<MembershipScope> GetApplicableScopes()
        {
            return MembershipScopeRepository.GetApplicableScopes(this.MembershipScope).OrderBy(x => x.Caption).ToList();
        }

        public string GetMembershipScopeCaption()
        {
            return string.Join(", ", GetApplicableScopes().Select(x => x.Caption).ToArray());
        }

        public int GetCategoryCount()
        {
            int test = 0;

            if (GetLocations().Count() > 0) test++;
            if (GetTypes().Count() > 0) test++;
            if (GetPositions().Count() > 0) test++;
            if (GetGrades().Count() > 0) test++;

            return 12 / test;

        }


        public List<MembershipScope> GetLocations()
        {
            return GetByQualifier(MembershipScopeQualifier.Location);
        }

        public List<MembershipScope> GetTypes()
        {
            return GetByQualifier(MembershipScopeQualifier.Type);
        }

        public List<MembershipScope> GetPositions()
        {
            return GetByQualifier(MembershipScopeQualifier.Position);
        }

        public List<MembershipScope> GetGrades()
        {
            return GetByQualifier(MembershipScopeQualifier.Grade);
        }

        private List<MembershipScope> GetByQualifier(MembershipScopeQualifier q)
        {
            return GetApplicableScopes().Where(x => x.ScopeQualifier == q).ToList();
        }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<DirectoryMember> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }
}

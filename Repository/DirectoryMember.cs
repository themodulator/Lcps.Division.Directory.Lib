using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;

using Anvil.Repository;
using Lcps.Division.Directory.Infrastructure;
using Lcps.Division.Directory.Providers;

using System.Reflection;
using System.Data;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lcps.Division.Directory.Repository
{
    public class DirectoryMember : Importable<DirectoryMemberInfo>, IDirectoryMember
    {

        #region Constants

        public const string GuidKey = "6B32BFFF-7A74-4036-AEA0-EE6A27E88FC2";

        #endregion

        #region Constructors

        public DirectoryMember()
            : base(new GenericRepository<DirectoryMemberInfo>(new LcpsRepositoryContext()))
        {

        }

        public DirectoryMember(IDirectoryMember directoryMember)
            : base(new GenericRepository<DirectoryMemberInfo>(new LcpsRepositoryContext()))
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
                    Exception = new ExceptionCollector(new Exception(String.Format("Could not get value for property {0}", p.Name), ex));
                    ImportStatus = BootstrapStatus.danger;
                    return;
                }

                try
                {
                    p.SetValue(this, v, null);
                }
                catch(Exception ex)
                {
                    Exception = new ExceptionCollector(new Exception(String.Format("Could not set value for property {0}", p.Name), ex));
                    SyncStatus = ImportSyncStatus.Error;
                    return;

                }
            }
        }

        public DirectoryMember(DataRow row)
            : base(new GenericRepository<DirectoryMemberInfo>(new LcpsRepositoryContext()))
        {
            string[] names = (from DataColumn x in row.Table.Columns
                              select x.ColumnName.ToLower()).ToArray();

            foreach (PropertyInfo p in typeof(IDirectoryMember).GetProperties())
            {
                if (names.Contains(p.Name.ToLower()))
                {
                    object v = row[p.Name];

                    try
                    {
                        if (p.PropertyType.IsEnum)
                            v = System.Enum.Parse(p.PropertyType, v.ToString());

                        p.SetValue(this, Convert.ChangeType(v, p.PropertyType), null);
                    }
                    catch (Exception ex)
                    {
                        Exception = new ExceptionCollector(new Exception(String.Format("Could not set value for property {0}", p.Name), ex));
                        SyncStatus = ImportSyncStatus.Error;
                        return;
                    }
                }
            }
        }

        #endregion

        #region Properties

        public LcpsAccountManager UserManager { get; set; }

        public string Id { get; set; }

        [Display(Name = "Division ID")]
        [MaxLength(128)]
        [Required]
        public string InternalId { get; set; }

        [ImportCompareField]
        [Required]
        [MaxLength(75)]
        [Display(Name = "First")]
        public string GivenName { get; set; }

        [ImportCompareField]
        [MaxLength(75)]
        [Display(Name = "Middle")]
        public string MiddleName { get; set; }

        [ImportCompareField]
        [Required]
        [MaxLength(75)]
        [Display(Name = "Last")]
        public string SurName { get; set; }

        [ImportCompareField]
        [DataType(DataType.Date)]
        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DOB { get; set; }

        [ImportCompareField]
        public Directory.Repository.DiurectoryMemberGender Gender { get; set; }

        [ImportCompareField]
        [Display(Name = "Scope")]
        public long MembershipScope { get; set; }

        [ImportCompareField]
        public string Title { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [Required]
        public string InitialPassword { get; set; }

        [MaxLength(256)]
        [Display(Name = "Confirm")]
        [DataType(DataType.Password)]
        public DirectoryMemberClass MemberClass { get; set; }

        [Display(Name = "Name")]
        public string SortName
        {
            get
            {
                return SurName + ", " + GivenName;
            }
        }

        #endregion

        #region Password

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

        #endregion

        #region Scope

        public List<MembershipScope> GetApplicableScopes()
        {
            return MembershipScopeRepository.GetApplicableScopes(this.MembershipScope).OrderBy(x => x.Caption).ToList();
        }

        public string GetMembershipScopeCaption()
        {
            return string.Join(", ", GetApplicableScopes().OrderBy(x => x.ScopeQualifier).Select(x => x.Caption).ToArray());
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

        public static List<DirectoryMemberInfo> GetOrFilter(long filter)
        {
            DirectoryContext context = new DirectoryContext();

            List<MembershipScope> scopes = MembershipScopeRepository.GetApplicableScopes(filter);

            List<DirectoryMemberInfo> members = new List<DirectoryMemberInfo>();

            foreach (MembershipScope scope in scopes)
            {
                DirectoryMemberInfo[] mm = context.DirectoryMembers.Get(x => (x.MembershipScope & scope.BinaryValue) == scope.BinaryValue).OrderBy(x => x.SurName + x.GivenName).ToArray();
                members.AddRange(mm);
            }

            return members.OrderBy(x => x.SurName + x.GivenName).ToList();

        }

        public static List<DirectoryMemberInfo> GetByFilter(long filter)
        {
            DirectoryContext context = new DirectoryContext();

            return context.DirectoryMembers.Get(x => ((x.MembershipScope & filter) == x.MembershipScope) | ((x.MembershipScope & filter) == filter)).OrderBy(x => x.SurName + x.GivenName).ToList();

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

        #endregion

        #region Import

        public override DirectoryMemberInfo GetItem()
        {
            return Repository.First(x => x.InternalId.ToLower().Equals(this.InternalId.ToLower()));
        }

        public override void GetSyncStatus()
        {
            if (SyncStatus == ImportSyncStatus.Error)
                return;

            if (this.InternalId.Equals("401073"))
            {
                int x = 0;
                x++;
            }

            SyncFields = new List<string>();
            try
            {
                IDirectoryMember member = GetItem();

                if (member == null)
                    SyncStatus = ImportSyncStatus.Insert;
                else
                {
                    PropertyInfo[] pp = this.GetType().GetProperties()
                        .Where(x => x.GetCustomAttributes(false)
                            .Any(a => a.GetType() == typeof(ImportCompareFieldAttribute)))
                            .ToArray();

                    foreach (PropertyInfo p in pp)
                    {
                        PropertyInfo pt = member.GetType().GetProperty(p.Name);

                        object sv = pt.GetValue(member, null);
                        object tv = p.GetValue(this, null);

                        if(sv == null & tv != null)
                            SyncFields.Add(p.Name);

                        if(sv != null)
                        {
                            if (!sv.Equals(tv))
                                SyncFields.Add(p.Name);
                        }
                    }

                    if (SyncFields.Count() > 0)
                        SyncStatus = ImportSyncStatus.Update;
                    else
                        SyncStatus = ImportSyncStatus.Current;

                }
            }
            catch (Exception ex)
            {
                Exception = new ExceptionCollector(ex);
                SyncStatus = ImportSyncStatus.Error;
            }

        }

        public override void Insert()
        {
            try
            {
                if (UserManager == null)
                    throw new Exception("Insert failed. The user manager has not been initialized");

                string pwd = this.InitialPassword;

                Anvil.RijndaelEnhanced re = new Anvil.RijndaelEnhanced(DirectoryMember.GuidKey);
                this.InitialPassword = re.Encrypt(pwd);

                DirectoryMemberInfo d = new DirectoryMemberInfo(this);
                d.Id = Guid.NewGuid().ToString();

                IdentityResult r = UserManager.Create(d, pwd);
                if (r.Succeeded)
                    ImportStatus = BootstrapStatus.success;
                else
                {
                    ExceptionCollector ec = new ExceptionCollector();
                    ec.AddRange(r.Errors.ToArray());
                    Exception = ec;
                    ImportStatus = BootstrapStatus.danger;
                }


                
            }
            catch(Exception ex)
            {
                Exception = new ExceptionCollector(ex);
                ImportStatus = BootstrapStatus.danger;
            }
        }

        public override void Update()
        {
            DirectoryMemberInfo i = this.GetItem();
            
            foreach(string n in SyncFields)
            {
                PropertyInfo p = this.GetType().GetProperty(n);
                object v = null;

                try
                {
                    v = p.GetValue(this, null);
                }
                catch (Exception ex)
                {
                    Exception = new ExceptionCollector(new Exception(string.Format("Could not get property {0}", p.Name), ex));
                    ImportStatus = BootstrapStatus.danger;
                    return;
                }

                try
                {
                    p.SetValue(i, v, null);
                }
                catch (Exception ex)
                {
                    Exception = new ExceptionCollector(new Exception(string.Format("Could not set property {0}", p.Name), ex));
                    ImportStatus = BootstrapStatus.danger;
                    return;
                }
            }

            try
            {
                Repository.Update(i);
                ImportStatus = BootstrapStatus.success;
            }
            catch(Exception ex)
            {
                Exception = new ExceptionCollector(ex);
                ImportStatus = BootstrapStatus.danger;
                return;
            }
        }

        #endregion

    }
}

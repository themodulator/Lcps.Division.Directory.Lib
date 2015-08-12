using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;
using System.Security;
using System.Security.Principal;
using System.Security.AccessControl;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.Reflection;
using Lcps.Division.Directory.Infrastructure;
using Anvil.Repository;
using Lcps.Division.Directory.Providers;

using Lcps.Division.Directory.Repository;
using System.DirectoryServices.AccountManagement;



namespace Lcps.Division.Directory.Providers.Ldap
{
    public class LdapUser : IImportable<DirectoryEntry>, IDirectoryMember
    {
        #region Fields

        DirectoryEntry _domain;
        DirectoryEntry _parentOu;

        #endregion

        #region Constructors

        public LdapUser(DirectoryEntry parentOu, DirectoryEntry domain)
        {
            _parentOu = parentOu;
            _domain = domain;
        }

        public LdapUser(DirectoryEntry parentOu, DirectoryEntry domain, IDirectoryMember directoryMember)
        {
            _domain = domain;
            _parentOu = parentOu;

            foreach (PropertyInfo p in typeof(IDirectoryMember).GetProperties())
            {
                object v = null;

                try
                {
                    v = p.GetValue(directoryMember, null);
                }
                catch (Exception ex)
                {
                    Exception = new ExceptionCollector(new Exception(String.Format("Could not get value for property {0}", p.Name), ex));
                    ImportStatus = BootstrapStatus.danger;
                    return;
                }

                try
                {
                    p.SetValue(this, v, null);
                }
                catch (Exception ex)
                {
                    Exception = new ExceptionCollector(new Exception(String.Format("Could not set value for property {0}", p.Name), ex));
                    SyncStatus = ImportSyncStatus.Error;
                    return;

                }
            }
        }

        #endregion

        #region Directory Member Properties

        public string Id { get; set; }

        [LdapFieldMap("employeeId")]
        [ImportCompareField]
        public string InternalId { get; set; }

        [LdapFieldMap("givenName")]
        [ImportCompareField]
        public string GivenName { get; set; }

        [LdapFieldMap("middleName")]
        [ImportCompareField]
        public string MiddleName { get; set; }

        [LdapFieldMap("sn")]
        [ImportCompareField]
        public string SurName { get; set; }

        [LdapFieldMap("carLicense")]
        [ImportCompareField]
        public DateTime DOB { get; set; }

        public Directory.Repository.DiurectoryMemberGender Gender { get; set; }

        [LdapFieldMap("adminCategory")]
        [ImportCompareField]
        public long MembershipScope { get; set; }

        [LdapFieldMap("title")]
        [ImportCompareField]
        public string Title { get; set; }

        [LdapFieldMap("samAccountName")]
        public string UserName { get; set; }

        [LdapFieldMap("mail")]
        [ImportCompareField]
        public string Email { get; set; }

        [LdapFieldMap("adminDescription")]
        [ImportCompareField]
        public string InitialPassword { get; set; }

        [LdapFieldMap("department")]
        [ImportCompareField]
        public DirectoryMemberClass MemberClass { get; set; }

        #endregion

        #region LDAP Directory Entry Properties

        public string PrincipalDomain { get; set; }

        public bool AllowPasswordChange { get; set; }

        public string HomeFolderRoot { get; set; }

        public string AdminUserName { get; set; }

        public string AdminPassword { get; set; }

        public string[] IntendedGroups { get; set; }

        [ImportCompareField]
        [LdapFieldMap("userAccountControl")]
        public LcpsAdsUserAccountControl UAC { get; set; }


        public static void GrantfullAccessToFolder(string folderPath, DirectoryEntry de, string userName, string principalDomain)
        {
            try
            {
                DirectoryInfo dInfo = new DirectoryInfo(folderPath);
                DirectorySecurity dSecurity = dInfo.GetAccessControl();

                byte[] sidByte = de.InvokeGet("objectSid") as byte[];
                SecurityIdentifier sid = new SecurityIdentifier(sidByte, 0);


                string id = userName + "@" + principalDomain;

                dSecurity.AddAccessRule(new FileSystemAccessRule(sid, FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                dInfo.SetAccessControl(dSecurity);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error granting user {0} full persmissions to folder {1}", userName, folderPath), ex);
            }
        }

        #endregion 

        #region Import

        public List<String> Warnings { get; set; }

        public ImportSyncStatus SyncStatus { get; set; }

        public BootstrapStatus GetBootStrapStatus(ImportSyncStatus syncStatus)
        {
            switch (syncStatus)
            {
                case ImportSyncStatus.Current:
                    return BootstrapStatus.info;

                case ImportSyncStatus.Error:
                    return BootstrapStatus.danger;

                case ImportSyncStatus.Update:
                    return BootstrapStatus.warning;

                case ImportSyncStatus.Insert:
                    return BootstrapStatus.success;

                default:
                    return BootstrapStatus.@default;
            }
        }

        public ExceptionCollector Exception { get; set; }

        public BootstrapStatus ImportStatus { get; set; }

        public DirectoryEntry GetItem()
        {
            string filter = "(&(samAccountname=" + this.UserName + "))";
            DirectorySearcher srch = new DirectorySearcher(_domain, filter);
            SearchResult r = srch.FindOne();
            if (r == null)
                return null;
            else
                return r.GetDirectoryEntry();
        }


        public bool ExistsInTarget { get; set; }

        public void Insert()
        {
            string cn = "CN=";
            
            if(MiddleName == null)
                cn += string.Format("{0}\\, {1} ({2})", SurName, GivenName, UserName);
            else
                cn += string.Format("{0}\\, {1} {3} ({2})", SurName, GivenName, UserName, MiddleName);

            DirectoryMember m = new DirectoryMember(this);
            string pwd = m.DecryptPassword();

            DirectoryEntry de = _parentOu.Children.Add(cn, "user");
            de.InvokeSet("samAccountName", UserName);
            de.Invoke("password", new object[] { pwd });

            de.CommitChanges();
            _parentOu.RefreshCache();

            de.InvokeSet("userPrincipalName", UserName + "@" + PrincipalDomain);

            this.UAC = LcpsAdsUserAccountControl.NORMAL_ACCOUNT | LcpsAdsUserAccountControl.DONT_EXPIRE_PASSWD;


            List<MembershipScope> scopes = MembershipScopeRepository.GetApplicableScopes(this.MembershipScope).OrderBy(x => x.Caption).ToList();
            if(scopes.Where(x => (x.ScopeQualifier == MembershipScopeQualifier.Location | x.ScopeQualifier == MembershipScopeQualifier.Grade)).Count() == 0)
                this.UAC = this.UAC | LcpsAdsUserAccountControl.ACCOUNTDISABLE;


            if (!AllowPasswordChange)
                SetCanChangePassword(true);

            de.InvokeSet("userAccountcontrol", this.UAC);

            de.CommitChanges();

            


        }

        public void SetCanChangePassword(bool canNotChange)
        {
            var principalContext = new PrincipalContext(ContextType.Domain, this.PrincipalDomain, AdminUserName, AdminPassword);
            UserPrincipal u = UserPrincipal.FindByIdentity(principalContext, UserName);
            u.UserCannotChangePassword = canNotChange;
            u.Save();
        }

        public void Update() 
        {
            DirectoryEntry de = GetItem();

        }

        public List<string> SyncFields { get; set; }

        public void GetSyncStatus()
        {
            SyncStatus = ImportSyncStatus.Current;

            DirectoryEntry user = GetItem();
            if(user == null)
            {
                SyncStatus = ImportSyncStatus.Insert;
                return;
            }

            PropertyInfo[] pp = this.GetType().GetProperties()
                        .Where(x => x.GetCustomAttributes(false)
                            .Any(a => a.GetType() == typeof(ImportCompareFieldAttribute)))
                            .ToArray();

            foreach (PropertyInfo p in pp)
            {
                LdapFieldMapAttribute a = (LdapFieldMapAttribute)p.GetCustomAttribute(typeof(LdapFieldMapAttribute));
                if (a == null)
                {
                    Exception = new ExceptionCollector(new Exception(string.Format("The field {0} was marked as comparable but has no corresponding ADSI attribute", p.Name)));
                    SyncStatus = ImportSyncStatus.Error;
                    return;
                }

                object sv = user.InvokeGet(a.ADSIFIeldName);

                object tv = p.GetValue(this, null);

                if (sv == null & tv != null)
                    SyncFields.Add(p.Name);

                if (sv != null)
                {
                    if (!sv.Equals(tv))
                        SyncFields.Add(p.Name);
                }
            }

            if (SyncFields.Count() > 0)
                SyncStatus = ImportSyncStatus.Update;
            else
                SyncStatus = ImportSyncStatus.Current;

            if (!user.Parent.Name.ToLower().Equals(_parentOu.Name.ToLower()))
            {
                SyncFields.Add("OU");
                SyncStatus = ImportSyncStatus.Update;
            }

        }

        public void SyncGroups()
        {
            DirectoryEntry de = GetItem();

        }

        #endregion

        #region Group Membership

        public static string[] GetGroupMembership(DirectoryEntry user)
        {
            List<string> l = new List<string>();

            var groups = user.Properties["memberOf"].Value;
            if (groups == null)
                return l.ToArray();

            if (groups.GetType() == typeof(string))
            {
                l.Add((string)groups);
            }
            else
            {
                ArrayList gc = groups as ArrayList;
                foreach (string g in gc)
                {
                    l.Add(g);
                }

            }
            return l.ToArray();
        }

        public static void AddToGroup(string groupPath, DirectoryEntry user, string userName, string password)
        {
            DirectoryEntry group = new DirectoryEntry(groupPath, userName, password);
            group.Invoke("add", user.Path);
            group.CommitChanges();
            group.RefreshCache();
        }

        #endregion
    }
}

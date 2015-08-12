using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Anvil.Repository;
using Lcps.Division.Directory.Infrastructure;
using System.Text.RegularExpressions;

namespace Lcps.Division.Directory.Repository
{
    public class MembershipScopeRepository : GenericRepository<MembershipScope>
    {
        public MembershipScopeRepository()
            :base(new LcpsRepositoryContext())
        { }

        

        public override void Insert(MembershipScope entity)
        {

            MembershipScope m = this.Get(x => !x.MembershipScopeId.Equals(Guid.Empty))
                .OrderByDescending( x=> x.BinaryValue)
                .FirstOrDefault();

            if (m == null)
                entity.BinaryValue = 1;
            else
                entity.BinaryValue = m.BinaryValue * 2;

            entity.LiteralName = GenerateLiteralName(entity.Caption);

            base.Insert(entity);
        }

        public static string GenerateLiteralName(string caption)
        {
            return Regex.Replace(caption, @"[^A-Za-z0-9]+", "");
        }

        public static Type GetEnumType()
        {
            MembershipScopeRepository r = new MembershipScopeRepository();
            Type t = r.Get().ToEnum(x => x.LiteralName, x => x.BinaryValue);
            return t;
        }

        public static bool HasFlag(long binaryValue, string literalName, System.Type t)
        {
            Enum bv = (Enum)Enum.ToObject(t, binaryValue);
            Enum cv = (Enum)Enum.Parse(t, literalName);
            return cv.HasFlag(bv);
        }

        public static bool HasFlag(long memberValue, long challenge, System.Type t)
        {
            Enum mv = (Enum)Enum.ToObject(t, memberValue);
            Enum cv = (Enum)Enum.ToObject(t, challenge);

            return mv.HasFlag(cv);
        }

        public string GetLiteralCaption(long binaryValue)
        {
            Type t = GetEnumType();
            Enum bv = (Enum)Enum.ToObject(t, binaryValue);
            return System.Enum.GetName(t, bv);
        }

        public static List<MembershipScope> GetApplicableScopes(long binaryValue)
        {
            List<MembershipScope> items = new List<MembershipScope>();

            LcpsRepositoryContext repo = new LcpsRepositoryContext();

            List<MembershipScope> all = repo.MembershipScopes.OrderBy(x => x.Caption).ToList();

            Type t = all.ToEnum(x => x.LiteralName, x => x.BinaryValue);
            
           
            foreach(MembershipScope s in all)
            {
                Enum sv = (Enum)Enum.ToObject(t, binaryValue);
                Enum cv = (Enum)Enum.ToObject(t, s.BinaryValue);

                if (sv.HasFlag(cv))
                    items.Add(s);
            }

            return items;
        }

        public static string[] GetApplicableCaptions(long binaryValue)
        {
            List<MembershipScope> scopes = GetApplicableScopes(binaryValue);
            return scopes.Select(x => x.Caption).ToArray();
        }

        public static List<MembershipScope> GetByQualifier(MembershipScopeQualifier q)
        {
            List<MembershipScope> scopes = (new MembershipScopeRepository()).Get()
                .Where(x => x.ScopeQualifier == q)
                .OrderBy(x => x.Caption).ToList();

            return scopes;
        }

    }


}

using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lcps.Division.Directory.Repository
{
    public interface IDirectoryMember
    {
        string InternalId { get; set; }

        string GivenName { get; set; }

        string MiddleName { get; set; }

        string SurName { get; set; }

        DateTime DOB { get; set; }

        Directory.Repository.DiurectoryMemberGender Gender { get; set; }

        long MembershipScope { get; set; }

        string Title { get; set; }

        string InitialPassword { get; set; }
    }
}

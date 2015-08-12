using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Anvil.Repository;


namespace Lcps.Division.Directory.Providers
{
    public interface IImportable<TEntity> 
    {
        List<String> Warnings { get; set; }

        ImportSyncStatus SyncStatus { get; set; }

        BootstrapStatus GetBootStrapStatus(ImportSyncStatus syncStatus);

        ExceptionCollector Exception { get; set; }

        BootstrapStatus ImportStatus { get; set; }

        TEntity GetItem();

        bool ExistsInTarget { get; set; }

        void Insert();

        void Update();

        List<string> SyncFields { get; set; }

        void GetSyncStatus();
    }
}

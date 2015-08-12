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
    public class Importable<TEntity> : IImportable<TEntity> where TEntity: class
    {
        public Importable(GenericRepository<TEntity> repository)
        {
            this.Repository = repository;
        }

        public GenericRepository<TEntity> Repository { get; set; }

        public List<String> Warnings { get; set; }

        public ImportSyncStatus SyncStatus { get; set; }

        public BootstrapStatus GetBootStrapStatus(ImportSyncStatus syncStatus)
        { 
            switch(syncStatus)
            {
                case ImportSyncStatus.Current :
                    return BootstrapStatus.info;

                case ImportSyncStatus.Error :
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

        public virtual TEntity GetItem()
        {
            return (TEntity)Activator.CreateInstance(typeof(TEntity));
        }

        public virtual bool ExistsInTarget { get; set; }

        public virtual void Insert()
        {}

        public virtual void Update() { }

        public virtual List<string> SyncFields { get; set; }

        public virtual void GetSyncStatus()
        {
            
        }

        
    }
}

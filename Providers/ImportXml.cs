using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Serialization;
using System.IO;
using System.Web;

using Anvil.Repository;

namespace Lcps.Division.Directory.Providers
{
    public class ImportXml<TEntity> where TEntity : class
    {
        public ImportXml(GenericRepository<TEntity> repo, Func<TEntity, object> keyField, bool overwrite) 
        {
            this.Repository = repo;
            this.KeyField = keyField;
            this.Overwrite = overwrite;
        }

        #region Properties

        public HttpPostedFileBase InputFile { get; set; }

        public Func<TEntity, object> KeyField { get; set; }

        public GenericRepository<TEntity> Repository;

        public bool Overwrite { get; set; }

        #endregion

        #region Import

        public void Import()
        {
            XmlSerializer xml = new XmlSerializer(typeof(TEntity));
            List<TEntity> items = (List<TEntity>)xml.Deserialize(InputFile.InputStream);
            foreach(TEntity item in items)
            {
                object k = KeyField(item);
                if (Repository.First(x => KeyField(item).Equals(k)) == null)
                    Repository.Insert(item);
                else
                {
                    if(Overwrite)
                        Repository.Update(item);
                }
            }
        }

        #endregion
    }
}

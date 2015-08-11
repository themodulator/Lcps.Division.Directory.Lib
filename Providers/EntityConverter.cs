using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;

using System.Data.Entity;
using System.Linq.Expressions;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using System.Data;

namespace Lcps.Division.Directory.Providers
{
    public class EntityConverter<TEntity>
    {
        public static TEntity FromDataRow(DataRow row)
        {
            TEntity item = (TEntity)Activator.CreateInstance(typeof(TEntity), true);


            string[] names = (from DataColumn x in row.Table.Columns
                              select x.ColumnName.ToLower()).ToArray();

            foreach (PropertyInfo p in typeof(TEntity).GetProperties())
            {
                if (names.Contains(p.Name.ToLower()))
                {
                    object v = row[p.Name];

                    try
                    {
                        if (p.PropertyType.IsEnum)
                            v = System.Enum.Parse(p.PropertyType, v.ToString());

                        p.SetValue(item, Convert.ChangeType(v, p.PropertyType), null);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Property {0}, Value {1}", p.Name, v.ToString()), ex);
                    }
                }
            }

            return item;
        }


    }
}

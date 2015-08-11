using System;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;
using System.Web;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lcps.Division.Directory.Providers
{
    public class ImportTextFile<TEntity> : DataTable
    {
        #region Fields

        Dictionary<int, string> _lines = new Dictionary<int, string>();

        #endregion

        #region Constructors

        public ImportTextFile()
        { }

        public ImportTextFile(string title, string delimiter, MvcRouteDefinition browseFormRoute)
        {
            Delimiter = delimiter;
            Title = title;
            this.BrowseFormRoute = browseFormRoute;
            this.Items = new List<TEntity>();
        }

        #endregion

        #region Properties

        public string Delimiter { get; set;}

        public Dictionary<int, string> Lines
        {
            get { return _lines; }
        }

        public string Title { get; set; }

        public HttpPostedFileBase InputFile { get; set; }

        public MvcRouteDefinition BrowseFormRoute { get; set; }

        public List<TEntity> Items { get; set; }

        #endregion


        #region Load

        public void Load()
        {
            InputFile.InputStream.Position = 0;
            StreamReader r = new System.IO.StreamReader(InputFile.InputStream);
            Load(r);
        }

        private void Load(StreamReader reader)
        {
            Columns.Clear();

            Rows.Clear();

            int index = 0;

            Items = new List<TEntity>();

            Columns.Add("Index", typeof(int));

            while(!reader.EndOfStream)
            {
                string line = reader.ReadLine();

                _lines.Add(index, line);


                string[] rec = line.Split(Delimiter.ToCharArray()[0]);

                if(index == 0)
                {
                    foreach(string c in rec)
                    {
                        Columns.Add(c, typeof(string));
                    }
                }
                else
                {
                    List<string> record = new List<string>();
                    record.Add(index.ToString());
                    record.AddRange(rec);

                    Rows.Add(record.ToArray());

                    Items.Add(EntityConverter<TEntity>.FromDataRow(Rows[index -1]));
                }

                index++;
            }

        }

        #endregion
    }
}

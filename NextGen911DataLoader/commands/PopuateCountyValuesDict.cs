using ArcGIS.Core.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGen911DataLoader.commands
{
    class PopuateCountyValuesDict
    {
        public static Dictionary<string, string> Execute(DatabaseConnectionProperties sgidConnectionProperties, string fgdbPath, StreamWriter streamWriter)
        {
            try
            {
                //Dictionary<string, string> dict = new models.CountyValues<string>();
                Dictionary<string, string> dict = new Dictionary<string, string>();

                // connect to sgid.
                using (Geodatabase sgid = new Geodatabase(sgidConnectionProperties))

                // connect to ng911 fgdb.
                using (Geodatabase NG911Utah = new Geodatabase(new FileGeodatabaseConnectionPath(new Uri(fgdbPath))))

                // get SGID Feature Classes.
                using (FeatureClass sgid_FeatClass = sgid.OpenDataset<FeatureClass>("SGID10.BOUNDARIES.Counties"))
                {
                    QueryFilter queryFilter1 = new QueryFilter
                    {
                        //WhereClause = "AddSystem = 'SALT LAKE CITY' and StreetName = 'ELIZABETH'"
                    };

                    // Get a Cursor of SGID features.
                    using (RowCursor SgidCursor = sgid_FeatClass.Search(queryFilter1, true))
                    {
                        // Loop through the sgid features.
                        while (SgidCursor.MoveNext())
                        {
                            // Values to dictionary.

                            dict.Add(SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("COUNTYNBR")).ToString(), SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("NAME")).ToString());

                        }
                    }

                }
                
                return dict;
            }
            catch
            {
                return null;
            }
        }
    }
}

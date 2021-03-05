using ArcGIS.Core.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Core.Hosting;
using ArcGIS.Core.Internal.CIM;
using Microsoft.Win32;

namespace NextGen911DataLoader.commands
{
    class GetPostalCommFromNumber
    {
        //Create dictionay
        public static Dictionary<string, string> postal_dict; // = new Dictionary<string, string>();

        public static void Execute(DatabaseConnectionProperties sgidConnectionProperties, string fgdbPath, StreamWriter streamWriter)
        {
            try
            {
                //Dictionary<string, string> dict = new models.CountyValues<string>();
                postal_dict = new Dictionary<string, string>();
                
                // connect to sgid.
                using (ArcGIS.Core.Data.Geodatabase sgid = new ArcGIS.Core.Data.Geodatabase(sgidConnectionProperties))
                {
                    // get SGID Feature Classes.
                    using (ArcGIS.Core.Data.FeatureClass sgid_FeatClass = sgid.OpenDataset<ArcGIS.Core.Data.FeatureClass>("SGID.BOUNDARIES.ZipCodes"))
                    {
                        ArcGIS.Core.Data.QueryFilter queryFilter1 = new ArcGIS.Core.Data.QueryFilter
                        {
                            //WhereClause = "AddSystem = 'SALT LAKE CITY' and StreetName = 'ELIZABETH'"
                        };

                        // Get a Cursor of SGID features.
                        using (ArcGIS.Core.Data.RowCursor SgidCursor = sgid_FeatClass.Search(queryFilter1, true))
                        {
                            // Loop through the sgid features.
                            while (SgidCursor.MoveNext())
                            {
                                // Values to dictionary.

                                postal_dict.Add(SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("ZIP5")).ToString(), SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("NAME")).ToString());

                            }
                        }
                    }
                }
               
            }
            catch
            {
            
            }
        }


        public static string GetPostalComm(string postal_number)
        {
            if (postal_dict.ContainsKey(postal_number))
                return postal_dict[postal_number].ToString();
                //return postal_comm;
            else
                return "";

            //string postal_comm = postal_dict[postal_number].ToString();
            //return postal_comm;
        }

    }
}

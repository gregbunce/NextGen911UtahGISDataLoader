using ArcGIS.Core.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Concider making this a dictionary based on a sql query to the sgid10.boundaries.counties table to load the number and name into the dict -- instead of hardcoding it like below.
// This would allow for the number/names to change and the dict would be dynamic. 
namespace NextGen911DataLoader.commands
{
    class GetCountyNameFromNumber
    {
        //public static string Execute(string countyNBR, StreamWriter streamWriter)
        //{
        //    try
        //    {
        //        string countyName = string.Empty;

        //        switch (countyNBR.Trim())
        //        {
        //            case "01":
        //                countyName = "BEAVER";
        //                break;
        //            case "02":
        //                countyName = "BOX ELDER";
        //                break;
        //            case "03":
        //                countyName = "CACHE";
        //                break;
        //            case "04":
        //                countyName = "CARBON";
        //                break;
        //            case "05":
        //                countyName = "DAGGETT";
        //                break;
        //            case "06":
        //                countyName = "DAVIS";
        //                break;
        //            case "07":
        //                countyName = "DUCHESNE";
        //                break;
        //            case "08":
        //                countyName = "EMERY";
        //                break;
        //            case "09":
        //                countyName = "GARFIELD";
        //                break;
        //            case "10":
        //                countyName = "GRAND";
        //                break;
        //            case "11":
        //                countyName = "IRON";
        //                break;
        //            case "12":
        //                countyName = "JUAB";
        //                break;
        //            case "13":
        //                countyName = "KANE";
        //                break;
        //            case "14":
        //                countyName = "MILLARD";
        //                break;
        //            case "15":
        //                countyName = "MORGAN";
        //                break;
        //            case "16":
        //                countyName = "PIUTE";
        //                break;
        //            case "17":
        //                countyName = "RICH";
        //                break;
        //            case "18":
        //                countyName = "SALT LAKE";
        //                break;
        //            case "19":
        //                countyName = "SAN JUAN";
        //                break;
        //            case "20":
        //                countyName = "SANPETE";
        //                break;
        //            case "21":
        //                countyName = "SEVIER";
        //                break;
        //            case "22":
        //                countyName = "SUMMIT";
        //                break;
        //            case "23":
        //                countyName = "TOOELE";
        //                break;
        //            case "24":
        //                countyName = "UINTAH";
        //                break;
        //            case "25":
        //                countyName = "UTAH";
        //                break;
        //            case "26":
        //                countyName = "WASATCH";
        //                break;
        //            case "27":
        //                countyName = "WASHINGTON";
        //                break;
        //            case "28":
        //                countyName = "WAYNE";
        //                break;
        //            case "29":
        //                countyName = "WEBER";
        //                break;
        //            default:
        //                // Log it to the text file.
        //                streamWriter.WriteLine("WARNING MESSAGE FROM GetCountyNameFromNumber Class...");
        //                streamWriter.WriteLine("Could not find a County Name from the following number: " + countyNBR);
        //                break;
        //        }

        //        return countyName;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("There was an error with GetCountyNameFromNumber method." +
        //        ex.Message + " " + ex.Source + " " + ex.InnerException + " " + ex.HResult + " " + ex.StackTrace + " " + ex);

        //        streamWriter.WriteLine();
        //        streamWriter.WriteLine("ERROR MESSAGE...");
        //        streamWriter.WriteLine("_______________________________________");
        //        streamWriter.WriteLine("There was an error with GetCountyNameFromNumber method." +
        //        ex.Message + " " + ex.Source + " " + ex.InnerException + " " + ex.HResult + " " + ex.StackTrace + " " + ex);

        //        return null;
        //    }
        //}


        ///
        /// 
        //Dictionary<string, string> dict = new models.CountyValues<string>();
        public static Dictionary<string, string> dict; // = new Dictionary<string, string>();

        public static void Execute(DatabaseConnectionProperties sgidConnectionProperties, string fgdbPath, StreamWriter streamWriter)
        {
            try
            {
                //Dictionary<string, string> dict = new models.CountyValues<string>();
                dict = new Dictionary<string, string>();

                // connect to sgid.
                using (Geodatabase sgid = new Geodatabase(sgidConnectionProperties))
                {
                    // connect to ng911 fgdb.
                    using (Geodatabase NG911Utah = new Geodatabase(new FileGeodatabaseConnectionPath(new Uri(fgdbPath))))
                    {
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
                    }
                }
               
            }
            catch
            {
            
            }
        }


        public static string GetCountyName(string countyNumber)
        {
            string countyName = dict[countyNumber].ToString();

            return countyName;
        }

    }
}

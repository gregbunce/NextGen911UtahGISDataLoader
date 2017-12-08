using ArcGIS.Core.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGen911DataLoader.commands
{
    class ETL_CodeStub
    {
        public static void Execute(DatabaseConnectionProperties sgidConnectionProperties, string fgdbPath, StreamWriter streamWriter)
        {
            try
            {
                // connect to sgid.
                using (Geodatabase sgid = new Geodatabase(sgidConnectionProperties))
                {
                    // connect to ng911 fgdb.
                    using (Geodatabase NG911Utah = new Geodatabase(new FileGeodatabaseConnectionPath(new Uri(fgdbPath))))
                    {
                        // Get access to NG911 feature class
                        using (FeatureClass ng911_FeatClass = NG911Utah.OpenDataset<FeatureClass>("NG911_FeatureClassName"))
                        {
                            // Create a row count bean-counter.
                            Int32 ng911FeatClassRowCount = 1;

                            // delete all the existing rows
                            QueryFilter queryFilter = new QueryFilter
                            {
                                WhereClause = "OBJECTID > 0"
                            };
                            // Delete all rows in the AddressPoints feature class.
                            ng911_FeatClass.DeleteRows(queryFilter);

                            // get SGID Feature Classes.
                            using (FeatureClass sgid_FeatClass = sgid.OpenDataset<FeatureClass>("SGID10..."), sgidZipCodes = sgid.OpenDataset<FeatureClass>("SGID10..."))
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
                                        // Get a feature class definition for the NG911 feature class.
                                        FeatureClassDefinition featureClassDefinitionNG911 = ng911_FeatClass.GetDefinition();

                                        // Get a feature class definition for the SGID feature class
                                        FeatureClassDefinition featureClassDefinitionSGID = sgid_FeatClass.GetDefinition();

                                        //Row SgidRow = SgidCursor.Current;
                                        Feature sgidFeature = (Feature)SgidCursor.Current;

                                        // Create row buffer.
                                        using (RowBuffer rowBuffer = ng911_FeatClass.CreateRowBuffer())
                                        {
                                            // Create geometry (via rowBuffer).
                                            rowBuffer[featureClassDefinitionNG911.GetShapeField()] = sgidFeature.GetShape();

                                            // Create attributes for direct transfer fields (via rowBuffer). //
                                            rowBuffer["NG911FieldName"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("SGID_FieldName"));

                                            // ADD OTHER FIELDS //



                                            // create the row, with attributes and geometry via rowBuffer, in the ng911 database
                                            using (Row row = ng911_FeatClass.CreateRow(rowBuffer))
                                            {
                                                Console.WriteLine("NGLayerName_Ng911RowCount: " + ng911FeatClassRowCount);
                                                Console.WriteLine("NGLayerName__SgidOID: " + SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("OBJECTID")).ToString());
                                                ng911FeatClassRowCount = ng911FeatClassRowCount + 1;
                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was an error with REPLACE_WITH_CLASS_NAME method. " +
                ex.Message + " " + ex.Source + " " + ex.InnerException + " " + ex.HResult + " " + ex.StackTrace + " " + ex);

                streamWriter.WriteLine();
                streamWriter.WriteLine("ERROR MESSAGE...");
                streamWriter.WriteLine("_______________________________________");
                streamWriter.WriteLine("There was an error with REPLACE_WITH_CLASS_NAME method." +
                ex.Message + " " + ex.Source + " " + ex.InnerException + " " + ex.HResult + " " + ex.StackTrace + " " + ex);
            }
        }
    }
}

using ArcGIS.Core.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGen911DataLoader.commands
{
    class LoadFire
    {
        public static void Execute(DatabaseConnectionProperties sgidConnectionProperties, string fgdbPath, StreamWriter streamWriter, bool truncate)
        {
            try
            {
                // connect to sgid and ng911
                using (Geodatabase sgid = new Geodatabase(sgidConnectionProperties), NG911Utah = new Geodatabase(new FileGeodatabaseConnectionPath(new Uri(fgdbPath))))
                {
                    // Get access to NG911 feature class
                    using (FeatureClass ng911_FeatClass = NG911Utah.OpenDataset<FeatureClass>("Fire"))
                    {
                        // Create a row count bean-counter.
                        Int32 ng911FeatClassRowCount = 1;

                        // Check if the user wants to truncate the layer first
                        if (truncate)
                        {
                            string featClassLocation = fgdbPath + "\\" + ng911_FeatClass.GetName().ToString();
                            string pythonFile = @"C:\Users\gbunce\source\repos\NextGen911DataLoader\NextGen911DataLoader\scripts_arcpy\TrancateTable.py";
                            commands.ExecuteArcpyScript.run_arcpy(pythonFile, featClassLocation);
                        }

                        // get SGID Feature Classes.
                        using (FeatureClass sgid_FeatClass = sgid.OpenDataset<FeatureClass>("SGID.HEALTH.EMSServiceAreas"))
                        {
                            QueryFilter queryFilter1 = new QueryFilter
                            {
                                WhereClause = "NAME LIKE '%FIRE%'"
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
                                        rowBuffer["Source"] = "AGRC";
                                        rowBuffer["State"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("STATE")).ToString();
                                        rowBuffer["DateUpdate"] = DateTime.Now;
                                        //rowBuffer["Effective"] = DateTime.Now;
                                        //rowBuffer["Expire"] = DateTime.Now;
                                        rowBuffer["ES_NGUID"] = "FIRE" + SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("OBJECTID")).ToString() + "@gis.utah.gov";
                                        rowBuffer["Agency_ID"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("AGENCY_ID"));
                                        // replace spaces, dashes, and parenthesis in tel
                                        string phone = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("PHONE")).ToString();
                                        phone = phone.Replace("-", "");
                                        phone = phone.Replace("(", "");
                                        phone = phone.Replace(")", "");
                                        phone = phone.Replace(" ", "");
                                        rowBuffer["ServiceURI"] = "tel:+" + phone;
                                        rowBuffer["ServiceURN"] = "urn:nena:service:responder.fire";
                                        rowBuffer["ServiceNum"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("PHONE"));
                                        //rowBuffer["AVcard_URI"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("SGID_FieldName"));
                                        rowBuffer["DsplayName"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("NAME")).ToString().Trim();

                                        // create the row, with attributes and geometry via rowBuffer, in the ng911 database
                                        using (Row row = ng911_FeatClass.CreateRow(rowBuffer))
                                        {
                                            Console.WriteLine("FIRE_Ng911RowCount: " + ng911FeatClassRowCount);
                                            Console.WriteLine("FIRE__SgidOID: " + SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("OBJECTID")).ToString());
                                            ng911FeatClassRowCount = ng911FeatClassRowCount + 1;
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
                Console.WriteLine("There was an error with LoadFire method. " +
                ex.Message + " " + ex.Source + " " + ex.InnerException + " " + ex.HResult + " " + ex.StackTrace + " " + ex);

                streamWriter.WriteLine();
                streamWriter.WriteLine("ERROR MESSAGE...");
                streamWriter.WriteLine("_______________________________________");
                streamWriter.WriteLine("There was an error with LoadFire method." +
                ex.Message + " " + ex.Source + " " + ex.InnerException + " " + ex.HResult + " " + ex.StackTrace + " " + ex);
            }
        }

    }
}

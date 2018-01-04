using ArcGIS.Core.Data;
using ArcGIS.Desktop.Core.Geoprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGen911DataLoader.commands
{
    class LoadLawEnforcement
    {
        public static void Execute(DatabaseConnectionProperties sgidConnectionProperties, string fgdbPath, StreamWriter streamWriter, bool truncate)
        {
            try
            {
                // connect to sgid and ng911
                using (Geodatabase sgid = new Geodatabase(sgidConnectionProperties), NG911Utah = new Geodatabase(new FileGeodatabaseConnectionPath(new Uri(fgdbPath))))
                {
                    // Get access to NG911 feature class
                    using (FeatureClass ng911_FeatClass = NG911Utah.OpenDataset<FeatureClass>("LawEnforcement"))
                    {
                        // Create a row count bean-counter.
                        Int32 ng911FeatClassRowCount = 1;

                        // Check if the user wants to truncate the layer first
                        if (truncate)
                        {
                            string featClassLocation = fgdbPath + "\\" + ng911_FeatClass.GetName().ToString();
                            string pythonFile = "../../scripts_arcpy/TrancateTable.py";
                            commands.ExecuteArcpyScript.run_arcpy(pythonFile, featClassLocation);
                        }


                        // get SGID Feature Classes.
                        using (FeatureClass sgid_FeatClass = sgid.OpenDataset<FeatureClass>("SGID10.SOCIETY.LawEnforcementBoundaries"))
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
                                        rowBuffer["Source"] = "AGRC";
                                        rowBuffer["State"] = "UT";
                                        rowBuffer["DateUpdate"] = DateTime.Now;
                                        //rowBuffer["Effective"] = DateTime.Now;
                                        //rowBuffer["Expire"] = DateTime.Now;
                                        rowBuffer["ES_NGUID"] = "LAW" + SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("OBJECTID")).ToString() + "@gis.utah.gov";
                                        rowBuffer["Agency_ID"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("NAME"));
                                        //rowBuffer["ServiceURI"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("SGID_FieldName"));
                                        rowBuffer["ServiceURN"] = "urn:service:sos";
                                        //rowBuffer["ServiceNum"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("SGID_FieldName"));
                                        //rowBuffer["AVcard_URI"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("SGID_FieldName"));

                                        // DISPLAY NAME //
                                        if (SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("NAME")).ToString().Contains("PD"))
                                        {
                                            string displayName = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("NAME")).ToString();
                                            displayName = displayName.Replace("PD", "POLICE DEPARTMENT");
                                            rowBuffer["DsplayName"] = displayName;
                                        }
                                        else if (SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("NAME")).ToString().Contains("SO"))
                                        {
                                            string displayName = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("NAME")).ToString();
                                            displayName = displayName.Replace("SO", "SHERIFF OFFICE");
                                            rowBuffer["DsplayName"] = displayName;
                                        }
                                        else
                                        {
                                            rowBuffer["DsplayName"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("NAME"));
                                        }


                                        // create the row, with attributes and geometry via rowBuffer, in the ng911 database
                                        using (Row row = ng911_FeatClass.CreateRow(rowBuffer))
                                        {

                                            //ng911_FeatClass.get.Search()
                                            Console.WriteLine("LawEnforcement_Ng911RowCount: " + ng911FeatClassRowCount);
                                            Console.WriteLine("LawEnforcement__SgidOID: " + SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("OBJECTID")).ToString());
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
                Console.WriteLine("There was an error with LoadLawEnforcement method. " +
                ex.Message + " " + ex.Source + " " + ex.InnerException + " " + ex.HResult + " " + ex.StackTrace + " " + ex);

                streamWriter.WriteLine();
                streamWriter.WriteLine("ERROR MESSAGE...");
                streamWriter.WriteLine("_______________________________________");
                streamWriter.WriteLine("There was an error with LoadLawEnforcement method." +
                ex.Message + " " + ex.Source + " " + ex.InnerException + " " + ex.HResult + " " + ex.StackTrace + " " + ex);
            }
        }

    }
}

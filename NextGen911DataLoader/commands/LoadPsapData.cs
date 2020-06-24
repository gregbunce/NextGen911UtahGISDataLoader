using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Core;
using ArcGIS.Core.Data;
using System.IO;

namespace NextGen911DataLoader.commands
{
    class LoadPsapData
    {
        public static void Execute(DatabaseConnectionProperties sgidConnectionProperties, string fgdbPath, StreamWriter streamWriter, bool truncate)
        {
            try
            {
                // connect to sgid.
                using (Geodatabase sgid = new Geodatabase(sgidConnectionProperties))
                {
                    // connect to ng911 fgdb.
                    using (Geodatabase NG911Utah = new Geodatabase(new FileGeodatabaseConnectionPath(new Uri(fgdbPath))))
                    {
                        // Get access to NG911 PSAP
                        using (FeatureClass ng911_FeatClass = NG911Utah.OpenDataset<FeatureClass>("PSAP_Boundaries"))
                        {
                            // Check if the user wants to truncate the layer first
                            if (truncate)
                            {
                                string featClassLocation = fgdbPath + "\\" + ng911_FeatClass.GetName().ToString();
                                string pythonFile = "../../scripts_arcpy/TrancateTable.py";
                                commands.ExecuteArcpyScript.run_arcpy(pythonFile, featClassLocation);
                            }

                            // get SGID Psap Feature Class.
                            using (FeatureClass sgidPsap = sgid.OpenDataset<FeatureClass>("SGID.SOCIETY.PSAPBoundaries"))
                            {
                                QueryFilter queryFilter1 = new QueryFilter
                                {
                                    //WhereClause = "DISTRCTNAME = 'Indian Prairie School District 204'"
                                };

                                // Get a Cursor of all sgidPsap features.
                                using (RowCursor SgidCursor = sgidPsap.Search(null, true))
                                {
                                    // Loop through the sgidPsap features.
                                    while (SgidCursor.MoveNext())
                                    {
                                        // Get a feature class definition for the NG911 feature class.
                                        FeatureClassDefinition featureClassDefinition = ng911_FeatClass.GetDefinition();

                                        //Row SgidRow = SgidPsapCursor.Current;
                                        Feature sgidFeature = (Feature)SgidCursor.Current;
                                        
                                        // Create row buffer.
                                        using (RowBuffer rowBuffer = ng911_FeatClass.CreateRowBuffer())
                                        {
                                            // Create geometry (via rowBuffer).
                                            rowBuffer[featureClassDefinition.GetShapeField()] = sgidFeature.GetShape();


                                            // Create attributes (via rowBuffer).
                                            rowBuffer["Source"] = "AGRC";
                                            rowBuffer["DateUpdate"] = DateTime.Now; // this needs to be the updated date in the sgid database, but we don't have one yet. 
                                            //rowBuffer["Effective"] = "";
                                            //rowBuffer["Expire"] = "";
                                            rowBuffer["ES_NGUID"] = "PSAP" + SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("OBJECTID")).ToString() + "@gis.utah.gov"; ;
                                            rowBuffer["State"] = "UT";
                                            rowBuffer["Agency_ID"] = "";
                                            // replace spaces, dashes, and parenthesis in tel
                                            if (SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("PHONE_NUMBER")) != null)
                                            {
                                                string phone = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("PHONE_NUMBER")).ToString();
                                                phone = phone.Replace("-", "");
                                                phone = phone.Replace("(", "");
                                                phone = phone.Replace(")", "");
                                                phone = phone.Replace(" ", "");
                                                rowBuffer["ServiceURI"] = "tel:+" + phone;
                                            }

                                            rowBuffer["ServiceURN"] = "urn:nena:service:sos:psap";
                                            rowBuffer["ServiceNum"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("PHONE_NUMBER"));
                                            rowBuffer["AVcard_URI"] = "";
                                            rowBuffer["DsplayName"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("PSAP_NAME"));


                                            //// DOMAIN VALUES >>>
                                            ////IReadOnlyList<Field> fields = featureClassDefinition.GetFields();

                                            //int NG911_Field = featureClassDefinition.FindField("State");
                                            //Field NG911_NameField = featureClassDefinition.GetFields()[NG911_Field];

                                            //Domain domain = NG911_NameField.GetDomain();
                                            //if (domain is CodedValueDomain)
                                            //{
                                            //    CodedValueDomain codedValueDomain = domain as CodedValueDomain;
                                            //    string value = codedValueDomain.GetName("UT");
                                            //    Console.WriteLine(value);
                                            //}
                                            //// <<< DOMAIN VALUES


                                            // create the row, with attributes and geometry via rowBuffer, in the ng911 database
                                            using (Row row = ng911_FeatClass.CreateRow(rowBuffer)) 
                                            {
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
                Console.WriteLine("There was an error with LoadPsapData method." +
                ex.Message + " " + ex.Source + " " + ex.InnerException + " " + ex.HResult + " " + ex.StackTrace + " " + ex);

                streamWriter.WriteLine();
                streamWriter.WriteLine("ERROR MESSAGE...");
                streamWriter.WriteLine("_______________________________________");
                streamWriter.WriteLine("There was an error with LoadPsapData method." +
                ex.Message + " " + ex.Source + " " + ex.InnerException + " " + ex.HResult + " " + ex.StackTrace + " " + ex);
            }
        }
    }
}


// NG911 PSAP FIELDS //
//Source is a type of String with a length of 75
//DateUpdate is a type of Date with a length of 8
//Effective is a type of Date with a length of 8
//Expire is a type of Date with a length of 8
//ES_NGUID is a type of String with a length of 100
//State is a type of String with a length of 2
//Agency_ID is a type of String with a length of 100
//ServiceURI is a type of String with a length of 254
//ServiceURN is a type of String with a length of 50
//ServiceNum is a type of String with a length of 15
//AVcard_URI is a type of String with a length of 254
//DsplayName is a type of String with a length of 60


// SGDI psap fields //
//COUNTYNBR is a type of String with a length of 2
//PSAP_JURIS is a type of String with a length of 30
//PSAP_NAME is a type of String with a length of 100
//COUNTY is a type of String with a length of 50
//FIPS is a type of String with a length of 5
//ECATSID is a type of String with a length of 5
//COLOR4 is a type of SmallInteger with a length of 2
//PHONE_NUMBER is a type of String with a length of 14
//DPS_PSAP_NAME is a type of String with a length of 100
//TEXT911 is a type of String with a length of 1




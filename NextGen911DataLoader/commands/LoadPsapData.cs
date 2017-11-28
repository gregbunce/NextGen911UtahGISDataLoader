using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Core;
using ArcGIS.Core.Data;

namespace NextGen911DataLoader.commands
{
    class LoadPsapData
    {
        public static void Execute(DatabaseConnectionProperties sgidConnectionProperties, string fgdbPath)
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
                        using (FeatureClass ng911Psap = NG911Utah.OpenDataset<FeatureClass>("PSAP_Boundaries"))
                        {
                            // delete all the existing rows
                            QueryFilter queryFilter = new QueryFilter
                            {
                                WhereClause = "OBJECTID > 0"
                            };
                            ng911Psap.DeleteRows(queryFilter);

                            // get SGID Psap Feature Class.
                            using (FeatureClass sgidPsap = sgid.OpenDataset<FeatureClass>("SGID10.SOCIETY.PSAPBoundaries"))
                            {
                                QueryFilter queryFilter1 = new QueryFilter
                                {
                                    //WhereClause = "DISTRCTNAME = 'Indian Prairie School District 204'"
                                };

                                // Get a Cursor of all sgidPsap features.
                                using (RowCursor SgidPsapCursor = sgidPsap.Search(null, true))
                                {
                                    // Loop through the sgidPsap features.
                                    while (SgidPsapCursor.MoveNext())
                                    {
                                        // Get a feature class definition for the NG911 feature class.
                                        FeatureClassDefinition featureClassDefinition = ng911Psap.GetDefinition();

                                        //Row SgidRow = SgidPsapCursor.Current;
                                        Feature sgidFeature = (Feature)SgidPsapCursor.Current;
                                        
                                        // Create row buffer.
                                        using (RowBuffer rowBuffer = ng911Psap.CreateRowBuffer())
                                        {
                                            // Create geometry (via rowBuffer).
                                            rowBuffer[featureClassDefinition.GetShapeField()] = sgidFeature.GetShape();

                                            // Create attributes (via rowBuffer).
                                            rowBuffer["DsplayName"] = SgidPsapCursor.Current.GetOriginalValue(SgidPsapCursor.Current.FindField("PSAP_NAME"));
                                            rowBuffer["State"] = "UT";

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
                                            using (Row row = ng911Psap.CreateRow(rowBuffer)) 
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
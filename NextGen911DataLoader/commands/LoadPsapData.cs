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
                            // get sgidPsap Feature Class.
                            using (FeatureClass sgidPsap = sgid.OpenDataset<FeatureClass>("SGID10.SOCIETY.PSAPBoundaries"))
                            {
                                QueryFilter queryFilter = new QueryFilter
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




                                            // DOMAIN VALUES >>>
                                            IReadOnlyList<Field> fields = featureClassDefinition.GetFields();

                                            int strNameField = featureClassDefinition.FindField("State");
                                            Field NameField = featureClassDefinition.GetFields()[strNameField];
                                           
                                            Domain domain = NameField.GetDomain();
                                            if (domain is CodedValueDomain)
                                            {
                                                CodedValueDomain codedValueDomain = domain as CodedValueDomain;
                                                string value = codedValueDomain.GetName("UT");
                                                Console.WriteLine(value);
                                            }
                                            // <<< DOMAIN VALUES


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

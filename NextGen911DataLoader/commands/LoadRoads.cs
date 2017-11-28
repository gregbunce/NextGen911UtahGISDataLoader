using ArcGIS.Core.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGen911DataLoader.commands
{
    class LoadRoads
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
                        // Get access to NG911 Roads
                        using (FeatureClass ng911Roads = NG911Utah.OpenDataset<FeatureClass>("RoadCenterlines"))
                        {
                            // delete all the existing rows
                            QueryFilter queryFilter = new QueryFilter
                            {
                                WhereClause = "OBJECTID > 0"
                            };
                            ng911Roads.DeleteRows(queryFilter);

                            // get SGID roads Feature Class.
                            using (FeatureClass sgidRoads = sgid.OpenDataset<FeatureClass>("SGID10.TRANSPORTATION.Roads"))
                            {
                                QueryFilter queryFilter1 = new QueryFilter
                                {
                                    // test the etl using axtell address system b/c there's only 15 segments in this address system
                                    // CARTOCODE 15 is proposed roads
                                    WhereClause = "ADDRSYS_L = 'ST GEORGE' and CARTOCODE <> '15'"
                                };

                                // Get a Cursor of SGID road features.
                                using (RowCursor SgidPsapCursor = sgidRoads.Search(queryFilter1, true))
                                {
                                    // Loop through the sgidPsap features.
                                    while (SgidPsapCursor.MoveNext())
                                    {
                                        // Get a feature class definition for the NG911 feature class.
                                        FeatureClassDefinition featureClassDefinitionNG911 = ng911Roads.GetDefinition();

                                        // Get a feature class definition for the SGID feature class
                                        FeatureClassDefinition featureClassDefinitionSGID = sgidRoads.GetDefinition();

                                        //Row SgidRow = SgidPsapCursor.Current;
                                        Feature sgidFeature = (Feature)SgidPsapCursor.Current;

                                        // Create row buffer.
                                        using (RowBuffer rowBuffer = ng911Roads.CreateRowBuffer())
                                        {
                                            // Create geometry (via rowBuffer).
                                            rowBuffer[featureClassDefinitionNG911.GetShapeField()] = sgidFeature.GetShape();

                                            // Create attributes for direct transfer fields (via rowBuffer). //
                                            rowBuffer["Source"] = "AGRC";
                                            rowBuffer["StreetName"] = SgidPsapCursor.Current.GetOriginalValue(SgidPsapCursor.Current.FindField("NAME"));
                                            rowBuffer["DateUpdated"] = SgidPsapCursor.Current.GetOriginalValue(SgidPsapCursor.Current.FindField("UPDATED"));
                                            rowBuffer["FromAddr_L"] = SgidPsapCursor.Current.GetOriginalValue(SgidPsapCursor.Current.FindField("FROMADDR_L"));
                                            rowBuffer["ToAddr_L"] = SgidPsapCursor.Current.GetOriginalValue(SgidPsapCursor.Current.FindField("TOADDR_L"));
                                            rowBuffer["FromAddr_R"] = SgidPsapCursor.Current.GetOriginalValue(SgidPsapCursor.Current.FindField("FROMADDR_R"));
                                            rowBuffer["ToAddr_R"] = SgidPsapCursor.Current.GetOriginalValue(SgidPsapCursor.Current.FindField("TOADDR_R"));
                                            rowBuffer["Effective"] = SgidPsapCursor.Current.GetOriginalValue(SgidPsapCursor.Current.FindField("EFFECTIVE"));
                                            rowBuffer["Expire"] = SgidPsapCursor.Current.GetOriginalValue(SgidPsapCursor.Current.FindField("EXPIRE"));
                                            rowBuffer["RCL_NGUID"] = SgidPsapCursor.Current.GetOriginalValue(SgidPsapCursor.Current.FindField("UNIQUE_ID"));
                                            rowBuffer["Parity_L"] = SgidPsapCursor.Current.GetOriginalValue(SgidPsapCursor.Current.FindField("PARITY_L"));
                                            rowBuffer["Parity_R"] = SgidPsapCursor.Current.GetOriginalValue(SgidPsapCursor.Current.FindField("PARITY_R"));
                                            rowBuffer["ESN_L"] = SgidPsapCursor.Current.GetOriginalValue(SgidPsapCursor.Current.FindField("ESN_L"));
                                            rowBuffer["ESN_R"] = SgidPsapCursor.Current.GetOriginalValue(SgidPsapCursor.Current.FindField("ESN_R"));
                                            rowBuffer["MSAGComm_L"] = SgidPsapCursor.Current.GetOriginalValue(SgidPsapCursor.Current.FindField("MSAGCOMM_L"));
                                            rowBuffer["MSAGComm_R"] = SgidPsapCursor.Current.GetOriginalValue(SgidPsapCursor.Current.FindField("MSAGCOMM_R"));
                                            rowBuffer["Country_L"] = "US";
                                            rowBuffer["Country_R"] = "US";
                                            rowBuffer["State_L"] = "UT";
                                            rowBuffer["State_R"] = "UT";
                                            rowBuffer["IncMuni_L"] = SgidPsapCursor.Current.GetOriginalValue(SgidPsapCursor.Current.FindField("INCMUNI_L"));
                                            rowBuffer["IncMuni_R"] = SgidPsapCursor.Current.GetOriginalValue(SgidPsapCursor.Current.FindField("INCMUNI_R"));
                                            rowBuffer["UnincCom_L"] = SgidPsapCursor.Current.GetOriginalValue(SgidPsapCursor.Current.FindField("UNINCCOM_L"));
                                            rowBuffer["UnincCom_R"] = SgidPsapCursor.Current.GetOriginalValue(SgidPsapCursor.Current.FindField("UNINCCOM_R"));
                                            rowBuffer["NbrhdCom_L"] = SgidPsapCursor.Current.GetOriginalValue(SgidPsapCursor.Current.FindField("NBRHDCOM_L"));
                                            rowBuffer["NbrhdCom_R"] = SgidPsapCursor.Current.GetOriginalValue(SgidPsapCursor.Current.FindField("NBRHDCOM_R"));
                                            rowBuffer["PostCode_L"] = SgidPsapCursor.Current.GetOriginalValue(SgidPsapCursor.Current.FindField("ZIPCODE_L"));
                                            rowBuffer["PostCode_R"] = SgidPsapCursor.Current.GetOriginalValue(SgidPsapCursor.Current.FindField("ZIPCODE_R"));
                                            rowBuffer["PostComm_L"] = SgidPsapCursor.Current.GetOriginalValue(SgidPsapCursor.Current.FindField("POSTCOMM_L"));
                                            rowBuffer["PostComm_R"] = SgidPsapCursor.Current.GetOriginalValue(SgidPsapCursor.Current.FindField("POSTCOMM_R"));
                                            rowBuffer["SpeedLimit"] = SgidPsapCursor.Current.GetOriginalValue(SgidPsapCursor.Current.FindField("SPEED_LMT"));

                                            // Derive RoadClass from COFIPS.
                                            Int32  cofips = Convert.ToInt32(SgidPsapCursor.Current.GetOriginalValue(SgidPsapCursor.Current.FindField("CARTOCODE")));
                                            switch (cofips)
                                            {
                                                case 1: // Interstates
                                                    rowBuffer["RoadClass"] = "Primary";
                                                    break;
                                                case 2: // US Highways, Separated
                                                    rowBuffer["RoadClass"] = "Primary";
                                                    break;
                                                case 3: // US Highways, Unseparated
                                                    rowBuffer["RoadClass"] = "Secondary";
                                                    break;
                                                case 4: // Major State Highways, Separated
                                                    rowBuffer["RoadClass"] = "Primary";
                                                    break;
                                                case 5: // Major State Highways, Unseparated
                                                    rowBuffer["RoadClass"] = "Secondary";
                                                    break;
                                                case 6: // Other State Highways(Institutional)
                                                    rowBuffer["RoadClass"] = "Secondary";
                                                    break;
                                                case 7: // Ramps, Collectors
                                                    rowBuffer["RoadClass"] = "Ramp";
                                                    break;
                                                case 8: // Major Local Roads, Paved
                                                    rowBuffer["RoadClass"] = "Local";
                                                    break;
                                                case 9: // Major Local Roads, Not Paved
                                                    rowBuffer["RoadClass"] = "Local";
                                                    break;
                                                case 10: // Other Fedaral Aid Eligible Local Roads
                                                    rowBuffer["RoadClass"] = "Local";
                                                    break;
                                                case 11: // Other Local, Neighborhood, Rural Roads
                                                    rowBuffer["RoadClass"] = "Local";
                                                    break;
                                                case 12: // Other
                                                    rowBuffer["RoadClass"] = "Other";
                                                    break;
                                                case 13: // Non - road feature
                                                    rowBuffer["RoadClass"] = "Other";
                                                    break;
                                                case 14: // Driveway
                                                    rowBuffer["RoadClass"] = "Private";
                                                    break;
                                                case 15: // Proposed
                                                    rowBuffer["RoadClass"] = "";
                                                    break;
                                                case 16: // 4WD and/ or high clearance may be required
                                                    rowBuffer["RoadClass"] = "Vehicular Trail";
                                                    break;
                                                case 17: // Service Access Roads
                                                    rowBuffer["RoadClass"] = "Other";
                                                    break;
                                                case 18: // General Acces Roads
                                                    rowBuffer["RoadClass"] = "Other";
                                                    break;
                                                default:
                                                    rowBuffer["RoadClass"] = "N/A";
                                                    break;
                                            }

                                            // Derive OneWay from ONEWAY.
                                            //rowBuffer["OneWay"] = SgidPsapCursor.Current.GetOriginalValue(SgidPsapCursor.Current.FindField("ONEWAY"));
                                            string oneWay = SgidPsapCursor.Current.GetOriginalValue(SgidPsapCursor.Current.FindField("ONEWAY")).ToString();
                                            switch (oneWay)
                                            {
                                                case "0": // Two way
                                                    rowBuffer["OneWay"] = "B";
                                                    break;
                                                case "1": // One way Direction of Arc
                                                    rowBuffer["OneWay"] = "FT";
                                                    break;
                                                case "2": // One way Opposite Direction of Arc
                                                    rowBuffer["OneWay"] = "TF";
                                                    break;
                                                default:
                                                    //rowBuffer["OneWay"] = "N/A";
                                                    break;
                                            }


                                            // Create attributes for fields that need to Get Domain Description value (Street Type in NG911 is fully spelled out). //
                                            // St_PosTyp //
                                            string codedDomainValue = GetDomainValue.Execute(featureClassDefinitionSGID, SgidPsapCursor, "POSTTYPE", "POSTTYPE");
                                            codedDomainValue.Trim();
                                            if (codedDomainValue != "")
                                            {
                                                // Proper case.
                                                //codedDomainValue = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(codedDomainValue.ToLower());
                                                rowBuffer["St_PosTyp"] = codedDomainValue.ToUpper();
                                            }

                                            // St_Predir //
                                            codedDomainValue = GetDomainValue.Execute(featureClassDefinitionSGID, SgidPsapCursor, "PREDIR", "PREDIR");
                                            codedDomainValue.Trim();
                                            if (codedDomainValue != "")
                                            {
                                                // Proper case.
                                                //codedDomainValue = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(codedDomainValue.ToLower());
                                                rowBuffer["St_Predir"] = codedDomainValue.ToUpper();
                                            }

                                            // St_PosDir //
                                            codedDomainValue = GetDomainValue.Execute(featureClassDefinitionSGID, SgidPsapCursor, "POSTDIR", "POSTDIR");
                                            codedDomainValue.Trim();
                                            if (codedDomainValue != "")
                                            {
                                                // Proper case.
                                                //codedDomainValue = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(codedDomainValue.ToLower());
                                                rowBuffer["St_PosDir"] = codedDomainValue.ToUpper();
                                            }

                                            // County_L //
                                            codedDomainValue = GetDomainValue.Execute(featureClassDefinitionSGID, SgidPsapCursor, "COUNTY_L", "COUNTY_L");
                                            codedDomainValue.Trim();
                                            if (codedDomainValue != "")
                                            {
                                                string countyName = string.Empty;
                                                // Proper case.
                                                //codedDomainValue = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(codedDomainValue.ToLower());
                                                if (codedDomainValue.Any(char.IsDigit))
                                                {
                                                    // The County value contains the cofips, so parse the stirng to obtain the name only.
                                                    string[] parsedDomain = codedDomainValue.Split(' ');
                                                    countyName = parsedDomain[2];
                                                    countyName = countyName.Trim() + " County";
                                                }
                                                rowBuffer["County_L"] = countyName.ToUpper();
                                            }

                                            // County_R //
                                            codedDomainValue = GetDomainValue.Execute(featureClassDefinitionSGID, SgidPsapCursor, "COUNTY_R", "COUNTY_R");
                                            codedDomainValue.Trim();
                                            if (codedDomainValue != "")
                                            {
                                                string countyName = string.Empty;
                                                // Proper case.
                                                //codedDomainValue = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(codedDomainValue.ToLower());
                                                if (codedDomainValue.Any(char.IsDigit))
                                                {
                                                    // The County value contains the cofips, so parse the stirng to obtain the name only.
                                                    string[] parsedDomain = codedDomainValue.Split(' ');
                                                    countyName = parsedDomain[2];
                                                    countyName = countyName.Trim() + " County";
                                                }
                                                rowBuffer["County_R"] = countyName.ToUpper();
                                            }


                                            // Create attributtes for fields that need a spatial intersect (point in polygon query). //
                                            // get points
                                            // get intersected boundaries
                                            // test this field > County_L

                                            // create the row, with attributes and geometry via rowBuffer, in the ng911 database
                                            using (Row row = ng911Roads.CreateRow(rowBuffer))
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
                Console.WriteLine("There was an error with LoadRoads method. " +
                ex.Message + " " + ex.Source + " " + ex.InnerException + " " + ex.HResult + " " + ex.StackTrace + " " + ex);
            }
        }

    }
}

    // SGID ROADS FIELDS //
//STATUS is a type of String with a length of 15
//CARTOCODE is a type of String with a length of 10
//FULLNAME is a type of String with a length of 50
//FROMADDR_L is a type of Integer with a length of 4
//TOADDR_L is a type of Integer with a length of 4
//FROMADDR_R is a type of Integer with a length of 4
//TOADDR_R is a type of Integer with a length of 4
//PARITY_L is a type of String with a length of 1
//PARITY_R is a type of String with a length of 1
//PREDIR is a type of String with a length of 2
//NAME is a type of String with a length of 40
//POSTTYPE is a type of String with a length of 4
//POSTDIR is a type of String with a length of 2
//AN_NAME is a type of String with a length of 10
//AN_POSTDIR is a type of String with a length of 2
//A1_PREDIR is a type of String with a length of 2
//A1_NAME is a type of String with a length of 40
//A1_POSTTYPE is a type of String with a length of 4
//A1_POSTDIR is a type of String with a length of 2
//A2_PREDIR is a type of String with a length of 2
//A2_NAME is a type of String with a length of 40
//A2_POSTTYPE is a type of String with a length of 4
//A2_POSTDIR is a type of String with a length of 2
//QUADRANT_L is a type of String with a length of 2
//QUADRANT_R is a type of String with a length of 2
//STATE_L is a type of String with a length of 2
//STATE_R is a type of String with a length of 2
//COUNTY_L is a type of String with a length of 30
//COUNTY_R is a type of String with a length of 30
//ADDRSYS_L is a type of String with a length of 30
//ADDRSYS_R is a type of String with a length of 30
//POSTCOMM_L is a type of String with a length of 30
//POSTCOMM_R is a type of String with a length of 30
//ZIPCODE_L is a type of String with a length of 5
//ZIPCODE_R is a type of String with a length of 5
//INCMUNI_L is a type of String with a length of 30
//INCMUNI_R is a type of String with a length of 30
//UNINCCOM_L is a type of String with a length of 30
//UNINCCOM_R is a type of String with a length of 30
//NBRHDCOM_L is a type of String with a length of 30
//NBRHDCOM_R is a type of String with a length of 30
//ER_CAD_ZONES is a type of String with a length of 255
//ESN_L is a type of String with a length of 5
//ESN_R is a type of String with a length of 5
//MSAGCOMM_L is a type of String with a length of 30
//MSAGCOMM_R is a type of String with a length of 30
//ONEWAY is a type of String with a length of 1
//VERT_LEVEL is a type of String with a length of 1
//SPEED_LMT is a type of SmallInteger with a length of 2
//ACCESSCODE is a type of String with a length of 1
//DOT_HWYNAM is a type of String with a length of 15
//DOT_RTNAME is a type of String with a length of 11
//DOT_RTPART is a type of String with a length of 3
//DOT_F_MILE is a type of Double with a length of 8
//DOT_T_MILE is a type of Double with a length of 8
//DOT_FCLASS is a type of String with a length of 20
//DOT_SRFTYP is a type of String with a length of 30
//DOT_CLASS is a type of String with a length of 1
//DOT_OWN_L is a type of String with a length of 30
//DOT_OWN_R is a type of String with a length of 30
//DOT_AADT is a type of Integer with a length of 4
//DOT_AADTYR is a type of String with a length of 4
//DOT_THRULANES is a type of SmallInteger with a length of 2
//BIKE_L is a type of String with a length of 4
//BIKE_R is a type of String with a length of 4
//UNIQUE_ID is a type of String with a length of 75
//LOCAL_UID is a type of String with a length of 30
//UTAHRD_UID is a type of String with a length of 100
//SOURCE is a type of String with a length of 75
//UPDATED is a type of Date with a length of 36
//EFFECTIVE is a type of Date with a length of 36
//EXPIRE is a type of Date with a length of 36
//CUSTOMTAGS is a type of String with a length of 1000



    // NG911 ROADS FIELDS //
//Source is a type of String with a length of 75
//DateUpdated is a type of Date with a length of 8
//Effective is a type of Date with a length of 8
//Expire is a type of Date with a length of 8
//RCL_NGUID is a type of String with a length of 100
//AdNumPre_L is a type of String with a length of 15
//AdNumPre_R is a type of String with a length of 15
//FromAddr_L is a type of Integer with a length of 0
//ToAddr_L is a type of Integer with a length of 0
//FromAddr_R is a type of Integer with a length of 4
//ToAddr_R is a type of Integer with a length of 0
//Parity_L is a type of String with a length of 1
//Parity_R is a type of String with a length of 1
//St_PreMod is a type of String with a length of 9
//St_PreDir is a type of String with a length of 50
//St_PreTyp is a type of String with a length of 25
//St_PreSep is a type of String with a length of 20
//StreetName is a type of String with a length of 60
//St_PosTyp is a type of String with a length of 25
//St_PosDir is a type of String with a length of 9
//St_PosMod is a type of String with a length of 25
//LSt_PreDir is a type of String with a length of 2
//LSt_Name is a type of String with a length of 75
//LSt_Type is a type of String with a length of 5
//LStPosDir is a type of String with a length of 2
//ESN_L is a type of Integer with a length of 0
//ESN_R is a type of Integer with a length of 0
//MSAGComm_L is a type of String with a length of 30
//MSAGComm_R is a type of String with a length of 30
//Country_L is a type of String with a length of 2
//Country_R is a type of String with a length of 2
//State_L is a type of String with a length of 2
//State_R is a type of String with a length of 2
//County_L is a type of String with a length of 40
//County_R is a type of String with a length of 40
//AddCode_L is a type of String with a length of 6
//AddCode_R is a type of String with a length of 6
//IncMuni_L is a type of String with a length of 100
//IncMuni_R is a type of String with a length of 100
//UnincCom_L is a type of String with a length of 100
//UnincCom_R is a type of String with a length of 100
//NbrhdCom_L is a type of String with a length of 100
//NbrhdCom_R is a type of String with a length of 100
//PostCode_L is a type of String with a length of 7
//PostCode_R is a type of String with a length of 7
//PostComm_L is a type of String with a length of 40
//PostComm_R is a type of String with a length of 40
//RoadClass is a type of String with a length of 15
//OneWay is a type of String with a length of 2
//SpeedLimit is a type of SmallInteger with a length of 2
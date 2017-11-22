using ArcGIS.Core.Data;
using System;
using System.Collections.Generic;
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
                            // get SGID roads Feature Class.
                            using (FeatureClass sgidRoads = sgid.OpenDataset<FeatureClass>("SGID10.TRANSPORTATION.Roads"))
                            {
                                QueryFilter queryFilter = new QueryFilter
                                {
                                    // test the etl using axtell address system b/c there's only 15 segments in this address system
                                    WhereClause = "ADDRSYS_L = 'AXTELL'"
                                };

                                // Get a Cursor of SGID road features.
                                //using (RowCursor SgidPsapCursor = sgidRoads.Search(null, true))
                                using (RowCursor SgidPsapCursor = sgidRoads.Search(queryFilter, true))
                                {
                                    // Loop through the sgidPsap features.
                                    while (SgidPsapCursor.MoveNext())
                                    {
                                        // Get a feature class definition for the NG911 feature class.
                                        FeatureClassDefinition featureClassDefinition = ng911Roads.GetDefinition();

                                        //Row SgidRow = SgidPsapCursor.Current;
                                        Feature sgidFeature = (Feature)SgidPsapCursor.Current;

                                        // Create row buffer.
                                        using (RowBuffer rowBuffer = ng911Roads.CreateRowBuffer())
                                        {
                                            // Create geometry (via rowBuffer).
                                            rowBuffer[featureClassDefinition.GetShapeField()] = sgidFeature.GetShape();

                                            // Create attributes (via rowBuffer).
                                            rowBuffer["StreetName"] = SgidPsapCursor.Current.GetOriginalValue(SgidPsapCursor.Current.FindField("NAME"));

                                            // DOMAIN VALUES >>>
                                            //IReadOnlyList<Field> NG911fields = featureClassDefinition.GetFields();

                                            int NG911_Field = featureClassDefinition.FindField("St_PreTyp");
                                            Field NG911_NameField = featureClassDefinition.GetFields()[NG911_Field];

                                            Domain domain = NG911_NameField.GetDomain();
                                            if (domain is CodedValueDomain)
                                            {
                                                CodedValueDomain codedValueDomain = domain as CodedValueDomain;
                                                // get the domain description 
                                                string NG911_DomainValue = codedValueDomain.GetName(SgidPsapCursor.Current.GetOriginalValue(SgidPsapCursor.Current.FindField("POSTTYPE")));
                                                rowBuffer["St_PreTyp"] = NG911_DomainValue;

                                                Console.WriteLine(NG911_DomainValue);
                                            }
                                            // <<< DOMAIN VALUES


                                            // GET RIGHT/LEFT OFFSET OF MIDPOINT OF LINE SEGMENT >>>
                                            //County_L

                                            // <<< GET RIGHT/LEFT OFFSET OF MIDPOINT OF LINE SEGMENT

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
                Console.WriteLine("There was an error with LoadRoads method." +
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
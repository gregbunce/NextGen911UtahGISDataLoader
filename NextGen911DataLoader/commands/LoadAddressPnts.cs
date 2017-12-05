using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGen911DataLoader.commands
{
    class LoadAddressPnts
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
                        // Get access to NG911 feature class
                        using (FeatureClass ng911AddrPnts = NG911Utah.OpenDataset<FeatureClass>("AddressPoints"))
                        {
                            // delete all the existing rows
                            QueryFilter queryFilter = new QueryFilter
                            {
                                WhereClause = "OBJECTID > 0"
                            };
                            ng911AddrPnts.DeleteRows(queryFilter);

                            // get SGID Feature Classes.
                            using (FeatureClass sgidRoads = sgid.OpenDataset<FeatureClass>("SGID10.LOCATION.AddressPoints"), sgidZipCodes = sgid.OpenDataset<FeatureClass>("SGID10.BOUNDARIES.ZipCodes"))
                            {
                                QueryFilter queryFilter1 = new QueryFilter
                                {
                                    WhereClause = "AddSystem = 'ROCKVILLE'"
                                };

                                // Get a Cursor of SGID features.
                                using (RowCursor SgidCursor = sgidRoads.Search(queryFilter1, true))
                                {
                                    // Loop through the sgid features.
                                    while (SgidCursor.MoveNext())
                                    {
                                        // Get a feature class definition for the NG911 feature class.
                                        FeatureClassDefinition featureClassDefinitionNG911 = ng911AddrPnts.GetDefinition();

                                        // Get a feature class definition for the SGID feature class
                                        FeatureClassDefinition featureClassDefinitionSGID = sgidRoads.GetDefinition();

                                        //Row SgidRow = SgidCursor.Current;
                                        Feature sgidFeature = (Feature)SgidCursor.Current;

                                        // Create row buffer.
                                        using (RowBuffer rowBuffer = ng911AddrPnts.CreateRowBuffer())
                                        {
                                            // Create geometry (via rowBuffer).
                                            rowBuffer[featureClassDefinitionNG911.GetShapeField()] = sgidFeature.GetShape();

                                            // Create attributes for direct transfer fields (via rowBuffer). //
                                            rowBuffer["Source"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("AddSource"));
                                            rowBuffer["DateUpdate"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("LoadDate"));
                                            //rowBuffer["Effective"] = "";
                                            //rowBuffer["Expire"] = "";
                                            rowBuffer["Site_NGUID"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("UTAddPtID"));
                                            rowBuffer["Country"] = "US";
                                            rowBuffer["State"] = "UT";
                                            //rowBuffer["AddCode"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));
                                            //rowBuffer["AddDataURI"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));
                                            rowBuffer["Inc_Muni"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("City"));
                                            rowBuffer["Uninc_Comm"] = "";
                                            //rowBuffer["Nbrhd_Comm"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));
                                            //rowBuffer["AddNum_Pre"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));
                                            rowBuffer["Add_Number"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("AddNum"));
                                            rowBuffer["AddNum_Suf"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("AddNumSuffix"));
                                            //rowBuffer["St_PreMod"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));
                                            //rowBuffer["St_PreTyp"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));
                                            //rowBuffer["St_PreSep"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));
                                            rowBuffer["StreetName"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("StreetName"));
                                            //rowBuffer["St_PosMod"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));
                                            //rowBuffer["LSt_PreDir"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));
                                            //rowBuffer["LSt_Name"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));
                                            //rowBuffer["LSt_Type"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));
                                            //rowBuffer["LStPosDir"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));
                                            rowBuffer["ESN"] = "0";
                                            //rowBuffer["MSAGComm"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));
                                            rowBuffer["Post_Comm"] = "";
                                            rowBuffer["Post_Code"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("ZipCode"));            
                                            //rowBuffer["Post_Code4"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));
                                            rowBuffer["Building"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("Building"));
                                            //rowBuffer["Floor"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));
                                            rowBuffer["Unit"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("UnitID"));
                                            //rowBuffer["Room"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));
                                            //rowBuffer["Seat"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));
                                            rowBuffer["Addtl_Loc"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("AddSystem"));
                                            //rowBuffer["LandmkName"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));
                                            //rowBuffer["Mile_Post"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));
                                            rowBuffer["Placement"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("PtLocation"));
                                            MapPoint mapPoint = sgidFeature.GetShape() as MapPoint;
                                            rowBuffer["Long"] = mapPoint.Y;
                                            rowBuffer["Lat"] = mapPoint.X;
                                            //rowBuffer["Elev"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));



                                            //AddSystem is a type of String with a length of 40
                                            //UTAddPtID is a type of String with a length of 140
                                            //FullAdd is a type of String with a length of 100
                                            //AddNum is a type of String with a length of 10
                                            //AddNumSuffix is a type of String with a length of 4
                                            //PrefixDir is a type of String with a length of 1
                                            //StreetName is a type of String with a length of 50
                                            //StreetType is a type of String with a length of 4
                                            //SuffixDir is a type of String with a length of 1
                                            //LandmarkName is a type of String with a length of 75
                                            //Building is a type of String with a length of 75
                                            //UnitType is a type of String with a length of 20
                                            //UnitID is a type of String with a length of 20
                                            //City is a type of String with a length of 30
                                            //ZipCode is a type of String with a length of 5
                                            //CountyID is a type of String with a length of 15
                                            //State is a type of String with a length of 2
                                            //PtLocation is a type of String with a length of 30
                                            //PtType is a type of String with a length of 15
                                            //Structure is a type of String with a length of 10
                                            //ParcelID is a type of String with a length of 30
                                            //AddSource is a type of String with a length of 30
                                            //LoadDate is a type of Date with a length of 36
                                            //USNG is a type of String with a length of 10



                                            //// Derive OneWay from ONEWAY.
                                            ////rowBuffer["OneWay"] = SgidPsapCursor.Current.GetOriginalValue(SgidPsapCursor.Current.FindField("ONEWAY"));
                                            //string oneWay = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("ONEWAY")).ToString();
                                            //switch (oneWay)
                                            //{
                                            //    case "0": // Two way
                                            //        rowBuffer["OneWay"] = "B";
                                            //        break;
                                            //    case "1": // One way Direction of Arc
                                            //        rowBuffer["OneWay"] = "FT";
                                            //        break;
                                            //    case "2": // One way Opposite Direction of Arc
                                            //        rowBuffer["OneWay"] = "TF";
                                            //        break;
                                            //    default:
                                            //        //rowBuffer["OneWay"] = "N/A";
                                            //        break;
                                            //}


                                            // Create attributes for fields that need to Get Domain Description value (Street Type in NG911 is fully spelled out). //
                                            // CountyID //
                                            string codedDomainValue = GetDomainValue.Execute(featureClassDefinitionSGID, SgidCursor, "CountyID", "CountyID");
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
                                                rowBuffer["County"] = countyName.ToUpper();
                                            }

                                            // St_PosTyp //
                                            codedDomainValue = GetDomainValue.Execute(featureClassDefinitionSGID, SgidCursor, "StreetType", "StreetType");
                                            codedDomainValue.Trim();
                                            if (codedDomainValue != "")
                                            {
                                                // Proper case.
                                                //codedDomainValue = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(codedDomainValue.ToLower());
                                                rowBuffer["St_PosTyp"] = codedDomainValue.ToUpper();
                                            }

                                            // St_PreDir //
                                            codedDomainValue = GetDomainValue.Execute(featureClassDefinitionSGID, SgidCursor, "PrefixDir", "PrefixDir");
                                            codedDomainValue.Trim();
                                            if (codedDomainValue != "")
                                            {
                                                // Proper case.
                                                //codedDomainValue = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(codedDomainValue.ToLower());
                                                rowBuffer["St_PreDir"] = codedDomainValue.ToUpper();
                                            }

                                            // St_PosDir //
                                            codedDomainValue = GetDomainValue.Execute(featureClassDefinitionSGID, SgidCursor, "SuffixDir", "SuffixDir");
                                            codedDomainValue.Trim();
                                            if (codedDomainValue != "")
                                            {
                                                // Proper case.
                                                //codedDomainValue = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(codedDomainValue.ToLower());
                                                rowBuffer["St_PosDir"] = codedDomainValue.ToUpper();
                                            }



                                            //// Create attributtes for fields that need a spatial intersect (point in polygon query). >>> //
                                            //// Cast the feature to a Polyline.
                                            //Polyline polyline = sgidFeature.GetShape() as Polyline;

                                            //// Get the right and left offset points based on the midpoint of the polyline.
                                            //List<MapPoint> mapPoints = GetRightLeftOffsetPointsFromPolyline.Execute(polyline);

                                            //// Get intersected boundaries.
                                            //List<string> listOfFields = new List<string>(new string[] { "NAME", "ZIP5" });
                                            //List<string> retunedAttrValuesLeft = PointInPolygonQuery.Execute(mapPoints[0], sgidZipCodes, listOfFields);
                                            //List<string> retunedAttrValuesRight = PointInPolygonQuery.Execute(mapPoints[1], sgidZipCodes, listOfFields);

                                            //rowBuffer["PostComm_L"] = retunedAttrValuesLeft[0]; // 0 = NAME ; 1 = ZIP5
                                            //rowBuffer["PostComm_R"] = retunedAttrValuesLeft[0]; // 0 = NAME ; 1 = ZIP5
                                            //rowBuffer["PostCode_L"] = retunedAttrValuesLeft[1]; // 0 = NAME ; 1 = ZIP5
                                            //rowBuffer["PostCode_R"] = retunedAttrValuesLeft[1]; // 0 = NAME ; 1 = ZIP5
                                            //// <<< Create attributtes for fields that need a spatial intersect (point in polygon query). //


                                            // create the row, with attributes and geometry via rowBuffer, in the ng911 database
                                            using (Row row = ng911AddrPnts.CreateRow(rowBuffer))
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
                Console.WriteLine("There was an error with LoadAddressPnts method." +
                ex.Message + " " + ex.Source + " " + ex.InnerException + " " + ex.HResult + " " + ex.StackTrace + " " + ex);
            }
        }
    }
}

// ng911 address point fields //
//Source is a type of String with a length of 75
//DateUpdate is a type of Date with a length of 8
//Effective is a type of Date with a length of 8
//Expire is a type of Date with a length of 8
//Site_NGUID is a type of String with a length of 100
//Country is a type of String with a length of 2
//State is a type of String with a length of 2
//County is a type of String with a length of 40
//AddCode is a type of String with a length of 6
//AddDataURI is a type of String with a length of 254
//Inc_Muni is a type of String with a length of 100
//Uninc_Comm is a type of String with a length of 100
//Nbrhd_Comm is a type of String with a length of 100
//AddNum_Pre is a type of String with a length of 15
//Add_Number is a type of SmallInteger with a length of 2
//AddNum_Suf is a type of String with a length of 15
//St_PreMod is a type of String with a length of 15
//St_PreDir is a type of String with a length of 9
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
//ESN is a type of Integer with a length of 0
//MSAGComm is a type of String with a length of 30
//Post_Comm is a type of String with a length of 40
//Post_Code is a type of String with a length of 7
//Post_Code4 is a type of String with a length of 4
//Building is a type of String with a length of 75
//Floor is a type of String with a length of 75
//Unit is a type of String with a length of 75
//Room is a type of String with a length of 75
//Seat is a type of String with a length of 75
//Addtl_Loc is a type of String with a length of 225
//LandmkName is a type of String with a length of 150
//Mile_Post is a type of String with a length of 150
//Place_Type is a type of String with a length of 50
//Placement is a type of String with a length of 25
//Long is a type of Single with a length of 4
//Lat is a type of Single with a length of 4
//Elev is a type of SmallInteger with a length of 2



// sgid address point fields //
//AddSystem is a type of String with a length of 40
//UTAddPtID is a type of String with a length of 140
//FullAdd is a type of String with a length of 100
//AddNum is a type of String with a length of 10
//AddNumSuffix is a type of String with a length of 4
//PrefixDir is a type of String with a length of 1
//StreetName is a type of String with a length of 50
//StreetType is a type of String with a length of 4
//SuffixDir is a type of String with a length of 1
//LandmarkName is a type of String with a length of 75
//Building is a type of String with a length of 75
//UnitType is a type of String with a length of 20
//UnitID is a type of String with a length of 20
//City is a type of String with a length of 30
//ZipCode is a type of String with a length of 5
//CountyID is a type of String with a length of 15
//State is a type of String with a length of 2
//PtLocation is a type of String with a length of 30
//PtType is a type of String with a length of 15
//Structure is a type of String with a length of 10
//ParcelID is a type of String with a length of 30
//AddSource is a type of String with a length of 30
//LoadDate is a type of Date with a length of 36
//USNG is a type of String with a length of 10

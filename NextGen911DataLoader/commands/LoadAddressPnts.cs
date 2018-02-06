using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NextGen911DataLoader.extentions;

namespace NextGen911DataLoader.commands
{
    class LoadAddressPnts
    {
        public static void Execute(DatabaseConnectionProperties sgidConnectionProperties, string fgdbPath, StreamWriter streamWriter, bool truncate)
        {
            try
            {
                // Create connection path to the scratch database.
                string scratchFgdbPath = "C:\\temp\\ng911scratch.gdb";

                // connect to sgid, ng911, open feature classes and table
                using (Geodatabase sgid = new Geodatabase(sgidConnectionProperties))
                using (Geodatabase NG911Utah = new Geodatabase(new FileGeodatabaseConnectionPath(new Uri(fgdbPath))), NG911Scratch = new Geodatabase(new FileGeodatabaseConnectionPath(new Uri(scratchFgdbPath))))
                using (FeatureClass ng911_FeatClass = NG911Utah.OpenDataset<FeatureClass>("AddressPoints"))
                using (Table ng911_CompleteLandmarkTable = NG911Utah.OpenDataset<Table>("LandmarkNameCompleteAliasTable"), ng911_PartLandmarkTable = NG911Utah.OpenDataset<Table>("LandmarkNamePartTable"))
                {
                    // Create a row count bean-counter.
                    Int32 addressTableCreateRowCount = 1;
                    Int32 landmarkNameRowCount = 0;

                    // Check if the user wants to truncate the layer first
                    if (truncate)
                    {
                        Console.WriteLine("truncating address points feature class....");
                        string featClassLocation = fgdbPath + "\\" + ng911_FeatClass.GetName().ToString();
                        string pythonFile = "../../scripts_arcpy/TrancateTable.py";
                        commands.ExecuteArcpyScript.run_arcpy(pythonFile, featClassLocation);
                        Console.WriteLine("   done truncating address points feature class");
                    }

                    // get SGID Feature Classes - or the Scratch database Feature Class (uncomment the appropriate one)
                    //using (FeatureClass sourceAddrPnts = sgid.OpenDataset<FeatureClass>("SGID10.LOCATION.AddressPoints"), sgidZipCodes = sgid.OpenDataset<FeatureClass>("SGID10.BOUNDARIES.ZipCodes"))
                    using (FeatureClass sourceAddrPnts = NG911Scratch.OpenDataset<FeatureClass>("AddressPoints"), sgidZipCodes = sgid.OpenDataset<FeatureClass>("SGID10.BOUNDARIES.ZipCodes"))
                    {
                        QueryFilter queryFilter1 = new QueryFilter
                        {
                            //WhereClause = "AddSystem = 'SALT LAKE CITY' AND LandmarkName LIKE '%university%'"
                            //WhereClause = "AddSystem = 'BLANDING' AND STREETNAME = '300'"
                            //WhereClause = "OBJECTID > 9832368"
                        };

                        // Get a Cursor of SGID features.
                        using (RowCursor sourceCursor = sourceAddrPnts.Search(queryFilter1, true))
                        {
                            // Loop through the sgid features.
                            while (sourceCursor.MoveNext())
                            {
                                // Get a feature class definition for the NG911 feature class.
                                FeatureClassDefinition featureClassDefinitionNG911 = ng911_FeatClass.GetDefinition();

                                // Get a feature class definition for the SGID feature class
                                FeatureClassDefinition featureClassDefinitionSourceAddrPnts = sourceAddrPnts.GetDefinition();

                                //Row SgidRow = SgidCursor.Current;
                                Feature sourceFeature = (Feature)sourceCursor.Current;

                                // Create row buffer.
                                using (RowBuffer rowBuffer = ng911_FeatClass.CreateRowBuffer())
                                {
                                    try
                                    {
                                        if (sourceFeature.GetShape() != null)
                                        {
                                            // Create geometry (via rowBuffer).
                                            rowBuffer[featureClassDefinitionNG911.GetShapeField()] = sourceFeature.GetShape();

                                            // Create attributes for direct transfer fields (via rowBuffer). //
                                            rowBuffer["Source"] = sourceCursor.Current.GetOriginalValue(sourceCursor.Current.FindField("AddSource"));
                                            rowBuffer["DateUpdate"] = sourceCursor.Current.GetOriginalValue(sourceCursor.Current.FindField("LoadDate"));
                                            //rowBuffer["Effective"] = "";
                                            //rowBuffer["Expire"] = "";
                                            rowBuffer["Site_NGUID"] = sourceCursor.Current.GetOriginalValue(sourceCursor.Current.FindField("UTAddPtID")).ToString().Trim() + "@gis.utah.gov";
                                            //rowBuffer["Site_NGUID"] = "SITE" + SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("OBJECTID")) + "@gis.utah.gov";
                                            rowBuffer["Country"] = "US";
                                            rowBuffer["State"] = "UT";
                                            //rowBuffer["AddCode"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));
                                            //rowBuffer["AddDataURI"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));
                                            rowBuffer["Inc_Muni"] = sourceCursor.Current.GetOriginalValue(sourceCursor.Current.FindField("City"));
                                            rowBuffer["Uninc_Comm"] = "";
                                            //rowBuffer["Nbrhd_Comm"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));
                                            //rowBuffer["AddNum_Pre"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));

                                            //rowBuffer["Add_Number"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("AddNum"));
                                            // Check for alpha characters in the AddNum field.
                                            if (Regex.IsMatch(sourceCursor.Current.GetOriginalValue(sourceCursor.Current.FindField("AddNum")).ToString(), ".*?[a-zA-Z].*?"))
                                            {
                                                // Remove the number and log the procedure
                                                string addNumAlphaRemoved = sourceCursor.Current.GetOriginalValue(sourceCursor.Current.FindField("AddNum")).ToString();
                                                addNumAlphaRemoved = Regex.Replace(addNumAlphaRemoved, "[^0-9.]", "");

                                                rowBuffer["Add_Number"] = addNumAlphaRemoved;
                                                streamWriter.WriteLine("AddressPoints" + "," + sourceCursor.Current.GetOriginalValue(sourceCursor.Current.FindField("OBJECTID")).ToString() + "," + addressTableCreateRowCount + "," + "Removed alpha character in AddNum");
                                            }
                                            else
                                            {
                                                rowBuffer["Add_Number"] = sourceCursor.Current.GetOriginalValue(sourceCursor.Current.FindField("AddNum"));
                                            }

                                            rowBuffer["AddNum_Suf"] = sourceCursor.Current.GetOriginalValue(sourceCursor.Current.FindField("AddNumSuffix"));
                                            //rowBuffer["St_PreMod"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));
                                            //rowBuffer["St_PreTyp"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));
                                            //rowBuffer["St_PreSep"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));
                                            rowBuffer["StreetName"] = sourceCursor.Current.GetOriginalValue(sourceCursor.Current.FindField("StreetName"));
                                            //rowBuffer["St_PosMod"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));
                                            //rowBuffer["LSt_PreDir"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));
                                            //rowBuffer["LSt_Name"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));
                                            //rowBuffer["LSt_Type"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));
                                            //rowBuffer["LStPosDir"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));
                                            rowBuffer["ESN"] = "0";
                                            //rowBuffer["MSAGComm"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));
                                            //rowBuffer["Post_Comm"] = "";
                                            rowBuffer["Post_Code"] = sourceCursor.Current.GetOriginalValue(sourceCursor.Current.FindField("ZipCode"));            
                                            //rowBuffer["Post_Code4"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));
                                            rowBuffer["Building"] = sourceCursor.Current.GetOriginalValue(sourceCursor.Current.FindField("Building"));
                                            //rowBuffer["Floor"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));
                                            rowBuffer["Unit"] = sourceCursor.Current.GetOriginalValue(sourceCursor.Current.FindField("UnitID"));
                                            //rowBuffer["Room"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));
                                            //rowBuffer["Seat"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));
                                            rowBuffer["Addtl_Loc"] = sourceCursor.Current.GetOriginalValue(sourceCursor.Current.FindField("AddSystem"));
                                            rowBuffer["LandmkName"] = sourceCursor.Current.GetOriginalValue(sourceCursor.Current.FindField("LandmarkName"));
                                            //rowBuffer["Mile_Post"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));
                                            //rowBuffer["Place_Type"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("PtType"));
                                            //rowBuffer["Placement"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("PtLocation"));
                                            MapPoint mapPoint = sourceFeature.GetShape() as MapPoint;
                                            MapPoint mapPointReprojected = ReprojectPoint.Execute(mapPoint, 4326);
                                            rowBuffer["Long"] = mapPointReprojected.X;
                                            rowBuffer["Lat"] = mapPointReprojected.Y;
                                            //rowBuffer["Elev"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));


                                            // Derive Place_Type from PtType.
                                            if (!(sourceCursor.Current.GetOriginalValue(sourceCursor.Current.FindField("PtType")) == null))
                                            {
                                                string placeType = sourceCursor.Current.GetOriginalValue(sourceCursor.Current.FindField("PtType")).ToString();
                                                if (placeType.Trim() != "")
                                                {
                                                    switch (placeType)
                                                    {
                                                        case "Agricultural":
                                                            rowBuffer["Place_Type"] = "";
                                                            break;
                                                        case "BASE ADDRESS":
                                                            rowBuffer["Place_Type"] = "Government-base";
                                                            break;
                                                        case "Business":
                                                            rowBuffer["Place_Type"] = "";
                                                            break;
                                                        case "Commercial":
                                                            rowBuffer["Place_Type"] = "";
                                                            break;
                                                        case "Education":
                                                            rowBuffer["Place_Type"] = "School";
                                                            break;
                                                        case "Government":
                                                            rowBuffer["Place_Type"] = "Government";
                                                            break;
                                                        case "Industrial":
                                                            rowBuffer["Place_Type"] = "Industrial";
                                                            break;
                                                        case "Med":
                                                            rowBuffer["Place_Type"] = "Hospital";
                                                            break;
                                                        case "Mixed Use":
                                                            rowBuffer["Place_Type"] = "";
                                                            break;
                                                        case "OTH":
                                                            rowBuffer["Place_Type"] = "Other";
                                                            break;
                                                        case "Other":
                                                            rowBuffer["Place_Type"] = "Other";
                                                            break;
                                                        case "Residential":
                                                            rowBuffer["Place_Type"] = "Residence";
                                                            break;
                                                        case "Unknown":
                                                            rowBuffer["Place_Type"] = "Unknown";
                                                            break;
                                                        case "Vacant":
                                                            rowBuffer["Place_Type"] = "";
                                                            break;
                                                        default:
                                                            if (placeType != "")
                                                            {
                                                                rowBuffer["Place_Type"] = "N/A";
                                                                // write out an error report as there may be a new domain
                                                                streamWriter.WriteLine("AddressPoints" + "," + sourceCursor.Current.GetOriginalValue(sourceCursor.Current.FindField("OBJECTID")).ToString() + "," + addressTableCreateRowCount + "," + "SGID Domain for PtType is not recognized : " + sourceCursor.Current.GetOriginalValue(sourceCursor.Current.FindField("PtType")).ToString() + ".. Possibly new?");
                                                            }
                                                            break;
                                                    }
                                                }
                                            }


                                            // Derive Placement from PtLocation.
                                            if (!(sourceCursor.Current.GetOriginalValue(sourceCursor.Current.FindField("PtLocation")) == null))
                                            {
                                                string pntLocation = sourceCursor.Current.GetOriginalValue(sourceCursor.Current.FindField("PtLocation")).ToString();
                                                if (pntLocation.Trim() != "")
                                                {
                                                    switch (pntLocation)
                                                    {
                                                        case "Centroid":
                                                            rowBuffer["Placement"] = "";
                                                            break;
                                                        case "Driveway Entrance":
                                                            rowBuffer["Placement"] = "Property Access";
                                                            break;
                                                        case "Geocoded":
                                                            rowBuffer["Placement"] = "Geocoding";
                                                            break;
                                                        case "Other":
                                                            rowBuffer["Place_Type"] = "";
                                                            break;
                                                        case "Parcel Centroid":
                                                            rowBuffer["Placement"] = "Parcel";
                                                            break;
                                                        case "Primary Structure Entrance":
                                                            rowBuffer["Placement"] = "Structure";
                                                            break;
                                                        case "Residential":
                                                            rowBuffer["Placement"] = "Site";
                                                            break;
                                                        case "Rooftop":
                                                            rowBuffer["Placement"] = "";
                                                            break;
                                                        case "Unknown":
                                                            rowBuffer["Placement"] = "Unknown";
                                                            break;
                                                        default:
                                                            if (pntLocation != "")
                                                            {
                                                                rowBuffer["Placement"] = "N/A";
                                                                // write out an error report as there may be a new domain
                                                                streamWriter.WriteLine("AddressPoints" + "," + sourceCursor.Current.GetOriginalValue(sourceCursor.Current.FindField("OBJECTID")).ToString() + "," + addressTableCreateRowCount + "," + "SGID Domain for PtLocation is not recognized: " + sourceCursor.Current.GetOriginalValue(sourceCursor.Current.FindField("PtLocation")).ToString() + ". Possibly new?");
                                                            }
                                                            break;
                                                    }
                                                }
                                            }


                                            // Create attributes for fields that need to Get Domain Description value (Street Type in NG911 is fully spelled out). //
                                            // CountyID //
                                            string codedDomainValue = GetDomainValue.Execute(featureClassDefinitionSourceAddrPnts, sourceCursor, "CountyID", "CountyID");
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
                                            codedDomainValue = GetDomainValue.Execute(featureClassDefinitionSourceAddrPnts, sourceCursor, "StreetType", "StreetType");
                                            codedDomainValue.Trim();
                                            if (codedDomainValue != "")
                                            {
                                                // Proper case.
                                                //codedDomainValue = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(codedDomainValue.ToLower());
                                                rowBuffer["St_PosTyp"] = codedDomainValue.ToUpper();
                                            }

                                            // St_PreDir //
                                            ////codedDomainValue = GetDomainValue.Execute(featureClassDefinitionSourceAddrPnts, sourceCursor, "PrefixDir", "PrefixDir");
                                            ////codedDomainValue.Trim();
                                            ////if (codedDomainValue != "")
                                            ////{
                                            ////    // Proper case.
                                            ////    //codedDomainValue = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(codedDomainValue.ToLower());
                                            ////    rowBuffer["St_PreDir"] = codedDomainValue.ToUpper();
                                            ////}
                                            if (!(sourceCursor.Current.GetOriginalValue(sourceCursor.Current.FindField("PrefixDir")) == null))
                                            {
                                                string preFixDir = sourceCursor.Current.GetOriginalValue(sourceCursor.Current.FindField("PrefixDir")).ToString();
                                                if (preFixDir.Trim() != "")
                                                {
                                                    switch (preFixDir)
                                                    {
                                                        case "N":
                                                            rowBuffer["St_PreDir"] = "NORTH";
                                                            break;
                                                        case "S":
                                                            rowBuffer["St_PreDir"] = "SOUTH";
                                                            break;
                                                        case "E":
                                                            rowBuffer["St_PreDir"] = "EAST";
                                                            break;
                                                        case "W":
                                                            rowBuffer["St_PreDir"] = "WEST";
                                                            break;
                                                        default:
                                                            rowBuffer["St_PreDir"] = "";
                                                            break;
                                                    }
                                                }
                                            }


                                            // St_PosDir //
                                            ////codedDomainValue = GetDomainValue.Execute(featureClassDefinitionSourceAddrPnts, sourceCursor, "SuffixDir", "SuffixDir");
                                            ////codedDomainValue.Trim();
                                            ////if (codedDomainValue != "")
                                            ////{
                                            ////    // Proper case.
                                            ////    //codedDomainValue = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(codedDomainValue.ToLower());
                                            ////    rowBuffer["St_PosDir"] = codedDomainValue.ToUpper();
                                            ////}
                                            if (!(sourceCursor.Current.GetOriginalValue(sourceCursor.Current.FindField("SuffixDir")) == null))
                                            {
                                                string suffixDir = sourceCursor.Current.GetOriginalValue(sourceCursor.Current.FindField("SuffixDir")).ToString();
                                                if (suffixDir.Trim() != "")
                                                {
                                                    switch (suffixDir)
                                                    {
                                                        case "N":
                                                            rowBuffer["St_PosDir"] = "NORTH";
                                                            break;
                                                        case "S":
                                                            rowBuffer["St_PosDir"] = "SOUTH";
                                                            break;
                                                        case "E":
                                                            rowBuffer["St_PosDir"] = "EAST";
                                                            break;
                                                        case "W":
                                                            rowBuffer["St_PosDir"] = "WEST";
                                                            break;
                                                        default:
                                                            rowBuffer["St_PosDir"] = "";
                                                            break;
                                                    }
                                                }
                                            }


                                            //// >>> Create attributtes for fields that need a spatial intersect (point in polygon query).
                                            //// Get intersected boundaries.
                                            //List<string> listOfFields = new List<string>(new string[] { "NAME", "ZIP5" });
                                            //List<string> retunedAttrValues = PointInPolygonQuery.Execute(mapPoint, sgidZipCodes, listOfFields);

                                            //// Check if null values were returned.
                                            //if (retunedAttrValues.Count() != 0)
                                            //{
                                            //    rowBuffer["Post_Comm"] = retunedAttrValues[0]; // 0 = NAME ; 1 = ZIP5
                                            //    rowBuffer["Post_Code"] = retunedAttrValues[1]; // 0 = NAME ; 1 = ZIP5
                                            //}
                                            //// <<< Create attributtes for fields that need a spatial intersect (point in polygon query).


                                            // create the row, with attributes and geometry via rowBuffer, in the ng911 database
                                            using (Row row = ng911_FeatClass.CreateRow(rowBuffer))
                                            {
                                                Console.WriteLine("AddrPntRowCount: " + addressTableCreateRowCount);
                                                Console.WriteLine("AddrPntSourceOID: " + sourceCursor.Current.GetOriginalValue(sourceCursor.Current.FindField("OBJECTID")).ToString());
                                                addressTableCreateRowCount = addressTableCreateRowCount + 1;
                                            }

                                            //////LANDMARK NAME TABLE >>>
                                            //////Check if the sgid addrpnt contains a value in the LandmarkName field.
                                            ////if (!(SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("LandmarkName")).ToString().IsEmpty()))
                                            ////{
                                            ////    if (SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("LandmarkName")).ToString().Trim() == "UNIVERSITY OF UTAH")
                                            ////    {
                                            ////        landmarkNameRowCount = landmarkNameRowCount + 1;
                                            ////        //Add landmark name to landmarkname table.
                                            ////        AddRowToCompleteLandmarkTable.Execute(featureClassDefinitionSGID, ng911_CompleteLandmarkTable, SgidCursor, streamWriter, landmarkNameRowCount, "UOFU");
                                            ////        landmarkNameRowCount = landmarkNameRowCount + 1;
                                            ////        //Add landmark name to landmarkname table.
                                            ////        AddRowToCompleteLandmarkTable.Execute(featureClassDefinitionSGID, ng911_CompleteLandmarkTable, SgidCursor, streamWriter, landmarkNameRowCount, "U OF U");
                                            ////    }
                                            ////}
                                            ////// <<< LANDMARK NAME TABLE

                                        }
                                        else // null geometry was found in the sgid address point layer, skip this feature and log it to the report.
                                        {
                                            streamWriter.WriteLine("AddressPoints" + "," + sourceCursor.Current.GetOriginalValue(sourceCursor.Current.FindField("OBJECTID")).ToString() + "," + addressTableCreateRowCount + "," + "Null geometry found in SGID Feature");
                                            Console.WriteLine("ObjectID: " + sourceCursor.Current.GetOriginalValue(sourceCursor.Current.FindField("OBJECTID")).ToString());
                                        }
                                    }
                                    catch (GeodatabaseException exObj) // trap errors
                                    {
                                        if (rowBuffer != null)
                                            rowBuffer.Dispose();

                                        //if (row != null)
                                        //    row.Dispose();

                                        using (RowBuffer rowBuffer2 = ng911_FeatClass.CreateRowBuffer())
                                        {
                                            rowBuffer2["StreetName"] = "ERROR";
                                            rowBuffer2["Source"] = sourceCursor.Current.GetOriginalValue(sourceCursor.Current.FindField("OBJECTID")).ToString();

                                            streamWriter.WriteLine("AddressPoints" + "," + sourceCursor.Current.GetOriginalValue(sourceCursor.Current.FindField("OBJECTID")).ToString() + "," + addressTableCreateRowCount + "," + "Caught Error: Check StreetName = Error.");

                                            using (Row row2 = ng911_FeatClass.CreateRow(rowBuffer2))
                                            {
                                            }
                                        }
                                    }

                                }

                                ////LANDMARK NAME TABLE >>>
                                ////Check if the sgid addrpnt contains a value in the LandmarkName field.
                                //if (SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("LandmarkName")) != "" || !(SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("LandmarkName")) == null))
                                //{
                                //    if (SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("LandmarkName")).ToString().Trim() == "UNIVERSITY OF UTAH")
                                //    {
                                //        landmarkNameRowCount = landmarkNameRowCount + 1;
                                //        //Add landmark name to landmarkname table.
                                //        AddRowToCompleteLandmarkTable.Execute(featureClassDefinitionSGID, ng911_CompleteLandmarkTable, SgidCursor, streamWriter, landmarkNameRowCount, "UOFU");
                                //        landmarkNameRowCount = landmarkNameRowCount + 1;
                                //        //Add landmark name to landmarkname table.
                                //        AddRowToCompleteLandmarkTable.Execute(featureClassDefinitionSGID, ng911_CompleteLandmarkTable, SgidCursor, streamWriter, landmarkNameRowCount, "U OF U");
                                //    }
                                //}
                                //// <<< LANDMARK NAME TABLE
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was an error with LoadAddressPnts method." +
                ex.Message + " " + ex.Source + " " + ex.InnerException + " " + ex.HResult + " " + ex.StackTrace + " " + ex);

                streamWriter.WriteLine();
                streamWriter.WriteLine("ERROR MESSAGE...");
                streamWriter.WriteLine("_______________________________________");
                streamWriter.WriteLine("There was an error with LoadAddressPnts method." +
                ex.Message + " " + ex.Source + " " + ex.InnerException + " " + ex.HResult + " " + ex.StackTrace + " " + ex);
            }
        }
    }
}


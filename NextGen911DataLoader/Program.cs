//using System;
//using ArcGIS.Core.Data;
//using System.IO;
using ArcGIS.Core.Hosting;
using ArcGIS.Core.Internal.CIM;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NextGen911DataLoader
{

    class Program
    {
        //[STAThread] must be present on the Application entry point
        [STAThread]

        static void Main(string[] args)
        {
            //Call Host.Initialize before constructing any objects from ArcGIS.Core
            Host.Initialize();

            bool etlRoads = false;
            bool truncateRoads = false;
            bool etlAddresspoints = false;
            bool truncateAddressPoints = false;
            bool etlPsaps = false;
            bool truncatePsaps = false;
            bool etlInc = false;
            bool truncateInc = false;
            bool etlUnInc = false;
            bool truncateUnInc = false;
            bool etlCounties = false;
            bool truncateCounties = false;
            bool etlMileMarkers = false;
            bool truncateMileMarkers = false;
            bool etlRailRoads = false;
            bool truncateRailRoads = false;
            bool etlLawEnforcement = false;
            bool truncateLawEnforcement = false;
            bool etlEMS = false;
            bool truncateEMS = false;
            bool etlFire = false;
            bool truncateFire = false;
            bool etlHydroLines = false;
            bool truncateHydroLines = false;
            bool etlHydroPoly = false;
            bool truncateHydroPoly = false;
            bool etlCellTower = false;
            bool truncateCellTower = false;

            // Check that minimum command line args are present.
            if (!(args.Length > 4))  
            {
                Console.WriteLine("You must provide the following command line arguments: [location of output fgdb database], [sde instance], [sde database name], [sde user/pass], [list of valid layer names to elt (in any order)(append -t to layer name if you want to truncate the layer before the load. ex: law-t): roads, addresspoints, psaps, inc, uninc, counties, milemarkers, railroads, law, ems, fire, hydroLine, hydroPoly]");
                Console.Read();
                return;
            }


            // Check if the scratch geodatabase exists, if so rename it with today's date.
            if (Directory.Exists("C:/temp/ng911scratch.gdb"))
            {
                Console.WriteLine("Rename the scratch database.");

                // Rename the existing fgdb.
                string renameDate = DateTime.Now.ToString("yyyyMMddHHmm");

                string pythonFileRename = @"C:\Users\gbunce\source\repos\NextGen911DataLoader\NextGen911DataLoader\scripts_arcpy\RenameFGDB.py";
                commands.ExecuteArcpyScript.run_arcpy(pythonFileRename, renameDate);

            }
            else
            {
                // Do nothing.
            }

            // Create a new (scratch) file geodatabase via arcpy script.
            Console.WriteLine("Creating scratch fgdb database for sgid layers....");
            string pythonFileCreateFGDB = @"C:\Users\gbunce\source\repos\NextGen911DataLoader\NextGen911DataLoader\scripts_arcpy\CreateFileGeodatabase.py";
            string dateNow = DateTime.Now.ToString("yyyyMMdd_HHmm");
            commands.ExecuteArcpyScript.run_arcpy(pythonFileCreateFGDB, @"C:/temp", "ng911scratch");
            Console.WriteLine("Done creating scratch fgdb database.");

            // Setup a file stream and a stream writer to write out the log and any odd unexpected stuff that happens.
            string path = @"C:\temp\NextGen911DataLoaderLog" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + ".txt";
            FileStream fileStream = new FileStream(path, FileMode.Create);
            StreamWriter streamWriter = new StreamWriter(fileStream);
            string startTime = ("Started at: " + DateTime.Now.ToString());
            streamWriter.WriteLine("List of Feature Classes that were ETL'd:");

            // Check what layers the user wants to etl and if they want to truncate the existing data.
            foreach (string s in args)
            {
                System.Console.WriteLine(s);

                switch (s.ToUpper()) // make the argument upper to allow the user to use any casing.
                {
                    case "ROADS":
                    case "ROADS-T":
                        if (s.ToUpper() == "ROADS-T")
                        {
                            streamWriter.WriteLine(" *" + s);
                            etlRoads = true;
                            truncateRoads = true;
                        }
                        else
                        {
                            streamWriter.WriteLine(" *" + s);
                            etlRoads = true;
                            truncateRoads = false;
                        }
                        break;
                    case "ADDRESSPOINTS":
                    case "ADDRESSPOINTS-T":
                        if (s.ToUpper() == "ADDRESSPOINTS-T")
                        {
                            streamWriter.WriteLine(" *" + s);
                            etlAddresspoints = true;
                            truncateAddressPoints = true;
                        }
                        else
                        {
                            streamWriter.WriteLine(" *" + s);
                            etlAddresspoints = true;
                            truncateAddressPoints = false;
                        }
                        break;
                    case "PSAPS":
                    case "PSAPS-T":
                        if (s.ToUpper() == "PSAPS-T")
                        {
                            streamWriter.WriteLine(" *" + s);
                            etlPsaps = true;
                            truncatePsaps = true;
                        }
                        else
                        {
                            streamWriter.WriteLine(" *" + s);
                            etlPsaps = true;
                            truncatePsaps = false;
                        }
                        break;
                    case "INC":
                    case "INC-T":
                        if (s.ToUpper() == "INC-T")
                        {
                            streamWriter.WriteLine(" *" + s);
                            etlInc = true;
                            truncateInc = true;
                        }
                        else
                        {
                            streamWriter.WriteLine(" *" + s);
                            etlInc = true;
                            truncateInc = false;
                        }
                        break;
                    case "UNINC":
                    case "UNINC-T":
                        if (s.ToUpper() == "UNINC-T")
                        {
                            streamWriter.WriteLine(" *" + s);
                            etlUnInc = true;
                            truncateUnInc = true;
                        }
                        else
                        {
                            streamWriter.WriteLine(" *" + s);
                            etlUnInc = true;
                            truncateUnInc = false;
                        }
                        break;
                    case "COUNTIES":
                    case "COUNTIES-T":
                        if (s.ToUpper() == "COUNTIES-T")
                        {
                            streamWriter.WriteLine(" *" + s);
                            etlCounties = true;
                            truncateCounties = true;
                        }
                        else
                        {
                            streamWriter.WriteLine(" *" + s);
                            etlCounties = true;
                            truncateCounties = false;
                        }
                        break;
                    case "MILEMARKERS":
                    case "MILEMARKERS-T":
                        if (s.ToUpper() == "MILEMARKERS-T")
                        {
                            streamWriter.WriteLine(" *" + s);
                            etlMileMarkers = true;
                            truncateMileMarkers = true;
                        }
                        else
                        {
                            streamWriter.WriteLine(" *" + s);
                            etlMileMarkers = true;
                            truncateMileMarkers = false;
                        }
                        break;
                    case "RAILROADS":
                    case "RAILROADS-T":
                        if (s.ToUpper() == "RAILROADS-T")
                        {
                            streamWriter.WriteLine(" *" + s);
                            etlRailRoads = true;
                            truncateRailRoads = true;
                        }
                        else
                        {
                            streamWriter.WriteLine(" *" + s);
                            etlRailRoads = true;
                            truncateRailRoads = false;
                        }
                        break;
                    case "LAW":
                    case "LAW-T":
                        if (s.ToUpper() == "LAW-T")
                        {
                            streamWriter.WriteLine(" *" + s);
                            etlLawEnforcement = true;
                            truncateLawEnforcement = true;
                        }
                        else
                        {
                            streamWriter.WriteLine(" *" + s);
                            etlLawEnforcement = true;
                            truncateLawEnforcement = false;
                        }
                        break;
                    case "EMS":
                    case "EMS-T":
                        if (s.ToUpper() == "EMS-T")
                        {
                            streamWriter.WriteLine(" *" + s);
                            etlEMS = true;
                            truncateEMS = true;
                        }
                        else
                        {
                            streamWriter.WriteLine(" *" + s);
                            etlEMS = true;
                            truncateEMS = false;
                        }
                        break;
                    case "FIRE":
                    case "FIRE-T":
                        if (s.ToUpper() == "FIRE-T")
                        {
                            streamWriter.WriteLine(" *" + s);
                            etlFire = true;
                            truncateFire = true;
                        }
                        else
                        {
                            streamWriter.WriteLine(" *" + s);
                            etlFire = true;
                            truncateFire = false;
                        }
                        break;
                    case "HYDROLINE":
                    case "HYDROLINE-T":
                        if (s.ToUpper() == "HYDROLINE-T")
                        {
                            streamWriter.WriteLine(" *" + s);
                            etlHydroLines = true;
                            truncateHydroLines = true;
                        }
                        else
                        {
                            streamWriter.WriteLine(" *" + s);
                            etlHydroLines = true;
                            truncateHydroLines = false;
                        }
                        break;
                    case "HYDROPOLY":
                    case "HYDROPOLY-T":
                        if (s.ToUpper() == "HYDROPOLY-T")
                        {
                            streamWriter.WriteLine(" *" + s);
                            etlHydroPoly = true;
                            truncateHydroPoly = true;
                        }
                        else
                        {
                            streamWriter.WriteLine(" *" + s);
                            etlHydroPoly = true;
                            truncateHydroPoly = false;
                        }
                        break;
                    case "CELLTOWER":
                    case "CELLTOWER-T":
                        if (s.ToUpper() == "CELLTOWER-T")
                        {
                            streamWriter.WriteLine(" *" + s);
                            etlCellTower = true;
                            truncateCellTower = true;
                        }
                        else
                        {
                            streamWriter.WriteLine(" *" + s);
                            etlCellTower = true;
                            truncateCellTower = false;
                        }
                        break;
                    default:
                        break;
                }
            }

            // Write out the field headings.
            streamWriter.WriteLine();
            streamWriter.WriteLine("ADJUSTED DATA DURING ETL >>> REPORT/WARNING...");
            streamWriter.WriteLine("_______________________________________");
            streamWriter.WriteLine("FeatureType" + "," + "SGID_OID" + "," + "NextGen_OID" + "," + "Notes");


            //// Host.Initialize before constructing any objects from ArcGIS.Core
            //try
            //{
            //    ArcGIS.Core.Hosting.Host.Initialize();
            //}
            //catch (Exception e)
            //{
            //    // Error (missing installation, no license, 64 bit mismatch, etc.)
            //    Console.WriteLine(string.Format("Initialization failed: {0}", e.Message));
            //    return;
            //}

            string fgdbPath = args[0];

            //// Connect to File Geodatabase
            //Geodatabase NG911Utah = new Geodatabase(new FileGeodatabaseConnectionPath(new Uri(args[0])));

            // Connect to SDID Database //
            ArcGIS.Core.Data.DatabaseConnectionProperties sgidConnectionProperties = commands.ConnectToSGID.Execute(args[1], args[2], args[3]);

            // Populate the CountyNumber/CountyName Dictionary lookup... for use later thoughout the project.
            commands.GetCountyNameFromNumber.Execute(sgidConnectionProperties, fgdbPath, streamWriter);

            // Populate the zipcode/postalcommname Dictionary lookup... for use later thoughout the project.
            commands.GetPostalCommFromNumber.Execute(sgidConnectionProperties, fgdbPath, streamWriter);

            // ETL Psap Data to NG911
            if (etlPsaps)
            {
                commands.LoadPsapData.Execute(sgidConnectionProperties, fgdbPath, streamWriter, truncatePsaps);
            }

            // ETL Roads Data to NG911
            if (etlRoads)
            {
                // Export the SGID roads into the local scratch database and etl those (in case we have database connectivity issues, etc.)
                Console.WriteLine("importing roads...");
                //string sgidRoads = "C:/Users/gbunce/AppData/Roaming/ESRI/ArcGISPro/Favorites/internal@sgid@internal.agrc.utah.gov.sde/SGID.Transportation.Roads";
                string sgidRoads = "C:/Users/gbunce/Documents/projects/SGID/local_sgid_data/SGID_2022_10_6.gdb/Roads";
                string pythonFileExportData = @"C:\Users\gbunce\source\repos\NextGen911DataLoader\NextGen911DataLoader\scripts_arcpy\ExportFeatClassToScratchFGDB.py";
                string where_clause = @"""(FROMADDR_L > 0 and TOADDR_L > 0 and FROMADDR_R > 0 and TOADDR_R > 0) and STATUS not in ( 'Construction', 'Planned' ) and NAME <> '' and (STATE_L = 'UT' and STATE_R = 'UT')"""; 
                commands.ExecuteArcpyScript.run_arcpy(pythonFileExportData, sgidRoads, "Roads", where_clause);
                Console.WriteLine("Done importing roads");

                // Call etl code.
                commands.LoadRoads.Execute(sgidConnectionProperties, fgdbPath, streamWriter, truncateRoads);
            }

            // ETL address point to NG911
            if (etlAddresspoints)
            {
                // Export the SGID roads into the local scratch database and etl those (in case we have database connectivity issues, etc.)
                Console.WriteLine("importing address points...");
                //string sgidAddressPnts = "C:/Users/gbunce/AppData/Roaming/Esri/ArcGISPro/Favorites/internal@sgid@internal.agrc.utah.gov.sde/SGID.Location.AddressPoints";
                string sgidAddressPnts = "C:/Users/gbunce/Documents/projects/SGID/local_sgid_data/SGID_2022_10_6.gdb/AddressPoints";
                string pythonFileExportData = @"C:\Users\gbunce\source\repos\NextGen911DataLoader\NextGen911DataLoader\scripts_arcpy\ExportFeatClassToScratchFGDB.py";
                string where_clause = @"""State = 'UT'""";
                commands.ExecuteArcpyScript.run_arcpy(pythonFileExportData, sgidAddressPnts, "AddressPoints", where_clause);
                Console.WriteLine("Done importing address points");

                // Call etl code.
                commands.LoadAddressPnts.Execute(sgidConnectionProperties, fgdbPath, streamWriter, truncateAddressPoints);
            }

            // ETL incorporated muni to NG911
            if (etlInc)
            {
                commands.LoadIncMuni.Execute(sgidConnectionProperties, fgdbPath, streamWriter, truncateInc);
            }

            // ETL Unicorporated Comm to NG911
            if (etlUnInc)
            {
                commands.LoadUnincComm.Execute(sgidConnectionProperties, fgdbPath, streamWriter, truncateUnInc);
            }

            // ETL counties to NG911
            if (etlCounties)
            {
                commands.LoadCounties.Execute(sgidConnectionProperties, fgdbPath, streamWriter, truncateCounties);
            }

            // ETL Mile Markers to NG911
            if (etlMileMarkers)
            {
                commands.LoadMileMarkerLocations.Execute(sgidConnectionProperties, fgdbPath, streamWriter, truncateMileMarkers);
            }
            
            // ETL Railroads to NG911
            if (etlRailRoads)
            {
                commands.LoadRailroads.Execute(sgidConnectionProperties, fgdbPath, streamWriter, truncateRailRoads);
            }

            // ETL law enforcement to NG911
            if (etlLawEnforcement)
            {
                commands.LoadLawEnforcement.Execute(sgidConnectionProperties, fgdbPath, streamWriter, truncateLawEnforcement);
            }

            // ETL emergency medical services to NG911
            if (etlEMS)
            {
                commands.LoadEmergencyMedicalServices.Execute(sgidConnectionProperties, fgdbPath, streamWriter, truncateEMS);
            }

            // ETL fire to NG911
            if (etlFire)
            {
                commands.LoadFire.Execute(sgidConnectionProperties, fgdbPath, streamWriter, truncateFire);
            }

            // ETL hydroline to NG911
            if (etlHydroLines)
            {
                commands.LoadHydroPolyline.Execute(sgidConnectionProperties, fgdbPath, streamWriter, truncateHydroLines);
            }

            // ETL hydropoly to NG911
            if (etlHydroPoly)
            {
                commands.LoadHydroPolygon.Execute(sgidConnectionProperties, fgdbPath, streamWriter, truncateHydroPoly);
            }



            // Test Dictionary for county number lookup.
            //Dictionary<string, string> myDict = commands.PopuateCountyValuesDict.Execute(sgidConnectionProperties, fgdbPath, streamWriter);
            //Console.WriteLine(myDict["03"]);
            //Console.WriteLine(myDict["25"]);

            //// Populate the CountyNumber/CountyName Dictionary lookup... for use later thoughout the project.
            //commands.GetCountyNameFromNumber.Execute(sgidConnectionProperties, fgdbPath, streamWriter);
            //string coutnyName = commands.GetCountyNameFromNumber.GetCountyName("25");
            //string coutnyName2 = commands.GetCountyNameFromNumber.GetCountyName("03");
            //Console.WriteLine(coutnyName);
            //Console.WriteLine(coutnyName2);


            // Get SGID feature classes //
            //FeatureClass psap = sgid.OpenDataset<FeatureClass>("SGID10.SOCIETY.PSAPBoundaries")
            //FeatureClass roads = sgid.OpenDataset<FeatureClass>("SGID10.TRANSPORTATION.Roads");
            //FeatureClass addressPnts = sgid.OpenDataset<FeatureClass>("SGID10.LOCATION.AddressPoints");
            //FeatureClass muni = sgid.OpenDataset<FeatureClass>("SGID10.BOUNDARIES.Municipalities");

            // Keep the console window open.
            Console.WriteLine("Done!  Press any key to continue...");
            Console.Read();
        
            // Close the stream writer.
            streamWriter.Close();
            // Copy all the contents of streamwriter.
            string oldText = File.ReadAllText(path);

            // Create a new streamwriter, to replace the older one -- this allows me insert lines at the top of the streamwriter, keeping all the log info together.
            using (var sw = new StreamWriter(path, false))
            {
                sw.WriteLine(startTime);
                sw.WriteLine("Finshed at: " + DateTime.Now.ToString());
                sw.WriteLine("Location of output NG911 FileGeodatabase: " + args[0]);
                sw.WriteLine(oldText);
                sw.Close();
            }
        }
    }
}

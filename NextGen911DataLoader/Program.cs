using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Core;
using ArcGIS.Core.Data;
using System.IO;

namespace NextGen911DataLoader
{

    class Program
    {
        [STAThread]

        static void Main(string[] args)
        {
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

            // Check that minimum command line args are present.
            if (!(args.Length > 4))
            {
                Console.WriteLine("You must provide the following command line arguments: [location of output fgdb database], [sde instance], [sde database name], [sde user/pass], [list of valid layer names to elt (in any order)(append -t to layer name if you want to truncate the layer before the load. ex: law-t): roads, addresspoints, psaps, inc, uninc, counties, milemarkers, railroads, law]");
                Console.Read();
                return;
            }

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
                        if (s.ToUpper() == s.ToUpper() + "INC-T")
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
                    default:
                        break;
                }
            }

            // Write out the field headings.
            streamWriter.WriteLine();
            streamWriter.WriteLine("ADJUSTED DATA DURING ETL >>> REPORT/WARNING...");
            streamWriter.WriteLine("_______________________________________");
            streamWriter.WriteLine("FeatureType" + "," + "SGID_OID" + "," + "NextGen_OID" + "," + "Notes");


            // Host.Initialize before constructing any objects from ArcGIS.Core
            try
            {
                ArcGIS.Core.Hosting.Host.Initialize();
            }
            catch (Exception e)
            {
                // Error (missing installation, no license, 64 bit mismatch, etc.)
                Console.WriteLine(string.Format("Initialization failed: {0}", e.Message));
                return;
            }

            string fgdbPath = args[0];

            //// Connect to File Geodatabase
            //Geodatabase NG911Utah = new Geodatabase(new FileGeodatabaseConnectionPath(new Uri(args[0])));

            // Connect to SDID Database //
            DatabaseConnectionProperties sgidConnectionProperties = commands.ConnectToSGID.Execute(args[1], args[2], args[3]);

            // Populate the CountyNumber/CountyName Dictionary lookup... for use later thoughout the project.
            commands.GetCountyNameFromNumber.Execute(sgidConnectionProperties, fgdbPath, streamWriter);

            // ETL Psap Data to NG911
            if (etlPsaps)
            {
                commands.LoadPsapData.Execute(sgidConnectionProperties, fgdbPath, streamWriter, truncatePsaps);
            }

            // ETL Roads Data to NG911
            if (etlRoads)
            {
                commands.LoadRoads.Execute(sgidConnectionProperties, fgdbPath, streamWriter, truncateRoads);
            }

            // ETL address point to NG911
            if (etlAddresspoints)
            {
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

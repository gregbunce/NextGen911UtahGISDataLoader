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
            bool eltRoads = false;
            bool etlAddresspoints = false;
            bool etlPsaps = false;
            bool etlMuni = false;

            // Check that minimum command line args are present.
            if (args.Length < 4)
            {
                Console.WriteLine("You must provide the following command line arguments: [location of output fgdb database], [sde instance], [sde database name], [sde user/pass], [list of valid layer names to elt (in any order): roads, addresspoints, psaps, muni]");
                return;
            }

            // Setup a file stream and a stream writer to write out the log and any odd unexpected stuff that happens.
            string path = @"C:\temp\NextGen911DataLoaderLog" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + ".txt";
            FileStream fileStream = new FileStream(path, FileMode.Create);
            StreamWriter streamWriter = new StreamWriter(fileStream);
            string startTime = ("Started at: " + DateTime.Now.ToString());
            streamWriter.WriteLine("List of Feature Classes that were ETL'd:");

            // Check what layers the user wants to etl.
            foreach (string s in args)
            {
                System.Console.WriteLine(s);

                switch (s.ToUpper()) // make the argument upper to allow the user to use any casing.
                {
                    case "ROADS":
                        streamWriter.WriteLine(" *" + s);
                        eltRoads = true;
                        break;
                    case "ADDRESSPOINTS":
                        streamWriter.WriteLine(" *" + s);
                        etlAddresspoints = true;
                        break;
                    case "PSAPS":
                        streamWriter.WriteLine(" *" + s);
                        etlPsaps = true;
                        break;
                    case "MUNI":
                        streamWriter.WriteLine(" *" + s);
                        etlMuni = true;
                        break;
                    default:
                        break;
                }
            }

            // Write out the field headings.
            streamWriter.WriteLine();
            streamWriter.WriteLine("ERROR REPORT...");
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

            // ETL Psap Data to NG911
            if (etlPsaps)
            {
                commands.LoadPsapData.Execute(sgidConnectionProperties, fgdbPath);
            }

            // ETL Roads Data to NG911
            if (eltRoads)
            {
                commands.LoadRoads.Execute(sgidConnectionProperties, fgdbPath, streamWriter);
            }

            // ETL address point to NG911
            if (etlAddresspoints)
            {
                commands.LoadAddressPnts.Execute(sgidConnectionProperties, fgdbPath, streamWriter);
            }


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

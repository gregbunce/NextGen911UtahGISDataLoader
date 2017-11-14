using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Core;
using ArcGIS.Core.Data;

namespace NextGen911DataLoader
{
    class Program
    {
        [STAThread]

        static void Main(string[] args)
        {
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
            commands.LoadPsapData.Execute(sgidConnectionProperties, fgdbPath);

            // ETL address point to NG911



            
            //using (Geodatabase sgid = new Geodatabase(sgidConnectionProperties))
            //{
            //    using (FeatureClass sgidPsap = sgid.OpenDataset<FeatureClass>("SGID10.SOCIETY.PSAPBoundaries"))
            //    {
            //        QueryFilter queryFilter = new QueryFilter
            //        {
            //            //WhereClause = "DISTRCTNAME = 'Indian Prairie School District 204'"
            //        };

            //        using (RowCursor SgidPsapCursor = sgidPsap.Search(null, true))
            //        {
            //            Row row = null;
            //            while (SgidPsapCursor.MoveNext())
            //            {
            //                row = SgidPsapCursor.Current;
                            
            //                Console.WriteLine(SgidPsapCursor.Current.GetOriginalValue(SgidPsapCursor.Current.FindField("PSAP_NAME")));
            //            }

            //        }



            //    }
            //    using (FeatureClass SgidMuni = sgid.OpenDataset<FeatureClass>("SGID10.BOUNDARIES.Municipalities"))
            //    {


            //    }
            //}

            // Get SGID feature classes //
            //FeatureClass psap = sgid.OpenDataset<FeatureClass>("SGID10.SOCIETY.PSAPBoundaries")
            //FeatureClass roads = sgid.OpenDataset<FeatureClass>("SGID10.TRANSPORTATION.Roads");
            //FeatureClass addressPnts = sgid.OpenDataset<FeatureClass>("SGID10.LOCATION.AddressPoints");
            //FeatureClass muni = sgid.OpenDataset<FeatureClass>("SGID10.BOUNDARIES.Municipalities");
















            // Keep the console window open
            Console.Read();
        }
    }
}

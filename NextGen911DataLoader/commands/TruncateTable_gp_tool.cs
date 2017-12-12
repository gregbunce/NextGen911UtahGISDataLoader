using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Framework;
using ArcGIS.Core.Data;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Core.Geoprocessing;

namespace NextGen911DataLoader.commands
{
    class TruncateTable_gp_tool
    {
        public async static void Execute(Dataset dataSet, string fgdbLocation,  StreamWriter streamWriter)
        {
            try
            {
                var gp_params = Geoprocessing.MakeValueArray(@"C:\temp\NG911GIS_Schema.gdb\LawEnforcement");
                var gp_result = await Geoprocessing.ExecuteToolAsync("managment.TruncateTable", gp_params);



                // example of using ArcGIS.Core.Geometry.SpatialReference object
                //var gp_result = await QueuedTask.Run(() =>
                //{
                //    //return Geoprocessing.ExecuteToolAsync("managment.TruncateTable", Geoprocessing.MakeValueArray((@"C:\temp\NG911GIS_Schema.gdb\LawEnforcement")));
                //    return Geoprocessing.ExecuteToolAsync("managment.TruncateTable", Geoprocessing.MakeValueArray(dataSet));
                //});


                //return gp_result;
                // call MakeValueArray on spatial_ref so that ExecuteToolAsync can internally use the object
                //var sr_param = Geoprocessing.MakeValueArray(spatial_ref);


            }
            catch (Exception ex)
            {
                Console.WriteLine("There was an error with TruncateTable method. " +
                ex.Message + " " + ex.Source + " " + ex.InnerException + " " + ex.HResult + " " + ex.StackTrace + " " + ex);

                streamWriter.WriteLine();
                streamWriter.WriteLine("ERROR MESSAGE...");
                streamWriter.WriteLine("_______________________________________");
                streamWriter.WriteLine("There was an error with TruncateTable method." +
                ex.Message + " " + ex.Source + " " + ex.InnerException + " " + ex.HResult + " " + ex.StackTrace + " " + ex);

                //return null;

            }
        }
    }
}

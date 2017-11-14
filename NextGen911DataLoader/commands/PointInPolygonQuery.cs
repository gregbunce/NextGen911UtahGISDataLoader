using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
//using ArcGIS.Core.Internal.CIM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGen911DataLoader.commands
{
    class PointInPolygonQuery
    {
        // pass in the point, the feature class (the polygon), and a list of fields to get values from ---- return a list of string to hold the attribute values
        public static List<string> Execute(Point point, FeatureClass polygonFeatureClass, IReadOnlyList<Field> fields)
        {
            try
            {

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was an error with PointInPolygonQuery method." +
                ex.Message + " " + ex.Source + " " + ex.InnerException + " " + ex.HResult + " " + ex.StackTrace + " " + ex);

                return null;
            }
        }

    }
}

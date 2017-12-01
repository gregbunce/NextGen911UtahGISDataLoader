using ArcGIS.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGen911DataLoader.commands
{
    class GetRightLeftOffsetPointsFromPolyline
    {
        public static List<MapPoint> Execute(Polyline polyline)
        {
            try
            {
                MapPoint pt_left = GeometryEngine.Instance.MovePointAlongLine(polyline, 0.5, true, -55, SegmentExtension.NoExtension);
                MapPoint pt_right = GeometryEngine.Instance.MovePointAlongLine(polyline, 0.5, true, 55, SegmentExtension.NoExtension);

                List<MapPoint> mapPoints = new List<MapPoint>();
                mapPoints.Add(pt_left);
                mapPoints.Add(pt_right);

                return mapPoints;
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was an error with GetRightLeftOffsetPointsFromPolyline method." +
                ex.Message + " " + ex.Source + " " + ex.InnerException + " " + ex.HResult + " " + ex.StackTrace + " " + ex);

                return null;
            }
        }
    }
}
 
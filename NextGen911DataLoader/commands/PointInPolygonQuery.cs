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
        public static List<string> Execute(MapPoint mapPoint, FeatureClass polygonFeatureClass, IReadOnlyList<Field> fields, List<string> listOfFields)
        {
            try
            {
                List<string> returnAttrList = new List<string>;

                // Set up Spatial Filter for intersect.
                SpatialQueryFilter spatialQueryFilter = new SpatialQueryFilter
                {
                    //WhereClause = "OWNER_NAME = 'ADA IAN'",
                    //FilterGeometry = new EnvelopeBuilder(minPoint, maxPoint).ToGeometry(),
                    SpatialRelationship = SpatialRelationship.Intersects,
                };

                // Search the polygon Feature Class to see if the point intersects a polygon.... without recycling ("false").
                using (RowCursor rowCursor = polygonFeatureClass.Search(spatialQueryFilter, false))
                {
                    while (rowCursor.MoveNext())
                    {
                        using (Feature feature = (Feature)rowCursor.Current)
                        {
                            //int nameFieldIndex = feature.FindField("NAME");
                            //string districtName = Convert.ToString(feature["DISTRCTNAME"]);
                            //double area = Convert.ToDouble(feature["SCHOOLAREA"]);
                            //string name = Convert.ToString(feature[nameFieldIndex]);
                            //Geometry geometry = feature.GetShape();

                            // Add attributes to the return list.
                            for (int i = 0; i < listOfFields.Count; i++)
                            {
                                returnAttrList.Add(feature.FindField(listOfFields[i]).ToString());
                            }
                        }
                    }
                }

                return returnAttrList;
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

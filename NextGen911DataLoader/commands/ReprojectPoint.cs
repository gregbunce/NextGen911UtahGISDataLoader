using ArcGIS.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGen911DataLoader.commands
{
    class ReprojectPoint
    {
        public static MapPoint Execute(MapPoint mapPointIn, Int32 wKID)
        {
            try
            {
                // Get the input spatial reference.
                SpatialReference spatialReferenceInput = mapPointIn.SpatialReference;

                // Set the output spatial reference.
                SpatialReference spatialReferenceOutput = SpatialReferenceBuilder.CreateSpatialReference(wKID);

                // Set up the datum transformation to be used in the projection.
                ProjectionTransformation transformation = ProjectionTransformation.Create(spatialReferenceInput, spatialReferenceOutput);

                // Perform the projection of the initial map point.
                var projectedPoint = GeometryEngine.Instance.ProjectEx(mapPointIn, transformation);

                // Return the reprojected point.
                return projectedPoint as MapPoint;

            }
            catch (Exception ex)
            {
                Console.WriteLine("There was an error with ReprojectPoint method." +
                ex.Message + " " + ex.Source + " " + ex.InnerException + " " + ex.HResult + " " + ex.StackTrace + " " + ex);

                return null;
            }


        }
    }
};

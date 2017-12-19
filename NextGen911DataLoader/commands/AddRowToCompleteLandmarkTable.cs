using ArcGIS.Core.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGen911DataLoader.commands
{
    class AddRowToCompleteLandmarkTable
    {
        public static void Execute(FeatureClassDefinition featureClassDefinitionSGID, Table ng911_CompleteLandmarkTable, RowCursor SgidCursor, StreamWriter streamWriter, Int32 aliasNameRowCount, string aliasName)
        {
            try
            {
                // Create row buffer.
                using (RowBuffer rowBuffer = ng911_CompleteLandmarkTable.CreateRowBuffer())
                {
                    // Populate fields.
                    rowBuffer["Source"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("AddSource"));
                    rowBuffer["DateUpdate"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("LoadDate"));
                    //rowBuffer["Effective"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));
                    //rowBuffer["Expire"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(""));
                    rowBuffer["ACLMNNGUID"] = "CLMN" +aliasNameRowCount + "@" + SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("UTAddPtID")).ToString().Trim() + "@gis.utah.gov";
                    rowBuffer["Site_NGUID"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("UTAddPtID")).ToString().Trim() + "@gis.utah.gov";
                    rowBuffer["ACLandmark"] = aliasName;

                    // create the row with the attributes, via rowBuffer, in the ng911 database
                    using (Row row = ng911_CompleteLandmarkTable.CreateRow(rowBuffer))
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was an error with AddRowToCompleteLandmarkTable method. " +
                ex.Message + " " + ex.Source + " " + ex.InnerException + " " + ex.HResult + " " + ex.StackTrace + " " + ex);

                streamWriter.WriteLine();
                streamWriter.WriteLine("ERROR MESSAGE...");
                streamWriter.WriteLine("_______________________________________");
                streamWriter.WriteLine("There was an error with AddRowToCompleteLandmarkTable method." +
                ex.Message + " " + ex.Source + " " + ex.InnerException + " " + ex.HResult + " " + ex.StackTrace + " " + ex);
            }
        }
    }
}

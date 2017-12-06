using ArcGIS.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGen911DataLoader.commands
{
    class AddRowToStreetNameAliasTable
    {
        public static void Execute(FeatureClassDefinition featureClassDefinitionSGID, Table ng911StreetNameAliasTable, RowCursor SgidCursor, string aliasType)
        {
            try
            {
                // Create row buffer.
                using (RowBuffer rowBuffer = ng911StreetNameAliasTable.CreateRowBuffer())
                {
                    // Populate the common fields.
                    rowBuffer["ASt_Name"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(aliasType + "_NAME"));
                    rowBuffer["Source"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("SOURCE"));
                    rowBuffer["DateUpdate"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("UPDATED"));
                    rowBuffer["Effective"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("EFFECTIVE"));
                    rowBuffer["Expire"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("EXPIRE"));
                    rowBuffer["RCL_NGUID"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("UNIQUE_ID"));
                    rowBuffer["ASt_NGUID"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("UNIQUE_ID")).ToString() + "_" + aliasType;

                    // Populate fields that need domain description values from SGID //
                    // ASt_PosDir //
                    string codedDomainValue = GetDomainValue.Execute(featureClassDefinitionSGID, SgidCursor, aliasType + "_POSTDIR", aliasType + "_POSTDIR");
                    codedDomainValue.Trim();
                    if (codedDomainValue != "")
                    {
                        // Proper case.
                        //codedDomainValue = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(codedDomainValue.ToLower());
                        // Upper case.
                        rowBuffer["ASt_PosDir"] = codedDomainValue.ToUpper();
                    }


                    // Populate the fields that are specific to alpha or numeric alias street types.
                    if (aliasType == "AN")
                    {
                        // Populate the PreDir from the primary street.
                        //rowBuffer["ASt_PreDir"] = SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField("PREDIR"));
                        codedDomainValue = GetDomainValue.Execute(featureClassDefinitionSGID, SgidCursor, "PREDIR", "PREDIR");
                        codedDomainValue.Trim();
                        if (codedDomainValue != "")
                        {
                            // Proper case.
                            //codedDomainValue = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(codedDomainValue.ToLower());
                            // Upper case.
                            rowBuffer["ASt_PreDir"] = codedDomainValue.ToUpper();
                        }
                    }
                    else if (aliasType == "A1" | aliasType == "A2")
                    {
                        // Populate the PostType field, which is specific to only alpha-named roads.
                        codedDomainValue = GetDomainValue.Execute(featureClassDefinitionSGID, SgidCursor, aliasType + "_POSTTYPE", aliasType + "_POSTTYPE");
                        codedDomainValue.Trim();
                        if (codedDomainValue != "")
                        {
                            // Proper case.
                            //codedDomainValue = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(codedDomainValue.ToLower());
                            // Upper case.
                            rowBuffer["AStPosType"] = codedDomainValue.ToUpper();
                        }

                        // Check if A*_PREDIR is populated, if not then use PREDIR.
                        if (SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(aliasType + "_PREDIR")).ToString() == "" | SgidCursor.Current.GetOriginalValue(SgidCursor.Current.FindField(aliasType + "_PREDIR")) is DBNull)
                        {
                            codedDomainValue = GetDomainValue.Execute(featureClassDefinitionSGID, SgidCursor, "PREDIR", "PREDIR");
                            codedDomainValue.Trim();
                            if (codedDomainValue != "")
                            {
                                // Proper case.
                                //codedDomainValue = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(codedDomainValue.ToLower());
                                // Upper case.
                                rowBuffer["ASt_PreDir"] = codedDomainValue.ToUpper();
                            }
                        }
                        else
                        {
                            codedDomainValue = GetDomainValue.Execute(featureClassDefinitionSGID, SgidCursor, aliasType + "_POSTDIR", aliasType + "_POSTDIR");
                            codedDomainValue.Trim();
                            if (codedDomainValue != "")
                            {
                                // Proper case.
                                //codedDomainValue = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(codedDomainValue.ToLower());
                                // Upper case.
                                rowBuffer["ASt_PreDir"] = codedDomainValue.ToUpper();
                            }
                        }
                    }

                    // create the row with the attributes, via rowBuffer, in the ng911 database
                    using (Row row = ng911StreetNameAliasTable.CreateRow(rowBuffer))
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was an error with AddRowToStreetNameAliasTable method. " +
                ex.Message + " " + ex.Source + " " + ex.InnerException + " " + ex.HResult + " " + ex.StackTrace + " " + ex);
            }
        }
    }
}

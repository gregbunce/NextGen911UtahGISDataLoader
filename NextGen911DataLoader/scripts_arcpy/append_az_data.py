import arcpy, sys, os

#: use python3
#: make sure you're connected to the State VPN
#: this script is a copy paste from the append geoprocessing tool and should be ready to go as is.

#: Append AZ Roads.
arcpy.Append_management(inputs="//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs", target="C:/Temp/NG911GIS_Schema.gdb/RoadCenterlines", schema_type="NO_TEST", field_mapping='Source "Source of Data" true true false 75 Text 0 0 ,First,#;DateUpdated "Date Updated" true true false 8 Date 0 0 ,First,#;Effective "Effective Date" true true false 8 Date 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,Effective,-1,-1;Expire "Expiration Date" true true false 8 Date 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,Expire,-1,-1;RCL_NGUID "Road Centerline NENA Globally Unique ID" true true false 254 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,RCL_NGUID,-1,-1;AdNumPre_L "Left Address Number Prefix" true true false 15 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,AdNumPre_L,-1,-1;AdNumPre_R "Right Address Number Prefix" true true false 15 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,AdNumPre_R,-1,-1;FromAddr_L "Left FROM Address" true true false 4 Long 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,FromAddr_L,-1,-1;ToAddr_L "Left TO Address" true true false 4 Long 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,ToAddr_L,-1,-1;FromAddr_R "Right FROM Address" true true false 4 Long 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,FromAddr_R,-1,-1;ToAddr_R "Right TO Address" true true false 4 Long 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,ToAddr_R,-1,-1;Parity_L "Parity Left" true true false 1 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,Parity_L,-1,-1;Parity_R "Parity Right" true true false 1 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,Parity_R,-1,-1;St_PreMod "Street Name Pre Modifier" true true false 9 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,St_PreMod,-1,-1;St_PreDir "Street Name Pre Directional" true true false 50 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,St_PreDir,-1,-1;St_PreTyp "Street Name Pre Type" true true false 25 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,St_PreTyp,-1,-1;St_PreSep "Street Name Pre Type Separator" true true false 20 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,St_PreSep,-1,-1;StreetName "Street Name" true true false 60 Text 0 0 ,First,#;St_PosTyp "Street Name Post Type" true true false 25 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,St_PosTyp,-1,-1;St_PosDir "Street Name Post Directional" true true false 9 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,St_PosDir,-1,-1;St_PosMod "Street Name Post Modifier" true true false 25 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,St_PosMod,-1,-1;LSt_PreDir "Legacy Street Name Pre Directional" true true false 2 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,LSt_PreDir,-1,-1;LSt_Name "Legacy Street Name" true true false 75 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,LSt_Name,-1,-1;LSt_Type "Legacy Street Name Type" true true false 5 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,LSt_Type,-1,-1;LStPosDir "Legacy Street Name Post Directional" true true false 2 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,LStPosDir,-1,-1;ESN_L "ESN Left" true true false 4 Long 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,ESN_L,-1,-1;ESN_R "ESN Right" true true false 4 Long 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,ESN_R,-1,-1;MSAGComm_L "MSAG Community Name Left" true true false 30 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,MSAGComm_L,-1,-1;MSAGComm_R "MSAG Community Name Right" true true false 30 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,MSAGComm_R,-1,-1;Country_L "Country Left" true true false 2 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,Country_L,-1,-1;Country_R "Country Right" true true false 2 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,Country_R,-1,-1;State_L "State Left" true true false 2 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,State_L,-1,-1;State_R "State Right" true true false 2 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,State_R,-1,-1;County_L "County Left" true true false 40 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,County_L,-1,-1;County_R "County Right" true true false 40 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,County_R,-1,-1;AddCode_L "Additional Code Left" true true false 6 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,AddCode_L,-1,-1;AddCode_R "Additional Code Right" true true false 6 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,AddCode_R,-1,-1;IncMuni_L "Incorporated Municipality Left" true true false 100 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,IncMuni_L,-1,-1;IncMuni_R "Incorporated Municipality Right" true true false 100 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,IncMuni_R,-1,-1;UnincCom_L "Unincorporated Community Left" true true false 100 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,UnincCom_L,-1,-1;UnincCom_R "Unincorporated Community Right" true true false 100 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,UnincCom_R,-1,-1;NbrhdCom_L "Neighborhood Community Left" true true false 100 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,NbrhdCom_L,-1,-1;NbrhdCom_R "Neighborhood Community Right" true true false 100 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,NbrhdCom_R,-1,-1;PostCode_L "Postal Code Left" true true false 7 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,PostCode_L,-1,-1;PostCode_R "Postal Code Right" true true false 7 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,PostCode_R,-1,-1;PostComm_L "Postal Community Name Left" true true false 40 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,PostComm_L,-1,-1;PostComm_R "Postal Community Name Right" true true false 40 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,PostComm_R,-1,-1;RoadClass "Road Class" true true false 15 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,RoadClass,-1,-1;OneWay "One-Way" true true false 2 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,OneWay,-1,-1;SpeedLimit "Speed Limit" true true false 2 Short 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,SpeedLimit,-1,-1;DiscrpAgID "Discrepancy Agency ID" true true false 75 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,DiscrpAgID,-1,-1;dmNotesXML "dmNotesXML" true true false 3999 Text 0 0 ,First,#;dmValidation "dmValidation" true true false 3999 Text 0 0 ,First,#;dmClientUploadedFileID "dmClientUploadedFileID" true true false 4 Long 0 0 ,First,#;Shape_Length "Shape_Length" false true true 8 Double 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_RCL_in_Utah_PSAPs,Shape_Length,-1,-1', subtype="")

#: Append AZ Address Points.
arcpy.Append_management(inputs="//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs", target="C:/Temp/NG911GIS_Schema.gdb/AddressPoints", schema_type="NO_TEST", field_mapping='Source "Source of Data" true true false 75 Text 0 0 ,First,#;DateUpdate "Date Update" true true false 8 Date 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,DateUpdate,-1,-1;Effective "Effective Date" true true false 8 Date 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,Effective,-1,-1;Expire "Expiration Date" true true false 8 Date 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,Expire,-1,-1;Site_NGUID "Site NENA Globally Unique ID" true true false 100 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,Site_NGUID,-1,-1;Country "Country" true true false 2 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,Country,-1,-1;State "State" true true false 2 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,State,-1,-1;County "County" true true false 40 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,County,-1,-1;AddCode "Additional Code" true true false 6 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,AddCode,-1,-1;AddDataURI "Additional Data URI" true true false 254 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,AddDataURI,-1,-1;Inc_Muni "Incorporated Municipality" true true false 100 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,Inc_Muni,-1,-1;Uninc_Comm "Unincorporated Community" true true false 100 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,Uninc_Comm,-1,-1;Nbrhd_Comm "Neighborhood Community" true true false 100 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,Nbrhd_Comm,-1,-1;AddNum_Pre "Address Number Prefix" true true false 15 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,AddNum_Pre,-1,-1;Add_Number "Address Number" true true false 2 Short 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,Add_Number,-1,-1;AddNum_Suf "Address Number Suffix" true true false 15 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,AddNum_Suf,-1,-1;St_PreMod "Street Name Pre Modifier" true true false 15 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,St_PreMod,-1,-1;St_PreDir "Street Name Pre Directional" true true false 9 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,St_PreDir,-1,-1;St_PreTyp "Street Name Pre Type" true true false 25 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,St_PreTyp,-1,-1;St_PreSep "Street Name Pre Type Separator" true true false 20 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,St_PreSep,-1,-1;StreetName "Street Name" true true false 60 Text 0 0 ,First,#;St_PosTyp "Street Name Post Type" true true false 25 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,St_PosTyp,-1,-1;St_PosDir "Street Name Post Directional" true true false 9 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,St_PosDir,-1,-1;St_PosMod "Street Name Post Modifier" true true false 25 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,St_PosMod,-1,-1;LSt_PreDir "Legacy Street Name Pre Directional" true true false 2 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,LSt_PreDir,-1,-1;LSt_Name "Legacy Street Name" true true false 75 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,LSt_Name,-1,-1;LSt_Type "Legacy Street Name Type" true true false 5 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,LSt_Type,-1,-1;LStPosDir "Legacy Street Name Post Directional" true true false 2 Text 0 0 ,First,#;ESN "ESN" true true false 0 Long 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,ESN,-1,-1;MSAGComm "MSAG Community Name" true true false 30 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,MSAGComm,-1,-1;Post_Comm "Postal Community Name" true true false 40 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,Post_Comm,-1,-1;Post_Code "Postal Code" true true false 7 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,Post_Code,-1,-1;Post_Code4 "ZIP Plus 4" true true false 4 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,Post_Code4,-1,-1;Building "Building" true true false 75 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,Building,-1,-1;Floor "Floor" true true false 75 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,Floor,-1,-1;Unit "Unit" true true false 75 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,Unit,-1,-1;Room "Room" true true false 75 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,Room,-1,-1;Seat "Seat" true true false 75 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,Seat,-1,-1;Addtl_Loc "Additional Location Information" true true false 225 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,Addtl_Loc,-1,-1;LandmkName "Complete Landmark Name" true true false 150 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,LandmkName,-1,-1;Mile_Post "Mile Post" true true false 150 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,Mile_Post,-1,-1;Place_Type "Place Type" true true false 50 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,Place_Type,-1,-1;Placement "Placement Method" true true false 25 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,Placement,-1,-1;Long "Longitude" true true false 4 Float 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,Long,-1,-1;Lat "Latitude" true true false 4 Float 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,Lat,-1,-1;Elev "Elevation" true true false 2 Short 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,Elev,-1,-1;DiscrpAgID "Discrepancy Agency ID" true true false 75 Text 0 0 ,First,#,//itwfpcap2/AGRC/agrc/data/ng911/UT_AZ_Border_PSAPs.gdb/AZNG911_SSAP_in_Utah_PSAPs,DiscrpAgID,-1,-1;GlobalID "GlobalID" false false true 38 GlobalID 0 0 ,First,#', subtype="")
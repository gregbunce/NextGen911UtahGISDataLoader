import arcpy, os

#: Parity Domain values:
#: O = ODD
#: E = EVEN
#: B = BOTH
#: Z = address range 0,0 or null,null

#: Set path to ng911 database.
#ng911_db = r"\\itwfpcap2\AGRC\agrc\data\ng911\SpatialStation_live_data\UtahNG911GIS.gdb"
ng911_db = r"C:\Temp\NG911GIS_Schema_paritytesting.gdb"
rcls = os.path.join(ng911_db, r'RoadCenterlines')

arcpy.env.workspace = ng911_db
arcpy.env.overwriteOutput = True
arcpy.env.qualifiedFieldNames = False

fields = ['Parity_L', 'Parity_R','FromAddr_L', 'ToAddr_L', 'FromAddr_R', 'ToAddr_R']
parL_index = fields.index('Parity_L')
parR_index = fields.index('Parity_R')
fromL_index = fields.index('FromAddr_L')
toL_index = fields.index('ToAddr_L')
fromR_index = fields.index('FromAddr_R')
toR_index = fields.index('ToAddr_R')

print("Looping through rows in FC ...")
with arcpy.da.UpdateCursor(rcls, fields) as update_cursor:
    for row in update_cursor:
        parity_l_new_value = ""
        parity_r_new_value = ""
        fromL = row[fromL_index]
        toL = row[toL_index]
        fromR = row[fromR_index]
        toR = row[toR_index]
        parL = row[parL_index]
        parR = row[parR_index]

        #: Check for parity inconsistency
        fromL_odd = True if fromL % 2 else False
        toL_odd = True if toL % 2 else False
        fromR_odd = True if fromR % 2 else False
        toR_odd = True if toR % 2 else False

        print("itterating...")

        #: assign parity_l when missing value
        if parL not in ('B', 'E', 'O', 'Z'):
            if (not fromL_odd and toL_odd) or (fromL_odd and not toL_index):
                parity_l_new_value = "B"
            elif fromL_odd and toL_odd:
                    parity_l_new_value = "O"
            else:
                parity_l_new_value = "E"

        #: assign parity_r when missing value
        if fromR_odd not in ('B', 'E', 'O', 'Z'):
            if (not fromR_odd and toR_odd) or (fromR_odd and not toR_index):
                parity_r_new_value = "B"
            elif fromR_odd and toR_odd:
                    parity_r_new_value = "O"
            else:
                parity_r_new_value = "E"

            row[parL_index] = parity_l_new_value
            row[parR_index] = parity_r_new_value
            update_cursor.updateRow(row)

print("done!")

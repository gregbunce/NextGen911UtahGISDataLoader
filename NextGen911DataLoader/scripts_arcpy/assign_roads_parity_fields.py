import arcpy, os

#: Parity Domain values:
#: O = ODD
#: E = EVEN
#: B = BOTH
#: Z = address range 0,0 or null,null

#: Set path to ng911 database.
#ng911_db = r"\\itwfpcap2\AGRC\agrc\data\ng911\SpatialStation_live_data\UtahNG911GIS.gdb"
ng911_db = r"C:\Temp\NG911GIS_Schema.gdb"
rcls = os.path.join(ng911_db, r'RoadCenterlines')

arcpy.env.workspace = ng911_db
arcpy.env.overwriteOutput = True
arcpy.env.qualifiedFieldNames = False

fields = ['OID@', 'Parity_L', 'Parity_R','FromAddr_L', 'ToAddr_L', 'FromAddr_R', 'ToAddr_R']
oid_index = fields.index('OID@')
parL_index = fields.index('Parity_L')
parR_index = fields.index('Parity_R')
fromL_index = fields.index('FromAddr_L')
toL_index = fields.index('ToAddr_L')
fromR_index = fields.index('FromAddr_R')
toR_index = fields.index('ToAddr_R')
itter = 0

print("Looping through rows in FC ...")
with arcpy.da.UpdateCursor(rcls, fields) as update_cursor:
    for row in update_cursor:
        oid = row[oid_index]
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

        #: assign parity_l when missing value
        if parL not in ('B', 'E', 'O', 'Z'):
            #: check for zeros.
            if fromL == 0 and toL == 0:
                parity_l_new_value = 'Z'
            #: check if parity is BOTH
            elif (not fromL_odd and toL_odd) or (fromL_odd and not toL_index):
                parity_l_new_value = "B"
            #: check if parity is ODD
            elif fromL_odd and toL_odd:

                    parity_l_new_value = "O"
            #: check if parity is EVEN
            else:
                parity_l_new_value = "E"
        else:
            parity_l_new_value = parL
            print("OID " + str(oid) + " contained an existing valid parity_l value and was skipped.")

        #: assign parity_r when missing value
        if parR not in ('B', 'E', 'O', 'Z'):
            #: check for zeros.
            if fromR == 0 and toR == 0:
                parity_r_new_value = 'Z'
            #: check if parity is BOTH
            elif (not fromR_odd and toR_odd) or (fromR_odd and not toR_index):
                parity_r_new_value = "B"
            #: check if parity is ODD
            elif fromR_odd and toR_odd:
                    parity_r_new_value = "O"
            #: check if parity is EVEN
            else:
                parity_r_new_value = "E"
        else:
            parity_r_new_value = parR
            print("OID " + str(oid) + " contained an existing valid parity_r value and was skipped.")

        itter = itter + 1
        if str(itter).endswith('0000'):
            print("processing the " + str(itter) + "s table block...")
        row[parL_index] = parity_l_new_value
        row[parR_index] = parity_r_new_value
        update_cursor.updateRow(row)

print("done!")

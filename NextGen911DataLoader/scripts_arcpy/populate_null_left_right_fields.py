from dataclasses import field
import arcpy, os

#: Notes before running
#: 1. use python 3
#: 2. make sure the following variables are pointed to the correct db and fc: ng911_db, rcls (both variables should be fine as is)

#: This helper function checks if a field has a value.
def HasFieldValue(field_value):
    """ example: (row.STATUS) """
    if field_value is None:
        # the value is of NoneType
        return False
    else:
        _str_field_value = str(field_value)

        # value is not of NoneType
        if _str_field_value.isdigit():
            # it's an int
            if _str_field_value == "":
                return False
            else:
                return True
        else:
            # it's not an int
            if _str_field_value == "" or _str_field_value is None or _str_field_value.isspace():
                return False
            else:
                return True


#: Set path to ng911 database.
#ng911_db = r"\\itwfpcap2\AGRC\agrc\data\ng911\SpatialStation_live_data\UtahNG911GIS.gdb"
ng911_db = r"C:\Temp\NG911GIS_Schema.gdb"
rcls = os.path.join(ng911_db, r'RoadCenterlines')

arcpy.env.workspace = ng911_db
arcpy.env.overwriteOutput = True
arcpy.env.qualifiedFieldNames = False

fields = ['OID@', 'MSAGComm_L', 'MSAGComm_R','County_L', 'County_R', 'PostCode_L', 'PostCode_R', 'PostComm_L', 'PostComm_R']
oid_index = fields.index('OID@')
msagL_index = fields.index('MSAGComm_L')
msagR_index = fields.index('MSAGComm_R')
countyL_index = fields.index('County_L')
countyR_index = fields.index('County_R')
postcodeL_index = fields.index('PostCode_L')
postcodeR_index = fields.index('PostCode_R')
postcomL_index = fields.index('PostComm_L')
postcomR_index = fields.index('PostComm_R')
itter = 0

print("Looping through rows in FC ...")
with arcpy.da.UpdateCursor(rcls, fields) as update_cursor:
    for row in update_cursor:
        oid = row[oid_index]
        msagL = row[msagL_index]
        msagR = row[msagR_index]
        countyL = row[countyL_index]
        countyR = row[countyR_index]
        postcodeL = row[postcodeL_index]
        postcodeR = row[postcodeR_index]
        postcomL = row[postcomL_index]
        postcomR = row[postcomR_index]

        #: Check for missing msag values.
        if HasFieldValue(msagL) == False and HasFieldValue(msagR) == True:
            row[msagL_index] = msagR
        if HasFieldValue(msagR) == False and HasFieldValue(msagL) == True:
            row[msagR_index] = msagL

        #: Check for missing county values.
        if HasFieldValue(countyL) == False and HasFieldValue(countyR) == True:
            row[countyL_index] = countyR
        if HasFieldValue(countyR) == False and HasFieldValue(countyL) == True:
            row[countyR_index] = countyL

        #: Check for missing postal code values.
        if HasFieldValue(postcodeL) == False and HasFieldValue(postcodeR) == True:
            row[postcodeL_index] = postcodeR
        if HasFieldValue(postcodeR) == False and HasFieldValue(postcodeL) == True:
            row[postcodeR_index] = postcodeL

        #: Check for missing postal community values.
        if HasFieldValue(postcomL) == False and HasFieldValue(postcomR) == True:
            row[postcomL_index] = postcomR
        if HasFieldValue(postcomR) == False and HasFieldValue(postcomL) == True:
            row[postcomR_index] = postcomL

        itter = itter + 1
        if str(itter).endswith('0000'):
            print("processing the " + str(itter) + "s table block...")

        update_cursor.updateRow(row)

print("done!")

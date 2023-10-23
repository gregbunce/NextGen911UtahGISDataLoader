import arcpy, sys, os

query_filter = "Country is null"
int_counter = 0

with arcpy.da.UpdateCursor(r"C:\Temp\NG911GIS_Schema.gdb\AddressPoints", "*", where_clause=query_filter) as uCur:
    for dRow in uCur:
        int_counter = int_counter + 1
        print("deleted " + str(int_counter))
        uCur.deleteRow ()
        
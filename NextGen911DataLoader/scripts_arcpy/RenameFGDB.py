import arcpy
from arcpy import sys

# Set local variables
in_data =  'C:/temp/ng911scratch.gdb'
out_data = 'C:/temp/ng911scratchReOn' + str(sys.argv[1]) + '.gdb'

# Execute Rename
arcpy.Rename_management(in_data, out_data)
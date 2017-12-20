import arcpy
import sys

# Execute CreateFileGDB - argv1 = out_folder_path; argv2 = out_name
arcpy.CreateFileGDB_management(sys.argv[1], sys.argv[2])
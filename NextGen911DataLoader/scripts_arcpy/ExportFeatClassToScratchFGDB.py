import arcpy
import sys

# arcpy.env.workspace = 'C:/Users/gbunce.UTAH/AppData/Roaming/Esri/ArcGISPro/Favorites/sgid.agrc.utah.gov.sde'

# Set local variables
# in_features = ['climate.shp', 'majorrds.shp']
in_features = sys.argv[1]
out_location = 'C:/temp/ng911scratch.gdb'
out_fc_name = sys.argv[2]

# Execute FeatureClassToGeodatabase
arcpy.FeatureClassToFeatureClass_conversion(in_features, out_location, out_fc_name)
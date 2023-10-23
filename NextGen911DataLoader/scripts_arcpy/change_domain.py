import arcpy
#: Use python3

# Set the workspace environment (e.g., geodatabase or folder)
arcpy.env.workspace = r"C:\Users\gbunce\Documents\projects\SGID\local_sgid_data\SGID_2023_8_3.gdb"

# Specify the feature class you want to modify
feature_class = "AddressPoints"

# List all domains in the workspace
domains = arcpy.da.ListDomains()

# Find the domain you want to change
target_domain_name = "CVDomain_StreetType_1"
target_domain = None

for domain in domains:
    if domain.name == target_domain_name:
        target_domain = domain
        break

if not target_domain:
    print(f"Domain '{target_domain_name}' not found.")
    sys.exit()

# Specify the field you want to update
field_name = "StreetType"

# Remove a domain from field.
arcpy.RemoveDomainFromField_management(feature_class, field_name)

# add new domain to field
arcpy.AssignDomainToField_management(feature_class, field_name, target_domain_name)

print("done!")

# Close it out.
arcpy.env.workspace = None  # Reset the workspace to avoid locking issues

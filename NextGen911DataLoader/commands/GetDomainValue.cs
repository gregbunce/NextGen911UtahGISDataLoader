using ArcGIS.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGen911DataLoader.commands
{
    class GetDomainValue
    {
        public static string Execute(FeatureClassDefinition featureClassDefinition, RowCursor rowCursor, string fieldValue, string fieldName)
        {
            try
            {
                //IReadOnlyList<Field> NG911fields = featureClassDefinition.GetFields();
                string domainValue = string.Empty;

                int fieldInt = featureClassDefinition.FindField(fieldName);
                Field field = featureClassDefinition.GetFields()[fieldInt];

                Domain domain = field.GetDomain();
                if (domain is CodedValueDomain)
                {
                    CodedValueDomain codedValueDomain = domain as CodedValueDomain;
 
                    // get the domain value (description) 
                    domainValue = codedValueDomain.GetName(rowCursor.Current.GetOriginalValue(rowCursor.Current.FindField(fieldValue)));
                }

                return domainValue;
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was an error with GetDomainValue method." +
                ex.Message + " " + ex.Source + " " + ex.InnerException + " " + ex.HResult + " " + ex.StackTrace + " " + ex);

                return null;
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NextGen911DataLoader.commands
{
    class RemoveAlphaCharsFromString
    {

        public static string Execute(string stringIn, StreamWriter streamWriter)
        {
            try
            {
                string stringOut = string.Empty;

                // Check for alpha characters in the AddNum field.
                if (Regex.IsMatch(stringIn, ".*?[a-zA-Z].*?"))
                {
                    // Remove the numberic characters
                    stringOut = Regex.Replace(stringOut, "[^0-9.]", "");
                }
                else
                {
                    stringOut = stringIn;
                }

                return stringOut;
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was an error with RemoveAlphaCharsFromString method. " +
                ex.Message + " " + ex.Source + " " + ex.InnerException + " " + ex.HResult + " " + ex.StackTrace + " " + ex);

                streamWriter.WriteLine();
                streamWriter.WriteLine("ERROR MESSAGE...");
                streamWriter.WriteLine("_______________________________________");
                streamWriter.WriteLine("There was an error with RemoveAlphaCharsFromString method." +
                ex.Message + " " + ex.Source + " " + ex.InnerException + " " + ex.HResult + " " + ex.StackTrace + " " + ex);

                return null;
            }

        }


    }
}

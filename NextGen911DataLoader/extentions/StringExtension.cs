using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace NextGen911DataLoader.extentions
{
    public static class StringExtension
    {

        // extension to see if value is null or empty string
        public static bool IsEmpty(this string theString)
        {
            return (theString == null || theString.Length == 0);
        }


    }
}

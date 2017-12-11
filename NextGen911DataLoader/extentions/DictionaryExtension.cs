using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace NextGen911DataLoader.extentions
{
    public static class DictionaryExtension
    {

        public static string GetCountyNameExt(this Dictionary<string, string> dict, string countyNumber)
        {
            return dict[countyNumber].ToString();
        }


    }
}

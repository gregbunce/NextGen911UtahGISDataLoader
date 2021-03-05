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

        // postal community name
        public static string GetPostalCommExt(this Dictionary<string, string> dict, string zipcode_num)
        {
            //return dict[zipcode_num].ToString();

            if (dict.ContainsKey(zipcode_num))
                return dict[zipcode_num].ToString();
            //return postal_comm;
            else
                return "";

        }



    }
}

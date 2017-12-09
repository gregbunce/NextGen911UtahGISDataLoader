using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGen911DataLoader.models
{
    public class CountyValues<T> : System.Collections.Generic.Dictionary<string, string>
    {
        public string Name { get; set; }
        public string Number { get; set; }
    }
}
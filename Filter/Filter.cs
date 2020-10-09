using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceramic3d_filter.Filter
{
    public class Filter : Named, IFilter
    {
        private readonly HashSet<string> values;
        public Filter(string name, IEnumerable<string> values) : base(name)
        {
            this.values = new HashSet<string>(values);
        }
        public IEnumerable<string> Values { get { return this.values; } }




    }
}

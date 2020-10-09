using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceramic3d_filter.Filter
{
    public interface IFilter
    {
        string Name { get; }
        IEnumerable<string> Values { get; }
    }
}

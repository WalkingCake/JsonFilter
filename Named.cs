using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceramic3d_filter
{
    public abstract class Named : INamed
    {
        public string Name { get; }
        public Named(string name)
        {
            this.Name = name;
        }
    }
}

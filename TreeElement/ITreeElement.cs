using Ceramic3d_filter.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceramic3d_filter.TreeElement
{
    public interface ITreeElement : INamed
    {
        IEnumerable<IFilter> FilterList { get; }
        IEnumerable<ITreeElement> Children { get; }
        string Caption { get; }
        IEnumerable<string> GetFilterValues(IFilter filter);
        ITreeElement AddChild(string name, string caption, IEnumerable<IFilter> filterList, IDictionary<IFilter, IEnumerable<string>> filters);
        void AddChild(ITreeElement child);
        IEnumerable<IFilter> AvailableFilters { get; }
    }
}

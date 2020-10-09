using Ceramic3d_filter.Filter;
using Ceramic3d_filter.Tree;
using Ceramic3d_filter.TreeElement;
using System.Collections.Generic;

namespace Ceramic3d_filter.Deserializer
{
    public interface IDeserializer
    {
        IEnumerable<IFilter> AvailableFilters { get; }
        IEnumerable<IFilter> DefaultFilters { get; }
        ITree Tree { get; }
        IEnumerable<ITreeElement> TreeElements { get; }
        IFilter GetFilter(string name);
        ITreeElement GetTreeElement(string name);
    }
}

using Ceramic3d_filter.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceramic3d_filter.TreeElement
{
    public class TreeElement : Named, ITreeElement
    {
        private readonly HashSet<IFilter> filterList;
        private readonly Dictionary<IFilter, IEnumerable<string>> filters;
        private readonly List<ITreeElement> children;

        public TreeElement(string name, string caption, IEnumerable<IFilter> filterList, IDictionary<IFilter, IEnumerable<string>> filters) : base(name)
        {
            this.filterList = new HashSet<IFilter>(filterList);
            this.Caption = caption;
            this.filters = new Dictionary<IFilter, IEnumerable<string>>(filters);
            this.children = new List<ITreeElement>();
        }
        public string Caption { get; }
        public void AddChild(ITreeElement child)
        {
            this.children.Add(child);
        }
        public ITreeElement AddChild(string name, string caption, IEnumerable<IFilter> filterList, IDictionary<IFilter, IEnumerable<string>> filters)
        {
            ITreeElement el = new TreeElement(name, caption, filterList, filters);
            this.AddChild(el);
            return el;
        }
        public IEnumerable<IFilter> AvailableFilters { get { return this.filters.Keys.Union(this.FilterList); } }

        public IEnumerable<IFilter> FilterList { get { return this.filterList; } }

        public IEnumerable<ITreeElement> Children { get { return this.children; } }

        public IEnumerable<string> GetFilterValues(IFilter filter)
        {
            return this.filters.ContainsKey(filter) ? this.filters[filter] : new List<string>();
        }
        
    }
}

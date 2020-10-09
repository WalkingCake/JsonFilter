using Ceramic3d_filter.Filter;
using Ceramic3d_filter.TreeElement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Ceramic3d_filter.Tree
{
    public class Tree : ITree
    {
        private readonly Stack<ITreeElement> stack;
        private Dictionary<IFilter, string> parameters;

        public Tree(ITreeElement root)
        {
            this.parameters = new Dictionary<IFilter, string>();
            this.Root = root;
            this.Current = root;
            this.stack = new Stack<ITreeElement>();
        }
        
        public IEnumerable<ITreeElement> GetAvailableChildren()
        {
            return this.Root.Children;
        }

        public ITreeElement GoToChild(ITreeElement child)
        {
            if(this.Current.Children.Contains(child))
            {
                ITreeElement c = child;
                do
                {
                    stack.Push(Current);
                    Current = c;
                    if (Current.Children.Count() == 0) return this.Current;
                    c = c.Children.First();
                } while (Current.Children.Count() == 1);
                return this.Current;
            }
            return null;
        }

        public ITreeElement GoToParent()
        {
            if(stack.Count > 0)
            {
                do
                {
                    Current = stack.Pop();
                } while (stack.Count > 0 && this.Current.Children.Count() == 1);
                return this.Current;
            }
            return null;
        }

        public ITreeElement Root { get; }
        public ITreeElement Current { get; private set; }
        
        public override string ToString()
        {
            return this.ToString(this.Current, 0);
        }

        private string ToString(ITreeElement element, int depth)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(' ', depth).Append(element.Caption);
            foreach(IFilter f in element.AvailableFilters)
            {
                builder.Append("   ").Append(f.Name).Append(": ");
                foreach (string s in element.GetFilterValues(f))
                    builder.Append(s).Append(" ");
            }
            foreach(ITreeElement el in element.Children)
            {
                builder.Append("\n").Append(this.ToString(el, depth + 1));
            }
            /*foreach(ITreeElement el in element.Children)
            {
                builder.Append(' ', depth).Append(el.Caption);
                foreach(IFilter f in el.AvailableFilters)
                {
                    builder.Append("   ").Append(f.Name).Append(": ");
                    foreach (string s in el.GetFilterValues(f))
                        builder.Append(s).Append(" ");
                }
                    
                builder.Append("\n").Append(this.ToString(el, depth + 1));
            }*/
            return builder.ToString();
        }
    }
}

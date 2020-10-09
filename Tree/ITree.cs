using Ceramic3d_filter.TreeElement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceramic3d_filter.Tree
{
    public interface ITree
    {
        ITreeElement Root { get; }
        ITreeElement Current { get; }
        ITreeElement GoToParent();
        ITreeElement GoToChild(ITreeElement child);
    }
}

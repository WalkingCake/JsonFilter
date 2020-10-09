using System;
using System.Linq;
using Ceramic3d_filter.Tree;

namespace Ceramic3d_filter
{
    public static class TestClass
    {
        public static void Main(string[] args)
        {
            Deserializer.IDeserializer deserializer = new Deserializer.Deserializer("cat_structure_demo.json", "ru");
            ITree tree = deserializer.Tree;
            tree.GoToChild(tree.Current.Children.First());
            tree.GoToChild(deserializer.GetTreeElement("item22"));
            tree.GoToChild(deserializer.GetTreeElement("item23"));
            tree.GoToParent();
            Console.WriteLine(tree.ToString());
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using Ceramic3d_filter.Filter;
using Ceramic3d_filter.Tree;
using Ceramic3d_filter.TreeElement;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ceramic3d_filter.Deserializer
{
    public class Deserializer : IDeserializer
    {
        private class AuxiliaryCaption
        {
            [JsonProperty("ru")]
            public string Ru { get; set; }

            [JsonProperty("en")]
            public string En { get; set; }
        }
        private class AuxiliaryValue
        {
            [JsonProperty("caption")]
            public AuxiliaryCaption Caption { get; set; }
        }
        private class AuxiliaryElement
        {
            [JsonProperty("values")]
            public Dictionary<string, AuxiliaryValue> Values { get; set; }
            [JsonProperty("caption")]
            public AuxiliaryCaption Caption { get; set; }
        }
        private class AuxiliaryItems
        {
            [JsonProperty("items")]
            public Dictionary<string, AuxiliaryElement> Items { get; set; }
        }

        private class AuxiliaryAvailableFilters
        {
            [JsonProperty("items")]
            public Dictionary<string, AuxiliaryElement> Items { get; set; }
            [JsonProperty("default_list")]
            public List<string> DefaultList { get; set; }
        }




        private class AuxiliaryItem
        {
            [JsonProperty("filter_list")]
            public List<string> FilterList { get; set; }
            [JsonProperty("caption")]
            public AuxiliaryCaption Caption { get; set; }
            [JsonProperty("filters")]
            public Dictionary<string, List<string>> Filters { get; set; }
        }

        private class AuxiliaryTreeNode
        {
            public AuxiliaryTreeNode()
            {
                this.children = new Dictionary<string, AuxiliaryTreeNode>();
            }
            private readonly Dictionary<string, AuxiliaryTreeNode> children;
            public Dictionary<string, AuxiliaryTreeNode> Children { get { return this.children; } }
        }
        private class AuxiliaryDataStructure
        {
            [JsonProperty("available_filters")]
            public AuxiliaryAvailableFilters AvailableFilters { get; set; }

            [JsonProperty("tree_elements")]
            public Dictionary<string, AuxiliaryItem> TreeElements { get; set; }

            [JsonIgnore]
            public AuxiliaryTreeNode Tree { get; set; }
        }
        private static void FillAuxiliaryTree(JToken token, ref AuxiliaryTreeNode node)
        {
            if (token is JProperty)
            {
                AuxiliaryTreeNode child = new AuxiliaryTreeNode();
                foreach (JToken t in token.Children())
                {
                    FillAuxiliaryTree(t, ref child);
                }
                node.Children.Add(((JProperty)token).Name, child);
            }
            else if (token is JObject)
            {
                foreach (JToken t in token.Children())
                {
                    FillAuxiliaryTree(t, ref node);
                }
            }
        }

        private string GetNameWithLocalization(AuxiliaryCaption caption)
        {
            if (this.localization == "ru")
                return caption.Ru;
            if (this.localization == "en")
                return caption.En;
            return "ru: " + caption.Ru + ", en: " + caption.En;
        }

        private readonly AuxiliaryDataStructure dataStructure;
        private readonly HashSet<IFilter> availableFilters;
        private readonly HashSet<IFilter> defaultFilters;
        private readonly string localization;
        private readonly HashSet<ITreeElement> treeElements;

        public Deserializer(string filepath, string localization)
        {
            this.localization = localization;
            
            //Parse JSON into our auxiliary system
            string data = System.IO.File.ReadAllText(filepath);
            this.dataStructure = new AuxiliaryDataStructure();
            JToken token = (JToken)JsonConvert.DeserializeObject(data);
            this.dataStructure = token.ToObject<AuxiliaryDataStructure>();
            JToken treeToken = token.Children<JProperty>().FirstOrDefault(x => x.Name == "tree");
            AuxiliaryTreeNode tree = new AuxiliaryTreeNode();
            FillAuxiliaryTree(treeToken, ref tree);
            this.dataStructure.Tree = tree;

            //Set available_filters category
            this.availableFilters = new HashSet<IFilter>(dataStructure.AvailableFilters.Items.Values
                .Select(x => new Filter.Filter(this.GetNameWithLocalization(x.Caption), x.Values.Values
                .Select(y => this.GetNameWithLocalization(y.Caption)))));

            this.defaultFilters = new HashSet<IFilter>(this.availableFilters
                .Where(x => this.dataStructure.AvailableFilters.Items
                    .Where(y => this.dataStructure.AvailableFilters.DefaultList.Contains(y.Key))
                    .Select(z => this.GetNameWithLocalization(z.Value.Caption))
                .Contains(x.Name)));

            //Set tree_elements category
            this.treeElements = new HashSet<ITreeElement>();
            foreach(KeyValuePair<string, AuxiliaryItem> pair in this.dataStructure.TreeElements)
            {
                Dictionary<IFilter, IEnumerable<string>> filters = new Dictionary<IFilter, IEnumerable<string>>();
                if(pair.Value.Filters != null)
                    foreach(KeyValuePair<string, List<string>> f in pair.Value.Filters)
                        filters.Add(this.GetFilter(f.Key), f.Value.Select(fil => this.GetNameWithLocalization(this.dataStructure.AvailableFilters.Items[f.Key].Values[fil].Caption)));
                this.treeElements.Add(new TreeElement.TreeElement(
                    pair.Key,
                    pair.Value.Caption == null ? pair.Key : this.GetNameWithLocalization(pair.Value.Caption),
                    this.defaultFilters
                        .Union(pair.Value.FilterList == null ? new List<IFilter>() : this.availableFilters
                            .Where(x => this.dataStructure.AvailableFilters.Items
                                .Where(y => pair.Value.FilterList.Contains(y.Key))
                                .Select(z => this.GetNameWithLocalization(z.Value.Caption))
                            .Contains(x.Name))),
                    filters
                    ));
            }

            //Generate Tree
            ITreeElement root = this.GetTreeElement(this.dataStructure.Tree.Children.First().Key);
            this.FillResultTree(root, this.dataStructure.Tree.Children.First().Value);
            this.Tree = new Tree.Tree(root);
        }
        
        private void FillResultTree(ITreeElement treeElement, AuxiliaryTreeNode nodeInfo)
        {
            foreach(KeyValuePair<string, AuxiliaryTreeNode> pair in nodeInfo.Children)
            {
                ITreeElement child = this.GetTreeElement(pair.Key);
                FillResultTree(child, pair.Value);
                treeElement.AddChild(child);
            }
        }
        
        public IFilter GetFilter(string name)
        {
            return this.availableFilters
                .First(x => x.Name == this.GetNameWithLocalization(this.dataStructure.AvailableFilters.Items[name].Caption));
        }

        public ITreeElement GetTreeElement(string name)
        {
            return this.treeElements.First(x => x.Name.Equals(name));
        }

        public IEnumerable<IFilter> AvailableFilters { get { return this.availableFilters; } }
        public IEnumerable<IFilter> DefaultFilters { get { return this.defaultFilters; } }
        public IEnumerable<ITreeElement> TreeElements { get { return this.treeElements; } }
        public ITree Tree { get; }


    }
}

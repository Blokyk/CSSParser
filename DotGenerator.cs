using System;
using System.Text;
using System.Collections.Generic;

namespace DotGenerator
{
    public class Graph
    {
        public string name;

        public List<GraphNode> rootNodes;

        public Graph(string name)
        {
            this.name = name;
            rootNodes = new List<GraphNode>();
        }

        public void AddNode(GraphNode node) => rootNodes.Add(node);

        public string ToText()
        {
            var strBuilder = new StringBuilder();

            strBuilder.AppendLine("digraph " + name + " {");
            strBuilder.AppendLine();

            var registry = new List<GraphNode>();

            foreach (var node in rootNodes)
            {
                registry.Add(node);
                strBuilder.AppendLine("\t" + node.ToText(registry));
            }

            strBuilder.AppendLine("\n}");

            return strBuilder.ToString();
        }
    }

    [System.Diagnostics.DebuggerDisplay("{id}:{label}")]
    public class GraphNode
    {
        public string id;
        public string label;

        public List<GraphNode> childrens;

        public GraphNode(string id)
        {
            this.id = id;
            label = id;
            childrens = new List<GraphNode>();
        }

        public GraphNode(string id, string label)
        {
            this.id = id;
            this.label = label.Replace('"', '\'');
            childrens = new List<GraphNode>();
        }

        public GraphNode(int id, string label) : this(id.ToString(), label) {}

        public void AddNode(GraphNode node) {
            if (node != null) {
                childrens.Add(node);
            } else {
                //Console.WriteLine("Node was null");
            }
        }

        public string ToText(List<GraphNode> registry)
        {
            var strBuilder = new StringBuilder();

            foreach (var child in childrens)
            {
                strBuilder.AppendLine($"{id} [label=\"{label}\"]");
                strBuilder.AppendLine("\t" + id + " -> " + child.id);

                if (!registry.Contains(child))
                {
                    registry.Add(child);
                    strBuilder.Append("\t" + child.ToText(registry));
                }
            }

            if (childrens.Count == 0)
            {
                strBuilder.Append($"{id} [label=\"{label}\"]\n\t");
            }

            return strBuilder.ToString();
        }
    }
}

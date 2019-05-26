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

            strBuilder.AppendLine("digraph " + name);
            strBuilder.AppendLine("{");

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

    [System.Diagnostics.DebuggerDisplay("{name}:{label}")]
    public class GraphNode
    {
        public string name;
        public string label;

        public List<GraphNode> childrens;

        public GraphNode(string name)
        {
            this.name = name;
            label = name;
            childrens = new List<GraphNode>();
        }

        public GraphNode(string name, string label)
        {
            this.name = name;
            this.label = label;
            childrens = new List<GraphNode>();
        }

        public void AddNode(GraphNode node) => childrens.Add(node);

        public string ToText(List<GraphNode> registry)
        {
            var strBuilder = new StringBuilder();

            foreach (var child in childrens)
            {
                strBuilder.AppendLine($"{name} [label={label}]");
                strBuilder.AppendLine("\t" + name + " -> " + child.name);

                if (!registry.Contains(child))
                {
                    registry.Add(child);
                    strBuilder.Append("\t" + child.ToText(registry));
                }
            }

            if (childrens.Count == 0)
            {
                strBuilder.Append($"{name} [label={label}]\n\t");
            }

            return strBuilder.ToString();
        }
    }
}

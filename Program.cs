
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CSSParser;
using CSSParser.Helpers;

using DotGenerator;

namespace CSSParser
{
    static class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllText(Directory.GetCurrentDirectory() + "/res/simple.css");

            lines = MiscHelpers.Preprocess(lines);

            /* foreach (var line in lines) {
                Console.WriteLine(line);
            }*/

            var tokens = Tokenizer.Tokenize(lines);

            foreach (var token in tokens)
            {
                Console.WriteLine(token.kind + " : " + token);
            }

            var rules = Parser.ParseStylesheet(tokens).cssRules;

            var graph = new Graph("Stylesheet");

            for (int i = 0; i < rules.Count; i++)
            {
                var rootNode = new GraphNode(rules[i].GetHashCode().ToString(), "root");

                rootNode.AddNode(ToGraphNode(rules[i]));
            }

            Console.WriteLine(graph.ToText());

            //Console.WriteLine(new SimpleBlockNode(new Token(" ", TokenKind.whitespaceToken)).token);
        }

        static GraphNode ToGraphNode(RuleNode node)
        {
            var root = new GraphNode(node.GetHashCode().ToString());

            var tempCompoNode = new GraphNode((node.GetHashCode() + "compo").GetHashCode().ToString(), "compo");

            foreach (var compoValue in ((RuleNode)node).prelude)
            {
                tempCompoNode.AddNode(ToGraphNode(compoValue));
            }

            var tempBlockNode = new GraphNode((node.GetHashCode() + "block").GetHashCode().ToString(), "block");
            tempBlockNode.AddNode(ToGraphNode(((RuleNode)node).block));

            root.AddNode(tempCompoNode);
            root.AddNode(tempBlockNode);

            return root;
        }
    }
}


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
        static Random random = new Random();

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
                //Console.WriteLine(token.kind + " : " + token);
            }

            var rules = Parser.ParseStylesheet(tokens).cssRules;

            var graph = new Graph("Stylesheet");
            
            var rootNode = new GraphNode("Hello world".GetHashCode().ToString(), "root");

            for (int i = 0; i < rules.Count; i++)
            {
                graph.AddNode(ToGraphNode((dynamic)rules[i]));
            }

            //graph.AddNode(rootNode);

            var graphText = graph.ToText();

            Console.WriteLine(graph.ToText());

            //Console.WriteLine(new SimpleBlockNode(new Token(" ", TokenKind.whitespaceToken)).token);
        }

        static GraphNode ToGraphNode(RuleNode node)
        {
            if (node == null) return null;

            var root = new GraphNode(random.Next(), node.GetType().Name);

            /* var nodeFields = node.GetType().GetFields();

            foreach (var field in nodeFields)
            {
                var temp = new GraphNode(random.Next(), field.Name);

                if (field.FieldType == typeof(List<ComponentValueNode>)) {
                    foreach (var item in (List<ComponentValueNode>)field.GetValue(node))
                    {
                        temp.AddNode(ToGraphNode(item));
                        root.AddNode(temp);
                    }

                    continue;
                }

                if (field.FieldType == typeof(Token)) {
                    var value = (Token)field.GetValue(node);
                    temp.AddNode(new GraphNode(random.Next(), value.GetRepresentation()));
                    root.AddNode(temp);
                    continue;
                }

                if (field.FieldType == typeof(string)) {
                    var value = (string)field.GetValue(node);
                    temp.AddNode(new GraphNode(random.Next(), value));
                    root.AddNode(temp);
                    continue;
                }

                //Console.WriteLine(field.Name + " in " + field.DeclaringType + " : " + field);
                //Console.WriteLine();
                //Console.WriteLine(field.DeclaringType + " " + (field.DeclaringType == typeof(List<ComponentValueNode>)));
            }*/

            var tempCompoNode = new GraphNode(random.Next(), "prelude");  

            foreach (var compoValue in node.prelude)
            {
                if (compoValue == null) continue;
                tempCompoNode.AddNode(ToGraphNode((dynamic)compoValue));
            }

            var tempBlockNode = new GraphNode(random.Next(), "block");
            if (node.block != null) tempBlockNode.AddNode(ToGraphNode((dynamic)node.block));

            root.AddNode(tempCompoNode);
            root.AddNode(tempBlockNode);

            return root;
        }

        static GraphNode ToGraphNode(AtRuleNode node)
        {
            if (node == null) return null;

            //Console.WriteLine("At");

            var root = ToGraphNode((RuleNode)node);

            root.label = node.name;

            return root;
        }

        static GraphNode ToGraphNode(SimpleBlockNode node)
        {
            if (node == null) return null;

            var root = new GraphNode(random.Next(), "value");

            foreach (var value in node.value)
            {
                root.AddNode(ToGraphNode((dynamic)value));
            }

            //Console.WriteLine("Simple");

            return root;
        }

        static GraphNode ToGraphNode(ComponentValueNode node)
        {
            if (node == null) return null;

            /* Console.WriteLine("Compo");
            Console.WriteLine(node.ToString());
            Console.WriteLine();*/

            var root = new GraphNode(random.Next(), "Component Value");

            var preludeNode = new GraphNode(random.Next(), "prelude");

            foreach (var prelude in node.prelude)
            {
                preludeNode.AddNode(ToGraphNode((dynamic)prelude));
            }

            root.AddNode(preludeNode);

            if (node.block != null) {
                var blockNode = new GraphNode(random.Next(), "block");
                blockNode.AddNode(ToGraphNode((dynamic)node.block));
                root.AddNode(blockNode);
            }

            return root;
        }

        static GraphNode ToGraphNode(PreservedTokensNode node)
        {
            if (node == null) return null;

            if (node.token == TokenKind.whitespaceToken || node.token == TokenKind.semicolonToken) return null;

            /* Console.WriteLine("Preserved");
            Console.WriteLine(node.token.GetRepresentation());
            Console.WriteLine();*/

            return new GraphNode(random.Next(), node.token.GetRepresentation());
        }

        static GraphNode ToGraphNode(QualifiedRuleNode node) 
        {
            if (node == null) return null;

            var root = new GraphNode(random.Next(), "Qualified Rule");

            var preludeNode = new GraphNode(random.Next(), "prelude");

            foreach (var prelude in node.prelude)
            {
                preludeNode.AddNode(ToGraphNode((dynamic)prelude));
            }

            root.AddNode(preludeNode);

            if (node.block != null) {
                var blockNode = new GraphNode(random.Next(), "block");
                blockNode.AddNode(ToGraphNode((dynamic)node.block));
                root.AddNode(blockNode);
            }

            return root;
        }

        /*static GraphNode ToGraphNode(RuleNode node)
        {
            if (node == null) {
                return new GraphNode(random.Next().ToString(), "null");
            }

            var root = new GraphNode(node.GetHashCode().ToString(), "name");

            var tempCompoNode = new GraphNode((node.GetHashCode() + "prelude").GetHashCode().ToString(), "prelude");

            foreach (var compoValue in node.prelude)
            {
                tempCompoNode.AddNode(ToGraphNode(compoValue));
            }

            var tempBlockNode = new GraphNode((node.GetHashCode() + "block").GetHashCode().ToString(), "block");
            tempBlockNode.AddNode(ToGraphNode(node.block));

            root.AddNode(tempCompoNode);
            root.AddNode(tempBlockNode);

            return root;
        }*/
    }
}

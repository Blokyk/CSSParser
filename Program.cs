
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CSSParser;
using CSSParser.Helpers;

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

            /*foreach (var token in tokens)
            {
                Console.WriteLine(token.kind + " : " + token);
            }*/

            var rules = ParseStylesheet(tokens).cssRules;

            foreach (RuleNode rule in rules)
            {
                foreach (var prelude in rule.prelude)
                {
                    // TODO: finish the ToString() functions in the different RuleNode classes
                }
            }

            //Console.WriteLine(new SimpleBlockNode(new Token(" ", TokenKind.whitespaceToken)).token);
        }
    }
}

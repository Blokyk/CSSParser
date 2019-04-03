
using System;
using System.IO;
using System.Collections.Generic;

using System.Linq;

namespace cssparser
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines(Directory.GetCurrentDirectory() + "/res/simple.css").ToList();

            for (int i = 0; i < lines.Count; i++)
            {
                var escaped = Escape(lines[i]);

                var escapedArray = escaped.Split(';');

                lines.RemoveAt(i);

                foreach (var str in escapedArray)
                {
                    lines.Insert(i++, str);
                }
                

            }

            Tokenize(lines);
        }

        // See https://www.w3.org/TR/css-syntax-3/#tokenizer-algorithms for reference
        static void Tokenize(string[] lines) {
            var globalTokens = new List<Tokens[]>();

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var lineTokens = new List<Tokens>();
                for (int j = 0; j < line.Length; j++)
                {
                    var currToken = line[j];

                    if (Char.IsWhiteSpace(currToken)) {
                        lineTokens.Add(Tokens.whitespaceToken);
                        continue;
                    }

                    if (currToken == '\u0022') {
                        
                    }
                }
            }
        }

        static (StringToken token, int offset) TokenizeString(string line) {
            var token = new StringToken("");
            int i = 0;

            for (; i < line; i++)
            {
                if (line[i] == '\n') {
                    token.token = Tokens.badStringToken;
                    break;
                }

                if (line[i] == '\\')
                {
                    if (line[i+1] == '\n')
                    {
                        i++;
                        continue;
                    }
                }
            }

            return (token, i);
        }

        static (char codePoint, bool success) TokenizeEscape(string line) {
            if (line[0] != '\\')
            {
                return (Char.MinValue, false);
            }

            if (line[1] == '\n') {
                return (Char.MinValue, false);
            }

            return (line[1], true);
        }
    }
}

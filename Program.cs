
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cssparser
{
    static class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllText(Directory.GetCurrentDirectory() + "/res/simple.css");

            lines = Preprocess(lines);

            /* foreach (var line in lines) {
                Console.WriteLine(line);
            }*/

            Tokenize(lines);
        }

        // See https://www.w3.org/TR/css-syntax-3/#input-preprocessing
        static string Preprocess(string input) {
            return input.Replace("\u000D\u000D\u000A", "\u000A").
                    Replace('\u000D', '\u000A').
                    Replace('\u000C', '\u000A').
                    Replace('\u0000', '\uFFFD');
        }

        // See https://www.w3.org/TR/css-syntax-3/#tokenizer-algorithms for reference
        static Token[] Tokenize(string input) {
            var inputTokens = new List<Tokens>();

            for (int i = 0; i < input.Length; i++)
            {
                var currToken = input[i];

                if (Char.IsWhiteSpace(currToken)) {
                    inputTokens.Add(Tokens.whitespaceToken);
                    continue;
                }

                if (currToken == '\u0022') { // '\u0022' = '"'
                    TokenizeString(input.Substring(i + 1).ToCharArray());
                }
            }

            return null;
        }

        // See https://www.w3.org/TR/css-syntax-3/#consume-a-string-token
        static (StringToken token, int offset) TokenizeString(ReadOnlySpan<char> line) {
            var token = new StringToken("");
            token.SetToken(Tokens.badStringToken);

            int i = 0;

            for (; i < line.Length; i++)
            {
                if (line[i] == '\n') {
                    break;
                }

                if (line[i] == '\\') {
                    if (line[i+1] == '\n') {
                        i++;
                        continue;
                    }

                    if (!CheckEscape(line.Slice(i, i+1)));

                    var escaped = TokenizeEscape(line.Slice(i + 1));

                    token.Add(escaped.codePoint);
                    i += escaped.offset;
                    continue;
                }

                if (line[i] == '\u0022') { // '\u0022' == '"'
                    token.SetToken(Tokens.stringToken);
                    Console.WriteLine("finished tokenizing string \"" + line.Slice(0, i).ToString() + "\"");
                    break;
                }

                token.Add(line[i]);
            }

            return (token, i);
        }

        // See https://www.w3.org/TR/css-syntax-3/#consume-an-escaped-code-point
        static (char codePoint, int offset) TokenizeEscape(ReadOnlySpan<char> line) {

            var hexDigits = 0;
            var hexDigitsCount = 0;

            int i = 0;

            char outputCodePoint = '\uFFFD';
            
            for (; i < line.Length; i++)
            {
                if (IsHexDigit(line[i])) {
                    hexDigits += (int)line[i];
                    hexDigitsCount++;

                    if (hexDigitsCount == 5) {
                        outputCodePoint = (char)hexDigits;

                        if (hexDigits == 0 ||                                    // If it's a null character escape
                            outputCodePoint.IsBetween((int)'\ud800', (int)'\udfff') || // Or if it's a surrogate code point
                            outputCodePoint.IsBetween(1114111, Char.MaxValue))         // Or if it's 
                        {
                            return ('\uFFFD', i);
                        }

                        return (outputCodePoint, i);
                    }
                } else {
                    Console.WriteLine("Escaped character \'" + line[i] + "\' because it isn't an hex digit");
                    outputCodePoint = line[i];
                }
            }

            return (outputCodePoint, i);
        }

        // See https://www.w3.org/TR/css-syntax-3/#starts-with-a-valid-escape
        static bool CheckEscape(ReadOnlySpan<char> line) {
            if (line[0] != '\\') {
                return false;
            }

            if (line[1] == '\n') {
                return false;
            }

            return true;
        }

        // See https://www.w3.org/TR/css-syntax-3/#hex-digit
        static bool IsHexDigit(char codePoint) {

            return codePoint.IsBetween(65, 70)  ||  // If it's between 'A' and 'F'
                   codePoint.IsBetween(97, 102) ||  // Or if it's between 'a' and 'f'
                   Char.IsDigit(codePoint);         // Or if it's a digit
        }

        static bool IsBetween(this char codePoint, int min, int max, bool inclusive=true) {
            int cpInt = (int)codePoint;

            if (inclusive) {
                return (cpInt >= min && cpInt <= max) ? true : false;
            } else {
                return (cpInt > min && cpInt < max) ? true : false;
            }
        }
    }
}

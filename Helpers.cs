using System;
//using 

namespace CSSParser.Helpers {
    public static class THelper {
            // See https://www.w3.org/TR/css-syntax-3/#starts-with-a-valid-escape
            public static bool CheckEscape(ReadOnlySpan<char> line) {
                if (line[0] != '\\') return false;

                if (line[1] == '\n') return false;

                return true;
            }

            // See https://www.w3.org/TR/css-syntax-3/#hex-digit
            public static bool IsHexDigit(char codePoint) {

                return codePoint.IsBetween(65, 70)  ||  // If it's between 'A' and 'F'
                       codePoint.IsBetween(97, 102) ||  // Or if it's between 'a' and 'f'
                       Char.IsDigit(codePoint);         // Or if it's a digit
            }

            public static bool IsBetween(this char codePoint, int min, int max, bool inclusive=true) {
                int cpInt = (int)codePoint;

                if (inclusive) {
                    return (cpInt >= min && cpInt <= max) ? true : false;
                } else {
                    return (cpInt > min && cpInt < max) ? true : false;
                }
            }

            public static bool IsBetween(this char codePoint, char min, char max, bool inclusive=true) {
                return codePoint.IsBetween((int)min, (int)max);
            }

            // See https://www.w3.org/TR/css-syntax-3/#name-code-point
            public static bool IsANameCodePoint(char codePoint) {
                return  (Char.IsDigit(codePoint))          || // If it's a digit
                        (codePoint == '-')                 || // Or a - (U+002D hyphen minus)
                        (IsANameStartCodePoint(codePoint));   // Of a name-start code point
            }

            // See https://www.w3.org/TR/css-syntax-3/#name-start-code-point
            public static bool IsANameStartCodePoint(char codePoint) {
                return  (Char.IsLetter(codePoint))        || // If it's a letter
                        ((int)codePoint >= (int)'\u0080') || // Or if it's a non-ascii (>= U+0080 control) char
                        (codePoint == '_');                  // Of if it's a _ (U+005F low line)
            }

            // See https://www.w3.org/TR/css-syntax-3/#check-if-three-code-points-would-start-an-identifier
            public static bool CheckIndentifier(ReadOnlySpan<char> line) {
                for (int i = 0; i < line.Length; i++) {
                    if (line[i] == '-') {
                        return  (IsANameStartCodePoint(line[i+1])) ||
                                (CheckEscape(line.Slice(i+1, 2)));
                    }

                    if (IsANameStartCodePoint(line[i])) {
                        return true;
                    }

                    if (line[i] == '\\') {
                        return (CheckEscape(line.Slice(i, 2)));
                    }
                }

                return false;
            }

            // See https://www.w3.org/TR/css-syntax-3/#check-if-three-code-points-would-start-a-number
            public static bool CheckNumber(ReadOnlySpan<char> line) {
                return  Char.IsDigit(line[1]) ||                    // If it's a digit
                       (line[1] == '.' && Char.IsDigit(line[2])); // Or if it's a full stop and then a digit
            }
            
            // See https://www.w3.org/TR/css-syntax-3/#non-printable-code-point
            public static bool IsNonPrintable(char codePoint) {
                return  (codePoint == '\u000B')                    || // If it's U+000B line tabulation
                        (codePoint == '\u007F')                    || // Or if it's U+007F delete
                        (codePoint.IsBetween('\u0000', '\u0008'))  || // Or if it's between U+0000 null and U+0008 backspace (inclusive) 
                        (codePoint.IsBetween('\u000E', '\u001F')); 
                        // Or if it's between U+000E shift out and U+001F INFORMATION SEPARATOR ONE (inclusive) 
            }


            public static void ParseError(int i, ReadOnlySpan<char> input) {
                Console.WriteLine($"/!\\ WARNING /!\\\nParse error at character '{input[i]}' at column {i}");
                Console.WriteLine($"Context : \n{input.ToString()}\n/!\\ END OF WARNING/!\\");
            }
            /*public static float ParseNumber(string representation) {

            }*/
    }

    public static class MiscHelpers {
        // See https://www.w3.org/TR/css-syntax-3/#input-preprocessing
        public static string Preprocess(string input) {
            return input.Replace("\u000D\u000D\u000A", "\u000A").
                    Replace('\u000D', '\u000A').
                    Replace('\u000C', '\u000A').
                    Replace('\u0000', '\uFFFD');
        }
    }
}
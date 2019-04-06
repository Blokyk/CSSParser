
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

using CSSParser.Helpers;

namespace CSSParser
{
    public static class Tokenizer {
        // See https://www.w3.org/TR/css-syntax-3/#tokenizer-algorithms for reference
        public static Token[] Tokenize(ReadOnlySpan<char> input) {
            var tokens = new List<Token>();

            for (int i = 0; i < input.Length; i++)
            {
                var currToken = input[i];

                while (Char.IsWhiteSpace(currToken)) {
                    tokens.Add(Tokens.whitespaceToken);
                    i++;
                    currToken = input[i];
                }

                if (currToken == '\u0022' || currToken == '\u0027') { // '\u0022' = ' " ' and \u0027 = ' ' '
                    var output = TokenizeString(input.Slice(i + 1), currToken);
                    tokens.Add(output.token);
                    i += output.offset;
                    Console.WriteLine("Tokenized string : " + output.token);
                    continue;
                }

                if (currToken == '#') {
                    if (THelper.IsANameCodePoint(input[i+1])) { // If it's a name code point
                        var hashToken = new HashToken("");
                        if (THelper.CheckIndentifier(input.Slice(i+1, 3)) || // If the next 3 code points would start an identifier
                            THelper.CheckEscape(input.Slice(i, 2)))          // Or if the next 2 code points are a valid escape
                        {
                            hashToken.SetTypeflag(TypeFlag.Id); // Set the type flag to <id>
                        }

                        var name = TokenizeName(input.Slice(i)); // Consume a name
                        hashToken.representation = new StringBuilder(name.result);
                        i += name.offset;
                        tokens.Add(hashToken); // and return it 
                        Console.WriteLine("Tokenized name : " + name.result);
                        continue;
                    }
                }

                if (currToken == '$') {
                    if (input[i + 1] == '=') {
                        i++;
                        tokens.Add(Tokens.suffixMatchToken);
                        continue;
                    }
                }

                if (currToken == '*') {
                    if (input[i + 1] == '=') {
                        i++;
                        tokens.Add(Tokens.substringMatchToken);
                        continue;
                    }
                }

                if (currToken == '+') {
                    if (THelper.CheckNumber(input.Slice(i, 3))) {
                        var number = TokenizeNumericToken(input.Slice(i));
                        tokens.Add(number.token);
                        i += number.offset;
                        continue;
                    }
                }

                if (currToken == '-') {
                    if (THelper.CheckNumber(input.Slice(i, 3))) {
                        var number = TokenizeNumericToken(input.Slice(i));
                        tokens.Add(number.token);
                        i += number.offset;
                        continue;
                    }

                    if (input[i+1] == '-' && input[i+2] == '>') {
                        tokens.Add(Tokens.CDCToken);
                        continue;
                    }
                }

                if (currToken == '.') {
                    if (THelper.CheckNumber(input.Slice(i, 3))) {
                        var number = TokenizeNumericToken(input.Slice(i)); // tokenize numeric token
                        i += number.offset; // consume number's tokens
                        tokens.Add(number.token); // add the a <number-token> with the value of the number
                        continue;
                    }
                }

                if (currToken == '/') {
                    if (input[i+1] == '*') { // If it's a comment of the form /*
                        i += 2; // consume the '/' and the '*' part
                        while (!(input[i] == '*' && input[i+1] == '/')) i++; // wait until you get */
                        i++; // to consume the '/' part as weel
                        continue;
                    }
                }

                if (currToken == ':') {
                    tokens.Add(Tokens.colonToken); // add <colon-token>
                    continue;
                }

                if (currToken == ';') {
                    tokens.Add(Tokens.semicolonToken); // add <semicolon-token>
                    continue;
                }
            
                if (currToken == '<') {
                    if (input[i+1] == '!' &&
                        input[i+2] == '-' &&
                        input[i+3] == '-') 
                    {
                        i += 3;
                        tokens.Add(Tokens.CDOToken);
                        continue;
                    }
                }

                if (currToken == '@') {
                    if (THelper.CheckIndentifier(input.Slice(i+1, 3))) {
                        var name = TokenizeName(input.Slice(i));
                        i += name.offset;
                        tokens.Add(new Token(name.result, Tokens.atToken));
                        continue;
                    }
                }

                if (currToken == '\\') {
                    if (THelper.CheckEscape(input.Slice(i, 2))) {
                        var ident = TokenizeIdentLike(input.Slice(i));
                        i += ident.offset;
                        tokens.Add(ident.token);
                        continue;
                    }
                }

                if (currToken == '^') {
                    if (input[i+1] == '=') {
                        i++;
                        tokens.Add(Tokens.prefixMatchToken);
                        continue;
                    }
                }
                
                if (Char.IsDigit(currToken)) {
                    var number = TokenizeNumericToken(input.Slice(i)); // Consume a numeric token
                    i += number.offset;
                    tokens.Add(number.token); // and add it
                    continue;
                }

                if (THelper.IsANameStartCodePoint(currToken)) {
                    var ident = TokenizeIdentLike(input.Slice(i));
                    i += ident.offset;
                    tokens.Add(ident.token);
                    continue;
                }

                if (currToken == '\u007C') {
                    if (input[i+1] == '=') {
                        i++;
                        tokens.Add(Tokens.dashMatchToken);
                        continue;
                    }

                    if (input[i+1] == '\u007C') {
                        i++;
                        tokens.Add(Tokens.columnToken);
                        continue;
                    }
                }

                if (currToken == '~') {
                    if (input[i+1] == '=') {
                        i++;
                        tokens.Add(Tokens.includeMatchToken);
                        continue;
                    }
                }

                if (currToken == 'U' || currToken == 'u') {
                    if (input[i+1] == '+') {
                        if (input[i+2] == '?' || 
                            THelper.IsHexDigit(input[i+2]))
                        {
                            i++;
                            var unicodeRange = TokenizeUnicodeRange(input.Slice(i+1));
                            i += unicodeRange.offset;
                            tokens.Add(unicodeRange.token);
                            continue;
                        }
                    }

                    var ident = TokenizeIdentLike(input.Slice(i));
                    i += ident.offset;
                    tokens.Add(ident.token);
                    continue;
                }

                if (currToken == '(') {
                    tokens.Add(Tokens.openParenToken);
                    continue;
                }

                if (currToken == ')') {
                    tokens.Add(Tokens.closeParenToken);
                    continue;
                }
            
                if (currToken =='[') {
                    tokens.Add(Tokens.openSquareToken);
                    continue;
                }

                if (currToken ==']') {
                    tokens.Add(Tokens.closeSquareToken);
                    continue;
                }

                if (currToken == '{') {
                    tokens.Add(Tokens.openCurlyToken);
                    continue;
                }

                if (currToken == '}') {
                    tokens.Add(Tokens.closeCurlyToken);
                    continue;
                }
            

                tokens.Add(Token.Default(currToken)); // otherwise add a <delim-token>
                continue;
            }

            return tokens.ToArray();
        }

        // See https://www.w3.org/TR/css-syntax-3/#consume-a-unicode-range-token
        public static (UnicodeRangeToken token, int offset) TokenizeUnicodeRange(ReadOnlySpan<char> line) {
            var token = new UnicodeRangeToken("", 0, 0);
            int i = 0;

            var strBuilder = new StringBuilder();

            while (THelper.IsHexDigit(line[i])) {
                i++;
                strBuilder.Append(line[i]);
            }

            if (i < 6) {
                token.start = Convert.ToInt32(strBuilder.Append('0', 6 - i).ToString(), 16);

                strBuilder.Remove(i, 6 - i);
                token.end = Convert.ToInt32(strBuilder.Append('F', 6 - i).ToString(), 16);

                token.representation = strBuilder.Remove(i, 6 - i).Append('?', 6 - i);

                i = 6;

                return (token, i);
            }

            token.start = Convert.ToInt32(strBuilder.ToString(), 16);

            strBuilder = new StringBuilder("");

            if (line[i+1] == '-' && THelper.IsHexDigit(line[i+2])) {
                i++;

                while (THelper.IsHexDigit(line[i])) {
                   i++;
                   strBuilder.Append(line[i]);
                }   

                token.end = Convert.ToInt32(strBuilder.ToString());

                token.representation = strBuilder;

                return (token, i);
            }

            token.end = token.start;
            
            return (token, i);
        }

        // See https://www.w3.org/TR/css-syntax-3/#consume-an-ident-like-token
        public static (Token token, int offset) TokenizeIdentLike(ReadOnlySpan<char> line) {
            var name = TokenizeName(line);

            int i = name.offset;

            if (line[i] == '(') {
                if (name.result.ToLower() == "url") {
                    var url = TokenizeUrl(line.Slice(i+1));
                    i += url.offset;
                    return (url.token, i);
                }

                return (new Token(name.result, Tokens.functionToken), name.offset);
            }

            return (new Token(name.result, Tokens.identToken), name.offset);
        }

        // See https://www.w3.org/TR/css-syntax-3/#consume-a-url-token
        public static (UrlToken token, int offset) TokenizeUrl(ReadOnlySpan<char> line) {
            var token = new UrlToken("", new StringToken(""));

            int i = 0;

            while (Char.IsWhiteSpace(line[i])) i++;

            if (line[i] == '"' || line[i] == '\'') {
                var str = TokenizeString(line.Slice(i+1), line[i]);
                i += str.offset;

                if (str.token.token == Tokens.badStringToken) {

                    THelper.ParseError(i, line);

                    token.SetToken(Tokens.badUrlToken);

                    i += ConsumeBadUrl(line.Slice(i));

                    return (token, i);
                }

                token.SetUrl(str.token.representation.ToString());

                while (Char.IsWhiteSpace(line[i])) i++;

                if (line[i+1] == ')') {
                    i++;
                    return (token, i);
                } else {

                    THelper.ParseError(i, line);

                    token.SetToken(Tokens.badUrlToken);

                    i += ConsumeBadUrl(line.Slice(i));

                    return (token, i);
                }
            }

            i++;

            for (; i < line.Length; i++)
            {
                while (Char.IsWhiteSpace(line[i])) i++;

                if (line[i+1] == ')') {
                    i++;
                    return (token, i);
                }

                if (line[i] == '"' ||
                    line[i] == '\''||
                    line[i] == '(') 
                {

                    THelper.ParseError(i, line);

                    token.SetToken(Tokens.badUrlToken);

                    i += ConsumeBadUrl(line.Slice(i));

                    return (token, i);
                }

                if (THelper.IsNonPrintable(line[i])) {

                    THelper.ParseError(i, line);

                    token.SetToken(Tokens.badUrlToken);

                    i += ConsumeBadUrl(line.Slice(i));

                    return (token, i);
                }

                if (line[i] == '\\') {

                    if (THelper.CheckEscape(line.Slice(i, 2))) {
                        var escaped = TokenizeEscape(line.Slice(i));
                        i += escaped.offset;
                        token.AppendToUrl(escaped.codePoint);
                        return (token, i);
                    }

                    THelper.ParseError(i, line);

                    token.SetToken(Tokens.badUrlToken);

                    i += ConsumeBadUrl(line.Slice(i));

                    return (token, i);
                }

                token.AppendToUrl(line[i]);
            }

            return (token, i);
        }

        // See https://www.w3.org/TR/css-syntax-3/#consume-the-remnants-of-a-bad-url
        public static int ConsumeBadUrl(ReadOnlySpan<char> line) {
            var token = new UrlToken("", new StringToken(""));
            token.SetToken(Tokens.badUrlToken);
            int i = 0;

            for (; i < line.Length; i++) {
                if (line[i] == ')') {
                    break;
                }

                if (THelper.CheckEscape(line.Slice(i))) {
                    i += TokenizeEscape(line.Slice(i)).offset;
                }   
            }
            
            return i;
        }

        // See https://www.w3.org/TR/css-syntax-3/#consume-a-name
        public static (string result, int offset) TokenizeName(ReadOnlySpan<char> line) {
            var strBuilder = new StringBuilder("");

            int i = 0;

            for (; i < line.Length; i++) {
                if (THelper.IsANameCodePoint(line[i])) {
                    strBuilder.Append(line[i]);
                    continue;
                }

                if (THelper.CheckEscape(line.Slice(i, 2))) {
                    var escaped = TokenizeEscape(line.Slice(i));
                    i += escaped.offset;
                    strBuilder.Append(escaped.codePoint);
                    continue;
                }

                return (strBuilder.ToString(), i);
            }

            return (strBuilder.ToString(), i);
        } 

        // See https://www.w3.org/TR/css-syntax-3/#consume-a-numeric-token
        public static (ComplexToken token, int offset) TokenizeNumericToken(ReadOnlySpan<char> line) {

            var numberTuple = TokenizeNumber(line);

            int i = numberTuple.offset;
            
            if (THelper.CheckIndentifier(line.Slice(i, 3))) {
                var token = new DimensionToken(numberTuple.representation, numberTuple.value, "", numberTuple.type);
                var unit = TokenizeName(line.Slice(i));
                token.SetUnit(unit.result);
                i += unit.offset;
                return (token, i);
            }

            if (line[i] == '%') {
                return (new PercentToken(numberTuple.representation, numberTuple.value), i);
            }

            return (new NumberToken(numberTuple.representation, numberTuple.value, numberTuple.type), i);
        }

        // See https://www.w3.org/TR/css-syntax-3/#consume-a-number
        public static (string representation, float value, TypeFlag type, int offset) TokenizeNumber(ReadOnlySpan<char> line) {
            var strBuilder = new StringBuilder("");
            var type = TypeFlag.Integrer;

            int i = 0;

            if (line[i] == '+' || line[i] == '-') {
                    strBuilder.Append(line[i]);
                    i++;
                }

                while (Char.IsDigit(line[i])) {
                    strBuilder.Append(line[i++]);
                }

                if (line[i] == '.' && Char.IsDigit(line[i+1])) {
                    strBuilder.Append('.');
                    i++;
                    while (Char.IsDigit(line[i])) {
                        strBuilder.Append(line[i++]);
                    }
                    type = TypeFlag.Number;
                }

                if (line[i] == 'E' || line[i] == 'e') {
                    if (line[i+1] == '+' || line[i+1] == '-') {
                        if (Char.IsDigit(line[i+2])) {
                            strBuilder.Append(line[i]);
                            strBuilder.Append(line[i+1]);
                            i += 2;
                        }

                        while (Char.IsDigit(line[i])) {
                            strBuilder.Append(line[i++]);
                        }

                        type = TypeFlag.Number;
                    }
                }

            return (strBuilder.ToString(), Single.Parse(strBuilder.ToString()), type, i);
        }

        // See https://www.w3.org/TR/css-syntax-3/#consume-a-string-token
         public static (StringToken token, int offset) TokenizeString(ReadOnlySpan<char> line, char delimChar) {
            var token = new StringToken("");
            token.SetToken(Tokens.badStringToken);


            int i = 0;

            for (; i < line.Length; i++)
            {
                if (line[i] == '\n') {
                    i--;
                    break;
                }

                if (line[i] == '\\') {
                    if (line[i+1] == '\n') {
                        i++;
                        //Console.WriteLine("at i = " + i + " line is : \n" + line.ToString());
                        continue;
                    }

                    if (!THelper.CheckEscape(line.Slice(i, i+1))) {
                        Console.WriteLine("WARNING : CheckEscape() failed at " + i + " for span " + line.Slice(i, 2).ToString());
                        break;
                    } else {
                        //Console.WriteLine("CheckEscape() successfull on : " + line.Slice(i, 2).ToString() + " at " + i);
                    }

                    var escaped = TokenizeEscape(line.Slice(i + 1));

                    token.Add(("\\" + escaped.codePoint.ToString()));
                    i += escaped.offset;
                    continue;
                }

                if (line[i] == delimChar) { // '\u0022' == '"'
                    token.SetToken(Tokens.stringToken);
                    break;
                }

                token.Add(line[i]);
            }

            return (token, i + 1);
        }

        // See https://www.w3.org/TR/css-syntax-3/#consume-an-escaped-code-point
        public static (char codePoint, int offset) TokenizeEscape(ReadOnlySpan<char> line) {

            var hexDigits = 0;
            var hexDigitsCount = 0;

            int i = 0;

            char outputCodePoint = '\uFFFD';
            
            for (; i < line.Length; i++)
            {
                if (THelper.IsHexDigit(line[i])) { 
                    hexDigits += (int)line[i];
                    hexDigitsCount++;

                    if (hexDigitsCount == 5) {
                        outputCodePoint = (char)hexDigits;

                        if (hexDigits == 0 ||                                    // If it's a null character escape
                            outputCodePoint.IsBetween((int)'\ud800', (int)'\udfff') || // Or if it's a surrogate code point
                            outputCodePoint.IsBetween(1114111, Char.MaxValue))         // Or if it's 
                        {
                            return ('\uFFFD', i + 1);
                        }

                        return (outputCodePoint, i + 1);
                    }
                } else {
                    //Console.WriteLine("Escaped character \'" + line[i] + "\' at " + i + " because it isn't an hex digit");
                    outputCodePoint = line[i];
                    break;
                }
            }

            return (outputCodePoint, i + 1);
        }
    }
}
using System;
using System.Globalization;
using System.Text;

// See https://www.w3.org/TR/css-syntax-3/#tokenization for reference
namespace CSSParser {
    public enum Tokens
    {
        identToken, functionToken, atToken, hashToken, stringToken, badStringToken,
        urlToken, badUrlToken, delimToken, numberToken, percentToken, dimensionToken,
        unicodeRangeToken, includeMatchToken, dashMatchToken, prefixMatchToken,
        suffixMatchToken, substringMatchToken, columnToken, whitespaceToken, CDOToken,
        CDCToken, colonToken, semicolonToken, commaToken, openSquareToken, closeSquareToken,
        openParenToken, closeParenToken, openCurlyToken, closeCurlyToken
    }

    public enum TypeFlag
    {
        Integrer, Number, Unrestricted, Id
    }

    [System.Diagnostics.DebuggerDisplay("{token} : {representation.ToString()}")]
    public class Token
    {
        public StringBuilder representation;

        public Tokens token;

        public Token(string codePoints, Tokens token)
        {
            representation = new StringBuilder(codePoints);
            this.token = token;
        }

        public Token(char codePoint, Tokens token) {
            representation = new StringBuilder(codePoint.ToString());
            this.token = token;
        }
        public string GetRepresentation()
        {
            return representation.ToString();
        }

        public void SetToken(Tokens newToken)
        {
            token = newToken;
        }

        public override string ToString()
        {
            return $"{representation}";
        }

        public static Token Default(char codePoint) {
            return new Token(codePoint, Tokens.delimToken);
        }

        public static implicit operator Token(Tokens token)
        {
            switch (token)
            {
                case Tokens.CDCToken:
                    return new Token("<!--", token);
                case Tokens.CDOToken:
                    return new Token("-->", token);
                case Tokens.atToken:
                    return new ComplexToken("@", token);
                case Tokens.badStringToken:
                    return new StringToken("");
                case Tokens.badUrlToken:
                    return new UrlToken("", new StringToken(""));
                case Tokens.closeCurlyToken:
                    return new Token("}", token);
                case Tokens.closeParenToken:
                    return new Token(")", token);
                case Tokens.closeSquareToken:
                    return new Token("]", token);
                case Tokens.colonToken:
                    return new Token(";", token);
                case Tokens.columnToken:
                    return new Token("||", token);
                case Tokens.commaToken:
                    return new Token(",", token);
                case Tokens.dashMatchToken:
                    return new Token("|=", token);
                case Tokens.delimToken:
                    return new Token("", token);
                case Tokens.dimensionToken:
                    return new DimensionToken("", 0, "");
                case Tokens.functionToken:
                    return new Token("_()", token);
                case Tokens.hashToken:
                    return new Token("#", token);
                case Tokens.identToken:
                    return new Token("_", token);
                case Tokens.includeMatchToken:
                    return new Token("~=", token);
                case Tokens.numberToken:
                    return new NumberToken("", 0);
                case Tokens.openCurlyToken:
                    return new Token("{", token);
                case Tokens.openParenToken:
                    return new Token("(", token);
                case Tokens.openSquareToken:
                    return new Token("[", token);
                case Tokens.percentToken:
                    return new PercentToken("%", 0);
                case Tokens.prefixMatchToken:
                    return new Token("^=", token);
                case Tokens.semicolonToken:
                    return new Token(";", token);
                case Tokens.stringToken:
                    return new StringToken("");
                case Tokens.substringMatchToken:
                    return new Token("*=", token);
                case Tokens.suffixMatchToken:
                    return new Token("$=", token);
                case Tokens.unicodeRangeToken:
                    return new UnicodeRangeToken("", -1, -1);
                case Tokens.urlToken:
                    return new UrlToken("", new StringToken(""));
                case Tokens.whitespaceToken:
                    return new Token(" ", token);
                default:
                    throw new System.NotImplementedException("This token is not implemented yet : " + token.ToString());
            }
        }
    }

    public class ComplexToken : Token
    { // ident ; function ; at ; hash ; string ; url ; 

        public ComplexToken(string codePoints, Tokens token) : base(codePoints, token) { }
    }

    public class StringToken : ComplexToken
    {

        public new Tokens token;
        public StringToken(string codePoints) : base(codePoints, Tokens.stringToken) { }

        public void Add(char representationChar)
        {
            representation.Append(representationChar);
        }

        public void Add(string representationString)
        {
            representation.Append(representationString);
        }
    }

    public class UrlToken : ComplexToken
    {

        public new Tokens token;
        public StringToken url;

        public UrlToken(string codePoints, StringToken url) : base(codePoints, Tokens.urlToken)
        {
            this.url = url;
        }

        public void SetUrl(string str) {
            representation.Append("url(" + str + ")");
            url = new StringToken(str);
        }

        public void SetUrl(StringToken token) {
            representation.Append("url(" + token.representation + ")");
            url = token;
        }

        public void AppendToUrl(char codePoint) {
            representation.Append(codePoint);
            url.Add(codePoint);
        }

        public void AppendToUrl(string str) {
            representation.Append(str);
            url.Add(str);
        }
    }

    public class HashToken : ComplexToken
    {

        public TypeFlag type = TypeFlag.Unrestricted;

        public HashToken(string codePoints) : base(codePoints, Tokens.hashToken) { }

        public void SetTypeflag(TypeFlag flag)
        {
            type = flag;
        }
    }

    public class NumberToken : ComplexToken
    {
        public TypeFlag type = TypeFlag.Integrer;
        public float value;

        public NumberToken(string codePoints, float value) : base(codePoints, Tokens.numberToken)
        {
            this.value = value;
        }

        public NumberToken(string codePoints, float value, TypeFlag flag) : base(codePoints, Tokens.numberToken)
        {
            this.value = value;
            type = flag;
        }

        public void SetTypeflag(TypeFlag flag)
        {
            type = flag;
        }
    }

    public class DimensionToken : ComplexToken
    {
        public TypeFlag type = TypeFlag.Integrer;
        public float value;
        public string unit;

        public DimensionToken(string codePoints, float value, string unit)
            : base(codePoints, Tokens.dimensionToken)
        {
            this.value = value;
            this.unit = unit;
        }

        public DimensionToken(string codePoints, float value, string unit, TypeFlag flag)
            : base(codePoints, Tokens.dimensionToken)
        {
            this.value = value;
            this.unit = unit;
            type = flag;
        }

        public void SetTypeflag(TypeFlag flag)
        {
            type = flag;
        }

        public void SetUnit(string unit) {
            this.unit = unit;
        }
    }

    public class PercentToken : ComplexToken
    {
        public float value;

        public PercentToken(string codePoints, float value) : base(codePoints, Tokens.percentToken)
        {
            this.value = value;
        }
    }

    public class UnicodeRangeToken : Token
    {
        public int start;
        public int end;

        public UnicodeRangeToken(string codePoints, int start, int end) : base(codePoints, Tokens.unicodeRangeToken)
        {
            this.start = start;
            this.end = end;
        }
    }
}
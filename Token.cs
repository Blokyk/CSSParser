using System;
using System.Globalization;
using System.Text;

// See https://www.w3.org/TR/css-syntax-3/#tokenization for reference
namespace CSSParser {
    public enum TokenKind
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

        public TokenKind kind;

        public Token(string codePoints, TokenKind token)
        {
            representation = new StringBuilder(codePoints);
            this.kind = token;
        }

        public Token(char codePoint, TokenKind token) {
            representation = new StringBuilder(codePoint.ToString());
            this.kind = token;
        }
        public string GetRepresentation()
        {
            return representation.ToString();
        }

        public void SetToken(TokenKind newToken)
        {
            kind = newToken;
        }

        public override string ToString()
        {
            return $"{representation}";
        }

        public static Token Default(char codePoint) {
            return new Token(codePoint, TokenKind.delimToken);
        }

        public static implicit operator Token(TokenKind token)
        {
            switch (token)
            {
                case TokenKind.CDCToken:
                    return new Token("<!--", token);
                case TokenKind.CDOToken:
                    return new Token("-->", token);
                case TokenKind.atToken:
                    return new ComplexToken("@", token);
                case TokenKind.badStringToken:
                    return new StringToken("");
                case TokenKind.badUrlToken:
                    return new UrlToken("", new StringToken(""));
                case TokenKind.closeCurlyToken:
                    return new Token("}", token);
                case TokenKind.closeParenToken:
                    return new Token(")", token);
                case TokenKind.closeSquareToken:
                    return new Token("]", token);
                case TokenKind.colonToken:
                    return new Token(";", token);
                case TokenKind.columnToken:
                    return new Token("||", token);
                case TokenKind.commaToken:
                    return new Token(",", token);
                case TokenKind.dashMatchToken:
                    return new Token("|=", token);
                case TokenKind.delimToken:
                    return new Token("", token);
                case TokenKind.dimensionToken:
                    return new DimensionToken("", 0, "");
                case TokenKind.functionToken:
                    return new Token("_()", token);
                case TokenKind.hashToken:
                    return new Token("#", token);
                case TokenKind.identToken:
                    return new Token("_", token);
                case TokenKind.includeMatchToken:
                    return new Token("~=", token);
                case TokenKind.numberToken:
                    return new NumberToken("", 0);
                case TokenKind.openCurlyToken:
                    return new Token("{", token);
                case TokenKind.openParenToken:
                    return new Token("(", token);
                case TokenKind.openSquareToken:
                    return new Token("[", token);
                case TokenKind.percentToken:
                    return new PercentToken("%", 0);
                case TokenKind.prefixMatchToken:
                    return new Token("^=", token);
                case TokenKind.semicolonToken:
                    return new Token(";", token);
                case TokenKind.stringToken:
                    return new StringToken("");
                case TokenKind.substringMatchToken:
                    return new Token("*=", token);
                case TokenKind.suffixMatchToken:
                    return new Token("$=", token);
                case TokenKind.unicodeRangeToken:
                    return new UnicodeRangeToken("", -1, -1);
                case TokenKind.urlToken:
                    return new UrlToken("", new StringToken(""));
                case TokenKind.whitespaceToken:
                    return new Token(" ", token);
                default:
                    throw new System.NotImplementedException("This token is not implemented yet : " + token.ToString());
            }
        }
    }

    public class ComplexToken : Token
    { // ident ; function ; at ; hash ; string ; url ; 

        public ComplexToken(string codePoints, TokenKind token) : base(codePoints, token) { }
    }

    public class StringToken : ComplexToken
    {

        public new TokenKind token;
        public StringToken(string codePoints) : base(codePoints, TokenKind.stringToken) { }

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

        public new TokenKind token;
        public StringToken url;

        public UrlToken(string codePoints, StringToken url) : base(codePoints, TokenKind.urlToken)
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

        public HashToken(string codePoints) : base(codePoints, TokenKind.hashToken) { }

        public void SetTypeflag(TypeFlag flag)
        {
            type = flag;
        }
    }

    public class NumberToken : ComplexToken
    {
        public TypeFlag type = TypeFlag.Integrer;
        public float value;

        public NumberToken(string codePoints, float value) : base(codePoints, TokenKind.numberToken)
        {
            this.value = value;
        }

        public NumberToken(string codePoints, float value, TypeFlag flag) : base(codePoints, TokenKind.numberToken)
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
            : base(codePoints, TokenKind.dimensionToken)
        {
            this.value = value;
            this.unit = unit;
        }

        public DimensionToken(string codePoints, float value, string unit, TypeFlag flag)
            : base(codePoints, TokenKind.dimensionToken)
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

        public PercentToken(string codePoints, float value) : base(codePoints, TokenKind.percentToken)
        {
            this.value = value;
        }
    }

    public class UnicodeRangeToken : Token
    {
        public int start;
        public int end;

        public UnicodeRangeToken(string codePoints, int start, int end) : base(codePoints, TokenKind.unicodeRangeToken)
        {
            this.start = start;
            this.end = end;
        }
    }
}
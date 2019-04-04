// See https://www.w3.org/TR/css-syntax-3/#tokenization for reference
public enum Tokens {
    identToken, functionToken, atToken, hashToken, stringToken, badStringToken,
    urlToken, badUrlToken, delimToken, numberToken, percentToken, dimensionToken,
    unicodeRangeToken, includeMatchToken, dashMatchToken, prefixMatchToken,
    suffixMatchToken, substringMatchToken, columnToken, whitespaceToken, CDOToken,
    CDCToken, colonToken, semicolonToken, commaToken, openSquareToken, closeSquareToken,
    openParenToken, closeParenToken, openCurlyToken, closeCurlyToken
}

public class Token {
    public string representation;

    public Tokens token;

    public Token(string codePoints, Tokens token) {
        representation = codePoints;
        this.token = token;
    }
}

public class ComplexToken : Token {
    public new string representation;

    public ComplexToken(string codePoints, Tokens token) : base(codePoints, token) {
        representation = codePoints;
        this.token = token;
    }
}

public class StringToken : ComplexToken {

    public new Tokens token;
    public StringToken(string codePoints) : base(codePoints, Tokens.stringToken) {}

    public void Add(char representationChar) {
        representation += representationChar;
    }
}

public class UrlToken : ComplexToken {

    public new Tokens token;
    public StringToken url;

    public UrlToken(string codePoints, StringToken url) : base(codePoints, Tokens.urlToken) {
        this.url = url;
    }
}
public class HashToken : ComplexToken {
    public enum TypeFlag { Unrestricted, Id }

    public TypeFlag type = TypeFlag.Unrestricted;

    public HashToken(string codePoints, TypeFlag typeFlag) : base(codePoints, Tokens.hashToken) {
        type = typeFlag;
    }
}

public class NumberToken : ComplexToken {
    public enum TypeFlag { Integrer, Number}
    public TypeFlag type = TypeFlag.Integrer;
    public float value;

    public NumberToken(string codePoints, float value, TypeFlag typeFlag) : base(codePoints, Tokens.numberToken) {
        this.value = value;
        type = typeFlag;
    }
}

public class DimensionToken : ComplexToken {
    public enum TypeFlag { Integrer, Number}
    public TypeFlag type = TypeFlag.Integrer;
    public float value;
    public string unit;

    public DimensionToken(string codePoints, float value, TypeFlag typeFlag, string unit) 
        : base(codePoints, Tokens.dimensionToken) 
    {
        this.value = value;
        type = typeFlag;
        this.unit = unit;
    }
}

public class PercentToken : ComplexToken {
    public float value;

    public PercentToken(string codePoints, float value) : base(codePoints, Tokens.percentToken) {
        this.value = value;
    }
}

public class UnicodeRangeToken : Token {
    public int start;
    public int end;
    
    public UnicodeRangeToken(string codePoints, int start, int end) : base(codePoints, Tokens.unicodeRangeToken) {
        this.start = start;
        this.end = end;
    }
}

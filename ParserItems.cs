using System;
//using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using CSSParser;
using CSSParser.Helpers;
using CSSParser.Tree;

namespace CSSParser 
{
    public enum MatchType
    {
        ExistMatch, ExactMatch, IncludeMatch, DashMatch, PrefixMatch, SuffixMatchToken, SubstringMatch
    }


    public class RuleNode 
    {
        public List<ComponentValueNode> prelude;
        public SimpleBlockNode block;

        public RuleNode() {
            prelude = new List<ComponentValueNode>();
        }
    }

    public class AtRuleNode : RuleNode
    {
        public string name;

        public AtRuleNode(string name) : base(){
            this.name = name;
        }
    }

    public class QualifiedRuleNode : RuleNode
    {

    }

    public class DeclarationNode : RuleNode
    {
        public string name;
        public bool important; 
    }

    public class ComponentValueNode : RuleNode
    {
        public override string ToString() {
            var strBuilder = new StringBuilder();

            foreach (var prelude in this.prelude) {
                strBuilder.AppendLine(prelude.ToString());
            }

            if (this.block != null) { strBuilder.AppendLine(this.block.ToString()); }

            return strBuilder.ToString();
        }
    }

    public class PreservedTokensNode : ComponentValueNode
    {
        public Token token;

        public PreservedTokensNode(Token token) {
            this.token = token;
        }

        public override string ToString() {
            return base.ToString();
        }
    }

    public class FunctionNode : ComponentValueNode
    {
        public string name;
        public List<ComponentValueNode> value;

        public FunctionNode(string name) {
            this.name = name;
            this.value = new List<ComponentValueNode>();
        }

        public FunctionNode(string name, List<ComponentValueNode> value) {
            this.name = name;
            this.value = new List<ComponentValueNode>(value);
        }
    }

    public class SimpleBlockNode : ComponentValueNode
    {
        public List<ComponentValueNode> value;
        public Token token;

        public SimpleBlockNode(Token token) {
            this.token = token;
            this.value = new List<ComponentValueNode>();
        }

        public SimpleBlockNode(Token token, List<ComponentValueNode> value) {
            this.token = token;
            this.value = new List<ComponentValueNode>(value);
        }
    }

    public class SelectorNode
    {

    }

    public class SimpleSelectorNode : SelectorNode
    {

    }

    public class TypeSelectorNode : SimpleSelectorNode
    {
        public string tagName;

        public TypeSelectorNode(string type)
        {
            tagName = type;
        }
    }

    public class UniversalTypeSelectorNode : SimpleSelectorNode
    {

    }

    public class AttributeSelectorNode : SimpleSelectorNode
    {
        public string attribute;
        public ComplexToken attrValue;

        public MatchType matchType;

        public bool caseSensitive;

        public AttributeSelectorNode(string attr, bool caseSensitive) : this(attr, (ComplexToken)null, MatchType.ExistMatch, caseSensitive)
        {

        }

        public AttributeSelectorNode(string attr, ComplexToken value, MatchType matchType, bool caseSensitive) {
            attribute = attr;
            attrValue = value;
            this.matchType = matchType;
            this.caseSensitive = caseSensitive;
        }

        public AttributeSelectorNode(string attr, string value, MatchType matchType, bool caseSensitive) {
            attribute = attr;
            attrValue = new ComplexToken(value, TokenKind.stringToken);
            this.matchType = matchType;
            this.caseSensitive = caseSensitive;
        }
    }

    public class ClassSelectorNode : AttributeSelectorNode
    {
        public new ComplexToken[] attrValue;

        public ClassSelectorNode(string className) : base("class", className, MatchType.ExactMatch, true) {
            attribute = "class";
            attrValue = new ComplexToken[] { new StringToken(className) };
        }

        public ClassSelectorNode(string[] classNames) : base("class", true) {
            attribute = "class";
            attrValue = classNames.Apply<string, ComplexToken>(item => new StringToken(item)).ToArray();
        }
    }

    public class CSSStyleSheet
    {
        public List<RuleNode> cssRules;
        public List<SelectorNode> cssSelectors;

        public CSSStyleSheet() {
            cssRules = new List<RuleNode>();
            cssSelectors = new List<SelectorNode>();
        }
    }
}
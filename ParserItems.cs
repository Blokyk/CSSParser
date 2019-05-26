using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using CSSParser;
using CSSParser.Helpers;
using CSSParser.Tree;

namespace CSSParser 
{
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

            strBuilder.AppendLine(this.block.ToString());

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

    public class CSSStyleSheet
    {
        public List<RuleNode> cssRules;

        public CSSStyleSheet() {
            cssRules = new List<RuleNode>();
        }
    }
}
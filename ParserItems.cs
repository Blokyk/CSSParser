using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using CSSParser;
using CSSParser.Helpers;
using CSSParser.Tree;

namespace CSSParser 
{
    public class SyntaxNode 
    {
        
    }

    public class RuleNode : SyntaxNode 
    {
        public List<ComponentValueNode> prelude;
        public SimpleBlockNode block;
    }

    public class AtRuleNode : RuleNode
    {
        public string name;
    }

    public class QualifiedRuleNode : RuleNode
    {

    }

    public class DeclarationNode : SyntaxNode
    {
        public string name;
        public List<ComponentValueNode> value;
        public bool important; 
    }

    public class ComponentValueNode : SyntaxNode
    {
        
    }

    public class PreservedTokensNode : ComponentValueNode
    {
        public Token token;

        public PreservedTokensNode(Token token) {
            this.token = token;
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
}
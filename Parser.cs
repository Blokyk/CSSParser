using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

using CSSParser.Helpers;

namespace CSSParser 
{
    public class Parser 
    {

        // See https://www.w3.org/TR/css-syntax-3/#parse-a-stylesheet
        public static SyntaxNode ParseStylesheet(ReadOnlySpan<Token> input) {
            var root = new SyntaxNode();

            return root;
        }

        // See https://www.w3.org/TR/css-syntax-3/#consume-a-list-of-rules
        public static (List<RuleNode> ruleList, int offset) ParseListOfRules(ReadOnlySpan<Token> input, bool topLevel) {
            var output = new List<RuleNode>();
            Token currToken;

            int i = 0;
            for (; i < input.Length; i++) {
                currToken = input[i];

                if (currToken.kind == TokenKind.whitespaceToken) {
                    continue;
                }

                if (currToken.kind == TokenKind.atToken) {

                }
            }

            return (output, i);
        }

        // See https://www.w3.org/TR/css-syntax-3/#consume-an-at-rule0
        public static (AtRuleNode node, int offset) ParseAtRule(ReadOnlySpan<Token> input) {
            var output = new AtRuleNode();
            Token currToken;

            int i = 0;
            for (; i < input.Length; i++) {
                currToken = input[i];
            }

            return (output, i);
        }

        // See https://www.w3.org/TR/css-syntax-3/#consume-a-simple-block
        public static (SimpleBlockNode node, int offset) ParseSimpleBlock(ReadOnlySpan<Token> input) {

            if (input.Length == 0) {
                return (new SimpleBlockNode(TokenKind.delimToken), 0);
            }

            TokenKind endingTokenKind = input[0].kind.Mirror();
            var output = new SimpleBlockNode(input[0]);
            Token currToken;

            int i = 0;
            for (; i < input.Length; i++) {
                currToken = input[i];

                if (currToken.kind == endingTokenKind) {
                    break;
                }


            }

            return (output, i);
        }

        // See https://www.w3.org/TR/css-syntax-3/#consume-a-component-value
        public static (ComponentValueNode node, int offset) ParseCompoValue(ReadOnlySpan<Token> input) {
            var currToken = input[1];
                
                if (currToken.kind == TokenKind.openCurlyToken
                    || currToken.kind == TokenKind.openParenToken
                    || currToken.kind == TokenKind.openSquareToken)
                {
                    var block = ParseSimpleBlock(input.Slice(1));

                    return (block.node, block.offset);
                }

                if (currToken.kind == TokenKind.functionToken) {
                    var function = ParseFunction(input.Slice(2)); /* 2 => 1 (i) + 1 (index increment) */

                    return (function.node, function.offset);
                }

            return (new PreservedTokensNode(input[1]), 1);
        }

        // See https://www.w3.org/TR/css-syntax-3/#consume-a-function
        public static (FunctionNode node, int offset) ParseFunction(ReadOnlySpan<Token> input) {

            if (input.Length == 0) {
                return (new FunctionNode(""), 0);
            }

            var output = new FunctionNode(input[0].GetRepresentation());
            Token currToken;

            int i = 0;
            for (; i < input.Length; i++) {
                currToken = input[i];                

                if (currToken.kind == TokenKind.closeParenToken) {
                    break;
                }

                var compoValue = ParseCompoValue(input.Slice(i));

                output.value.Add(compoValue.node);
                i += compoValue.offset;
            }

            return (output, i);
        }
    }
}
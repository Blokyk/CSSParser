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
        public static CSSStyleSheet ParseStylesheet(ReadOnlySpan<Token> input) {
            var output = new CSSStyleSheet();

            var rules = ParseListOfRules(input, true);
            
            output.cssRules = rules;

            return output;
        }

        // See https://www.w3.org/TR/css-syntax-3/#consume-a-list-of-rules
        public static (List<RuleNode> ruleList, int offset) ParseListOfRules(ReadOnlySpan<Token> input, bool topLevel) {
            var output = new List<RuleNode>();
            Token currToken;

            int i = 0;
            for (; i < input.Length; i++) {
                currToken = input[i];

                if (currToken == TokenKind.whitespaceToken) {
                    continue;
                }

                if (currToken == TokenKind.atToken) {
                    var atRule = ParseAtRule(input.Slice(i + 1));

                    output.Add(atRule.node);
                    i += atRule.offset;
                    continue;
                }

                if (currToken == TokenKind.CDOToken
                    || currToken == TokenKind.CDCToken) 
                {
                    if (topLevel) {
                        continue;
                    }

                    var qualifiedRule = ParseQualifiedRule(input.Slice(i));

                    if (qualifiedRule == null) {
                        continue;
                    }

                    output.Add(qualifiedRule.node);
                    i += qualifiedRule.offset;
                    continue;
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

                if (currToken == TokenKind.openCurlyToken) {
                    var simpleBlock = ParseSimpleBlock(input.Slice(i));

                    output.block = simpleBlock.node;
                    i += simpleBlock.offset;
                    break;
                }

                if (currToken == TokenKind.semicolonToken) {
                    break;
                }

                var compo = ParseCompoValue(input.Slice(i));

                output.prelude.Add(compo.node);
                i += compo.offset;
            }

            return (output, i);
        }

        // See https://www.w3.org/TR/css-syntax-3/#consume-a-simple-block
        public static (SimpleBlockNode node, int offset) ParseSimpleBlock(ReadOnlySpan<Token> input) {

            if (input.Length == 0) {
                return (new SimpleBlockNode(TokenKind.delimToken), 0);
            }

            TokenKind endingToken = input[0].kind.Mirror();
            var output = new SimpleBlockNode(input[0]);
            Token currToken;

            int i = 0;
            for (; i < input.Length; i++) {
                currToken = input[i];

                if (currToken == endingToken) {
                    break;
                }

                var compo = ParseCompoValue(input.Slice(i));

                output.value.Add(compo.node);
                i += compo.offset;
            }

            return (output, i);
        }

        // See https://www.w3.org/TR/css-syntax-3/#consume-a-component-value
        public static (ComponentValueNode node, int offset) ParseCompoValue(ReadOnlySpan<Token> input) {
            var currToken = input[1];
                
                if (currToken == TokenKind.openCurlyToken
                    || currToken == TokenKind.openParenToken
                    || currToken == TokenKind.openSquareToken)
                {
                    var block = ParseSimpleBlock(input.Slice(1));

                    return (block.node, block.offset);
                }

                if (currToken == TokenKind.functionToken) {
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

                if (currToken == TokenKind.closeParenToken) {
                    break;
                }

                var compo = ParseCompoValue(input.Slice(i));

                output.value.Add(compo.node);
                i += compo.offset;
            }

            return (output, i);
        }

        // See https://www.w3.org/TR/css-syntax-3/#consume-a-qualified-rule
        public static (QualifiedRuleNode node, int offset) ParseQualifiedRule(ReadOnlySpan<Token> input) {
            var output = new QualifiedRuleNode();
            Token currToken;

            int i = 0;
            for (; i < input.Length; i++) {
                currToken = input[i];

                if (currToken == TokenKind.openCurlyToken) {
                    var simpleBlock = ParseSimpleBlock(input.Slice(i));

                    output.value = simpleBlock.node;
                    i += simpleBlock.offset;
                    return (output, i);
                }

                var compo = ParseCompoValue(input.Slice(i));

                output.prelude.Add(compo.node);
                i += compo.offset;
            }

            Console.WriteLine("Parse error : EOF in qualified rule");
        }
    }
}
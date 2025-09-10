using CalculatorAvalonia.Models.ExpressionHistory;
using CalculatorAvalonia.Models.Rpn.ExpressionTokens;
using CalculatorAvalonia.Models.Rpn.Operations;
using CalculatorAvalonia.Services;
using System;
using System.Collections.Generic;

namespace CalculatorAvalonia.Models.XmlTagHandling
{
    public static class TagHandlerExtension
    {
        public static bool TryParse(this TagHandler tag, out ExpressionHistoryItem item)
        {
            var tokens = new List<ExpressionTokenBase>();
            var result = "";

            item = new(tokens, result);

            if (tag.TagName != XmlHistoryService.NODE_EXPRESSION_HISTORY_ITEM) return false;

            if (!tag.SearchInnerTagByName(XmlHistoryService.NODE_TOKENS, out var tokensTagHandler)) return false;

            foreach (var tokenTag in tokensTagHandler.InnerTags)
            {
                tokenTag.FindUntilValue(out var tokenValue);

                if (!tokenTag.TryParse(tokenValue, out var token)) return false;

                tokens.Add(token);
            }

            if (!tag.SearchInnerTagByName(XmlHistoryService.NODE_RESULT, out var resultTag)) return false;

            if (!resultTag.FindUntilValue(out result)) return false;

            item = new(tokens, result);
            return true;

        }

        public static bool TryParse(this TagHandler tag, string tokenValue, out ExpressionTokenBase expressionToken)
        {
            expressionToken = null!;
            if (!tag.TagName.Contains("ExpressionToken")) return false;

            if (tag.TagName == XmlHistoryService.NODE_NUMBER_TOKEN)
            {
                if (!double.TryParse(tokenValue, out var value)) return false;
                expressionToken = new NumberExpressionToken(value);
            }
            else if (tag.TagName == XmlHistoryService.NODE_OPERATION_TOKEN)
            {
                if (!Enum.TryParse(tokenValue, out OperationType operationType)) return false;
                expressionToken = new OperationExpressionToken(operationType);
            }
            else if (tag.TagName == XmlHistoryService.NODE_BRACKET_TOKEN)
            {
                if (!Enum.TryParse(tokenValue, out BracketType bracketType)) return false;
                expressionToken = new BracketExpressionToken(bracketType);
            }
            else
            {
                return false;
            }

            return true;
        }

        public static bool TryParse(this ExpressionHistoryItem expressionHistoryItem, out TagHandler tagHandler)
        {
            tagHandler = new TagHandler(XmlHistoryService.NODE_EXPRESSION_HISTORY_ITEM);

            var tokensTag = new TagHandler(XmlHistoryService.NODE_TOKENS);
            var resultTag = new TagHandler(XmlHistoryService.NODE_RESULT);

            foreach (var token in expressionHistoryItem.Tokens)
            {
                if (!token.TryParse(out var tokenTag)) return false;

                tokensTag.InnerTags.Add(tokenTag);
            }

            resultTag.PutValueTag(XmlHistoryService.ELEMENT_VALUE, expressionHistoryItem.Result);

            tagHandler.AddInnerTag(tokensTag);
            tagHandler.AddInnerTag(resultTag);

            return true;
        }

        public static bool TryParse(this ExpressionTokenBase token, out TagHandler tagHandler)
        {
            tagHandler = null!;

            var tagName = "";
            var valueTagName = "";
            var value = "";
            if (token is NumberExpressionToken numberToken)
            {
                tagName = XmlHistoryService.NODE_NUMBER_TOKEN;
                valueTagName = XmlHistoryService.ELEMENT_VALUE;
                value = numberToken.Value.ToString();
            }
            else if (token is OperationExpressionToken opToken)
            {
                tagName = XmlHistoryService.NODE_OPERATION_TOKEN;
                valueTagName = XmlHistoryService.ELEMENT_OPERATION_TYPE;
                value = opToken.OperationType.ToString();
            }
            else if (token is BracketExpressionToken bracketToken)
            {
                tagName = XmlHistoryService.NODE_BRACKET_TOKEN;
                valueTagName = XmlHistoryService.ELEMENT_BRACKET_TYPE;
                value = bracketToken.BracketType.ToString();
            }
            else return false;

            tagHandler = new(tagName);
            tagHandler.PutValueTag(valueTagName, value);

            return true;
        }

        public static void PutValueTag(this TagHandler tagHandler, string tagName, string value)
        {
            var valueTag = new TagHandler(tagName);
            valueTag.Value = value;

            tagHandler.InnerTags.Add(valueTag);
        }



        public static bool FindUntilValue(this TagHandler tag, out string value)
        {
            value = "";

            if (string.IsNullOrEmpty(tag.Value))
            {
                if (tag.InnerTags.Count == 0) return false;

                foreach (var innerTag in tag.InnerTags)
                {
                    if (innerTag.FindUntilValue(out value)) return true;
                }

                return false;
            }

            value = tag.Value;
            return true;
        }

        public static bool SearchInnerTagByName(this TagHandler tag, string searchName, out TagHandler foundTag)
        {
            foundTag = null!;

            if (tag.TagName != searchName)
            {
                if (tag.InnerTags.Count == 0) return false;

                foreach (var innerTag in tag.InnerTags)
                {
                    if (innerTag.SearchInnerTagByName(searchName, out foundTag)) return true;
                }

                return false;
            }

            foundTag = tag;
            return true;
        }
    }
}

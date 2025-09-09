using CalculatorAvalonia.Models.ExpressionHistory;
using CalculatorAvalonia.Models.Rpn.ExpressionTokens;
using CalculatorAvalonia.Models.Rpn.Operations;
using System;
using System.Collections.Generic;
using System.Xml;

namespace CalculatorAvalonia.Services
{
    /// <summary>
    /// Reads and writes .xml files with following structure: <br/>
    /// &lt;root&gt; <br/>
    ///     &lt;ExpressionHistoryItem&gt; <br/>
    ///         &lt;Tokens&gt;<br/>
    ///             &lt;NumberExpressionToken&gt;[Value]&lt;/NumberExpressionToken&gt;<br/>
    ///             &lt;OperationExpressionToken&gt;[OperationType]&lt;/OperationExpressionToken&gt;<br/>
    ///             &lt;BracketExpressionToken&gt;[BracketType]&lt;/BracketExpressionToken&gt;<br/>
    ///         &lt;/Tokens&gt;<br/>
    ///         &lt;Result&gt;[Value]&lt;/Result &gt;<br/>
    ///     &lt;/ExpressionHistoryItem&gt;<br/>
    ///     <br/>
    ///     &lt;ExpressionHistoryItem&gt;<br/>
    ///      ...<br/>
    ///     &lt;/ExpressionHistoryItem&gt;<br/>
    ///     <br/>
    ///     ...<br/>
    ///     <br/>
    ///     &lt;ExpressionHistoryItem&gt;<br/>
    ///      ...<br/>
    ///     &lt;/ExpressionHistoryItem&gt;<br/>
    /// &lt;/root &gt;<br/>
    /// </summary>
    public static class XmlHistoryService
    {
        private const string NODE_EXPRESSION_HISTORY_ITEM = "ExpressionHistoryItem";
        private const string NODE_TOKENS = "Tokens";
        private const string NODE_NUMBER_TOKEN = "NumberExpressionToken";
        private const string NODE_OPERATION_TOKEN = "OperationExpressionToken";
        private const string NODE_BRACKET_TOKEN = "BracketExpressionToken";
        private const string NODE_RESULT = "Result";

        private const string ELEMENT_VALUE = "Value";
        private const string ELEMENT_OPERATION_TYPE = "OperationType";
        private const string ELEMENT_BRACKET_TYPE = "BracketType";

        public static void Write(IEnumerable<ExpressionHistoryItem> historyItems, string savePath)
        {
            using (XmlWriter w = XmlWriter.Create(savePath))
            {
                w.WriteStartDocument();
                w.WriteStartElement("root");

                foreach (var item in historyItems)
                {
                    w.WriteStartElement(NODE_EXPRESSION_HISTORY_ITEM);

                    w.WriteStartElement(NODE_TOKENS);
                    foreach (var token in item.Tokens)
                    {
                        if (token is NumberExpressionToken numberToken)
                        {
                            w.WriteStartElement(NODE_NUMBER_TOKEN);
                            w.WriteElementString(ELEMENT_VALUE, numberToken.Value.ToString());
                            w.WriteEndElement();
                        }
                        else if (token is OperationExpressionToken opToken)
                        {
                            w.WriteStartElement(NODE_OPERATION_TOKEN);
                            w.WriteElementString(ELEMENT_OPERATION_TYPE, opToken.OperationType.ToString());
                            w.WriteEndElement();
                        }
                        else if (token is BracketExpressionToken bracketToken)
                        {
                            w.WriteStartElement(NODE_BRACKET_TOKEN);
                            w.WriteElementString(ELEMENT_BRACKET_TYPE, bracketToken.BracketType.ToString());
                            w.WriteEndElement();
                        }
                    }
                    w.WriteEndElement();

                    w.WriteStartElement(NODE_RESULT);
                    w.WriteElementString(ELEMENT_VALUE, item.Result);
                    w.WriteEndElement();

                    w.WriteEndElement();

                }


                w.WriteEndElement();
                w.WriteEndDocument();
            }
        }

        public static bool Read(string path, out List<ExpressionHistoryItem> historyItemsList)
        {
            historyItemsList = new List<ExpressionHistoryItem>();

            using (XmlReader r = XmlReader.Create(path))
            {
                while (r.Read())
                {
                    if (r.NodeType == XmlNodeType.Element && r.Name == NODE_EXPRESSION_HISTORY_ITEM)
                    {
                        if (!ReadExpressionHistoryItemNode(r, out var item)) return false;
                        historyItemsList.Add(item);
                    }
                    else if (r.NodeType == XmlNodeType.XmlDeclaration || r.Name == "root")
                    {
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool ReadExpressionHistoryItemNode(XmlReader r, out ExpressionHistoryItem item)
        {
            var tokens = new List<ExpressionTokenBase>();
            var result = "";

            item = new(tokens, result);

            while (r.Read())
            {
                if (r.NodeType == XmlNodeType.EndElement && r.Name == NODE_EXPRESSION_HISTORY_ITEM)
                {
                    item = new(tokens, result);
                    return true;
                }

                if (r.NodeType == XmlNodeType.Element && r.Name == NODE_TOKENS)
                {
                    if (!ReadTokens(r, tokens)) return false;
                }

                if (r.NodeType == XmlNodeType.Element && r.Name == NODE_RESULT)
                {
                    r.Read();
                    if (r.NodeType == XmlNodeType.Element && r.Name == ELEMENT_VALUE)
                    {
                        result = r.ReadElementContentAsString();
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        private static bool ReadTokens(XmlReader r, List<ExpressionTokenBase> tokens)
        {
            while (r.Read())
            {
                if (r.NodeType == XmlNodeType.EndElement && r.Name == NODE_TOKENS)
                {
                    return true;
                }

                if (r.NodeType == XmlNodeType.Element && r.Name == NODE_NUMBER_TOKEN)
                {
                    if (!ReadConcreteToken(r, NODE_NUMBER_TOKEN, ELEMENT_VALUE, out var token)) return false;
                    tokens.Add(token);
                }
                else if (r.NodeType == XmlNodeType.Element && r.Name == NODE_OPERATION_TOKEN)
                {
                    if (!ReadConcreteToken(r, NODE_OPERATION_TOKEN, ELEMENT_OPERATION_TYPE, out var token)) return false;
                    tokens.Add(token);
                }
                else if (r.NodeType == XmlNodeType.Element && r.Name == NODE_BRACKET_TOKEN)
                {
                    if (!ReadConcreteToken(r, NODE_BRACKET_TOKEN, ELEMENT_BRACKET_TYPE, out var token)) return false;
                    tokens.Add(token);
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        private static bool ReadConcreteToken(XmlReader r, string tokenNode, string tokenElement, out ExpressionTokenBase token)
        {
            token = null!;

            while (r.Read())
            {
                if (r.NodeType == XmlNodeType.Element && r.Name == tokenElement)
                {
                    var elementValue = r.ReadElementContentAsString();
                    if (tokenNode == NODE_NUMBER_TOKEN && tokenElement == ELEMENT_VALUE)
                    {
                        if (!double.TryParse(elementValue, out var result)) return false;
                        token = new NumberExpressionToken(result);
                    }

                    if (tokenNode == NODE_OPERATION_TOKEN && tokenElement == ELEMENT_OPERATION_TYPE)
                    {
                        if (!Enum.TryParse(elementValue, out OperationType operationType)) return false;
                        token = new OperationExpressionToken(operationType);
                    }

                    if (tokenNode == NODE_BRACKET_TOKEN && tokenElement == ELEMENT_BRACKET_TYPE)
                    {
                        if (!Enum.TryParse(elementValue, out BracketType bracketType)) return false;
                        token = new BracketExpressionToken(bracketType);
                    }
                }

                if (r.NodeType == XmlNodeType.EndElement && r.Name == tokenNode)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

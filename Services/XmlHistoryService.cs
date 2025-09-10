using CalculatorAvalonia.Models.ExpressionHistory;
using CalculatorAvalonia.Models.XmlTagHandling;
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
        public const string NODE_EXPRESSION_HISTORY_ITEM = "ExpressionHistoryItem";
        public const string NODE_TOKENS = "Tokens";
        public const string NODE_NUMBER_TOKEN = "NumberExpressionToken";
        public const string NODE_OPERATION_TOKEN = "OperationExpressionToken";
        public const string NODE_BRACKET_TOKEN = "BracketExpressionToken";
        public const string NODE_RESULT = "Result";

        public const string ELEMENT_VALUE = "Value";
        public const string ELEMENT_OPERATION_TYPE = "OperationType";
        public const string ELEMENT_BRACKET_TYPE = "BracketType";

        public static void Write(IEnumerable<ExpressionHistoryItem> historyItems, string savePath)
        {
            using (XmlWriter w = XmlWriter.Create(savePath))
            {
                w.WriteStartDocument();
                w.WriteStartElement("root");

                foreach (var item in historyItems)
                {
                    if (!item.Wrap(out var itemTag)) return;
                    WriteElement(w, itemTag);
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
                        if (!ReadElement(r, r.Name, out var outputTag)) return false;

                        if (!outputTag.Unwrap(out var item)) return false;

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

        private static bool ReadElement(XmlReader r, string elementName, out TagHandler outputTag)
        {
            outputTag = new(elementName);

            while (r.Read())
            {
                if (r.NodeType == XmlNodeType.EndElement && r.Name == elementName)
                {
                    return true;
                }

                if (r.NodeType == XmlNodeType.Element)
                {
                    if (!ReadElement(r, r.Name, out var otherOutput)) return false;

                    outputTag.AddInnerTag(otherOutput);
                }

                if (r.NodeType == XmlNodeType.Text)
                {
                    outputTag.Value = r.Value;
                }
            }

            return false;
        }

        private static void WriteElement(XmlWriter w, TagHandler tagHandler)
        {
            if (string.IsNullOrEmpty(tagHandler.TagName)) return;

            if (string.IsNullOrEmpty(tagHandler.Value) && tagHandler.InnerTags.Count != 0)
            {
                w.WriteStartElement(tagHandler.TagName);

                foreach (var innerTag in tagHandler.InnerTags)
                {
                    WriteElement(w, innerTag);
                }

                w.WriteEndElement();
            }

            if (!string.IsNullOrEmpty(tagHandler.Value))
            {
                w.WriteElementString(tagHandler.TagName, tagHandler.Value);
            }
        }
    }
}

using System.Collections.Generic;

namespace CalculatorAvalonia.Models.XmlTagHandling
{
    public class TagHandler
    {
        public string TagName { get; }
        public List<TagHandler> InnerTags { get; }
        public string Value { get; set; }

        public TagHandler(string tagName)
        {
            TagName = tagName;
            InnerTags ??= new();
            Value = "";
        }

        public void AddInnerTag(TagHandler tagHandler)
        {
            InnerTags.Add(tagHandler);
        }

    }
}

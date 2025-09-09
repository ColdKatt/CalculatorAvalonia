using CalculatorAvalonia.Models.Rpn.ExpressionTokens;
using CalculatorAvalonia.Services;
using System.Collections.Generic;
using System.Linq;

namespace CalculatorAvalonia.Models.ExpressionHistory
{
    public class ExpressionHistoryItem
    {
        public List<ExpressionTokenBase> Tokens { get; private set; }
        public string Expression { get; private set; }
        public string FullExpression { get; private set; }
        public string Result { get; private set; }

        public ExpressionHistoryItem(List<ExpressionTokenBase> tokens, string result)
        {
            Tokens = tokens.Select((x) => x).ToList();
            Result = result;

            Expression = Tokens.ReadExpression() + " = ";
            FullExpression = Expression + Result;
        }
    }
}

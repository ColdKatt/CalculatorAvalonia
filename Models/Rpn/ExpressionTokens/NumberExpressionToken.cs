using System.Collections.Generic;

namespace CalculatorAvalonia.Models.Rpn.ExpressionTokens
{
    public sealed class NumberExpressionToken : ExpressionTokenBase
    {
        public double Value { get => _value; }

        private double _value;

        public NumberExpressionToken(double value) : base()
        {
            _value = value;
        }

        public override void Handle(Queue<ExpressionTokenBase> outputQueue, Stack<ExpressionTokenBase> operationStack)
        {
            outputQueue.Enqueue(this);
        }

        public override string Read() => _value.ToString();
    }
}

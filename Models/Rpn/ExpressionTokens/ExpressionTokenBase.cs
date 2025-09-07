using System.Collections.Generic;

namespace CalculatorAvalonia.Models.Rpn.ExpressionTokens
{
    public abstract class ExpressionTokenBase
    {
        public int Priority { get => _priority; }

        protected int _priority;

        public ExpressionTokenBase() { }

        public abstract void Handle(Queue<ExpressionTokenBase> outputQueue, Stack<ExpressionTokenBase> operationStack);

        public abstract string Read();
    }
}

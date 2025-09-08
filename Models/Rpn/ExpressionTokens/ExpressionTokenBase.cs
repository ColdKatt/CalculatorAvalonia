using System.Collections.Generic;

namespace CalculatorAvalonia.Models.Rpn.ExpressionTokens
{
    /// <summary>
    /// Base class for expression tokens.
    /// </summary>
    public abstract class ExpressionTokenBase
    {
        public int Priority { get => _priority; }

        protected int _priority;

        public ExpressionTokenBase() { }

        /// <summary>
        /// Determines the tokens behaviour during RPN parsing.
        /// </summary>
        /// <param name="outputQueue"></param>
        /// <param name="operationStack"></param>
        public abstract void Handle(Queue<ExpressionTokenBase> outputQueue, Stack<ExpressionTokenBase> operationStack);

        /// <summary>
        /// Determines the operator's reading during constructing an expression string.
        /// </summary>
        /// <returns></returns>
        public abstract string Read();
    }
}

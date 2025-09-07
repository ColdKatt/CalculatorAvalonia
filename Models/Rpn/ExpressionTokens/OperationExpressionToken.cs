using CalculatorAvalonia.Models.Rpn.ExpressionTokens.Extensions;
using CalculatorAvalonia.Models.Rpn.Operations;
using System;
using System.Collections.Generic;

namespace CalculatorAvalonia.Models.Rpn.ExpressionTokens
{
    public sealed class OperationExpressionToken : ExpressionTokenBase
    {
        public readonly Func<double, double, double> Operation;

        public OperationType OperationType { get => _operationType; }
        public Associativity Associativity { get => _associativity; }

        private OperationType _operationType;
        private Associativity _associativity;

        private string _tokenReading;

        public OperationExpressionToken(OperationType operationType) : base()
        {
            _operationType = operationType;

            _priority = this.GetTokenPriority();
            _associativity = this.GetAssociativity();
            _tokenReading = this.GetOperationReading();
            Operation = this.GetOperation();
        }

        public override void Handle(Queue<ExpressionTokenBase> outputQueue, Stack<ExpressionTokenBase> operationStack)
        {
            if (operationStack.Count == 0) // if empty opStack -> opStack
            {
                operationStack.Push(this);
                return;
            }

            // operator with left associativity have to be less or equal with top stack operator priority to replace the current one
            // operator with right associativity have to be only less
            if (Associativity == Associativity.Left && Priority <= operationStack.Peek().Priority
                || Associativity == Associativity.Right && Priority < operationStack.Peek().Priority)
            {
                var allow = true;
                while (allow)
                {
                    outputQueue.Enqueue(operationStack.Pop());

                    if (operationStack.Count == 0 || Priority > operationStack.Peek().Priority)
                    {
                        allow = false;
                    }
                }

                operationStack.Push(this);
            }
            else
            {
                operationStack.Push(this);
                return;
            }
        }

        public override string Read() => _tokenReading;
    }
}

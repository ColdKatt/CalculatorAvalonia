using System;
using System.Collections.Generic;
using CalculatorAvalonia.Models.Rpn.Operations;

namespace CalculatorAvalonia.Models.Rpn.ExpressionTokens
{
    public sealed class BracketExpressionToken : ExpressionTokenBase
    {
        public BracketType BracketType { get => _bracketType; }

        private BracketType _bracketType;

        public BracketExpressionToken(BracketType bracketType)
        {
            _bracketType = bracketType;
        }

        public override void Handle(Queue<ExpressionTokenBase> outputQueue, Stack<ExpressionTokenBase> operationStack)
        {
            if (BracketType == BracketType.Open)
            {
                operationStack.Push(this);
            }

            if (BracketType == BracketType.Close)
            {
                while (operationStack.Count > 0)
                {
                    var currentToken = operationStack.Pop();
                    if (currentToken is OperationExpressionToken)
                    {
                        outputQueue.Enqueue(currentToken);
                    }

                    if (currentToken is BracketExpressionToken otherBracketToken
                        && otherBracketToken.BracketType == BracketType.Open) break;
                }
            }
        }

        public override string Read()
        {
            switch (BracketType)
            {
                case BracketType.Open:
                    return "(";
                case BracketType.Close:
                    return ")";
                default:
                    throw new NotImplementedException();
            }
        }
    }
}

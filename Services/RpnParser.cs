using CalculatorAvalonia.Models.Rpn.ExpressionTokens;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CalculatorAvalonia.Services
{
    public static class RpnParser
    {
        public static bool TryParse(this List<ExpressionTokenBase> tokens, out List<ExpressionTokenBase> rpnParsedExpressionTokens)
        {
            var outputQueue = new Queue<ExpressionTokenBase>();
            var operationStack = new Stack<ExpressionTokenBase>();

            foreach (var token in tokens)
            {
                token.Handle(outputQueue, operationStack);
            }

            while (operationStack.Count > 0)
            {
                outputQueue.Enqueue(operationStack.Pop());
            }

            rpnParsedExpressionTokens = outputQueue.ToList();

            return true;
        }

        public static bool TryEvaluateRpn(this List<ExpressionTokenBase> rpnParsedExpressionTokens, out double result)
        {
            var stack = new Stack<ExpressionTokenBase>();

            result = 0;

            foreach (var token in rpnParsedExpressionTokens)
            {
                if (token is NumberExpressionToken)
                {
                    stack.Push(token);
                }

                if (token is OperationExpressionToken operationToken) // BINARY OPERATION
                {
                    if (!stack.TryPop(out var firstNumber) || !stack.TryPop(out var secondNumber))
                    {
                        return false;
                    }

                    if (firstNumber is not NumberExpressionToken firstNumberToken || secondNumber is not NumberExpressionToken secondNumberToken)
                    {
                        return false;
                    }

                    var operationResult = new NumberExpressionToken(operationToken.Operation(secondNumberToken.Value, firstNumberToken.Value));
                    stack.Push(operationResult);
                }
            }

            // expected 1 token left in stack and its type == Number
            var leftToken = stack.Pop() as NumberExpressionToken ?? throw new ArgumentException();
            result = leftToken.Value;
            return true;
        }

        public static string ReadExpression(this List<ExpressionTokenBase> rpnParsedExpressionTokens)
        {
            var expression = "";

            foreach (var token in rpnParsedExpressionTokens)
            {
                expression += token.Read() + " ";
            }

            return expression.Trim();
        }
    }
}

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

        public static bool TryEvaluateRpn(this List<ExpressionTokenBase> rpnParsedExpressionTokens, out double result, out string message)
        {
            var stack = new Stack<ExpressionTokenBase>();

            result = 0;
            message = "";

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
                        message = "Invalid input!";
                        return false;
                    }

                    if (firstNumber is not NumberExpressionToken firstNumberToken || secondNumber is not NumberExpressionToken secondNumberToken)
                    {
                        message = "Invalid input!";
                        return false;
                    }

                    if (operationToken.OperationType == Models.Rpn.Operations.OperationType.Divide && firstNumberToken.Value == 0)
                    {
                        message = "Can't divide to zero!";
                        return false;
                    }

                    var operationResult = new NumberExpressionToken(operationToken.Operation(secondNumberToken.Value, firstNumberToken.Value));
                    stack.Push(operationResult);
                }
            }

            // expected 1 token left in stack and its type == Number
            if (stack.Count != 1 || stack.Pop() is not NumberExpressionToken leftToken)
            {
                message = "Invalid input!";
                return false;
            }

            result = leftToken.Value;
            message = "Success!";
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

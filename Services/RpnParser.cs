using CalculatorAvalonia.Models.Rpn.ExpressionTokens;
using System.Collections.Generic;
using System.Linq;
using Tmds.DBus.Protocol;

namespace CalculatorAvalonia.Services
{
    /// <summary>
    /// Basic parser class for expression reduction to RPN-notation.
    /// </summary>
    public static class RpnParser
    {
        /// <summary>
        /// Parsing list of tokens to list of tokens with RPN-notation.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="rpnParsedExpressionTokens"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Trying get result from parsed token list to RPN-notation.
        /// </summary>
        /// <param name="rpnParsedExpressionTokens"></param>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <returns></returns>
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

                if (token is OperationExpressionToken operationToken)
                {
                    if (!TryPopNumberToken(stack, out var firstNumberToken))
                    {
                        message = "Invalid input!";
                        return false;
                    }

                    if (operationToken.Operands == Models.Rpn.Operations.Operands.Binary)
                    {
                        if (!TryPopNumberToken(stack, out var secondNumberToken))
                        {
                            message = "Invalid input!";
                            return false;
                        }

                        // checking divide to zero
                        if (operationToken.OperationType == Models.Rpn.Operations.OperationType.Divide && firstNumberToken.Value == 0)
                        {
                            message = "Can't divide to zero!";
                            return false;
                        }

                        var operationResult = new NumberExpressionToken(operationToken.Operation([secondNumberToken.Value, firstNumberToken.Value]));
                        stack.Push(operationResult);
                    }
                    else
                    {
                        var operationResult = new NumberExpressionToken(operationToken.Operation([firstNumberToken.Value]));
                        stack.Push(operationResult);
                    }

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

        /// <summary>
        /// Reads full expression from list of tokens.
        /// </summary>
        /// <param name="rpnParsedExpressionTokens"></param>
        /// <returns></returns>
        public static string ReadExpression(this List<ExpressionTokenBase> rpnParsedExpressionTokens)
        {
            var expression = "";

            foreach (var token in rpnParsedExpressionTokens)
            {
                expression += token.Read() + " ";
            }

            return expression.Trim();
        }

        /// <summary>
        /// Determines possibility to take a number from the stack.
        /// </summary>
        /// <param name="stack"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private static bool TryPopNumberToken(Stack<ExpressionTokenBase> stack, out NumberExpressionToken token)
        {
            token = new(0);

            if (!stack.TryPop(out var number))
            {
                return false;
            }

            if (number is not NumberExpressionToken numberToken)
            {
                return false;
            }

            token = numberToken;
            return true;
        }
    }
}

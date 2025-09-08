using CalculatorAvalonia.Models.Rpn.Operations;
using System;

namespace CalculatorAvalonia.Models.Rpn.ExpressionTokens.Extensions
{
    /// <summary>
    /// Class-extension for expression tokens
    /// </summary>
    public static class ExpressionTokenExtensions
    {
        /// <summary>
        /// Gives priority to operation token.
        /// </summary>
        /// <param name="operationExpressionToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static int GetTokenPriority(this OperationExpressionToken operationExpressionToken)
        {
            switch (operationExpressionToken.OperationType)
            {
                case OperationType.Add:
                    return 2;
                case OperationType.Subtract:
                    return 2;
                case OperationType.Multiply:
                    return 4;
                case OperationType.Divide:
                    return 4;
                case OperationType.Factorial:
                    return 5;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gives instructions to operation token.
        /// </summary>
        /// <param name="operationExpressionToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static Func<double[], double> GetOperation(this OperationExpressionToken operationExpressionToken)
        {
            switch (operationExpressionToken.OperationType)
            {
                case OperationType.Add:
                    return (a) => a[0] + a[1];
                case OperationType.Subtract:
                    return (a) => a[0] - a[1];
                case OperationType.Multiply:
                    return (a) => a[0] * a[1];
                case OperationType.Divide:
                    return (a) => a[0] / a[1];
                case OperationType.Factorial:
                    return (a) => 
                    {
                        var res = 1d;
                        while (a[0] >= 1)
                        {
                            res *= a[0];
                            a[0]--;
                        }
                        return res;
                    };
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Determines operands count to operation types.
        /// </summary>
        /// <param name="operationExpressionToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static Operands GetOperands(this OperationExpressionToken operationExpressionToken)
        {
            switch (operationExpressionToken.OperationType)
            {
                case OperationType.Add:
                    return Operands.Binary;
                case OperationType.Subtract:
                    return Operands.Binary;
                case OperationType.Multiply:
                    return Operands.Binary;
                case OperationType.Divide:
                    return Operands.Binary;
                case OperationType.Factorial:
                    return Operands.Unary;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Determines associativity to operation types.
        /// </summary>
        /// <param name="operationExpressionToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static Associativity GetAssociativity(this OperationExpressionToken operationExpressionToken)
        {
            switch (operationExpressionToken.OperationType)
            {
                case OperationType.Add:
                    return Associativity.Left;
                case OperationType.Subtract:
                    return Associativity.Left;
                case OperationType.Multiply:
                    return Associativity.Left;
                case OperationType.Divide:
                    return Associativity.Left;
                case OperationType.Factorial:
                    return Associativity.Right;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Determines reading for operation types.
        /// </summary>
        /// <param name="operationExpressionToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static string GetOperationReading(this OperationExpressionToken operationExpressionToken)
        {
            switch (operationExpressionToken.OperationType)
            {
                case OperationType.Add:
                    return "+";
                case OperationType.Subtract:
                    return "-";
                case OperationType.Multiply:
                    return "*";
                case OperationType.Divide:
                    return "/";
                case OperationType.Factorial:
                    return "!";
                default:
                    throw new NotImplementedException();
            }
        }
    }
}

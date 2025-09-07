using CalculatorAvalonia.Models.Rpn.Operations;
using System;

namespace CalculatorAvalonia.Models.Rpn.ExpressionTokens.Extensions
{
    public static class ExpressionTokenExtensions
    {
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
                default:
                    throw new NotImplementedException();
            }
        }

        public static Func<double, double, double> GetOperation(this OperationExpressionToken operationExpressionToken)
        {
            switch (operationExpressionToken.OperationType)
            {
                case OperationType.Add:
                    return (a, b) => a + b;
                case OperationType.Subtract:
                    return (a, b) => a - b;
                case OperationType.Multiply:
                    return (a, b) => a * b;
                case OperationType.Divide:
                    return (a, b) => a / b;
                default:
                    throw new NotImplementedException();
            }
        }

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
                default:
                    throw new NotImplementedException();
            }
        }

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
                default:
                    throw new NotImplementedException();
            }
        }
    }
}

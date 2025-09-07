using CalculatorAvalonia.Models.Rpn.ExpressionTokens;
using CalculatorAvalonia.Models.Rpn.Operations;
using CalculatorAvalonia.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;

namespace CalculatorAvalonia.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _result;

    [ObservableProperty]
    private string _expression;

    private List<ExpressionTokenBase> _tokens;

    private bool _isResultShown;
    private bool _isNewNumber;

    public MainWindowViewModel()
    {
        _tokens ??= new();
        Result = "0";
        Expression = "";

        _isNewNumber = true;
    }

    [RelayCommand]
    private void AddDigit(string digit)
    {
        if (_isNewNumber)
        {
            Result = "0";
            _isNewNumber = false;
        }
        Result = Result == "0" ? Result = Result.Replace("0", digit) : Result + digit;
    }

    [RelayCommand]
    private void RemoveDigit()
    {
        if (_isNewNumber)
        {
            Result = "0";
            _isNewNumber = false;
        }
        Result = Result.Length == 1 ? "0" : Result.Remove(Result.Length - 1);
    }

    [RelayCommand]
    private void SetOperation(OperationType operationType)
    {
        _isResultShown = false;
        if (!_isNewNumber)
        {
            GetNumber();
        }

        if (_tokens.Count == 0 || (_tokens[^1] is not NumberExpressionToken && _tokens[^1] is not BracketExpressionToken))
        {
            _tokens.Add(new NumberExpressionToken(0));
            _tokens.Add(new OperationExpressionToken(operationType));
        }
        else
        {
            var lastToken = _tokens[^1];
            if (lastToken is OperationExpressionToken)
            {
                _tokens[^1] = new OperationExpressionToken(operationType);
            }
            else
            {
                _tokens.Add(new OperationExpressionToken(operationType));
            }
        }


        Expression = _tokens.ReadExpression();
    }

    [RelayCommand]
    private void AddPoint()
    {
        if (Result.Contains(',')) return;

        Result += ",";
    }

    [RelayCommand]
    private void GetEqual()
    {
        if (!_isNewNumber)
        {
            GetNumber();
        }
        _tokens.ReadExpression();
        _tokens.TryParse(out var parsed);

        Result = !parsed.TryEvaluateRpn(out var result, out var message) ? message : result.ToString("0.###########");

        if (!_isResultShown)
        {
            Expression += " =";
        }

        _isResultShown = true;

    }

    [RelayCommand]
    private void Clear()
    {
        _tokens.Clear();
        Result = "0";
        Expression = "";

        _isResultShown = false;
    }

    [RelayCommand]
    private void TurnSign()
    {
        if (Result == "0") return;
        Result = Result[0] == '-' ? Result.TrimStart('-') : Result.Insert(0, "-");
    }

    [RelayCommand]
    private void SetBracket(BracketType bracketType)
    {
        if (!_isNewNumber)
        {
            GetNumber();
        }

        if (_tokens.Count > 0 && bracketType == BracketType.Open)
        {
            var lastToken = _tokens[^1];
            if (lastToken is NumberExpressionToken || (lastToken is BracketExpressionToken br && br.BracketType == BracketType.Close))
            {
                _tokens.Add(new OperationExpressionToken(OperationType.Multiply));
            }
        }

        _tokens.Add(new BracketExpressionToken(bracketType));

        Expression = _tokens.ReadExpression();
    }

    private void GetNumber()
    {
        if (!_isResultShown)
        {
            if (!double.TryParse(Result, out var number))
            {
                Result = "Invalid input!";
                return;
            }

            _tokens.Add(new NumberExpressionToken(number));
        }

        Expression = _tokens.ReadExpression();

        Result = "0";
        _isResultShown = false;
        _isNewNumber = true;
    }
}

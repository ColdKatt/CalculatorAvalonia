using CalculatorAvalonia.Models.Rpn.ExpressionTokens;
using CalculatorAvalonia.Models.Rpn.Operations;
using CalculatorAvalonia.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;

namespace CalculatorAvalonia.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, IDisposable
{
    public NumpadViewModel NumpadViewModel { get; }

    [ObservableProperty]
    private string _result;

    [ObservableProperty]
    private string _expression;

    private List<ExpressionTokenBase> _tokens;

    private bool _isResultShown;

    public MainWindowViewModel()
    {
        NumpadViewModel ??= new();
        NumpadViewModel.OnNumberChanged += () => Result = NumpadViewModel.Number;

        _tokens ??= new();
        Result = "0";
        Expression = "";
    }

    [RelayCommand]
    private void SetOperation(OperationType operationType)
    {
        var numStr = NumpadViewModel.EndWrite();
        _isResultShown = false;
        
        PutNumberIntoToken(numStr);

        if (_tokens.Count == 0)
        {
            _tokens.Add(new NumberExpressionToken(0));
            _tokens.Add(new OperationExpressionToken(operationType));
        }
        else if (_tokens[^1] is not NumberExpressionToken && _tokens[^1] is not BracketExpressionToken)
        {
            _tokens[^1] = new OperationExpressionToken(operationType);
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
    private void GetEqual()
    {
        var numStr = NumpadViewModel.EndWrite();
        PutNumberIntoToken(numStr);
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
        NumpadViewModel.ClearInput();

        Expression = "";

        _isResultShown = false;
    }

    [RelayCommand]
    private void SetBracket(BracketType bracketType)
    {
        var numStr = NumpadViewModel.EndWrite();
        PutNumberIntoToken(numStr);

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

    private void PutNumberIntoToken(string numStr)
    {
        if (string.IsNullOrEmpty(numStr)) return;
        if (!_isResultShown)
        {
            if (!double.TryParse(numStr, out var number))
            {
                Result = "Invalid input!";
                return;
            }

            _tokens.Add(new NumberExpressionToken(number));
        }

        Expression = _tokens.ReadExpression();

        _isResultShown = false;
    }

    public void Dispose()
    {
        NumpadViewModel.OnNumberChanged -= () => Result = NumpadViewModel.Number;
    }
}

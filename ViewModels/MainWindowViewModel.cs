using Avalonia.Controls;
using CalculatorAvalonia.Models.ExpressionHistory;
using CalculatorAvalonia.Models.Rpn.ExpressionTokens;
using CalculatorAvalonia.Models.Rpn.Operations;
using CalculatorAvalonia.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CalculatorAvalonia.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, IDisposable
{
    public NumpadViewModel NumpadViewModel { get; }
    public HistoryPanelViewModel HistoryPanelViewModel { get; }

    [ObservableProperty]
    private string _result;

    [ObservableProperty]
    private string _expression;

    private List<ExpressionTokenBase> _tokens;

    private bool _isResultShown;

    public MainWindowViewModel(HistoryPanelViewModel historyPanelViewModel)
    {
        HistoryPanelViewModel = historyPanelViewModel;
        NumpadViewModel ??= new();

        _tokens ??= new();

        Result = "0";
        Expression = "";

        NumpadViewModel.OnNumberChanged += () => Result = NumpadViewModel.Number;
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
        else if (_tokens[^1] is OperationExpressionToken opToken)
        {
            if (opToken.Associativity != Associativity.Right)
            {
                _tokens[^1] = new OperationExpressionToken(operationType);
            }
            else
            {
                _tokens.Add(new OperationExpressionToken(operationType));
            }
        }
        else
        {
            _tokens.Add(new OperationExpressionToken(operationType));
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

        var isEvaluateSuccess = parsed.TryEvaluateRpn(out var result, out var message);

        if (isEvaluateSuccess)
        {
            Result = result.ToString("0.###########");

            var newHistoryItem = new ExpressionHistoryItem(_tokens, Result);
            HistoryPanelViewModel.HistoryService.Add(newHistoryItem);
        }
        else
        {
            Result = message;
        }

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

    [RelayCommand]
    private void RestoreExpression(ExpressionHistoryItem item)
    {
        if (item == null)
        {
            return;
        }

        _tokens = new(item.Tokens);
        Expression = item.Expression;
        Result = item.Result;

        _isResultShown = true;
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

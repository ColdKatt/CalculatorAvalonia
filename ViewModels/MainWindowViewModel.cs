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

    private List<ExpressionTokenBase> _tokens;

    private bool _isResultShown;

    public MainWindowViewModel()
    {
        _tokens ??= new();
        Result = "0";
    }

    [RelayCommand]
    private void AddDigit(string digit)
    {
        Result = Result == "0" ? Result = Result.Replace("0", digit) : Result + digit;
    }

    [RelayCommand]
    private void RemoveDigit()
    {
        Result = Result.Length == 1 ? "0" : Result.Remove(Result.Length - 1);
    }

    [RelayCommand]
    private void SetOperation(OperationType operationType)
    {
        GetNumber();
        _tokens.Add(new OperationExpressionToken(operationType));
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
        GetNumber();
        _tokens.ReadExpression();
        _tokens.TryParse(out var parsed);
        parsed.TryEvaluateRpn(out var result);
        Result = result.ToString();

        _isResultShown = true;
    }

    [RelayCommand]
    private void Clear()
    {
        _tokens.Clear();
        Result = "0";

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
        GetNumber();

        _tokens.Add(new BracketExpressionToken(bracketType));
    }

    private void GetNumber()
    {
        if (!_isResultShown)
        {
            if (!double.TryParse(Result, out var number))
            {
                Result = "Error";
                return;
            }

            _tokens.Add(new NumberExpressionToken(number));
        }

        Result = "0";
        _isResultShown = false;
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace CalculatorAvalonia.ViewModels
{
    public partial class NumpadViewModel : ViewModelBase
    {
        public event Action? OnNumberChanged;

        public string Number
        {
            get => _number;
            protected set
            {
                _number = value;
                OnNumberChanged?.Invoke();
            }
        }

        private string _number;

        private bool _isWriting;

        public NumpadViewModel()
        {
            _number = "0";
            _isWriting = false;
        }

        public string EndWrite()
        {
            if (!_isWriting)
            {
                return string.Empty;
            }
            _isWriting = false;
            return Number;
        }
        
        public void ClearInput()
        {
            EndWrite();
            Number = "0";
        }

        [RelayCommand]
        private void AddDigit(string digit)
        {
            if (!_isWriting)
            {
                Number = "0";
                _isWriting = true;
            }
            Number = Number == "0" ? Number.Replace("0", digit) : Number + digit;
        }

        [RelayCommand]
        private void RemoveDigit()
        {
            if (!_isWriting)
            {
                Number = "0";
                _isWriting = true;
            }
            Number = Number.Length == 1 ? "0" : Number.Remove(Number.Length - 1);
        }

        [RelayCommand]
        private void AddDot()
        {
            if (Number.Contains(',')) return;

            Number += ",";
        }

        [RelayCommand]
        private void ReverseSign()
        {
            if (Number == "0") return;
            Number = Number[0] == '-' ? Number.TrimStart('-') : Number.Insert(0, "-");
        }
    }
}

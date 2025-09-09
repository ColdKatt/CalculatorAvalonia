using CalculatorAvalonia.Models.ExpressionHistory;
using System;

namespace CalculatorAvalonia.Services.ExpressionHistory
{
    public interface IExpressionHistoryService
    {
        public event Action<ExpressionHistoryItem>? OnItemAdded;

        public event Action<ExpressionHistoryItem>? OnItemRemoved;
        public event Action? OnHistoryClear;

        public void Add(ExpressionHistoryItem item);
        public void Clear();

        public void Save(string path);
        public void Load(string path);
    }
}
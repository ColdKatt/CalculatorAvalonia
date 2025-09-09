using System;
using System.Collections.Generic;
using CalculatorAvalonia.Models.ExpressionHistory;

namespace CalculatorAvalonia.Services.ExpressionHistory
{
    public class ExpressionHistory : IExpressionHistoryService
    {
        public event Action<ExpressionHistoryItem>? OnItemAdded;
        public event Action<ExpressionHistoryItem>? OnItemRemoved;
        public event Action? OnHistoryClear;

        private const int QUEUE_CAPACITY = 15;

        private readonly Queue<ExpressionHistoryItem> _items;

        public ExpressionHistory()
        {
            _items ??= new();
        }

        public void Add(ExpressionHistoryItem item)
        {
            _items.Enqueue(item);
            if (_items.Count > QUEUE_CAPACITY)
            {
                var oldItem = _items.Dequeue();
                OnItemRemoved?.Invoke(oldItem);
            }

            OnItemAdded?.Invoke(item);
        }

        public void Clear()
        {
            _items.Clear();
            OnHistoryClear?.Invoke();
        }

        public void Save(string path) => XmlHistoryService.Write(_items, path);

        public void Load(string path)
        {
            if (!XmlHistoryService.Read(path, out var items))
            {
                throw new ArgumentException();
            }

            Clear();
            foreach (var item in items)
            {
                Add(item);
            }
        }
    }
}

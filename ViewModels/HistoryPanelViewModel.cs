using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CalculatorAvalonia.Models.ExpressionHistory;
using CalculatorAvalonia.Services.ExpressionHistory;
using CalculatorAvalonia.Services.FIlesService;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculatorAvalonia.ViewModels
{
    public partial class HistoryPanelViewModel : ViewModelBase, IDisposable
    {
        public readonly IExpressionHistoryService HistoryService;
        public IFilesService FilesService;

        public string filter = "2 *";

        [ObservableProperty]
        private bool _isPaneOpen;

        public ObservableCollection<ExpressionHistoryItem> History { get; set; }

        public HistoryPanelViewModel(IExpressionHistoryService service, IFilesService filesService) 
        {
            HistoryService = service;
            FilesService = filesService;

            History ??= new();

            HistoryService.OnItemAdded += (item) => History.Insert(0, item);
            HistoryService.OnItemRemoved += (item) => History.Remove(item);
            HistoryService.OnHistoryClear += () => History.Clear();
        }

        [RelayCommand]
        private void ClearHistory()
        {
            HistoryService.Clear();
        }

        [RelayCommand]
        private async Task ExportHistory()
        {
            var file = await FilesService.SaveFileAsync();

            if (file != null)
            {
                HistoryService.Save(file.Path.AbsolutePath);
            }
        }

        [RelayCommand]
        private async Task ImportHistory()
        {
            var file = await FilesService.OpenFileAsync();

            if (file != null)
            { 
                HistoryService.Load(file.Path.AbsolutePath);
            }
        }

        [RelayCommand]
        private void OpenPane()
        {
            IsPaneOpen = !IsPaneOpen;
        }

        public void Dispose()
        {
            HistoryService.OnItemAdded -= (item) => History.Insert(0, item);
            HistoryService.OnItemRemoved -= (item) => History.Remove(item);
            HistoryService.OnHistoryClear -= () => History.Clear();
        }
    }
}

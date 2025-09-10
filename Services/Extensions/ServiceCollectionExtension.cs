using CalculatorAvalonia.Services.ExpressionHistory;
using CalculatorAvalonia.Services.FIlesService;
using CalculatorAvalonia.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CalculatorAvalonia.Services.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection RegisterCommonDependencies(this IServiceCollection collection)
        {
            collection.AddSingleton<IExpressionHistoryService, ExpressionHistory.ExpressionHistory>()
                      .AddSingleton<IFilesService, FilesService>()
                      .AddSingleton<HistoryPanelViewModel>()
                      .AddSingleton<MainWindowViewModel>();

            return collection;
        }
    }
}

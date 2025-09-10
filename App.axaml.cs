using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using CalculatorAvalonia.Services.ExpressionHistory;
using CalculatorAvalonia.Services.Extensions;
using CalculatorAvalonia.Services.FIlesService;
using CalculatorAvalonia.ViewModels;
using CalculatorAvalonia.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace CalculatorAvalonia;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        BindingPlugins.DataValidators.RemoveAt(0);


        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var collection = new ServiceCollection();

            var services = collection.RegisterCommonDependencies()
                                     .BuildServiceProvider();

            var vm = services.GetRequiredService<MainWindowViewModel>();

            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow
            {
                DataContext = vm,
            };

        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}
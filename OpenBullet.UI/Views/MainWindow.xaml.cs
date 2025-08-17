using OpenBullet.UI.ViewModels;
using System.Windows;

namespace OpenBullet.UI.Views;

/// <summary>
/// Main window code-behind
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        
        // Initialize the view model
        Loaded += async (s, e) => await viewModel.InitializeAsync();
        
        // Cleanup on closing
        Closing += (s, e) => viewModel.Cleanup();
    }
}

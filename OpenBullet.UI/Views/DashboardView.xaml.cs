using OpenBullet.UI.ViewModels;
using System.Windows.Controls;

namespace OpenBullet.UI.Views;

/// <summary>
/// Dashboard view code-behind
/// </summary>
public partial class DashboardView : UserControl
{
    public DashboardView(DashboardViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        
        // Initialize the view model when loaded
        Loaded += async (s, e) => await viewModel.InitializeAsync();
    }
}

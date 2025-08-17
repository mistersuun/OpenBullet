using OpenBullet.UI.ViewModels;
using System.Windows.Controls;

namespace OpenBullet.UI.Views;

/// <summary>
/// Job detail view code-behind
/// </summary>
public partial class JobDetailView : UserControl
{
    public JobDetailView(JobDetailViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}

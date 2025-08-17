using System.Windows.Controls;
using OpenBullet.Core.Models;
using OpenBullet.UI.ViewModels;

namespace OpenBullet.UI.Views;

/// <summary>
/// Configuration detail view for editing LoliScript configurations
/// </summary>
public partial class ConfigurationDetailView : UserControl
{
    private ConfigurationDetailViewModel _viewModel;

    public ConfigurationDetailView(ConfigurationDetailViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
    }

    private void ScriptEditor_ScriptTextChanged(object sender, string newScript)
    {
        // Update the view model when script text changes
        if (_viewModel != null)
        {
            _viewModel.ConfigurationScript = newScript;
        }
    }


}

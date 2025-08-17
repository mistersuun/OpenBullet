using OpenBullet.UI.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace OpenBullet.UI.Services;

/// <summary>
/// Navigation service for handling view navigation
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Current view
    /// </summary>
    UserControl? CurrentView { get; }

    /// <summary>
    /// Navigate to a view by view model type
    /// </summary>
    void NavigateTo<T>() where T : ViewModelBase;

    /// <summary>
    /// Navigate to a view by view model type with parameter
    /// </summary>
    void NavigateTo<T>(object parameter) where T : ViewModelBase;

    /// <summary>
    /// Navigate to a view by view model instance
    /// </summary>
    void NavigateTo(ViewModelBase viewModel);

    /// <summary>
    /// Navigate back
    /// </summary>
    bool CanGoBack { get; }
    void GoBack();

    /// <summary>
    /// Navigate forward
    /// </summary>
    bool CanGoForward { get; }
    void GoForward();

    /// <summary>
    /// Clear navigation history
    /// </summary>
    void ClearHistory();

    /// <summary>
    /// Current view changed event
    /// </summary>
    event EventHandler<NavigationEventArgs>? CurrentViewChanged;
}

/// <summary>
/// Dialog service for showing dialogs and message boxes
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Show information message
    /// </summary>
    void ShowInformation(string message, string title = "Information");

    /// <summary>
    /// Show warning message
    /// </summary>
    void ShowWarning(string message, string title = "Warning");

    /// <summary>
    /// Show error message
    /// </summary>
    void ShowError(string message, string title = "Error");

    /// <summary>
    /// Show confirmation dialog
    /// </summary>
    bool ShowConfirmation(string message, string title = "Confirm");

    /// <summary>
    /// Show input dialog
    /// </summary>
    string? ShowInput(string message, string title = "Input", string defaultValue = "");

    /// <summary>
    /// Show open file dialog
    /// </summary>
    string? ShowOpenFileDialog(string filter = "All files (*.*)|*.*", string title = "Open File");

    /// <summary>
    /// Show save file dialog
    /// </summary>
    string? ShowSaveFileDialog(string filter = "All files (*.*)|*.*", string title = "Save File", string defaultFileName = "");

    /// <summary>
    /// Show open folder dialog
    /// </summary>
    string? ShowOpenFolderDialog(string title = "Select Folder");

    /// <summary>
    /// Show custom dialog
    /// </summary>
    Task<T?> ShowDialogAsync<T>(UserControl dialog) where T : class;

    /// <summary>
    /// Show progress dialog
    /// </summary>
    Task ShowProgressDialogAsync(string title, Func<IProgress<ProgressInfo>, CancellationToken, Task> work);
}

/// <summary>
/// Theme service for managing application themes
/// </summary>
public interface IThemeService
{
    /// <summary>
    /// Current theme
    /// </summary>
    string CurrentTheme { get; }

    /// <summary>
    /// Available themes
    /// </summary>
    IEnumerable<string> AvailableThemes { get; }

    /// <summary>
    /// Current primary color
    /// </summary>
    string CurrentPrimaryColor { get; }

    /// <summary>
    /// Available primary colors
    /// </summary>
    IEnumerable<string> AvailablePrimaryColors { get; }

    /// <summary>
    /// Apply theme
    /// </summary>
    void ApplyTheme(string theme, string primaryColor);

    /// <summary>
    /// Theme changed event
    /// </summary>
    event EventHandler<ThemeChangedEventArgs>? ThemeChanged;
}

/// <summary>
/// Notification service for showing notifications
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Show success notification
    /// </summary>
    void ShowSuccess(string message, string title = "Success");

    /// <summary>
    /// Show info notification
    /// </summary>
    void ShowInfo(string message, string title = "Information");

    /// <summary>
    /// Show warning notification
    /// </summary>
    void ShowWarning(string message, string title = "Warning");

    /// <summary>
    /// Show error notification
    /// </summary>
    void ShowError(string message, string title = "Error");

    /// <summary>
    /// Show custom notification
    /// </summary>
    void ShowNotification(NotificationInfo notification);

    /// <summary>
    /// Clear all notifications
    /// </summary>
    void ClearAll();
}

/// <summary>
/// File service for file operations
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Read text file
    /// </summary>
    Task<string> ReadTextFileAsync(string filePath);

    /// <summary>
    /// Write text file
    /// </summary>
    Task WriteTextFileAsync(string filePath, string content);

    /// <summary>
    /// Read JSON file
    /// </summary>
    Task<T?> ReadJsonFileAsync<T>(string filePath);

    /// <summary>
    /// Write JSON file
    /// </summary>
    Task WriteJsonFileAsync<T>(string filePath, T data);

    /// <summary>
    /// Check if file exists
    /// </summary>
    bool FileExists(string filePath);

    /// <summary>
    /// Check if directory exists
    /// </summary>
    bool DirectoryExists(string directoryPath);

    /// <summary>
    /// Create directory
    /// </summary>
    void CreateDirectory(string directoryPath);

    /// <summary>
    /// Get file extension
    /// </summary>
    string GetFileExtension(string filePath);

    /// <summary>
    /// Get file name without extension
    /// </summary>
    string GetFileNameWithoutExtension(string filePath);

    /// <summary>
    /// Combine paths
    /// </summary>
    string CombinePaths(params string[] paths);
}

/// <summary>
/// ViewModel factory for creating view models
/// </summary>
public interface IViewModelFactory
{
    /// <summary>
    /// Create view model
    /// </summary>
    T Create<T>() where T : ViewModelBase;

    /// <summary>
    /// Create view model with parameter
    /// </summary>
    T Create<T>(object parameter) where T : ViewModelBase;
}

/// <summary>
/// View factory for creating views
/// </summary>
public interface IViewFactory
{
    /// <summary>
    /// Create view for view model
    /// </summary>
    UserControl CreateView<T>() where T : ViewModelBase;

    /// <summary>
    /// Create view for view model type
    /// </summary>
    UserControl CreateView(Type viewModelType);
}

/// <summary>
/// Navigation event arguments
/// </summary>
public class NavigationEventArgs : EventArgs
{
    public UserControl? PreviousView { get; set; }
    public UserControl? CurrentView { get; set; }
    public ViewModelBase? ViewModel { get; set; }
}

/// <summary>
/// Theme changed event arguments
/// </summary>
public class ThemeChangedEventArgs : EventArgs
{
    public string PreviousTheme { get; set; } = string.Empty;
    public string CurrentTheme { get; set; } = string.Empty;
    public string PrimaryColor { get; set; } = string.Empty;
}

/// <summary>
/// Notification information
/// </summary>
public class NotificationInfo
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; } = NotificationType.Info;
    public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(5);
    public bool CanClose { get; set; } = true;
    public string? ActionText { get; set; }
    public Action? Action { get; set; }
}

/// <summary>
/// Notification type
/// </summary>
public enum NotificationType
{
    Info,
    Success,
    Warning,
    Error
}

/// <summary>
/// Progress information
/// </summary>
public class ProgressInfo
{
    public string Status { get; set; } = string.Empty;
    public double Percentage { get; set; }
    public bool IsIndeterminate { get; set; }
    public string? Detail { get; set; }
}

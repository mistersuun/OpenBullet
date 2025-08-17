using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using MaterialDesignColors;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using OpenFolderDialog = Microsoft.Win32.OpenFolderDialog;

namespace OpenBullet.UI.Services;

/// <summary>
/// Dialog service implementation
/// </summary>
public class DialogService : IDialogService
{
    public void ShowInformation(string message, string title = "Information")
    {
        System.Windows.MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public void ShowWarning(string message, string title = "Warning")
    {
        System.Windows.MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    public void ShowError(string message, string title = "Error")
    {
        System.Windows.MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public bool ShowConfirmation(string message, string title = "Confirm")
    {
        var result = System.Windows.MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
        return result == MessageBoxResult.Yes;
    }

    public string? ShowInput(string message, string title = "Input", string defaultValue = "")
    {
        var dialog = new InputDialog(message, title, defaultValue);
        return dialog.ShowDialog() == true ? dialog.InputText : null;
    }

    public string? ShowOpenFileDialog(string filter = "All files (*.*)|*.*", string title = "Open File")
    {
        var dialog = new OpenFileDialog
        {
            Filter = filter,
            Title = title,
            CheckFileExists = true,
            CheckPathExists = true
        };

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    public string? ShowSaveFileDialog(string filter = "All files (*.*)|*.*", string title = "Save File", string defaultFileName = "")
    {
        var dialog = new SaveFileDialog
        {
            Filter = filter,
            Title = title,
            FileName = defaultFileName,
            CheckPathExists = true
        };

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    public string? ShowOpenFolderDialog(string title = "Select Folder")
    {
        var dialog = new OpenFolderDialog
        {
            Title = title,
            Multiselect = false
        };

        return dialog.ShowDialog() == true ? dialog.FolderName : null;
    }

    public async Task<T?> ShowDialogAsync<T>(UserControl dialog) where T : class
    {
        var result = await DialogHost.Show(dialog);
        return result as T;
    }

    public async Task ShowProgressDialogAsync(string title, Func<IProgress<ProgressInfo>, CancellationToken, Task> work)
    {
        var progressDialog = new ProgressDialog(title);
        var cancellationTokenSource = new CancellationTokenSource();
        
        var progress = new Progress<ProgressInfo>(info =>
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                progressDialog.UpdateProgress(info);
            });
        });

        progressDialog.CancelRequested += () => cancellationTokenSource.Cancel();

        var dialogTask = DialogHost.Show(progressDialog);
        
        try
        {
            await work(progress, cancellationTokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            // User cancelled
        }
        finally
        {
            progressDialog.Close();
        }
    }
}

/// <summary>
/// Theme service implementation
/// </summary>
public class ThemeService : IThemeService
{
    private readonly PaletteHelper _paletteHelper = new();
    private string _currentTheme = "Dark";
    private string _currentPrimaryColor = "DeepPurple";

    public string CurrentTheme => _currentTheme;

    public IEnumerable<string> AvailableThemes => new[] { "Light", "Dark" };

    public string CurrentPrimaryColor => _currentPrimaryColor;

    public IEnumerable<string> AvailablePrimaryColors => new[]
    {
        "Red", "Pink", "Purple", "DeepPurple", "Indigo", "Blue", "LightBlue", 
        "Cyan", "Teal", "Green", "LightGreen", "Lime", "Yellow", "Amber", 
        "Orange", "DeepOrange", "Brown", "Grey", "BlueGrey"
    };

    public event EventHandler<ThemeChangedEventArgs>? ThemeChanged;

    public void ApplyTheme(string theme, string primaryColor)
    {
        try
        {
            var previousTheme = _currentTheme;
            
            var isDark = theme.Equals("Dark", StringComparison.OrdinalIgnoreCase);
            var baseTheme = isDark ? BaseTheme.Dark : BaseTheme.Light;
            
            // Simple theme tracking for now 
            // TODO: Implement full MaterialDesign theming later
            _currentTheme = theme;
            _currentPrimaryColor = primaryColor;
            
            ThemeChanged?.Invoke(this, new ThemeChangedEventArgs
            {
                PreviousTheme = previousTheme,
                CurrentTheme = _currentTheme,
                PrimaryColor = _currentPrimaryColor
            });
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to apply theme: {ex.Message}", ex);
        }
    }
}

/// <summary>
/// Notification service implementation
/// </summary>
public class NotificationService : INotificationService
{
    private readonly List<NotificationInfo> _notifications = new();

    public void ShowSuccess(string message, string title = "Success")
    {
        ShowNotification(new NotificationInfo
        {
            Title = title,
            Message = message,
            Type = NotificationType.Success
        });
    }

    public void ShowInfo(string message, string title = "Information")
    {
        ShowNotification(new NotificationInfo
        {
            Title = title,
            Message = message,
            Type = NotificationType.Info
        });
    }

    public void ShowWarning(string message, string title = "Warning")
    {
        ShowNotification(new NotificationInfo
        {
            Title = title,
            Message = message,
            Type = NotificationType.Warning
        });
    }

    public void ShowError(string message, string title = "Error")
    {
        ShowNotification(new NotificationInfo
        {
            Title = title,
            Message = message,
            Type = NotificationType.Error,
            Duration = TimeSpan.FromSeconds(10) // Errors stay longer
        });
    }

    public void ShowNotification(NotificationInfo notification)
    {
        _notifications.Add(notification);
        
        // Create and show notification UI element
        var notificationView = new NotificationView(notification);
        
        // Auto-remove after duration
        if (notification.Duration > TimeSpan.Zero)
        {
            Task.Delay(notification.Duration).ContinueWith(_ =>
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    _notifications.Remove(notification);
                    notificationView.Hide();
                });
            });
        }
    }

    public void ClearAll()
    {
        _notifications.Clear();
    }
}

/// <summary>
/// File service implementation
/// </summary>
public class FileService : IFileService
{
    public async Task<string> ReadTextFileAsync(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}");

        return await File.ReadAllTextAsync(filePath);
    }

    public async Task WriteTextFileAsync(string filePath, string content)
    {
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await File.WriteAllTextAsync(filePath, content);
    }

    public async Task<T?> ReadJsonFileAsync<T>(string filePath)
    {
        if (!File.Exists(filePath))
            return default;

        var json = await ReadTextFileAsync(filePath);
        return JsonConvert.DeserializeObject<T>(json);
    }

    public async Task WriteJsonFileAsync<T>(string filePath, T data)
    {
        var json = JsonConvert.SerializeObject(data, Formatting.Indented);
        await WriteTextFileAsync(filePath, json);
    }

    public bool FileExists(string filePath) => File.Exists(filePath);

    public bool DirectoryExists(string directoryPath) => Directory.Exists(directoryPath);

    public void CreateDirectory(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }

    public string GetFileExtension(string filePath) => Path.GetExtension(filePath);

    public string GetFileNameWithoutExtension(string filePath) => Path.GetFileNameWithoutExtension(filePath);

    public string CombinePaths(params string[] paths) => Path.Combine(paths);
}

/// <summary>
/// Simple input dialog - Code-only implementation
/// </summary>
public class InputDialog : Window
{
    public string InputText { get; private set; } = string.Empty;
    private TextBox inputTextBox = new();

    public InputDialog(string message, string title, string defaultValue = "")
    {
        Title = title;
        Width = 400;
        Height = 200;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        
        var stack = new StackPanel { Margin = new Thickness(20) };
        stack.Children.Add(new TextBlock { Text = message, Margin = new Thickness(0, 0, 0, 10) });
        
        inputTextBox.Text = defaultValue;
        inputTextBox.Margin = new Thickness(0, 0, 0, 20);
        stack.Children.Add(inputTextBox);
        
        var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
        var okButton = new Button { Content = "OK", Width = 75, Margin = new Thickness(0, 0, 10, 0) };
        var cancelButton = new Button { Content = "Cancel", Width = 75 };
        
        okButton.Click += (s, e) => { InputText = inputTextBox.Text; DialogResult = true; };
        cancelButton.Click += (s, e) => DialogResult = false;
        
        buttonPanel.Children.Add(okButton);
        buttonPanel.Children.Add(cancelButton);
        stack.Children.Add(buttonPanel);
        
        Content = stack;
        inputTextBox.Focus();
        inputTextBox.SelectAll();
    }
}

/// <summary>
/// Progress dialog
/// </summary>
public class ProgressDialog : UserControl
{
    public event Action? CancelRequested;
    private TextBlock titleTextBlock = new();
    private TextBlock statusTextBlock = new();
    private ProgressBar progressBar = new();
    private TextBlock detailTextBlock = new();

    public ProgressDialog(string title)
    {
        var stack = new StackPanel { Margin = new Thickness(20) };
        
        titleTextBlock.Text = title;
        titleTextBlock.FontWeight = FontWeights.Bold;
        titleTextBlock.Margin = new Thickness(0, 0, 0, 15);
        stack.Children.Add(titleTextBlock);
        
        statusTextBlock.Margin = new Thickness(0, 0, 0, 10);
        stack.Children.Add(statusTextBlock);
        
        progressBar.Height = 20;
        progressBar.Margin = new Thickness(0, 0, 0, 10);
        stack.Children.Add(progressBar);
        
        detailTextBlock.Visibility = Visibility.Collapsed;
        detailTextBlock.Margin = new Thickness(0, 0, 0, 15);
        stack.Children.Add(detailTextBlock);
        
        var cancelButton = new Button { Content = "Cancel", Width = 75, HorizontalAlignment = HorizontalAlignment.Right };
        cancelButton.Click += (s, e) => CancelRequested?.Invoke();
        stack.Children.Add(cancelButton);
        
        Content = stack;
    }

    public void UpdateProgress(ProgressInfo info)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            statusTextBlock.Text = info.Status;
            
            if (info.IsIndeterminate)
            {
                progressBar.IsIndeterminate = true;
            }
            else
            {
                progressBar.IsIndeterminate = false;
                progressBar.Value = info.Percentage;
            }
            
            if (!string.IsNullOrEmpty(info.Detail))
            {
                detailTextBlock.Text = info.Detail;
                detailTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                detailTextBlock.Visibility = Visibility.Collapsed;
            }
        });
    }

    public void Close()
    {
        // Simple close implementation
        if (Parent is Panel panel)
            panel.Children.Remove(this);
    }
}

/// <summary>
/// Notification view - Code-only implementation
/// </summary>
public class NotificationView : UserControl
{
    private readonly NotificationInfo _notification;

    public NotificationView(NotificationInfo notification)
    {
        _notification = notification;
        
        var border = new Border 
        { 
            CornerRadius = new CornerRadius(5),
            Padding = new Thickness(15),
            Margin = new Thickness(5)
        };
        
        var stack = new StackPanel();
        var messageText = new TextBlock 
        { 
            Text = notification.Message, 
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 0, 0, 10)
        };
        stack.Children.Add(messageText);
        
        var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
        
        if (notification.Action != null)
        {
            var actionButton = new Button { Content = "Action", Margin = new Thickness(0, 0, 5, 0) };
            actionButton.Click += (s, e) => { notification.Action.Invoke(); Hide(); };
            buttonPanel.Children.Add(actionButton);
        }
        
        var closeButton = new Button { Content = "×", Width = 25, Height = 25 };
        closeButton.Click += (s, e) => Hide();
        buttonPanel.Children.Add(closeButton);
        
        stack.Children.Add(buttonPanel);
        border.Child = stack;
        Content = border;
        
        // Set colors based on type
        UpdateAppearance(border);
    }

    private void UpdateAppearance(Border border)
    {
        var color = _notification.Type switch
        {
            NotificationType.Success => Colors.LightGreen,
            NotificationType.Warning => Colors.Orange,
            NotificationType.Error => Colors.LightCoral,
            _ => Colors.LightBlue
        };
        
        border.Background = new SolidColorBrush(color);
    }

    public void Hide()
    {
        Visibility = Visibility.Collapsed;
    }
}

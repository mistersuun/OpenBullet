using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using OpenBullet.UI.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OpenBullet.UI.ViewModels;

/// <summary>
/// Base class for all view models with common functionality
/// </summary>
public abstract partial class ViewModelBase : ObservableObject, INotifyPropertyChanged
{
    protected readonly ILogger _logger;
    protected readonly IDialogService _dialogService;
    protected readonly INotificationService _notificationService;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _busyMessage = string.Empty;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private bool _hasErrors;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    protected ViewModelBase(
        ILogger logger,
        IDialogService dialogService,
        INotificationService notificationService)
    {
        _logger = logger;
        _dialogService = dialogService;
        _notificationService = notificationService;
    }

    /// <summary>
    /// Execute an async operation with busy indication and error handling
    /// </summary>
    protected async Task ExecuteAsync(Func<Task> operation, string busyMessage = "Please wait...")
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            BusyMessage = busyMessage;
            HasErrors = false;
            ErrorMessage = string.Empty;

            await operation();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing operation: {Message}", ex.Message);
            HandleError(ex);
        }
        finally
        {
            IsBusy = false;
            BusyMessage = string.Empty;
        }
    }

    /// <summary>
    /// Execute an async operation with result and busy indication
    /// </summary>
    protected async Task<T?> ExecuteAsync<T>(Func<Task<T>> operation, string busyMessage = "Please wait...")
    {
        if (IsBusy) return default;

        try
        {
            IsBusy = true;
            BusyMessage = busyMessage;
            HasErrors = false;
            ErrorMessage = string.Empty;

            return await operation();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing operation: {Message}", ex.Message);
            HandleError(ex);
            return default;
        }
        finally
        {
            IsBusy = false;
            BusyMessage = string.Empty;
        }
    }

    /// <summary>
    /// Handle errors with user notification
    /// </summary>
    protected virtual void HandleError(Exception ex)
    {
        HasErrors = true;
        ErrorMessage = ex.Message;
        _notificationService.ShowError(ex.Message, "Error");
    }

    /// <summary>
    /// Show success notification
    /// </summary>
    protected void ShowSuccess(string message, string title = "Success")
    {
        _notificationService.ShowSuccess(message, title);
    }

    /// <summary>
    /// Show info notification
    /// </summary>
    protected void ShowInfo(string message, string title = "Information")
    {
        _notificationService.ShowInfo(message, title);
    }

    /// <summary>
    /// Show warning notification
    /// </summary>
    protected void ShowWarning(string message, string title = "Warning")
    {
        _notificationService.ShowWarning(message, title);
    }

    /// <summary>
    /// Show confirmation dialog
    /// </summary>
    protected bool ShowConfirmation(string message, string title = "Confirm")
    {
        return _dialogService.ShowConfirmation(message, title);
    }

    /// <summary>
    /// Validate required string property
    /// </summary>
    protected bool ValidateRequired(string? value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            SetError($"{propertyName} is required", propertyName);
            return false;
        }
        
        ClearError(propertyName);
        return true;
    }

    /// <summary>
    /// Set validation error
    /// </summary>
    protected void SetError(string error, [CallerMemberName] string? propertyName = null)
    {
        if (propertyName != null)
        {
            HasErrors = true;
            ErrorMessage = error;
            OnPropertyChanged(nameof(HasErrors));
            OnPropertyChanged(nameof(ErrorMessage));
        }
    }

    /// <summary>
    /// Clear validation error
    /// </summary>
    protected void ClearError([CallerMemberName] string? propertyName = null)
    {
        if (propertyName != null)
        {
            HasErrors = false;
            ErrorMessage = string.Empty;
            OnPropertyChanged(nameof(HasErrors));
            OnPropertyChanged(nameof(ErrorMessage));
        }
    }

    /// <summary>
    /// Create an async relay command with can execute
    /// </summary>
    protected AsyncRelayCommand CreateAsyncCommand(Func<Task> execute, Func<bool>? canExecute = null)
    {
        return canExecute == null 
            ? new AsyncRelayCommand(execute) 
            : new AsyncRelayCommand(execute, canExecute);
    }

    /// <summary>
    /// Create an async relay command with parameter
    /// </summary>
    protected AsyncRelayCommand<T> CreateAsyncCommand<T>(Func<T?, Task> execute, Func<T?, bool>? canExecute = null)
    {
        // For now, we'll skip the canExecute parameter since AsyncRelayCommand<T> constructor 
        // doesn't accept it directly in this version of CommunityToolkit.Mvvm
        // TODO: Implement proper canExecute support later
        return new AsyncRelayCommand<T>(execute);
    }

    /// <summary>
    /// Create a relay command
    /// </summary>
    protected RelayCommand CreateCommand(Action execute, Func<bool>? canExecute = null)
    {
        return canExecute == null 
            ? new RelayCommand(execute) 
            : new RelayCommand(execute, canExecute);
    }

    /// <summary>
    /// Create a relay command with parameter
    /// </summary>
    protected RelayCommand<T> CreateCommand<T>(Action<T?> execute, Func<T?, bool>? canExecute = null)
    {
        if (canExecute == null)
            return new RelayCommand<T>(execute);
        else
            return new RelayCommand<T>(execute, new Predicate<T?>(canExecute));
    }

    /// <summary>
    /// Refresh data - override in derived classes
    /// </summary>
    public virtual Task RefreshAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Initialize view model - override in derived classes
    /// </summary>
    public virtual Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Cleanup resources - override in derived classes
    /// </summary>
    public virtual void Cleanup()
    {
        // Override in derived classes
    }

    /// <summary>
    /// Show error message to user
    /// </summary>
    protected void ShowError(string message, string title = "Error")
    {
        _logger.LogError("{Message}", message);
        // For now, just log the error. In a real app, you'd show a dialog
        // This can be enhanced later to use IDialogService
        System.Diagnostics.Debug.WriteLine($"Error: {title} - {message}");
    }
}

/// <summary>
/// Base class for list view models with common list operations
/// </summary>
public abstract partial class ListViewModelBase<T> : ViewModelBase where T : class
{
    [ObservableProperty]
    private IList<T> _items = new List<T>();

    [ObservableProperty]
    private T? _selectedItem;

    [ObservableProperty]
    private IList<T> _selectedItems = new List<T>();

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private bool _hasSelection;

    [ObservableProperty]
    private int _totalCount;

    [ObservableProperty]
    private int _currentPage = 1;

    [ObservableProperty]
    private int _pageSize = 50;

    [ObservableProperty]
    private bool _hasNextPage;

    [ObservableProperty]
    private bool _hasPreviousPage;

    protected ListViewModelBase(
        ILogger logger,
        IDialogService dialogService,
        INotificationService notificationService)
        : base(logger, dialogService, notificationService)
    {
    }

    partial void OnSelectedItemChanged(T? value)
    {
        HasSelection = value != null;
        OnSelectionChanged();
    }

    partial void OnSelectedItemsChanged(IList<T> value)
    {
        HasSelection = value?.Count > 0;
        OnSelectionChanged();
    }

    partial void OnSearchTextChanged(string value)
    {
        _ = Task.Run(async () =>
        {
            await Task.Delay(300); // Debounce search
            if (SearchText == value) // Check if search text hasn't changed
            {
                await SearchAsync();
            }
        });
    }

    /// <summary>
    /// Load items with pagination
    /// </summary>
    protected abstract Task<(IList<T> items, int totalCount)> LoadItemsAsync(int page, int pageSize, string searchText);

    /// <summary>
    /// Handle selection changed
    /// </summary>
    protected virtual void OnSelectionChanged()
    {
        // Override in derived classes
    }

    /// <summary>
    /// Search items
    /// </summary>
    protected virtual async Task SearchAsync()
    {
        CurrentPage = 1;
        await LoadItemsAsync();
    }

    /// <summary>
    /// Load items
    /// </summary>
    protected async Task LoadItemsAsync()
    {
        await ExecuteAsync(async () =>
        {
            var (items, totalCount) = await LoadItemsAsync(CurrentPage, PageSize, SearchText);
            Items = items;
            TotalCount = totalCount;
            
            var totalPages = (int)Math.Ceiling((double)totalCount / PageSize);
            HasNextPage = CurrentPage < totalPages;
            HasPreviousPage = CurrentPage > 1;
        }, "Loading items...");
    }

    /// <summary>
    /// Go to next page
    /// </summary>
    [RelayCommand(CanExecute = nameof(HasNextPage))]
    protected async Task NextPageAsync()
    {
        CurrentPage++;
        await LoadItemsAsync();
    }

    /// <summary>
    /// Go to previous page
    /// </summary>
    [RelayCommand(CanExecute = nameof(HasPreviousPage))]
    protected async Task PreviousPageAsync()
    {
        CurrentPage--;
        await LoadItemsAsync();
    }

    /// <summary>
    /// Refresh items
    /// </summary>
    [RelayCommand]
    public override async Task RefreshAsync()
    {
        await LoadItemsAsync();
    }

    /// <summary>
    /// Clear selection
    /// </summary>
    [RelayCommand(CanExecute = nameof(HasSelection))]
    protected void ClearSelection()
    {
        SelectedItem = null;
        SelectedItems = new List<T>();
    }
}

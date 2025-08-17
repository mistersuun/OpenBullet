using OpenBullet.UI.ViewModels;
using OpenBullet.UI.Views;
using System.Collections.Generic;
using System.Windows.Controls;

namespace OpenBullet.UI.Services;

/// <summary>
/// Navigation service implementation
/// </summary>
public class NavigationService : INavigationService
{
    private readonly IViewModelFactory _viewModelFactory;
    private readonly IViewFactory _viewFactory;
    private readonly Stack<NavigationFrame> _backStack = new();
    private readonly Stack<NavigationFrame> _forwardStack = new();
    private NavigationFrame? _currentFrame;

    public NavigationService(IViewModelFactory viewModelFactory, IViewFactory viewFactory)
    {
        _viewModelFactory = viewModelFactory;
        _viewFactory = viewFactory;
    }

    public UserControl? CurrentView => _currentFrame?.View;

    public bool CanGoBack => _backStack.Count > 0;

    public bool CanGoForward => _forwardStack.Count > 0;

    public event EventHandler<NavigationEventArgs>? CurrentViewChanged;

    public void NavigateTo<T>() where T : ViewModelBase
    {
        var viewModel = _viewModelFactory.Create<T>();
        NavigateTo(viewModel);
    }

    public void NavigateTo<T>(object parameter) where T : ViewModelBase
    {
        var viewModel = _viewModelFactory.Create<T>(parameter);
        NavigateTo(viewModel);
    }

    public void NavigateTo(ViewModelBase viewModel)
    {
        try
        {
            var view = _viewFactory.CreateView(viewModel.GetType());
            view.DataContext = viewModel;

            var previousView = CurrentView;
            var previousViewModel = _currentFrame?.ViewModel;

            // Save current frame to back stack
            if (_currentFrame != null)
            {
                _backStack.Push(_currentFrame);
            }

            // Clear forward stack when navigating to new view
            _forwardStack.Clear();

            // Set new current frame
            _currentFrame = new NavigationFrame
            {
                View = view,
                ViewModel = viewModel
            };

            // Initialize view model if it implements INavigationAware
            if (viewModel is INavigationAware navigationAware)
            {
                navigationAware.OnNavigatedTo();
            }

            // Cleanup previous view model if it implements INavigationAware
            if (previousViewModel is INavigationAware previousNavigationAware)
            {
                previousNavigationAware.OnNavigatedFrom();
            }

            // Raise navigation event
            CurrentViewChanged?.Invoke(this, new NavigationEventArgs
            {
                PreviousView = previousView,
                CurrentView = view,
                ViewModel = viewModel
            });
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to navigate to {viewModel.GetType().Name}: {ex.Message}", ex);
        }
    }

    public void GoBack()
    {
        if (!CanGoBack)
            return;

        try
        {
            var previousFrame = _backStack.Pop();
            
            // Save current frame to forward stack
            if (_currentFrame != null)
            {
                _forwardStack.Push(_currentFrame);
            }

            var currentView = CurrentView;
            var currentViewModel = _currentFrame?.ViewModel;

            // Set previous frame as current
            _currentFrame = previousFrame;

            // Handle navigation events
            if (currentViewModel is INavigationAware currentNavigationAware)
            {
                currentNavigationAware.OnNavigatedFrom();
            }

            if (previousFrame.ViewModel is INavigationAware previousNavigationAware)
            {
                previousNavigationAware.OnNavigatedTo();
            }

            // Raise navigation event
            CurrentViewChanged?.Invoke(this, new NavigationEventArgs
            {
                PreviousView = currentView,
                CurrentView = previousFrame.View,
                ViewModel = previousFrame.ViewModel
            });
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to navigate back: {ex.Message}", ex);
        }
    }

    public void GoForward()
    {
        if (!CanGoForward)
            return;

        try
        {
            var forwardFrame = _forwardStack.Pop();
            
            // Save current frame to back stack
            if (_currentFrame != null)
            {
                _backStack.Push(_currentFrame);
            }

            var currentView = CurrentView;
            var currentViewModel = _currentFrame?.ViewModel;

            // Set forward frame as current
            _currentFrame = forwardFrame;

            // Handle navigation events
            if (currentViewModel is INavigationAware currentNavigationAware)
            {
                currentNavigationAware.OnNavigatedFrom();
            }

            if (forwardFrame.ViewModel is INavigationAware forwardNavigationAware)
            {
                forwardNavigationAware.OnNavigatedTo();
            }

            // Raise navigation event
            CurrentViewChanged?.Invoke(this, new NavigationEventArgs
            {
                PreviousView = currentView,
                CurrentView = forwardFrame.View,
                ViewModel = forwardFrame.ViewModel
            });
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to navigate forward: {ex.Message}", ex);
        }
    }

    public void ClearHistory()
    {
        _backStack.Clear();
        _forwardStack.Clear();
    }

    private class NavigationFrame
    {
        public UserControl View { get; set; } = null!;
        public ViewModelBase ViewModel { get; set; } = null!;
    }
}

/// <summary>
/// Interface for navigation-aware view models
/// </summary>
public interface INavigationAware
{
    /// <summary>
    /// Called when navigated to this view model
    /// </summary>
    void OnNavigatedTo();

    /// <summary>
    /// Called when navigated away from this view model
    /// </summary>
    void OnNavigatedFrom();
}

/// <summary>
/// ViewModel factory implementation
/// </summary>
public class ViewModelFactory : IViewModelFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ViewModelFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public T Create<T>() where T : ViewModelBase
    {
        var viewModel = (T)_serviceProvider.GetService(typeof(T));
        if (viewModel == null)
        {
            throw new InvalidOperationException($"ViewModel {typeof(T).Name} is not registered in DI container");
        }
        return viewModel;
    }

    public T Create<T>(object parameter) where T : ViewModelBase
    {
        var viewModel = Create<T>();
        
        // Pass parameter if view model supports it
        if (viewModel is IParameterReceiver parameterReceiver)
        {
            parameterReceiver.ReceiveParameter(parameter);
        }
        
        return viewModel;
    }
}

/// <summary>
/// View factory implementation
/// </summary>
public class ViewFactory : IViewFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<Type, Type> _viewModelToViewMap = new();

    public ViewFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InitializeViewMap();
    }

    public UserControl CreateView<T>() where T : ViewModelBase
    {
        return CreateView(typeof(T));
    }

    public UserControl CreateView(Type viewModelType)
    {
        if (!_viewModelToViewMap.TryGetValue(viewModelType, out var viewType))
        {
            throw new InvalidOperationException($"No view registered for ViewModel {viewModelType.Name}");
        }

        var view = _serviceProvider.GetService(viewType);
        if (view == null)
        {
            throw new InvalidOperationException($"View {viewType.Name} is not registered in DI container");
        }

        return (UserControl)view;
    }

    private void InitializeViewMap()
    {
        // Map ViewModels to Views
        _viewModelToViewMap[typeof(DashboardViewModel)] = typeof(DashboardView);
        _viewModelToViewMap[typeof(ConfigurationListViewModel)] = typeof(ConfigurationListView);
        _viewModelToViewMap[typeof(ConfigurationDetailViewModel)] = typeof(ConfigurationDetailView);
        _viewModelToViewMap[typeof(JobListViewModel)] = typeof(JobListView);
        _viewModelToViewMap[typeof(JobDetailViewModel)] = typeof(JobDetailView);
        _viewModelToViewMap[typeof(ProxyListViewModel)] = typeof(ProxyListView);
        _viewModelToViewMap[typeof(ProxyDetailViewModel)] = typeof(ProxyDetailView);
        _viewModelToViewMap[typeof(SettingsViewModel)] = typeof(SettingsView);
    }
}

/// <summary>
/// Interface for view models that can receive parameters
/// </summary>
public interface IParameterReceiver
{
    /// <summary>
    /// Receive navigation parameter
    /// </summary>
    void ReceiveParameter(object parameter);
}

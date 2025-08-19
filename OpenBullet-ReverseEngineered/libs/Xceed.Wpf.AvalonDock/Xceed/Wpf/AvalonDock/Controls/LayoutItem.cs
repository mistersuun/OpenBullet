// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.LayoutItem
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.AvalonDock.Commands;
using Xceed.Wpf.AvalonDock.Layout;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

public abstract class LayoutItem : FrameworkElement
{
  private ICommand _defaultCloseCommand;
  private ICommand _defaultFloatCommand;
  private ICommand _defaultDockAsDocumentCommand;
  private ICommand _defaultCloseAllButThisCommand;
  private ICommand _defaultCloseAllCommand;
  private ICommand _defaultActivateCommand;
  private ICommand _defaultNewVerticalTabGroupCommand;
  private ICommand _defaultNewHorizontalTabGroupCommand;
  private ICommand _defaultMoveToNextTabGroupCommand;
  private ICommand _defaultMoveToPreviousTabGroupCommand;
  private ContentPresenter _view;
  private ReentrantFlag _isSelectedReentrantFlag = new ReentrantFlag();
  private ReentrantFlag _isActiveReentrantFlag = new ReentrantFlag();
  public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof (Title), typeof (string), typeof (LayoutItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(LayoutItem.OnTitleChanged)));
  public static readonly DependencyProperty IconSourceProperty = DependencyProperty.Register(nameof (IconSource), typeof (ImageSource), typeof (LayoutItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(LayoutItem.OnIconSourceChanged)));
  public static readonly DependencyProperty ContentIdProperty = DependencyProperty.Register(nameof (ContentId), typeof (string), typeof (LayoutItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(LayoutItem.OnContentIdChanged)));
  public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof (IsSelected), typeof (bool), typeof (LayoutItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(LayoutItem.OnIsSelectedChanged)));
  public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(nameof (IsActive), typeof (bool), typeof (LayoutItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(LayoutItem.OnIsActiveChanged)));
  public static readonly DependencyProperty CanCloseProperty = DependencyProperty.Register(nameof (CanClose), typeof (bool), typeof (LayoutItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) true, new PropertyChangedCallback(LayoutItem.OnCanCloseChanged)));
  public static readonly DependencyProperty CanFloatProperty = DependencyProperty.Register(nameof (CanFloat), typeof (bool), typeof (LayoutItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) true, new PropertyChangedCallback(LayoutItem.OnCanFloatChanged)));
  public static readonly DependencyProperty CloseCommandProperty = DependencyProperty.Register(nameof (CloseCommand), typeof (ICommand), typeof (LayoutItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(LayoutItem.OnCloseCommandChanged), new CoerceValueCallback(LayoutItem.CoerceCloseCommandValue)));
  public static readonly DependencyProperty FloatCommandProperty = DependencyProperty.Register(nameof (FloatCommand), typeof (ICommand), typeof (LayoutItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(LayoutItem.OnFloatCommandChanged), new CoerceValueCallback(LayoutItem.CoerceFloatCommandValue)));
  public static readonly DependencyProperty DockAsDocumentCommandProperty = DependencyProperty.Register(nameof (DockAsDocumentCommand), typeof (ICommand), typeof (LayoutItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(LayoutItem.OnDockAsDocumentCommandChanged), new CoerceValueCallback(LayoutItem.CoerceDockAsDocumentCommandValue)));
  public static readonly DependencyProperty CloseAllButThisCommandProperty = DependencyProperty.Register(nameof (CloseAllButThisCommand), typeof (ICommand), typeof (LayoutItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(LayoutItem.OnCloseAllButThisCommandChanged), new CoerceValueCallback(LayoutItem.CoerceCloseAllButThisCommandValue)));
  public static readonly DependencyProperty CloseAllCommandProperty = DependencyProperty.Register(nameof (CloseAllCommand), typeof (ICommand), typeof (LayoutItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(LayoutItem.OnCloseAllCommandChanged), new CoerceValueCallback(LayoutItem.CoerceCloseAllCommandValue)));
  public static readonly DependencyProperty ActivateCommandProperty = DependencyProperty.Register(nameof (ActivateCommand), typeof (ICommand), typeof (LayoutItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(LayoutItem.OnActivateCommandChanged), new CoerceValueCallback(LayoutItem.CoerceActivateCommandValue)));
  public static readonly DependencyProperty NewVerticalTabGroupCommandProperty = DependencyProperty.Register(nameof (NewVerticalTabGroupCommand), typeof (ICommand), typeof (LayoutItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(LayoutItem.OnNewVerticalTabGroupCommandChanged)));
  public static readonly DependencyProperty NewHorizontalTabGroupCommandProperty = DependencyProperty.Register(nameof (NewHorizontalTabGroupCommand), typeof (ICommand), typeof (LayoutItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(LayoutItem.OnNewHorizontalTabGroupCommandChanged)));
  public static readonly DependencyProperty MoveToNextTabGroupCommandProperty = DependencyProperty.Register(nameof (MoveToNextTabGroupCommand), typeof (ICommand), typeof (LayoutItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(LayoutItem.OnMoveToNextTabGroupCommandChanged)));
  public static readonly DependencyProperty MoveToPreviousTabGroupCommandProperty = DependencyProperty.Register(nameof (MoveToPreviousTabGroupCommand), typeof (ICommand), typeof (LayoutItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(LayoutItem.OnMoveToPreviousTabGroupCommandChanged)));

  static LayoutItem()
  {
    FrameworkElement.ToolTipProperty.OverrideMetadata(typeof (LayoutItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, (PropertyChangedCallback) ((s, e) => LayoutItem.OnToolTipChanged(s, e))));
    UIElement.VisibilityProperty.OverrideMetadata(typeof (LayoutItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) Visibility.Visible, (PropertyChangedCallback) ((s, e) => LayoutItem.OnVisibilityChanged(s, e))));
  }

  internal LayoutItem()
  {
  }

  public LayoutContent LayoutElement { get; private set; }

  public object Model { get; private set; }

  public ContentPresenter View
  {
    get
    {
      if (this._view == null)
      {
        this._view = new ContentPresenter();
        this._view.SetBinding(ContentPresenter.ContentProperty, (BindingBase) new Binding("Content")
        {
          Source = (object) this.LayoutElement
        });
        this._view.SetBinding(ContentPresenter.ContentTemplateProperty, (BindingBase) new Binding("LayoutItemTemplate")
        {
          Source = (object) this.LayoutElement.Root.Manager
        });
        this._view.SetBinding(ContentPresenter.ContentTemplateSelectorProperty, (BindingBase) new Binding("LayoutItemTemplateSelector")
        {
          Source = (object) this.LayoutElement.Root.Manager
        });
        this.LayoutElement.Root.Manager.InternalAddLogicalChild((object) this._view);
      }
      return this._view;
    }
  }

  public string Title
  {
    get => (string) this.GetValue(LayoutItem.TitleProperty);
    set => this.SetValue(LayoutItem.TitleProperty, (object) value);
  }

  private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    ((LayoutItem) d).OnTitleChanged(e);
  }

  protected virtual void OnTitleChanged(DependencyPropertyChangedEventArgs e)
  {
    if (this.LayoutElement == null)
      return;
    this.LayoutElement.Title = (string) e.NewValue;
  }

  public ImageSource IconSource
  {
    get => (ImageSource) this.GetValue(LayoutItem.IconSourceProperty);
    set => this.SetValue(LayoutItem.IconSourceProperty, (object) value);
  }

  private static void OnIconSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    ((LayoutItem) d).OnIconSourceChanged(e);
  }

  protected virtual void OnIconSourceChanged(DependencyPropertyChangedEventArgs e)
  {
    if (this.LayoutElement == null)
      return;
    this.LayoutElement.IconSource = this.IconSource;
  }

  public string ContentId
  {
    get => (string) this.GetValue(LayoutItem.ContentIdProperty);
    set => this.SetValue(LayoutItem.ContentIdProperty, (object) value);
  }

  private static void OnContentIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    ((LayoutItem) d).OnContentIdChanged(e);
  }

  protected virtual void OnContentIdChanged(DependencyPropertyChangedEventArgs e)
  {
    if (this.LayoutElement == null)
      return;
    this.LayoutElement.ContentId = (string) e.NewValue;
  }

  public bool IsSelected
  {
    get => (bool) this.GetValue(LayoutItem.IsSelectedProperty);
    set => this.SetValue(LayoutItem.IsSelectedProperty, (object) value);
  }

  private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    ((LayoutItem) d).OnIsSelectedChanged(e);
  }

  protected virtual void OnIsSelectedChanged(DependencyPropertyChangedEventArgs e)
  {
    if (!this._isSelectedReentrantFlag.CanEnter)
      return;
    using (this._isSelectedReentrantFlag.Enter())
    {
      if (this.LayoutElement == null)
        return;
      this.LayoutElement.IsSelected = (bool) e.NewValue;
    }
  }

  public bool IsActive
  {
    get => (bool) this.GetValue(LayoutItem.IsActiveProperty);
    set => this.SetValue(LayoutItem.IsActiveProperty, (object) value);
  }

  private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    ((LayoutItem) d).OnIsActiveChanged(e);
  }

  protected virtual void OnIsActiveChanged(DependencyPropertyChangedEventArgs e)
  {
    if (!this._isActiveReentrantFlag.CanEnter)
      return;
    using (this._isActiveReentrantFlag.Enter())
    {
      if (this.LayoutElement == null)
        return;
      this.LayoutElement.IsActive = (bool) e.NewValue;
    }
  }

  public bool CanClose
  {
    get => (bool) this.GetValue(LayoutItem.CanCloseProperty);
    set => this.SetValue(LayoutItem.CanCloseProperty, (object) value);
  }

  private static void OnCanCloseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    ((LayoutItem) d).OnCanCloseChanged(e);
  }

  protected virtual void OnCanCloseChanged(DependencyPropertyChangedEventArgs e)
  {
    if (this.LayoutElement == null)
      return;
    this.LayoutElement.CanClose = (bool) e.NewValue;
  }

  public bool CanFloat
  {
    get => (bool) this.GetValue(LayoutItem.CanFloatProperty);
    set => this.SetValue(LayoutItem.CanFloatProperty, (object) value);
  }

  private static void OnCanFloatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    ((LayoutItem) d).OnCanFloatChanged(e);
  }

  protected virtual void OnCanFloatChanged(DependencyPropertyChangedEventArgs e)
  {
    if (this.LayoutElement == null)
      return;
    this.LayoutElement.CanFloat = (bool) e.NewValue;
  }

  public ICommand CloseCommand
  {
    get => (ICommand) this.GetValue(LayoutItem.CloseCommandProperty);
    set => this.SetValue(LayoutItem.CloseCommandProperty, (object) value);
  }

  private static void OnCloseCommandChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((LayoutItem) d).OnCloseCommandChanged(e);
  }

  protected virtual void OnCloseCommandChanged(DependencyPropertyChangedEventArgs e)
  {
  }

  private static object CoerceCloseCommandValue(DependencyObject d, object value) => value;

  private bool CanExecuteCloseCommand(object parameter)
  {
    return this.LayoutElement != null && this.LayoutElement.CanClose;
  }

  private void ExecuteCloseCommand(object parameter) => this.Close();

  protected abstract void Close();

  public ICommand FloatCommand
  {
    get => (ICommand) this.GetValue(LayoutItem.FloatCommandProperty);
    set => this.SetValue(LayoutItem.FloatCommandProperty, (object) value);
  }

  private static void OnFloatCommandChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((LayoutItem) d).OnFloatCommandChanged(e);
  }

  protected virtual void OnFloatCommandChanged(DependencyPropertyChangedEventArgs e)
  {
  }

  private static object CoerceFloatCommandValue(DependencyObject d, object value) => value;

  private bool CanExecuteFloatCommand(object anchorable)
  {
    return this.LayoutElement != null && this.LayoutElement.CanFloat && this.LayoutElement.FindParent<LayoutFloatingWindow>() == null;
  }

  private void ExecuteFloatCommand(object parameter)
  {
    this.LayoutElement.Root.Manager._ExecuteFloatCommand(this.LayoutElement);
  }

  protected virtual void Float()
  {
  }

  public ICommand DockAsDocumentCommand
  {
    get => (ICommand) this.GetValue(LayoutItem.DockAsDocumentCommandProperty);
    set => this.SetValue(LayoutItem.DockAsDocumentCommandProperty, (object) value);
  }

  private static void OnDockAsDocumentCommandChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((LayoutItem) d).OnDockAsDocumentCommandChanged(e);
  }

  protected virtual void OnDockAsDocumentCommandChanged(DependencyPropertyChangedEventArgs e)
  {
  }

  private static object CoerceDockAsDocumentCommandValue(DependencyObject d, object value) => value;

  protected virtual bool CanExecuteDockAsDocumentCommand()
  {
    return this.LayoutElement != null && this.LayoutElement.FindParent<LayoutDocumentPane>() == null;
  }

  private bool CanExecuteDockAsDocumentCommand(object parameter)
  {
    return this.CanExecuteDockAsDocumentCommand();
  }

  private void ExecuteDockAsDocumentCommand(object parameter)
  {
    this.LayoutElement.Root.Manager._ExecuteDockAsDocumentCommand(this.LayoutElement);
  }

  public ICommand CloseAllButThisCommand
  {
    get => (ICommand) this.GetValue(LayoutItem.CloseAllButThisCommandProperty);
    set => this.SetValue(LayoutItem.CloseAllButThisCommandProperty, (object) value);
  }

  private static void OnCloseAllButThisCommandChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((LayoutItem) d).OnCloseAllButThisCommandChanged(e);
  }

  protected virtual void OnCloseAllButThisCommandChanged(DependencyPropertyChangedEventArgs e)
  {
  }

  private static object CoerceCloseAllButThisCommandValue(DependencyObject d, object value)
  {
    return value;
  }

  private bool CanExecuteCloseAllButThisCommand(object parameter)
  {
    return this.LayoutElement != null && this.LayoutElement.Root != null && this.LayoutElement.Root.Manager.Layout.Descendents().OfType<LayoutContent>().Where<LayoutContent>((Func<LayoutContent, bool>) (d =>
    {
      if (d == this.LayoutElement)
        return false;
      return d.Parent is LayoutDocumentPane || d.Parent is LayoutDocumentFloatingWindow;
    })).Any<LayoutContent>();
  }

  private void ExecuteCloseAllButThisCommand(object parameter)
  {
    this.LayoutElement.Root.Manager._ExecuteCloseAllButThisCommand(this.LayoutElement);
  }

  public ICommand CloseAllCommand
  {
    get => (ICommand) this.GetValue(LayoutItem.CloseAllCommandProperty);
    set => this.SetValue(LayoutItem.CloseAllCommandProperty, (object) value);
  }

  private static void OnCloseAllCommandChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((LayoutItem) d).OnCloseAllCommandChanged(e);
  }

  protected virtual void OnCloseAllCommandChanged(DependencyPropertyChangedEventArgs e)
  {
  }

  private static object CoerceCloseAllCommandValue(DependencyObject d, object value) => value;

  private bool CanExecuteCloseAllCommand(object parameter)
  {
    return this.LayoutElement != null && this.LayoutElement.Root != null && this.LayoutElement.Root.Manager.Layout.Descendents().OfType<LayoutContent>().Where<LayoutContent>((Func<LayoutContent, bool>) (d => d.Parent is LayoutDocumentPane || d.Parent is LayoutDocumentFloatingWindow)).Any<LayoutContent>();
  }

  private void ExecuteCloseAllCommand(object parameter)
  {
    this.LayoutElement.Root.Manager._ExecuteCloseAllCommand(this.LayoutElement);
  }

  public ICommand ActivateCommand
  {
    get => (ICommand) this.GetValue(LayoutItem.ActivateCommandProperty);
    set => this.SetValue(LayoutItem.ActivateCommandProperty, (object) value);
  }

  private static void OnActivateCommandChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((LayoutItem) d).OnActivateCommandChanged(e);
  }

  protected virtual void OnActivateCommandChanged(DependencyPropertyChangedEventArgs e)
  {
  }

  private static object CoerceActivateCommandValue(DependencyObject d, object value) => value;

  private bool CanExecuteActivateCommand(object parameter) => this.LayoutElement != null;

  private void ExecuteActivateCommand(object parameter)
  {
    this.LayoutElement.Root.Manager._ExecuteContentActivateCommand(this.LayoutElement);
  }

  public ICommand NewVerticalTabGroupCommand
  {
    get => (ICommand) this.GetValue(LayoutItem.NewVerticalTabGroupCommandProperty);
    set => this.SetValue(LayoutItem.NewVerticalTabGroupCommandProperty, (object) value);
  }

  private static void OnNewVerticalTabGroupCommandChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((LayoutItem) d).OnNewVerticalTabGroupCommandChanged(e);
  }

  protected virtual void OnNewVerticalTabGroupCommandChanged(DependencyPropertyChangedEventArgs e)
  {
  }

  private bool CanExecuteNewVerticalTabGroupCommand(object parameter)
  {
    if (this.LayoutElement == null || this.LayoutElement is LayoutDocument layoutElement && !layoutElement.CanMove)
      return false;
    LayoutDocumentPaneGroup parent1 = this.LayoutElement.FindParent<LayoutDocumentPaneGroup>();
    LayoutDocumentPane parent2 = this.LayoutElement.Parent as LayoutDocumentPane;
    return (parent1 == null || parent1.ChildrenCount == 1 || parent1.Root.Manager.AllowMixedOrientation || parent1.Orientation == Orientation.Horizontal) && parent2 != null && parent2.ChildrenCount > 1;
  }

  private void ExecuteNewVerticalTabGroupCommand(object parameter)
  {
    LayoutContent layoutElement = this.LayoutElement;
    LayoutDocumentPaneGroup documentPaneGroup = layoutElement.FindParent<LayoutDocumentPaneGroup>();
    LayoutDocumentPane parent1 = layoutElement.Parent as LayoutDocumentPane;
    if (documentPaneGroup == null)
    {
      ILayoutContainer parent2 = parent1.Parent;
      documentPaneGroup = new LayoutDocumentPaneGroup()
      {
        Orientation = Orientation.Horizontal
      };
      LayoutDocumentPane oldElement = parent1;
      LayoutDocumentPaneGroup newElement = documentPaneGroup;
      parent2.ReplaceChild((ILayoutElement) oldElement, (ILayoutElement) newElement);
      documentPaneGroup.Children.Add((ILayoutDocumentPane) parent1);
    }
    documentPaneGroup.Orientation = Orientation.Horizontal;
    int num = documentPaneGroup.IndexOfChild((ILayoutElement) parent1);
    documentPaneGroup.InsertChildAt(num + 1, (ILayoutElement) new LayoutDocumentPane(layoutElement));
    layoutElement.IsActive = true;
    layoutElement.Root.CollectGarbage();
  }

  public ICommand NewHorizontalTabGroupCommand
  {
    get => (ICommand) this.GetValue(LayoutItem.NewHorizontalTabGroupCommandProperty);
    set => this.SetValue(LayoutItem.NewHorizontalTabGroupCommandProperty, (object) value);
  }

  private static void OnNewHorizontalTabGroupCommandChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((LayoutItem) d).OnNewHorizontalTabGroupCommandChanged(e);
  }

  protected virtual void OnNewHorizontalTabGroupCommandChanged(DependencyPropertyChangedEventArgs e)
  {
  }

  private bool CanExecuteNewHorizontalTabGroupCommand(object parameter)
  {
    if (this.LayoutElement == null || this.LayoutElement is LayoutDocument layoutElement && !layoutElement.CanMove)
      return false;
    LayoutDocumentPaneGroup parent1 = this.LayoutElement.FindParent<LayoutDocumentPaneGroup>();
    LayoutDocumentPane parent2 = this.LayoutElement.Parent as LayoutDocumentPane;
    return (parent1 == null || parent1.ChildrenCount == 1 || parent1.Root.Manager.AllowMixedOrientation || parent1.Orientation == Orientation.Vertical) && parent2 != null && parent2.ChildrenCount > 1;
  }

  private void ExecuteNewHorizontalTabGroupCommand(object parameter)
  {
    LayoutContent layoutElement = this.LayoutElement;
    LayoutDocumentPaneGroup documentPaneGroup = layoutElement.FindParent<LayoutDocumentPaneGroup>();
    LayoutDocumentPane parent1 = layoutElement.Parent as LayoutDocumentPane;
    if (documentPaneGroup == null)
    {
      ILayoutContainer parent2 = parent1.Parent;
      documentPaneGroup = new LayoutDocumentPaneGroup()
      {
        Orientation = Orientation.Vertical
      };
      LayoutDocumentPane oldElement = parent1;
      LayoutDocumentPaneGroup newElement = documentPaneGroup;
      parent2.ReplaceChild((ILayoutElement) oldElement, (ILayoutElement) newElement);
      documentPaneGroup.Children.Add((ILayoutDocumentPane) parent1);
    }
    documentPaneGroup.Orientation = Orientation.Vertical;
    int num = documentPaneGroup.IndexOfChild((ILayoutElement) parent1);
    documentPaneGroup.InsertChildAt(num + 1, (ILayoutElement) new LayoutDocumentPane(layoutElement));
    layoutElement.IsActive = true;
    layoutElement.Root.CollectGarbage();
  }

  public ICommand MoveToNextTabGroupCommand
  {
    get => (ICommand) this.GetValue(LayoutItem.MoveToNextTabGroupCommandProperty);
    set => this.SetValue(LayoutItem.MoveToNextTabGroupCommandProperty, (object) value);
  }

  private static void OnMoveToNextTabGroupCommandChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((LayoutItem) d).OnMoveToNextTabGroupCommandChanged(e);
  }

  protected virtual void OnMoveToNextTabGroupCommandChanged(DependencyPropertyChangedEventArgs e)
  {
  }

  private bool CanExecuteMoveToNextTabGroupCommand(object parameter)
  {
    if (this.LayoutElement == null)
      return false;
    LayoutDocumentPaneGroup parent1 = this.LayoutElement.FindParent<LayoutDocumentPaneGroup>();
    LayoutDocumentPane parent2 = this.LayoutElement.Parent as LayoutDocumentPane;
    return parent1 != null && parent2 != null && parent1.ChildrenCount > 1 && parent1.IndexOfChild((ILayoutElement) parent2) < parent1.ChildrenCount - 1 && parent1.Children[parent1.IndexOfChild((ILayoutElement) parent2) + 1] is LayoutDocumentPane;
  }

  private void ExecuteMoveToNextTabGroupCommand(object parameter)
  {
    LayoutContent layoutElement = this.LayoutElement;
    LayoutDocumentPaneGroup parent = layoutElement.FindParent<LayoutDocumentPaneGroup>();
    (parent.Children[parent.IndexOfChild((ILayoutElement) (layoutElement.Parent as LayoutDocumentPane)) + 1] as LayoutDocumentPane).InsertChildAt(0, (ILayoutElement) layoutElement);
    layoutElement.IsActive = true;
    layoutElement.Root.CollectGarbage();
  }

  public ICommand MoveToPreviousTabGroupCommand
  {
    get => (ICommand) this.GetValue(LayoutItem.MoveToPreviousTabGroupCommandProperty);
    set => this.SetValue(LayoutItem.MoveToPreviousTabGroupCommandProperty, (object) value);
  }

  private static void OnMoveToPreviousTabGroupCommandChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((LayoutItem) d).OnMoveToPreviousTabGroupCommandChanged(e);
  }

  protected virtual void OnMoveToPreviousTabGroupCommandChanged(DependencyPropertyChangedEventArgs e)
  {
  }

  private bool CanExecuteMoveToPreviousTabGroupCommand(object parameter)
  {
    if (this.LayoutElement == null)
      return false;
    LayoutDocumentPaneGroup parent1 = this.LayoutElement.FindParent<LayoutDocumentPaneGroup>();
    LayoutDocumentPane parent2 = this.LayoutElement.Parent as LayoutDocumentPane;
    return parent1 != null && parent2 != null && parent1.ChildrenCount > 1 && parent1.IndexOfChild((ILayoutElement) parent2) > 0 && parent1.Children[parent1.IndexOfChild((ILayoutElement) parent2) - 1] is LayoutDocumentPane;
  }

  private void ExecuteMoveToPreviousTabGroupCommand(object parameter)
  {
    LayoutContent layoutElement = this.LayoutElement;
    LayoutDocumentPaneGroup parent = layoutElement.FindParent<LayoutDocumentPaneGroup>();
    (parent.Children[parent.IndexOfChild((ILayoutElement) (layoutElement.Parent as LayoutDocumentPane)) - 1] as LayoutDocumentPane).InsertChildAt(0, (ILayoutElement) layoutElement);
    layoutElement.IsActive = true;
    layoutElement.Root.CollectGarbage();
  }

  protected virtual void InitDefaultCommands()
  {
    this._defaultCloseCommand = (ICommand) new RelayCommand((Action<object>) (p => this.ExecuteCloseCommand(p)), (Predicate<object>) (p => this.CanExecuteCloseCommand(p)));
    this._defaultFloatCommand = (ICommand) new RelayCommand((Action<object>) (p => this.ExecuteFloatCommand(p)), (Predicate<object>) (p => this.CanExecuteFloatCommand(p)));
    this._defaultDockAsDocumentCommand = (ICommand) new RelayCommand((Action<object>) (p => this.ExecuteDockAsDocumentCommand(p)), (Predicate<object>) (p => this.CanExecuteDockAsDocumentCommand(p)));
    this._defaultCloseAllButThisCommand = (ICommand) new RelayCommand((Action<object>) (p => this.ExecuteCloseAllButThisCommand(p)), (Predicate<object>) (p => this.CanExecuteCloseAllButThisCommand(p)));
    this._defaultCloseAllCommand = (ICommand) new RelayCommand((Action<object>) (p => this.ExecuteCloseAllCommand(p)), (Predicate<object>) (p => this.CanExecuteCloseAllCommand(p)));
    this._defaultActivateCommand = (ICommand) new RelayCommand((Action<object>) (p => this.ExecuteActivateCommand(p)), (Predicate<object>) (p => this.CanExecuteActivateCommand(p)));
    this._defaultNewVerticalTabGroupCommand = (ICommand) new RelayCommand((Action<object>) (p => this.ExecuteNewVerticalTabGroupCommand(p)), (Predicate<object>) (p => this.CanExecuteNewVerticalTabGroupCommand(p)));
    this._defaultNewHorizontalTabGroupCommand = (ICommand) new RelayCommand((Action<object>) (p => this.ExecuteNewHorizontalTabGroupCommand(p)), (Predicate<object>) (p => this.CanExecuteNewHorizontalTabGroupCommand(p)));
    this._defaultMoveToNextTabGroupCommand = (ICommand) new RelayCommand((Action<object>) (p => this.ExecuteMoveToNextTabGroupCommand(p)), (Predicate<object>) (p => this.CanExecuteMoveToNextTabGroupCommand(p)));
    this._defaultMoveToPreviousTabGroupCommand = (ICommand) new RelayCommand((Action<object>) (p => this.ExecuteMoveToPreviousTabGroupCommand(p)), (Predicate<object>) (p => this.CanExecuteMoveToPreviousTabGroupCommand(p)));
  }

  protected virtual void ClearDefaultBindings()
  {
    if (this.CloseCommand == this._defaultCloseCommand)
      BindingOperations.ClearBinding((DependencyObject) this, LayoutItem.CloseCommandProperty);
    if (this.FloatCommand == this._defaultFloatCommand)
      BindingOperations.ClearBinding((DependencyObject) this, LayoutItem.FloatCommandProperty);
    if (this.DockAsDocumentCommand == this._defaultDockAsDocumentCommand)
      BindingOperations.ClearBinding((DependencyObject) this, LayoutItem.DockAsDocumentCommandProperty);
    if (this.CloseAllButThisCommand == this._defaultCloseAllButThisCommand)
      BindingOperations.ClearBinding((DependencyObject) this, LayoutItem.CloseAllButThisCommandProperty);
    if (this.CloseAllCommand == this._defaultCloseAllCommand)
      BindingOperations.ClearBinding((DependencyObject) this, LayoutItem.CloseAllCommandProperty);
    if (this.ActivateCommand == this._defaultActivateCommand)
      BindingOperations.ClearBinding((DependencyObject) this, LayoutItem.ActivateCommandProperty);
    if (this.NewVerticalTabGroupCommand == this._defaultNewVerticalTabGroupCommand)
      BindingOperations.ClearBinding((DependencyObject) this, LayoutItem.NewVerticalTabGroupCommandProperty);
    if (this.NewHorizontalTabGroupCommand == this._defaultNewHorizontalTabGroupCommand)
      BindingOperations.ClearBinding((DependencyObject) this, LayoutItem.NewHorizontalTabGroupCommandProperty);
    if (this.MoveToNextTabGroupCommand == this._defaultMoveToNextTabGroupCommand)
      BindingOperations.ClearBinding((DependencyObject) this, LayoutItem.MoveToNextTabGroupCommandProperty);
    if (this.MoveToPreviousTabGroupCommand != this._defaultMoveToPreviousTabGroupCommand)
      return;
    BindingOperations.ClearBinding((DependencyObject) this, LayoutItem.MoveToPreviousTabGroupCommandProperty);
  }

  protected virtual void SetDefaultBindings()
  {
    if (this.CloseCommand == null)
      this.CloseCommand = this._defaultCloseCommand;
    if (this.FloatCommand == null)
      this.FloatCommand = this._defaultFloatCommand;
    if (this.DockAsDocumentCommand == null)
      this.DockAsDocumentCommand = this._defaultDockAsDocumentCommand;
    if (this.CloseAllButThisCommand == null)
      this.CloseAllButThisCommand = this._defaultCloseAllButThisCommand;
    if (this.CloseAllCommand == null)
      this.CloseAllCommand = this._defaultCloseAllCommand;
    if (this.ActivateCommand == null)
      this.ActivateCommand = this._defaultActivateCommand;
    if (this.NewVerticalTabGroupCommand == null)
      this.NewVerticalTabGroupCommand = this._defaultNewVerticalTabGroupCommand;
    if (this.NewHorizontalTabGroupCommand == null)
      this.NewHorizontalTabGroupCommand = this._defaultNewHorizontalTabGroupCommand;
    if (this.MoveToNextTabGroupCommand == null)
      this.MoveToNextTabGroupCommand = this._defaultMoveToNextTabGroupCommand;
    if (this.MoveToPreviousTabGroupCommand == null)
      this.MoveToPreviousTabGroupCommand = this._defaultMoveToPreviousTabGroupCommand;
    this.IsSelected = this.LayoutElement.IsSelected;
    this.IsActive = this.LayoutElement.IsActive;
    this.CanClose = this.LayoutElement.CanClose;
  }

  protected virtual void OnVisibilityChanged()
  {
    if (this.LayoutElement == null || this.Visibility != Visibility.Collapsed)
      return;
    this.LayoutElement.Close();
  }

  internal virtual void Attach(LayoutContent model)
  {
    this.LayoutElement = model;
    this.Model = model.Content;
    this.InitDefaultCommands();
    this.LayoutElement.IsSelectedChanged += new EventHandler(this.LayoutElement_IsSelectedChanged);
    this.LayoutElement.IsActiveChanged += new EventHandler(this.LayoutElement_IsActiveChanged);
    this.DataContext = (object) this;
  }

  internal virtual void Detach()
  {
    this.LayoutElement.IsSelectedChanged -= new EventHandler(this.LayoutElement_IsSelectedChanged);
    this.LayoutElement.IsActiveChanged -= new EventHandler(this.LayoutElement_IsActiveChanged);
    this.LayoutElement = (LayoutContent) null;
    this.Model = (object) null;
  }

  internal void _ClearDefaultBindings() => this.ClearDefaultBindings();

  internal void _SetDefaultBindings() => this.SetDefaultBindings();

  internal bool IsViewExists() => this._view != null;

  private void LayoutElement_IsActiveChanged(object sender, EventArgs e)
  {
    if (!this._isActiveReentrantFlag.CanEnter)
      return;
    using (this._isActiveReentrantFlag.Enter())
    {
      BindingOperations.GetBinding((DependencyObject) this, LayoutItem.IsActiveProperty);
      this.IsActive = this.LayoutElement.IsActive;
      BindingOperations.GetBinding((DependencyObject) this, LayoutItem.IsActiveProperty);
    }
  }

  private void LayoutElement_IsSelectedChanged(object sender, EventArgs e)
  {
    if (!this._isSelectedReentrantFlag.CanEnter)
      return;
    using (this._isSelectedReentrantFlag.Enter())
      this.IsSelected = this.LayoutElement.IsSelected;
  }

  private static void OnToolTipChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
  {
    ((LayoutItem) s).OnToolTipChanged();
  }

  private void OnToolTipChanged()
  {
    if (this.LayoutElement == null)
      return;
    this.LayoutElement.ToolTip = this.ToolTip;
  }

  private static void OnVisibilityChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
  {
    ((LayoutItem) s).OnVisibilityChanged();
  }
}

// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.LayoutAnchorableItem
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock.Commands;
using Xceed.Wpf.AvalonDock.Layout;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

public class LayoutAnchorableItem : LayoutItem
{
  private LayoutAnchorable _anchorable;
  private ICommand _defaultHideCommand;
  private ICommand _defaultAutoHideCommand;
  private ICommand _defaultDockCommand;
  private ReentrantFlag _visibilityReentrantFlag = new ReentrantFlag();
  public static readonly DependencyProperty HideCommandProperty = DependencyProperty.Register(nameof (HideCommand), typeof (ICommand), typeof (LayoutAnchorableItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(LayoutAnchorableItem.OnHideCommandChanged), new CoerceValueCallback(LayoutAnchorableItem.CoerceHideCommandValue)));
  public static readonly DependencyProperty AutoHideCommandProperty = DependencyProperty.Register(nameof (AutoHideCommand), typeof (ICommand), typeof (LayoutAnchorableItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(LayoutAnchorableItem.OnAutoHideCommandChanged), new CoerceValueCallback(LayoutAnchorableItem.CoerceAutoHideCommandValue)));
  public static readonly DependencyProperty DockCommandProperty = DependencyProperty.Register(nameof (DockCommand), typeof (ICommand), typeof (LayoutAnchorableItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(LayoutAnchorableItem.OnDockCommandChanged), new CoerceValueCallback(LayoutAnchorableItem.CoerceDockCommandValue)));
  public static readonly DependencyProperty CanHideProperty = DependencyProperty.Register(nameof (CanHide), typeof (bool), typeof (LayoutAnchorableItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) true, new PropertyChangedCallback(LayoutAnchorableItem.OnCanHideChanged)));

  internal LayoutAnchorableItem()
  {
  }

  public ICommand HideCommand
  {
    get => (ICommand) this.GetValue(LayoutAnchorableItem.HideCommandProperty);
    set => this.SetValue(LayoutAnchorableItem.HideCommandProperty, (object) value);
  }

  private static void OnHideCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    ((LayoutAnchorableItem) d).OnHideCommandChanged(e);
  }

  protected virtual void OnHideCommandChanged(DependencyPropertyChangedEventArgs e)
  {
  }

  private static object CoerceHideCommandValue(DependencyObject d, object value) => value;

  private bool CanExecuteHideCommand(object parameter)
  {
    return this.LayoutElement != null && this._anchorable.CanHide;
  }

  private void ExecuteHideCommand(object parameter)
  {
    if (this._anchorable == null || this._anchorable.Root == null || this._anchorable.Root.Manager == null)
      return;
    this._anchorable.Root.Manager._ExecuteHideCommand(this._anchorable);
  }

  public ICommand AutoHideCommand
  {
    get => (ICommand) this.GetValue(LayoutAnchorableItem.AutoHideCommandProperty);
    set => this.SetValue(LayoutAnchorableItem.AutoHideCommandProperty, (object) value);
  }

  private static void OnAutoHideCommandChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((LayoutAnchorableItem) d).OnAutoHideCommandChanged(e);
  }

  protected virtual void OnAutoHideCommandChanged(DependencyPropertyChangedEventArgs e)
  {
  }

  private static object CoerceAutoHideCommandValue(DependencyObject d, object value) => value;

  private bool CanExecuteAutoHideCommand(object parameter)
  {
    return this.LayoutElement != null && this.LayoutElement.FindParent<LayoutAnchorableFloatingWindow>() == null && this._anchorable.CanAutoHide;
  }

  private void ExecuteAutoHideCommand(object parameter)
  {
    if (this._anchorable == null || this._anchorable.Root == null || this._anchorable.Root.Manager == null)
      return;
    this._anchorable.Root.Manager._ExecuteAutoHideCommand(this._anchorable);
  }

  public ICommand DockCommand
  {
    get => (ICommand) this.GetValue(LayoutAnchorableItem.DockCommandProperty);
    set => this.SetValue(LayoutAnchorableItem.DockCommandProperty, (object) value);
  }

  private static void OnDockCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    ((LayoutAnchorableItem) d).OnDockCommandChanged(e);
  }

  protected virtual void OnDockCommandChanged(DependencyPropertyChangedEventArgs e)
  {
  }

  private static object CoerceDockCommandValue(DependencyObject d, object value) => value;

  private bool CanExecuteDockCommand(object parameter)
  {
    return this.LayoutElement != null && this.LayoutElement.FindParent<LayoutAnchorableFloatingWindow>() != null;
  }

  private void ExecuteDockCommand(object parameter)
  {
    this.LayoutElement.Root.Manager._ExecuteDockCommand(this._anchorable);
  }

  public bool CanHide
  {
    get => (bool) this.GetValue(LayoutAnchorableItem.CanHideProperty);
    set => this.SetValue(LayoutAnchorableItem.CanHideProperty, (object) value);
  }

  private static void OnCanHideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    ((LayoutAnchorableItem) d).OnCanHideChanged(e);
  }

  protected virtual void OnCanHideChanged(DependencyPropertyChangedEventArgs e)
  {
    if (this._anchorable == null)
      return;
    this._anchorable.CanHide = (bool) e.NewValue;
  }

  internal override void Attach(LayoutContent model)
  {
    this._anchorable = model as LayoutAnchorable;
    this._anchorable.IsVisibleChanged += new EventHandler(this._anchorable_IsVisibleChanged);
    base.Attach(model);
  }

  internal override void Detach()
  {
    this._anchorable.IsVisibleChanged -= new EventHandler(this._anchorable_IsVisibleChanged);
    this._anchorable = (LayoutAnchorable) null;
    base.Detach();
  }

  protected override bool CanExecuteDockAsDocumentCommand()
  {
    bool flag = base.CanExecuteDockAsDocumentCommand();
    return flag && this._anchorable != null ? this._anchorable.CanDockAsTabbedDocument : flag;
  }

  protected override void Close()
  {
    if (this._anchorable.Root == null || this._anchorable.Root.Manager == null)
      return;
    this._anchorable.Root.Manager._ExecuteCloseCommand(this._anchorable);
  }

  protected override void InitDefaultCommands()
  {
    this._defaultHideCommand = (ICommand) new RelayCommand((Action<object>) (p => this.ExecuteHideCommand(p)), (Predicate<object>) (p => this.CanExecuteHideCommand(p)));
    this._defaultAutoHideCommand = (ICommand) new RelayCommand((Action<object>) (p => this.ExecuteAutoHideCommand(p)), (Predicate<object>) (p => this.CanExecuteAutoHideCommand(p)));
    this._defaultDockCommand = (ICommand) new RelayCommand((Action<object>) (p => this.ExecuteDockCommand(p)), (Predicate<object>) (p => this.CanExecuteDockCommand(p)));
    base.InitDefaultCommands();
  }

  protected override void ClearDefaultBindings()
  {
    if (this.HideCommand == this._defaultHideCommand)
      BindingOperations.ClearBinding((DependencyObject) this, LayoutAnchorableItem.HideCommandProperty);
    if (this.AutoHideCommand == this._defaultAutoHideCommand)
      BindingOperations.ClearBinding((DependencyObject) this, LayoutAnchorableItem.AutoHideCommandProperty);
    if (this.DockCommand == this._defaultDockCommand)
      BindingOperations.ClearBinding((DependencyObject) this, LayoutAnchorableItem.DockCommandProperty);
    base.ClearDefaultBindings();
  }

  protected override void SetDefaultBindings()
  {
    if (this.HideCommand == null)
      this.HideCommand = this._defaultHideCommand;
    if (this.AutoHideCommand == null)
      this.AutoHideCommand = this._defaultAutoHideCommand;
    if (this.DockCommand == null)
      this.DockCommand = this._defaultDockCommand;
    this.Visibility = this._anchorable.IsVisible ? Visibility.Visible : Visibility.Hidden;
    base.SetDefaultBindings();
  }

  protected override void OnVisibilityChanged()
  {
    if (this._anchorable != null && this._anchorable.Root != null && this._visibilityReentrantFlag.CanEnter)
    {
      using (this._visibilityReentrantFlag.Enter())
      {
        if (this.Visibility == Visibility.Hidden)
          this._anchorable.Hide(false);
        else if (this.Visibility == Visibility.Visible)
          this._anchorable.Show();
      }
    }
    base.OnVisibilityChanged();
  }

  private void _anchorable_IsVisibleChanged(object sender, EventArgs e)
  {
    if (this._anchorable == null || this._anchorable.Root == null || !this._visibilityReentrantFlag.CanEnter)
      return;
    using (this._visibilityReentrantFlag.Enter())
    {
      if (this._anchorable.IsVisible)
        this.Visibility = Visibility.Visible;
      else
        this.Visibility = Visibility.Hidden;
    }
  }
}

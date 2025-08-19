// Decompiled with JetBrains decompiler
// Type: System.Windows.Controls.SelectorSelectionAdapter
// Assembly: System.Windows.Controls.Input.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 14A20646-D206-4805-A36B-30DDEEBAF814
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Windows.Controls.Input.Toolkit.dll

using System.Collections;
using System.Linq;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

#nullable disable
namespace System.Windows.Controls;

public class SelectorSelectionAdapter : ISelectionAdapter
{
  private Selector _selector;

  private bool IgnoringSelectionChanged { get; set; }

  public Selector SelectorControl
  {
    get => this._selector;
    set
    {
      if (this._selector != null)
      {
        this._selector.SelectionChanged -= new SelectionChangedEventHandler(this.OnSelectionChanged);
        this._selector.MouseLeftButtonUp -= new MouseButtonEventHandler(this.OnSelectorMouseLeftButtonUp);
      }
      this._selector = value;
      if (this._selector == null)
        return;
      this._selector.SelectionChanged += new SelectionChangedEventHandler(this.OnSelectionChanged);
      this._selector.MouseLeftButtonUp += new MouseButtonEventHandler(this.OnSelectorMouseLeftButtonUp);
    }
  }

  public event SelectionChangedEventHandler SelectionChanged;

  public event RoutedEventHandler Commit;

  public event RoutedEventHandler Cancel;

  public SelectorSelectionAdapter()
  {
  }

  public SelectorSelectionAdapter(Selector selector) => this.SelectorControl = selector;

  public object SelectedItem
  {
    get => this.SelectorControl != null ? this.SelectorControl.SelectedItem : (object) null;
    set
    {
      this.IgnoringSelectionChanged = true;
      if (this.SelectorControl != null)
        this.SelectorControl.SelectedItem = value;
      if (value == null)
        this.ResetScrollViewer();
      this.IgnoringSelectionChanged = false;
    }
  }

  public IEnumerable ItemsSource
  {
    get => this.SelectorControl != null ? this.SelectorControl.ItemsSource : (IEnumerable) null;
    set
    {
      if (this.SelectorControl == null)
        return;
      this.SelectorControl.ItemsSource = value;
    }
  }

  private void ResetScrollViewer()
  {
    if (this.SelectorControl == null)
      return;
    this.SelectorControl.GetLogicalChildrenBreadthFirst().OfType<ScrollViewer>().FirstOrDefault<ScrollViewer>()?.ScrollToTop();
  }

  private void OnSelectorMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
  {
    this.OnCommit();
  }

  private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
  {
    if (this.IgnoringSelectionChanged)
      return;
    SelectionChangedEventHandler selectionChanged = this.SelectionChanged;
    if (selectionChanged == null)
      return;
    selectionChanged(sender, e);
  }

  protected void SelectedIndexIncrement()
  {
    if (this.SelectorControl == null)
      return;
    this.SelectorControl.SelectedIndex = this.SelectorControl.SelectedIndex + 1 >= this.SelectorControl.Items.Count ? -1 : this.SelectorControl.SelectedIndex + 1;
  }

  protected void SelectedIndexDecrement()
  {
    if (this.SelectorControl == null)
      return;
    int selectedIndex = this.SelectorControl.SelectedIndex;
    if (selectedIndex >= 0)
    {
      --this.SelectorControl.SelectedIndex;
    }
    else
    {
      if (selectedIndex != -1)
        return;
      this.SelectorControl.SelectedIndex = this.SelectorControl.Items.Count - 1;
    }
  }

  public void HandleKeyDown(KeyEventArgs e)
  {
    switch (e.Key)
    {
      case Key.Return:
        this.OnCommit();
        e.Handled = true;
        break;
      case Key.Escape:
        this.OnCancel();
        e.Handled = true;
        break;
      case Key.Up:
        this.SelectedIndexDecrement();
        e.Handled = true;
        break;
      case Key.Down:
        if ((ModifierKeys.Alt & Keyboard.Modifiers) != ModifierKeys.None)
          break;
        this.SelectedIndexIncrement();
        e.Handled = true;
        break;
    }
  }

  protected virtual void OnCommit() => this.OnCommit((object) this, new RoutedEventArgs());

  private void OnCommit(object sender, RoutedEventArgs e)
  {
    RoutedEventHandler commit = this.Commit;
    if (commit != null)
      commit(sender, e);
    this.AfterAdapterAction();
  }

  protected virtual void OnCancel() => this.OnCancel((object) this, new RoutedEventArgs());

  private void OnCancel(object sender, RoutedEventArgs e)
  {
    RoutedEventHandler cancel = this.Cancel;
    if (cancel != null)
      cancel(sender, e);
    this.AfterAdapterAction();
  }

  private void AfterAdapterAction()
  {
    this.IgnoringSelectionChanged = true;
    if (this.SelectorControl != null)
    {
      this.SelectorControl.SelectedItem = (object) null;
      this.SelectorControl.SelectedIndex = -1;
    }
    this.IgnoringSelectionChanged = false;
  }

  public AutomationPeer CreateAutomationPeer()
  {
    return this._selector == null ? (AutomationPeer) null : UIElementAutomationPeer.CreatePeerForElement((UIElement) this._selector);
  }
}

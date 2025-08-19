// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Primitives.SelectAllSelector
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.Collections.Specialized;
using System.Windows;

#nullable disable
namespace Xceed.Wpf.Toolkit.Primitives;

[TemplatePart(Name = "PART_SelectAllSelectorItem", Type = typeof (SelectAllSelectorItem))]
public class SelectAllSelector : Selector
{
  private const string PART_SelectAllSelectorItem = "PART_SelectAllSelectorItem";
  private SelectAllSelectorItem _selectAllSelecotrItem;
  public static readonly DependencyProperty IsSelectAllActiveProperty = DependencyProperty.Register(nameof (IsSelectAllActive), typeof (bool), typeof (SelectAllSelector), (PropertyMetadata) new UIPropertyMetadata((object) false, new PropertyChangedCallback(SelectAllSelector.OnIsSelectAllActiveChanged)));
  public static readonly DependencyProperty SelectAllContentProperty = DependencyProperty.Register(nameof (SelectAllContent), typeof (object), typeof (SelectAllSelector), (PropertyMetadata) new UIPropertyMetadata((object) "Select All"));

  public bool IsSelectAllActive
  {
    get => (bool) this.GetValue(SelectAllSelector.IsSelectAllActiveProperty);
    set => this.SetValue(SelectAllSelector.IsSelectAllActiveProperty, (object) value);
  }

  private static void OnIsSelectAllActiveChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is SelectAllSelector selectAllSelector))
      return;
    selectAllSelector.OnIsSelectAllActiveChanged((bool) e.OldValue, (bool) e.NewValue);
  }

  protected virtual void OnIsSelectAllActiveChanged(bool oldValue, bool newValue)
  {
    if (!newValue || this.Items.Count <= 0)
      return;
    this.UpdateSelectAllSelectorItem();
  }

  public object SelectAllContent
  {
    get => this.GetValue(SelectAllSelector.SelectAllContentProperty);
    set => this.SetValue(SelectAllSelector.SelectAllContentProperty, value);
  }

  protected override void OnSelectedItemsCollectionChanged(
    object sender,
    NotifyCollectionChangedEventArgs e)
  {
    base.OnSelectedItemsCollectionChanged(sender, e);
    this.UpdateSelectAllSelectorItem();
  }

  protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
  {
    base.OnItemsChanged(e);
    this.UpdateSelectAllSelectorItem();
  }

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    this._selectAllSelecotrItem = this.GetTemplateChild("PART_SelectAllSelectorItem") as SelectAllSelectorItem;
  }

  public void SelectAll()
  {
    foreach (object items in this.ItemsCollection)
    {
      if (!this.SelectedItems.Contains(items))
        this.SelectedItems.Add(items);
    }
  }

  public void UnSelectAll() => this.SelectedItems.Clear();

  private void UpdateSelectAllSelectorItem()
  {
    if (this._selectAllSelecotrItem == null)
      return;
    if (this.Items.Count == this.SelectedItems.Count)
      this._selectAllSelecotrItem.ModifyCurrentSelection(new bool?(true));
    else if (this.SelectedItems.Count > 0)
      this._selectAllSelecotrItem.ModifyCurrentSelection(new bool?());
    else
      this._selectAllSelecotrItem.ModifyCurrentSelection(new bool?(false));
  }
}

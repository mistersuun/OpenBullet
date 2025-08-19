// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.DataGridBoundColumn
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.Windows;
using System.Windows.Data;

#nullable disable
namespace Microsoft.Windows.Controls;

public abstract class DataGridBoundColumn : DataGridColumn
{
  public static readonly DependencyProperty ElementStyleProperty = DependencyProperty.Register(nameof (ElementStyle), typeof (Style), typeof (DataGridBoundColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGridColumn.NotifyPropertyChangeForRefreshContent)));
  public static readonly DependencyProperty EditingElementStyleProperty = DependencyProperty.Register(nameof (EditingElementStyle), typeof (Style), typeof (DataGridBoundColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGridColumn.NotifyPropertyChangeForRefreshContent)));
  private BindingBase _binding;
  private bool _bindingEnsured = true;

  static DataGridBoundColumn()
  {
    DataGridColumn.SortMemberPathProperty.OverrideMetadata(typeof (DataGridBoundColumn), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null, new CoerceValueCallback(DataGridBoundColumn.OnCoerceSortMemberPath)));
  }

  private static object OnCoerceSortMemberPath(DependencyObject d, object baseValue)
  {
    DataGridBoundColumn dataGridBoundColumn = (DataGridBoundColumn) d;
    string str = (string) baseValue;
    if (string.IsNullOrEmpty(str))
      str = DataGridHelper.GetPathFromBinding(dataGridBoundColumn.Binding as System.Windows.Data.Binding);
    return (object) str;
  }

  public virtual BindingBase Binding
  {
    get
    {
      if (!this._bindingEnsured)
      {
        if (!this.IsReadOnly)
          DataGridHelper.EnsureTwoWayIfNotOneWay(this._binding);
        this._bindingEnsured = true;
      }
      return this._binding;
    }
    set
    {
      if (this._binding == value)
        return;
      BindingBase binding = this._binding;
      this._binding = value;
      this.CoerceValue(DataGridColumn.IsReadOnlyProperty);
      this.CoerceValue(DataGridColumn.SortMemberPathProperty);
      this._bindingEnsured = false;
      this.OnBindingChanged(binding, this._binding);
    }
  }

  protected override bool OnCoerceIsReadOnly(bool baseValue)
  {
    return DataGridHelper.IsOneWay(this._binding) || base.OnCoerceIsReadOnly(baseValue);
  }

  protected virtual void OnBindingChanged(BindingBase oldBinding, BindingBase newBinding)
  {
    this.NotifyPropertyChanged("Binding");
  }

  internal void ApplyBinding(DependencyObject target, DependencyProperty property)
  {
    BindingBase binding = this.Binding;
    if (binding != null)
      BindingOperations.SetBinding(target, property, binding);
    else
      BindingOperations.ClearBinding(target, property);
  }

  public Style ElementStyle
  {
    get => (Style) this.GetValue(DataGridBoundColumn.ElementStyleProperty);
    set => this.SetValue(DataGridBoundColumn.ElementStyleProperty, (object) value);
  }

  public Style EditingElementStyle
  {
    get => (Style) this.GetValue(DataGridBoundColumn.EditingElementStyleProperty);
    set => this.SetValue(DataGridBoundColumn.EditingElementStyleProperty, (object) value);
  }

  internal void ApplyStyle(bool isEditing, bool defaultToElementStyle, FrameworkElement element)
  {
    Style style = this.PickStyle(isEditing, defaultToElementStyle);
    if (style == null)
      return;
    element.Style = style;
  }

  private Style PickStyle(bool isEditing, bool defaultToElementStyle)
  {
    Style style = isEditing ? this.EditingElementStyle : this.ElementStyle;
    if (isEditing && defaultToElementStyle && style == null)
      style = this.ElementStyle;
    return style;
  }

  public override BindingBase ClipboardContentBinding
  {
    get => base.ClipboardContentBinding ?? this.Binding;
    set => base.ClipboardContentBinding = value;
  }

  protected internal override void RefreshCellContent(FrameworkElement element, string propertyName)
  {
    if (element is DataGridCell dataGridCell)
    {
      bool isEditing = dataGridCell.IsEditing;
      if (string.Compare(propertyName, "Binding", StringComparison.Ordinal) == 0 || string.Compare(propertyName, "ElementStyle", StringComparison.Ordinal) == 0 && !isEditing || string.Compare(propertyName, "EditingElementStyle", StringComparison.Ordinal) == 0 && isEditing)
      {
        dataGridCell.BuildVisualTree();
        return;
      }
    }
    base.RefreshCellContent(element, propertyName);
  }
}

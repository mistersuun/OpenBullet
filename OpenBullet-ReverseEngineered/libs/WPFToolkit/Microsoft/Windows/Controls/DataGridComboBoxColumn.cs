// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.DataGridComboBoxColumn
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

#nullable disable
namespace Microsoft.Windows.Controls;

public class DataGridComboBoxColumn : DataGridColumn
{
  public static readonly DependencyProperty ElementStyleProperty = DataGridBoundColumn.ElementStyleProperty.AddOwner(typeof (DataGridComboBoxColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) DataGridComboBoxColumn.DefaultElementStyle));
  public static readonly DependencyProperty EditingElementStyleProperty = DataGridBoundColumn.EditingElementStyleProperty.AddOwner(typeof (DataGridComboBoxColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) DataGridComboBoxColumn.DefaultEditingElementStyle));
  public static readonly DependencyProperty ItemsSourceProperty = ItemsControl.ItemsSourceProperty.AddOwner(typeof (DataGridComboBoxColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGridColumn.NotifyPropertyChangeForRefreshContent)));
  public static readonly DependencyProperty DisplayMemberPathProperty = ItemsControl.DisplayMemberPathProperty.AddOwner(typeof (DataGridComboBoxColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) string.Empty, new PropertyChangedCallback(DataGridColumn.NotifyPropertyChangeForRefreshContent)));
  public static readonly DependencyProperty SelectedValuePathProperty = Selector.SelectedValuePathProperty.AddOwner(typeof (DataGridComboBoxColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) string.Empty, new PropertyChangedCallback(DataGridColumn.NotifyPropertyChangeForRefreshContent)));
  private static Style _defaultElementStyle;
  private BindingBase _selectedValueBinding;
  private BindingBase _selectedItemBinding;
  private BindingBase _textBinding;
  private bool _selectedValueBindingEnsured = true;
  private bool _selectedItemBindingEnsured = true;
  private bool _textBindingEnsured = true;

  static DataGridComboBoxColumn()
  {
    DataGridColumn.SortMemberPathProperty.OverrideMetadata(typeof (DataGridComboBoxColumn), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null, new CoerceValueCallback(DataGridComboBoxColumn.OnCoerceSortMemberPath)));
  }

  private static object OnCoerceSortMemberPath(DependencyObject d, object baseValue)
  {
    DataGridComboBoxColumn gridComboBoxColumn = (DataGridComboBoxColumn) d;
    string str = (string) baseValue;
    if (string.IsNullOrEmpty(str))
      str = DataGridHelper.GetPathFromBinding(gridComboBoxColumn.EffectiveBinding as Binding);
    return (object) str;
  }

  private BindingBase EffectiveBinding
  {
    get
    {
      if (this.SelectedItemBinding != null)
        return this.SelectedItemBinding;
      return this.SelectedValueBinding != null ? this.SelectedValueBinding : this.TextBinding;
    }
  }

  public virtual BindingBase SelectedValueBinding
  {
    get
    {
      if (!this._selectedValueBindingEnsured)
      {
        if (!this.IsReadOnly)
          DataGridHelper.EnsureTwoWayIfNotOneWay(this._selectedValueBinding);
        this._selectedValueBindingEnsured = true;
      }
      return this._selectedValueBinding;
    }
    set
    {
      if (this._selectedValueBinding == value)
        return;
      BindingBase selectedValueBinding = this._selectedValueBinding;
      this._selectedValueBinding = value;
      this.CoerceValue(DataGridColumn.IsReadOnlyProperty);
      this.CoerceValue(DataGridColumn.SortMemberPathProperty);
      this._selectedValueBindingEnsured = false;
      this.OnSelectedValueBindingChanged(selectedValueBinding, this._selectedValueBinding);
    }
  }

  protected override bool OnCoerceIsReadOnly(bool baseValue)
  {
    return DataGridHelper.IsOneWay(this.EffectiveBinding) || base.OnCoerceIsReadOnly(baseValue);
  }

  public virtual BindingBase SelectedItemBinding
  {
    get
    {
      if (!this._selectedItemBindingEnsured)
      {
        if (!this.IsReadOnly)
          DataGridHelper.EnsureTwoWayIfNotOneWay(this._selectedItemBinding);
        this._selectedItemBindingEnsured = true;
      }
      return this._selectedItemBinding;
    }
    set
    {
      if (this._selectedItemBinding == value)
        return;
      BindingBase selectedItemBinding = this._selectedItemBinding;
      this._selectedItemBinding = value;
      this.CoerceValue(DataGridColumn.IsReadOnlyProperty);
      this.CoerceValue(DataGridColumn.SortMemberPathProperty);
      this._selectedItemBindingEnsured = false;
      this.OnSelectedItemBindingChanged(selectedItemBinding, this._selectedItemBinding);
    }
  }

  public virtual BindingBase TextBinding
  {
    get
    {
      if (!this._textBindingEnsured)
      {
        if (!this.IsReadOnly)
          DataGridHelper.EnsureTwoWayIfNotOneWay(this._textBinding);
        this._textBindingEnsured = true;
      }
      return this._textBinding;
    }
    set
    {
      if (this._textBinding == value)
        return;
      BindingBase textBinding = this._textBinding;
      this._textBinding = value;
      this.CoerceValue(DataGridColumn.IsReadOnlyProperty);
      this.CoerceValue(DataGridColumn.SortMemberPathProperty);
      this._textBindingEnsured = false;
      this.OnTextBindingChanged(textBinding, this._textBinding);
    }
  }

  protected virtual void OnSelectedValueBindingChanged(
    BindingBase oldBinding,
    BindingBase newBinding)
  {
    this.NotifyPropertyChanged("SelectedValueBinding");
  }

  protected virtual void OnSelectedItemBindingChanged(
    BindingBase oldBinding,
    BindingBase newBinding)
  {
    this.NotifyPropertyChanged("SelectedItemBinding");
  }

  protected virtual void OnTextBindingChanged(BindingBase oldBinding, BindingBase newBinding)
  {
    this.NotifyPropertyChanged("TextBinding");
  }

  public static Style DefaultElementStyle
  {
    get
    {
      if (DataGridComboBoxColumn._defaultElementStyle == null)
      {
        Style style = new Style(typeof (ComboBox));
        style.Setters.Add((SetterBase) new Setter(Selector.IsSynchronizedWithCurrentItemProperty, (object) false));
        style.Seal();
        DataGridComboBoxColumn._defaultElementStyle = style;
      }
      return DataGridComboBoxColumn._defaultElementStyle;
    }
  }

  public static Style DefaultEditingElementStyle => DataGridComboBoxColumn.DefaultElementStyle;

  public Style ElementStyle
  {
    get => (Style) this.GetValue(DataGridComboBoxColumn.ElementStyleProperty);
    set => this.SetValue(DataGridComboBoxColumn.ElementStyleProperty, (object) value);
  }

  public Style EditingElementStyle
  {
    get => (Style) this.GetValue(DataGridComboBoxColumn.EditingElementStyleProperty);
    set => this.SetValue(DataGridComboBoxColumn.EditingElementStyleProperty, (object) value);
  }

  private void ApplyStyle(bool isEditing, bool defaultToElementStyle, FrameworkElement element)
  {
    Style style = this.PickStyle(isEditing, defaultToElementStyle);
    if (style == null)
      return;
    element.Style = style;
  }

  internal void ApplyStyle(
    bool isEditing,
    bool defaultToElementStyle,
    FrameworkContentElement element)
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

  private static void ApplyBinding(
    BindingBase binding,
    DependencyObject target,
    DependencyProperty property)
  {
    if (binding != null)
      BindingOperations.SetBinding(target, property, binding);
    else
      BindingOperations.ClearBinding(target, property);
  }

  public override BindingBase ClipboardContentBinding
  {
    get => base.ClipboardContentBinding ?? this.EffectiveBinding;
    set => base.ClipboardContentBinding = value;
  }

  public IEnumerable ItemsSource
  {
    get => (IEnumerable) this.GetValue(DataGridComboBoxColumn.ItemsSourceProperty);
    set => this.SetValue(DataGridComboBoxColumn.ItemsSourceProperty, (object) value);
  }

  public string DisplayMemberPath
  {
    get => (string) this.GetValue(DataGridComboBoxColumn.DisplayMemberPathProperty);
    set => this.SetValue(DataGridComboBoxColumn.DisplayMemberPathProperty, (object) value);
  }

  public string SelectedValuePath
  {
    get => (string) this.GetValue(DataGridComboBoxColumn.SelectedValuePathProperty);
    set => this.SetValue(DataGridComboBoxColumn.SelectedValuePathProperty, (object) value);
  }

  protected internal override void RefreshCellContent(FrameworkElement element, string propertyName)
  {
    if (element is DataGridCell dataGridCell)
    {
      bool isEditing = dataGridCell.IsEditing;
      if (string.Compare(propertyName, "ElementStyle", StringComparison.Ordinal) == 0 && !isEditing || string.Compare(propertyName, "EditingElementStyle", StringComparison.Ordinal) == 0 && isEditing)
      {
        dataGridCell.BuildVisualTree();
      }
      else
      {
        ComboBox content = dataGridCell.Content as ComboBox;
        switch (propertyName)
        {
          case "SelectedItemBinding":
            DataGridComboBoxColumn.ApplyBinding(this.SelectedItemBinding, (DependencyObject) content, Selector.SelectedItemProperty);
            break;
          case "SelectedValueBinding":
            DataGridComboBoxColumn.ApplyBinding(this.SelectedValueBinding, (DependencyObject) content, Selector.SelectedValueProperty);
            break;
          case "TextBinding":
            DataGridComboBoxColumn.ApplyBinding(this.TextBinding, (DependencyObject) content, ComboBox.TextProperty);
            break;
          case "SelectedValuePath":
            DataGridHelper.SyncColumnProperty((DependencyObject) this, (DependencyObject) content, Selector.SelectedValuePathProperty, DataGridComboBoxColumn.SelectedValuePathProperty);
            break;
          case "DisplayMemberPath":
            DataGridHelper.SyncColumnProperty((DependencyObject) this, (DependencyObject) content, ItemsControl.DisplayMemberPathProperty, DataGridComboBoxColumn.DisplayMemberPathProperty);
            break;
          case "ItemsSource":
            DataGridHelper.SyncColumnProperty((DependencyObject) this, (DependencyObject) content, ItemsControl.ItemsSourceProperty, DataGridComboBoxColumn.ItemsSourceProperty);
            break;
          default:
            base.RefreshCellContent(element, propertyName);
            break;
        }
      }
    }
    else
      base.RefreshCellContent(element, propertyName);
  }

  private object GetComboBoxSelectionValue(ComboBox comboBox)
  {
    if (this.SelectedItemBinding != null)
      return comboBox.SelectedItem;
    return this.SelectedValueBinding != null ? comboBox.SelectedValue : (object) comboBox.Text;
  }

  protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
  {
    DataGridComboBoxColumn.TextBlockComboBox element = new DataGridComboBoxColumn.TextBlockComboBox();
    this.ApplyStyle(false, false, (FrameworkElement) element);
    this.ApplyColumnProperties((ComboBox) element);
    return (FrameworkElement) element;
  }

  protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
  {
    ComboBox editingElement = new ComboBox();
    this.ApplyStyle(true, false, (FrameworkElement) editingElement);
    this.ApplyColumnProperties(editingElement);
    return (FrameworkElement) editingElement;
  }

  private void ApplyColumnProperties(ComboBox comboBox)
  {
    DataGridComboBoxColumn.ApplyBinding(this.SelectedItemBinding, (DependencyObject) comboBox, Selector.SelectedItemProperty);
    DataGridComboBoxColumn.ApplyBinding(this.SelectedValueBinding, (DependencyObject) comboBox, Selector.SelectedValueProperty);
    DataGridComboBoxColumn.ApplyBinding(this.TextBinding, (DependencyObject) comboBox, ComboBox.TextProperty);
    DataGridHelper.SyncColumnProperty((DependencyObject) this, (DependencyObject) comboBox, Selector.SelectedValuePathProperty, DataGridComboBoxColumn.SelectedValuePathProperty);
    DataGridHelper.SyncColumnProperty((DependencyObject) this, (DependencyObject) comboBox, ItemsControl.DisplayMemberPathProperty, DataGridComboBoxColumn.DisplayMemberPathProperty);
    DataGridHelper.SyncColumnProperty((DependencyObject) this, (DependencyObject) comboBox, ItemsControl.ItemsSourceProperty, DataGridComboBoxColumn.ItemsSourceProperty);
  }

  protected override object PrepareCellForEdit(
    FrameworkElement editingElement,
    RoutedEventArgs editingEventArgs)
  {
    if (!(editingElement is ComboBox comboBox))
      return (object) null;
    comboBox.Focus();
    object boxSelectionValue = this.GetComboBoxSelectionValue(comboBox);
    if (DataGridComboBoxColumn.IsComboBoxOpeningInputEvent(editingEventArgs))
      comboBox.IsDropDownOpen = true;
    return boxSelectionValue;
  }

  protected override bool CommitCellEdit(FrameworkElement editingElement)
  {
    if (!(editingElement is ComboBox element))
      return true;
    DataGridHelper.UpdateSource((FrameworkElement) element, Selector.SelectedValueProperty);
    DataGridHelper.UpdateSource((FrameworkElement) element, Selector.SelectedItemProperty);
    DataGridHelper.UpdateSource((FrameworkElement) element, ComboBox.TextProperty);
    return !Validation.GetHasError((DependencyObject) element);
  }

  protected override void CancelCellEdit(FrameworkElement editingElement, object uneditedValue)
  {
    if (!(editingElement is ComboBox element))
      return;
    DataGridHelper.UpdateTarget((FrameworkElement) element, Selector.SelectedValueProperty);
    DataGridHelper.UpdateTarget((FrameworkElement) element, Selector.SelectedItemProperty);
    DataGridHelper.UpdateTarget((FrameworkElement) element, ComboBox.TextProperty);
  }

  internal override void OnInput(InputEventArgs e)
  {
    if (!DataGridComboBoxColumn.IsComboBoxOpeningInputEvent((RoutedEventArgs) e))
      return;
    this.BeginEdit(e);
  }

  private static bool IsComboBoxOpeningInputEvent(RoutedEventArgs e)
  {
    if (!(e is KeyEventArgs keyEventArgs) || (keyEventArgs.KeyStates & KeyStates.Down) != KeyStates.Down)
      return false;
    bool flag = (keyEventArgs.KeyboardDevice.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt;
    Key key = keyEventArgs.Key;
    if (key == Key.System)
      key = keyEventArgs.SystemKey;
    if (key == Key.F4 && !flag)
      return true;
    return (key == Key.Up || key == Key.Down) && flag;
  }

  internal class TextBlockComboBox : ComboBox
  {
    static TextBlockComboBox()
    {
      FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (DataGridComboBoxColumn.TextBlockComboBox), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (DataGridComboBoxColumn.TextBlockComboBox)));
      KeyboardNavigation.IsTabStopProperty.OverrideMetadata(typeof (DataGridComboBoxColumn.TextBlockComboBox), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
      FrameworkElement.DataContextProperty.OverrideMetadata(typeof (DataGridComboBoxColumn.TextBlockComboBox), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(DataGridComboBoxColumn.TextBlockComboBox.OnDataContextPropertyChanged)));
    }

    private static void OnDataContextPropertyChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      DataGridComboBoxColumn.TextBlockComboBox target = (DataGridComboBoxColumn.TextBlockComboBox) d;
      if (DependencyPropertyHelper.GetValueSource((DependencyObject) target, Selector.SelectedItemProperty).BaseValueSource == BaseValueSource.Local)
      {
        BindingBase bindingBase = BindingOperations.GetBindingBase((DependencyObject) target, Selector.SelectedItemProperty);
        if (bindingBase == null)
          return;
        target.ClearValue(Selector.SelectedItemProperty);
        DataGridComboBoxColumn.ApplyBinding(bindingBase, (DependencyObject) target, Selector.SelectedItemProperty);
      }
      else
      {
        target.SelectedItem = (object) null;
        target.ClearValue(Selector.SelectedItemProperty);
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.DataGridCheckBoxColumn
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

#nullable disable
namespace Microsoft.Windows.Controls;

public class DataGridCheckBoxColumn : DataGridBoundColumn
{
  public static readonly DependencyProperty IsThreeStateProperty = ToggleButton.IsThreeStateProperty.AddOwner(typeof (DataGridCheckBoxColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(DataGridColumn.NotifyPropertyChangeForRefreshContent)));
  private static Style _defaultElementStyle;
  private static Style _defaultEditingElementStyle;

  static DataGridCheckBoxColumn()
  {
    DataGridBoundColumn.ElementStyleProperty.OverrideMetadata(typeof (DataGridCheckBoxColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) DataGridCheckBoxColumn.DefaultElementStyle));
    DataGridBoundColumn.EditingElementStyleProperty.OverrideMetadata(typeof (DataGridCheckBoxColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) DataGridCheckBoxColumn.DefaultEditingElementStyle));
  }

  public static Style DefaultElementStyle
  {
    get
    {
      if (DataGridCheckBoxColumn._defaultElementStyle == null)
      {
        Style style = new Style(typeof (CheckBox));
        style.Setters.Add((SetterBase) new Setter(UIElement.IsHitTestVisibleProperty, (object) false));
        style.Setters.Add((SetterBase) new Setter(UIElement.FocusableProperty, (object) false));
        style.Setters.Add((SetterBase) new Setter(FrameworkElement.HorizontalAlignmentProperty, (object) HorizontalAlignment.Center));
        style.Setters.Add((SetterBase) new Setter(FrameworkElement.VerticalAlignmentProperty, (object) VerticalAlignment.Top));
        style.Seal();
        DataGridCheckBoxColumn._defaultElementStyle = style;
      }
      return DataGridCheckBoxColumn._defaultElementStyle;
    }
  }

  public static Style DefaultEditingElementStyle
  {
    get
    {
      if (DataGridCheckBoxColumn._defaultEditingElementStyle == null)
      {
        Style style = new Style(typeof (CheckBox));
        style.Setters.Add((SetterBase) new Setter(FrameworkElement.HorizontalAlignmentProperty, (object) HorizontalAlignment.Center));
        style.Setters.Add((SetterBase) new Setter(FrameworkElement.VerticalAlignmentProperty, (object) VerticalAlignment.Top));
        style.Seal();
        DataGridCheckBoxColumn._defaultEditingElementStyle = style;
      }
      return DataGridCheckBoxColumn._defaultEditingElementStyle;
    }
  }

  protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
  {
    return (FrameworkElement) this.GenerateCheckBox(false, cell);
  }

  protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
  {
    return (FrameworkElement) this.GenerateCheckBox(true, cell);
  }

  private CheckBox GenerateCheckBox(bool isEditing, DataGridCell cell)
  {
    CheckBox checkBox = (cell != null ? cell.Content as CheckBox : (CheckBox) null) ?? new CheckBox();
    checkBox.IsThreeState = this.IsThreeState;
    this.ApplyStyle(isEditing, true, (FrameworkElement) checkBox);
    this.ApplyBinding((DependencyObject) checkBox, ToggleButton.IsCheckedProperty);
    return checkBox;
  }

  protected internal override void RefreshCellContent(FrameworkElement element, string propertyName)
  {
    if (element is DataGridCell dataGridCell && string.Compare(propertyName, "IsThreeState", StringComparison.Ordinal) == 0)
    {
      if (!(dataGridCell.Content is CheckBox content))
        return;
      content.IsThreeState = this.IsThreeState;
    }
    else
      base.RefreshCellContent(element, propertyName);
  }

  public bool IsThreeState
  {
    get => (bool) this.GetValue(DataGridCheckBoxColumn.IsThreeStateProperty);
    set => this.SetValue(DataGridCheckBoxColumn.IsThreeStateProperty, (object) value);
  }

  protected override object PrepareCellForEdit(
    FrameworkElement editingElement,
    RoutedEventArgs editingEventArgs)
  {
    if (!(editingElement is CheckBox checkBox1))
      return (object) new bool?(false);
    checkBox1.Focus();
    bool? isChecked = checkBox1.IsChecked;
    if (DataGridCheckBoxColumn.IsMouseLeftButtonDown(editingEventArgs) && DataGridCheckBoxColumn.IsMouseOver(checkBox1, editingEventArgs) || DataGridCheckBoxColumn.IsSpaceKeyDown(editingEventArgs))
    {
      CheckBox checkBox2 = checkBox1;
      bool? nullable1 = isChecked;
      bool? nullable2 = new bool?(!nullable1.GetValueOrDefault() || !nullable1.HasValue);
      checkBox2.IsChecked = nullable2;
    }
    return (object) isChecked;
  }

  protected override bool CommitCellEdit(FrameworkElement editingElement)
  {
    if (!(editingElement is CheckBox element))
      return true;
    DataGridHelper.UpdateSource((FrameworkElement) element, ToggleButton.IsCheckedProperty);
    return !Validation.GetHasError((DependencyObject) element);
  }

  protected override void CancelCellEdit(FrameworkElement editingElement, object uneditedValue)
  {
    if (!(editingElement is CheckBox element))
      return;
    DataGridHelper.UpdateTarget((FrameworkElement) element, ToggleButton.IsCheckedProperty);
  }

  internal override void OnInput(InputEventArgs e)
  {
    if (!DataGridCheckBoxColumn.IsSpaceKeyDown((RoutedEventArgs) e))
      return;
    this.BeginEdit(e);
  }

  private static bool IsMouseLeftButtonDown(RoutedEventArgs e)
  {
    return e is MouseButtonEventArgs mouseButtonEventArgs && mouseButtonEventArgs.ChangedButton == MouseButton.Left && mouseButtonEventArgs.ButtonState == MouseButtonState.Pressed;
  }

  private static bool IsMouseOver(CheckBox checkBox, RoutedEventArgs e)
  {
    return checkBox.InputHitTest(((MouseEventArgs) e).GetPosition((IInputElement) checkBox)) != null;
  }

  private static bool IsSpaceKeyDown(RoutedEventArgs e)
  {
    return e is KeyEventArgs keyEventArgs && (keyEventArgs.KeyStates & KeyStates.Down) == KeyStates.Down && keyEventArgs.Key == Key.Space;
  }
}

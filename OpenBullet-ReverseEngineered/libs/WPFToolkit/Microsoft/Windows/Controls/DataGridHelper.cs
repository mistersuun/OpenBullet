// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.DataGridHelper
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using Microsoft.Windows.Controls.Primitives;
using MS.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

#nullable disable
namespace Microsoft.Windows.Controls;

internal static class DataGridHelper
{
  private const char _escapeChar = '\u001B';
  private static WeakHashtable _propertyTransferEnabledMap = new WeakHashtable();
  private static readonly DependencyProperty ThemeProperty = DependencyProperty.RegisterAttached("Theme", typeof (string), typeof (DataGridHelper), (PropertyMetadata) new FrameworkPropertyMetadata((object) string.Empty));
  private static ComponentResourceKey _themeKey = new ComponentResourceKey(typeof (DataGrid), (object) "Theme");

  public static Size SubtractFromSize(Size size, double thickness, bool height)
  {
    return height ? new Size(size.Width, Math.Max(0.0, size.Height - thickness)) : new Size(Math.Max(0.0, size.Width - thickness), size.Height);
  }

  public static bool IsGridLineVisible(DataGrid dataGrid, bool isHorizontal)
  {
    if (dataGrid != null)
    {
      switch (dataGrid.GridLinesVisibility)
      {
        case DataGridGridLinesVisibility.All:
          return true;
        case DataGridGridLinesVisibility.Horizontal:
          return isHorizontal;
        case DataGridGridLinesVisibility.None:
          return false;
        case DataGridGridLinesVisibility.Vertical:
          return !isHorizontal;
      }
    }
    return false;
  }

  public static bool ShouldNotifyCells(NotificationTarget target)
  {
    return DataGridHelper.TestTarget(target, NotificationTarget.Cells);
  }

  public static bool ShouldNotifyCellsPresenter(NotificationTarget target)
  {
    return DataGridHelper.TestTarget(target, NotificationTarget.CellsPresenter);
  }

  public static bool ShouldNotifyColumns(NotificationTarget target)
  {
    return DataGridHelper.TestTarget(target, NotificationTarget.Columns);
  }

  public static bool ShouldNotifyColumnHeaders(NotificationTarget target)
  {
    return DataGridHelper.TestTarget(target, NotificationTarget.ColumnHeaders);
  }

  public static bool ShouldNotifyColumnHeadersPresenter(NotificationTarget target)
  {
    return DataGridHelper.TestTarget(target, NotificationTarget.ColumnHeadersPresenter);
  }

  public static bool ShouldNotifyColumnCollection(NotificationTarget target)
  {
    return DataGridHelper.TestTarget(target, NotificationTarget.ColumnCollection);
  }

  public static bool ShouldNotifyDataGrid(NotificationTarget target)
  {
    return DataGridHelper.TestTarget(target, NotificationTarget.DataGrid);
  }

  public static bool ShouldNotifyDetailsPresenter(NotificationTarget target)
  {
    return DataGridHelper.TestTarget(target, NotificationTarget.DetailsPresenter);
  }

  public static bool ShouldRefreshCellContent(NotificationTarget target)
  {
    return DataGridHelper.TestTarget(target, NotificationTarget.RefreshCellContent);
  }

  public static bool ShouldNotifyRowHeaders(NotificationTarget target)
  {
    return DataGridHelper.TestTarget(target, NotificationTarget.RowHeaders);
  }

  public static bool ShouldNotifyRows(NotificationTarget target)
  {
    return DataGridHelper.TestTarget(target, NotificationTarget.Rows);
  }

  public static bool ShouldNotifyRowSubtree(NotificationTarget target)
  {
    NotificationTarget notificationTarget = NotificationTarget.Cells | NotificationTarget.CellsPresenter | NotificationTarget.DetailsPresenter | NotificationTarget.RefreshCellContent | NotificationTarget.RowHeaders | NotificationTarget.Rows;
    return DataGridHelper.TestTarget(target, notificationTarget);
  }

  private static bool TestTarget(NotificationTarget target, NotificationTarget value)
  {
    return (target & value) != NotificationTarget.None;
  }

  public static T FindParent<T>(FrameworkElement element) where T : FrameworkElement
  {
    for (FrameworkElement templatedParent = element.TemplatedParent as FrameworkElement; templatedParent != null; templatedParent = templatedParent.TemplatedParent as FrameworkElement)
    {
      if (templatedParent is T parent)
        return parent;
    }
    return default (T);
  }

  public static T FindVisualParent<T>(UIElement element) where T : UIElement
  {
    for (UIElement reference = element; reference != null; reference = VisualTreeHelper.GetParent((DependencyObject) reference) as UIElement)
    {
      if (reference is T visualParent)
        return visualParent;
    }
    return default (T);
  }

  public static bool TreeHasFocusAndTabStop(DependencyObject element)
  {
    switch (element)
    {
      case null:
        return false;
      case UIElement element1:
        if (element1.Focusable && KeyboardNavigation.GetIsTabStop((DependencyObject) element1))
          return true;
        break;
      case ContentElement element2:
        if (element2.Focusable && KeyboardNavigation.GetIsTabStop((DependencyObject) element2))
          return true;
        break;
    }
    int childrenCount = VisualTreeHelper.GetChildrenCount(element);
    for (int childIndex = 0; childIndex < childrenCount; ++childIndex)
    {
      if (DataGridHelper.TreeHasFocusAndTabStop(VisualTreeHelper.GetChild(element, childIndex)))
        return true;
    }
    return false;
  }

  public static void OnColumnWidthChanged(
    IProvideDataGridColumn cell,
    DependencyPropertyChangedEventArgs e)
  {
    UIElement uiElement = (UIElement) cell;
    DataGridColumn column = cell.Column;
    bool isHeader = cell is DataGridColumnHeader;
    if (column == null)
      return;
    DataGridLength width = column.Width;
    if (!width.IsAuto && (isHeader || !width.IsSizeToCells) && (!isHeader || !width.IsSizeToHeader))
      return;
    DataGridLength oldValue = (DataGridLength) e.OldValue;
    double desiredValue;
    if (oldValue.UnitType != width.UnitType)
    {
      double constraintWidth = column.GetConstraintWidth(isHeader);
      if (!DoubleUtil.AreClose(uiElement.DesiredSize.Width, constraintWidth))
      {
        uiElement.InvalidateMeasure();
        uiElement.Measure(new Size(constraintWidth, double.PositiveInfinity));
      }
      desiredValue = uiElement.DesiredSize.Width;
    }
    else
      desiredValue = oldValue.DesiredValue;
    if (!DoubleUtil.IsNaN(width.DesiredValue) && !DoubleUtil.LessThan(width.DesiredValue, desiredValue))
      return;
    column.SetWidthInternal(new DataGridLength(width.Value, width.UnitType, desiredValue, width.DisplayValue));
  }

  public static Geometry GetFrozenClipForCell(IProvideDataGridColumn cell)
  {
    return DataGridHelper.GetParentPanelForCell(cell)?.GetFrozenClipForChild((UIElement) cell);
  }

  public static DataGridCellsPanel GetParentPanelForCell(IProvideDataGridColumn cell)
  {
    return VisualTreeHelper.GetParent((DependencyObject) cell) as DataGridCellsPanel;
  }

  public static double GetParentCellsPanelHorizontalOffset(IProvideDataGridColumn cell)
  {
    DataGridCellsPanel parentPanelForCell = DataGridHelper.GetParentPanelForCell(cell);
    return parentPanelForCell != null ? parentPanelForCell.ComputeCellsPanelHorizontalOffset() : 0.0;
  }

  public static bool IsDefaultValue(DependencyObject d, DependencyProperty dp)
  {
    return DependencyPropertyHelper.GetValueSource(d, dp).BaseValueSource == BaseValueSource.Default;
  }

  public static object GetCoercedTransferPropertyValue(
    DependencyObject baseObject,
    object baseValue,
    DependencyProperty baseProperty,
    DependencyObject parentObject,
    DependencyProperty parentProperty)
  {
    return DataGridHelper.GetCoercedTransferPropertyValue(baseObject, baseValue, baseProperty, parentObject, parentProperty, (DependencyObject) null, (DependencyProperty) null);
  }

  public static object GetCoercedTransferPropertyValue(
    DependencyObject baseObject,
    object baseValue,
    DependencyProperty baseProperty,
    DependencyObject parentObject,
    DependencyProperty parentProperty,
    DependencyObject grandParentObject,
    DependencyProperty grandParentProperty)
  {
    object transferPropertyValue = baseValue;
    if (DataGridHelper.IsPropertyTransferEnabled(baseObject, baseProperty))
    {
      BaseValueSource baseValueSource1 = DependencyPropertyHelper.GetValueSource(baseObject, baseProperty).BaseValueSource;
      if (parentObject != null)
      {
        ValueSource valueSource = DependencyPropertyHelper.GetValueSource(parentObject, parentProperty);
        if (valueSource.BaseValueSource > baseValueSource1)
        {
          transferPropertyValue = parentObject.GetValue(parentProperty);
          baseValueSource1 = valueSource.BaseValueSource;
        }
      }
      if (grandParentObject != null)
      {
        ValueSource valueSource = DependencyPropertyHelper.GetValueSource(grandParentObject, grandParentProperty);
        if (valueSource.BaseValueSource > baseValueSource1)
        {
          transferPropertyValue = grandParentObject.GetValue(grandParentProperty);
          BaseValueSource baseValueSource2 = valueSource.BaseValueSource;
        }
      }
    }
    return transferPropertyValue;
  }

  public static void TransferProperty(DependencyObject d, DependencyProperty p)
  {
    Dictionary<DependencyProperty, bool> enabledMapForObject = DataGridHelper.GetPropertyTransferEnabledMapForObject(d);
    enabledMapForObject[p] = true;
    d.CoerceValue(p);
    enabledMapForObject[p] = false;
  }

  private static Dictionary<DependencyProperty, bool> GetPropertyTransferEnabledMapForObject(
    DependencyObject d)
  {
    if (!(DataGridHelper._propertyTransferEnabledMap[(object) d] is Dictionary<DependencyProperty, bool> enabledMapForObject))
    {
      enabledMapForObject = new Dictionary<DependencyProperty, bool>();
      DataGridHelper._propertyTransferEnabledMap.SetWeak((object) d, (object) enabledMapForObject);
    }
    return enabledMapForObject;
  }

  internal static bool IsPropertyTransferEnabled(DependencyObject d, DependencyProperty p)
  {
    bool flag;
    return DataGridHelper._propertyTransferEnabledMap[(object) d] is Dictionary<DependencyProperty, bool> propertyTransferEnabled && propertyTransferEnabled.TryGetValue(p, out flag) && flag;
  }

  public static string GetTheme(FrameworkElement element)
  {
    if (element.ReadLocalValue(DataGridHelper.ThemeProperty) == DependencyProperty.UnsetValue)
      element.SetResourceReference(DataGridHelper.ThemeProperty, (object) DataGridHelper._themeKey);
    return (string) element.GetValue(DataGridHelper.ThemeProperty);
  }

  public static void HookThemeChange(Type type, PropertyChangedCallback propertyChangedCallback)
  {
    DataGridHelper.ThemeProperty.OverrideMetadata(type, (PropertyMetadata) new FrameworkPropertyMetadata((object) string.Empty, propertyChangedCallback));
  }

  internal static bool IsOneWay(BindingBase bindingBase)
  {
    switch (bindingBase)
    {
      case null:
        return false;
      case Binding binding:
        return binding.Mode == BindingMode.OneWay;
      case MultiBinding multiBinding:
        return multiBinding.Mode == BindingMode.OneWay;
      case PriorityBinding priorityBinding:
        Collection<BindingBase> bindings = priorityBinding.Bindings;
        int count = bindings.Count;
        for (int index = 0; index < count; ++index)
        {
          if (DataGridHelper.IsOneWay(bindings[index]))
            return true;
        }
        break;
    }
    return false;
  }

  internal static void EnsureTwoWayIfNotOneWay(BindingBase bindingBase)
  {
    switch (bindingBase)
    {
      case Binding binding:
        if (binding.Mode == BindingMode.OneWay)
          break;
        if (binding.Mode != BindingMode.TwoWay)
          binding.Mode = BindingMode.TwoWay;
        if (binding.UpdateSourceTrigger == UpdateSourceTrigger.Explicit)
          break;
        binding.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;
        break;
      case MultiBinding multiBinding:
        if (multiBinding.Mode == BindingMode.OneWay)
          break;
        if (multiBinding.Mode != BindingMode.TwoWay)
          multiBinding.Mode = BindingMode.TwoWay;
        if (multiBinding.UpdateSourceTrigger == UpdateSourceTrigger.Explicit)
          break;
        multiBinding.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;
        break;
      case PriorityBinding priorityBinding:
        Collection<BindingBase> bindings = priorityBinding.Bindings;
        int count = bindings.Count;
        for (int index = 0; index < count; ++index)
          DataGridHelper.EnsureTwoWayIfNotOneWay(bindings[index]);
        break;
    }
  }

  internal static BindingExpression GetBindingExpression(
    FrameworkElement element,
    DependencyProperty dp)
  {
    return element?.GetBindingExpression(dp);
  }

  internal static void UpdateSource(FrameworkElement element, DependencyProperty dp)
  {
    DataGridHelper.GetBindingExpression(element, dp)?.UpdateSource();
  }

  internal static void UpdateTarget(FrameworkElement element, DependencyProperty dp)
  {
    DataGridHelper.GetBindingExpression(element, dp)?.UpdateTarget();
  }

  internal static void SyncColumnProperty(
    DependencyObject column,
    DependencyObject content,
    DependencyProperty contentProperty,
    DependencyProperty columnProperty)
  {
    if (DataGridHelper.IsDefaultValue(column, columnProperty))
      content.ClearValue(contentProperty);
    else
      content.SetValue(contentProperty, column.GetValue(columnProperty));
  }

  internal static string GetPathFromBinding(Binding binding)
  {
    if (binding != null)
    {
      if (!string.IsNullOrEmpty(binding.XPath))
        return binding.XPath;
      if (binding.Path != null)
        return binding.Path.Path;
    }
    return (string) null;
  }

  public static bool AreRowHeadersVisible(DataGridHeadersVisibility headersVisibility)
  {
    return (headersVisibility & DataGridHeadersVisibility.Row) == DataGridHeadersVisibility.Row;
  }

  public static double CoerceToMinMax(double value, double minValue, double maxValue)
  {
    value = Math.Max(value, minValue);
    value = Math.Min(value, maxValue);
    return value;
  }

  public static bool HasNonEscapeCharacters(TextCompositionEventArgs textArgs)
  {
    if (textArgs != null)
    {
      string text = textArgs.Text;
      int index = 0;
      for (int length = text.Length; index < length; ++index)
      {
        if (text[index] != '\u001B')
          return true;
      }
    }
    return false;
  }
}

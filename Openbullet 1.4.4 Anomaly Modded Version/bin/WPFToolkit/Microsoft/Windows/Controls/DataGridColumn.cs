// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.DataGridColumn
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using MS.Internal;
using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

#nullable disable
namespace Microsoft.Windows.Controls;

public abstract class DataGridColumn : DependencyObject
{
  private const double _starMaxWidth = 10000.0;
  public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof (Header), typeof (object), typeof (DataGridColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGridColumn.OnNotifyColumnHeaderPropertyChanged)));
  public static readonly DependencyProperty HeaderStyleProperty = DependencyProperty.Register(nameof (HeaderStyle), typeof (Style), typeof (DataGridColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGridColumn.OnNotifyColumnHeaderPropertyChanged), new CoerceValueCallback(DataGridColumn.OnCoerceHeaderStyle)));
  public static readonly DependencyProperty HeaderStringFormatProperty = DependencyProperty.Register(nameof (HeaderStringFormat), typeof (string), typeof (DataGridColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGridColumn.OnNotifyColumnHeaderPropertyChanged)));
  public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(nameof (HeaderTemplate), typeof (DataTemplate), typeof (DataGridColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGridColumn.OnNotifyColumnHeaderPropertyChanged)));
  public static readonly DependencyProperty HeaderTemplateSelectorProperty = DependencyProperty.Register(nameof (HeaderTemplateSelector), typeof (DataTemplateSelector), typeof (DataGridColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGridColumn.OnNotifyColumnHeaderPropertyChanged)));
  public static readonly DependencyProperty CellStyleProperty = DependencyProperty.Register(nameof (CellStyle), typeof (Style), typeof (DataGridColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGridColumn.OnNotifyCellPropertyChanged), new CoerceValueCallback(DataGridColumn.OnCoerceCellStyle)));
  public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof (IsReadOnly), typeof (bool), typeof (DataGridColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(DataGridColumn.OnNotifyCellPropertyChanged), new CoerceValueCallback(DataGridColumn.OnCoerceIsReadOnly)));
  public static readonly DependencyProperty WidthProperty = DependencyProperty.Register(nameof (Width), typeof (DataGridLength), typeof (DataGridColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) DataGridLength.Auto, new PropertyChangedCallback(DataGridColumn.OnWidthPropertyChanged), new CoerceValueCallback(DataGridColumn.OnCoerceWidth)));
  public static readonly DependencyProperty MinWidthProperty = DependencyProperty.Register(nameof (MinWidth), typeof (double), typeof (DataGridColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) 20.0, new PropertyChangedCallback(DataGridColumn.OnMinWidthPropertyChanged), new CoerceValueCallback(DataGridColumn.OnCoerceMinWidth)), new ValidateValueCallback(DataGridColumn.ValidateMinWidth));
  public static readonly DependencyProperty MaxWidthProperty = DependencyProperty.Register(nameof (MaxWidth), typeof (double), typeof (DataGridColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) double.PositiveInfinity, new PropertyChangedCallback(DataGridColumn.OnMaxWidthPropertyChanged), new CoerceValueCallback(DataGridColumn.OnCoerceMaxWidth)), new ValidateValueCallback(DataGridColumn.ValidateMaxWidth));
  private static readonly DependencyPropertyKey ActualWidthPropertyKey = DependencyProperty.RegisterReadOnly(nameof (ActualWidth), typeof (double), typeof (DataGridColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0, (PropertyChangedCallback) null, new CoerceValueCallback(DataGridColumn.OnCoerceActualWidth)));
  public static readonly DependencyProperty ActualWidthProperty = DataGridColumn.ActualWidthPropertyKey.DependencyProperty;
  private static readonly DependencyProperty OriginalValueProperty = DependencyProperty.RegisterAttached("OriginalValue", typeof (object), typeof (DataGridColumn), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty DisplayIndexProperty = DependencyProperty.Register(nameof (DisplayIndex), typeof (int), typeof (DataGridColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) -1, new PropertyChangedCallback(DataGridColumn.DisplayIndexChanged), new CoerceValueCallback(DataGridColumn.OnCoerceDisplayIndex)));
  public static readonly DependencyProperty SortMemberPathProperty = DependencyProperty.Register(nameof (SortMemberPath), typeof (string), typeof (DataGridColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) string.Empty));
  public static readonly DependencyProperty CanUserSortProperty = DependencyProperty.Register(nameof (CanUserSort), typeof (bool), typeof (DataGridColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) true, new PropertyChangedCallback(DataGridColumn.OnCanUserSortPropertyChanged), new CoerceValueCallback(DataGridColumn.OnCoerceCanUserSort)));
  public static readonly DependencyProperty SortDirectionProperty = DependencyProperty.Register(nameof (SortDirection), typeof (ListSortDirection?), typeof (DataGridColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGridColumn.OnNotifySortPropertyChanged)));
  private static readonly DependencyPropertyKey IsAutoGeneratedPropertyKey = DependencyProperty.RegisterReadOnly(nameof (IsAutoGenerated), typeof (bool), typeof (DataGridColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
  public static readonly DependencyProperty IsAutoGeneratedProperty = DataGridColumn.IsAutoGeneratedPropertyKey.DependencyProperty;
  private static readonly DependencyPropertyKey IsFrozenPropertyKey = DependencyProperty.RegisterReadOnly(nameof (IsFrozen), typeof (bool), typeof (DataGridColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(DataGridColumn.OnNotifyFrozenPropertyChanged), new CoerceValueCallback(DataGridColumn.OnCoerceIsFrozen)));
  public static readonly DependencyProperty IsFrozenProperty = DataGridColumn.IsFrozenPropertyKey.DependencyProperty;
  public static readonly DependencyProperty CanUserReorderProperty = DependencyProperty.Register(nameof (CanUserReorder), typeof (bool), typeof (DataGridColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) true, new PropertyChangedCallback(DataGridColumn.OnNotifyColumnPropertyChanged), new CoerceValueCallback(DataGridColumn.OnCoerceCanUserReorder)));
  public static readonly DependencyProperty DragIndicatorStyleProperty = DependencyProperty.Register(nameof (DragIndicatorStyle), typeof (Style), typeof (DataGridColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGridColumn.OnNotifyColumnPropertyChanged), new CoerceValueCallback(DataGridColumn.OnCoerceDragIndicatorStyle)));
  private static readonly DependencyProperty CellValueProperty = DependencyProperty.RegisterAttached("CellValue", typeof (object), typeof (DataGridColumn), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty CanUserResizeProperty = DependencyProperty.Register(nameof (CanUserResize), typeof (bool), typeof (DataGridColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) true, new PropertyChangedCallback(DataGridColumn.OnNotifyColumnHeaderPropertyChanged), new CoerceValueCallback(DataGridColumn.OnCoerceCanUserResize)));
  public static readonly DependencyProperty VisibilityProperty = DependencyProperty.Register(nameof (Visibility), typeof (Visibility), typeof (DataGridColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) Visibility.Visible, new PropertyChangedCallback(DataGridColumn.OnVisibilityPropertyChanged)));
  private DataGrid _dataGridOwner;
  private BindingBase _clipboardContentBinding;
  private bool _ignoreRedistributionOnWidthChange;
  private bool _processingWidthChange;

  public object Header
  {
    get => this.GetValue(DataGridColumn.HeaderProperty);
    set => this.SetValue(DataGridColumn.HeaderProperty, value);
  }

  public Style HeaderStyle
  {
    get => (Style) this.GetValue(DataGridColumn.HeaderStyleProperty);
    set => this.SetValue(DataGridColumn.HeaderStyleProperty, (object) value);
  }

  private static object OnCoerceHeaderStyle(DependencyObject d, object baseValue)
  {
    DataGridColumn baseObject = d as DataGridColumn;
    return DataGridHelper.GetCoercedTransferPropertyValue((DependencyObject) baseObject, baseValue, DataGridColumn.HeaderStyleProperty, (DependencyObject) baseObject.DataGridOwner, DataGrid.ColumnHeaderStyleProperty);
  }

  public string HeaderStringFormat
  {
    get => (string) this.GetValue(DataGridColumn.HeaderStringFormatProperty);
    set => this.SetValue(DataGridColumn.HeaderStringFormatProperty, (object) value);
  }

  public DataTemplate HeaderTemplate
  {
    get => (DataTemplate) this.GetValue(DataGridColumn.HeaderTemplateProperty);
    set => this.SetValue(DataGridColumn.HeaderTemplateProperty, (object) value);
  }

  public DataTemplateSelector HeaderTemplateSelector
  {
    get => (DataTemplateSelector) this.GetValue(DataGridColumn.HeaderTemplateSelectorProperty);
    set => this.SetValue(DataGridColumn.HeaderTemplateSelectorProperty, (object) value);
  }

  public Style CellStyle
  {
    get => (Style) this.GetValue(DataGridColumn.CellStyleProperty);
    set => this.SetValue(DataGridColumn.CellStyleProperty, (object) value);
  }

  private static object OnCoerceCellStyle(DependencyObject d, object baseValue)
  {
    DataGridColumn baseObject = d as DataGridColumn;
    return DataGridHelper.GetCoercedTransferPropertyValue((DependencyObject) baseObject, baseValue, DataGridColumn.CellStyleProperty, (DependencyObject) baseObject.DataGridOwner, DataGrid.CellStyleProperty);
  }

  public bool IsReadOnly
  {
    get => (bool) this.GetValue(DataGridColumn.IsReadOnlyProperty);
    set => this.SetValue(DataGridColumn.IsReadOnlyProperty, (object) value);
  }

  private static object OnCoerceIsReadOnly(DependencyObject d, object baseValue)
  {
    return (object) (d as DataGridColumn).OnCoerceIsReadOnly((bool) baseValue);
  }

  protected virtual bool OnCoerceIsReadOnly(bool baseValue)
  {
    return (bool) DataGridHelper.GetCoercedTransferPropertyValue((DependencyObject) this, (object) baseValue, DataGridColumn.IsReadOnlyProperty, (DependencyObject) this.DataGridOwner, DataGrid.IsReadOnlyProperty);
  }

  public DataGridLength Width
  {
    get => (DataGridLength) this.GetValue(DataGridColumn.WidthProperty);
    set => this.SetValue(DataGridColumn.WidthProperty, (object) value);
  }

  internal void SetWidthInternal(DataGridLength width)
  {
    bool redistributionOnWidthChange = this._ignoreRedistributionOnWidthChange;
    this._ignoreRedistributionOnWidthChange = true;
    try
    {
      this.Width = width;
    }
    finally
    {
      this._ignoreRedistributionOnWidthChange = redistributionOnWidthChange;
    }
  }

  private static void OnWidthPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    DataGridColumn changedColumn = (DataGridColumn) d;
    DataGridLength oldValue = (DataGridLength) e.OldValue;
    DataGridLength newValue = (DataGridLength) e.NewValue;
    DataGrid dataGridOwner = changedColumn.DataGridOwner;
    if (dataGridOwner != null && !DoubleUtil.AreClose(oldValue.DisplayValue, newValue.DisplayValue))
      dataGridOwner.InternalColumns.InvalidateAverageColumnWidth();
    if (changedColumn._processingWidthChange)
    {
      changedColumn.CoerceValue(DataGridColumn.ActualWidthProperty);
    }
    else
    {
      changedColumn._processingWidthChange = true;
      if (oldValue.IsStar != newValue.IsStar)
        changedColumn.CoerceValue(DataGridColumn.MaxWidthProperty);
      try
      {
        if (dataGridOwner != null && newValue.IsStar ^ oldValue.IsStar)
          dataGridOwner.InternalColumns.InvalidateHasVisibleStarColumns();
        changedColumn.NotifyPropertyChanged(d, e, NotificationTarget.Cells | NotificationTarget.CellsPresenter | NotificationTarget.Columns | NotificationTarget.ColumnCollection | NotificationTarget.ColumnHeaders | NotificationTarget.ColumnHeadersPresenter);
        if (dataGridOwner == null || changedColumn._ignoreRedistributionOnWidthChange || !changedColumn.IsVisible)
          return;
        if (!newValue.IsStar && !newValue.IsAbsolute)
        {
          DataGridLength width = changedColumn.Width;
          double minMax = DataGridHelper.CoerceToMinMax(width.DesiredValue, changedColumn.MinWidth, changedColumn.MaxWidth);
          changedColumn.SetWidthInternal(new DataGridLength(width.Value, width.UnitType, width.DesiredValue, minMax));
        }
        dataGridOwner.InternalColumns.RedistributeColumnWidthsOnWidthChangeOfColumn(changedColumn, (DataGridLength) e.OldValue);
      }
      finally
      {
        changedColumn._processingWidthChange = false;
      }
    }
  }

  public double MinWidth
  {
    get => (double) this.GetValue(DataGridColumn.MinWidthProperty);
    set => this.SetValue(DataGridColumn.MinWidthProperty, (object) value);
  }

  private static void OnMinWidthPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    DataGridColumn changedColumn = (DataGridColumn) d;
    DataGrid dataGridOwner = changedColumn.DataGridOwner;
    changedColumn.NotifyPropertyChanged(d, e, NotificationTarget.Columns);
    if (dataGridOwner == null || !changedColumn.IsVisible)
      return;
    dataGridOwner.InternalColumns.RedistributeColumnWidthsOnMinWidthChangeOfColumn(changedColumn, (double) e.OldValue);
  }

  public double MaxWidth
  {
    get => (double) this.GetValue(DataGridColumn.MaxWidthProperty);
    set => this.SetValue(DataGridColumn.MaxWidthProperty, (object) value);
  }

  private static void OnMaxWidthPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    DataGridColumn changedColumn = (DataGridColumn) d;
    DataGrid dataGridOwner = changedColumn.DataGridOwner;
    changedColumn.NotifyPropertyChanged(d, e, NotificationTarget.Columns);
    if (dataGridOwner == null || !changedColumn.IsVisible)
      return;
    dataGridOwner.InternalColumns.RedistributeColumnWidthsOnMaxWidthChangeOfColumn(changedColumn, (double) e.OldValue);
  }

  private static double CoerceDesiredOrDisplayWidthValue(
    double widthValue,
    double memberValue,
    DataGridLengthUnitType type)
  {
    if (DoubleUtil.IsNaN(memberValue))
    {
      switch (type)
      {
        case DataGridLengthUnitType.Auto:
        case DataGridLengthUnitType.SizeToCells:
        case DataGridLengthUnitType.SizeToHeader:
          memberValue = 0.0;
          break;
        case DataGridLengthUnitType.Pixel:
          memberValue = widthValue;
          break;
      }
    }
    return memberValue;
  }

  private static object OnCoerceWidth(DependencyObject d, object baseValue)
  {
    DataGridColumn baseObject = d as DataGridColumn;
    DataGridLength transferPropertyValue = (DataGridLength) DataGridHelper.GetCoercedTransferPropertyValue((DependencyObject) baseObject, baseValue, DataGridColumn.WidthProperty, (DependencyObject) baseObject.DataGridOwner, DataGrid.ColumnWidthProperty);
    double desiredValue = DataGridColumn.CoerceDesiredOrDisplayWidthValue(transferPropertyValue.Value, transferPropertyValue.DesiredValue, transferPropertyValue.UnitType);
    double num = DataGridColumn.CoerceDesiredOrDisplayWidthValue(transferPropertyValue.Value, transferPropertyValue.DisplayValue, transferPropertyValue.UnitType);
    double displayValue = DoubleUtil.IsNaN(num) ? num : DataGridHelper.CoerceToMinMax(num, baseObject.MinWidth, baseObject.MaxWidth);
    return DoubleUtil.IsNaN(displayValue) || DoubleUtil.AreClose(displayValue, transferPropertyValue.DisplayValue) ? (object) transferPropertyValue : (object) new DataGridLength(transferPropertyValue.Value, transferPropertyValue.UnitType, desiredValue, displayValue);
  }

  private static object OnCoerceMinWidth(DependencyObject d, object baseValue)
  {
    DataGridColumn baseObject = d as DataGridColumn;
    return DataGridHelper.GetCoercedTransferPropertyValue((DependencyObject) baseObject, baseValue, DataGridColumn.MinWidthProperty, (DependencyObject) baseObject.DataGridOwner, DataGrid.MinColumnWidthProperty);
  }

  private static object OnCoerceMaxWidth(DependencyObject d, object baseValue)
  {
    DataGridColumn baseObject = d as DataGridColumn;
    double transferPropertyValue = (double) DataGridHelper.GetCoercedTransferPropertyValue((DependencyObject) baseObject, baseValue, DataGridColumn.MaxWidthProperty, (DependencyObject) baseObject.DataGridOwner, DataGrid.MaxColumnWidthProperty);
    return double.IsPositiveInfinity(transferPropertyValue) && baseObject.Width.IsStar ? (object) 10000.0 : (object) transferPropertyValue;
  }

  private static bool ValidateMinWidth(object v)
  {
    double d = (double) v;
    return d >= 0.0 && !DoubleUtil.IsNaN(d) && !double.IsPositiveInfinity(d);
  }

  private static bool ValidateMaxWidth(object v)
  {
    double num = (double) v;
    return num >= 0.0 && !DoubleUtil.IsNaN(num);
  }

  public double ActualWidth
  {
    get => (double) this.GetValue(DataGridColumn.ActualWidthProperty);
    private set => this.SetValue(DataGridColumn.ActualWidthPropertyKey, (object) value);
  }

  private static object OnCoerceActualWidth(DependencyObject d, object baseValue)
  {
    DataGridColumn dataGridColumn = (DataGridColumn) d;
    double num = (double) baseValue;
    double minWidth = dataGridColumn.MinWidth;
    double maxWidth = dataGridColumn.MaxWidth;
    DataGridLength width = dataGridColumn.Width;
    if (width.IsAbsolute)
      num = width.DisplayValue;
    if (num < minWidth)
      num = minWidth;
    else if (num > maxWidth)
      num = maxWidth;
    return (object) num;
  }

  internal double GetConstraintWidth(bool isHeader)
  {
    DataGridLength width = this.Width;
    if (!DoubleUtil.IsNaN(width.DisplayValue))
      return width.DisplayValue;
    return width.IsAbsolute || width.IsStar || width.IsSizeToCells && isHeader || width.IsSizeToHeader && !isHeader ? this.ActualWidth : double.PositiveInfinity;
  }

  internal void UpdateDesiredWidthForAutoColumn(bool isHeader, double pixelWidth)
  {
    DataGridLength width = this.Width;
    double minWidth = this.MinWidth;
    double maxWidth = this.MaxWidth;
    double minMax1 = DataGridHelper.CoerceToMinMax(pixelWidth, minWidth, maxWidth);
    if (!width.IsAuto && (!width.IsSizeToCells || isHeader) && (!width.IsSizeToHeader || !isHeader))
      return;
    if (DoubleUtil.IsNaN(width.DesiredValue) || DoubleUtil.LessThan(width.DesiredValue, pixelWidth))
    {
      if (DoubleUtil.IsNaN(width.DisplayValue))
      {
        this.SetWidthInternal(new DataGridLength(width.Value, width.UnitType, pixelWidth, minMax1));
      }
      else
      {
        double minMax2 = DataGridHelper.CoerceToMinMax(width.DesiredValue, minWidth, maxWidth);
        this.SetWidthInternal(new DataGridLength(width.Value, width.UnitType, pixelWidth, width.DisplayValue));
        if (DoubleUtil.AreClose(minMax2, width.DisplayValue))
          this.DataGridOwner.InternalColumns.RecomputeColumnWidthsOnColumnResize(this, pixelWidth - width.DisplayValue, true);
      }
      width = this.Width;
    }
    if (DoubleUtil.IsNaN(width.DisplayValue))
    {
      if (this.ActualWidth >= minMax1)
        return;
      this.ActualWidth = minMax1;
    }
    else
    {
      if (DoubleUtil.AreClose(this.ActualWidth, width.DisplayValue))
        return;
      this.ActualWidth = width.DisplayValue;
    }
  }

  internal void UpdateWidthForStarColumn(
    double displayWidth,
    double desiredWidth,
    double starValue)
  {
    DataGridLength width = this.Width;
    if (DoubleUtil.AreClose(displayWidth, width.DisplayValue) && DoubleUtil.AreClose(desiredWidth, width.DesiredValue) && DoubleUtil.AreClose(width.Value, starValue))
      return;
    this.SetWidthInternal(new DataGridLength(starValue, width.UnitType, desiredWidth, displayWidth));
    this.ActualWidth = displayWidth;
  }

  public FrameworkElement GetCellContent(object dataItem)
  {
    if (dataItem == null)
      throw new ArgumentNullException(nameof (dataItem));
    return this._dataGridOwner != null && this._dataGridOwner.ItemContainerGenerator.ContainerFromItem(dataItem) is DataGridRow dataGridRow ? this.GetCellContent(dataGridRow) : (FrameworkElement) null;
  }

  public FrameworkElement GetCellContent(DataGridRow dataGridRow)
  {
    if (dataGridRow == null)
      throw new ArgumentNullException(nameof (dataGridRow));
    if (this._dataGridOwner != null)
    {
      int index = this._dataGridOwner.Columns.IndexOf(this);
      if (index >= 0)
      {
        DataGridCell cell = dataGridRow.TryGetCell(index);
        if (cell != null)
          return cell.Content as FrameworkElement;
      }
    }
    return (FrameworkElement) null;
  }

  internal FrameworkElement BuildVisualTree(bool isEditing, object dataItem, DataGridCell cell)
  {
    return isEditing ? this.GenerateEditingElement(cell, dataItem) : this.GenerateElement(cell, dataItem);
  }

  protected abstract FrameworkElement GenerateElement(DataGridCell cell, object dataItem);

  protected abstract FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem);

  protected virtual object PrepareCellForEdit(
    FrameworkElement editingElement,
    RoutedEventArgs editingEventArgs)
  {
    return (object) null;
  }

  protected virtual void CancelCellEdit(FrameworkElement editingElement, object uneditedValue)
  {
  }

  protected virtual bool CommitCellEdit(FrameworkElement editingElement) => true;

  internal void BeginEdit(FrameworkElement editingElement, RoutedEventArgs e)
  {
    if (editingElement == null)
      return;
    editingElement.UpdateLayout();
    object obj = this.PrepareCellForEdit(editingElement, e);
    DataGridColumn.SetOriginalValue((DependencyObject) editingElement, obj);
  }

  internal void CancelEdit(FrameworkElement editingElement)
  {
    if (editingElement == null)
      return;
    this.CancelCellEdit(editingElement, DataGridColumn.GetOriginalValue((DependencyObject) editingElement));
    DataGridColumn.ClearOriginalValue((DependencyObject) editingElement);
  }

  internal bool CommitEdit(FrameworkElement editingElement)
  {
    if (editingElement == null)
      return true;
    if (!this.CommitCellEdit(editingElement))
      return false;
    DataGridColumn.ClearOriginalValue((DependencyObject) editingElement);
    return true;
  }

  private static object GetOriginalValue(DependencyObject obj)
  {
    return obj.GetValue(DataGridColumn.OriginalValueProperty);
  }

  private static void SetOriginalValue(DependencyObject obj, object value)
  {
    obj.SetValue(DataGridColumn.OriginalValueProperty, value);
  }

  private static void ClearOriginalValue(DependencyObject obj)
  {
    obj.ClearValue(DataGridColumn.OriginalValueProperty);
  }

  internal static void OnNotifyCellPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DataGridColumn) d).NotifyPropertyChanged(d, e, NotificationTarget.Cells | NotificationTarget.Columns);
  }

  private static void OnNotifyColumnHeaderPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DataGridColumn) d).NotifyPropertyChanged(d, e, NotificationTarget.Columns | NotificationTarget.ColumnHeaders);
  }

  private static void OnNotifyColumnPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DataGridColumn) d).NotifyPropertyChanged(d, e, NotificationTarget.Columns);
  }

  internal void NotifyPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e,
    NotificationTarget target)
  {
    if (DataGridHelper.ShouldNotifyColumns(target))
    {
      target &= ~NotificationTarget.Columns;
      if (e.Property == DataGrid.MaxColumnWidthProperty || e.Property == DataGridColumn.MaxWidthProperty)
        DataGridHelper.TransferProperty((DependencyObject) this, DataGridColumn.MaxWidthProperty);
      else if (e.Property == DataGrid.MinColumnWidthProperty || e.Property == DataGridColumn.MinWidthProperty)
        DataGridHelper.TransferProperty((DependencyObject) this, DataGridColumn.MinWidthProperty);
      else if (e.Property == DataGrid.ColumnWidthProperty || e.Property == DataGridColumn.WidthProperty)
        DataGridHelper.TransferProperty((DependencyObject) this, DataGridColumn.WidthProperty);
      else if (e.Property == DataGrid.ColumnHeaderStyleProperty || e.Property == DataGridColumn.HeaderStyleProperty)
        DataGridHelper.TransferProperty((DependencyObject) this, DataGridColumn.HeaderStyleProperty);
      else if (e.Property == DataGrid.CellStyleProperty || e.Property == DataGridColumn.CellStyleProperty)
        DataGridHelper.TransferProperty((DependencyObject) this, DataGridColumn.CellStyleProperty);
      else if (e.Property == DataGrid.IsReadOnlyProperty || e.Property == DataGridColumn.IsReadOnlyProperty)
        DataGridHelper.TransferProperty((DependencyObject) this, DataGridColumn.IsReadOnlyProperty);
      else if (e.Property == DataGrid.DragIndicatorStyleProperty || e.Property == DataGridColumn.DragIndicatorStyleProperty)
        DataGridHelper.TransferProperty((DependencyObject) this, DataGridColumn.DragIndicatorStyleProperty);
      else if (e.Property == DataGridColumn.DisplayIndexProperty)
        this.CoerceValue(DataGridColumn.IsFrozenProperty);
      else if (e.Property == DataGrid.CanUserSortColumnsProperty)
        DataGridHelper.TransferProperty((DependencyObject) this, DataGridColumn.CanUserSortProperty);
      else if (e.Property == DataGrid.CanUserResizeColumnsProperty || e.Property == DataGridColumn.CanUserResizeProperty)
        DataGridHelper.TransferProperty((DependencyObject) this, DataGridColumn.CanUserResizeProperty);
      else if (e.Property == DataGrid.CanUserReorderColumnsProperty || e.Property == DataGridColumn.CanUserReorderProperty)
        DataGridHelper.TransferProperty((DependencyObject) this, DataGridColumn.CanUserReorderProperty);
      if (e.Property == DataGridColumn.WidthProperty || e.Property == DataGridColumn.MinWidthProperty || e.Property == DataGridColumn.MaxWidthProperty)
        this.CoerceValue(DataGridColumn.ActualWidthProperty);
    }
    if (target == NotificationTarget.None)
      return;
    ((DataGridColumn) d).DataGridOwner?.NotifyPropertyChanged(d, e, target);
  }

  protected void NotifyPropertyChanged(string propertyName)
  {
    if (this.DataGridOwner == null)
      return;
    this.DataGridOwner.NotifyPropertyChanged((DependencyObject) this, propertyName, new DependencyPropertyChangedEventArgs(), NotificationTarget.RefreshCellContent);
  }

  internal static void NotifyPropertyChangeForRefreshContent(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DataGridColumn) d).NotifyPropertyChanged(e.Property.Name);
  }

  protected internal virtual void RefreshCellContent(FrameworkElement element, string propertyName)
  {
  }

  internal void SyncProperties()
  {
    DataGridHelper.TransferProperty((DependencyObject) this, DataGridColumn.MinWidthProperty);
    DataGridHelper.TransferProperty((DependencyObject) this, DataGridColumn.MaxWidthProperty);
    DataGridHelper.TransferProperty((DependencyObject) this, DataGridColumn.WidthProperty);
    DataGridHelper.TransferProperty((DependencyObject) this, DataGridColumn.HeaderStyleProperty);
    DataGridHelper.TransferProperty((DependencyObject) this, DataGridColumn.CellStyleProperty);
    DataGridHelper.TransferProperty((DependencyObject) this, DataGridColumn.IsReadOnlyProperty);
    DataGridHelper.TransferProperty((DependencyObject) this, DataGridColumn.DragIndicatorStyleProperty);
    DataGridHelper.TransferProperty((DependencyObject) this, DataGridColumn.CanUserSortProperty);
    DataGridHelper.TransferProperty((DependencyObject) this, DataGridColumn.CanUserReorderProperty);
    DataGridHelper.TransferProperty((DependencyObject) this, DataGridColumn.CanUserResizeProperty);
  }

  protected internal DataGrid DataGridOwner
  {
    get => this._dataGridOwner;
    internal set => this._dataGridOwner = value;
  }

  public int DisplayIndex
  {
    get => (int) this.GetValue(DataGridColumn.DisplayIndexProperty);
    set => this.SetValue(DataGridColumn.DisplayIndexProperty, (object) value);
  }

  private static object OnCoerceDisplayIndex(DependencyObject d, object baseValue)
  {
    DataGridColumn column = (DataGridColumn) d;
    if (column.DataGridOwner != null)
      column.DataGridOwner.ValidateDisplayIndex(column, (int) baseValue);
    return baseValue;
  }

  private static void DisplayIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    ((DataGridColumn) d).NotifyPropertyChanged(d, e, NotificationTarget.Cells | NotificationTarget.CellsPresenter | NotificationTarget.Columns | NotificationTarget.ColumnCollection | NotificationTarget.ColumnHeaders | NotificationTarget.ColumnHeadersPresenter);
  }

  public string SortMemberPath
  {
    get => (string) this.GetValue(DataGridColumn.SortMemberPathProperty);
    set => this.SetValue(DataGridColumn.SortMemberPathProperty, (object) value);
  }

  public bool CanUserSort
  {
    get => (bool) this.GetValue(DataGridColumn.CanUserSortProperty);
    set => this.SetValue(DataGridColumn.CanUserSortProperty, (object) value);
  }

  internal static object OnCoerceCanUserSort(DependencyObject d, object baseValue)
  {
    DataGridColumn baseObject = d as DataGridColumn;
    ValueSource valueSource1 = DependencyPropertyHelper.GetValueSource((DependencyObject) baseObject, DataGridColumn.CanUserSortProperty);
    bool flag1 = valueSource1.IsAnimated || valueSource1.IsCoerced || valueSource1.IsExpression;
    if (baseObject.DataGridOwner != null)
    {
      ValueSource valueSource2 = DependencyPropertyHelper.GetValueSource((DependencyObject) baseObject.DataGridOwner, DataGrid.CanUserSortColumnsProperty);
      bool flag2 = valueSource2.IsAnimated || valueSource2.IsCoerced || valueSource2.IsExpression;
      if (valueSource2.BaseValueSource == valueSource1.BaseValueSource && !flag1 && flag2)
        return baseObject.DataGridOwner.GetValue(DataGrid.CanUserSortColumnsProperty);
    }
    return DataGridHelper.GetCoercedTransferPropertyValue((DependencyObject) baseObject, baseValue, DataGridColumn.CanUserSortProperty, (DependencyObject) baseObject.DataGridOwner, DataGrid.CanUserSortColumnsProperty);
  }

  private static void OnCanUserSortPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    if (!DataGridHelper.IsPropertyTransferEnabled(d, DataGridColumn.CanUserSortProperty))
      DataGridHelper.TransferProperty(d, DataGridColumn.CanUserSortProperty);
    ((DataGridColumn) d).NotifyPropertyChanged(d, e, NotificationTarget.ColumnHeaders);
  }

  public ListSortDirection? SortDirection
  {
    get => (ListSortDirection?) this.GetValue(DataGridColumn.SortDirectionProperty);
    set => this.SetValue(DataGridColumn.SortDirectionProperty, (object) value);
  }

  private static void OnNotifySortPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DataGridColumn) d).NotifyPropertyChanged(d, e, NotificationTarget.ColumnHeaders);
  }

  public bool IsAutoGenerated
  {
    get => (bool) this.GetValue(DataGridColumn.IsAutoGeneratedProperty);
    internal set => this.SetValue(DataGridColumn.IsAutoGeneratedPropertyKey, (object) value);
  }

  internal static DataGridColumn CreateDefaultColumn(ItemPropertyInfo itemProperty)
  {
    DataGridComboBoxColumn gridComboBoxColumn = (DataGridComboBoxColumn) null;
    Type propertyType = itemProperty.PropertyType;
    DataGridColumn defaultColumn;
    if (propertyType.IsEnum)
    {
      gridComboBoxColumn = new DataGridComboBoxColumn();
      gridComboBoxColumn.ItemsSource = (IEnumerable) Enum.GetValues(propertyType);
      defaultColumn = (DataGridColumn) gridComboBoxColumn;
    }
    else
      defaultColumn = !typeof (string).IsAssignableFrom(propertyType) ? (!typeof (bool).IsAssignableFrom(propertyType) ? (!typeof (Uri).IsAssignableFrom(propertyType) ? (DataGridColumn) new DataGridTextColumn() : (DataGridColumn) new DataGridHyperlinkColumn()) : (DataGridColumn) new DataGridCheckBoxColumn()) : (DataGridColumn) new DataGridTextColumn();
    if (!typeof (IComparable).IsAssignableFrom(propertyType))
      defaultColumn.CanUserSort = false;
    defaultColumn.Header = (object) itemProperty.Name;
    if (defaultColumn is DataGridBoundColumn dataGridBoundColumn || gridComboBoxColumn != null)
    {
      Binding binding = new Binding(itemProperty.Name);
      if (gridComboBoxColumn != null)
        gridComboBoxColumn.SelectedItemBinding = (BindingBase) binding;
      else
        dataGridBoundColumn.Binding = (BindingBase) binding;
      if (itemProperty.Descriptor is PropertyDescriptor descriptor1)
      {
        if (descriptor1.IsReadOnly)
        {
          binding.Mode = BindingMode.OneWay;
          defaultColumn.IsReadOnly = true;
        }
      }
      else
      {
        PropertyInfo descriptor = itemProperty.Descriptor as PropertyInfo;
        if ((object) descriptor != null && !descriptor.CanWrite)
        {
          binding.Mode = BindingMode.OneWay;
          defaultColumn.IsReadOnly = true;
        }
      }
    }
    return defaultColumn;
  }

  public bool IsFrozen
  {
    get => (bool) this.GetValue(DataGridColumn.IsFrozenProperty);
    internal set => this.SetValue(DataGridColumn.IsFrozenPropertyKey, (object) value);
  }

  private static void OnNotifyFrozenPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DataGridColumn) d).NotifyPropertyChanged(d, e, NotificationTarget.ColumnHeaders);
  }

  private static object OnCoerceIsFrozen(DependencyObject d, object baseValue)
  {
    DataGridColumn dataGridColumn = (DataGridColumn) d;
    DataGrid dataGridOwner = dataGridColumn.DataGridOwner;
    if (dataGridOwner == null)
      return baseValue;
    return dataGridColumn.DisplayIndex < dataGridOwner.FrozenColumnCount ? (object) true : (object) false;
  }

  public bool CanUserReorder
  {
    get => (bool) this.GetValue(DataGridColumn.CanUserReorderProperty);
    set => this.SetValue(DataGridColumn.CanUserReorderProperty, (object) value);
  }

  private static object OnCoerceCanUserReorder(DependencyObject d, object baseValue)
  {
    DataGridColumn baseObject = d as DataGridColumn;
    return DataGridHelper.GetCoercedTransferPropertyValue((DependencyObject) baseObject, baseValue, DataGridColumn.CanUserReorderProperty, (DependencyObject) baseObject.DataGridOwner, DataGrid.CanUserReorderColumnsProperty);
  }

  public Style DragIndicatorStyle
  {
    get => (Style) this.GetValue(DataGridColumn.DragIndicatorStyleProperty);
    set => this.SetValue(DataGridColumn.DragIndicatorStyleProperty, (object) value);
  }

  private static object OnCoerceDragIndicatorStyle(DependencyObject d, object baseValue)
  {
    DataGridColumn baseObject = d as DataGridColumn;
    return DataGridHelper.GetCoercedTransferPropertyValue((DependencyObject) baseObject, baseValue, DataGridColumn.DragIndicatorStyleProperty, (DependencyObject) baseObject.DataGridOwner, DataGrid.DragIndicatorStyleProperty);
  }

  public virtual BindingBase ClipboardContentBinding
  {
    get => this._clipboardContentBinding;
    set => this._clipboardContentBinding = value;
  }

  public virtual object OnCopyingCellClipboardContent(object item)
  {
    object content = (object) null;
    BindingBase clipboardContentBinding = this.ClipboardContentBinding;
    if (clipboardContentBinding != null)
    {
      FrameworkElement frameworkElement = new FrameworkElement();
      frameworkElement.DataContext = item;
      frameworkElement.SetBinding(DataGridColumn.CellValueProperty, clipboardContentBinding);
      content = frameworkElement.GetValue(DataGridColumn.CellValueProperty);
    }
    if (this.CopyingCellClipboardContent != null)
    {
      DataGridCellClipboardEventArgs e = new DataGridCellClipboardEventArgs(item, this, content);
      this.CopyingCellClipboardContent((object) this, e);
      content = e.Content;
    }
    return content;
  }

  public virtual void OnPastingCellClipboardContent(object item, object cellContent)
  {
    BindingBase clipboardContentBinding = this.ClipboardContentBinding;
    if (clipboardContentBinding == null)
      return;
    if (this.PastingCellClipboardContent != null)
    {
      DataGridCellClipboardEventArgs e = new DataGridCellClipboardEventArgs(item, this, cellContent);
      this.PastingCellClipboardContent((object) this, e);
      cellContent = e.Content;
    }
    if (cellContent == null)
      return;
    FrameworkElement frameworkElement = new FrameworkElement();
    frameworkElement.DataContext = item;
    frameworkElement.SetBinding(DataGridColumn.CellValueProperty, clipboardContentBinding);
    frameworkElement.SetValue(DataGridColumn.CellValueProperty, cellContent);
    frameworkElement.GetBindingExpression(DataGridColumn.CellValueProperty).UpdateSource();
  }

  public event EventHandler<DataGridCellClipboardEventArgs> CopyingCellClipboardContent;

  public event EventHandler<DataGridCellClipboardEventArgs> PastingCellClipboardContent;

  internal virtual void OnInput(InputEventArgs e)
  {
  }

  internal void BeginEdit(InputEventArgs e)
  {
    DataGrid dataGridOwner = this.DataGridOwner;
    if (dataGridOwner == null || !dataGridOwner.BeginEdit((RoutedEventArgs) e))
      return;
    e.Handled = true;
  }

  public bool CanUserResize
  {
    get => (bool) this.GetValue(DataGridColumn.CanUserResizeProperty);
    set => this.SetValue(DataGridColumn.CanUserResizeProperty, (object) value);
  }

  private static object OnCoerceCanUserResize(DependencyObject d, object baseValue)
  {
    DataGridColumn baseObject = d as DataGridColumn;
    return DataGridHelper.GetCoercedTransferPropertyValue((DependencyObject) baseObject, baseValue, DataGridColumn.CanUserResizeProperty, (DependencyObject) baseObject.DataGridOwner, DataGrid.CanUserResizeColumnsProperty);
  }

  public Visibility Visibility
  {
    get => (Visibility) this.GetValue(DataGridColumn.VisibilityProperty);
    set => this.SetValue(DataGridColumn.VisibilityProperty, (object) value);
  }

  private static void OnVisibilityPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs eventArgs)
  {
    Visibility oldValue = (Visibility) eventArgs.OldValue;
    Visibility newValue = (Visibility) eventArgs.NewValue;
    if (oldValue != Visibility.Visible && newValue != Visibility.Visible)
      return;
    ((DataGridColumn) d).NotifyPropertyChanged(d, eventArgs, NotificationTarget.CellsPresenter | NotificationTarget.ColumnCollection | NotificationTarget.ColumnHeaders | NotificationTarget.ColumnHeadersPresenter);
  }

  internal bool IsVisible => this.Visibility == Visibility.Visible;
}

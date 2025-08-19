// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.DataGridTemplateColumn
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

#nullable disable
namespace Microsoft.Windows.Controls;

public class DataGridTemplateColumn : DataGridColumn
{
  public static readonly DependencyProperty CellTemplateProperty = DependencyProperty.Register(nameof (CellTemplate), typeof (DataTemplate), typeof (DataGridTemplateColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGridColumn.NotifyPropertyChangeForRefreshContent)));
  public static readonly DependencyProperty CellTemplateSelectorProperty = DependencyProperty.Register(nameof (CellTemplateSelector), typeof (DataTemplateSelector), typeof (DataGridTemplateColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGridColumn.NotifyPropertyChangeForRefreshContent)));
  public static readonly DependencyProperty CellEditingTemplateProperty = DependencyProperty.Register(nameof (CellEditingTemplate), typeof (DataTemplate), typeof (DataGridTemplateColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGridColumn.NotifyPropertyChangeForRefreshContent)));
  public static readonly DependencyProperty CellEditingTemplateSelectorProperty = DependencyProperty.Register(nameof (CellEditingTemplateSelector), typeof (DataTemplateSelector), typeof (DataGridTemplateColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGridColumn.NotifyPropertyChangeForRefreshContent)));

  static DataGridTemplateColumn()
  {
    DataGridColumn.CanUserSortProperty.OverrideMetadata(typeof (DataGridTemplateColumn), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null, new CoerceValueCallback(DataGridTemplateColumn.OnCoerceTemplateColumnCanUserSort)));
    DataGridColumn.SortMemberPathProperty.OverrideMetadata(typeof (DataGridTemplateColumn), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(DataGridTemplateColumn.OnTemplateColumnSortMemberPathChanged)));
  }

  private static void OnTemplateColumnSortMemberPathChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    d.CoerceValue(DataGridColumn.CanUserSortProperty);
  }

  private static object OnCoerceTemplateColumnCanUserSort(DependencyObject d, object baseValue)
  {
    return string.IsNullOrEmpty(((DataGridColumn) d).SortMemberPath) ? (object) false : DataGridColumn.OnCoerceCanUserSort(d, baseValue);
  }

  public DataTemplate CellTemplate
  {
    get => (DataTemplate) this.GetValue(DataGridTemplateColumn.CellTemplateProperty);
    set => this.SetValue(DataGridTemplateColumn.CellTemplateProperty, (object) value);
  }

  public DataTemplateSelector CellTemplateSelector
  {
    get
    {
      return (DataTemplateSelector) this.GetValue(DataGridTemplateColumn.CellTemplateSelectorProperty);
    }
    set => this.SetValue(DataGridTemplateColumn.CellTemplateSelectorProperty, (object) value);
  }

  public DataTemplate CellEditingTemplate
  {
    get => (DataTemplate) this.GetValue(DataGridTemplateColumn.CellEditingTemplateProperty);
    set => this.SetValue(DataGridTemplateColumn.CellEditingTemplateProperty, (object) value);
  }

  public DataTemplateSelector CellEditingTemplateSelector
  {
    get
    {
      return (DataTemplateSelector) this.GetValue(DataGridTemplateColumn.CellEditingTemplateSelectorProperty);
    }
    set
    {
      this.SetValue(DataGridTemplateColumn.CellEditingTemplateSelectorProperty, (object) value);
    }
  }

  private DataTemplate ChooseCellTemplate(bool isEditing)
  {
    DataTemplate dataTemplate = (DataTemplate) null;
    if (isEditing)
      dataTemplate = this.CellEditingTemplate;
    if (dataTemplate == null)
      dataTemplate = this.CellTemplate;
    return dataTemplate;
  }

  private DataTemplateSelector ChooseCellTemplateSelector(bool isEditing)
  {
    DataTemplateSelector templateSelector = (DataTemplateSelector) null;
    if (isEditing)
      templateSelector = this.CellEditingTemplateSelector;
    if (templateSelector == null)
      templateSelector = this.CellTemplateSelector;
    return templateSelector;
  }

  private FrameworkElement LoadTemplateContent(bool isEditing, object dataItem, DataGridCell cell)
  {
    DataTemplate dataTemplate = this.ChooseCellTemplate(isEditing);
    DataTemplateSelector templateSelector = this.ChooseCellTemplateSelector(isEditing);
    if (dataTemplate == null && templateSelector == null)
      return (FrameworkElement) null;
    ContentPresenter target = new ContentPresenter();
    BindingOperations.SetBinding((DependencyObject) target, ContentPresenter.ContentProperty, (BindingBase) new Binding());
    target.ContentTemplate = dataTemplate;
    target.ContentTemplateSelector = templateSelector;
    return (FrameworkElement) target;
  }

  protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
  {
    return this.LoadTemplateContent(false, dataItem, cell);
  }

  protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
  {
    return this.LoadTemplateContent(true, dataItem, cell);
  }

  protected internal override void RefreshCellContent(FrameworkElement element, string propertyName)
  {
    if (element is DataGridCell dataGridCell)
    {
      bool isEditing = dataGridCell.IsEditing;
      if (!isEditing && (string.Compare(propertyName, "CellTemplate", StringComparison.Ordinal) == 0 || string.Compare(propertyName, "CellTemplateSelector", StringComparison.Ordinal) == 0) || isEditing && (string.Compare(propertyName, "CellEditingTemplate", StringComparison.Ordinal) == 0 || string.Compare(propertyName, "CellEditingTemplateSelector", StringComparison.Ordinal) == 0))
      {
        dataGridCell.BuildVisualTree();
        return;
      }
    }
    base.RefreshCellContent(element, propertyName);
  }
}

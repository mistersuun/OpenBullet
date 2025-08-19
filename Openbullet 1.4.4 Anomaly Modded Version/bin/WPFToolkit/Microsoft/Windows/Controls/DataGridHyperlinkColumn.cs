// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.DataGridHyperlinkColumn
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

#nullable disable
namespace Microsoft.Windows.Controls;

public class DataGridHyperlinkColumn : DataGridBoundColumn
{
  public static readonly DependencyProperty TargetNameProperty = Hyperlink.TargetNameProperty.AddOwner(typeof (DataGridHyperlinkColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGridColumn.NotifyPropertyChangeForRefreshContent)));
  private BindingBase _contentBinding;

  static DataGridHyperlinkColumn()
  {
    DataGridBoundColumn.ElementStyleProperty.OverrideMetadata(typeof (DataGridHyperlinkColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) DataGridHyperlinkColumn.DefaultElementStyle));
    DataGridBoundColumn.EditingElementStyleProperty.OverrideMetadata(typeof (DataGridHyperlinkColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) DataGridHyperlinkColumn.DefaultEditingElementStyle));
  }

  public string TargetName
  {
    get => (string) this.GetValue(DataGridHyperlinkColumn.TargetNameProperty);
    set => this.SetValue(DataGridHyperlinkColumn.TargetNameProperty, (object) value);
  }

  public BindingBase ContentBinding
  {
    get => this._contentBinding;
    set
    {
      if (this._contentBinding == value)
        return;
      BindingBase contentBinding = this._contentBinding;
      this._contentBinding = value;
      this.OnContentBindingChanged(contentBinding, value);
    }
  }

  protected virtual void OnContentBindingChanged(BindingBase oldBinding, BindingBase newBinding)
  {
    this.NotifyPropertyChanged("ContentBinding");
  }

  private void ApplyContentBinding(DependencyObject target, DependencyProperty property)
  {
    if (this.ContentBinding != null)
      BindingOperations.SetBinding(target, property, this.ContentBinding);
    else if (this.Binding != null)
      BindingOperations.SetBinding(target, property, this.Binding);
    else
      BindingOperations.ClearBinding(target, property);
  }

  protected internal override void RefreshCellContent(FrameworkElement element, string propertyName)
  {
    if (element is DataGridCell dataGridCell && !dataGridCell.IsEditing)
    {
      if (string.Compare(propertyName, "ContentBinding", StringComparison.Ordinal) == 0)
      {
        dataGridCell.BuildVisualTree();
      }
      else
      {
        if (string.Compare(propertyName, "TargetName", StringComparison.Ordinal) != 0 || !(dataGridCell.Content is TextBlock content) || content.Inlines.Count <= 0 || !(content.Inlines.FirstInline is Hyperlink firstInline))
          return;
        firstInline.TargetName = this.TargetName;
      }
    }
    else
      base.RefreshCellContent(element, propertyName);
  }

  public static Style DefaultElementStyle => DataGridTextColumn.DefaultElementStyle;

  public static Style DefaultEditingElementStyle => DataGridTextColumn.DefaultEditingElementStyle;

  protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
  {
    TextBlock element = new TextBlock();
    Hyperlink target1 = new Hyperlink();
    InlineUIContainer inlineUiContainer = new InlineUIContainer();
    ContentPresenter target2 = new ContentPresenter();
    element.Inlines.Add((Inline) target1);
    target1.Inlines.Add((Inline) inlineUiContainer);
    inlineUiContainer.Child = (UIElement) target2;
    target1.TargetName = this.TargetName;
    this.ApplyStyle(false, false, (FrameworkElement) element);
    this.ApplyBinding((DependencyObject) target1, Hyperlink.NavigateUriProperty);
    this.ApplyContentBinding((DependencyObject) target2, ContentPresenter.ContentProperty);
    return (FrameworkElement) element;
  }

  protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
  {
    TextBox editingElement = new TextBox();
    this.ApplyStyle(true, false, (FrameworkElement) editingElement);
    this.ApplyBinding((DependencyObject) editingElement, TextBox.TextProperty);
    return (FrameworkElement) editingElement;
  }

  protected override object PrepareCellForEdit(
    FrameworkElement editingElement,
    RoutedEventArgs editingEventArgs)
  {
    if (!(editingElement is TextBox textBox))
      return (object) null;
    textBox.Focus();
    string text1 = textBox.Text;
    if (editingEventArgs is TextCompositionEventArgs compositionEventArgs)
    {
      string text2 = compositionEventArgs.Text;
      textBox.Text = text2;
      textBox.Select(text2.Length, 0);
    }
    else
      textBox.SelectAll();
    return (object) text1;
  }

  protected override bool CommitCellEdit(FrameworkElement editingElement)
  {
    if (!(editingElement is TextBox element))
      return true;
    DataGridHelper.UpdateSource((FrameworkElement) element, TextBox.TextProperty);
    return !Validation.GetHasError((DependencyObject) element);
  }

  protected override void CancelCellEdit(FrameworkElement editingElement, object uneditedValue)
  {
    if (!(editingElement is TextBox element))
      return;
    DataGridHelper.UpdateTarget((FrameworkElement) element, TextBox.TextProperty);
  }

  internal override void OnInput(InputEventArgs e)
  {
    if (!DataGridHelper.HasNonEscapeCharacters(e as TextCompositionEventArgs))
      return;
    this.BeginEdit(e);
  }
}

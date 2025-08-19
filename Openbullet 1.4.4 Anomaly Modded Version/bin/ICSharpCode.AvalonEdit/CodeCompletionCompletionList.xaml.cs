// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.CodeCompletion.CompletionList
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

#nullable disable
namespace ICSharpCode.AvalonEdit.CodeCompletion;

public partial class CompletionList : Control
{
  private bool isFiltering = true;
  public static readonly DependencyProperty EmptyTemplateProperty = DependencyProperty.Register(nameof (EmptyTemplate), typeof (ControlTemplate), typeof (CompletionList), (PropertyMetadata) new FrameworkPropertyMetadata());
  private CompletionListBox listBox;
  private ObservableCollection<ICompletionData> completionData = new ObservableCollection<ICompletionData>();
  private string currentText;
  private ObservableCollection<ICompletionData> currentList;

  static CompletionList()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (CompletionList), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (CompletionList)));
  }

  public bool IsFiltering
  {
    get => this.isFiltering;
    set => this.isFiltering = value;
  }

  public ControlTemplate EmptyTemplate
  {
    get => (ControlTemplate) this.GetValue(CompletionList.EmptyTemplateProperty);
    set => this.SetValue(CompletionList.EmptyTemplateProperty, (object) value);
  }

  public event EventHandler InsertionRequested;

  public void RequestInsertion(EventArgs e)
  {
    if (this.InsertionRequested == null)
      return;
    this.InsertionRequested((object) this, e);
  }

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    this.listBox = this.GetTemplateChild("PART_ListBox") as CompletionListBox;
    if (this.listBox == null)
      return;
    this.listBox.ItemsSource = (IEnumerable) this.completionData;
  }

  public CompletionListBox ListBox
  {
    get
    {
      if (this.listBox == null)
        this.ApplyTemplate();
      return this.listBox;
    }
  }

  public ScrollViewer ScrollViewer
  {
    get => this.listBox == null ? (ScrollViewer) null : this.listBox.scrollViewer;
  }

  public IList<ICompletionData> CompletionData => (IList<ICompletionData>) this.completionData;

  protected override void OnKeyDown(KeyEventArgs e)
  {
    base.OnKeyDown(e);
    if (e.Handled)
      return;
    this.HandleKey(e);
  }

  public void HandleKey(KeyEventArgs e)
  {
    if (this.listBox == null)
      return;
    switch (e.Key)
    {
      case Key.Tab:
      case Key.Return:
        e.Handled = true;
        this.RequestInsertion((EventArgs) e);
        break;
      case Key.Prior:
        e.Handled = true;
        this.listBox.SelectIndex(this.listBox.SelectedIndex - this.listBox.VisibleItemCount);
        break;
      case Key.Next:
        e.Handled = true;
        this.listBox.SelectIndex(this.listBox.SelectedIndex + this.listBox.VisibleItemCount);
        break;
      case Key.End:
        e.Handled = true;
        this.listBox.SelectIndex(this.listBox.Items.Count - 1);
        break;
      case Key.Home:
        e.Handled = true;
        this.listBox.SelectIndex(0);
        break;
      case Key.Up:
        e.Handled = true;
        this.listBox.SelectIndex(this.listBox.SelectedIndex - 1);
        break;
      case Key.Down:
        e.Handled = true;
        this.listBox.SelectIndex(this.listBox.SelectedIndex + 1);
        break;
    }
  }

  protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
  {
    base.OnMouseDoubleClick(e);
    if (e.ChangedButton != MouseButton.Left || !(e.OriginalSource as DependencyObject).VisualAncestorsAndSelf().TakeWhile<DependencyObject>((Func<DependencyObject, bool>) (obj => obj != this)).Any<DependencyObject>((Func<DependencyObject, bool>) (obj => obj is ListBoxItem)))
      return;
    e.Handled = true;
    this.RequestInsertion((EventArgs) e);
  }

  public ICompletionData SelectedItem
  {
    get => (this.listBox != null ? this.listBox.SelectedItem : (object) null) as ICompletionData;
    set
    {
      if (this.listBox == null && value != null)
        this.ApplyTemplate();
      if (this.listBox == null)
        return;
      this.listBox.SelectedItem = (object) value;
    }
  }

  public void ScrollIntoView(ICompletionData item)
  {
    if (this.listBox == null)
      this.ApplyTemplate();
    if (this.listBox == null)
      return;
    this.listBox.ScrollIntoView((object) item);
  }

  public event SelectionChangedEventHandler SelectionChanged
  {
    add => this.AddHandler(Selector.SelectionChangedEvent, (Delegate) value);
    remove => this.RemoveHandler(Selector.SelectionChangedEvent, (Delegate) value);
  }

  public void SelectItem(string text)
  {
    if (text == this.currentText)
      return;
    if (this.listBox == null)
      this.ApplyTemplate();
    if (this.IsFiltering)
      this.SelectItemFiltering(text);
    else
      this.SelectItemWithStart(text);
    this.currentText = text;
  }

  private void SelectItemFiltering(string query)
  {
    IEnumerable<\u003C\u003Ef__AnonymousType1<ICompletionData, int>> datas = (this.currentList == null || string.IsNullOrEmpty(this.currentText) || string.IsNullOrEmpty(query) || !query.StartsWith(this.currentText, StringComparison.Ordinal) ? (IEnumerable<ICompletionData>) this.completionData : (IEnumerable<ICompletionData>) this.currentList).Select(item => new
    {
      item = item,
      quality = this.GetMatchQuality(item.Text, query)
    }).Where(_param0 => _param0.quality > 0).Select(_param0 => new
    {
      Item = _param0.item,
      Quality = _param0.quality
    });
    ICompletionData completionData = this.listBox.SelectedIndex != -1 ? (ICompletionData) this.listBox.Items[this.listBox.SelectedIndex] : (ICompletionData) null;
    ObservableCollection<ICompletionData> observableCollection = new ObservableCollection<ICompletionData>();
    int bestIndex = -1;
    int num1 = -1;
    double num2 = 0.0;
    int num3 = 0;
    foreach (var data in datas)
    {
      double num4 = data.Item == completionData ? double.PositiveInfinity : data.Item.Priority;
      int quality = data.Quality;
      if (quality > num1 || quality == num1 && num4 > num2)
      {
        bestIndex = num3;
        num2 = num4;
        num1 = quality;
      }
      observableCollection.Add(data.Item);
      ++num3;
    }
    this.currentList = observableCollection;
    this.listBox.ItemsSource = (IEnumerable) observableCollection;
    this.SelectIndexCentered(bestIndex);
  }

  private void SelectItemWithStart(string query)
  {
    if (string.IsNullOrEmpty(query))
      return;
    int selectedIndex = this.listBox.SelectedIndex;
    int bestIndex = -1;
    int num1 = -1;
    double num2 = 0.0;
    for (int index = 0; index < this.completionData.Count; ++index)
    {
      int matchQuality = this.GetMatchQuality(this.completionData[index].Text, query);
      if (matchQuality >= 0)
      {
        double priority = this.completionData[index].Priority;
        if (num1 < matchQuality || bestIndex != selectedIndex && (index != selectedIndex ? num1 == matchQuality && num2 < priority : num1 == matchQuality))
        {
          bestIndex = index;
          num2 = priority;
          num1 = matchQuality;
        }
      }
    }
    this.SelectIndexCentered(bestIndex);
  }

  private void SelectIndexCentered(int bestIndex)
  {
    if (bestIndex < 0)
    {
      this.listBox.ClearSelection();
    }
    else
    {
      int firstVisibleItem = this.listBox.FirstVisibleItem;
      if (bestIndex < firstVisibleItem || firstVisibleItem + this.listBox.VisibleItemCount <= bestIndex)
      {
        this.listBox.CenterViewOn(bestIndex);
        this.listBox.SelectIndex(bestIndex);
      }
      else
        this.listBox.SelectIndex(bestIndex);
    }
  }

  private int GetMatchQuality(string itemText, string query)
  {
    if (itemText == null)
      throw new ArgumentNullException(nameof (itemText), "ICompletionData.Text returned null");
    if (query == itemText)
      return 8;
    if (string.Equals(itemText, query, StringComparison.InvariantCultureIgnoreCase))
      return 7;
    if (itemText.StartsWith(query, StringComparison.InvariantCulture))
      return 6;
    if (itemText.StartsWith(query, StringComparison.InvariantCultureIgnoreCase))
      return 5;
    bool? nullable1 = new bool?();
    if (query.Length <= 2)
    {
      nullable1 = new bool?(CompletionList.CamelCaseMatch(itemText, query));
      bool? nullable2 = nullable1;
      if ((!nullable2.GetValueOrDefault() ? 0 : (nullable2.HasValue ? 1 : 0)) != 0)
        return 4;
    }
    if (this.IsFiltering)
    {
      if (itemText.IndexOf(query, StringComparison.InvariantCulture) >= 0)
        return 3;
      if (itemText.IndexOf(query, StringComparison.InvariantCultureIgnoreCase) >= 0)
        return 2;
    }
    if (!nullable1.HasValue)
      nullable1 = new bool?(CompletionList.CamelCaseMatch(itemText, query));
    bool? nullable3 = nullable1;
    return (!nullable3.GetValueOrDefault() ? 0 : (nullable3.HasValue ? 1 : 0)) != 0 ? 1 : -1;
  }

  private static bool CamelCaseMatch(string text, string query)
  {
    IEnumerable<char> chars = text.Take<char>(1).Concat<char>(text.Skip<char>(1).Where<char>(new Func<char, bool>(char.IsUpper)));
    int index = 0;
    foreach (char c in chars)
    {
      if (index > query.Length - 1)
        return true;
      if ((int) char.ToUpperInvariant(query[index]) != (int) char.ToUpperInvariant(c))
        return false;
      ++index;
    }
    return index >= query.Length;
  }
}

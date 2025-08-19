// Decompiled with JetBrains decompiler
// Type: System.Windows.Controls.Rating
// Assembly: System.Windows.Controls.Input.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 14A20646-D206-4805-A36B-30DDEEBAF814
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Windows.Controls.Input.Toolkit.dll

using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Automation.Peers;
using System.Windows.Data;
using System.Windows.Input;

#nullable disable
namespace System.Windows.Controls;

[TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
[StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof (RatingItem))]
[TemplateVisualState(Name = "Pressed", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
[TemplateVisualState(Name = "ReadOnly", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Focused", GroupName = "FocusStates")]
[TemplateVisualState(Name = "Unfocused", GroupName = "FocusStates")]
public class Rating : ItemsControl, IUpdateVisualState
{
  protected static readonly DependencyProperty DisplayValueProperty = DependencyProperty.Register(nameof (DisplayValue), typeof (double), typeof (Rating), new PropertyMetadata((object) 0.0, new PropertyChangedCallback(Rating.OnDisplayValueChanged)));
  public static readonly DependencyProperty ItemCountProperty = DependencyProperty.Register(nameof (ItemCount), typeof (int), typeof (Rating), new PropertyMetadata((object) 0, new PropertyChangedCallback(Rating.OnItemCountChanged)));
  public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof (IsReadOnly), typeof (bool), typeof (Rating), new PropertyMetadata((object) false, new PropertyChangedCallback(Rating.OnIsReadOnlyChanged)));
  public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register(nameof (SelectionMode), typeof (RatingSelectionMode), typeof (Rating), new PropertyMetadata((object) RatingSelectionMode.Continuous, new PropertyChangedCallback(Rating.OnSelectionModeChanged)));
  public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof (Value), typeof (double?), typeof (Rating), new PropertyMetadata((object) new double?(), new PropertyChangedCallback(Rating.OnValueChanged)));
  public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, typeof (RoutedPropertyChangedEventHandler<double?>), typeof (Rating));

  protected double DisplayValue
  {
    get => (double) this.GetValue(Rating.DisplayValueProperty);
    set => this.SetValue(Rating.DisplayValueProperty, (object) value);
  }

  private static void OnDisplayValueChanged(
    DependencyObject dependencyObject,
    DependencyPropertyChangedEventArgs eventArgs)
  {
    ((Rating) dependencyObject).OnDisplayValueChanged();
  }

  private void OnDisplayValueChanged() => this.UpdateDisplayValues();

  private RatingItem HoveredRatingItem { get; set; }

  internal InteractionHelper Interaction { get; private set; }

  public int ItemCount
  {
    get => (int) this.GetValue(Rating.ItemCountProperty);
    set => this.SetValue(Rating.ItemCountProperty, (object) value);
  }

  private static void OnItemCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    (d as Rating).OnItemCountChanged((int) e.NewValue);
  }

  private void OnItemCountChanged(int newValue)
  {
    if (newValue < 0)
      throw new ArgumentException(System.Windows.Controls.Properties.Resources.Rating_SetItemCount_ItemCountMustBeLargerThanOrEqualToZero);
    int num = newValue - this.Items.Count;
    if (num > 0)
    {
      for (int index = 0; index < num; ++index)
        this.Items.Add((object) new RatingItem());
    }
    else
    {
      if (num >= 0)
        return;
      for (int index = 0; index < Math.Abs(num); ++index)
        this.Items.RemoveAt(this.Items.Count - 1);
    }
  }

  public bool IsReadOnly
  {
    get => (bool) this.GetValue(Rating.IsReadOnlyProperty);
    set => this.SetValue(Rating.IsReadOnlyProperty, (object) value);
  }

  private static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    ((Rating) d).OnIsReadOnlyChanged((bool) e.OldValue, (bool) e.NewValue);
  }

  protected virtual void OnIsReadOnlyChanged(bool oldValue, bool newValue)
  {
    this.Interaction.OnIsReadOnlyChanged(newValue);
    foreach (RatingItem ratingItem in this.GetRatingItems())
      ratingItem.IsReadOnly = newValue;
    this.UpdateHoverStates();
  }

  public RatingSelectionMode SelectionMode
  {
    get => (RatingSelectionMode) this.GetValue(Rating.SelectionModeProperty);
    set => this.SetValue(Rating.SelectionModeProperty, (object) value);
  }

  private static void OnSelectionModeChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((Rating) d).OnSelectionModeChanged((RatingSelectionMode) e.OldValue, (RatingSelectionMode) e.NewValue);
  }

  protected virtual void OnSelectionModeChanged(
    RatingSelectionMode oldValue,
    RatingSelectionMode newValue)
  {
    this.UpdateDisplayValues();
  }

  [TypeConverter(typeof (NullableConverter<double>))]
  public double? Value
  {
    get => (double?) this.GetValue(Rating.ValueProperty);
    set => this.SetValue(Rating.ValueProperty, (object) value);
  }

  private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    ((Rating) d).OnValueChanged((double?) e.OldValue, (double?) e.NewValue);
  }

  protected virtual void OnValueChanged(double? oldValue, double? newValue)
  {
    this.UpdateValues();
    this.RaiseEvent((RoutedEventArgs) new RoutedPropertyChangedEventArgs<double?>(oldValue, newValue, Rating.ValueChangedEvent));
  }

  protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
  {
    EventHandler layoutUpdated = (EventHandler) null;
    layoutUpdated = (EventHandler) delegate
    {
      this.LayoutUpdated -= layoutUpdated;
      this.UpdateValues();
      this.UpdateDisplayValues();
    };
    this.LayoutUpdated += layoutUpdated;
    this.ItemCount = this.Items.Count;
    base.OnItemsChanged(e);
  }

  public event RoutedPropertyChangedEventHandler<double?> ValueChanged
  {
    add => this.AddHandler(Rating.ValueChangedEvent, (Delegate) value);
    remove => this.RemoveHandler(Rating.ValueChangedEvent, (Delegate) value);
  }

  static Rating()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (Rating), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (Rating)));
  }

  public Rating() => this.Interaction = new InteractionHelper((Control) this);

  protected override void OnMouseEnter(MouseEventArgs e)
  {
    if (this.Interaction.AllowMouseEnter(e))
      this.Interaction.UpdateVisualStateBase(true);
    base.OnMouseEnter(e);
  }

  protected override void OnMouseLeave(MouseEventArgs e)
  {
    if (this.Interaction.AllowMouseLeave(e))
      this.Interaction.UpdateVisualStateBase(true);
    base.OnMouseLeave(e);
  }

  protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
  {
    if (this.Interaction.AllowMouseLeftButtonDown(e))
      this.Interaction.OnMouseLeftButtonDownBase();
    base.OnMouseLeftButtonDown(e);
  }

  protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
  {
    if (this.Interaction.AllowMouseLeftButtonUp(e))
      this.Interaction.OnMouseLeftButtonUpBase();
    base.OnMouseLeftButtonUp(e);
  }

  private void UpdateValues()
  {
    IList<RatingItem> list = (IList<RatingItem>) this.GetRatingItems().ToList<RatingItem>();
    RatingItem selectedRatingItem1 = this.GetSelectedRatingItem();
    foreach (Tuple<RatingItem, double> tuple in EnumerableFunctions.Zip<RatingItem, double, Tuple<RatingItem, double>>((IEnumerable<RatingItem>) list, list.Select<RatingItem, double>((Func<RatingItem, double>) (ratingItem => 1.0)).GetWeightedValues(this.Value.GetValueOrDefault()), (Func<RatingItem, double, Tuple<RatingItem, double>>) ((item, percent) => Tuple.Create<RatingItem, double>(item, percent))))
      tuple.First.Value = tuple.Second;
    RatingItem selectedRatingItem2 = this.GetSelectedRatingItem();
    if (selectedRatingItem1 != selectedRatingItem2)
    {
      if (selectedRatingItem2 != null && AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementSelected))
        UIElementAutomationPeer.CreatePeerForElement((UIElement) selectedRatingItem2)?.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementSelected);
      if (selectedRatingItem1 != null && AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection))
        UIElementAutomationPeer.CreatePeerForElement((UIElement) selectedRatingItem1)?.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection);
    }
    if (this.HoveredRatingItem != null)
      return;
    this.DisplayValue = this.Value.GetValueOrDefault();
  }

  private void UpdateDisplayValues()
  {
    IList<RatingItem> list = (IList<RatingItem>) this.GetRatingItems().ToList<RatingItem>();
    IEnumerable<Tuple<RatingItem, double>> source = EnumerableFunctions.Zip<RatingItem, double, Tuple<RatingItem, double>>((IEnumerable<RatingItem>) list, list.Select<RatingItem, double>((Func<RatingItem, double>) (ratingItem => 1.0)).GetWeightedValues(this.DisplayValue), (Func<RatingItem, double, Tuple<RatingItem, double>>) ((item, percent) => Tuple.Create<RatingItem, double>(item, percent)));
    Tuple<RatingItem, double> tuple1 = source.LastOrDefault<Tuple<RatingItem, double>>((Func<Tuple<RatingItem, double>, bool>) (i => i.Second > 0.0));
    RatingItem ratingItem1 = tuple1 == null ? this.GetSelectedRatingItem() : tuple1.First;
    foreach (Tuple<RatingItem, double> tuple2 in source)
      tuple2.First.DisplayValue = this.SelectionMode != RatingSelectionMode.Individual || tuple2.First == ratingItem1 ? tuple2.Second : 0.0;
  }

  private void UpdateHoverStates()
  {
    if (this.HoveredRatingItem != null && !this.IsReadOnly)
    {
      IList<RatingItem> list = (IList<RatingItem>) this.GetRatingItems().ToList<RatingItem>();
      int num1 = list.IndexOf(this.HoveredRatingItem);
      double num2 = (double) list.Count<RatingItem>();
      this.DisplayValue = (double) (num1 + 1) / num2;
      for (int index = 0; index < list.Count; ++index)
      {
        RatingItem ratingItem = list[index];
        if (index <= num1 && this.SelectionMode == RatingSelectionMode.Continuous)
          VisualStates.GoToState((Control) ratingItem, true, "MouseOver");
        else
          ((IUpdateVisualState) ratingItem).UpdateVisualState(true);
      }
    }
    else
    {
      this.DisplayValue = this.Value.GetValueOrDefault();
      foreach (IUpdateVisualState updateVisualState in this.GetRatingItems().OfType<IUpdateVisualState>())
        updateVisualState.UpdateVisualState(true);
    }
  }

  protected override DependencyObject GetContainerForItemOverride()
  {
    return (DependencyObject) new RatingItem();
  }

  protected override bool IsItemItsOwnContainerOverride(object item) => item is RatingItem;

  protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
  {
    RatingItem ratingItem = (RatingItem) element;
    if (ratingItem.ReadLocalValue(Control.ForegroundProperty) == DependencyProperty.UnsetValue)
      ratingItem.SetBinding(Control.ForegroundProperty, (BindingBase) new Binding("Foreground")
      {
        Source = (object) this
      });
    ratingItem.IsReadOnly = this.IsReadOnly;
    if (ratingItem.Style == null)
      ratingItem.Style = this.ItemContainerStyle;
    ratingItem.Click += new RoutedEventHandler(this.RatingItemClick);
    ratingItem.MouseEnter += new MouseEventHandler(this.RatingItemMouseEnter);
    ratingItem.MouseLeave += new MouseEventHandler(this.RatingItemMouseLeave);
    ratingItem.ParentRating = this;
    base.PrepareContainerForItemOverride(element, item);
  }

  protected override void ClearContainerForItemOverride(DependencyObject element, object item)
  {
    RatingItem ratingItem = (RatingItem) element;
    ratingItem.Click -= new RoutedEventHandler(this.RatingItemClick);
    ratingItem.MouseEnter -= new MouseEventHandler(this.RatingItemMouseEnter);
    ratingItem.MouseLeave -= new MouseEventHandler(this.RatingItemMouseLeave);
    ratingItem.ParentRating = (Rating) null;
    if (ratingItem == this.HoveredRatingItem)
    {
      this.HoveredRatingItem = (RatingItem) null;
      this.UpdateDisplayValues();
      this.UpdateHoverStates();
    }
    base.ClearContainerForItemOverride(element, item);
  }

  private void RatingItemMouseEnter(object sender, MouseEventArgs e)
  {
    this.HoveredRatingItem = (RatingItem) sender;
    this.UpdateHoverStates();
  }

  private void RatingItemMouseLeave(object sender, MouseEventArgs e)
  {
    this.HoveredRatingItem = (RatingItem) null;
    this.UpdateDisplayValues();
    this.UpdateHoverStates();
  }

  internal IEnumerable<RatingItem> GetRatingItems() => this.Items.Cast<RatingItem>();

  internal void SelectRatingItem(RatingItem selectedRatingItem)
  {
    if (this.IsReadOnly)
      return;
    IList<RatingItem> list = (IList<RatingItem>) this.GetRatingItems().ToList<RatingItem>();
    IEnumerable<double> source = list.Select<RatingItem, double>((Func<RatingItem, double>) (ratingItem => 1.0));
    double num = (double) list.Count<RatingItem>();
    if (num == 0.0)
      return;
    this.Value = new double?(source.Take<double>(list.IndexOf(selectedRatingItem) + 1).Sum() / num);
  }

  private void RatingItemClick(object sender, RoutedEventArgs e)
  {
    if (this.IsReadOnly)
      return;
    this.OnRatingItemValueSelected((RatingItem) sender, 1.0);
  }

  private RatingItem GetSelectedRatingItem()
  {
    return this.GetRatingItems().LastOrDefault<RatingItem>((Func<RatingItem, bool>) (ratingItem => ratingItem.Value > 0.0));
  }

  protected virtual void OnRatingItemValueSelected(RatingItem ratingItem, double newValue)
  {
    List<RatingItem> list = this.GetRatingItems().ToList<RatingItem>();
    double num = (double) list.Count<RatingItem>();
    this.Value = new double?(((double) list.Take<RatingItem>(list.IndexOf(ratingItem)).Count<RatingItem>() + newValue) / num);
  }

  protected override AutomationPeer OnCreateAutomationPeer()
  {
    return (AutomationPeer) new RatingAutomationPeer(this);
  }

  protected override void OnKeyDown(KeyEventArgs e)
  {
    if (!this.Interaction.AllowKeyDown(e))
      return;
    base.OnKeyDown(e);
    if (e.Handled)
      return;
    switch (e.Key)
    {
      case Key.Left:
        RatingItem ratingItem1 = !(FocusManager.GetFocusedElement((DependencyObject) Application.Current.MainWindow) is RatingItem focusedElement1) ? this.GetRatingItems().FirstOrDefault<RatingItem>() : this.GetRatingItemAtOffsetFrom(focusedElement1, -1);
        if (ratingItem1 == null || !ratingItem1.Focus())
          break;
        e.Handled = true;
        break;
      case Key.Right:
        RatingItem ratingItem2 = !(FocusManager.GetFocusedElement((DependencyObject) Application.Current.MainWindow) is RatingItem focusedElement2) ? this.GetRatingItems().FirstOrDefault<RatingItem>() : this.GetRatingItemAtOffsetFrom(focusedElement2, 1);
        if (ratingItem2 == null || !ratingItem2.Focus())
          break;
        e.Handled = true;
        break;
      case Key.Add:
        if (this.IsReadOnly)
          break;
        RatingItem selectedRatingItem = this.GetSelectedRatingItem();
        RatingItem ratingItem3 = selectedRatingItem == null ? this.GetRatingItems().FirstOrDefault<RatingItem>() : this.GetRatingItemAtOffsetFrom(selectedRatingItem, 1);
        if (ratingItem3 == null)
          break;
        ratingItem3.SelectValue();
        e.Handled = true;
        break;
      case Key.Subtract:
        if (this.IsReadOnly)
          break;
        RatingItem ratingItem4 = this.GetSelectedRatingItem();
        if (ratingItem4 != null)
          ratingItem4 = this.GetRatingItemAtOffsetFrom(ratingItem4, -1);
        if (ratingItem4 == null)
          break;
        ratingItem4.SelectValue();
        e.Handled = true;
        break;
    }
  }

  private RatingItem GetRatingItemAtOffsetFrom(RatingItem ratingItem, int offset)
  {
    IList<RatingItem> list = (IList<RatingItem>) this.GetRatingItems().ToList<RatingItem>();
    int num = list.IndexOf(ratingItem);
    if (num == -1)
      return (RatingItem) null;
    int index = num + offset;
    ratingItem = index < 0 || index >= list.Count ? (RatingItem) null : list[index];
    return ratingItem;
  }

  void IUpdateVisualState.UpdateVisualState(bool useTransitions)
  {
    this.Interaction.UpdateVisualStateBase(useTransitions);
  }
}

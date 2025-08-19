// Decompiled with JetBrains decompiler
// Type: System.Windows.Controls.Accordion
// Assembly: System.Windows.Controls.Layout.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 2878816D-F7B3-441D-96A5-F68332B17866
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Windows.Controls.Layout.Toolkit.dll

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

#nullable disable
namespace System.Windows.Controls;

[StyleTypedProperty(Property = "AccordionButtonStyle", StyleTargetType = typeof (AccordionButton))]
[TemplateVisualState(Name = "Focused", GroupName = "FocusStates")]
[TemplateVisualState(Name = "Unfocused", GroupName = "FocusStates")]
[TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
[StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof (AccordionItem))]
[TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
[TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Pressed", GroupName = "CommonStates")]
public class Accordion : ItemsControl, IUpdateVisualState
{
  private const string AccordionButtonStyleName = "AccordionButtonStyle";
  private readonly List<AccordionItem> _scheduledActions;
  private bool _isAllowedToWriteSelectedItems;
  private bool _isAllowedToWriteSelectedIndices;
  private bool _isIgnoringSelectedIndicesChanges;
  private bool _isIgnoringSelectedItemsChanges;
  private bool _isInSelectedItemsCollectionChanged;
  private bool _isInSelectedIndicesCollectionChanged;
  private AccordionItem _currentActioningItem;
  public static readonly DependencyProperty ExpandDirectionProperty = DependencyProperty.Register(nameof (ExpandDirection), typeof (ExpandDirection), typeof (Accordion), new PropertyMetadata((object) ExpandDirection.Down, new PropertyChangedCallback(Accordion.OnExpandDirectionPropertyChanged)));
  public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register(nameof (SelectionMode), typeof (AccordionSelectionMode), typeof (Accordion), new PropertyMetadata((object) AccordionSelectionMode.One, new PropertyChangedCallback(Accordion.OnSelectionModePropertyChanged)));
  public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(nameof (SelectedItem), typeof (object), typeof (Accordion), new PropertyMetadata((object) null, new PropertyChangedCallback(Accordion.OnSelectedItemPropertyChanged)));
  private int _selectedItemNestedLevel;
  public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(nameof (SelectedIndex), typeof (int), typeof (Accordion), new PropertyMetadata((object) -1, new PropertyChangedCallback(Accordion.OnSelectedIndexPropertyChanged)));
  private int _selectedIndexNestedLevel;
  public static readonly DependencyProperty SelectionSequenceProperty = DependencyProperty.Register(nameof (SelectionSequence), typeof (SelectionSequence), typeof (Accordion), new PropertyMetadata((object) SelectionSequence.Simultaneous, new PropertyChangedCallback(Accordion.OnSelectionSequencePropertyChanged)));
  public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(nameof (SelectedItems), typeof (IList), typeof (Accordion), new PropertyMetadata(new PropertyChangedCallback(Accordion.OnSelectedItemsChanged)));
  public static readonly DependencyProperty SelectedIndicesProperty = DependencyProperty.Register(nameof (SelectedIndices), typeof (IList<int>), typeof (Accordion), new PropertyMetadata((object) null, new PropertyChangedCallback(Accordion.OnSelectedIndicesChanged)));
  public static readonly DependencyProperty AccordionButtonStyleProperty = DependencyProperty.Register(nameof (AccordionButtonStyle), typeof (Style), typeof (Accordion), new PropertyMetadata((object) null, new PropertyChangedCallback(Accordion.OnAccordionButtonStylePropertyChanged)));
  public static readonly DependencyProperty ContentTemplateProperty = DependencyProperty.Register(nameof (ContentTemplate), typeof (DataTemplate), typeof (Accordion), new PropertyMetadata((PropertyChangedCallback) null));
  public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent("SelectionChanged", RoutingStrategy.Bubble, typeof (SelectionChangedEventHandler), typeof (Accordion));

  internal ItemsControlHelper ItemsControlHelper { get; private set; }

  internal bool IsResizing { get; private set; }

  private InteractionHelper Interaction { get; set; }

  public ExpandDirection ExpandDirection
  {
    get => (ExpandDirection) this.GetValue(Accordion.ExpandDirectionProperty);
    set => this.SetValue(Accordion.ExpandDirectionProperty, (object) value);
  }

  private static void OnExpandDirectionPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    Accordion accordion = (Accordion) d;
    ExpandDirection newValue = (ExpandDirection) e.NewValue;
    switch (newValue)
    {
      case ExpandDirection.Down:
      case ExpandDirection.Up:
      case ExpandDirection.Left:
      case ExpandDirection.Right:
        for (int index = 0; index < accordion.Items.Count; ++index)
        {
          if (accordion.ItemContainerGenerator.ContainerFromIndex(index) is AccordionItem accordionItem)
            accordionItem.ExpandDirection = newValue;
        }
        accordion.SetPanelOrientation();
        accordion.Dispatcher.BeginInvoke((Delegate) new Action(accordion.LayoutChildren));
        break;
      default:
        accordion.SetValue(Accordion.ExpandDirectionProperty, e.OldValue);
        throw new ArgumentOutOfRangeException(nameof (e), string.Format((IFormatProvider) CultureInfo.InvariantCulture, System.Windows.Controls.Properties.Resources.Accordion_OnExpandDirectionPropertyChanged_InvalidValue, (object) newValue));
    }
  }

  public AccordionSelectionMode SelectionMode
  {
    get => (AccordionSelectionMode) this.GetValue(Accordion.SelectionModeProperty);
    set => this.SetValue(Accordion.SelectionModeProperty, (object) value);
  }

  private static void OnSelectionModePropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    Accordion accordion = (Accordion) d;
    AccordionSelectionMode newValue = (AccordionSelectionMode) e.NewValue;
    switch (newValue)
    {
      case AccordionSelectionMode.One:
      case AccordionSelectionMode.OneOrMore:
      case AccordionSelectionMode.ZeroOrOne:
      case AccordionSelectionMode.ZeroOrMore:
        for (int index = 0; index < accordion.Items.Count; ++index)
        {
          if (accordion.ItemContainerGenerator.ContainerFromIndex(index) is AccordionItem accordionItem)
            accordionItem.IsLocked = false;
        }
        if (accordion.IsMinimumOneSelected && accordion.GetValue(Accordion.SelectedItemProperty) == null && accordion.Items.Count > 0)
          accordion.SetValue(Accordion.SelectedItemProperty, accordion.Items[0]);
        if (accordion.IsMaximumOneSelected && accordion.SelectedIndices.Count > 1)
        {
          foreach (int index in accordion.SelectedIndices.ToList<int>())
          {
            if (index != accordion.SelectedIndex)
              accordion.UnselectItem(index, (object) null);
          }
        }
        accordion.SetLockedProperties();
        break;
      default:
        accordion.SetValue(Accordion.SelectionModeProperty, e.OldValue);
        throw new ArgumentOutOfRangeException(nameof (e), string.Format((IFormatProvider) CultureInfo.InvariantCulture, System.Windows.Controls.Properties.Resources.Accordion_OnSelectionModePropertyChanged_InvalidValue, (object) newValue));
    }
  }

  private bool IsMinimumOneSelected
  {
    get
    {
      return this.SelectionMode == AccordionSelectionMode.One || this.SelectionMode == AccordionSelectionMode.OneOrMore;
    }
  }

  private bool IsMaximumOneSelected
  {
    get
    {
      return this.SelectionMode == AccordionSelectionMode.One || this.SelectionMode == AccordionSelectionMode.ZeroOrOne;
    }
  }

  public object SelectedItem
  {
    get => this.GetValue(Accordion.SelectedItemProperty);
    set => this.SetValue(Accordion.SelectedItemProperty, value);
  }

  private static void OnSelectedItemPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    Accordion source = (Accordion) d;
    object oldValue = e.OldValue;
    object newValue = e.NewValue;
    object[] objArray1;
    if (newValue != null)
      objArray1 = new object[1]{ newValue };
    else
      objArray1 = new object[0];
    object[] addedItems = objArray1;
    object[] objArray2;
    if (oldValue != null)
      objArray2 = new object[1]{ oldValue };
    else
      objArray2 = new object[0];
    object[] removedItems = objArray2;
    if (oldValue != null && oldValue.Equals(newValue))
      source.OnSelectedItemChanged(new SelectionChangedEventArgs(Accordion.SelectionChangedEvent, (IList) removedItems, (IList) addedItems));
    else if (!source.IsValidItemForSelection(newValue))
    {
      ++source._selectedItemNestedLevel;
      source.SetValue(Accordion.SelectedItemProperty, oldValue);
      --source._selectedItemNestedLevel;
    }
    else
    {
      if (source._selectedItemNestedLevel != 0)
        return;
      if (newValue == null)
      {
        source.SelectedIndex = -1;
      }
      else
      {
        int index = source.SelectedIndex;
        if (index < 0 || index > source.Items.Count || !newValue.Equals(source.Items[index]))
        {
          IEnumerable<int> source1 = source.SelectedIndices.Where<int>((Func<int, bool>) (i => i >= 0 && i < source.Items.Count && newValue.Equals(source.Items[i])));
          index = source1.Count<int>() > 0 ? source1.First<int>() : source.Items.IndexOf(newValue);
        }
        source.SelectedIndex = index;
      }
      source.OnSelectedItemChanged(new SelectionChangedEventArgs(Accordion.SelectionChangedEvent, (IList) removedItems, (IList) addedItems));
    }
  }

  private bool IsValidItemForSelection(object newValue)
  {
    if (newValue != null)
      return this.Items.OfType<object>().Contains<object>(newValue);
    return !this.IsMinimumOneSelected || this.Items.Count == 0;
  }

  public int SelectedIndex
  {
    get => (int) this.GetValue(Accordion.SelectedIndexProperty);
    set => this.SetValue(Accordion.SelectedIndexProperty, (object) value);
  }

  private static void OnSelectedIndexPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    Accordion accordion = (Accordion) d;
    int oldValue = (int) e.OldValue;
    int newValue = (int) e.NewValue;
    if (accordion._isIgnoringSelectedIndicesChanges)
      return;
    if (!accordion.IsValidIndexForSelection(newValue))
    {
      if (accordion.IsValidIndexForSelection(oldValue))
      {
        ++accordion._selectedIndexNestedLevel;
        accordion.SetValue(Accordion.SelectedIndexProperty, (object) oldValue);
        --accordion._selectedIndexNestedLevel;
      }
      else
        accordion.SetValue(Accordion.SelectedIndexProperty, (object) accordion.ProposeSelectedIndexCandidate(newValue));
    }
    else
    {
      if (accordion._selectedIndexNestedLevel != 0)
        return;
      accordion.SelectedItem = newValue < 0 || newValue >= accordion.Items.Count ? (object) null : accordion.Items[newValue];
      accordion.ChangeSelectedIndex(oldValue, newValue);
    }
  }

  private bool IsValidIndexForSelection(int newValue)
  {
    return newValue == -1 ? !this.IsMinimumOneSelected || this.Items.Count == 0 : newValue >= 0 && newValue < this.Items.Count;
  }

  public SelectionSequence SelectionSequence
  {
    get => (SelectionSequence) this.GetValue(Accordion.SelectionSequenceProperty);
    set => this.SetValue(Accordion.SelectionSequenceProperty, (object) value);
  }

  private static void OnSelectionSequencePropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    SelectionSequence newValue = (SelectionSequence) e.NewValue;
    switch (newValue)
    {
      case SelectionSequence.CollapseBeforeExpand:
        break;
      case SelectionSequence.Simultaneous:
        break;
      default:
        d.SetValue(Accordion.SelectionSequenceProperty, e.OldValue);
        throw new ArgumentOutOfRangeException(nameof (e), string.Format((IFormatProvider) CultureInfo.InvariantCulture, System.Windows.Controls.Properties.Resources.Accordion_OnSelectionSequencepropertyChanged_InvalidValue, (object) newValue));
    }
  }

  public IList SelectedItems
  {
    get => this.GetValue(Accordion.SelectedItemsProperty) as IList;
    private set
    {
      this._isAllowedToWriteSelectedItems = true;
      this.SetValue(Accordion.SelectedItemsProperty, (object) value);
      this._isAllowedToWriteSelectedItems = false;
    }
  }

  private static void OnSelectedItemsChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    Accordion accordion = (Accordion) d;
    if (!accordion._isAllowedToWriteSelectedItems)
    {
      accordion.SelectedItems = e.OldValue as IList;
      throw new InvalidOperationException(System.Windows.Controls.Properties.Resources.Accordion_OnSelectedItemsChanged_InvalidWrite);
    }
  }

  public IList<int> SelectedIndices
  {
    get => this.GetValue(Accordion.SelectedIndicesProperty) as IList<int>;
    private set
    {
      this._isAllowedToWriteSelectedIndices = true;
      this.SetValue(Accordion.SelectedIndicesProperty, (object) value);
      this._isAllowedToWriteSelectedIndices = false;
    }
  }

  private static void OnSelectedIndicesChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    Accordion accordion = (Accordion) d;
    if (!accordion._isAllowedToWriteSelectedIndices)
    {
      accordion.SelectedIndices = e.OldValue as IList<int>;
      throw new InvalidOperationException(System.Windows.Controls.Properties.Resources.Accordion_OnSelectedIndicesChanged_InvalidWrite);
    }
  }

  public Style AccordionButtonStyle
  {
    get => this.GetValue(Accordion.AccordionButtonStyleProperty) as Style;
    set => this.SetValue(Accordion.AccordionButtonStyleProperty, (object) value);
  }

  private static void OnAccordionButtonStylePropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
  }

  public DataTemplate ContentTemplate
  {
    get => (DataTemplate) this.GetValue(Accordion.ContentTemplateProperty);
    set => this.SetValue(Accordion.ContentTemplateProperty, (object) value);
  }

  public event SelectionChangedEventHandler SelectionChanged
  {
    add => this.AddHandler(Accordion.SelectionChangedEvent, (Delegate) value);
    remove => this.RemoveHandler(Accordion.SelectionChangedEvent, (Delegate) value);
  }

  public event NotifyCollectionChangedEventHandler SelectedItemsChanged;

  static Accordion()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (Accordion), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (Accordion)));
  }

  public Accordion()
  {
    this.ItemsControlHelper = new ItemsControlHelper((ItemsControl) this);
    ObservableCollection<object> observableCollection1 = new ObservableCollection<object>();
    ObservableCollection<int> observableCollection2 = new ObservableCollection<int>();
    this.SelectedItems = (IList) observableCollection1;
    this.SelectedIndices = (IList<int>) observableCollection2;
    observableCollection1.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnSelectedItemsCollectionChanged);
    observableCollection2.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnSelectedIndicesCollectionChanged);
    this._scheduledActions = new List<AccordionItem>();
    this.SizeChanged += new SizeChangedEventHandler(this.OnAccordionSizeChanged);
    this.Interaction = new InteractionHelper((Control) this);
  }

  public override void OnApplyTemplate()
  {
    this.ItemsControlHelper.OnApplyTemplate();
    base.OnApplyTemplate();
    this.Interaction.OnApplyTemplateBase();
  }

  protected override AutomationPeer OnCreateAutomationPeer()
  {
    return (AutomationPeer) new AccordionAutomationPeer(this);
  }

  protected override DependencyObject GetContainerForItemOverride()
  {
    return (DependencyObject) new AccordionItem();
  }

  protected override bool IsItemItsOwnContainerOverride(object item) => item is AccordionItem;

  protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
  {
    if (element is AccordionItem accordionItem)
    {
      DataTemplate contentTemplate1 = accordionItem.ContentTemplate;
      base.PrepareContainerForItemOverride(element, item);
      ItemsControlHelper.PrepareContainerForItemOverride((DependencyObject) accordionItem, this.ItemContainerStyle);
      AccordionItem.PreparePrepareHeaderedContentControlContainerForItemOverride((HeaderedContentControl) accordionItem, item, (ItemsControl) this, this.ItemContainerStyle);
      DataTemplate contentTemplate2 = accordionItem.ContentTemplate;
      accordionItem.ContentTemplate = contentTemplate1;
      if (accordionItem.ContentTemplate == null)
        accordionItem.SetBinding(ContentControl.ContentTemplateProperty, (BindingBase) new Binding("ContentTemplate")
        {
          Source = (object) this,
          Mode = BindingMode.OneWay
        });
      if (accordionItem.HeaderTemplate == null)
        accordionItem.SetBinding(HeaderedContentControl.HeaderTemplateProperty, (BindingBase) new Binding("ItemTemplate")
        {
          Source = (object) this,
          Mode = BindingMode.OneWay
        });
      if (accordionItem.AccordionButtonStyle == null)
        accordionItem.SetBinding(AccordionItem.AccordionButtonStyleProperty, (BindingBase) new Binding("AccordionButtonStyle")
        {
          Source = (object) this,
          Mode = BindingMode.OneWay
        });
      if (contentTemplate2 != null && !string.IsNullOrEmpty(this.DisplayMemberPath))
      {
        if (accordionItem.ContentTemplate == null)
          accordionItem.ContentTemplate = contentTemplate2;
        if (accordionItem.HeaderTemplate == null)
          accordionItem.HeaderTemplate = contentTemplate2;
      }
      accordionItem.ParentAccordion = this;
      int num = this.ItemContainerGenerator.IndexFromContainer((DependencyObject) accordionItem);
      if (!accordionItem.IsSelected && this.SelectedIndices.Contains(num))
        accordionItem.IsSelected = true;
      if (accordionItem.IsSelected)
        this.SelectedItem = item;
      if (accordionItem.IsSelected)
      {
        if (!this.SelectedItems.OfType<object>().Contains<object>(item))
          this.SelectedItems.Add(item);
        if (!this.SelectedIndices.Contains(num))
          this.SelectedIndices.Add(num);
      }
      accordionItem.ExpandDirection = this.ExpandDirection;
    }
    else
    {
      base.PrepareContainerForItemOverride(element, item);
      ItemsControlHelper.PrepareContainerForItemOverride(element, this.ItemContainerStyle);
    }
    this.SetPanelOrientation();
    this.SetLockedProperties();
    this.Dispatcher.BeginInvoke((Delegate) new Action(this.LayoutChildren));
  }

  protected override void ClearContainerForItemOverride(DependencyObject element, object item)
  {
    if (element is AccordionItem accordionItem)
    {
      accordionItem.IsLocked = false;
      accordionItem.IsSelected = false;
      accordionItem.ParentAccordion = (Accordion) null;
    }
    base.ClearContainerForItemOverride(element, item);
  }

  protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
  {
    base.OnItemsChanged(e);
    switch (e.Action)
    {
      case NotifyCollectionChangedAction.Add:
        try
        {
          this._isIgnoringSelectedIndicesChanges = true;
          for (int index = 0; index < this.SelectedIndices.Count; ++index)
          {
            if (this.SelectedIndices[index] >= e.NewStartingIndex)
              ++this.SelectedIndices[index];
          }
        }
        finally
        {
          this._isIgnoringSelectedIndicesChanges = false;
        }
        if (this.SelectedIndex >= e.NewStartingIndex && this.SelectedIndex > -1)
          ++this.SelectedIndex;
        if (this.SelectedItem == null && this.IsMinimumOneSelected)
        {
          if (!this.SelectedItems.OfType<object>().Contains<object>(e.NewItems[0]))
            this.SelectedItems.Add(e.NewItems[0]);
          this.SelectedItem = e.NewItems[0];
          break;
        }
        break;
      case NotifyCollectionChangedAction.Remove:
        int oldStartingIndex = e.OldStartingIndex;
        object item = e.OldItems[0];
        try
        {
          this._isIgnoringSelectedIndicesChanges = true;
          if (this.SelectedIndices.Contains(oldStartingIndex))
            this.SelectedIndices.Remove(oldStartingIndex);
          for (int index = 0; index < this.SelectedIndices.Count; ++index)
          {
            if (this.SelectedIndices[index] > oldStartingIndex)
              --this.SelectedIndices[index];
          }
        }
        finally
        {
          this._isIgnoringSelectedIndicesChanges = false;
        }
        try
        {
          this._isIgnoringSelectedItemsChanges = true;
          if (this.SelectedItems.Contains(item))
          {
            if (this.SelectedIndices.Count<int>((Func<int, bool>) (i => i < this.Items.Count && this.Items[i].Equals(item))) == 0)
              this.SelectedItems.Remove(item);
          }
        }
        finally
        {
          this._isIgnoringSelectedItemsChanges = false;
        }
        if (this.SelectedIndex == oldStartingIndex)
          this.SelectedIndex = -1;
        if (this.SelectedIndex > e.OldStartingIndex && this.SelectedIndex > -1)
        {
          --this.SelectedIndex;
          break;
        }
        break;
      case NotifyCollectionChangedAction.Reset:
        this._isIgnoringSelectedIndicesChanges = true;
        this._isIgnoringSelectedItemsChanges = true;
        try
        {
          this.SelectedItems.Clear();
          this.SelectedIndices.Clear();
          this.SelectedItem = (object) null;
          this.SelectedIndex = -1;
        }
        finally
        {
          this._isIgnoringSelectedIndicesChanges = false;
          this._isIgnoringSelectedItemsChanges = false;
        }
        this.InitializeNewItemsSource();
        break;
    }
    this.SetPanelOrientation();
  }

  private void InitializeNewItemsSource()
  {
    if (!this.IsMinimumOneSelected || this.SelectedItem != null || this.Items.Count <= 0)
      return;
    if (!this.SelectedItems.OfType<object>().Contains<object>(this.Items[0]))
      this.SelectedItems.Add(this.Items[0]);
    this.SelectedItem = this.Items[0];
  }

  internal void OnAccordionItemUnselected(AccordionItem accordionItem)
  {
    this.UnselectItem(this.ItemContainerGenerator.IndexFromContainer((DependencyObject) accordionItem), this.ItemContainerGenerator.ItemFromContainer((DependencyObject) accordionItem));
  }

  private void UnselectItem(int index, object item)
  {
    if (index < 0 || index > this.Items.Count)
      return;
    AccordionItem accordionItem = index < 0 || index >= this.Items.Count ? (AccordionItem) null : this.ItemContainerGenerator.ContainerFromIndex(index) as AccordionItem;
    if (accordionItem != null && accordionItem.IsSelected)
    {
      accordionItem.IsLocked = false;
      accordionItem.IsSelected = false;
    }
    else
    {
      item = item ?? this.Items[index];
      int num = -1;
      if (this.SelectedIndex > -1 && this.SelectedIndex == index)
      {
        num = this.ProposeSelectedIndexCandidate(index);
        this.SelectedIndex = num;
      }
      if (this.SelectedItems.OfType<object>().Contains<object>(item) && index != num && !item.Equals(this.SelectedItem) && this.SelectedIndices.Count<int>((Func<int, bool>) (i => i != index && i < this.Items.Count && this.Items[i].Equals(item))) == 0)
      {
        if (this._isInSelectedItemsCollectionChanged)
          throw new InvalidOperationException(System.Windows.Controls.Properties.Resources.Accordion_InvalidManipulationOfSelectionCollections);
        this.SelectedItems.Remove(item);
      }
      if (!this.SelectedIndices.Contains(index))
        return;
      if (this._isInSelectedIndicesCollectionChanged)
        throw new InvalidOperationException(System.Windows.Controls.Properties.Resources.Accordion_InvalidManipulationOfSelectionCollections);
      this.SelectedIndices.Remove(index);
    }
  }

  internal void OnAccordionItemSelected(AccordionItem accordionItem)
  {
    this.SelectItem(this.ItemContainerGenerator.IndexFromContainer((DependencyObject) accordionItem));
  }

  private void SelectItem(int index)
  {
    AccordionItem accordionItem = index < 0 || index >= this.Items.Count ? (AccordionItem) null : this.ItemContainerGenerator.ContainerFromIndex(index) as AccordionItem;
    if (accordionItem != null && !accordionItem.IsSelected)
    {
      accordionItem.IsSelected = true;
    }
    else
    {
      this.SelectedIndex = index;
      object obj = this.Items[index];
      if (obj == null)
        return;
      if (!this.SelectedItems.OfType<object>().Contains<object>(obj))
      {
        if (this._isInSelectedItemsCollectionChanged)
          throw new InvalidOperationException(System.Windows.Controls.Properties.Resources.Accordion_InvalidManipulationOfSelectionCollections);
        this.SelectedItems.Add(obj);
      }
      if (this.SelectedIndices.Contains(index))
        return;
      if (this._isInSelectedIndicesCollectionChanged)
        throw new InvalidOperationException(System.Windows.Controls.Properties.Resources.Accordion_InvalidManipulationOfSelectionCollections);
      this.SelectedIndices.Add(index);
    }
  }

  private void ChangeSelectedIndex(int oldIndex, int newIndex)
  {
    AccordionItem accordionItem = oldIndex < 0 || oldIndex >= this.Items.Count ? (AccordionItem) null : this.ItemContainerGenerator.ContainerFromIndex(oldIndex) as AccordionItem;
    AccordionItem element = newIndex < 0 || newIndex >= this.Items.Count ? (AccordionItem) null : this.ItemContainerGenerator.ContainerFromIndex(newIndex) as AccordionItem;
    if (oldIndex != newIndex && (this.IsMaximumOneSelected || newIndex == -1))
    {
      if (accordionItem != null)
      {
        accordionItem.IsLocked = false;
        accordionItem.IsSelected = false;
      }
      else if (oldIndex > -1)
        this.UnselectItem(oldIndex, (object) null);
      if (element != null && AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementSelected))
        UIElementAutomationPeer.CreatePeerForElement((UIElement) element)?.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementSelected);
    }
    if (element != null)
      element.IsSelected = true;
    else if (newIndex != -1)
      this.SelectItem(newIndex);
    this.SelectedIndex = newIndex;
  }

  private void OnSelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
  {
    if (this._isIgnoringSelectedItemsChanges)
      return;
    this._isInSelectedItemsCollectionChanged = true;
    Action<object> action = (Action<object>) (item =>
    {
      List<int> list = this.SelectedIndices.Where<int>((Func<int, bool>) (i => i < this.Items.Count && item.Equals(this.Items[i]))).ToList<int>();
      if (list.Count > 0)
      {
        foreach (int index in list)
          this.UnselectItem(index, item);
      }
      else
        this.UnselectItem(this.Items.IndexOf(item), item);
    });
    switch (e.Action)
    {
      case NotifyCollectionChangedAction.Add:
        if (this.IsMaximumOneSelected && this.SelectedItem != null && !e.NewItems.Contains(this.SelectedItem))
          throw new InvalidOperationException(System.Windows.Controls.Properties.Resources.Accordion_InvalidManipulationOfSelectionCollections);
        IEnumerator enumerator1 = e.NewItems.GetEnumerator();
        try
        {
          while (enumerator1.MoveNext())
            this.SelectedItem = enumerator1.Current;
          break;
        }
        finally
        {
          if (enumerator1 is IDisposable disposable)
            disposable.Dispose();
        }
      case NotifyCollectionChangedAction.Remove:
        if (this.IsMinimumOneSelected && e.OldItems.Contains(this.SelectedItem))
          throw new InvalidOperationException(System.Windows.Controls.Properties.Resources.Accordion_InvalidManipulationOfSelectionCollections);
        IEnumerator enumerator2 = e.OldItems.GetEnumerator();
        try
        {
          while (enumerator2.MoveNext())
          {
            object current = enumerator2.Current;
            action(current);
          }
          break;
        }
        finally
        {
          if (enumerator2 is IDisposable disposable)
            disposable.Dispose();
        }
      case NotifyCollectionChangedAction.Reset:
        if (this.IsMinimumOneSelected && this.Items.Count > 0)
          throw new InvalidOperationException(System.Windows.Controls.Properties.Resources.Accordion_InvalidManipulationOfSelectionCollections);
        for (int index = this.SelectedIndices.Count - 1; index >= 0; --index)
        {
          int selectedIndex = this.SelectedIndices[index];
          if (selectedIndex < this.Items.Count)
          {
            object obj = this.Items[selectedIndex];
            action(obj);
          }
        }
        break;
      default:
        throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, System.Windows.Controls.Properties.Resources.Accordion_UnsupportedCollectionAction, (object) e.Action));
    }
    this.RaiseOnSelectedItemsCollectionChanged(e);
    this._isInSelectedItemsCollectionChanged = false;
  }

  private void OnSelectedIndicesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
  {
    if (this._isIgnoringSelectedIndicesChanges)
      return;
    this._isInSelectedIndicesCollectionChanged = true;
    switch (e.Action)
    {
      case NotifyCollectionChangedAction.Add:
        if (this.IsMaximumOneSelected && (this.SelectedItem != null && e.NewItems.Count != 1 || (int) e.NewItems[0] < this.Items.Count && !this.Items[(int) e.NewItems[0]].Equals(this.SelectedItem)))
          throw new InvalidOperationException(System.Windows.Controls.Properties.Resources.Accordion_InvalidManipulationOfSelectionCollections);
        IEnumerator enumerator1 = e.NewItems.GetEnumerator();
        try
        {
          while (enumerator1.MoveNext())
          {
            int current = (int) enumerator1.Current;
            if (current < this.Items.Count)
            {
              this.SelectedIndex = current;
              if (AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementAddedToSelection))
                UIElementAutomationPeer.CreatePeerForElement((UIElement) this)?.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementAddedToSelection);
            }
          }
          break;
        }
        finally
        {
          if (enumerator1 is IDisposable disposable)
            disposable.Dispose();
        }
      case NotifyCollectionChangedAction.Remove:
        if (this.IsMinimumOneSelected && e.OldItems.Contains((object) this.SelectedIndex) && this.SelectedIndex < this.Items.Count && this.Items[this.SelectedIndex].Equals(this.SelectedItem) && this.SelectedIndices.Count == 0)
          throw new InvalidOperationException(System.Windows.Controls.Properties.Resources.Accordion_InvalidManipulationOfSelectionCollections);
        IEnumerator enumerator2 = e.OldItems.GetEnumerator();
        try
        {
          while (enumerator2.MoveNext())
          {
            int current = (int) enumerator2.Current;
            if (current < this.Items.Count)
            {
              this.UnselectItem(current, (object) null);
              if (AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection))
                UIElementAutomationPeer.CreatePeerForElement((UIElement) this)?.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection);
            }
          }
          break;
        }
        finally
        {
          if (enumerator2 is IDisposable disposable)
            disposable.Dispose();
        }
      case NotifyCollectionChangedAction.Reset:
        if (this.IsMinimumOneSelected && this.Items.Count > 0)
          throw new InvalidOperationException(System.Windows.Controls.Properties.Resources.Accordion_InvalidManipulationOfSelectionCollections);
        for (int index = this.SelectedItems.Count - 1; index >= 0; --index)
        {
          object selectedItem = this.SelectedItems[index];
          this.UnselectItem(index, selectedItem);
          if (AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection))
            UIElementAutomationPeer.CreatePeerForElement((UIElement) this)?.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection);
        }
        break;
      default:
        throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, System.Windows.Controls.Properties.Resources.Accordion_UnsupportedCollectionAction, (object) e.Action));
    }
    this.SetLockedProperties();
    this.LayoutChildren();
    this._isInSelectedIndicesCollectionChanged = false;
  }

  private int ProposeSelectedIndexCandidate(int nonCandidateIndex)
  {
    object item = nonCandidateIndex < 0 || nonCandidateIndex >= this.Items.Count ? (object) null : this.Items[nonCandidateIndex];
    IEnumerable<int> source = this.SelectedIndices.Where<int>((Func<int, bool>) (i =>
    {
      if (i == nonCandidateIndex)
        return false;
      return item == null || !item.Equals(this.Items[i]);
    }));
    if (source.Count<int>() > 0)
      return source.First<int>();
    return this.IsMinimumOneSelected && this.Items.Count > 0 ? 0 : -1;
  }

  public void SelectAll() => this.UpdateAccordionItemsSelection(true);

  public void UnselectAll() => this.UpdateAccordionItemsSelection(false);

  private void UpdateAccordionItemsSelection(bool selectedValue)
  {
    foreach (object obj in (IEnumerable) this.Items)
    {
      if (this.ItemContainerGenerator.ContainerFromItem(obj) is AccordionItem accordionItem && !accordionItem.IsLocked)
        accordionItem.IsSelected = selectedValue;
    }
  }

  private void SetLockedProperties()
  {
    for (int index = 0; index < this.Items.Count; ++index)
    {
      if (this.ItemContainerGenerator.ContainerFromIndex(index) is AccordionItem accordionItem)
        accordionItem.IsLocked = accordionItem.IsSelected && this.IsMinimumOneSelected && this.SelectedIndices.Count == 1;
    }
  }

  protected virtual void OnSelectedItemChanged(SelectionChangedEventArgs e)
  {
    this.RaiseEvent((RoutedEventArgs) e);
  }

  private void RaiseOnSelectedItemsCollectionChanged(NotifyCollectionChangedEventArgs e)
  {
    NotifyCollectionChangedEventHandler selectedItemsChanged = this.SelectedItemsChanged;
    if (selectedItemsChanged == null)
      return;
    selectedItemsChanged((object) this, e);
  }

  private void OnAccordionSizeChanged(object sender, SizeChangedEventArgs e)
  {
    this.IsResizing = true;
    this.LayoutChildren();
    this.IsResizing = false;
  }

  internal void OnHeaderSizeChange(AccordionItem item) => this.LayoutChildren();

  internal virtual bool ScheduleAction(AccordionItem item, AccordionAction action)
  {
    if (this.SelectionSequence != SelectionSequence.CollapseBeforeExpand)
      return true;
    lock (this)
    {
      if (!this._scheduledActions.Contains(item))
        this._scheduledActions.Add(item);
    }
    if (this._currentActioningItem == null)
      this.Dispatcher.BeginInvoke((Delegate) new Action(this.StartNextAction));
    return false;
  }

  internal virtual void OnActionFinish(AccordionItem item)
  {
    if (this.SelectionSequence != SelectionSequence.CollapseBeforeExpand)
      return;
    lock (this)
    {
      this._currentActioningItem = this._currentActioningItem.Equals((object) item) ? (AccordionItem) null : throw new InvalidOperationException(System.Windows.Controls.Properties.Resources.Accordion_OnActionFinish_InvalidFinish);
      this.StartNextAction();
    }
  }

  private void StartNextAction()
  {
    if (this._currentActioningItem != null)
      return;
    AccordionItem accordionItem = (this._scheduledActions.FirstOrDefault<AccordionItem>((Func<AccordionItem, bool>) (item => item.ScheduledAction == AccordionAction.Collapse)) ?? this._scheduledActions.FirstOrDefault<AccordionItem>((Func<AccordionItem, bool>) (item => item.ScheduledAction == AccordionAction.Resize))) ?? this._scheduledActions.FirstOrDefault<AccordionItem>((Func<AccordionItem, bool>) (item => item.ScheduledAction == AccordionAction.Expand));
    if (accordionItem == null)
      return;
    this._currentActioningItem = accordionItem;
    this._scheduledActions.Remove(accordionItem);
    accordionItem.StartAction();
  }

  private void LayoutChildren()
  {
    ScrollViewer scrollHost = this.ItemsControlHelper.ScrollHost;
    Size size = new Size(double.NaN, double.NaN);
    if (scrollHost == null || this.ItemsControlHelper.ItemsHost == null)
      return;
    if (this.IsShouldFillWidth)
    {
      size.Width = Math.Max(0.0, scrollHost.ViewportWidth - this.ItemsControlHelper.ItemsHost.ActualWidth);
      foreach (object obj in (IEnumerable) this.Items)
      {
        if (this.ItemContainerGenerator.ContainerFromItem(obj) is AccordionItem accordionItem)
          size.Width += accordionItem.RelevantContentSize.Width;
      }
      double num = scrollHost.ViewportWidth - this.ItemsControlHelper.ItemsHost.ActualWidth;
      if (num < 0.0)
        size.Width = Math.Max(0.0, size.Width + num);
      if (size.Width > 1.0)
        --size.Width;
      if (scrollHost.ExtentWidth > scrollHost.ViewportWidth)
        size.Width = Math.Max(0.0, size.Width - (scrollHost.ExtentWidth - scrollHost.ViewportWidth));
      size.Width = this.SelectedItems.Count > 0 ? size.Width / (double) this.SelectedItems.Count : size.Width;
    }
    else if (this.IsShouldFillHeight)
    {
      size.Height = Math.Max(0.0, scrollHost.ViewportHeight - this.ItemsControlHelper.ItemsHost.ActualHeight);
      foreach (object obj in (IEnumerable) this.Items)
      {
        if (this.ItemContainerGenerator.ContainerFromItem(obj) is AccordionItem accordionItem)
          size.Height += accordionItem.RelevantContentSize.Height;
      }
      double num = scrollHost.ViewportHeight - this.ItemsControlHelper.ItemsHost.ActualHeight;
      if (num < 0.0)
        size.Height = Math.Max(0.0, size.Height + num);
      if (size.Height > 1.0)
        --size.Height;
      size.Height = this.SelectedItems.Count > 0 ? size.Height / (double) this.SelectedItems.Count : size.Height;
    }
    foreach (object obj in (IEnumerable) this.Items)
    {
      if (this.ItemContainerGenerator.ContainerFromItem(obj) is AccordionItem accordionItem && accordionItem.IsSelected)
        accordionItem.ContentTargetSize = size;
    }
  }

  private bool IsShouldFillWidth
  {
    get
    {
      if (this.ExpandDirection != ExpandDirection.Left && this.ExpandDirection != ExpandDirection.Right)
        return false;
      return !double.IsNaN(this.Width) || this.HorizontalAlignment == HorizontalAlignment.Stretch;
    }
  }

  private bool IsShouldFillHeight
  {
    get
    {
      if (this.ExpandDirection != ExpandDirection.Down && this.ExpandDirection != ExpandDirection.Up)
        return false;
      return !double.IsNaN(this.Height) || this.VerticalAlignment == VerticalAlignment.Stretch;
    }
  }

  private void SetPanelOrientation()
  {
    if (!(this.ItemsControlHelper.ItemsHost is StackPanel itemsHost))
      return;
    switch (this.ExpandDirection)
    {
      case ExpandDirection.Down:
      case ExpandDirection.Up:
        itemsHost.HorizontalAlignment = HorizontalAlignment.Stretch;
        itemsHost.VerticalAlignment = this.ExpandDirection == ExpandDirection.Down ? VerticalAlignment.Top : VerticalAlignment.Bottom;
        itemsHost.Orientation = Orientation.Vertical;
        break;
      case ExpandDirection.Left:
      case ExpandDirection.Right:
        itemsHost.VerticalAlignment = VerticalAlignment.Stretch;
        itemsHost.HorizontalAlignment = this.ExpandDirection == ExpandDirection.Left ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        itemsHost.Orientation = Orientation.Horizontal;
        break;
    }
  }

  protected override void OnGotFocus(RoutedEventArgs e)
  {
    if (!this.Interaction.AllowGotFocus(e))
      return;
    this.Interaction.OnGotFocusBase();
    base.OnGotFocus(e);
  }

  protected override void OnLostFocus(RoutedEventArgs e)
  {
    if (!this.Interaction.AllowLostFocus(e))
      return;
    this.Interaction.OnLostFocusBase();
    base.OnLostFocus(e);
  }

  protected override void OnMouseEnter(MouseEventArgs e)
  {
    if (!this.Interaction.AllowMouseEnter(e))
      return;
    this.Interaction.OnMouseEnterBase();
    base.OnMouseEnter(e);
  }

  protected override void OnMouseLeave(MouseEventArgs e)
  {
    if (!this.Interaction.AllowMouseLeave(e))
      return;
    this.Interaction.OnMouseLeaveBase();
    base.OnMouseLeave(e);
  }

  protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
  {
    if (!this.Interaction.AllowMouseLeftButtonDown(e))
      return;
    this.Interaction.OnMouseLeftButtonDownBase();
    base.OnMouseLeftButtonDown(e);
  }

  protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
  {
    if (!this.Interaction.AllowMouseLeftButtonUp(e))
      return;
    this.Interaction.OnMouseLeftButtonUpBase();
    base.OnMouseLeftButtonUp(e);
  }

  void IUpdateVisualState.UpdateVisualState(bool useTransitions)
  {
    this.UpdateVisualState(useTransitions);
  }

  internal new virtual void UpdateVisualState(bool useTransitions)
  {
    this.Interaction.UpdateVisualStateBase(useTransitions);
  }
}

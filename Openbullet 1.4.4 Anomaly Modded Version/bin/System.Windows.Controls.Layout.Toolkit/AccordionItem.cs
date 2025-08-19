// Decompiled with JetBrains decompiler
// Type: System.Windows.Controls.AccordionItem
// Assembly: System.Windows.Controls.Layout.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 2878816D-F7B3-441D-96A5-F68332B17866
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Windows.Controls.Layout.Toolkit.dll

using System.Globalization;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

#nullable disable
namespace System.Windows.Controls;

[TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
[TemplatePart(Name = "ExpandSite", Type = typeof (ExpandableContentControl))]
[TemplatePart(Name = "ExpanderButton", Type = typeof (AccordionButton))]
[StyleTypedProperty(Property = "AccordionButtonStyle", StyleTargetType = typeof (AccordionButton))]
[StyleTypedProperty(Property = "ExpandableContentControlStyle", StyleTargetType = typeof (ExpandableContentControl))]
[TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Pressed", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Focused", GroupName = "FocusStates")]
[TemplateVisualState(Name = "Unfocused", GroupName = "FocusStates")]
[TemplateVisualState(Name = "Expanded", GroupName = "ExpansionStates")]
[TemplateVisualState(Name = "Collapsed", GroupName = "ExpansionStates")]
[TemplateVisualState(Name = "Locked", GroupName = "LockedStates")]
[TemplateVisualState(Name = "Unlocked", GroupName = "LockedStates")]
[TemplateVisualState(Name = "ExpandDown", GroupName = "ExpandDirectionStates")]
[TemplateVisualState(Name = "ExpandUp", GroupName = "ExpandDirectionStates")]
[TemplateVisualState(Name = "ExpandLeft", GroupName = "ExpandDirectionStates")]
[TemplateVisualState(Name = "ExpandRight", GroupName = "ExpandDirectionStates")]
public class AccordionItem : HeaderedContentControl, IUpdateVisualState
{
  private const string ElementExpanderButtonName = "ExpanderButton";
  private const string ElementExpandSiteName = "ExpandSite";
  private AccordionButton _expanderButton;
  private ExpandableContentControl _expandSite;
  private bool _isBusyWithAction;
  private bool _allowedToWriteExpandDirection;
  public static readonly DependencyProperty ExpandDirectionProperty = DependencyProperty.Register(nameof (ExpandDirection), typeof (ExpandDirection), typeof (AccordionItem), new PropertyMetadata((object) ExpandDirection.Down, new PropertyChangedCallback(AccordionItem.OnExpandDirectionPropertyChanged)));
  public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof (IsSelected), typeof (bool), typeof (AccordionItem), new PropertyMetadata(new PropertyChangedCallback(AccordionItem.OnIsSelectedPropertyChanged)));
  private int _isSelectedNestedLevel;
  private bool _isLocked;
  public static readonly DependencyProperty AccordionButtonStyleProperty = DependencyProperty.Register(nameof (AccordionButtonStyle), typeof (Style), typeof (AccordionItem), new PropertyMetadata(new PropertyChangedCallback(AccordionItem.OnAccordionButtonStylePropertyChanged)));
  public static readonly DependencyProperty ExpandableContentControlStyleProperty = DependencyProperty.Register(nameof (ExpandableContentControlStyle), typeof (Style), typeof (AccordionItem), new PropertyMetadata(new PropertyChangedCallback(AccordionItem.OnExpandableContentControlStylePropertyChanged)));
  private bool _allowedToWriteContentTargetSize;
  public static readonly DependencyProperty ContentTargetSizeProperty = DependencyProperty.Register(nameof (ContentTargetSize), typeof (Size), typeof (AccordionItem), new PropertyMetadata((object) new Size(double.NaN, double.NaN), new PropertyChangedCallback(AccordionItem.OnContentTargetSizePropertyChanged)));
  public static RoutedEvent SelectedEvent = EventManager.RegisterRoutedEvent("Selected", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (AccordionItem));
  public static RoutedEvent UnselectedEvent = EventManager.RegisterRoutedEvent("Unselected", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (AccordionItem));
  private InteractionHelper _interaction;

  private AccordionButton ExpanderButton
  {
    get => this._expanderButton;
    set
    {
      if (this._expanderButton != null)
      {
        this._expanderButton.Click -= new RoutedEventHandler(this.OnExpanderButtonClicked);
        this._expanderButton.ParentAccordionItem = (AccordionItem) null;
        this._expanderButton.SizeChanged -= new SizeChangedEventHandler(this.OnHeaderSizeChanged);
      }
      this._expanderButton = value;
      if (this._expanderButton == null)
        return;
      this._expanderButton.IsChecked = new bool?(this.IsSelected);
      this._expanderButton.Click += new RoutedEventHandler(this.OnExpanderButtonClicked);
      this._expanderButton.ParentAccordionItem = this;
      this._expanderButton.SizeChanged += new SizeChangedEventHandler(this.OnHeaderSizeChanged);
    }
  }

  private ExpandableContentControl ExpandSite
  {
    get => this._expandSite;
    set
    {
      if (this._expandSite != null)
        this._expandSite.ContentSizeChanged -= new SizeChangedEventHandler(this.OnExpandSiteContentSizeChanged);
      this._expandSite = value;
      if (this._expandSite == null)
        return;
      this._expandSite.ContentSizeChanged += new SizeChangedEventHandler(this.OnExpandSiteContentSizeChanged);
    }
  }

  public ExpandDirection ExpandDirection
  {
    get => (ExpandDirection) this.GetValue(AccordionItem.ExpandDirectionProperty);
    protected internal set
    {
      this._allowedToWriteExpandDirection = true;
      this.SetValue(AccordionItem.ExpandDirectionProperty, (object) value);
      this._allowedToWriteExpandDirection = false;
    }
  }

  private static void OnExpandDirectionPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    AccordionItem accordionItem = (AccordionItem) d;
    ExpandDirection oldValue = (ExpandDirection) e.OldValue;
    ExpandDirection newValue = (ExpandDirection) e.NewValue;
    if (!accordionItem._allowedToWriteExpandDirection)
    {
      accordionItem.ExpandDirection = oldValue;
      throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, System.Windows.Controls.Properties.Resources.AccordionItem_InvalidWriteToExpandDirection, (object) newValue));
    }
    if (newValue != ExpandDirection.Down && newValue != ExpandDirection.Left && newValue != ExpandDirection.Right && newValue != ExpandDirection.Up)
    {
      accordionItem.ExpandDirection = oldValue;
      throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, System.Windows.Controls.Properties.Resources.Expander_OnExpandDirectionPropertyChanged_InvalidValue, (object) newValue), nameof (e));
    }
    if (accordionItem.ExpandSite != null)
      accordionItem.ExpandSite.RecalculatePercentage(accordionItem.IsSelected ? 1.0 : 0.0);
    accordionItem.UpdateVisualState(true);
  }

  public bool IsSelected
  {
    get => (bool) this.GetValue(AccordionItem.IsSelectedProperty);
    set => this.SetValue(AccordionItem.IsSelectedProperty, (object) value);
  }

  private static void OnIsSelectedPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    AccordionItem accordionItem = (AccordionItem) d;
    bool newValue = (bool) e.NewValue;
    if (accordionItem.IsLocked && accordionItem._isSelectedNestedLevel == 0)
    {
      ++accordionItem._isSelectedNestedLevel;
      accordionItem.SetValue(AccordionItem.IsSelectedProperty, e.OldValue);
      --accordionItem._isSelectedNestedLevel;
      throw new InvalidOperationException(System.Windows.Controls.Properties.Resources.AccordionItem_OnIsSelectedPropertyChanged_InvalidChange);
    }
    if (accordionItem._isSelectedNestedLevel != 0)
      return;
    Accordion parentAccordion = accordionItem.ParentAccordion;
    if (parentAccordion != null)
    {
      if (newValue)
        parentAccordion.OnAccordionItemSelected(accordionItem);
      else
        parentAccordion.OnAccordionItemUnselected(accordionItem);
    }
    if (newValue)
      accordionItem.OnSelected();
    else
      accordionItem.OnUnselected();
  }

  public bool IsLocked
  {
    get => this._isLocked;
    internal set
    {
      if (this._isLocked == value)
        return;
      this._isLocked = value;
      this.UpdateVisualState(true);
    }
  }

  public Style AccordionButtonStyle
  {
    get => this.GetValue(AccordionItem.AccordionButtonStyleProperty) as Style;
    set => this.SetValue(AccordionItem.AccordionButtonStyleProperty, (object) value);
  }

  private static void OnAccordionButtonStylePropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((AccordionItem) d).OnAccordionButtonStyleChanged(e.OldValue as Style, e.NewValue as Style);
  }

  protected virtual void OnAccordionButtonStyleChanged(Style oldStyle, Style newStyle)
  {
  }

  public Style ExpandableContentControlStyle
  {
    get => this.GetValue(AccordionItem.ExpandableContentControlStyleProperty) as Style;
    set => this.SetValue(AccordionItem.ExpandableContentControlStyleProperty, (object) value);
  }

  private static void OnExpandableContentControlStylePropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((AccordionItem) d).OnExpandableContentControlStyleChanged(e.OldValue as Style, e.NewValue as Style);
  }

  protected virtual void OnExpandableContentControlStyleChanged(Style oldStyle, Style newStyle)
  {
  }

  public Size ContentTargetSize
  {
    get => (Size) this.GetValue(AccordionItem.ContentTargetSizeProperty);
    internal set
    {
      this._allowedToWriteContentTargetSize = true;
      this.SetValue(AccordionItem.ContentTargetSizeProperty, (object) value);
      this._allowedToWriteContentTargetSize = false;
    }
  }

  private static void OnContentTargetSizePropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    AccordionItem accordionItem = (AccordionItem) d;
    Size newValue = (Size) e.NewValue;
    if (!accordionItem._allowedToWriteContentTargetSize)
    {
      accordionItem.ContentTargetSize = (Size) e.OldValue;
      throw new InvalidOperationException(System.Windows.Controls.Properties.Resources.AccordionItem_InvalidWriteToContentTargetSize);
    }
    ExpandableContentControl expandSite = accordionItem.ExpandSite;
    if (expandSite == null || expandSite.TargetSize.Equals(newValue))
      return;
    expandSite.TargetSize = newValue;
    if (!accordionItem.IsSelected)
      return;
    if (accordionItem.ParentAccordion != null && accordionItem.ParentAccordion.IsResizing)
      expandSite.RecalculatePercentage(1.0);
    else
      accordionItem.Schedule(AccordionAction.Resize);
  }

  internal Accordion ParentAccordion { get; set; }

  internal AccordionAction ScheduledAction { get; private set; }

  public event RoutedEventHandler Selected
  {
    add => this.AddHandler(AccordionItem.SelectedEvent, (Delegate) value);
    remove => this.RemoveHandler(AccordionItem.SelectedEvent, (Delegate) value);
  }

  public event RoutedEventHandler Unselected
  {
    add => this.AddHandler(AccordionItem.UnselectedEvent, (Delegate) value);
    remove => this.RemoveHandler(AccordionItem.UnselectedEvent, (Delegate) value);
  }

  static AccordionItem()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (AccordionItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (AccordionItem)));
  }

  public AccordionItem()
  {
    this.ScheduledAction = AccordionAction.None;
    this._interaction = new InteractionHelper((Control) this);
  }

  private void OnHeaderSizeChanged(object sender, SizeChangedEventArgs e)
  {
    if (this.ParentAccordion == null)
      return;
    this.ParentAccordion.OnHeaderSizeChange(this);
  }

  internal Size RelevantContentSize
  {
    get => this.ExpandSite != null ? this.ExpandSite.RelevantContentSize : new Size(0.0, 0.0);
  }

  private void Schedule(AccordionAction action)
  {
    this.ScheduledAction = action;
    if (this.ParentAccordion == null)
    {
      this.StartAction();
    }
    else
    {
      if (!this.ParentAccordion.ScheduleAction(this, action))
        return;
      this.StartAction();
    }
  }

  internal virtual void StartAction()
  {
    if (this.ScheduledAction == AccordionAction.None)
      throw new InvalidOperationException(System.Windows.Controls.Properties.Resources.AccordionItem_StartAction_InvalidCall);
    Action action;
    switch (this.ScheduledAction)
    {
      case AccordionAction.Collapse:
        action = (Action) (() =>
        {
          VisualStateManager.GoToState((Control) this, "Expanded", false);
          if (!VisualStateManager.GoToState((Control) this, "Collapsed", true))
            return;
          this.ParentAccordion.OnActionFinish(this);
        });
        break;
      case AccordionAction.Expand:
        action = (Action) (() =>
        {
          VisualStateManager.GoToState((Control) this, "Collapsed", false);
          if (!VisualStateManager.GoToState((Control) this, "Expanded", true))
            return;
          this.ParentAccordion.OnActionFinish(this);
        });
        break;
      case AccordionAction.Resize:
        action = (Action) (() =>
        {
          VisualStateManager.GoToState((Control) this, "Expanded", false);
          VisualStateManager.GoToState((Control) this, "Collapsed", false);
          if (!VisualStateManager.GoToState((Control) this, "Expanded", true))
            return;
          this.ParentAccordion.OnActionFinish(this);
        });
        break;
      default:
        throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, System.Windows.Controls.Properties.Resources.AccordionItem_StartAction_InvalidAction, (object) this.ScheduledAction));
    }
    this.ScheduledAction = AccordionAction.None;
    this._isBusyWithAction = true;
    action();
  }

  private void OnExpandSiteContentSizeChanged(object sender, SizeChangedEventArgs e)
  {
    if (!this.IsSelected || this._isBusyWithAction || (this.ShouldFillWidth || e.PreviousSize.Width == e.NewSize.Width) && (this.ShouldFillHeight || e.PreviousSize.Height == e.NewSize.Height))
      return;
    this.ExpandSite.MeasureContent(this.ExpandSite.CalculateDesiredContentSize());
    this.ExpandSite.RecalculatePercentage(this.ExpandSite.TargetSize);
    this.Schedule(AccordionAction.Resize);
  }

  private bool ShouldFillWidth
  {
    get
    {
      if (this.ExpandDirection != ExpandDirection.Left && this.ExpandDirection != ExpandDirection.Right)
        return false;
      return !double.IsNaN(this.ContentTargetSize.Width) || this.HorizontalAlignment == HorizontalAlignment.Stretch;
    }
  }

  private bool ShouldFillHeight
  {
    get
    {
      if (this.ExpandDirection != ExpandDirection.Down && this.ExpandDirection != ExpandDirection.Up)
        return false;
      return !double.IsNaN(this.ContentTargetSize.Height) || this.VerticalAlignment == VerticalAlignment.Stretch;
    }
  }

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    this.ExpanderButton = this.GetTemplateChild("ExpanderButton") as AccordionButton;
    this.ExpandSite = this.GetTemplateChild("ExpandSite") as ExpandableContentControl;
    if (VisualTreeHelper.GetChildrenCount((DependencyObject) this) > 0)
      VisualTreeHelper.GetChild((DependencyObject) this, 0);
    this._interaction.OnApplyTemplateBase();
    this.UpdateVisualState(false);
    if (this.IsSelected)
      this.Schedule(AccordionAction.Expand);
    else
      this.Schedule(AccordionAction.Collapse);
  }

  protected override AutomationPeer OnCreateAutomationPeer()
  {
    return (AutomationPeer) new AccordionItemWrapperAutomationPeer(this);
  }

  internal static void PreparePrepareHeaderedContentControlContainerForItemOverride(
    HeaderedContentControl element,
    object item,
    ItemsControl parent,
    Style parentItemContainerStyle)
  {
    if (element == item || item is UIElement || !AccordionItem.HasDefaultValue((Control) element, HeaderedContentControl.HeaderProperty))
      return;
    element.Header = item;
  }

  private static bool HasDefaultValue(Control control, DependencyProperty property)
  {
    return control.ReadLocalValue(property) == DependencyProperty.UnsetValue;
  }

  protected override void OnKeyDown(KeyEventArgs e)
  {
    base.OnKeyDown(e);
    if (e.Handled || !this.IsEnabled || this.IsLocked)
      return;
    bool isSelected = this.IsSelected;
    switch (this.ExpandDirection)
    {
      case ExpandDirection.Down:
        if ((!isSelected || e.Key != Key.Up) && (isSelected || e.Key != Key.Down))
          break;
        this.IsSelected = !isSelected;
        break;
      case ExpandDirection.Up:
        if ((!isSelected || e.Key != Key.Down) && (isSelected || e.Key != Key.Up))
          break;
        this.IsSelected = !isSelected;
        break;
      case ExpandDirection.Left:
        if ((!isSelected || e.Key != Key.Right) && (isSelected || e.Key != Key.Left))
          break;
        this.IsSelected = !isSelected;
        break;
      case ExpandDirection.Right:
        if ((!isSelected || e.Key != Key.Left) && (isSelected || e.Key != Key.Right))
          break;
        this.IsSelected = !isSelected;
        break;
    }
  }

  protected virtual void OnSelected()
  {
    this.ToggleSelected(new RoutedEventArgs(AccordionItem.SelectedEvent));
  }

  protected virtual void OnUnselected()
  {
    this.ToggleSelected(new RoutedEventArgs(AccordionItem.UnselectedEvent));
  }

  private void ToggleSelected(RoutedEventArgs args)
  {
    ToggleButton expanderButton = (ToggleButton) this.ExpanderButton;
    if (expanderButton != null)
      expanderButton.IsChecked = new bool?(this.IsSelected);
    if (this.IsSelected)
      this.Schedule(AccordionAction.Expand);
    else
      this.Schedule(AccordionAction.Collapse);
    this.UpdateVisualState(true);
    this.RaiseEvent(args);
  }

  private void OnExpanderButtonClicked(object sender, RoutedEventArgs e)
  {
    if (this.IsLocked)
      return;
    this.IsSelected = !this.IsSelected;
  }

  protected override void OnGotFocus(RoutedEventArgs e)
  {
    if (!this._interaction.AllowGotFocus(e))
      return;
    this._interaction.OnGotFocusBase();
    base.OnGotFocus(e);
  }

  protected override void OnLostFocus(RoutedEventArgs e)
  {
    if (!this._interaction.AllowLostFocus(e))
      return;
    this._interaction.OnLostFocusBase();
    base.OnLostFocus(e);
  }

  protected override void OnMouseEnter(MouseEventArgs e)
  {
    if (!this._interaction.AllowMouseEnter(e))
      return;
    this._interaction.OnMouseEnterBase();
    base.OnMouseEnter(e);
  }

  protected override void OnMouseLeave(MouseEventArgs e)
  {
    if (!this._interaction.AllowMouseLeave(e))
      return;
    this._interaction.OnMouseLeaveBase();
    base.OnMouseLeave(e);
  }

  protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
  {
    if (!this._interaction.AllowMouseLeftButtonDown(e))
      return;
    this._interaction.OnMouseLeftButtonDownBase();
    base.OnMouseLeftButtonDown(e);
  }

  protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
  {
    if (!this._interaction.AllowMouseLeftButtonUp(e))
      return;
    this._interaction.OnMouseLeftButtonUpBase();
    base.OnMouseLeftButtonUp(e);
  }

  void IUpdateVisualState.UpdateVisualState(bool useTransitions)
  {
    this.UpdateVisualState(useTransitions);
  }

  internal new virtual void UpdateVisualState(bool useTransitions)
  {
    if (this.IsLocked)
      VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "Locked");
    else
      VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "Unlocked");
    switch (this.ExpandDirection)
    {
      case ExpandDirection.Down:
        VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "ExpandDown");
        break;
      case ExpandDirection.Up:
        VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "ExpandUp");
        break;
      case ExpandDirection.Left:
        VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "ExpandLeft");
        break;
      default:
        VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "ExpandRight");
        break;
    }
    if (this.ExpanderButton != null)
      this.ExpanderButton.UpdateVisualState(useTransitions);
    this._interaction.UpdateVisualStateBase(useTransitions);
  }
}

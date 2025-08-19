// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Layout.LayoutContent
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Layout;

[ContentProperty("Content")]
[Serializable]
public abstract class LayoutContent : 
  LayoutElement,
  IXmlSerializable,
  ILayoutElementForFloatingWindow,
  IComparable<LayoutContent>,
  ILayoutPreviousContainer
{
  public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof (Title), typeof (string), typeof (LayoutContent), (PropertyMetadata) new UIPropertyMetadata((object) null, new PropertyChangedCallback(LayoutContent.OnTitlePropertyChanged), new CoerceValueCallback(LayoutContent.CoerceTitleValue)));
  [NonSerialized]
  private object _content;
  private string _contentId;
  private bool _isSelected;
  [NonSerialized]
  private bool _isActive;
  private bool _isLastFocusedDocument;
  [NonSerialized]
  private ILayoutContainer _previousContainer;
  [NonSerialized]
  private int _previousContainerIndex = -1;
  private DateTime? _lastActivationTimeStamp;
  private double _floatingWidth;
  private double _floatingHeight;
  private double _floatingLeft;
  private double _floatingTop;
  private bool _isMaximized;
  private object _toolTip;
  private ImageSource _iconSource;
  internal bool _canClose = true;
  private bool _canFloat = true;
  private bool _isEnabled = true;

  internal LayoutContent()
  {
  }

  public string Title
  {
    get => (string) this.GetValue(LayoutContent.TitleProperty);
    set => this.SetValue(LayoutContent.TitleProperty, (object) value);
  }

  private static object CoerceTitleValue(DependencyObject obj, object value)
  {
    LayoutContent layoutContent = (LayoutContent) obj;
    if ((string) value != layoutContent.Title)
      layoutContent.RaisePropertyChanging(LayoutContent.TitleProperty.Name);
    return value;
  }

  private static void OnTitlePropertyChanged(
    DependencyObject obj,
    DependencyPropertyChangedEventArgs args)
  {
    ((LayoutElement) obj).RaisePropertyChanged(LayoutContent.TitleProperty.Name);
  }

  [XmlIgnore]
  public object Content
  {
    get => this._content;
    set
    {
      if (this._content == value)
        return;
      this.RaisePropertyChanging(nameof (Content));
      this._content = value;
      this.RaisePropertyChanged(nameof (Content));
    }
  }

  public string ContentId
  {
    get
    {
      return this._contentId == null && this._content is FrameworkElement content && !string.IsNullOrWhiteSpace(content.Name) ? content.Name : this._contentId;
    }
    set
    {
      if (!(this._contentId != value))
        return;
      this._contentId = value;
      this.RaisePropertyChanged(nameof (ContentId));
    }
  }

  public bool IsSelected
  {
    get => this._isSelected;
    set
    {
      if (this._isSelected == value)
        return;
      bool isSelected = this._isSelected;
      this.RaisePropertyChanging(nameof (IsSelected));
      this._isSelected = value;
      if (this.Parent is ILayoutContentSelector parent)
        parent.SelectedContentIndex = this._isSelected ? parent.IndexOf(this) : -1;
      this.OnIsSelectedChanged(isSelected, value);
      this.RaisePropertyChanged(nameof (IsSelected));
    }
  }

  protected virtual void OnIsSelectedChanged(bool oldValue, bool newValue)
  {
    if (this.IsSelectedChanged == null)
      return;
    this.IsSelectedChanged((object) this, EventArgs.Empty);
  }

  public event EventHandler IsSelectedChanged;

  [XmlIgnore]
  public bool IsActive
  {
    get => this._isActive;
    set
    {
      if (this._isActive == value)
        return;
      this.RaisePropertyChanging(nameof (IsActive));
      bool isActive = this._isActive;
      this._isActive = value;
      ILayoutRoot root = this.Root;
      if (root != null && this._isActive)
        root.ActiveContent = this;
      if (this._isActive)
        this.IsSelected = true;
      this.OnIsActiveChanged(isActive, value);
      this.RaisePropertyChanged(nameof (IsActive));
    }
  }

  protected virtual void OnIsActiveChanged(bool oldValue, bool newValue)
  {
    if (newValue)
      this.LastActivationTimeStamp = new DateTime?(DateTime.Now);
    if (this.IsActiveChanged == null)
      return;
    this.IsActiveChanged((object) this, EventArgs.Empty);
  }

  public event EventHandler IsActiveChanged;

  public bool IsLastFocusedDocument
  {
    get => this._isLastFocusedDocument;
    internal set
    {
      if (this._isLastFocusedDocument == value)
        return;
      this.RaisePropertyChanging(nameof (IsLastFocusedDocument));
      this._isLastFocusedDocument = value;
      this.RaisePropertyChanged(nameof (IsLastFocusedDocument));
    }
  }

  [XmlIgnore]
  ILayoutContainer ILayoutPreviousContainer.PreviousContainer
  {
    get => this._previousContainer;
    set
    {
      if (this._previousContainer == value)
        return;
      this._previousContainer = value;
      this.RaisePropertyChanged("PreviousContainer");
      if (!(this._previousContainer is ILayoutPaneSerializable previousContainer) || previousContainer.Id != null)
        return;
      previousContainer.Id = Guid.NewGuid().ToString();
    }
  }

  protected ILayoutContainer PreviousContainer
  {
    get => ((ILayoutPreviousContainer) this).PreviousContainer;
    set => ((ILayoutPreviousContainer) this).PreviousContainer = value;
  }

  [XmlIgnore]
  string ILayoutPreviousContainer.PreviousContainerId { get; set; }

  protected string PreviousContainerId
  {
    get => ((ILayoutPreviousContainer) this).PreviousContainerId;
    set => ((ILayoutPreviousContainer) this).PreviousContainerId = value;
  }

  [XmlIgnore]
  public int PreviousContainerIndex
  {
    get => this._previousContainerIndex;
    set
    {
      if (this._previousContainerIndex == value)
        return;
      this._previousContainerIndex = value;
      this.RaisePropertyChanged(nameof (PreviousContainerIndex));
    }
  }

  public DateTime? LastActivationTimeStamp
  {
    get => this._lastActivationTimeStamp;
    set
    {
      DateTime? activationTimeStamp = this._lastActivationTimeStamp;
      DateTime? nullable = value;
      if ((activationTimeStamp.HasValue == nullable.HasValue ? (activationTimeStamp.HasValue ? (activationTimeStamp.GetValueOrDefault() != nullable.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0)
        return;
      this._lastActivationTimeStamp = value;
      this.RaisePropertyChanged(nameof (LastActivationTimeStamp));
    }
  }

  public double FloatingWidth
  {
    get => this._floatingWidth;
    set
    {
      if (this._floatingWidth == value)
        return;
      this.RaisePropertyChanging(nameof (FloatingWidth));
      this._floatingWidth = value;
      this.RaisePropertyChanged(nameof (FloatingWidth));
    }
  }

  public double FloatingHeight
  {
    get => this._floatingHeight;
    set
    {
      if (this._floatingHeight == value)
        return;
      this.RaisePropertyChanging(nameof (FloatingHeight));
      this._floatingHeight = value;
      this.RaisePropertyChanged(nameof (FloatingHeight));
    }
  }

  public double FloatingLeft
  {
    get => this._floatingLeft;
    set
    {
      if (this._floatingLeft == value)
        return;
      this.RaisePropertyChanging(nameof (FloatingLeft));
      this._floatingLeft = value;
      this.RaisePropertyChanged(nameof (FloatingLeft));
    }
  }

  public double FloatingTop
  {
    get => this._floatingTop;
    set
    {
      if (this._floatingTop == value)
        return;
      this.RaisePropertyChanging(nameof (FloatingTop));
      this._floatingTop = value;
      this.RaisePropertyChanged(nameof (FloatingTop));
    }
  }

  public bool IsMaximized
  {
    get => this._isMaximized;
    set
    {
      if (this._isMaximized == value)
        return;
      this.RaisePropertyChanging(nameof (IsMaximized));
      this._isMaximized = value;
      this.RaisePropertyChanged(nameof (IsMaximized));
    }
  }

  public object ToolTip
  {
    get => this._toolTip;
    set
    {
      if (this._toolTip == value)
        return;
      this._toolTip = value;
      this.RaisePropertyChanged(nameof (ToolTip));
    }
  }

  public bool IsFloating => this.FindParent<LayoutFloatingWindow>() != null;

  public ImageSource IconSource
  {
    get => this._iconSource;
    set
    {
      if (this._iconSource == value)
        return;
      this._iconSource = value;
      this.RaisePropertyChanged(nameof (IconSource));
    }
  }

  public bool CanClose
  {
    get => this._canClose;
    set
    {
      if (this._canClose == value)
        return;
      this._canClose = value;
      this.RaisePropertyChanged(nameof (CanClose));
    }
  }

  public bool CanFloat
  {
    get => this._canFloat;
    set
    {
      if (this._canFloat == value)
        return;
      this._canFloat = value;
      this.RaisePropertyChanged(nameof (CanFloat));
    }
  }

  public bool IsEnabled
  {
    get => this._isEnabled;
    set
    {
      if (this._isEnabled == value)
        return;
      this._isEnabled = value;
      this.RaisePropertyChanged(nameof (IsEnabled));
    }
  }

  protected override void OnParentChanging(ILayoutContainer oldValue, ILayoutContainer newValue)
  {
    ILayoutRoot root = this.Root;
    if (oldValue != null)
      this.IsSelected = false;
    base.OnParentChanging(oldValue, newValue);
  }

  protected override void OnParentChanged(ILayoutContainer oldValue, ILayoutContainer newValue)
  {
    if (this.IsSelected && this.Parent != null && this.Parent is ILayoutContentSelector)
    {
      ILayoutContentSelector parent = this.Parent as ILayoutContentSelector;
      parent.SelectedContentIndex = parent.IndexOf(this);
    }
    base.OnParentChanged(oldValue, newValue);
  }

  public abstract void Close();

  public XmlSchema GetSchema() => (XmlSchema) null;

  public virtual void ReadXml(XmlReader reader)
  {
    if (reader.MoveToAttribute("Title"))
      this.Title = reader.Value;
    if (reader.MoveToAttribute("IsSelected"))
      this.IsSelected = bool.Parse(reader.Value);
    if (reader.MoveToAttribute("ContentId"))
      this.ContentId = reader.Value;
    if (reader.MoveToAttribute("IsLastFocusedDocument"))
      this.IsLastFocusedDocument = bool.Parse(reader.Value);
    if (reader.MoveToAttribute("PreviousContainerId"))
      this.PreviousContainerId = reader.Value;
    if (reader.MoveToAttribute("PreviousContainerIndex"))
      this.PreviousContainerIndex = int.Parse(reader.Value);
    if (reader.MoveToAttribute("FloatingLeft"))
      this.FloatingLeft = double.Parse(reader.Value, (IFormatProvider) CultureInfo.InvariantCulture);
    if (reader.MoveToAttribute("FloatingTop"))
      this.FloatingTop = double.Parse(reader.Value, (IFormatProvider) CultureInfo.InvariantCulture);
    if (reader.MoveToAttribute("FloatingWidth"))
      this.FloatingWidth = double.Parse(reader.Value, (IFormatProvider) CultureInfo.InvariantCulture);
    if (reader.MoveToAttribute("FloatingHeight"))
      this.FloatingHeight = double.Parse(reader.Value, (IFormatProvider) CultureInfo.InvariantCulture);
    if (reader.MoveToAttribute("IsMaximized"))
      this.IsMaximized = bool.Parse(reader.Value);
    if (reader.MoveToAttribute("CanClose"))
      this.CanClose = bool.Parse(reader.Value);
    if (reader.MoveToAttribute("CanFloat"))
      this.CanFloat = bool.Parse(reader.Value);
    if (reader.MoveToAttribute("LastActivationTimeStamp"))
      this.LastActivationTimeStamp = new DateTime?(DateTime.Parse(reader.Value, (IFormatProvider) CultureInfo.InvariantCulture));
    reader.Read();
  }

  public virtual void WriteXml(XmlWriter writer)
  {
    if (!string.IsNullOrWhiteSpace(this.Title))
      writer.WriteAttributeString("Title", this.Title);
    bool flag;
    if (this.IsSelected)
    {
      XmlWriter xmlWriter = writer;
      flag = this.IsSelected;
      string str = flag.ToString();
      xmlWriter.WriteAttributeString("IsSelected", str);
    }
    if (this.IsLastFocusedDocument)
    {
      XmlWriter xmlWriter = writer;
      flag = this.IsLastFocusedDocument;
      string str = flag.ToString();
      xmlWriter.WriteAttributeString("IsLastFocusedDocument", str);
    }
    if (!string.IsNullOrWhiteSpace(this.ContentId))
      writer.WriteAttributeString("ContentId", this.ContentId);
    if (this.ToolTip != null && this.ToolTip is string && !string.IsNullOrWhiteSpace((string) this.ToolTip))
      writer.WriteAttributeString("ToolTip", (string) this.ToolTip);
    if (this.FloatingLeft != 0.0)
      writer.WriteAttributeString("FloatingLeft", this.FloatingLeft.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    if (this.FloatingTop != 0.0)
      writer.WriteAttributeString("FloatingTop", this.FloatingTop.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    if (this.FloatingWidth != 0.0)
      writer.WriteAttributeString("FloatingWidth", this.FloatingWidth.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    if (this.FloatingHeight != 0.0)
      writer.WriteAttributeString("FloatingHeight", this.FloatingHeight.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    if (this.IsMaximized)
    {
      XmlWriter xmlWriter = writer;
      flag = this.IsMaximized;
      string str = flag.ToString();
      xmlWriter.WriteAttributeString("IsMaximized", str);
    }
    if (!this.CanClose)
    {
      XmlWriter xmlWriter = writer;
      flag = this.CanClose;
      string str = flag.ToString();
      xmlWriter.WriteAttributeString("CanClose", str);
    }
    if (!this.CanFloat)
    {
      XmlWriter xmlWriter = writer;
      flag = this.CanFloat;
      string str = flag.ToString();
      xmlWriter.WriteAttributeString("CanFloat", str);
    }
    DateTime? activationTimeStamp = this.LastActivationTimeStamp;
    if (activationTimeStamp.HasValue)
    {
      XmlWriter xmlWriter = writer;
      activationTimeStamp = this.LastActivationTimeStamp;
      string str = activationTimeStamp.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      xmlWriter.WriteAttributeString("LastActivationTimeStamp", str);
    }
    if (this._previousContainer == null || !(this._previousContainer is ILayoutPaneSerializable previousContainer))
      return;
    writer.WriteAttributeString("PreviousContainerId", previousContainer.Id);
    writer.WriteAttributeString("PreviousContainerIndex", this._previousContainerIndex.ToString());
  }

  public int CompareTo(LayoutContent other)
  {
    return this.Content is IComparable content ? content.CompareTo(other.Content) : string.Compare(this.Title, other.Title);
  }

  public void Float()
  {
    if (this.PreviousContainer != null && this.PreviousContainer.FindParent<LayoutFloatingWindow>() != null)
    {
      ILayoutPane parent = this.Parent as ILayoutPane;
      int num = (parent as ILayoutGroup).IndexOfChild((ILayoutElement) this);
      ILayoutGroup previousContainer = this.PreviousContainer as ILayoutGroup;
      if (this.PreviousContainerIndex < previousContainer.ChildrenCount)
        previousContainer.InsertChildAt(this.PreviousContainerIndex, (ILayoutElement) this);
      else
        previousContainer.InsertChildAt(previousContainer.ChildrenCount, (ILayoutElement) this);
      this.PreviousContainer = (ILayoutContainer) parent;
      this.PreviousContainerIndex = num;
      this.IsSelected = true;
      this.IsActive = true;
      this.Root.CollectGarbage();
    }
    else
    {
      this.Root.Manager.StartDraggingFloatingWindowForContent(this, false);
      this.IsSelected = true;
      this.IsActive = true;
    }
  }

  public void DockAsDocument()
  {
    if (!(this.Root is LayoutRoot root))
      throw new InvalidOperationException();
    if (this.Parent is LayoutDocumentPane)
      return;
    if (this.PreviousContainer is LayoutDocumentPane)
    {
      this.Dock();
    }
    else
    {
      LayoutDocumentPane layoutDocumentPane = root.LastFocusedDocument == null ? root.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault<LayoutDocumentPane>() : root.LastFocusedDocument.Parent as LayoutDocumentPane;
      if (layoutDocumentPane != null)
      {
        layoutDocumentPane.Children.Add(this);
        root.CollectGarbage();
      }
      this.IsSelected = true;
      this.IsActive = true;
    }
  }

  public void Dock()
  {
    if (this.PreviousContainer != null)
    {
      ILayoutContainer parent = this.Parent;
      int num = parent is ILayoutGroup ? (parent as ILayoutGroup).IndexOfChild((ILayoutElement) this) : -1;
      ILayoutGroup previousContainer = this.PreviousContainer as ILayoutGroup;
      if (this.PreviousContainerIndex < previousContainer.ChildrenCount)
        previousContainer.InsertChildAt(this.PreviousContainerIndex, (ILayoutElement) this);
      else
        previousContainer.InsertChildAt(previousContainer.ChildrenCount, (ILayoutElement) this);
      if (num > -1)
      {
        this.PreviousContainer = parent;
        this.PreviousContainerIndex = num;
      }
      else
      {
        this.PreviousContainer = (ILayoutContainer) null;
        this.PreviousContainerIndex = 0;
      }
      this.IsSelected = true;
      this.IsActive = true;
    }
    else
      this.InternalDock();
    this.Root.CollectGarbage();
  }

  internal bool TestCanClose()
  {
    CancelEventArgs args = new CancelEventArgs();
    this.OnClosing(args);
    return !args.Cancel;
  }

  internal void CloseInternal()
  {
    ILayoutRoot root = this.Root;
    this.Parent.RemoveChild((ILayoutElement) this);
    root?.CollectGarbage();
    this.OnClosed();
  }

  protected virtual void OnClosed()
  {
    if (this.Closed == null)
      return;
    this.Closed((object) this, EventArgs.Empty);
  }

  protected virtual void OnClosing(CancelEventArgs args)
  {
    if (this.Closing == null)
      return;
    this.Closing((object) this, args);
  }

  protected virtual void InternalDock()
  {
  }

  public event EventHandler Closed;

  public event EventHandler<CancelEventArgs> Closing;
}

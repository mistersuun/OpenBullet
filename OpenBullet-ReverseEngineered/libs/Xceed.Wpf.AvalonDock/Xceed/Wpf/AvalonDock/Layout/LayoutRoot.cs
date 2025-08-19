// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Layout.LayoutRoot
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Layout;

[ContentProperty("RootPanel")]
[Serializable]
public class LayoutRoot : 
  LayoutElement,
  ILayoutContainer,
  ILayoutElement,
  INotifyPropertyChanged,
  INotifyPropertyChanging,
  ILayoutRoot,
  IXmlSerializable
{
  private LayoutPanel _rootPanel;
  private LayoutAnchorSide _topSide;
  private LayoutAnchorSide _rightSide;
  private LayoutAnchorSide _leftSide;
  private LayoutAnchorSide _bottomSide;
  private ObservableCollection<LayoutFloatingWindow> _floatingWindows;
  private ObservableCollection<LayoutAnchorable> _hiddenAnchorables;
  [NonSerialized]
  private WeakReference _activeContent;
  private bool _activeContentSet;
  [NonSerialized]
  private WeakReference _lastFocusedDocument;
  [NonSerialized]
  private bool _lastFocusedDocumentSet;
  [NonSerialized]
  private DockingManager _manager;

  public LayoutRoot()
  {
    this.RightSide = new LayoutAnchorSide();
    this.LeftSide = new LayoutAnchorSide();
    this.TopSide = new LayoutAnchorSide();
    this.BottomSide = new LayoutAnchorSide();
    this.RootPanel = new LayoutPanel((ILayoutPanelElement) new LayoutDocumentPane());
  }

  public LayoutPanel RootPanel
  {
    get => this._rootPanel;
    set
    {
      if (this._rootPanel == value)
        return;
      this.RaisePropertyChanging(nameof (RootPanel));
      if (this._rootPanel != null && this._rootPanel.Parent == this)
        this._rootPanel.Parent = (ILayoutContainer) null;
      this._rootPanel = value;
      if (this._rootPanel == null)
        this._rootPanel = new LayoutPanel((ILayoutPanelElement) new LayoutDocumentPane());
      if (this._rootPanel != null)
        this._rootPanel.Parent = (ILayoutContainer) this;
      this.RaisePropertyChanged(nameof (RootPanel));
    }
  }

  public LayoutAnchorSide TopSide
  {
    get => this._topSide;
    set
    {
      if (this._topSide == value)
        return;
      this.RaisePropertyChanging(nameof (TopSide));
      this._topSide = value;
      if (this._topSide != null)
        this._topSide.Parent = (ILayoutContainer) this;
      this.RaisePropertyChanged(nameof (TopSide));
    }
  }

  public LayoutAnchorSide RightSide
  {
    get => this._rightSide;
    set
    {
      if (this._rightSide == value)
        return;
      this.RaisePropertyChanging(nameof (RightSide));
      this._rightSide = value;
      if (this._rightSide != null)
        this._rightSide.Parent = (ILayoutContainer) this;
      this.RaisePropertyChanged(nameof (RightSide));
    }
  }

  public LayoutAnchorSide LeftSide
  {
    get => this._leftSide;
    set
    {
      if (this._leftSide == value)
        return;
      this.RaisePropertyChanging(nameof (LeftSide));
      this._leftSide = value;
      if (this._leftSide != null)
        this._leftSide.Parent = (ILayoutContainer) this;
      this.RaisePropertyChanged(nameof (LeftSide));
    }
  }

  public LayoutAnchorSide BottomSide
  {
    get => this._bottomSide;
    set
    {
      if (this._bottomSide == value)
        return;
      this.RaisePropertyChanging(nameof (BottomSide));
      this._bottomSide = value;
      if (this._bottomSide != null)
        this._bottomSide.Parent = (ILayoutContainer) this;
      this.RaisePropertyChanged(nameof (BottomSide));
    }
  }

  public ObservableCollection<LayoutFloatingWindow> FloatingWindows
  {
    get
    {
      if (this._floatingWindows == null)
      {
        this._floatingWindows = new ObservableCollection<LayoutFloatingWindow>();
        this._floatingWindows.CollectionChanged += new NotifyCollectionChangedEventHandler(this._floatingWindows_CollectionChanged);
      }
      return this._floatingWindows;
    }
  }

  public ObservableCollection<LayoutAnchorable> Hidden
  {
    get
    {
      if (this._hiddenAnchorables == null)
      {
        this._hiddenAnchorables = new ObservableCollection<LayoutAnchorable>();
        this._hiddenAnchorables.CollectionChanged += new NotifyCollectionChangedEventHandler(this._hiddenAnchorables_CollectionChanged);
      }
      return this._hiddenAnchorables;
    }
  }

  public IEnumerable<ILayoutElement> Children
  {
    get
    {
      if (this.RootPanel != null)
        yield return (ILayoutElement) this.RootPanel;
      if (this._floatingWindows != null)
      {
        foreach (ILayoutElement floatingWindow in (Collection<LayoutFloatingWindow>) this._floatingWindows)
          yield return floatingWindow;
      }
      if (this.TopSide != null)
        yield return (ILayoutElement) this.TopSide;
      if (this.RightSide != null)
        yield return (ILayoutElement) this.RightSide;
      if (this.BottomSide != null)
        yield return (ILayoutElement) this.BottomSide;
      if (this.LeftSide != null)
        yield return (ILayoutElement) this.LeftSide;
      if (this._hiddenAnchorables != null)
      {
        foreach (ILayoutElement hiddenAnchorable in (Collection<LayoutAnchorable>) this._hiddenAnchorables)
          yield return hiddenAnchorable;
      }
    }
  }

  public int ChildrenCount
  {
    get
    {
      return 5 + (this._floatingWindows != null ? this._floatingWindows.Count : 0) + (this._hiddenAnchorables != null ? this._hiddenAnchorables.Count : 0);
    }
  }

  [XmlIgnore]
  public LayoutContent ActiveContent
  {
    get => this._activeContent.GetValueOrDefault<LayoutContent>();
    set
    {
      LayoutContent activeContent = this.ActiveContent;
      if (activeContent == value)
        return;
      this.InternalSetActiveContent(activeContent, value);
    }
  }

  [XmlIgnore]
  public LayoutContent LastFocusedDocument
  {
    get => this._lastFocusedDocument.GetValueOrDefault<LayoutContent>();
    private set
    {
      LayoutContent lastFocusedDocument1 = this.LastFocusedDocument;
      if (lastFocusedDocument1 == value)
        return;
      this.RaisePropertyChanging(nameof (LastFocusedDocument));
      if (lastFocusedDocument1 != null)
        lastFocusedDocument1.IsLastFocusedDocument = false;
      this._lastFocusedDocument = new WeakReference((object) value);
      LayoutContent lastFocusedDocument2 = this.LastFocusedDocument;
      if (lastFocusedDocument2 != null)
        lastFocusedDocument2.IsLastFocusedDocument = true;
      this._lastFocusedDocumentSet = lastFocusedDocument2 != null;
      this.RaisePropertyChanged(nameof (LastFocusedDocument));
    }
  }

  [XmlIgnore]
  public DockingManager Manager
  {
    get => this._manager;
    internal set
    {
      if (this._manager == value)
        return;
      this.RaisePropertyChanging(nameof (Manager));
      this._manager = value;
      this.RaisePropertyChanged(nameof (Manager));
    }
  }

  public override void ConsoleDump(int tab)
  {
    Trace.Write(new string(' ', tab * 4));
    Trace.WriteLine("RootPanel()");
    this.RootPanel.ConsoleDump(tab + 1);
    Trace.Write(new string(' ', tab * 4));
    Trace.WriteLine("FloatingWindows()");
    foreach (LayoutElement floatingWindow in (Collection<LayoutFloatingWindow>) this.FloatingWindows)
      floatingWindow.ConsoleDump(tab + 1);
    Trace.Write(new string(' ', tab * 4));
    Trace.WriteLine("Hidden()");
    foreach (LayoutElement layoutElement in (Collection<LayoutAnchorable>) this.Hidden)
      layoutElement.ConsoleDump(tab + 1);
  }

  public void RemoveChild(ILayoutElement element)
  {
    if (element == this.RootPanel)
      this.RootPanel = (LayoutPanel) null;
    else if (this._floatingWindows != null && Xceed.Wpf.AvalonDock.Extensions.Contains(this._floatingWindows, (object) element))
      this._floatingWindows.Remove(element as LayoutFloatingWindow);
    else if (this._hiddenAnchorables != null && Xceed.Wpf.AvalonDock.Extensions.Contains(this._hiddenAnchorables, (object) element))
      this._hiddenAnchorables.Remove(element as LayoutAnchorable);
    else if (element == this.TopSide)
      this.TopSide = (LayoutAnchorSide) null;
    else if (element == this.RightSide)
      this.RightSide = (LayoutAnchorSide) null;
    else if (element == this.BottomSide)
    {
      this.BottomSide = (LayoutAnchorSide) null;
    }
    else
    {
      if (element != this.LeftSide)
        return;
      this.LeftSide = (LayoutAnchorSide) null;
    }
  }

  public void ReplaceChild(ILayoutElement oldElement, ILayoutElement newElement)
  {
    if (oldElement == this.RootPanel)
      this.RootPanel = (LayoutPanel) newElement;
    else if (this._floatingWindows != null && Xceed.Wpf.AvalonDock.Extensions.Contains(this._floatingWindows, (object) oldElement))
    {
      int index = this._floatingWindows.IndexOf(oldElement as LayoutFloatingWindow);
      this._floatingWindows.Remove(oldElement as LayoutFloatingWindow);
      this._floatingWindows.Insert(index, newElement as LayoutFloatingWindow);
    }
    else if (this._hiddenAnchorables != null && Xceed.Wpf.AvalonDock.Extensions.Contains(this._hiddenAnchorables, (object) oldElement))
    {
      int index = this._hiddenAnchorables.IndexOf(oldElement as LayoutAnchorable);
      this._hiddenAnchorables.Remove(oldElement as LayoutAnchorable);
      this._hiddenAnchorables.Insert(index, newElement as LayoutAnchorable);
    }
    else if (oldElement == this.TopSide)
      this.TopSide = (LayoutAnchorSide) newElement;
    else if (oldElement == this.RightSide)
      this.RightSide = (LayoutAnchorSide) newElement;
    else if (oldElement == this.BottomSide)
    {
      this.BottomSide = (LayoutAnchorSide) newElement;
    }
    else
    {
      if (oldElement != this.LeftSide)
        return;
      this.LeftSide = (LayoutAnchorSide) newElement;
    }
  }

  public void CollectGarbage()
  {
    bool flag1;
    do
    {
      flag1 = true;
      foreach (ILayoutPreviousContainer previousContainer in this.Descendents().OfType<ILayoutPreviousContainer>().Where<ILayoutPreviousContainer>((Func<ILayoutPreviousContainer, bool>) (c =>
      {
        if (c.PreviousContainer == null)
          return false;
        return c.PreviousContainer.Parent == null || c.PreviousContainer.Parent.Root != this;
      })))
        previousContainer.PreviousContainer = (ILayoutContainer) null;
      foreach (ILayoutPane layoutPane in this.Descendents().OfType<ILayoutPane>().Where<ILayoutPane>((Func<ILayoutPane, bool>) (p => p.ChildrenCount == 0)))
      {
        ILayoutPane emptyPane = layoutPane;
        foreach (LayoutContent layoutContent in this.Descendents().OfType<LayoutContent>().Where<LayoutContent>((Func<LayoutContent, bool>) (c => ((ILayoutPreviousContainer) c).PreviousContainer == emptyPane && !c.IsFloating)))
        {
          if (!(layoutContent is LayoutAnchorable) || ((LayoutAnchorable) layoutContent).IsVisible)
          {
            ((ILayoutPreviousContainer) layoutContent).PreviousContainer = (ILayoutContainer) null;
            layoutContent.PreviousContainerIndex = -1;
          }
        }
        if ((!(emptyPane is LayoutDocumentPane) || this.Descendents().OfType<LayoutDocumentPane>().Count<LayoutDocumentPane>((Func<LayoutDocumentPane, bool>) (c => c != emptyPane)) != 0) && !this.Descendents().OfType<ILayoutPreviousContainer>().Any<ILayoutPreviousContainer>((Func<ILayoutPreviousContainer, bool>) (c => c.PreviousContainer == emptyPane)))
        {
          emptyPane.Parent.RemoveChild((ILayoutElement) emptyPane);
          flag1 = false;
          break;
        }
      }
      if (!flag1)
      {
        using (IEnumerator<LayoutAnchorablePaneGroup> enumerator = this.Descendents().OfType<LayoutAnchorablePaneGroup>().Where<LayoutAnchorablePaneGroup>((Func<LayoutAnchorablePaneGroup, bool>) (p => p.ChildrenCount == 0)).GetEnumerator())
        {
          if (enumerator.MoveNext())
          {
            LayoutAnchorablePaneGroup current = enumerator.Current;
            current.Parent.RemoveChild((ILayoutElement) current);
            flag1 = false;
          }
        }
      }
      if (!flag1)
      {
        using (IEnumerator<LayoutPanel> enumerator = this.Descendents().OfType<LayoutPanel>().Where<LayoutPanel>((Func<LayoutPanel, bool>) (p => p.ChildrenCount == 0)).GetEnumerator())
        {
          if (enumerator.MoveNext())
          {
            LayoutPanel current = enumerator.Current;
            current.Parent.RemoveChild((ILayoutElement) current);
            flag1 = false;
          }
        }
      }
      if (!flag1)
      {
        using (IEnumerator<LayoutFloatingWindow> enumerator = this.Descendents().OfType<LayoutFloatingWindow>().Where<LayoutFloatingWindow>((Func<LayoutFloatingWindow, bool>) (p => p.ChildrenCount == 0)).GetEnumerator())
        {
          if (enumerator.MoveNext())
          {
            LayoutFloatingWindow current = enumerator.Current;
            current.Parent.RemoveChild((ILayoutElement) current);
            flag1 = false;
          }
        }
      }
      if (!flag1)
      {
        using (IEnumerator<LayoutAnchorGroup> enumerator = this.Descendents().OfType<LayoutAnchorGroup>().Where<LayoutAnchorGroup>((Func<LayoutAnchorGroup, bool>) (p => p.ChildrenCount == 0)).GetEnumerator())
        {
          if (enumerator.MoveNext())
          {
            LayoutAnchorGroup current = enumerator.Current;
            current.Parent.RemoveChild((ILayoutElement) current);
            flag1 = false;
          }
        }
      }
    }
    while (!flag1);
    bool flag2;
    do
    {
      flag2 = true;
      LayoutAnchorablePaneGroup[] array = this.Descendents().OfType<LayoutAnchorablePaneGroup>().Where<LayoutAnchorablePaneGroup>((Func<LayoutAnchorablePaneGroup, bool>) (p => p.ChildrenCount == 1 && p.Children[0] is LayoutAnchorablePaneGroup)).ToArray<LayoutAnchorablePaneGroup>();
      int index = 0;
      if (index < array.Length)
      {
        LayoutAnchorablePaneGroup anchorablePaneGroup = array[index];
        LayoutAnchorablePaneGroup child = anchorablePaneGroup.Children[0] as LayoutAnchorablePaneGroup;
        anchorablePaneGroup.Orientation = child.Orientation;
        anchorablePaneGroup.RemoveChild((ILayoutElement) child);
        while (child.ChildrenCount > 0)
          anchorablePaneGroup.InsertChildAt(anchorablePaneGroup.ChildrenCount, (ILayoutElement) child.Children[0]);
        flag2 = false;
      }
    }
    while (!flag2);
    bool flag3;
    do
    {
      flag3 = true;
      LayoutDocumentPaneGroup[] array = this.Descendents().OfType<LayoutDocumentPaneGroup>().Where<LayoutDocumentPaneGroup>((Func<LayoutDocumentPaneGroup, bool>) (p => p.ChildrenCount == 1 && p.Children[0] is LayoutDocumentPaneGroup)).ToArray<LayoutDocumentPaneGroup>();
      int index = 0;
      if (index < array.Length)
      {
        LayoutDocumentPaneGroup documentPaneGroup = array[index];
        LayoutDocumentPaneGroup child = documentPaneGroup.Children[0] as LayoutDocumentPaneGroup;
        documentPaneGroup.Orientation = child.Orientation;
        documentPaneGroup.RemoveChild((ILayoutElement) child);
        while (child.ChildrenCount > 0)
          documentPaneGroup.InsertChildAt(documentPaneGroup.ChildrenCount, (ILayoutElement) child.Children[0]);
        flag3 = false;
      }
    }
    while (!flag3);
    this.UpdateActiveContentProperty();
  }

  public XmlSchema GetSchema() => (XmlSchema) null;

  public void ReadXml(XmlReader reader)
  {
    int content = (int) reader.MoveToContent();
    if (reader.IsEmptyElement)
    {
      reader.Read();
    }
    else
    {
      Orientation orientation;
      List<ILayoutPanelElement> layoutPanelElementList = this.ReadRootPanel(reader, out orientation);
      if (layoutPanelElementList != null)
      {
        this.RootPanel = new LayoutPanel()
        {
          Orientation = orientation
        };
        for (int index = 0; index < layoutPanelElementList.Count; ++index)
          this.RootPanel.Children.Add(layoutPanelElementList[index]);
      }
      this.TopSide = new LayoutAnchorSide();
      if (this.ReadElement(reader) != null)
        this.FillLayoutAnchorSide(reader, this.TopSide);
      this.RightSide = new LayoutAnchorSide();
      if (this.ReadElement(reader) != null)
        this.FillLayoutAnchorSide(reader, this.RightSide);
      this.LeftSide = new LayoutAnchorSide();
      if (this.ReadElement(reader) != null)
        this.FillLayoutAnchorSide(reader, this.LeftSide);
      this.BottomSide = new LayoutAnchorSide();
      if (this.ReadElement(reader) != null)
        this.FillLayoutAnchorSide(reader, this.BottomSide);
      this.FloatingWindows.Clear();
      foreach (LayoutFloatingWindow readElement in this.ReadElementList(reader, true))
        this.FloatingWindows.Add(readElement);
      this.Hidden.Clear();
      foreach (LayoutAnchorable readElement in this.ReadElementList(reader, false))
        this.Hidden.Add(readElement);
    }
  }

  public void WriteXml(XmlWriter writer)
  {
    writer.WriteStartElement("RootPanel");
    if (this.RootPanel != null)
      this.RootPanel.WriteXml(writer);
    writer.WriteEndElement();
    writer.WriteStartElement("TopSide");
    if (this.TopSide != null)
      this.TopSide.WriteXml(writer);
    writer.WriteEndElement();
    writer.WriteStartElement("RightSide");
    if (this.RightSide != null)
      this.RightSide.WriteXml(writer);
    writer.WriteEndElement();
    writer.WriteStartElement("LeftSide");
    if (this.LeftSide != null)
      this.LeftSide.WriteXml(writer);
    writer.WriteEndElement();
    writer.WriteStartElement("BottomSide");
    if (this.BottomSide != null)
      this.BottomSide.WriteXml(writer);
    writer.WriteEndElement();
    writer.WriteStartElement("FloatingWindows");
    foreach (LayoutFloatingWindow floatingWindow in (Collection<LayoutFloatingWindow>) this.FloatingWindows)
    {
      writer.WriteStartElement(floatingWindow.GetType().Name);
      floatingWindow.WriteXml(writer);
      writer.WriteEndElement();
    }
    writer.WriteEndElement();
    writer.WriteStartElement("Hidden");
    foreach (LayoutAnchorable layoutAnchorable in (Collection<LayoutAnchorable>) this.Hidden)
    {
      writer.WriteStartElement(layoutAnchorable.GetType().Name);
      layoutAnchorable.WriteXml(writer);
      writer.WriteEndElement();
    }
    writer.WriteEndElement();
  }

  internal static Type FindType(string name)
  {
    foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
    {
      foreach (Type type in assembly.GetTypes())
      {
        if (type.Name.Equals(name))
          return type;
      }
    }
    return (Type) null;
  }

  internal void FireLayoutUpdated()
  {
    if (this.Updated == null)
      return;
    this.Updated((object) this, EventArgs.Empty);
  }

  internal void OnLayoutElementAdded(LayoutElement element)
  {
    if (this.ElementAdded == null)
      return;
    this.ElementAdded((object) this, new LayoutElementEventArgs(element));
  }

  internal void OnLayoutElementRemoved(LayoutElement element)
  {
    if (element.Descendents().OfType<LayoutContent>().Any<LayoutContent>((Func<LayoutContent, bool>) (c => c == this.LastFocusedDocument)))
      this.LastFocusedDocument = (LayoutContent) null;
    if (element.Descendents().OfType<LayoutContent>().Any<LayoutContent>((Func<LayoutContent, bool>) (c => c == this.ActiveContent)))
      this.ActiveContent = (LayoutContent) null;
    if (this.ElementRemoved == null)
      return;
    this.ElementRemoved((object) this, new LayoutElementEventArgs(element));
  }

  private void _floatingWindows_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
  {
    if (e.OldItems != null && (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace))
    {
      foreach (LayoutFloatingWindow oldItem in (IEnumerable) e.OldItems)
      {
        if (oldItem.Parent == this)
          oldItem.Parent = (ILayoutContainer) null;
      }
    }
    if (e.NewItems == null || e.Action != NotifyCollectionChangedAction.Add && e.Action != NotifyCollectionChangedAction.Replace)
      return;
    foreach (LayoutElement newItem in (IEnumerable) e.NewItems)
      newItem.Parent = (ILayoutContainer) this;
  }

  private void _hiddenAnchorables_CollectionChanged(
    object sender,
    NotifyCollectionChangedEventArgs e)
  {
    if ((e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace) && e.OldItems != null)
    {
      foreach (LayoutAnchorable oldItem in (IEnumerable) e.OldItems)
      {
        if (oldItem.Parent == this)
          oldItem.Parent = (ILayoutContainer) null;
      }
    }
    if (e.Action != NotifyCollectionChangedAction.Add && e.Action != NotifyCollectionChangedAction.Replace || e.NewItems == null)
      return;
    foreach (LayoutAnchorable newItem in (IEnumerable) e.NewItems)
    {
      if (newItem.Parent != this)
      {
        if (newItem.Parent != null)
          newItem.Parent.RemoveChild((ILayoutElement) newItem);
        newItem.Parent = (ILayoutContainer) this;
      }
    }
  }

  private void InternalSetActiveContent(LayoutContent currentValue, LayoutContent newActiveContent)
  {
    this.RaisePropertyChanging("ActiveContent");
    if (currentValue != null)
      currentValue.IsActive = false;
    this._activeContent = new WeakReference((object) newActiveContent);
    currentValue = this.ActiveContent;
    if (currentValue != null)
      currentValue.IsActive = true;
    this.RaisePropertyChanged("ActiveContent");
    this._activeContentSet = currentValue != null;
    if (currentValue != null)
    {
      if (!(currentValue.Parent is LayoutDocumentPane) && !(currentValue is LayoutDocument))
        return;
      this.LastFocusedDocument = currentValue;
    }
    else
      this.LastFocusedDocument = (LayoutContent) null;
  }

  private void UpdateActiveContentProperty()
  {
    LayoutContent activeContent = this.ActiveContent;
    if (!this._activeContentSet || activeContent != null && activeContent.Root == this)
      return;
    this._activeContentSet = false;
    this.InternalSetActiveContent(activeContent, (LayoutContent) null);
  }

  private void FillLayoutAnchorSide(XmlReader reader, LayoutAnchorSide layoutAnchorSide)
  {
    List<LayoutAnchorGroup> layoutAnchorGroupList = new List<LayoutAnchorGroup>();
    while (true)
    {
      while (!(this.ReadElement(reader) is LayoutAnchorGroup layoutAnchorGroup))
      {
        if (reader.NodeType == XmlNodeType.EndElement)
        {
          reader.ReadEndElement();
          using (List<LayoutAnchorGroup>.Enumerator enumerator = layoutAnchorGroupList.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              LayoutAnchorGroup current = enumerator.Current;
              layoutAnchorSide.Children.Add(current);
            }
            return;
          }
        }
      }
      layoutAnchorGroupList.Add(layoutAnchorGroup);
    }
  }

  private List<ILayoutPanelElement> ReadRootPanel(XmlReader reader, out Orientation orientation)
  {
    orientation = Orientation.Horizontal;
    List<ILayoutPanelElement> layoutPanelElementList = new List<ILayoutPanelElement>();
    string localName = reader.LocalName;
    reader.Read();
    if (reader.LocalName.Equals(localName) && reader.NodeType == XmlNodeType.EndElement)
      return (List<ILayoutPanelElement>) null;
    while (reader.NodeType == XmlNodeType.Whitespace)
      reader.Read();
    if (reader.LocalName.Equals("RootPanel"))
    {
      orientation = reader.GetAttribute("Orientation") == "Vertical" ? Orientation.Vertical : Orientation.Horizontal;
      reader.Read();
      while (true)
      {
        while (!(this.ReadElement(reader) is ILayoutPanelElement layoutPanelElement))
        {
          if (reader.NodeType == XmlNodeType.EndElement)
            goto label_9;
        }
        layoutPanelElementList.Add(layoutPanelElement);
      }
    }
label_9:
    reader.ReadEndElement();
    return layoutPanelElementList;
  }

  private List<object> ReadElementList(XmlReader reader, bool isFloatingWindow)
  {
    List<object> objectList = new List<object>();
    while (reader.NodeType == XmlNodeType.Whitespace)
      reader.Read();
    if (reader.IsEmptyElement)
    {
      reader.Read();
      return objectList;
    }
    string localName = reader.LocalName;
    reader.Read();
    if (reader.LocalName.Equals(localName) && reader.NodeType == XmlNodeType.EndElement)
      return (List<object>) null;
    while (reader.NodeType == XmlNodeType.Whitespace)
      reader.Read();
    while (true)
    {
      while (!isFloatingWindow)
      {
        if (this.ReadElement(reader) is LayoutAnchorable layoutAnchorable)
          objectList.Add((object) layoutAnchorable);
        else
          goto label_14;
      }
      if (this.ReadElement(reader) is LayoutFloatingWindow layoutFloatingWindow)
        objectList.Add((object) layoutFloatingWindow);
      else
        break;
    }
label_14:
    reader.ReadEndElement();
    return objectList;
  }

  private object ReadElement(XmlReader reader)
  {
    while (reader.NodeType == XmlNodeType.Whitespace)
      reader.Read();
    if (reader.NodeType == XmlNodeType.EndElement)
      return (object) null;
    XmlSerializer xmlSerializer;
    switch (reader.LocalName)
    {
      case "BottomSide":
      case "LeftSide":
      case "RightSide":
      case "TopSide":
        if (!reader.IsEmptyElement)
          return (object) reader.Read();
        reader.Read();
        return (object) null;
      case "LayoutAnchorGroup":
        xmlSerializer = new XmlSerializer(typeof (LayoutAnchorGroup));
        break;
      case "LayoutAnchorable":
        xmlSerializer = new XmlSerializer(typeof (LayoutAnchorable));
        break;
      case "LayoutAnchorableFloatingWindow":
        xmlSerializer = new XmlSerializer(typeof (LayoutAnchorableFloatingWindow));
        break;
      case "LayoutAnchorablePane":
        xmlSerializer = new XmlSerializer(typeof (LayoutAnchorablePane));
        break;
      case "LayoutAnchorablePaneGroup":
        xmlSerializer = new XmlSerializer(typeof (LayoutAnchorablePaneGroup));
        break;
      case "LayoutDocument":
        xmlSerializer = new XmlSerializer(typeof (LayoutDocument));
        break;
      case "LayoutDocumentFloatingWindow":
        xmlSerializer = new XmlSerializer(typeof (LayoutDocumentFloatingWindow));
        break;
      case "LayoutDocumentPane":
        xmlSerializer = new XmlSerializer(typeof (LayoutDocumentPane));
        break;
      case "LayoutDocumentPaneGroup":
        xmlSerializer = new XmlSerializer(typeof (LayoutDocumentPaneGroup));
        break;
      case "LayoutPanel":
        xmlSerializer = new XmlSerializer(typeof (LayoutPanel));
        break;
      default:
        Type type = LayoutRoot.FindType(reader.LocalName);
        xmlSerializer = !(type == (Type) null) ? new XmlSerializer(type) : throw new ArgumentException("AvalonDock.LayoutRoot doesn't know how to deserialize " + reader.LocalName);
        break;
    }
    return xmlSerializer.Deserialize(reader);
  }

  public event EventHandler Updated;

  public event EventHandler<LayoutElementEventArgs> ElementAdded;

  public event EventHandler<LayoutElementEventArgs> ElementRemoved;
}

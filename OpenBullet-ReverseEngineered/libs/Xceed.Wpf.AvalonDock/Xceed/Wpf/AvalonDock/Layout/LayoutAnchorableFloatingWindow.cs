// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Layout.LayoutAnchorableFloatingWindow
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Serialization;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Layout;

[ContentProperty("RootPanel")]
[Serializable]
public class LayoutAnchorableFloatingWindow : LayoutFloatingWindow, ILayoutElementWithVisibility
{
  private LayoutAnchorablePaneGroup _rootPanel;
  [NonSerialized]
  private bool _isVisible = true;

  public bool IsSinglePane
  {
    get
    {
      return this.RootPanel != null && this.RootPanel.Descendents().OfType<ILayoutAnchorablePane>().Where<ILayoutAnchorablePane>((Func<ILayoutAnchorablePane, bool>) (p => p.IsVisible)).Count<ILayoutAnchorablePane>() == 1;
    }
  }

  [XmlIgnore]
  public bool IsVisible
  {
    get => this._isVisible;
    private set
    {
      if (this._isVisible == value)
        return;
      this.RaisePropertyChanging(nameof (IsVisible));
      this._isVisible = value;
      this.RaisePropertyChanged(nameof (IsVisible));
      if (this.IsVisibleChanged == null)
        return;
      this.IsVisibleChanged((object) this, EventArgs.Empty);
    }
  }

  public LayoutAnchorablePaneGroup RootPanel
  {
    get => this._rootPanel;
    set
    {
      if (this._rootPanel == value)
        return;
      this.RaisePropertyChanging(nameof (RootPanel));
      if (this._rootPanel != null)
        this._rootPanel.ChildrenTreeChanged -= new EventHandler<ChildrenTreeChangedEventArgs>(this._rootPanel_ChildrenTreeChanged);
      this._rootPanel = value;
      if (this._rootPanel != null)
        this._rootPanel.Parent = (ILayoutContainer) this;
      if (this._rootPanel != null)
        this._rootPanel.ChildrenTreeChanged += new EventHandler<ChildrenTreeChangedEventArgs>(this._rootPanel_ChildrenTreeChanged);
      this.RaisePropertyChanged(nameof (RootPanel));
      this.RaisePropertyChanged("IsSinglePane");
      this.RaisePropertyChanged("SinglePane");
      this.RaisePropertyChanged("Children");
      this.RaisePropertyChanged("ChildrenCount");
      ((ILayoutElementWithVisibility) this).ComputeVisibility();
    }
  }

  public ILayoutAnchorablePane SinglePane
  {
    get
    {
      if (!this.IsSinglePane)
        return (ILayoutAnchorablePane) null;
      LayoutAnchorablePane singlePane = this.RootPanel.Descendents().OfType<LayoutAnchorablePane>().Single<LayoutAnchorablePane>((Func<LayoutAnchorablePane, bool>) (p => p.IsVisible));
      singlePane.UpdateIsDirectlyHostedInFloatingWindow();
      return (ILayoutAnchorablePane) singlePane;
    }
  }

  public override IEnumerable<ILayoutElement> Children
  {
    get
    {
      LayoutAnchorableFloatingWindow anchorableFloatingWindow = this;
      if (anchorableFloatingWindow.ChildrenCount == 1)
        yield return (ILayoutElement) anchorableFloatingWindow.RootPanel;
    }
  }

  public override void RemoveChild(ILayoutElement element)
  {
    this.RootPanel = (LayoutAnchorablePaneGroup) null;
  }

  public override void ReplaceChild(ILayoutElement oldElement, ILayoutElement newElement)
  {
    this.RootPanel = newElement as LayoutAnchorablePaneGroup;
  }

  public override int ChildrenCount => this.RootPanel == null ? 0 : 1;

  public override bool IsValid => this.RootPanel != null;

  public override void ReadXml(XmlReader reader)
  {
    int content = (int) reader.MoveToContent();
    if (reader.IsEmptyElement)
    {
      reader.Read();
      this.ComputeVisibility();
    }
    else
    {
      string localName = reader.LocalName;
      reader.Read();
      while (!reader.LocalName.Equals(localName) || reader.NodeType != XmlNodeType.EndElement)
      {
        if (reader.NodeType == XmlNodeType.Whitespace)
        {
          reader.Read();
        }
        else
        {
          XmlSerializer xmlSerializer;
          if (reader.LocalName.Equals("LayoutAnchorablePaneGroup"))
          {
            xmlSerializer = new XmlSerializer(typeof (LayoutAnchorablePaneGroup));
          }
          else
          {
            Type type = LayoutRoot.FindType(reader.LocalName);
            xmlSerializer = !(type == (Type) null) ? new XmlSerializer(type) : throw new ArgumentException("AvalonDock.LayoutAnchorableFloatingWindow doesn't know how to deserialize " + reader.LocalName);
          }
          this.RootPanel = (LayoutAnchorablePaneGroup) xmlSerializer.Deserialize(reader);
        }
      }
      reader.ReadEndElement();
    }
  }

  public override void ConsoleDump(int tab)
  {
    Trace.Write(new string(' ', tab * 4));
    Trace.WriteLine("FloatingAnchorableWindow()");
    this.RootPanel.ConsoleDump(tab + 1);
  }

  private void _rootPanel_ChildrenTreeChanged(object sender, ChildrenTreeChangedEventArgs e)
  {
    this.RaisePropertyChanged("IsSinglePane");
    this.RaisePropertyChanged("SinglePane");
  }

  private void ComputeVisibility()
  {
    if (this.RootPanel != null)
      this.IsVisible = this.RootPanel.IsVisible;
    else
      this.IsVisible = false;
  }

  public event EventHandler IsVisibleChanged;

  void ILayoutElementWithVisibility.ComputeVisibility() => this.ComputeVisibility();
}

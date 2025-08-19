// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Layout.LayoutDocumentFloatingWindow
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Serialization;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Layout;

[ContentProperty("RootDocument")]
[Serializable]
public class LayoutDocumentFloatingWindow : LayoutFloatingWindow
{
  private LayoutDocument _rootDocument;

  public LayoutDocument RootDocument
  {
    get => this._rootDocument;
    set
    {
      if (this._rootDocument == value)
        return;
      this.RaisePropertyChanging(nameof (RootDocument));
      this._rootDocument = value;
      if (this._rootDocument != null)
        this._rootDocument.Parent = (ILayoutContainer) this;
      this.RaisePropertyChanged(nameof (RootDocument));
      if (this.RootDocumentChanged == null)
        return;
      this.RootDocumentChanged((object) this, EventArgs.Empty);
    }
  }

  public override IEnumerable<ILayoutElement> Children
  {
    get
    {
      if (this.RootDocument != null)
        yield return (ILayoutElement) this.RootDocument;
    }
  }

  public override void RemoveChild(ILayoutElement element)
  {
    this.RootDocument = (LayoutDocument) null;
  }

  public override void ReplaceChild(ILayoutElement oldElement, ILayoutElement newElement)
  {
    this.RootDocument = newElement as LayoutDocument;
  }

  public override int ChildrenCount => this.RootDocument == null ? 0 : 1;

  public override bool IsValid => this.RootDocument != null;

  public override void ReadXml(XmlReader reader)
  {
    int content = (int) reader.MoveToContent();
    if (reader.IsEmptyElement)
    {
      reader.Read();
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
          if (reader.LocalName.Equals("LayoutDocument"))
          {
            xmlSerializer = new XmlSerializer(typeof (LayoutDocument));
          }
          else
          {
            Type type = LayoutRoot.FindType(reader.LocalName);
            xmlSerializer = !(type == (Type) null) ? new XmlSerializer(type) : throw new ArgumentException("AvalonDock.LayoutDocumentFloatingWindow doesn't know how to deserialize " + reader.LocalName);
          }
          this.RootDocument = (LayoutDocument) xmlSerializer.Deserialize(reader);
        }
      }
      reader.ReadEndElement();
    }
  }

  public override void ConsoleDump(int tab)
  {
    Trace.Write(new string(' ', tab * 4));
    Trace.WriteLine("FloatingDocumentWindow()");
    this.RootDocument.ConsoleDump(tab + 1);
  }

  public event EventHandler RootDocumentChanged;
}

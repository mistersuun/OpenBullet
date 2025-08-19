// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Layout.LayoutFloatingWindow
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Layout;

[Serializable]
public abstract class LayoutFloatingWindow : 
  LayoutElement,
  ILayoutContainer,
  ILayoutElement,
  INotifyPropertyChanged,
  INotifyPropertyChanging,
  IXmlSerializable
{
  public abstract IEnumerable<ILayoutElement> Children { get; }

  public abstract int ChildrenCount { get; }

  public abstract bool IsValid { get; }

  public abstract void RemoveChild(ILayoutElement element);

  public abstract void ReplaceChild(ILayoutElement oldElement, ILayoutElement newElement);

  public XmlSchema GetSchema() => (XmlSchema) null;

  public abstract void ReadXml(XmlReader reader);

  public virtual void WriteXml(XmlWriter writer)
  {
    foreach (ILayoutElement child in this.Children)
      new XmlSerializer(child.GetType()).Serialize(writer, (object) child);
  }
}

// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Layout.Serialization.XmlLayoutSerializer
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System.IO;
using System.Xml;
using System.Xml.Serialization;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Layout.Serialization;

public class XmlLayoutSerializer(DockingManager manager) : LayoutSerializer(manager)
{
  public void Serialize(XmlWriter writer)
  {
    new XmlSerializer(typeof (LayoutRoot)).Serialize(writer, (object) this.Manager.Layout);
  }

  public void Serialize(TextWriter writer)
  {
    new XmlSerializer(typeof (LayoutRoot)).Serialize(writer, (object) this.Manager.Layout);
  }

  public void Serialize(Stream stream)
  {
    new XmlSerializer(typeof (LayoutRoot)).Serialize(stream, (object) this.Manager.Layout);
  }

  public void Serialize(string filepath)
  {
    using (StreamWriter writer = new StreamWriter(filepath))
      this.Serialize((TextWriter) writer);
  }

  public void Deserialize(Stream stream)
  {
    try
    {
      this.StartDeserialization();
      LayoutRoot layout = new XmlSerializer(typeof (LayoutRoot)).Deserialize(stream) as LayoutRoot;
      this.FixupLayout(layout);
      this.Manager.Layout = layout;
    }
    finally
    {
      this.EndDeserialization();
    }
  }

  public void Deserialize(TextReader reader)
  {
    try
    {
      this.StartDeserialization();
      LayoutRoot layout = new XmlSerializer(typeof (LayoutRoot)).Deserialize(reader) as LayoutRoot;
      this.FixupLayout(layout);
      this.Manager.Layout = layout;
    }
    finally
    {
      this.EndDeserialization();
    }
  }

  public void Deserialize(XmlReader reader)
  {
    try
    {
      this.StartDeserialization();
      LayoutRoot layout = new XmlSerializer(typeof (LayoutRoot)).Deserialize(reader) as LayoutRoot;
      this.FixupLayout(layout);
      this.Manager.Layout = layout;
    }
    finally
    {
      this.EndDeserialization();
    }
  }

  public void Deserialize(string filepath)
  {
    using (StreamReader reader = new StreamReader(filepath))
      this.Deserialize((TextReader) reader);
  }
}

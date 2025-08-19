// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Converters.IXmlNode
// Assembly: Newtonsoft.Json, Version=12.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: D47DE75A-7E3F-422C-A4CA-64A654C80495
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Newtonsoft.Json.dll

using System.Collections.Generic;
using System.Xml;

#nullable disable
namespace Newtonsoft.Json.Converters;

internal interface IXmlNode
{
  XmlNodeType NodeType { get; }

  string LocalName { get; }

  List<IXmlNode> ChildNodes { get; }

  List<IXmlNode> Attributes { get; }

  IXmlNode ParentNode { get; }

  string Value { get; set; }

  IXmlNode AppendChild(IXmlNode newChild);

  string NamespaceUri { get; }

  object WrappedNode { get; }
}

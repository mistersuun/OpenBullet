// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Converters.XDeclarationWrapper
// Assembly: Newtonsoft.Json, Version=12.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: D47DE75A-7E3F-422C-A4CA-64A654C80495
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Newtonsoft.Json.dll

using System.Xml;
using System.Xml.Linq;

#nullable disable
namespace Newtonsoft.Json.Converters;

internal class XDeclarationWrapper : XObjectWrapper, IXmlDeclaration, IXmlNode
{
  internal XDeclaration Declaration { get; }

  public XDeclarationWrapper(XDeclaration declaration)
    : base((XObject) null)
  {
    this.Declaration = declaration;
  }

  public override XmlNodeType NodeType => XmlNodeType.XmlDeclaration;

  public string Version => this.Declaration.Version;

  public string Encoding
  {
    get => this.Declaration.Encoding;
    set => this.Declaration.Encoding = value;
  }

  public string Standalone
  {
    get => this.Declaration.Standalone;
    set => this.Declaration.Standalone = value;
  }
}

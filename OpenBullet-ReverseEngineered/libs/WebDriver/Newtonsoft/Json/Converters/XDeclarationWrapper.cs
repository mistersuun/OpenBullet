// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Converters.XDeclarationWrapper
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

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

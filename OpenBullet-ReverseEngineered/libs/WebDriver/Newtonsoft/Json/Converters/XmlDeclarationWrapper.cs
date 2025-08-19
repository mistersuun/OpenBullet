// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Converters.XmlDeclarationWrapper
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System.Xml;

#nullable disable
namespace Newtonsoft.Json.Converters;

internal class XmlDeclarationWrapper : XmlNodeWrapper, IXmlDeclaration, IXmlNode
{
  private readonly XmlDeclaration _declaration;

  public XmlDeclarationWrapper(XmlDeclaration declaration)
    : base((XmlNode) declaration)
  {
    this._declaration = declaration;
  }

  public string Version => this._declaration.Version;

  public string Encoding
  {
    get => this._declaration.Encoding;
    set => this._declaration.Encoding = value;
  }

  public string Standalone
  {
    get => this._declaration.Standalone;
    set => this._declaration.Standalone = value;
  }
}

// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Converters.XmlDocumentTypeWrapper
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System.Xml;

#nullable disable
namespace Newtonsoft.Json.Converters;

internal class XmlDocumentTypeWrapper : XmlNodeWrapper, IXmlDocumentType, IXmlNode
{
  private readonly XmlDocumentType _documentType;

  public XmlDocumentTypeWrapper(XmlDocumentType documentType)
    : base((XmlNode) documentType)
  {
    this._documentType = documentType;
  }

  public string Name => this._documentType.Name;

  public string System => this._documentType.SystemId;

  public string Public => this._documentType.PublicId;

  public string InternalSubset => this._documentType.InternalSubset;

  public override string LocalName => "DOCTYPE";
}

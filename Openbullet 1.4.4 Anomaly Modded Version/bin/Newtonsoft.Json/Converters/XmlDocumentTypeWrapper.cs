// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Converters.XmlDocumentTypeWrapper
// Assembly: Newtonsoft.Json, Version=12.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: D47DE75A-7E3F-422C-A4CA-64A654C80495
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Newtonsoft.Json.dll

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

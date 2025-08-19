// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.Entity
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;

#nullable disable
namespace AngleSharp.Dom;

[DomName("Entity")]
public sealed class Entity(Document owner, string name) : Node(owner, name, NodeType.Entity)
{
  private string _publicId;
  private string _systemId;
  private string _notationName;
  private string _inputEncoding;
  private string _xmlVersion;
  private string _xmlEncoding;
  private string _value;

  public Entity(Document owner)
    : this(owner, string.Empty)
  {
  }

  [DomName("publicId")]
  public string PublicId => this._publicId;

  [DomName("systemId")]
  public string SystemId => this._systemId;

  [DomName("notationName")]
  public string NotationName
  {
    get => this._notationName;
    set => this._notationName = value;
  }

  [DomName("inputEncoding")]
  public string InputEncoding => this._inputEncoding;

  [DomName("xmlEncoding")]
  public string XmlEncoding => this._xmlEncoding;

  [DomName("xmlVersion")]
  public string XmlVersion => this._xmlVersion;

  [DomName("textContent")]
  public override string TextContent
  {
    get => this.NodeValue;
    set => this.NodeValue = value;
  }

  [DomName("nodeValue")]
  public override string NodeValue
  {
    get => this._value;
    set => this._value = value;
  }

  public override Node Clone(Document newOwner, bool deep)
  {
    Entity target = new Entity(newOwner, this.NodeName);
    this.CloneNode((Node) target, newOwner, deep);
    target._xmlEncoding = this._xmlEncoding;
    target._xmlVersion = this._xmlVersion;
    target._systemId = this._systemId;
    target._publicId = this._publicId;
    target._inputEncoding = this._inputEncoding;
    target._notationName = this._notationName;
    return (Node) target;
  }
}

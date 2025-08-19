// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.Attr
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Text;
using System;

#nullable disable
namespace AngleSharp.Dom;

public sealed class Attr : IAttr, IEquatable<IAttr>
{
  private readonly string _localName;
  private readonly string _prefix;
  private readonly string _namespace;
  private string _value;

  public Attr(string localName)
    : this(localName, string.Empty)
  {
  }

  public Attr(string localName, string value)
  {
    this._localName = localName;
    this._value = value;
  }

  public Attr(string prefix, string localName, string value, string namespaceUri)
  {
    this._prefix = prefix;
    this._localName = localName;
    this._value = value;
    this._namespace = namespaceUri;
  }

  internal NamedNodeMap Container { get; set; }

  public string Prefix => this._prefix;

  public bool IsId => this._prefix == null && this._localName.Isi(AttributeNames.Id);

  public bool Specified => !string.IsNullOrEmpty(this._value);

  public string Name
  {
    get => this._prefix != null ? $"{this._prefix}:{this._localName}" : this._localName;
  }

  public string Value
  {
    get => this._value;
    set
    {
      string oldValue = this._value;
      this._value = value;
      this.Container?.RaiseChangedEvent(this, value, oldValue);
    }
  }

  public string LocalName => this._localName;

  public string NamespaceUri => this._namespace;

  public bool Equals(IAttr other)
  {
    return this.Prefix.Is(other.Prefix) && this.NamespaceUri.Is(other.NamespaceUri) && this.Value.Is(other.Value);
  }

  public override int GetHashCode()
  {
    return (((1 * 31 /*0x1F*/ + this._localName.GetHashCode()) * 31 /*0x1F*/ + (this._value ?? string.Empty).GetHashCode()) * 31 /*0x1F*/ + (this._namespace ?? string.Empty).GetHashCode()) * 31 /*0x1F*/ + (this._prefix ?? string.Empty).GetHashCode();
  }
}

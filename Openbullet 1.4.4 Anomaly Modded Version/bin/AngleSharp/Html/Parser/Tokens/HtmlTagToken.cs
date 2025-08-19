// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Parser.Tokens.HtmlTagToken
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Text;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Html.Parser.Tokens;

public sealed class HtmlTagToken(HtmlTokenType type, TextPosition position, string name) : 
  HtmlToken(type, position, name),
  ISourceReference
{
  private readonly List<HtmlAttributeToken> _attributes = new List<HtmlAttributeToken>();
  private bool _selfClosing;

  public HtmlTagToken(HtmlTokenType type, TextPosition position)
    : this(type, position, string.Empty)
  {
  }

  public static HtmlTagToken Open(string name)
  {
    return new HtmlTagToken(HtmlTokenType.StartTag, TextPosition.Empty, name);
  }

  public static HtmlTagToken Close(string name)
  {
    return new HtmlTagToken(HtmlTokenType.EndTag, TextPosition.Empty, name);
  }

  public bool IsSelfClosing
  {
    get => this._selfClosing;
    set => this._selfClosing = value;
  }

  public List<HtmlAttributeToken> Attributes => this._attributes;

  public void AddAttribute(string name, TextPosition position)
  {
    this._attributes.Add(new HtmlAttributeToken(position, name, string.Empty));
  }

  public void AddAttribute(string name, string value)
  {
    this._attributes.Add(new HtmlAttributeToken(TextPosition.Empty, name, value));
  }

  public void SetAttributeValue(string value)
  {
    int index = this._attributes.Count - 1;
    HtmlAttributeToken attribute = this._attributes[index];
    this._attributes[index] = new HtmlAttributeToken(attribute.Position, attribute.Name, value);
  }

  public string GetAttribute(string name)
  {
    for (int index = 0; index != this._attributes.Count; ++index)
    {
      HtmlAttributeToken attribute = this._attributes[index];
      if (attribute.Name.Is(name))
        return attribute.Value;
    }
    return string.Empty;
  }
}

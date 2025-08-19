// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Parser.Tokens.HtmlToken
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Text;

#nullable disable
namespace AngleSharp.Html.Parser.Tokens;

public class HtmlToken
{
  private readonly HtmlTokenType _type;
  private readonly TextPosition _position;
  private string _name;

  public HtmlToken(HtmlTokenType type, TextPosition position, string name = null)
  {
    this._type = type;
    this._position = position;
    this._name = name;
  }

  public bool HasContent
  {
    get
    {
      for (int index = 0; index < this._name.Length; ++index)
      {
        if (!this._name[index].IsSpaceCharacter())
          return true;
      }
      return false;
    }
  }

  public string Name
  {
    get => this._name;
    set => this._name = value;
  }

  public bool IsEmpty => string.IsNullOrEmpty(this._name);

  public string Data => this._name;

  public TextPosition Position => this._position;

  public bool IsHtmlCompatible
  {
    get => this._type == HtmlTokenType.StartTag || this._type == HtmlTokenType.Character;
  }

  public bool IsSvg => this.IsStartTag(TagNames.Svg);

  public bool IsMathCompatible
  {
    get
    {
      return !this.IsStartTag("mglyph") && !this.IsStartTag("malignmark") || this._type == HtmlTokenType.Character;
    }
  }

  public HtmlTokenType Type => this._type;

  public bool IsProcessingInstruction { get; internal set; }

  public string TrimStart()
  {
    int num = 0;
    while (num < this._name.Length && this._name[num].IsSpaceCharacter())
      ++num;
    string str = this._name.Substring(0, num);
    this._name = this._name.Substring(num);
    return str;
  }

  public void RemoveNewLine()
  {
    if (!this._name.Has('\n'))
      return;
    this._name = this._name.Substring(1);
  }

  public HtmlTagToken AsTag() => (HtmlTagToken) this;

  public bool IsStartTag(string name)
  {
    return this._type == HtmlTokenType.StartTag && this._name.Is(name);
  }
}

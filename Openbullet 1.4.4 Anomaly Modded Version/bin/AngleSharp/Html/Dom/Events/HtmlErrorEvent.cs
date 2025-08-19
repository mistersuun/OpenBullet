// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.Events.HtmlErrorEvent
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Common;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Html.Parser;
using AngleSharp.Text;

#nullable disable
namespace AngleSharp.Html.Dom.Events;

public class HtmlErrorEvent : Event
{
  private readonly HtmlParseError _code;
  private readonly TextPosition _position;

  public HtmlErrorEvent(HtmlParseError code, TextPosition position)
    : base(EventNames.Error)
  {
    this._code = code;
    this._position = position;
  }

  public TextPosition Position => this._position;

  public int Code => this._code.GetCode();

  public string Message => this._code.GetMessage<HtmlParseError>();
}

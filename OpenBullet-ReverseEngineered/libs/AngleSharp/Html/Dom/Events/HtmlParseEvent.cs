// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.Events.HtmlParseEvent
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Dom.Events;

#nullable disable
namespace AngleSharp.Html.Dom.Events;

public class HtmlParseEvent : Event
{
  public HtmlParseEvent(IHtmlDocument document, bool completed)
    : base(completed ? EventNames.Parsed : EventNames.Parsing)
  {
    this.Document = document;
  }

  public IHtmlDocument Document { get; private set; }
}

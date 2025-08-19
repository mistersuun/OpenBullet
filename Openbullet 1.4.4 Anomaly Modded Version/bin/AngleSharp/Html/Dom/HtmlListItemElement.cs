// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlListItemElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using System;
using System.Globalization;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class HtmlListItemElement(Document owner, string name = null, string prefix = null) : 
  HtmlElement(owner, name ?? TagNames.Li, prefix, NodeFlags.Special | NodeFlags.ImplicitelyClosed | NodeFlags.ImpliedEnd),
  IHtmlListItemElement,
  IHtmlElement,
  IElement,
  INode,
  IEventTarget,
  IMarkupFormattable,
  IParentNode,
  IChildNode,
  INonDocumentTypeChildNode,
  IGlobalEventHandlers
{
  public int? Value
  {
    get
    {
      int result;
      return !int.TryParse(this.GetOwnAttribute(AttributeNames.Value), NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result) ? new int?() : new int?(result);
    }
    set
    {
      this.SetOwnAttribute(AttributeNames.Value, value.HasValue ? value.Value.ToString() : (string) null);
    }
  }
}

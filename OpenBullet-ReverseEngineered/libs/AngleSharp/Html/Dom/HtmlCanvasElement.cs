// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlCanvasElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Common;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Io;
using AngleSharp.Media.Dom;
using AngleSharp.Text;
using System;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class HtmlCanvasElement : 
  HtmlElement,
  IHtmlCanvasElement,
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
  private readonly IEnumerable<IRenderingService> _renderServices;
  private HtmlCanvasElement.ContextMode _mode;
  private IRenderingContext _current;

  public HtmlCanvasElement(Document owner, string prefix = null)
    : base(owner, TagNames.Canvas, prefix)
  {
    this._renderServices = owner.Context.GetServices<IRenderingService>();
    this._mode = HtmlCanvasElement.ContextMode.None;
  }

  public int Width
  {
    get => this.GetOwnAttribute(AttributeNames.Width).ToInteger(300);
    set => this.SetOwnAttribute(AttributeNames.Width, value.ToString());
  }

  public int Height
  {
    get => this.GetOwnAttribute(AttributeNames.Height).ToInteger(150);
    set => this.SetOwnAttribute(AttributeNames.Height, value.ToString());
  }

  public IRenderingContext GetContext(string contextId)
  {
    if (this._current != null && !contextId.Isi(this._current.ContextId))
      return this._current;
    foreach (IRenderingService renderService in this._renderServices)
    {
      if (renderService.IsSupportingContext(contextId))
      {
        IRenderingContext context = renderService.CreateContext((IHtmlCanvasElement) this, contextId);
        if (context != null)
        {
          this._mode = HtmlCanvasElement.GetModeFrom(contextId);
          this._current = context;
        }
        return context;
      }
    }
    return (IRenderingContext) null;
  }

  public bool IsSupportingContext(string contextId)
  {
    foreach (IRenderingService renderService in this._renderServices)
    {
      if (renderService.IsSupportingContext(contextId))
        return true;
    }
    return false;
  }

  public void SetContext(IRenderingContext context)
  {
    if (this._mode != HtmlCanvasElement.ContextMode.None && this._mode != HtmlCanvasElement.ContextMode.Indirect)
      throw new DomException(DomError.InvalidState);
    if (context.IsFixed)
      throw new DomException(DomError.InvalidState);
    this._current = context.Host == this ? context : throw new DomException(DomError.InUse);
    this._mode = HtmlCanvasElement.ContextMode.Indirect;
  }

  public string ToDataUrl(string type = null) => Convert.ToBase64String(this.GetImageData(type));

  public void ToBlob(Action<Stream> callback, string type = null)
  {
    MemoryStream memoryStream = new MemoryStream(this.GetImageData(type));
    callback((Stream) memoryStream);
  }

  private byte[] GetImageData(string type)
  {
    return this._current?.ToImage(type ?? MimeTypeNames.Plain) ?? new byte[0];
  }

  private static HtmlCanvasElement.ContextMode GetModeFrom(string contextId)
  {
    if (contextId.Isi(Keywords.TwoD))
      return HtmlCanvasElement.ContextMode.Direct2d;
    return contextId.Isi(Keywords.WebGl) ? HtmlCanvasElement.ContextMode.DirectWebGl : HtmlCanvasElement.ContextMode.None;
  }

  private enum ContextMode : byte
  {
    None,
    Direct2d,
    DirectWebGl,
    Indirect,
    Proxied,
  }
}

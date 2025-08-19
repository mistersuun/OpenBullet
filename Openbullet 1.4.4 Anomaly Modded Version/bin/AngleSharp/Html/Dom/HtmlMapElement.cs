// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlMapElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Text;
using System;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class HtmlMapElement(Document owner, string prefix = null) : 
  HtmlElement(owner, TagNames.Map, prefix),
  IHtmlMapElement,
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
  private HtmlCollection<IHtmlAreaElement> _areas;
  private HtmlCollection<IHtmlImageElement> _images;

  public string Name
  {
    get => this.GetOwnAttribute(AttributeNames.Name);
    set => this.SetOwnAttribute(AttributeNames.Name, value);
  }

  public IHtmlCollection<IHtmlAreaElement> Areas
  {
    get
    {
      return (IHtmlCollection<IHtmlAreaElement>) this._areas ?? (IHtmlCollection<IHtmlAreaElement>) (this._areas = new HtmlCollection<IHtmlAreaElement>((INode) this, false));
    }
  }

  public IHtmlCollection<IHtmlImageElement> Images
  {
    get
    {
      return (IHtmlCollection<IHtmlImageElement>) this._images ?? (IHtmlCollection<IHtmlImageElement>) (this._images = new HtmlCollection<IHtmlImageElement>((INode) this.Owner.DocumentElement, predicate: new Func<IHtmlImageElement, bool>(this.IsAssociatedImage)));
    }
  }

  private bool IsAssociatedImage(IHtmlImageElement image)
  {
    string useMap = image.UseMap;
    if (string.IsNullOrEmpty(useMap))
      return false;
    string other = useMap.Has('#') ? "#" + this.Name : this.Name;
    return useMap.Is(other);
  }
}

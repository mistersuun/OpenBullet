// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.ImageExtensions
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Text;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Html.Dom;

public static class ImageExtensions
{
  public static Stack<IHtmlSourceElement> GetSources(this IHtmlImageElement img)
  {
    IElement parentElement = img.ParentElement;
    Stack<IHtmlSourceElement> sources = new Stack<IHtmlSourceElement>();
    if (parentElement != null && parentElement.LocalName.Is(TagNames.Picture))
    {
      for (IHtmlSourceElement previousElementSibling = img.PreviousElementSibling as IHtmlSourceElement; previousElementSibling != null; previousElementSibling = previousElementSibling.PreviousElementSibling as IHtmlSourceElement)
        sources.Push(previousElementSibling);
    }
    return sources;
  }
}

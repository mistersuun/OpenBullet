// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.LinkRels.StyleSheetLinkRelation
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Io;
using AngleSharp.Io.Processors;
using System.Threading.Tasks;

#nullable disable
namespace AngleSharp.Html.LinkRels;

internal class StyleSheetLinkRelation(IHtmlLinkElement link) : BaseLinkRelation(link, (IRequestProcessor) new StyleSheetRequestProcessor(link?.Owner.Context, link))
{
  public IStyleSheet Sheet
  {
    get
    {
      return !(this.Processor is StyleSheetRequestProcessor processor) ? (IStyleSheet) null : processor.Sheet;
    }
  }

  public override async Task LoadAsync()
  {
    StyleSheetLinkRelation sheetLinkRelation = this;
    if (sheetLinkRelation.Url == null)
      return;
    ResourceRequest requestFor = sheetLinkRelation.Link.CreateRequestFor(sheetLinkRelation.Url);
    await sheetLinkRelation.Processor?.ProcessAsync(requestFor);
  }
}

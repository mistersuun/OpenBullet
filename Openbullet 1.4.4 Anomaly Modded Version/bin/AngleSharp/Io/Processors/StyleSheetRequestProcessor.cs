// Decompiled with JetBrains decompiler
// Type: AngleSharp.Io.Processors.StyleSheetRequestProcessor
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Css;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Text;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace AngleSharp.Io.Processors;

internal sealed class StyleSheetRequestProcessor : BaseRequestProcessor
{
  private readonly IHtmlLinkElement _link;
  private readonly IBrowsingContext _context;
  private IStylingService _engine;

  public StyleSheetRequestProcessor(IBrowsingContext context, IHtmlLinkElement link)
    : base(context?.GetService<IResourceLoader>())
  {
    this._context = context;
    this._link = link;
  }

  public IStyleSheet Sheet { get; private set; }

  public IStylingService Engine
  {
    get => this._engine ?? (this._engine = this._context.GetStyling(this.LinkType));
  }

  public string LinkType => this._link.Type ?? MimeTypeNames.Css;

  public override Task ProcessAsync(ResourceRequest request)
  {
    if (!this.IsAvailable || this.Engine == null || !this.IsDifferentToCurrentDownloadUrl(request.Target))
      return Task.CompletedTask;
    this.CancelDownload();
    this.Download = this.DownloadWithCors(new CorsRequest(request)
    {
      Setting = this._link.CrossOrigin.ToEnum<CorsSetting>(CorsSetting.None),
      Behavior = OriginBehavior.Taint,
      Integrity = this._context.GetProvider<IIntegrityProvider>()
    });
    return this.FinishDownloadAsync();
  }

  protected override async Task ProcessResponseAsync(IResponse response)
  {
    CancellationToken none = CancellationToken.None;
    StyleOptions options = new StyleOptions(this._link.Owner)
    {
      Element = (IElement) this._link,
      IsDisabled = this._link.IsDisabled,
      IsAlternate = this._link.RelationList.Contains(AngleSharp.Common.Keywords.Alternate)
    };
    IStyleSheet styleSheet = await this._engine.ParseStylesheetAsync(response, options, none).ConfigureAwait(false);
    styleSheet.Media.MediaText = this._link.Media ?? string.Empty;
    this.Sheet = styleSheet;
  }
}

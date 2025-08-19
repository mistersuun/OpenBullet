// Decompiled with JetBrains decompiler
// Type: AngleSharp.Io.Processors.ScriptRequestProcessor
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Scripting;
using AngleSharp.Text;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace AngleSharp.Io.Processors;

internal sealed class ScriptRequestProcessor : IRequestProcessor
{
  private readonly IBrowsingContext _context;
  private readonly Document _document;
  private readonly HtmlScriptElement _script;
  private readonly IResourceLoader _loader;
  private IResponse _response;
  private IScriptingService _engine;

  public ScriptRequestProcessor(IBrowsingContext context, HtmlScriptElement script)
  {
    this._context = context;
    this._document = script.Owner;
    this._script = script;
    this._loader = context.GetService<IResourceLoader>();
  }

  public IDownload Download { get; private set; }

  public IScriptingService Engine
  {
    get => this._engine ?? (this._engine = this._context.GetScripting(this.ScriptLanguage));
  }

  public string AlternativeLanguage
  {
    get
    {
      string ownAttribute = this._script.GetOwnAttribute(AttributeNames.Language);
      return ownAttribute == null ? (string) null : "text/" + ownAttribute;
    }
  }

  public string ScriptLanguage
  {
    get
    {
      string str = this._script.Type ?? this.AlternativeLanguage;
      return !string.IsNullOrEmpty(str) ? str : MimeTypeNames.DefaultJavaScript;
    }
  }

  public async Task RunAsync(CancellationToken cancel)
  {
    ScriptRequestProcessor requestProcessor = this;
    IDownload download = requestProcessor.Download;
    if (download != null)
    {
      try
      {
        IResponse response = await download.Task.ConfigureAwait(false);
        requestProcessor._response = response;
      }
      catch
      {
        await requestProcessor._document.QueueTaskAsync(new Action<CancellationToken>(requestProcessor.FireErrorEvent)).ConfigureAwait(false);
      }
    }
    if (requestProcessor._response == null)
      return;
    if (await requestProcessor._document.QueueTaskAsync<bool>(new Func<CancellationToken, bool>(requestProcessor.FireBeforeScriptExecuteEvent)).ConfigureAwait(false))
      return;
    ScriptOptions options = requestProcessor.CreateOptions();
    int index = requestProcessor._document.Source.Index;
    try
    {
      await requestProcessor._engine.EvaluateScriptAsync(requestProcessor._response, options, cancel).ConfigureAwait(false);
    }
    catch (Exception ex)
    {
    }
    requestProcessor._document.Source.Index = index;
    ConfiguredTaskAwaitable configuredTaskAwaitable = requestProcessor._document.QueueTaskAsync(new Action<CancellationToken>(requestProcessor.FireAfterScriptExecuteEvent)).ConfigureAwait(false);
    await configuredTaskAwaitable;
    configuredTaskAwaitable = requestProcessor._document.QueueTaskAsync(new Action<CancellationToken>(requestProcessor.FireLoadEvent)).ConfigureAwait(false);
    await configuredTaskAwaitable;
    requestProcessor._response.Dispose();
    requestProcessor._response = (IResponse) null;
  }

  public void Process(string content)
  {
    if (this.Engine == null)
      return;
    this._response = VirtualResponse.Create((Action<VirtualResponse>) (res => res.Content(content).Address(this._script.BaseUri)));
  }

  public Task ProcessAsync(ResourceRequest request)
  {
    if (this._loader == null || this.Engine == null)
      return Task.CompletedTask;
    this.Download = this._loader.FetchWithCorsAsync(new CorsRequest(request)
    {
      Behavior = OriginBehavior.Taint,
      Setting = this._script.CrossOrigin.ToEnum<CorsSetting>(CorsSetting.None),
      Integrity = this._context.GetProvider<IIntegrityProvider>()
    });
    return (Task) this.Download.Task;
  }

  private ScriptOptions CreateOptions()
  {
    return new ScriptOptions((IDocument) this._document, this._document.Loop)
    {
      Element = (IHtmlScriptElement) this._script,
      Encoding = TextEncoding.Resolve(this._script.CharacterSet)
    };
  }

  private void FireLoadEvent(CancellationToken _) => this._script.FireSimpleEvent(EventNames.Load);

  private void FireErrorEvent(CancellationToken _)
  {
    this._script.FireSimpleEvent(EventNames.Error);
  }

  private bool FireBeforeScriptExecuteEvent(CancellationToken _)
  {
    return this._script.FireSimpleEvent(EventNames.BeforeScriptExecute, cancelable: true);
  }

  private void FireAfterScriptExecuteEvent(CancellationToken _)
  {
    this._script.FireSimpleEvent(EventNames.AfterScriptExecute, true);
  }
}

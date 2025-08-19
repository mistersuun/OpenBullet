// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlScriptElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Io;
using AngleSharp.Io.Processors;
using System;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class HtmlScriptElement : 
  HtmlElement,
  IHtmlScriptElement,
  IHtmlElement,
  IElement,
  INode,
  IEventTarget,
  IMarkupFormattable,
  IParentNode,
  IChildNode,
  INonDocumentTypeChildNode,
  IGlobalEventHandlers,
  ILoadableElement
{
  private readonly bool _parserInserted;
  private readonly ScriptRequestProcessor _request;
  private bool _started;
  private bool _forceAsync;

  public HtmlScriptElement(Document owner, string prefix = null, bool parserInserted = false, bool started = false)
    : base(owner, TagNames.Script, prefix, NodeFlags.Special | NodeFlags.LiteralText)
  {
    this._forceAsync = false;
    this._started = started;
    this._parserInserted = parserInserted;
    this._request = new ScriptRequestProcessor(owner.Context, this);
  }

  public IDownload CurrentDownload => this._request?.Download;

  public string Source
  {
    get => this.GetOwnAttribute(AttributeNames.Src);
    set => this.SetOwnAttribute(AttributeNames.Src, value);
  }

  public string Type
  {
    get => this.GetOwnAttribute(AttributeNames.Type);
    set => this.SetOwnAttribute(AttributeNames.Type, value);
  }

  public string CharacterSet
  {
    get => this.GetOwnAttribute(AttributeNames.Charset);
    set => this.SetOwnAttribute(AttributeNames.Charset, value);
  }

  public string Text
  {
    get => this.TextContent;
    set => this.TextContent = value;
  }

  public string CrossOrigin
  {
    get => this.GetOwnAttribute(AttributeNames.CrossOrigin);
    set => this.SetOwnAttribute(AttributeNames.CrossOrigin, value);
  }

  public bool IsDeferred
  {
    get => this.GetBoolAttribute(AttributeNames.Defer);
    set => this.SetBoolAttribute(AttributeNames.Defer, value);
  }

  public bool IsAsync
  {
    get => this.GetBoolAttribute(AttributeNames.Async);
    set => this.SetBoolAttribute(AttributeNames.Async, value);
  }

  public string Integrity
  {
    get => this.GetOwnAttribute(AttributeNames.Integrity);
    set => this.SetOwnAttribute(AttributeNames.Integrity, value);
  }

  public override Node Clone(Document owner, bool deep)
  {
    HtmlScriptElement htmlScriptElement = new HtmlScriptElement(owner, this.Prefix, this._parserInserted, this._started);
    this.CloneElement((Element) htmlScriptElement, owner, deep);
    htmlScriptElement._forceAsync = this._forceAsync;
    return (Node) htmlScriptElement;
  }

  protected override void OnParentChanged()
  {
    base.OnParentChanged();
    if (this._parserInserted || !this.Prepare(this.Owner))
      return;
    this.RunAsync(CancellationToken.None);
  }

  internal Task RunAsync(CancellationToken cancel) => this._request?.RunAsync(cancel);

  internal bool Prepare(Document document)
  {
    string ownAttribute1 = this.GetOwnAttribute(AttributeNames.Event);
    string ownAttribute2 = this.GetOwnAttribute(AttributeNames.For);
    string source = this.Source;
    bool parserInserted = this._parserInserted;
    if (this._started)
      return false;
    if (parserInserted)
      this._forceAsync = !this.IsAsync;
    if (string.IsNullOrEmpty(source) && string.IsNullOrEmpty(this.Text) || this._request.Engine == null)
      return false;
    if (parserInserted)
      this._forceAsync = false;
    this._started = true;
    if (!string.IsNullOrEmpty(ownAttribute1) && !string.IsNullOrEmpty(ownAttribute2))
    {
      string str1 = ownAttribute1.Trim();
      string str2 = ownAttribute2.Trim();
      if (str1.EndsWith("()"))
        str1 = str1.Substring(0, str1.Length - 2);
      int num = str2.Equals(AttributeNames.Window, StringComparison.OrdinalIgnoreCase) ? 1 : 0;
      bool flag = str1.Equals("onload", StringComparison.OrdinalIgnoreCase);
      if (num == 0 || !flag)
        return false;
    }
    switch (source)
    {
      case null:
        this._request.Process(this.Text);
        return true;
      case "":
        document.QueueTask(new Action(this.FireErrorEvent));
        return false;
      default:
        return this.InvokeLoadingScript(document, this.HyperReference(source));
    }
  }

  private bool InvokeLoadingScript(Document document, Url url)
  {
    bool flag = true;
    if (this._parserInserted && (this.IsDeferred || this.IsAsync))
    {
      document.AddScript(this);
      flag = false;
    }
    this.Process((IRequestProcessor) this._request, url);
    return flag;
  }

  private void FireErrorEvent()
  {
    this.Owner.QueueTask((Action) (() => this.FireSimpleEvent(EventNames.Error)));
  }
}

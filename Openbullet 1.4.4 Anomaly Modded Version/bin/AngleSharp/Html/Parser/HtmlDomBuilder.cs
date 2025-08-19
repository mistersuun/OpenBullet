// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Parser.HtmlDomBuilder
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Dom.Events;
using AngleSharp.Html.Parser.Tokens;
using AngleSharp.Io;
using AngleSharp.Mathml.Dom;
using AngleSharp.Svg.Dom;
using AngleSharp.Text;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace AngleSharp.Html.Parser;

internal sealed class HtmlDomBuilder
{
  private readonly HtmlTokenizer _tokenizer;
  private readonly HtmlDocument _document;
  private readonly List<Element> _openElements;
  private readonly List<Element> _formattingElements;
  private readonly Stack<HtmlTreeMode> _templateModes;
  private HtmlFormElement _currentFormElement;
  private HtmlTreeMode _currentMode;
  private HtmlTreeMode _previousMode;
  private HtmlParserOptions _options;
  private Element _fragmentContext;
  private bool _foster;
  private bool _frameset;
  private Task _waiting;

  public event EventHandler<HtmlErrorEvent> Error
  {
    add => this._tokenizer.Error += value;
    remove => this._tokenizer.Error -= value;
  }

  public HtmlDomBuilder(HtmlDocument document)
  {
    IBrowsingContext context = document.Context;
    this._tokenizer = new HtmlTokenizer(document.Source, document.Entities);
    this._document = document;
    this._openElements = new List<Element>();
    this._templateModes = new Stack<HtmlTreeMode>();
    this._formattingElements = new List<Element>();
    this._frameset = true;
    this._currentMode = HtmlTreeMode.Initial;
  }

  public bool IsFragmentCase => this._fragmentContext != null;

  public Element AdjustedCurrentNode
  {
    get
    {
      return this._fragmentContext == null || this._openElements.Count != 1 ? this.CurrentNode : this._fragmentContext;
    }
  }

  public Element CurrentNode
  {
    get
    {
      return this._openElements.Count <= 0 ? (Element) null : this._openElements[this._openElements.Count - 1];
    }
  }

  public async Task<HtmlDocument> ParseAsync(
    HtmlParserOptions options,
    CancellationToken cancelToken)
  {
    TextSource source = this._document.Source;
    HtmlToken token = (HtmlToken) null;
    this.SetOptions(options);
    do
    {
      ConfiguredTaskAwaitable configuredTaskAwaitable;
      if (source.Length - source.Index < 1024 /*0x0400*/)
      {
        configuredTaskAwaitable = source.PrefetchAsync(8192 /*0x2000*/, cancelToken).ConfigureAwait(false);
        await configuredTaskAwaitable;
      }
      token = this._tokenizer.Get();
      this.Consume(token);
      if (this._waiting != null)
      {
        configuredTaskAwaitable = this._waiting.ConfigureAwait(false);
        await configuredTaskAwaitable;
        this._waiting = (Task) null;
      }
    }
    while (token.Type != HtmlTokenType.EndOfFile);
    return this._document;
  }

  public HtmlDocument Parse(HtmlParserOptions options)
  {
    this.SetOptions(options);
    HtmlToken token;
    do
    {
      token = this._tokenizer.Get();
      this.Consume(token);
      this._waiting?.Wait();
      this._waiting = (Task) null;
    }
    while (token.Type != HtmlTokenType.EndOfFile);
    return this._document;
  }

  public HtmlDocument ParseFragment(HtmlParserOptions options, Element context)
  {
    context = context ?? throw new ArgumentNullException(nameof (context));
    this._fragmentContext = context;
    string localName = context.LocalName;
    if (localName.IsOneOf(TagNames.Title, TagNames.Textarea))
      this._tokenizer.State = HtmlParseMode.RCData;
    else if (localName.IsOneOf(TagNames.Style, TagNames.Xmp, TagNames.Iframe, TagNames.NoEmbed))
      this._tokenizer.State = HtmlParseMode.Rawtext;
    else if (localName.Is(TagNames.Script))
      this._tokenizer.State = HtmlParseMode.Script;
    else if (localName.Is(TagNames.Plaintext))
      this._tokenizer.State = HtmlParseMode.Plaintext;
    else if (localName.Is(TagNames.NoScript) && options.IsScripting)
      this._tokenizer.State = HtmlParseMode.Rawtext;
    else if (localName.Is(TagNames.NoFrames) && !options.IsNotSupportingFrames)
      this._tokenizer.State = HtmlParseMode.Rawtext;
    HtmlHtmlElement htmlHtmlElement = new HtmlHtmlElement((Document) this._document);
    this._document.AddNode((AngleSharp.Dom.Node) htmlHtmlElement);
    this._openElements.Add((Element) htmlHtmlElement);
    if (context is HtmlTemplateElement)
      this._templateModes.Push(HtmlTreeMode.InTemplate);
    this.Reset();
    this._tokenizer.IsAcceptingCharacterData = (this.AdjustedCurrentNode.Flags & NodeFlags.HtmlMember) != NodeFlags.HtmlMember;
    while (!(context is HtmlFormElement))
    {
      context = context.ParentElement as Element;
      if (context == null)
        goto label_20;
    }
    this._currentFormElement = (HtmlFormElement) context;
label_20:
    return this.Parse(options);
  }

  private void Restart()
  {
    this._currentMode = HtmlTreeMode.Initial;
    this._tokenizer.State = HtmlParseMode.PCData;
    this._document.Clear();
    this._frameset = true;
    this._openElements.Clear();
    this._formattingElements.Clear();
    this._templateModes.Clear();
  }

  private void Reset()
  {
    for (int index = this._openElements.Count - 1; index >= 0; --index)
    {
      Element element = this._openElements[index];
      bool isLast = index == 0;
      if (isLast && this._fragmentContext != null)
        element = this._fragmentContext;
      HtmlTreeMode? nullable = element.SelectMode(isLast, this._templateModes);
      if (nullable.HasValue)
      {
        this._currentMode = nullable.Value;
        break;
      }
    }
  }

  private void Consume(HtmlToken token)
  {
    Element adjustedCurrentNode = this.AdjustedCurrentNode;
    if (adjustedCurrentNode == null || token.Type == HtmlTokenType.EndOfFile || (adjustedCurrentNode.Flags & NodeFlags.HtmlMember) == NodeFlags.HtmlMember || (adjustedCurrentNode.Flags & NodeFlags.HtmlTip) == NodeFlags.HtmlTip && token.IsHtmlCompatible || (adjustedCurrentNode.Flags & NodeFlags.MathTip) == NodeFlags.MathTip && token.IsMathCompatible || (adjustedCurrentNode.Flags & NodeFlags.MathMember) == NodeFlags.MathMember && token.IsSvg && adjustedCurrentNode.LocalName.Is(TagNames.AnnotationXml))
      this.Home(token);
    else
      this.Foreign(token);
  }

  private void SetOptions(HtmlParserOptions options)
  {
    this._tokenizer.IsStrictMode = options.IsStrictMode;
    this._tokenizer.IsSupportingProcessingInstructions = options.IsSupportingProcessingInstructions;
    this._tokenizer.IsNotConsumingCharacterReferences = options.IsNotConsumingCharacterReferences;
    this._options = options;
  }

  private void Home(HtmlToken token)
  {
    switch (this._currentMode)
    {
      case HtmlTreeMode.Initial:
        this.Initial(token);
        break;
      case HtmlTreeMode.BeforeHtml:
        this.BeforeHtml(token);
        break;
      case HtmlTreeMode.BeforeHead:
        this.BeforeHead(token);
        break;
      case HtmlTreeMode.InHead:
        this.InHead(token);
        break;
      case HtmlTreeMode.InHeadNoScript:
        this.InHeadNoScript(token);
        break;
      case HtmlTreeMode.AfterHead:
        this.AfterHead(token);
        break;
      case HtmlTreeMode.InBody:
        this.InBody(token);
        break;
      case HtmlTreeMode.Text:
        this.Text(token);
        break;
      case HtmlTreeMode.InTable:
        this.InTable(token);
        break;
      case HtmlTreeMode.InCaption:
        this.InCaption(token);
        break;
      case HtmlTreeMode.InColumnGroup:
        this.InColumnGroup(token);
        break;
      case HtmlTreeMode.InTableBody:
        this.InTableBody(token);
        break;
      case HtmlTreeMode.InRow:
        this.InRow(token);
        break;
      case HtmlTreeMode.InCell:
        this.InCell(token);
        break;
      case HtmlTreeMode.InSelect:
        this.InSelect(token);
        break;
      case HtmlTreeMode.InSelectInTable:
        this.InSelectInTable(token);
        break;
      case HtmlTreeMode.InTemplate:
        this.InTemplate(token);
        break;
      case HtmlTreeMode.AfterBody:
        this.AfterBody(token);
        break;
      case HtmlTreeMode.InFrameset:
        this.InFrameset(token);
        break;
      case HtmlTreeMode.AfterFrameset:
        this.AfterFrameset(token);
        break;
      case HtmlTreeMode.AfterAfterBody:
        this.AfterAfterBody(token);
        break;
      case HtmlTreeMode.AfterAfterFrameset:
        this.AfterAfterFrameset(token);
        break;
    }
  }

  private void Initial(HtmlToken token)
  {
    switch (token.Type)
    {
      case HtmlTokenType.Doctype:
        HtmlDoctypeToken doctype = (HtmlDoctypeToken) token;
        if (!doctype.IsValid)
          this.RaiseErrorOccurred(HtmlParseError.DoctypeInvalid, token);
        this._document.AddNode((AngleSharp.Dom.Node) new DocumentType((Document) this._document, doctype.Name ?? string.Empty)
        {
          SystemIdentifier = doctype.SystemIdentifier,
          PublicIdentifier = doctype.PublicIdentifier
        });
        this._document.QuirksMode = doctype.GetQuirksMode();
        this._currentMode = HtmlTreeMode.BeforeHtml;
        return;
      case HtmlTokenType.Comment:
        this._document.AddComment(token);
        return;
      case HtmlTokenType.Character:
        token.TrimStart();
        if (token.IsEmpty)
          return;
        break;
    }
    if (!this._options.IsEmbedded)
    {
      this.RaiseErrorOccurred(HtmlParseError.DoctypeMissing, token);
      this._document.QuirksMode = QuirksMode.On;
    }
    this._currentMode = HtmlTreeMode.BeforeHtml;
    this.BeforeHtml(token);
  }

  private void BeforeHtml(HtmlToken token)
  {
    switch (token.Type)
    {
      case HtmlTokenType.Doctype:
        this.RaiseErrorOccurred(HtmlParseError.DoctypeTagInappropriate, token);
        return;
      case HtmlTokenType.StartTag:
        if (token.Name.Is(TagNames.Html))
        {
          this.AddRoot(token.AsTag());
          this._currentMode = HtmlTreeMode.BeforeHead;
          return;
        }
        break;
      case HtmlTokenType.EndTag:
        if (!TagNames.AllBeforeHead.Contains(token.Name))
        {
          this.RaiseErrorOccurred(HtmlParseError.TagCannotEndHere, token);
          return;
        }
        break;
      case HtmlTokenType.Comment:
        this._document.AddComment(token);
        return;
      case HtmlTokenType.Character:
        token.TrimStart();
        if (token.IsEmpty)
          return;
        break;
    }
    this.BeforeHtml((HtmlToken) HtmlTagToken.Open(TagNames.Html));
    this.BeforeHead(token);
  }

  private void BeforeHead(HtmlToken token)
  {
    switch (token.Type)
    {
      case HtmlTokenType.Doctype:
        this.RaiseErrorOccurred(HtmlParseError.DoctypeTagInappropriate, token);
        return;
      case HtmlTokenType.StartTag:
        string name = token.Name;
        if (name.Is(TagNames.Html))
        {
          this.InBody(token);
          return;
        }
        if (name.Is(TagNames.Head))
        {
          this.AddElement((Element) new HtmlHeadElement((Document) this._document), token.AsTag());
          this._currentMode = HtmlTreeMode.InHead;
          return;
        }
        break;
      case HtmlTokenType.EndTag:
        if (!TagNames.AllBeforeHead.Contains(token.Name))
        {
          this.RaiseErrorOccurred(HtmlParseError.TagCannotEndHere, token);
          return;
        }
        break;
      case HtmlTokenType.Comment:
        this.CurrentNode.AddComment(token);
        return;
      case HtmlTokenType.Character:
        token.TrimStart();
        if (token.IsEmpty)
          return;
        break;
    }
    this.BeforeHead((HtmlToken) HtmlTagToken.Open(TagNames.Head));
    this.InHead(token);
  }

  private void InHead(HtmlToken token)
  {
    switch (token.Type)
    {
      case HtmlTokenType.Doctype:
        this.RaiseErrorOccurred(HtmlParseError.DoctypeTagInappropriate, token);
        return;
      case HtmlTokenType.StartTag:
        string name1 = token.Name;
        if (name1.Is(TagNames.Html))
        {
          this.InBody(token);
          return;
        }
        if (name1.Is(TagNames.Meta))
        {
          HtmlMetaElement htmlMetaElement = new HtmlMetaElement((Document) this._document);
          this.AddElement((Element) htmlMetaElement, token.AsTag(), true);
          this.CloseCurrentNode();
          try
          {
            htmlMetaElement.Handle();
            return;
          }
          catch (NotSupportedException ex)
          {
            this.Restart();
            return;
          }
        }
        else
        {
          if (TagNames.AllHeadBase.Contains(name1))
          {
            this.AddElement(token.AsTag(), true);
            this.CloseCurrentNode();
            return;
          }
          if (name1.Is(TagNames.Title))
          {
            this.RCDataAlgorithm(token.AsTag());
            return;
          }
          if (name1.Is(TagNames.Style))
          {
            this.RawtextAlgorithm(token.AsTag());
            return;
          }
          if (this._options.IsScripting && name1.Is(TagNames.NoScript))
          {
            this.RawtextAlgorithm(token.AsTag());
            return;
          }
          if (name1.Is(TagNames.NoFrames))
          {
            if (this._options.IsNotSupportingFrames)
            {
              this.AddElement(token.AsTag());
              this._currentMode = HtmlTreeMode.InBody;
              return;
            }
            this.RawtextAlgorithm(token.AsTag());
            return;
          }
          if (name1.Is(TagNames.NoScript))
          {
            this.AddElement(token.AsTag());
            this._currentMode = HtmlTreeMode.InHeadNoScript;
            return;
          }
          if (name1.Is(TagNames.Script))
          {
            this.AddElement((Element) new HtmlScriptElement((Document) this._document, parserInserted: true, started: this.IsFragmentCase), token.AsTag());
            this._tokenizer.State = HtmlParseMode.Script;
            this._previousMode = this._currentMode;
            this._currentMode = HtmlTreeMode.Text;
            return;
          }
          if (name1.Is(TagNames.Head))
          {
            this.RaiseErrorOccurred(HtmlParseError.HeadTagMisplaced, token);
            return;
          }
          if (name1.Is(TagNames.Template))
          {
            this.AddElement((Element) new HtmlTemplateElement((Document) this._document), token.AsTag());
            this._formattingElements.AddScopeMarker();
            this._frameset = false;
            this._currentMode = HtmlTreeMode.InTemplate;
            this._templateModes.Push(HtmlTreeMode.InTemplate);
            return;
          }
          break;
        }
      case HtmlTokenType.EndTag:
        string name2 = token.Name;
        if (name2.Is(TagNames.Head))
        {
          this.CloseCurrentNode();
          this._currentMode = HtmlTreeMode.AfterHead;
          this._waiting = this._document.WaitForReadyAsync();
          return;
        }
        if (name2.Is(TagNames.Template))
        {
          if (this.TagCurrentlyOpen(TagNames.Template))
          {
            this.GenerateImpliedEndTags();
            if (!this.CurrentNode.LocalName.Is(TagNames.Template))
              this.RaiseErrorOccurred(HtmlParseError.TagClosingMismatch, token);
            this.CloseTemplate();
            return;
          }
          this.RaiseErrorOccurred(HtmlParseError.TagInappropriate, token);
          return;
        }
        if (!name2.IsOneOf(TagNames.Html, TagNames.Body, TagNames.Br))
        {
          this.RaiseErrorOccurred(HtmlParseError.TagCannotEndHere, token);
          return;
        }
        break;
      case HtmlTokenType.Comment:
        this.CurrentNode.AddComment(token);
        return;
      case HtmlTokenType.Character:
        this.AddCharacters(token.TrimStart());
        if (token.IsEmpty)
          return;
        break;
    }
    this.CloseCurrentNode();
    this._currentMode = HtmlTreeMode.AfterHead;
    this.AfterHead(token);
  }

  private void InHeadNoScript(HtmlToken token)
  {
    switch (token.Type)
    {
      case HtmlTokenType.Doctype:
        this.RaiseErrorOccurred(HtmlParseError.DoctypeTagInappropriate, token);
        return;
      case HtmlTokenType.StartTag:
        string name1 = token.Name;
        if (TagNames.AllNoScript.Contains(name1))
        {
          this.InHead(token);
          return;
        }
        if (name1.Is(TagNames.Html))
        {
          this.InBody(token);
          return;
        }
        if (name1.IsOneOf(TagNames.Head, TagNames.NoScript))
        {
          this.RaiseErrorOccurred(HtmlParseError.TagInappropriate, token);
          return;
        }
        break;
      case HtmlTokenType.EndTag:
        string name2 = token.Name;
        if (name2.Is(TagNames.NoScript))
        {
          this.CloseCurrentNode();
          this._currentMode = HtmlTreeMode.InHead;
          return;
        }
        if (!name2.Is(TagNames.Br))
        {
          this.RaiseErrorOccurred(HtmlParseError.TagCannotEndHere, token);
          return;
        }
        break;
      case HtmlTokenType.Comment:
        this.InHead(token);
        return;
      case HtmlTokenType.Character:
        this.AddCharacters(token.TrimStart());
        if (token.IsEmpty)
          return;
        break;
    }
    this.RaiseErrorOccurred(HtmlParseError.TokenNotPossible, token);
    this.CloseCurrentNode();
    this._currentMode = HtmlTreeMode.InHead;
    this.InHead(token);
  }

  private void AfterHead(HtmlToken token)
  {
    switch (token.Type)
    {
      case HtmlTokenType.Doctype:
        this.RaiseErrorOccurred(HtmlParseError.DoctypeTagInappropriate, token);
        return;
      case HtmlTokenType.StartTag:
        string name = token.Name;
        if (name.Is(TagNames.Html))
        {
          this.InBody(token);
          return;
        }
        if (name.Is(TagNames.Body))
        {
          this.AfterHeadStartTagBody(token.AsTag());
          return;
        }
        if (name.Is(TagNames.Frameset))
        {
          this.AddElement((Element) new HtmlFrameSetElement((Document) this._document), token.AsTag());
          this._currentMode = HtmlTreeMode.InFrameset;
          return;
        }
        if (TagNames.AllHeadNoTemplate.Contains(name))
        {
          this.RaiseErrorOccurred(HtmlParseError.TagMustBeInHead, token);
          int count = this._openElements.Count;
          Element head = this._document.Head as Element;
          this._openElements.Add(head);
          this.InHead(token);
          this.CloseNode(head);
          return;
        }
        if (name.Is(TagNames.Head))
        {
          this.RaiseErrorOccurred(HtmlParseError.HeadTagMisplaced, token);
          return;
        }
        break;
      case HtmlTokenType.EndTag:
        if (!token.Name.IsOneOf(TagNames.Html, TagNames.Body, TagNames.Br))
        {
          this.RaiseErrorOccurred(HtmlParseError.TagCannotEndHere, token);
          return;
        }
        break;
      case HtmlTokenType.Comment:
        this.CurrentNode.AddComment(token);
        return;
      case HtmlTokenType.Character:
        this.AddCharacters(token.TrimStart());
        if (token.IsEmpty)
          return;
        break;
    }
    this.AfterHeadStartTagBody(HtmlTagToken.Open(TagNames.Body));
    this._frameset = true;
    this.Home(token);
  }

  private void InBodyStartTag(HtmlTagToken tag)
  {
    string name = tag.Name;
    if (name.Is(TagNames.Div))
    {
      if (this.IsInButtonScope())
        this.InBodyEndTagParagraph((HtmlToken) tag);
      this.AddElement(tag);
    }
    else if (name.Is(TagNames.A))
    {
      for (int index = this._formattingElements.Count - 1; index >= 0 && this._formattingElements[index] != null; --index)
      {
        if (this._formattingElements[index].LocalName.Is(TagNames.A))
        {
          Element formattingElement = this._formattingElements[index];
          this.RaiseErrorOccurred(HtmlParseError.AnchorNested, (HtmlToken) tag);
          this.HeisenbergAlgorithm(HtmlTagToken.Close(TagNames.A));
          this.CloseNode(formattingElement);
          this._formattingElements.Remove(formattingElement);
          break;
        }
      }
      this.ReconstructFormatting();
      HtmlAnchorElement htmlAnchorElement = new HtmlAnchorElement((Document) this._document);
      this.AddElement((Element) htmlAnchorElement, tag);
      this._formattingElements.AddFormatting((Element) htmlAnchorElement);
    }
    else if (name.Is(TagNames.Span))
    {
      this.ReconstructFormatting();
      this.AddElement(tag);
    }
    else if (name.Is(TagNames.Li))
      this.InBodyStartTagListItem(tag);
    else if (name.Is(TagNames.Img))
      this.InBodyStartTagBreakrow(tag);
    else if (name.IsOneOf(TagNames.Ul, TagNames.P))
    {
      if (this.IsInButtonScope())
        this.InBodyEndTagParagraph((HtmlToken) tag);
      this.AddElement(tag);
    }
    else if (TagNames.AllSemanticFormatting.Contains(name))
    {
      this.ReconstructFormatting();
      this._formattingElements.AddFormatting(this.AddElement(tag));
    }
    else if (name.Is(TagNames.Script))
      this.InHead((HtmlToken) tag);
    else if (TagNames.AllHeadings.Contains(name))
    {
      if (this.IsInButtonScope())
        this.InBodyEndTagParagraph((HtmlToken) tag);
      if (TagNames.AllHeadings.Contains(this.CurrentNode.LocalName))
      {
        this.RaiseErrorOccurred(HtmlParseError.HeadingNested, (HtmlToken) tag);
        this.CloseCurrentNode();
      }
      this.AddElement((Element) new HtmlHeadingElement((Document) this._document, name), tag);
    }
    else if (name.Is(TagNames.Input))
    {
      this.ReconstructFormatting();
      this.AddElement((Element) new HtmlInputElement((Document) this._document), tag, true);
      this.CloseCurrentNode();
      if (tag.GetAttribute(AttributeNames.Type).Isi(AttributeNames.Hidden))
        return;
      this._frameset = false;
    }
    else if (name.Is(TagNames.Form))
    {
      if (this._currentFormElement == null)
      {
        if (this.IsInButtonScope())
          this.InBodyEndTagParagraph((HtmlToken) tag);
        this._currentFormElement = new HtmlFormElement((Document) this._document);
        this.AddElement((Element) this._currentFormElement, tag);
      }
      else
        this.RaiseErrorOccurred(HtmlParseError.FormAlreadyOpen, (HtmlToken) tag);
    }
    else if (TagNames.AllBody.Contains(name))
    {
      if (this.IsInButtonScope())
        this.InBodyEndTagParagraph((HtmlToken) tag);
      this.AddElement(tag);
    }
    else if (TagNames.AllClassicFormatting.Contains(name))
    {
      this.ReconstructFormatting();
      this._formattingElements.AddFormatting(this.AddElement(tag));
    }
    else if (TagNames.AllHead.Contains(name))
      this.InHead((HtmlToken) tag);
    else if (name.IsOneOf(TagNames.Pre, TagNames.Listing))
    {
      if (this.IsInButtonScope())
        this.InBodyEndTagParagraph((HtmlToken) tag);
      this.AddElement(tag);
      this._frameset = false;
      this.PreventNewLine();
    }
    else if (name.Is(TagNames.Button))
    {
      if (this.IsInScope(TagNames.Button))
      {
        this.RaiseErrorOccurred(HtmlParseError.ButtonInScope, (HtmlToken) tag);
        this.InBodyEndTagBlock(tag);
        this.InBody((HtmlToken) tag);
      }
      else
      {
        this.ReconstructFormatting();
        this.AddElement((Element) new HtmlButtonElement((Document) this._document), tag);
        this._frameset = false;
      }
    }
    else if (name.Is(TagNames.Table))
    {
      if (this._document.QuirksMode != QuirksMode.On && this.IsInButtonScope())
        this.InBodyEndTagParagraph((HtmlToken) tag);
      this.AddElement((Element) new HtmlTableElement((Document) this._document), tag);
      this._frameset = false;
      this._currentMode = HtmlTreeMode.InTable;
    }
    else if (TagNames.AllBodyBreakrow.Contains(name))
      this.InBodyStartTagBreakrow(tag);
    else if (TagNames.AllBodyClosed.Contains(name))
    {
      this.AddElement(tag, true);
      this.CloseCurrentNode();
    }
    else if (name.Is(TagNames.Hr))
    {
      if (this.IsInButtonScope())
        this.InBodyEndTagParagraph((HtmlToken) tag);
      this.AddElement((Element) new HtmlHrElement((Document) this._document), tag, true);
      this.CloseCurrentNode();
      this._frameset = false;
    }
    else if (name.Is(TagNames.Textarea))
    {
      this.AddElement((Element) new HtmlTextAreaElement((Document) this._document), tag);
      this._tokenizer.State = HtmlParseMode.RCData;
      this._previousMode = this._currentMode;
      this._frameset = false;
      this._currentMode = HtmlTreeMode.Text;
      this.PreventNewLine();
    }
    else if (name.Is(TagNames.Select))
    {
      this.ReconstructFormatting();
      this.AddElement((Element) new HtmlSelectElement((Document) this._document), tag);
      this._frameset = false;
      switch (this._currentMode)
      {
        case HtmlTreeMode.InTable:
        case HtmlTreeMode.InCaption:
        case HtmlTreeMode.InTableBody:
        case HtmlTreeMode.InRow:
        case HtmlTreeMode.InCell:
          this._currentMode = HtmlTreeMode.InSelectInTable;
          break;
        default:
          this._currentMode = HtmlTreeMode.InSelect;
          break;
      }
    }
    else if (name.IsOneOf(TagNames.Optgroup, TagNames.Option))
    {
      if (this.CurrentNode.LocalName.Is(TagNames.Option))
        this.InBodyEndTagAnythingElse(HtmlTagToken.Close(TagNames.Option));
      this.ReconstructFormatting();
      this.AddElement(tag);
    }
    else if (name.IsOneOf(TagNames.Dd, TagNames.Dt))
      this.InBodyStartTagDefinitionItem(tag);
    else if (name.Is(TagNames.Iframe))
    {
      this._frameset = false;
      this.RawtextAlgorithm(tag);
    }
    else if (TagNames.AllBodyObsolete.Contains(name))
    {
      this.ReconstructFormatting();
      this.AddElement(tag);
      this._formattingElements.AddScopeMarker();
      this._frameset = false;
    }
    else if (name.Is(TagNames.Image))
    {
      this.RaiseErrorOccurred(HtmlParseError.ImageTagNamedWrong, (HtmlToken) tag);
      tag.Name = TagNames.Img;
      this.InBodyStartTagBreakrow(tag);
    }
    else if (name.Is(TagNames.NoBr))
    {
      this.ReconstructFormatting();
      if (this.IsInScope(TagNames.NoBr))
      {
        this.RaiseErrorOccurred(HtmlParseError.NobrInScope, (HtmlToken) tag);
        this.HeisenbergAlgorithm(tag);
        this.ReconstructFormatting();
      }
      this._formattingElements.AddFormatting(this.AddElement(tag));
    }
    else if (name.Is(TagNames.Xmp))
    {
      if (this.IsInButtonScope())
        this.InBodyEndTagParagraph((HtmlToken) tag);
      this.ReconstructFormatting();
      this._frameset = false;
      this.RawtextAlgorithm(tag);
    }
    else if (name.IsOneOf(TagNames.Rb, TagNames.Rtc))
    {
      if (this.IsInScope(TagNames.Ruby))
      {
        this.GenerateImpliedEndTags();
        if (!this.CurrentNode.LocalName.Is(TagNames.Ruby))
          this.RaiseErrorOccurred(HtmlParseError.TagDoesNotMatchCurrentNode, (HtmlToken) tag);
      }
      this.AddElement(tag);
    }
    else if (name.IsOneOf(TagNames.Rp, TagNames.Rt))
    {
      if (this.IsInScope(TagNames.Ruby))
      {
        this.GenerateImpliedEndTagsExceptFor(TagNames.Rtc);
        if (!this.CurrentNode.LocalName.IsOneOf(TagNames.Ruby, TagNames.Rtc))
          this.RaiseErrorOccurred(HtmlParseError.TagDoesNotMatchCurrentNode, (HtmlToken) tag);
      }
      this.AddElement(tag);
    }
    else if (name.Is(TagNames.NoEmbed))
      this.RawtextAlgorithm(tag);
    else if (name.Is(TagNames.NoScript))
    {
      if (this._options.IsScripting)
      {
        this.RawtextAlgorithm(tag);
      }
      else
      {
        this.ReconstructFormatting();
        this.AddElement(tag);
      }
    }
    else if (name.Is(TagNames.Math))
    {
      MathElement element = new MathElement((Document) this._document, name);
      this.ReconstructFormatting();
      this.AddElement((Element) element.Setup(tag));
      if (!tag.IsSelfClosing)
        return;
      this.CloseNode((Element) element);
    }
    else if (name.Is(TagNames.Svg))
    {
      SvgElement element = new SvgElement((Document) this._document, name);
      this.ReconstructFormatting();
      this.AddElement((Element) element.Setup(tag));
      if (!tag.IsSelfClosing)
        return;
      this.CloseNode((Element) element);
    }
    else if (name.Is(TagNames.Plaintext))
    {
      if (this.IsInButtonScope())
        this.InBodyEndTagParagraph((HtmlToken) tag);
      this.AddElement(tag);
      this._tokenizer.State = HtmlParseMode.Plaintext;
    }
    else if (name.Is(TagNames.Frameset))
    {
      this.RaiseErrorOccurred(HtmlParseError.FramesetMisplaced, (HtmlToken) tag);
      if (this._openElements.Count == 1 || !this._openElements[1].LocalName.Is(TagNames.Body) || !this._frameset)
        return;
      this._openElements[1].RemoveFromParent();
      while (this._openElements.Count > 1)
        this.CloseCurrentNode();
      this.AddElement((Element) new HtmlFrameSetElement((Document) this._document), tag);
      this._currentMode = HtmlTreeMode.InFrameset;
    }
    else if (name.Is(TagNames.Html))
    {
      this.RaiseErrorOccurred(HtmlParseError.HtmlTagMisplaced, (HtmlToken) tag);
      if (this._templateModes.Count != 0)
        return;
      this._openElements[0].SetUniqueAttributes(tag.Attributes);
    }
    else if (name.Is(TagNames.Body))
    {
      this.RaiseErrorOccurred(HtmlParseError.BodyTagMisplaced, (HtmlToken) tag);
      if (this._templateModes.Count != 0 || this._openElements.Count <= 1 || !this._openElements[1].LocalName.Is(TagNames.Body))
        return;
      this._frameset = false;
      this._openElements[1].SetUniqueAttributes(tag.Attributes);
    }
    else if (name.Is(TagNames.IsIndex))
    {
      this.RaiseErrorOccurred(HtmlParseError.TagInappropriate, (HtmlToken) tag);
      if (this._currentFormElement != null)
        return;
      this.InBody((HtmlToken) HtmlTagToken.Open(TagNames.Form));
      if (tag.GetAttribute(AttributeNames.Action).Length > 0)
        this._currentFormElement.SetAttribute(AttributeNames.Action, tag.GetAttribute(AttributeNames.Action));
      this.InBody((HtmlToken) HtmlTagToken.Open(TagNames.Hr));
      this.InBody((HtmlToken) HtmlTagToken.Open(TagNames.Label));
      if (tag.GetAttribute(AttributeNames.Prompt).Length > 0)
        this.AddCharacters(tag.GetAttribute(AttributeNames.Prompt));
      else
        this.AddCharacters("This is a searchable index. Enter search keywords: ");
      HtmlTagToken token = HtmlTagToken.Open(TagNames.Input);
      token.AddAttribute(AttributeNames.Name, TagNames.IsIndex);
      for (int index = 0; index < tag.Attributes.Count; ++index)
      {
        HtmlAttributeToken attribute = tag.Attributes[index];
        if (!attribute.Name.IsOneOf(AttributeNames.Name, AttributeNames.Action, AttributeNames.Prompt))
          token.AddAttribute(attribute.Name, attribute.Value);
      }
      this.InBody((HtmlToken) token);
      this.InBody((HtmlToken) HtmlTagToken.Close(TagNames.Label));
      this.InBody((HtmlToken) HtmlTagToken.Open(TagNames.Hr));
      this.InBody((HtmlToken) HtmlTagToken.Close(TagNames.Form));
    }
    else if (TagNames.AllNested.Contains(name))
    {
      this.RaiseErrorOccurred(HtmlParseError.TagCannotStartHere, (HtmlToken) tag);
    }
    else
    {
      this.ReconstructFormatting();
      this.AddElement(tag);
    }
  }

  private void InBodyEndTag(HtmlTagToken tag)
  {
    string name = tag.Name;
    if (name.Is(TagNames.Div))
      this.InBodyEndTagBlock(tag);
    else if (name.Is(TagNames.A))
      this.HeisenbergAlgorithm(tag);
    else if (name.Is(TagNames.Li))
    {
      if (this.IsInListItemScope())
      {
        this.GenerateImpliedEndTagsExceptFor(name);
        if (!this.CurrentNode.LocalName.Is(TagNames.Li))
          this.RaiseErrorOccurred(HtmlParseError.TagDoesNotMatchCurrentNode, (HtmlToken) tag);
        this.ClearStackBackTo(TagNames.Li);
        this.CloseCurrentNode();
      }
      else
        this.RaiseErrorOccurred(HtmlParseError.ListItemNotInScope, (HtmlToken) tag);
    }
    else if (name.Is(TagNames.P))
      this.InBodyEndTagParagraph((HtmlToken) tag);
    else if (TagNames.AllBlocks.Contains(name))
      this.InBodyEndTagBlock(tag);
    else if (TagNames.AllFormatting.Contains(name))
      this.HeisenbergAlgorithm(tag);
    else if (name.Is(TagNames.Form))
    {
      HtmlFormElement currentFormElement = this._currentFormElement;
      this._currentFormElement = (HtmlFormElement) null;
      if (currentFormElement != null && this.IsInScope(currentFormElement.LocalName))
      {
        this.GenerateImpliedEndTags();
        if (this.CurrentNode != currentFormElement)
          this.RaiseErrorOccurred(HtmlParseError.FormClosedWrong, (HtmlToken) tag);
        this.CloseNode((Element) currentFormElement);
      }
      else
        this.RaiseErrorOccurred(HtmlParseError.FormNotInScope, (HtmlToken) tag);
    }
    else if (name.Is(TagNames.Br))
    {
      this.RaiseErrorOccurred(HtmlParseError.TagCannotEndHere, (HtmlToken) tag);
      this.InBodyStartTagBreakrow(HtmlTagToken.Open(TagNames.Br));
    }
    else if (TagNames.AllHeadings.Contains(name))
    {
      if (this.IsInScope(TagNames.AllHeadings))
      {
        this.GenerateImpliedEndTags();
        if (!this.CurrentNode.LocalName.Is(name))
          this.RaiseErrorOccurred(HtmlParseError.TagDoesNotMatchCurrentNode, (HtmlToken) tag);
        this.ClearStackBackTo(TagNames.AllHeadings);
        this.CloseCurrentNode();
      }
      else
        this.RaiseErrorOccurred(HtmlParseError.HeadingNotInScope, (HtmlToken) tag);
    }
    else if (name.IsOneOf(TagNames.Dd, TagNames.Dt))
    {
      if (this.IsInScope(name))
      {
        this.GenerateImpliedEndTagsExceptFor(name);
        if (!this.CurrentNode.LocalName.Is(name))
          this.RaiseErrorOccurred(HtmlParseError.TagDoesNotMatchCurrentNode, (HtmlToken) tag);
        this.ClearStackBackTo(name);
        this.CloseCurrentNode();
      }
      else
        this.RaiseErrorOccurred(HtmlParseError.ListItemNotInScope, (HtmlToken) tag);
    }
    else if (name.IsOneOf(TagNames.Applet, TagNames.Marquee, TagNames.Object))
    {
      if (this.IsInScope(name))
      {
        this.GenerateImpliedEndTags();
        if (!this.CurrentNode.LocalName.Is(name))
          this.RaiseErrorOccurred(HtmlParseError.TagDoesNotMatchCurrentNode, (HtmlToken) tag);
        this.ClearStackBackTo(name);
        this.CloseCurrentNode();
        this._formattingElements.ClearFormatting();
      }
      else
        this.RaiseErrorOccurred(HtmlParseError.ObjectNotInScope, (HtmlToken) tag);
    }
    else if (name.Is(TagNames.Body))
      this.InBodyEndTagBody((HtmlToken) tag);
    else if (name.Is(TagNames.Html))
    {
      if (!this.InBodyEndTagBody((HtmlToken) tag))
        return;
      this.AfterBody((HtmlToken) tag);
    }
    else if (name.Is(TagNames.Template))
      this.InHead((HtmlToken) tag);
    else
      this.InBodyEndTagAnythingElse(tag);
  }

  private void InBody(HtmlToken token)
  {
    switch (token.Type)
    {
      case HtmlTokenType.Doctype:
        this.RaiseErrorOccurred(HtmlParseError.DoctypeTagInappropriate, token);
        break;
      case HtmlTokenType.StartTag:
        this.InBodyStartTag(token.AsTag());
        break;
      case HtmlTokenType.EndTag:
        this.InBodyEndTag(token.AsTag());
        break;
      case HtmlTokenType.Comment:
        this.CurrentNode.AddComment(token);
        break;
      case HtmlTokenType.Character:
        this.ReconstructFormatting();
        this.AddCharacters(token.Data);
        this._frameset = !token.HasContent && this._frameset;
        break;
      case HtmlTokenType.EndOfFile:
        this.CheckBodyOnClosing(token);
        if (this._templateModes.Count != 0)
        {
          this.InTemplate(token);
          break;
        }
        this.End();
        break;
    }
  }

  private void Text(HtmlToken token)
  {
    switch (token.Type)
    {
      case HtmlTokenType.EndTag:
        if (!token.Name.Is(TagNames.Script))
        {
          this.CloseCurrentNode();
          this._currentMode = this._previousMode;
          break;
        }
        this.HandleScript(this.CurrentNode as HtmlScriptElement);
        break;
      case HtmlTokenType.Character:
        this.AddCharacters(token.Data);
        break;
      case HtmlTokenType.EndOfFile:
        this.RaiseErrorOccurred(HtmlParseError.EOF, token);
        this.CloseCurrentNode();
        this._currentMode = this._previousMode;
        this.Consume(token);
        break;
    }
  }

  private void InTable(HtmlToken token)
  {
    switch (token.Type)
    {
      case HtmlTokenType.Doctype:
        this.RaiseErrorOccurred(HtmlParseError.DoctypeTagInappropriate, token);
        return;
      case HtmlTokenType.StartTag:
        string name1 = token.Name;
        if (name1.Is(TagNames.Caption))
        {
          this.ClearStackBackTo(TagNames.Table);
          this._formattingElements.AddScopeMarker();
          this.AddElement((Element) new HtmlTableCaptionElement((Document) this._document), token.AsTag());
          this._currentMode = HtmlTreeMode.InCaption;
          return;
        }
        if (name1.Is(TagNames.Colgroup))
        {
          this.ClearStackBackTo(TagNames.Table);
          this.AddElement((Element) new HtmlTableColgroupElement((Document) this._document), token.AsTag());
          this._currentMode = HtmlTreeMode.InColumnGroup;
          return;
        }
        if (name1.Is(TagNames.Col))
        {
          this.InTable((HtmlToken) HtmlTagToken.Open(TagNames.Colgroup));
          this.InColumnGroup(token);
          return;
        }
        if (TagNames.AllTableSections.Contains(name1))
        {
          this.ClearStackBackTo(TagNames.Table);
          this.AddElement((Element) new HtmlTableSectionElement((Document) this._document, name1), token.AsTag());
          this._currentMode = HtmlTreeMode.InTableBody;
          return;
        }
        if (TagNames.AllTableCellsRows.Contains(name1))
        {
          this.InTable((HtmlToken) HtmlTagToken.Open(TagNames.Tbody));
          this.InTableBody(token);
          return;
        }
        if (name1.Is(TagNames.Table))
        {
          this.RaiseErrorOccurred(HtmlParseError.TableNesting, token);
          if (!this.InTableEndTagTable(token))
            return;
          this.Home(token);
          return;
        }
        if (name1.Is(TagNames.Input))
        {
          HtmlTagToken tag = token.AsTag();
          if (tag.GetAttribute(AttributeNames.Type).Isi(AttributeNames.Hidden))
          {
            this.RaiseErrorOccurred(HtmlParseError.InputUnexpected, token);
            this.AddElement((Element) new HtmlInputElement((Document) this._document), tag, true);
            this.CloseCurrentNode();
            return;
          }
          this.RaiseErrorOccurred(HtmlParseError.TokenNotPossible, token);
          this.InBodyWithFoster(token);
          return;
        }
        if (name1.Is(TagNames.Form))
        {
          this.RaiseErrorOccurred(HtmlParseError.FormInappropriate, token);
          if (this._currentFormElement != null)
            return;
          this._currentFormElement = new HtmlFormElement((Document) this._document);
          this.AddElement((Element) this._currentFormElement, token.AsTag());
          this.CloseCurrentNode();
          return;
        }
        if (TagNames.AllTableHead.Contains(name1))
        {
          this.InHead(token);
          return;
        }
        this.RaiseErrorOccurred(HtmlParseError.IllegalElementInTableDetected, token);
        this.InBodyWithFoster(token);
        return;
      case HtmlTokenType.EndTag:
        string name2 = token.Name;
        if (name2.Is(TagNames.Table))
        {
          this.InTableEndTagTable(token);
          return;
        }
        if (name2.Is(TagNames.Template))
        {
          this.InHead(token);
          return;
        }
        if (TagNames.AllTableSpecial.Contains(name2) || TagNames.AllTableInner.Contains(name2))
        {
          this.RaiseErrorOccurred(HtmlParseError.TagCannotEndHere, token);
          return;
        }
        this.RaiseErrorOccurred(HtmlParseError.IllegalElementInTableDetected, token);
        this.InBodyWithFoster(token);
        return;
      case HtmlTokenType.Comment:
        this.CurrentNode.AddComment(token);
        return;
      case HtmlTokenType.Character:
        if (TagNames.AllTableMajor.Contains(this.CurrentNode.LocalName))
        {
          this.InTableText(token);
          return;
        }
        break;
      case HtmlTokenType.EndOfFile:
        this.InBody(token);
        return;
    }
    this.RaiseErrorOccurred(HtmlParseError.TokenNotPossible, token);
    this.InBodyWithFoster(token);
  }

  private void InTableText(HtmlToken token)
  {
    if (token.HasContent)
    {
      this.RaiseErrorOccurred(HtmlParseError.TokenNotPossible, token);
      this.InBodyWithFoster(token);
    }
    else
      this.AddCharacters(token.Data);
  }

  private void InCaption(HtmlToken token)
  {
    switch (token.Type)
    {
      case HtmlTokenType.StartTag:
        string name1 = token.Name;
        if (TagNames.AllCaptionEnd.Contains(name1))
        {
          this.RaiseErrorOccurred(HtmlParseError.TagCannotStartHere, token);
          if (!this.InCaptionEndTagCaption(token))
            return;
          this.InTable(token);
          return;
        }
        break;
      case HtmlTokenType.EndTag:
        string name2 = token.Name;
        if (name2.Is(TagNames.Caption))
        {
          this.InCaptionEndTagCaption(token);
          return;
        }
        if (TagNames.AllCaptionStart.Contains(name2))
        {
          this.RaiseErrorOccurred(HtmlParseError.TagCannotEndHere, token);
          return;
        }
        if (name2.Is(TagNames.Table))
        {
          this.RaiseErrorOccurred(HtmlParseError.TableNesting, token);
          if (!this.InCaptionEndTagCaption(token))
            return;
          this.InTable(token);
          return;
        }
        break;
    }
    this.InBody(token);
  }

  private void InColumnGroup(HtmlToken token)
  {
    switch (token.Type)
    {
      case HtmlTokenType.Doctype:
        this.RaiseErrorOccurred(HtmlParseError.DoctypeTagInappropriate, token);
        return;
      case HtmlTokenType.StartTag:
        string name1 = token.Name;
        if (name1.Is(TagNames.Html))
        {
          this.InBody(token);
          return;
        }
        if (name1.Is(TagNames.Col))
        {
          this.AddElement((Element) new HtmlTableColElement((Document) this._document), token.AsTag(), true);
          this.CloseCurrentNode();
          return;
        }
        if (name1.Is(TagNames.Template))
        {
          this.InHead(token);
          return;
        }
        break;
      case HtmlTokenType.EndTag:
        string name2 = token.Name;
        if (name2.Is(TagNames.Colgroup))
        {
          this.InColumnGroupEndTagColgroup(token);
          return;
        }
        if (name2.Is(TagNames.Col))
        {
          this.RaiseErrorOccurred(HtmlParseError.TagClosedWrong, token);
          return;
        }
        if (name2.Is(TagNames.Template))
        {
          this.InHead(token);
          return;
        }
        break;
      case HtmlTokenType.Comment:
        this.CurrentNode.AddComment(token);
        return;
      case HtmlTokenType.Character:
        this.AddCharacters(token.TrimStart());
        if (token.IsEmpty)
          return;
        break;
      case HtmlTokenType.EndOfFile:
        this.InBody(token);
        return;
    }
    if (!this.InColumnGroupEndTagColgroup(token))
      return;
    this.InTable(token);
  }

  private void InTableBody(HtmlToken token)
  {
    switch (token.Type)
    {
      case HtmlTokenType.StartTag:
        string name1 = token.Name;
        if (name1.Is(TagNames.Tr))
        {
          this.ClearStackBackTo(TagNames.AllTableSections);
          this.AddElement((Element) new HtmlTableRowElement((Document) this._document), token.AsTag());
          this._currentMode = HtmlTreeMode.InRow;
          return;
        }
        if (TagNames.AllTableCells.Contains(name1))
        {
          this.InTableBody((HtmlToken) HtmlTagToken.Open(TagNames.Tr));
          this.InRow(token);
          return;
        }
        if (TagNames.AllTableGeneral.Contains(name1))
        {
          this.InTableBodyCloseTable(token.AsTag());
          return;
        }
        break;
      case HtmlTokenType.EndTag:
        string name2 = token.Name;
        if (TagNames.AllTableSections.Contains(name2))
        {
          if (this.IsInTableScope(name2))
          {
            this.ClearStackBackTo(TagNames.AllTableSections);
            this.CloseCurrentNode();
            this._currentMode = HtmlTreeMode.InTable;
            return;
          }
          this.RaiseErrorOccurred(HtmlParseError.TableSectionNotInScope, token);
          return;
        }
        if (name2.Is(TagNames.Tr) || TagNames.AllTableSpecial.Contains(name2))
        {
          this.RaiseErrorOccurred(HtmlParseError.TagCannotEndHere, token);
          return;
        }
        if (name2.Is(TagNames.Table))
        {
          this.InTableBodyCloseTable(token.AsTag());
          return;
        }
        break;
    }
    this.InTable(token);
  }

  private void InRow(HtmlToken token)
  {
    switch (token.Type)
    {
      case HtmlTokenType.StartTag:
        string name1 = token.Name;
        if (TagNames.AllTableCells.Contains(name1))
        {
          this.ClearStackBackTo(TagNames.Tr);
          this.AddElement(token.AsTag());
          this._currentMode = HtmlTreeMode.InCell;
          this._formattingElements.AddScopeMarker();
          return;
        }
        if (name1.Is(TagNames.Tr) || TagNames.AllTableGeneral.Contains(name1))
        {
          if (!this.InRowEndTagTablerow(token))
            return;
          this.InTableBody(token);
          return;
        }
        break;
      case HtmlTokenType.EndTag:
        string name2 = token.Name;
        if (name2.Is(TagNames.Tr))
        {
          this.InRowEndTagTablerow(token);
          return;
        }
        if (name2.Is(TagNames.Table))
        {
          if (!this.InRowEndTagTablerow(token))
            return;
          this.InTableBody(token);
          return;
        }
        if (TagNames.AllTableSections.Contains(name2))
        {
          if (this.IsInTableScope(name2))
          {
            this.InRowEndTagTablerow(token);
            this.InTableBody(token);
            return;
          }
          this.RaiseErrorOccurred(HtmlParseError.TableSectionNotInScope, token);
          return;
        }
        if (TagNames.AllTableSpecial.Contains(name2))
        {
          this.RaiseErrorOccurred(HtmlParseError.TagCannotEndHere, token);
          return;
        }
        break;
    }
    this.InTable(token);
  }

  private void InCell(HtmlToken token)
  {
    switch (token.Type)
    {
      case HtmlTokenType.StartTag:
        string name1 = token.Name;
        if (TagNames.AllTableCellsRows.Contains(name1) || TagNames.AllTableGeneral.Contains(name1))
        {
          if (this.IsInTableScope(TagNames.AllTableCells))
          {
            this.InCellEndTagCell(token);
            this.Home(token);
            return;
          }
          this.RaiseErrorOccurred(HtmlParseError.TableCellNotInScope, token);
          return;
        }
        break;
      case HtmlTokenType.EndTag:
        string name2 = token.Name;
        if (TagNames.AllTableCells.Contains(name2))
        {
          this.InCellEndTagCell(token);
          return;
        }
        if (TagNames.AllTableCore.Contains(name2))
        {
          if (this.IsInTableScope(name2))
          {
            this.InCellEndTagCell(token);
            this.Home(token);
            return;
          }
          this.RaiseErrorOccurred(HtmlParseError.TableNotInScope, token);
          return;
        }
        if (!TagNames.AllTableSpecial.Contains(name2))
        {
          this.InBody(token);
          return;
        }
        this.RaiseErrorOccurred(HtmlParseError.TagCannotEndHere, token);
        return;
    }
    this.InBody(token);
  }

  private void InSelect(HtmlToken token)
  {
    switch (token.Type)
    {
      case HtmlTokenType.Doctype:
        this.RaiseErrorOccurred(HtmlParseError.DoctypeTagInappropriate, token);
        break;
      case HtmlTokenType.StartTag:
        string name1 = token.Name;
        if (name1.Is(TagNames.Html))
        {
          this.InBody(token);
          break;
        }
        if (name1.Is(TagNames.Option))
        {
          if (this.CurrentNode.LocalName.Is(TagNames.Option))
            this.InSelectEndTagOption(token);
          this.AddElement((Element) new HtmlOptionElement((Document) this._document), token.AsTag());
          break;
        }
        if (name1.Is(TagNames.Optgroup))
        {
          if (this.CurrentNode.LocalName.Is(TagNames.Option))
            this.InSelectEndTagOption(token);
          if (this.CurrentNode.LocalName.Is(TagNames.Optgroup))
            this.InSelectEndTagOptgroup(token);
          this.AddElement((Element) new HtmlOptionsGroupElement((Document) this._document), token.AsTag());
          break;
        }
        if (name1.Is(TagNames.Select))
        {
          this.RaiseErrorOccurred(HtmlParseError.SelectNesting, token);
          this.InSelectEndTagSelect();
          break;
        }
        if (TagNames.AllInput.Contains(name1))
        {
          this.RaiseErrorOccurred(HtmlParseError.IllegalElementInSelectDetected, token);
          if (!this.IsInSelectScope(TagNames.Select))
            break;
          this.InSelectEndTagSelect();
          this.Home(token);
          break;
        }
        if (name1.IsOneOf(TagNames.Template, TagNames.Script))
        {
          this.InHead(token);
          break;
        }
        this.RaiseErrorOccurred(HtmlParseError.IllegalElementInSelectDetected, token);
        break;
      case HtmlTokenType.EndTag:
        string name2 = token.Name;
        if (name2.Is(TagNames.Template))
        {
          this.InHead(token);
          break;
        }
        if (name2.Is(TagNames.Optgroup))
        {
          this.InSelectEndTagOptgroup(token);
          break;
        }
        if (name2.Is(TagNames.Option))
        {
          this.InSelectEndTagOption(token);
          break;
        }
        if (name2.Is(TagNames.Select) && this.IsInSelectScope(TagNames.Select))
        {
          this.InSelectEndTagSelect();
          break;
        }
        if (name2.Is(TagNames.Select))
        {
          this.RaiseErrorOccurred(HtmlParseError.SelectNotInScope, token);
          break;
        }
        this.RaiseErrorOccurred(HtmlParseError.TagCannotEndHere, token);
        break;
      case HtmlTokenType.Comment:
        this.CurrentNode.AddComment(token);
        break;
      case HtmlTokenType.Character:
        this.AddCharacters(token.Data);
        break;
      case HtmlTokenType.EndOfFile:
        this.InBody(token);
        break;
      default:
        this.RaiseErrorOccurred(HtmlParseError.TokenNotPossible, token);
        break;
    }
  }

  private void InSelectInTable(HtmlToken token)
  {
    switch (token.Type)
    {
      case HtmlTokenType.StartTag:
        string name1 = token.Name;
        if (TagNames.AllTableSelects.Contains(name1))
        {
          this.RaiseErrorOccurred(HtmlParseError.IllegalElementInSelectDetected, token);
          this.InSelectEndTagSelect();
          this.Home(token);
          return;
        }
        break;
      case HtmlTokenType.EndTag:
        string name2 = token.Name;
        if (TagNames.AllTableSelects.Contains(name2))
        {
          this.RaiseErrorOccurred(HtmlParseError.TagCannotEndHere, token);
          if (!this.IsInTableScope(name2))
            return;
          this.InSelectEndTagSelect();
          this.Home(token);
          return;
        }
        break;
    }
    this.InSelect(token);
  }

  private void InTemplate(HtmlToken token)
  {
    switch (token.Type)
    {
      case HtmlTokenType.StartTag:
        string name = token.Name;
        if (name.Is(TagNames.Script) || TagNames.AllHead.Contains(name))
        {
          this.InHead(token);
          break;
        }
        if (TagNames.AllTableRoot.Contains(name))
        {
          this.TemplateStep(token, HtmlTreeMode.InTable);
          break;
        }
        if (name.Is(TagNames.Col))
        {
          this.TemplateStep(token, HtmlTreeMode.InColumnGroup);
          break;
        }
        if (name.Is(TagNames.Tr))
        {
          this.TemplateStep(token, HtmlTreeMode.InTableBody);
          break;
        }
        if (TagNames.AllTableCells.Contains(name))
        {
          this.TemplateStep(token, HtmlTreeMode.InRow);
          break;
        }
        this.TemplateStep(token, HtmlTreeMode.InBody);
        break;
      case HtmlTokenType.EndTag:
        if (token.Name.Is(TagNames.Template))
        {
          this.InHead(token);
          break;
        }
        this.RaiseErrorOccurred(HtmlParseError.TagCannotEndHere, token);
        break;
      case HtmlTokenType.EndOfFile:
        if (this.TagCurrentlyOpen(TagNames.Template))
        {
          this.RaiseErrorOccurred(HtmlParseError.EOF, token);
          this.CloseTemplate();
          this.Home(token);
          break;
        }
        this.End();
        break;
      default:
        this.InBody(token);
        break;
    }
  }

  private void AfterBody(HtmlToken token)
  {
    switch (token.Type)
    {
      case HtmlTokenType.Doctype:
        this.RaiseErrorOccurred(HtmlParseError.DoctypeTagInappropriate, token);
        return;
      case HtmlTokenType.StartTag:
        if (token.Name.Is(TagNames.Html))
        {
          this.InBody(token);
          return;
        }
        break;
      case HtmlTokenType.EndTag:
        if (token.Name.Is(TagNames.Html))
        {
          if (this.IsFragmentCase)
          {
            this.RaiseErrorOccurred(HtmlParseError.TagInvalidInFragmentMode, token);
            return;
          }
          this._currentMode = HtmlTreeMode.AfterAfterBody;
          return;
        }
        break;
      case HtmlTokenType.Comment:
        this._openElements[0].AddComment(token);
        return;
      case HtmlTokenType.Character:
        string text = token.TrimStart();
        this.ReconstructFormatting();
        this.AddCharacters(text);
        if (token.IsEmpty)
          return;
        break;
      case HtmlTokenType.EndOfFile:
        this.End();
        return;
    }
    this.RaiseErrorOccurred(HtmlParseError.TokenNotPossible, token);
    this._currentMode = HtmlTreeMode.InBody;
    this.InBody(token);
  }

  private void InFrameset(HtmlToken token)
  {
    switch (token.Type)
    {
      case HtmlTokenType.Doctype:
        this.RaiseErrorOccurred(HtmlParseError.DoctypeTagInappropriate, token);
        return;
      case HtmlTokenType.StartTag:
        string name = token.Name;
        if (name.Is(TagNames.Html))
        {
          this.InBody(token);
          return;
        }
        if (name.Is(TagNames.Frameset))
        {
          this.AddElement((Element) new HtmlFrameSetElement((Document) this._document), token.AsTag());
          return;
        }
        if (name.Is(TagNames.Frame))
        {
          if (this._options.IsNotSupportingFrames)
            this.AddElement((Element) new HtmlUnknownElement((Document) this._document, name), token.AsTag());
          else
            this.AddElement((Element) new HtmlFrameElement((Document) this._document), token.AsTag(), true);
          this.CloseCurrentNode();
          return;
        }
        if (name.Is(TagNames.NoFrames))
        {
          this.InHead(token);
          return;
        }
        break;
      case HtmlTokenType.EndTag:
        if (token.Name.Is(TagNames.Frameset))
        {
          if (this.CurrentNode != this._openElements[0])
          {
            this.CloseCurrentNode();
            if (this.IsFragmentCase || this.CurrentNode.LocalName.Is(TagNames.Frameset))
              return;
            this._currentMode = HtmlTreeMode.AfterFrameset;
            return;
          }
          this.RaiseErrorOccurred(HtmlParseError.CurrentNodeIsRoot, token);
          return;
        }
        break;
      case HtmlTokenType.Comment:
        this.CurrentNode.AddComment(token);
        return;
      case HtmlTokenType.Character:
        this.AddCharacters(token.TrimStart());
        if (token.IsEmpty)
          return;
        break;
      case HtmlTokenType.EndOfFile:
        if (this.CurrentNode != this._document.DocumentElement)
          this.RaiseErrorOccurred(HtmlParseError.CurrentNodeIsNotRoot, token);
        this.End();
        return;
    }
    this.RaiseErrorOccurred(HtmlParseError.TokenNotPossible, token);
  }

  private void AfterFrameset(HtmlToken token)
  {
    switch (token.Type)
    {
      case HtmlTokenType.Doctype:
        this.RaiseErrorOccurred(HtmlParseError.DoctypeTagInappropriate, token);
        return;
      case HtmlTokenType.StartTag:
        string name = token.Name;
        if (name.Is(TagNames.Html))
        {
          this.InBody(token);
          return;
        }
        if (name.Is(TagNames.NoFrames))
        {
          this.InHead(token);
          return;
        }
        break;
      case HtmlTokenType.EndTag:
        if (token.Name.Is(TagNames.Html))
        {
          this._currentMode = HtmlTreeMode.AfterAfterFrameset;
          return;
        }
        break;
      case HtmlTokenType.Comment:
        this.CurrentNode.AddComment(token);
        return;
      case HtmlTokenType.Character:
        this.AddCharacters(token.TrimStart());
        if (token.IsEmpty)
          return;
        break;
      case HtmlTokenType.EndOfFile:
        this.End();
        return;
    }
    this.RaiseErrorOccurred(HtmlParseError.TokenNotPossible, token);
  }

  private void AfterAfterBody(HtmlToken token)
  {
    switch (token.Type)
    {
      case HtmlTokenType.Doctype:
        this.InBody(token);
        return;
      case HtmlTokenType.StartTag:
        if (token.Name.Is(TagNames.Html))
        {
          this.InBody(token);
          return;
        }
        break;
      case HtmlTokenType.Comment:
        this._document.AddComment(token);
        return;
      case HtmlTokenType.Character:
        string text = token.TrimStart();
        this.ReconstructFormatting();
        this.AddCharacters(text);
        if (token.IsEmpty)
          return;
        break;
      case HtmlTokenType.EndOfFile:
        this.End();
        return;
    }
    this.RaiseErrorOccurred(HtmlParseError.TokenNotPossible, token);
    this._currentMode = HtmlTreeMode.InBody;
    this.InBody(token);
  }

  private void AfterAfterFrameset(HtmlToken token)
  {
    switch (token.Type)
    {
      case HtmlTokenType.Doctype:
        this.InBody(token);
        return;
      case HtmlTokenType.StartTag:
        string name = token.Name;
        if (name.Is(TagNames.Html))
        {
          this.InBody(token);
          return;
        }
        if (name.Is(TagNames.NoFrames))
        {
          this.InHead(token);
          return;
        }
        break;
      case HtmlTokenType.Comment:
        this._document.AddComment(token);
        return;
      case HtmlTokenType.Character:
        string text = token.TrimStart();
        this.ReconstructFormatting();
        this.AddCharacters(text);
        if (token.IsEmpty)
          return;
        break;
      case HtmlTokenType.EndOfFile:
        this.End();
        return;
    }
    this.RaiseErrorOccurred(HtmlParseError.TokenNotPossible, token);
  }

  private void TemplateStep(HtmlToken token, HtmlTreeMode mode)
  {
    int num = (int) this._templateModes.Pop();
    this._templateModes.Push(mode);
    this._currentMode = mode;
    this.Home(token);
  }

  private void CloseTemplate()
  {
    while (this._openElements.Count > 0)
    {
      HtmlTemplateElement currentNode = this.CurrentNode as HtmlTemplateElement;
      this.CloseCurrentNode();
      if (currentNode != null)
      {
        currentNode.PopulateFragment();
        break;
      }
    }
    this._formattingElements.ClearFormatting();
    int num = (int) this._templateModes.Pop();
    this.Reset();
  }

  private void InTableBodyCloseTable(HtmlTagToken tag)
  {
    if (this.IsInTableScope(TagNames.AllTableSections))
    {
      this.ClearStackBackTo(TagNames.AllTableSections);
      this.CloseCurrentNode();
      this._currentMode = HtmlTreeMode.InTable;
      this.InTable((HtmlToken) tag);
    }
    else
      this.RaiseErrorOccurred(HtmlParseError.TableSectionNotInScope, (HtmlToken) tag);
  }

  private void InSelectEndTagOption(HtmlToken token)
  {
    if (this.CurrentNode.LocalName.Is(TagNames.Option))
      this.CloseCurrentNode();
    else
      this.RaiseErrorOccurred(HtmlParseError.TagDoesNotMatchCurrentNode, token);
  }

  private void InSelectEndTagOptgroup(HtmlToken token)
  {
    if (this._openElements.Count > 1 && this._openElements[this._openElements.Count - 1].LocalName.Is(TagNames.Option) && this._openElements[this._openElements.Count - 2].LocalName.Is(TagNames.Optgroup))
      this.CloseCurrentNode();
    if (this.CurrentNode.LocalName.Is(TagNames.Optgroup))
      this.CloseCurrentNode();
    else
      this.RaiseErrorOccurred(HtmlParseError.TagDoesNotMatchCurrentNode, token);
  }

  private bool InColumnGroupEndTagColgroup(HtmlToken token)
  {
    if (this.CurrentNode.LocalName.Is(TagNames.Colgroup))
    {
      this.CloseCurrentNode();
      this._currentMode = HtmlTreeMode.InTable;
      return true;
    }
    this.RaiseErrorOccurred(HtmlParseError.TagDoesNotMatchCurrentNode, token);
    return false;
  }

  private void AfterHeadStartTagBody(HtmlTagToken token)
  {
    this.AddElement((Element) new HtmlBodyElement((Document) this._document), token);
    this._frameset = false;
    this._currentMode = HtmlTreeMode.InBody;
  }

  private void RawtextAlgorithm(HtmlTagToken tag)
  {
    this.AddElement(tag);
    this._previousMode = this._currentMode;
    this._currentMode = HtmlTreeMode.Text;
    this._tokenizer.State = HtmlParseMode.Rawtext;
  }

  private void RCDataAlgorithm(HtmlTagToken tag)
  {
    this.AddElement(tag);
    this._previousMode = this._currentMode;
    this._currentMode = HtmlTreeMode.Text;
    this._tokenizer.State = HtmlParseMode.RCData;
  }

  private void InBodyStartTagListItem(HtmlTagToken tag)
  {
    int index = this._openElements.Count - 1;
    Element openElement = this._openElements[index];
    this._frameset = false;
    for (; !openElement.LocalName.Is(TagNames.Li); openElement = this._openElements[--index])
    {
      if ((openElement.Flags & NodeFlags.Special) == NodeFlags.Special && !TagNames.AllBasicBlocks.Contains(openElement.LocalName))
        goto label_5;
    }
    this.InBody((HtmlToken) HtmlTagToken.Close(openElement.LocalName));
label_5:
    if (this.IsInButtonScope())
      this.InBodyEndTagParagraph((HtmlToken) tag);
    this.AddElement(tag);
  }

  private void InBodyStartTagDefinitionItem(HtmlTagToken tag)
  {
    this._frameset = false;
    int index = this._openElements.Count - 1;
    Element openElement;
    for (openElement = this._openElements[index]; !openElement.LocalName.IsOneOf(TagNames.Dd, TagNames.Dt); openElement = this._openElements[--index])
    {
      if ((openElement.Flags & NodeFlags.Special) == NodeFlags.Special && !TagNames.AllBasicBlocks.Contains(openElement.LocalName))
        goto label_5;
    }
    this.InBody((HtmlToken) HtmlTagToken.Close(openElement.LocalName));
label_5:
    if (this.IsInButtonScope())
      this.InBodyEndTagParagraph((HtmlToken) tag);
    this.AddElement(tag);
  }

  private bool InBodyEndTagBlock(HtmlTagToken tag)
  {
    if (this.IsInScope(tag.Name))
    {
      this.GenerateImpliedEndTags();
      if (!this.CurrentNode.LocalName.Is(tag.Name))
        this.RaiseErrorOccurred(HtmlParseError.TagDoesNotMatchCurrentNode, (HtmlToken) tag);
      this.ClearStackBackTo(tag.Name);
      this.CloseCurrentNode();
      return true;
    }
    this.RaiseErrorOccurred(HtmlParseError.BlockNotInScope, (HtmlToken) tag);
    return false;
  }

  private void HeisenbergAlgorithm(HtmlTagToken tag)
  {
    int num1 = 0;
    while (num1 < 8)
    {
      Element element1 = (Element) null;
      Element element2 = (Element) null;
      ++num1;
      int index1 = 0;
      int num2 = 0;
      for (int index2 = this._formattingElements.Count - 1; index2 >= 0 && this._formattingElements[index2] != null; --index2)
      {
        if (this._formattingElements[index2].LocalName.Is(tag.Name))
        {
          index1 = index2;
          element1 = this._formattingElements[index2];
          break;
        }
      }
      if (element1 == null)
      {
        this.InBodyEndTagAnythingElse(tag);
        break;
      }
      int num3 = this._openElements.IndexOf(element1);
      if (num3 == -1)
      {
        this.RaiseErrorOccurred(HtmlParseError.FormattingElementNotFound, (HtmlToken) tag);
        this._formattingElements.Remove(element1);
        break;
      }
      if (!this.IsInScope(element1.LocalName))
      {
        this.RaiseErrorOccurred(HtmlParseError.ElementNotInScope, (HtmlToken) tag);
        break;
      }
      if (num3 != this._openElements.Count - 1)
        this.RaiseErrorOccurred(HtmlParseError.TagClosedWrong, (HtmlToken) tag);
      int index3 = index1;
      for (int index4 = num3 + 1; index4 < this._openElements.Count; ++index4)
      {
        if ((this._openElements[index4].Flags & NodeFlags.Special) == NodeFlags.Special)
        {
          index1 = index4;
          element2 = this._openElements[index4];
          break;
        }
      }
      if (element2 == null)
      {
        Element currentNode;
        do
        {
          currentNode = this.CurrentNode;
          this.CloseCurrentNode();
        }
        while (currentNode != element1);
        this._formattingElements.Remove(element1);
        break;
      }
      Element openElement1 = this._openElements[num3 - 1];
      Element element3 = element2;
      while (true)
      {
        ++num2;
        Element openElement2 = this._openElements[--index1];
        if (openElement2 != element1)
        {
          if (num2 > 3 && this._formattingElements.Contains(openElement2))
            this._formattingElements.Remove(openElement2);
          if (!this._formattingElements.Contains(openElement2))
          {
            this.CloseNode(openElement2);
          }
          else
          {
            Element element4 = this.CopyElement(openElement2);
            openElement1.AddNode((AngleSharp.Dom.Node) element4);
            this._openElements[index1] = element4;
            for (int index5 = 0; index5 != this._formattingElements.Count; ++index5)
            {
              if (this._formattingElements[index5] == openElement2)
              {
                this._formattingElements[index5] = element4;
                break;
              }
            }
            Element element5 = element4;
            if (element3 == element2)
              ++index3;
            element3.Parent?.RemoveChild((INode) element3);
            element5.AddNode((AngleSharp.Dom.Node) element3);
            element3 = element5;
          }
        }
        else
          break;
      }
      element3.Parent?.RemoveChild((INode) element3);
      if (!TagNames.AllTableMajor.Contains(openElement1.LocalName))
        openElement1.AddNode((AngleSharp.Dom.Node) element3);
      else
        this.AddElementWithFoster(element3);
      Element element6 = this.CopyElement(element1);
      while (element2.ChildNodes.Length > 0)
      {
        AngleSharp.Dom.Node childNode = element2.ChildNodes[0];
        element2.RemoveNode(0, childNode);
        element6.AddNode(childNode);
      }
      element2.AddNode((AngleSharp.Dom.Node) element6);
      this._formattingElements.Remove(element1);
      this._formattingElements.Insert(index3, element6);
      this.CloseNode(element1);
      this._openElements.Insert(this._openElements.IndexOf(element2) + 1, element6);
    }
  }

  private Element CopyElement(Element element) => (Element) element.Clone(false);

  private void InBodyWithFoster(HtmlToken token)
  {
    this._foster = true;
    this.InBody(token);
    this._foster = false;
  }

  private void InBodyEndTagAnythingElse(HtmlTagToken tag)
  {
    int index = this._openElements.Count - 1;
    for (Element element = this.CurrentNode; element != null; element = this._openElements[--index])
    {
      if (element.LocalName.Is(tag.Name))
      {
        this.GenerateImpliedEndTagsExceptFor(tag.Name);
        if (!element.LocalName.Is(tag.Name))
          this.RaiseErrorOccurred(HtmlParseError.TagClosedWrong, (HtmlToken) tag);
        this.CloseNodesFrom(index);
        break;
      }
      if ((element.Flags & NodeFlags.Special) == NodeFlags.Special)
      {
        this.RaiseErrorOccurred(HtmlParseError.TagClosedWrong, (HtmlToken) tag);
        break;
      }
    }
  }

  private bool InBodyEndTagBody(HtmlToken token)
  {
    if (this.IsInScope(TagNames.Body))
    {
      this.CheckBodyOnClosing(token);
      this._currentMode = HtmlTreeMode.AfterBody;
      return true;
    }
    this.RaiseErrorOccurred(HtmlParseError.BodyNotInScope, token);
    return false;
  }

  private void InBodyStartTagBreakrow(HtmlTagToken tag)
  {
    this.ReconstructFormatting();
    this.AddElement(tag, true);
    this.CloseCurrentNode();
    this._frameset = false;
  }

  private bool InBodyEndTagParagraph(HtmlToken token)
  {
    if (this.IsInButtonScope())
    {
      this.GenerateImpliedEndTagsExceptFor(TagNames.P);
      if (!this.CurrentNode.LocalName.Is(TagNames.P))
        this.RaiseErrorOccurred(HtmlParseError.TagDoesNotMatchCurrentNode, token);
      this.ClearStackBackTo(TagNames.P);
      this.CloseCurrentNode();
      return true;
    }
    this.RaiseErrorOccurred(HtmlParseError.ParagraphNotInScope, token);
    this.InBody((HtmlToken) HtmlTagToken.Open(TagNames.P));
    this.InBodyEndTagParagraph(token);
    return false;
  }

  private bool InTableEndTagTable(HtmlToken token)
  {
    if (this.IsInTableScope(TagNames.Table))
    {
      this.ClearStackBackTo(TagNames.Table);
      this.CloseCurrentNode();
      this.Reset();
      return true;
    }
    this.RaiseErrorOccurred(HtmlParseError.TableNotInScope, token);
    return false;
  }

  private bool InRowEndTagTablerow(HtmlToken token)
  {
    if (this.IsInTableScope(TagNames.Tr))
    {
      this.ClearStackBackTo(TagNames.Tr);
      this.CloseCurrentNode();
      this._currentMode = HtmlTreeMode.InTableBody;
      return true;
    }
    this.RaiseErrorOccurred(HtmlParseError.TableRowNotInScope, token);
    return false;
  }

  private void InSelectEndTagSelect()
  {
    this.ClearStackBackTo(TagNames.Select);
    this.CloseCurrentNode();
    this.Reset();
  }

  private bool InCaptionEndTagCaption(HtmlToken token)
  {
    if (this.IsInTableScope(TagNames.Caption))
    {
      this.GenerateImpliedEndTags();
      if (!this.CurrentNode.LocalName.Is(TagNames.Caption))
        this.RaiseErrorOccurred(HtmlParseError.TagDoesNotMatchCurrentNode, token);
      this.ClearStackBackTo(TagNames.Caption);
      this.CloseCurrentNode();
      this._formattingElements.ClearFormatting();
      this._currentMode = HtmlTreeMode.InTable;
      return true;
    }
    this.RaiseErrorOccurred(HtmlParseError.CaptionNotInScope, token);
    return false;
  }

  private bool InCellEndTagCell(HtmlToken token)
  {
    if (this.IsInTableScope(TagNames.AllTableCells))
    {
      this.GenerateImpliedEndTags();
      if (!TagNames.AllTableCells.Contains(this.CurrentNode.LocalName))
        this.RaiseErrorOccurred(HtmlParseError.TagDoesNotMatchCurrentNode, token);
      this.ClearStackBackTo(TagNames.AllTableCells);
      this.CloseCurrentNode();
      this._formattingElements.ClearFormatting();
      this._currentMode = HtmlTreeMode.InRow;
      return true;
    }
    this.RaiseErrorOccurred(HtmlParseError.TableCellNotInScope, token);
    return false;
  }

  private void Foreign(HtmlToken token)
  {
    switch (token.Type)
    {
      case HtmlTokenType.Doctype:
        this.RaiseErrorOccurred(HtmlParseError.DoctypeTagInappropriate, token);
        break;
      case HtmlTokenType.StartTag:
        string name1 = token.Name;
        HtmlTagToken tag = token.AsTag();
        if (name1.Is(TagNames.Font))
        {
          for (int index = 0; index != tag.Attributes.Count; ++index)
          {
            if (tag.Attributes[index].Name.IsOneOf(AttributeNames.Color, AttributeNames.Face, AttributeNames.Size))
            {
              this.ForeignNormalTag(tag);
              return;
            }
          }
          this.ForeignSpecialTag(tag);
          break;
        }
        if (TagNames.AllForeignExceptions.Contains(name1))
        {
          this.ForeignNormalTag(tag);
          break;
        }
        this.ForeignSpecialTag(tag);
        break;
      case HtmlTokenType.EndTag:
        string name2 = token.Name;
        Element element = this.CurrentNode;
        if (element is HtmlScriptElement script)
        {
          this.HandleScript(script);
          break;
        }
        if (!element.LocalName.Is(name2))
          this.RaiseErrorOccurred(HtmlParseError.TagClosingMismatch, token);
        for (int index = this._openElements.Count - 1; index > 0; --index)
        {
          if (element.LocalName.Isi(name2))
          {
            this.CloseNodesFrom(index);
            break;
          }
          element = this._openElements[index - 1];
          if ((element.Flags & NodeFlags.HtmlMember) == NodeFlags.HtmlMember)
          {
            this.Home(token);
            break;
          }
        }
        break;
      case HtmlTokenType.Comment:
        this.CurrentNode.AddComment(token);
        break;
      case HtmlTokenType.Character:
        this.AddCharacters(token.Data.Replace(char.MinValue, '�'));
        this._frameset = !token.HasContent && this._frameset;
        break;
    }
  }

  private void ForeignSpecialTag(HtmlTagToken tag)
  {
    Element foreignElementFrom = this.CreateForeignElementFrom(tag);
    if (foreignElementFrom == null)
      return;
    int num = tag.IsSelfClosing ? 1 : 0;
    this.CurrentNode.AddNode((AngleSharp.Dom.Node) foreignElementFrom);
    if (num != 0)
      foreignElementFrom.SetupElement();
    if (num == 0)
    {
      this._openElements.Add(foreignElementFrom);
      this._tokenizer.IsAcceptingCharacterData = true;
    }
    else
    {
      if (!tag.Name.Is(TagNames.Script))
        return;
      this.Foreign((HtmlToken) HtmlTagToken.Close(TagNames.Script));
    }
  }

  private Element CreateForeignElementFrom(HtmlTagToken tag)
  {
    if ((this.AdjustedCurrentNode.Flags & NodeFlags.MathMember) == NodeFlags.MathMember)
    {
      MathElement mathElement = this._document.CreateMathElement(tag.Name);
      this.AuxiliarySetupSteps((Element) mathElement, tag);
      return (Element) mathElement.Setup(tag);
    }
    if ((this.AdjustedCurrentNode.Flags & NodeFlags.SvgMember) != NodeFlags.SvgMember)
      return (Element) null;
    SvgElement svgElement = this._document.CreateSvgElement(tag.Name.SanatizeSvgTagName());
    this.AuxiliarySetupSteps((Element) svgElement, tag);
    return (Element) svgElement.Setup(tag);
  }

  private void ForeignNormalTag(HtmlTagToken tag)
  {
    this.RaiseErrorOccurred(HtmlParseError.TagCannotStartHere, (HtmlToken) tag);
    if (!this.IsFragmentCase)
    {
      Element currentNode = this.CurrentNode;
      do
      {
        if (currentNode.LocalName.Is(TagNames.AnnotationXml))
        {
          string attribute = currentNode.GetAttribute((string) null, AttributeNames.Encoding);
          if (attribute.Isi(MimeTypeNames.Html) || attribute.Isi(MimeTypeNames.ApplicationXHtml))
          {
            this.AddElement(tag);
            return;
          }
        }
        this.CloseCurrentNode();
        currentNode = this.CurrentNode;
      }
      while ((currentNode.Flags & (NodeFlags.HtmlMember | NodeFlags.HtmlTip | NodeFlags.MathTip)) == NodeFlags.None);
      this.Consume((HtmlToken) tag);
    }
    else
      this.ForeignSpecialTag(tag);
  }

  private bool IsInScope(string tagName)
  {
    for (int index = this._openElements.Count - 1; index >= 0; --index)
    {
      Element openElement = this._openElements[index];
      if (openElement.LocalName.Is(tagName))
        return true;
      if ((openElement.Flags & NodeFlags.Scoped) == NodeFlags.Scoped)
        return false;
    }
    return false;
  }

  private bool IsInScope(HashSet<string> tags)
  {
    for (int index = this._openElements.Count - 1; index >= 0; --index)
    {
      Element openElement = this._openElements[index];
      if (tags.Contains(openElement.LocalName))
        return true;
      if ((openElement.Flags & NodeFlags.Scoped) == NodeFlags.Scoped)
        return false;
    }
    return false;
  }

  private bool IsInListItemScope()
  {
    for (int index = this._openElements.Count - 1; index >= 0; --index)
    {
      Element openElement = this._openElements[index];
      if (openElement.LocalName.Is(TagNames.Li))
        return true;
      if ((openElement.Flags & NodeFlags.HtmlListScoped) == NodeFlags.HtmlListScoped)
        return false;
    }
    return false;
  }

  private bool IsInButtonScope()
  {
    for (int index = this._openElements.Count - 1; index >= 0; --index)
    {
      Element openElement = this._openElements[index];
      if (openElement.LocalName.Is(TagNames.P))
        return true;
      if ((openElement.Flags & NodeFlags.Scoped) == NodeFlags.Scoped || openElement.LocalName.Is(TagNames.Button))
        return false;
    }
    return false;
  }

  private bool IsInTableScope(HashSet<string> tags)
  {
    for (int index = this._openElements.Count - 1; index >= 0; --index)
    {
      Element openElement = this._openElements[index];
      if (tags.Contains(openElement.LocalName))
        return true;
      if ((openElement.Flags & NodeFlags.HtmlTableScoped) == NodeFlags.HtmlTableScoped)
        return false;
    }
    return false;
  }

  private bool IsInTableScope(string tagName)
  {
    for (int index = this._openElements.Count - 1; index >= 0; --index)
    {
      Element openElement = this._openElements[index];
      if (openElement.LocalName.Is(tagName))
        return true;
      if ((openElement.Flags & NodeFlags.HtmlTableScoped) == NodeFlags.HtmlTableScoped)
        return false;
    }
    return false;
  }

  private bool IsInSelectScope(string tagName)
  {
    for (int index = this._openElements.Count - 1; index >= 0; --index)
    {
      Element openElement = this._openElements[index];
      if (openElement.LocalName.Is(tagName))
        return true;
      if ((openElement.Flags & NodeFlags.HtmlSelectScoped) != NodeFlags.HtmlSelectScoped)
        return false;
    }
    return false;
  }

  private void HandleScript(HtmlScriptElement script)
  {
    if (script == null)
      return;
    if (this.IsFragmentCase)
    {
      this.CloseCurrentNode();
      this._currentMode = this._previousMode;
    }
    else
    {
      this._document.PerformMicrotaskCheckpoint();
      this._document.ProvideStableState();
      this.CloseCurrentNode();
      this._currentMode = this._previousMode;
      if (!script.Prepare((Document) this._document))
        return;
      this._waiting = this.RunScript(script);
    }
  }

  private async Task RunScript(HtmlScriptElement script)
  {
    await this._document.WaitForReadyAsync().ConfigureAwait(false);
    await script.RunAsync(CancellationToken.None).ConfigureAwait(false);
  }

  private void CheckBodyOnClosing(HtmlToken token)
  {
    for (int index = 0; index < this._openElements.Count; ++index)
    {
      if ((this._openElements[index].Flags & NodeFlags.ImplicitelyClosed) != NodeFlags.ImplicitelyClosed)
      {
        this.RaiseErrorOccurred(HtmlParseError.BodyClosedWrong, token);
        break;
      }
    }
  }

  private bool TagCurrentlyOpen(string tagName)
  {
    for (int index = 0; index < this._openElements.Count; ++index)
    {
      if (this._openElements[index].LocalName.Is(tagName))
        return true;
    }
    return false;
  }

  private void PreventNewLine()
  {
    HtmlToken token = this._tokenizer.Get();
    if (token.Type == HtmlTokenType.Character)
      token.RemoveNewLine();
    this.Home(token);
  }

  private void End()
  {
    while (this._openElements.Count != 0)
      this.CloseCurrentNode();
    if (!this._document.IsLoading)
      return;
    this._waiting = this._document.FinishLoadingAsync();
  }

  private void AddRoot(HtmlTagToken tag)
  {
    HtmlHtmlElement htmlHtmlElement = new HtmlHtmlElement((Document) this._document);
    this._document.AddNode((AngleSharp.Dom.Node) htmlHtmlElement);
    this.SetupElement((Element) htmlHtmlElement, tag, false);
    this._openElements.Add((Element) htmlHtmlElement);
    this._tokenizer.IsAcceptingCharacterData = false;
    this._document.ApplyManifest();
  }

  private void CloseNode(Element element)
  {
    element.SetupElement();
    this._openElements.Remove(element);
  }

  private void CloseNodesFrom(int index)
  {
    for (int index1 = this._openElements.Count - 1; index1 > index; --index1)
    {
      this._openElements[index1].SetupElement();
      this._openElements.RemoveAt(index1);
    }
    this.CloseCurrentNode();
  }

  private void CloseCurrentNode()
  {
    if (this._openElements.Count <= 0)
      return;
    int index = this._openElements.Count - 1;
    this._openElements[index].SetupElement();
    this._openElements.RemoveAt(index);
    Element adjustedCurrentNode = this.AdjustedCurrentNode;
    this._tokenizer.IsAcceptingCharacterData = adjustedCurrentNode != null && (adjustedCurrentNode.Flags & NodeFlags.HtmlMember) != NodeFlags.HtmlMember;
  }

  private void SetupElement(Element element, HtmlTagToken tag, bool acknowledgeSelfClosing)
  {
    if (tag.IsSelfClosing && !acknowledgeSelfClosing)
      this.RaiseErrorOccurred(HtmlParseError.TagCannotBeSelfClosed, (HtmlToken) tag);
    this.AuxiliarySetupSteps(element, tag);
    element.SetAttributes(tag.Attributes);
  }

  private Element AddElement(HtmlTagToken tag, bool acknowledgeSelfClosing = false)
  {
    HtmlElement htmlElement = this._document.CreateHtmlElement(tag.Name);
    this.SetupElement((Element) htmlElement, tag, acknowledgeSelfClosing);
    this.AddElement((Element) htmlElement);
    return (Element) htmlElement;
  }

  private void AddElement(Element element, HtmlTagToken tag, bool acknowledgeSelfClosing = false)
  {
    this.SetupElement(element, tag, acknowledgeSelfClosing);
    this.AddElement(element);
  }

  private void AddElement(Element element)
  {
    Element currentNode = this.CurrentNode;
    if (this._foster && TagNames.AllTableMajor.Contains(currentNode.LocalName))
      this.AddElementWithFoster(element);
    else
      currentNode.AddNode((AngleSharp.Dom.Node) element);
    this._openElements.Add(element);
    this._tokenizer.IsAcceptingCharacterData = (element.Flags & NodeFlags.HtmlMember) != NodeFlags.HtmlMember;
  }

  private void AddElementWithFoster(Element element)
  {
    bool flag = false;
    int count = this._openElements.Count;
    while (--count != 0)
    {
      if (this._openElements[count].LocalName.Is(TagNames.Template))
      {
        this._openElements[count].AddNode((AngleSharp.Dom.Node) element);
        return;
      }
      if (this._openElements[count].LocalName.Is(TagNames.Table))
      {
        flag = true;
        break;
      }
    }
    AngleSharp.Dom.Node node = this._openElements[count].Parent ?? (AngleSharp.Dom.Node) this._openElements[count + 1];
    if (flag && this._openElements[count].Parent != null)
    {
      for (int index = 0; index < node.ChildNodes.Length; ++index)
      {
        if (node.ChildNodes[index] == this._openElements[count])
        {
          node.InsertNode(index, (AngleSharp.Dom.Node) element);
          break;
        }
      }
    }
    else
      node.AddNode((AngleSharp.Dom.Node) element);
  }

  private void AddCharacters(string text)
  {
    if (string.IsNullOrEmpty(text))
      return;
    Element currentNode = this.CurrentNode;
    if (this._foster && TagNames.AllTableMajor.Contains(currentNode.LocalName))
      this.AddCharactersWithFoster(text);
    else
      currentNode.AppendText(text);
  }

  private void AddCharactersWithFoster(string text)
  {
    bool flag = false;
    int count = this._openElements.Count;
    while (--count != 0)
    {
      if (this._openElements[count].LocalName.Is(TagNames.Template))
      {
        this._openElements[count].AppendText(text);
        return;
      }
      if (this._openElements[count].LocalName.Is(TagNames.Table))
      {
        flag = true;
        break;
      }
    }
    AngleSharp.Dom.Node node = this._openElements[count].Parent ?? (AngleSharp.Dom.Node) this._openElements[count + 1];
    if (flag && this._openElements[count].Parent != null)
    {
      for (int index = 0; index < node.ChildNodes.Length; ++index)
      {
        if (node.ChildNodes[index] == this._openElements[count])
        {
          node.InsertText(index, text);
          break;
        }
      }
    }
    else
      node.AppendText(text);
  }

  private void AuxiliarySetupSteps(Element element, HtmlTagToken tag)
  {
    if (this._options.IsKeepingSourceReferences)
      element.SourceReference = (ISourceReference) tag;
    if (this._options.OnCreated == null)
      return;
    this._options.OnCreated((IElement) element, tag.Position);
  }

  private void ClearStackBackTo(string tagName)
  {
    for (Element currentNode = this.CurrentNode; !currentNode.LocalName.IsOneOf(tagName, TagNames.Html, TagNames.Template); currentNode = this.CurrentNode)
      this.CloseCurrentNode();
  }

  private void ClearStackBackTo(HashSet<string> tags)
  {
    for (Element currentNode = this.CurrentNode; !tags.Contains(currentNode.LocalName) && !currentNode.LocalName.IsOneOf(TagNames.Html, TagNames.Template); currentNode = this.CurrentNode)
      this.CloseCurrentNode();
  }

  private void GenerateImpliedEndTagsExceptFor(string tagName)
  {
    for (Element currentNode = this.CurrentNode; (currentNode.Flags & NodeFlags.ImpliedEnd) == NodeFlags.ImpliedEnd && !currentNode.LocalName.Is(tagName); currentNode = this.CurrentNode)
      this.CloseCurrentNode();
  }

  private void GenerateImpliedEndTags()
  {
    while ((this.CurrentNode.Flags & NodeFlags.ImpliedEnd) == NodeFlags.ImpliedEnd)
      this.CloseCurrentNode();
  }

  private void ReconstructFormatting()
  {
    if (this._formattingElements.Count == 0)
      return;
    int index = this._formattingElements.Count - 1;
    Element formattingElement1 = this._formattingElements[index];
    if (formattingElement1 == null || this._openElements.Contains(formattingElement1))
      return;
    while (index > 0)
    {
      Element formattingElement2 = this._formattingElements[--index];
      if (formattingElement2 == null || this._openElements.Contains(formattingElement2))
      {
        ++index;
        break;
      }
    }
    for (; index < this._formattingElements.Count; ++index)
    {
      Element element = this.CopyElement(this._formattingElements[index]);
      this.AddElement(element);
      this._formattingElements[index] = element;
    }
  }

  private void RaiseErrorOccurred(HtmlParseError code, HtmlToken token)
  {
    this._tokenizer.RaiseErrorOccurred(code, token.Position);
  }
}

// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.HtmlElementFactory
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Text;
using System;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Html;

internal sealed class HtmlElementFactory : IElementFactory<Document, HtmlElement>
{
  private readonly Dictionary<string, HtmlElementFactory.Creator> creators = new Dictionary<string, HtmlElementFactory.Creator>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
  {
    {
      TagNames.Div,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlDivElement(document, prefix))
    },
    {
      TagNames.A,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlAnchorElement(document, prefix))
    },
    {
      TagNames.Img,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlImageElement(document, prefix))
    },
    {
      TagNames.P,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlParagraphElement(document, prefix))
    },
    {
      TagNames.Br,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlBreakRowElement(document, prefix))
    },
    {
      TagNames.Input,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlInputElement(document, prefix))
    },
    {
      TagNames.Button,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlButtonElement(document, prefix))
    },
    {
      TagNames.Textarea,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlTextAreaElement(document, prefix))
    },
    {
      TagNames.Li,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlListItemElement(document, TagNames.Li, prefix))
    },
    {
      TagNames.H1,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlHeadingElement(document, TagNames.H1, prefix))
    },
    {
      TagNames.H2,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlHeadingElement(document, TagNames.H2, prefix))
    },
    {
      TagNames.H3,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlHeadingElement(document, TagNames.H3, prefix))
    },
    {
      TagNames.H4,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlHeadingElement(document, TagNames.H4, prefix))
    },
    {
      TagNames.H5,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlHeadingElement(document, TagNames.H5, prefix))
    },
    {
      TagNames.H6,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlHeadingElement(document, TagNames.H6, prefix))
    },
    {
      TagNames.Ul,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlUnorderedListElement(document, prefix))
    },
    {
      TagNames.Ol,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlOrderedListElement(document, prefix))
    },
    {
      TagNames.Dl,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlDefinitionListElement(document, prefix))
    },
    {
      TagNames.Link,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlLinkElement(document, prefix))
    },
    {
      TagNames.Meta,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlMetaElement(document, prefix))
    },
    {
      TagNames.Label,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlLabelElement(document, prefix))
    },
    {
      TagNames.Fieldset,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlFieldSetElement(document, prefix))
    },
    {
      TagNames.Legend,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlLegendElement(document, prefix))
    },
    {
      TagNames.Form,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlFormElement(document, prefix))
    },
    {
      TagNames.Select,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlSelectElement(document, prefix))
    },
    {
      TagNames.Pre,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlPreElement(document, prefix))
    },
    {
      TagNames.Hr,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlHrElement(document, prefix))
    },
    {
      TagNames.Dir,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlDirectoryElement(document, prefix))
    },
    {
      TagNames.Font,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlFontElement(document, prefix))
    },
    {
      TagNames.Param,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlParamElement(document, prefix))
    },
    {
      TagNames.BlockQuote,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlQuoteElement(document, TagNames.BlockQuote, prefix))
    },
    {
      TagNames.Quote,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlQuoteElement(document, TagNames.Quote, prefix))
    },
    {
      TagNames.Q,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlQuoteElement(document, TagNames.Q, prefix))
    },
    {
      TagNames.Canvas,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlCanvasElement(document, prefix))
    },
    {
      TagNames.Caption,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlTableCaptionElement(document, prefix))
    },
    {
      TagNames.Td,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlTableDataCellElement(document, prefix))
    },
    {
      TagNames.Tr,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlTableRowElement(document, prefix))
    },
    {
      TagNames.Table,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlTableElement(document, prefix))
    },
    {
      TagNames.Tbody,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlTableSectionElement(document, TagNames.Tbody, prefix))
    },
    {
      TagNames.Th,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlTableHeaderCellElement(document, prefix))
    },
    {
      TagNames.Tfoot,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlTableSectionElement(document, TagNames.Tfoot, prefix))
    },
    {
      TagNames.Thead,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlTableSectionElement(document, TagNames.Thead, prefix))
    },
    {
      TagNames.Colgroup,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlTableColgroupElement(document, prefix))
    },
    {
      TagNames.Col,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlTableColElement(document, prefix))
    },
    {
      TagNames.Del,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlModElement(document, TagNames.Del, prefix))
    },
    {
      TagNames.Ins,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlModElement(document, TagNames.Ins, prefix))
    },
    {
      TagNames.Applet,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlAppletElement(document, prefix))
    },
    {
      TagNames.Object,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlObjectElement(document, prefix))
    },
    {
      TagNames.Optgroup,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlOptionsGroupElement(document, prefix))
    },
    {
      TagNames.Option,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlOptionElement(document, prefix))
    },
    {
      TagNames.Style,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlStyleElement(document, prefix))
    },
    {
      TagNames.Script,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlScriptElement(document, prefix))
    },
    {
      TagNames.Iframe,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlIFrameElement(document, prefix))
    },
    {
      TagNames.Dd,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlListItemElement(document, TagNames.Dd, prefix))
    },
    {
      TagNames.Dt,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlListItemElement(document, TagNames.Dt, prefix))
    },
    {
      TagNames.Frameset,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlFrameSetElement(document, prefix))
    },
    {
      TagNames.Frame,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlFrameElement(document, prefix))
    },
    {
      TagNames.Audio,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlAudioElement(document, prefix))
    },
    {
      TagNames.Video,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlVideoElement(document, prefix))
    },
    {
      TagNames.Span,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlSpanElement(document, prefix))
    },
    {
      TagNames.Dialog,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlDialogElement(document, prefix))
    },
    {
      TagNames.Details,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlDetailsElement(document, prefix))
    },
    {
      TagNames.Source,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlSourceElement(document, prefix))
    },
    {
      TagNames.Track,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlTrackElement(document, prefix))
    },
    {
      TagNames.Wbr,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlWbrElement(document, prefix))
    },
    {
      TagNames.B,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlBoldElement(document, prefix))
    },
    {
      TagNames.Big,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlBigElement(document, prefix))
    },
    {
      TagNames.Strike,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlStrikeElement(document, prefix))
    },
    {
      TagNames.Code,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlCodeElement(document, prefix))
    },
    {
      TagNames.Em,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlEmphasizeElement(document, prefix))
    },
    {
      TagNames.I,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlItalicElement(document, prefix))
    },
    {
      TagNames.S,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlStruckElement(document, prefix))
    },
    {
      TagNames.Small,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlSmallElement(document, prefix))
    },
    {
      TagNames.Strong,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlStrongElement(document, prefix))
    },
    {
      TagNames.U,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlUnderlineElement(document, prefix))
    },
    {
      TagNames.Tt,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlTeletypeTextElement(document, prefix))
    },
    {
      TagNames.Address,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlAddressElement(document, prefix))
    },
    {
      TagNames.Main,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlSemanticElement(document, TagNames.Main, prefix))
    },
    {
      TagNames.Summary,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlSemanticElement(document, TagNames.Summary, prefix))
    },
    {
      TagNames.Center,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlSemanticElement(document, TagNames.Center, prefix))
    },
    {
      TagNames.Listing,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlSemanticElement(document, TagNames.Listing, prefix))
    },
    {
      TagNames.Nav,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlSemanticElement(document, TagNames.Nav, prefix))
    },
    {
      TagNames.Article,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlSemanticElement(document, TagNames.Article, prefix))
    },
    {
      TagNames.Aside,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlSemanticElement(document, TagNames.Aside, prefix))
    },
    {
      TagNames.Figcaption,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlSemanticElement(document, TagNames.Figcaption, prefix))
    },
    {
      TagNames.Figure,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlSemanticElement(document, TagNames.Figure, prefix))
    },
    {
      TagNames.Section,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlSemanticElement(document, TagNames.Section, prefix))
    },
    {
      TagNames.Footer,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlSemanticElement(document, TagNames.Footer, prefix))
    },
    {
      TagNames.Header,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlSemanticElement(document, TagNames.Header, prefix))
    },
    {
      TagNames.Hgroup,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlSemanticElement(document, TagNames.Hgroup, prefix))
    },
    {
      TagNames.Cite,
      (HtmlElementFactory.Creator) ((document, prefix) => new HtmlElement(document, TagNames.Cite, prefix))
    },
    {
      TagNames.Ruby,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlRubyElement(document, prefix))
    },
    {
      TagNames.Rt,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlRtElement(document, prefix))
    },
    {
      TagNames.Rp,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlRpElement(document, prefix))
    },
    {
      TagNames.Rtc,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlRtcElement(document, prefix))
    },
    {
      TagNames.Rb,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlRbElement(document, prefix))
    },
    {
      TagNames.Map,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlMapElement(document, prefix))
    },
    {
      TagNames.Datalist,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlDataListElement(document, prefix))
    },
    {
      TagNames.Xmp,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlXmpElement(document, prefix))
    },
    {
      TagNames.Picture,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlPictureElement(document, prefix))
    },
    {
      TagNames.Template,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlTemplateElement(document, prefix))
    },
    {
      TagNames.Time,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlTimeElement(document, prefix))
    },
    {
      TagNames.Progress,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlProgressElement(document, prefix))
    },
    {
      TagNames.Meter,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlMeterElement(document, prefix))
    },
    {
      TagNames.Output,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlOutputElement(document, prefix))
    },
    {
      TagNames.Keygen,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlKeygenElement(document, prefix))
    },
    {
      TagNames.Title,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlTitleElement(document, prefix))
    },
    {
      TagNames.Head,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlHeadElement(document, prefix))
    },
    {
      TagNames.Body,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlBodyElement(document, prefix))
    },
    {
      TagNames.Html,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlHtmlElement(document, prefix))
    },
    {
      TagNames.Area,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlAreaElement(document, prefix))
    },
    {
      TagNames.Embed,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlEmbedElement(document, prefix))
    },
    {
      TagNames.MenuItem,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlMenuItemElement(document, prefix))
    },
    {
      TagNames.Slot,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlSlotElement(document, prefix))
    },
    {
      TagNames.NoScript,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlNoScriptElement(document, prefix))
    },
    {
      TagNames.NoEmbed,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlNoEmbedElement(document, prefix))
    },
    {
      TagNames.NoFrames,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlNoFramesElement(document, prefix))
    },
    {
      TagNames.NoBr,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlNoNewlineElement(document, prefix))
    },
    {
      TagNames.Menu,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlMenuElement(document, prefix))
    },
    {
      TagNames.Base,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlBaseElement(document, prefix))
    },
    {
      TagNames.BaseFont,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlBaseFontElement(document, prefix))
    },
    {
      TagNames.Bgsound,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlBgsoundElement(document, prefix))
    },
    {
      TagNames.Marquee,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlMarqueeElement(document, prefix))
    },
    {
      TagNames.Data,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlDataElement(document, prefix))
    },
    {
      TagNames.Plaintext,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlSemanticElement(document, TagNames.Plaintext, prefix))
    },
    {
      TagNames.IsIndex,
      (HtmlElementFactory.Creator) ((document, prefix) => (HtmlElement) new HtmlIsIndexElement(document, prefix))
    },
    {
      TagNames.Mark,
      (HtmlElementFactory.Creator) ((document, prefix) => new HtmlElement(document, TagNames.Mark))
    },
    {
      TagNames.Sub,
      (HtmlElementFactory.Creator) ((document, prefix) => new HtmlElement(document, TagNames.Sub))
    },
    {
      TagNames.Sup,
      (HtmlElementFactory.Creator) ((document, prefix) => new HtmlElement(document, TagNames.Sup))
    },
    {
      TagNames.Dfn,
      (HtmlElementFactory.Creator) ((document, prefix) => new HtmlElement(document, TagNames.Dfn))
    },
    {
      TagNames.Kbd,
      (HtmlElementFactory.Creator) ((document, prefix) => new HtmlElement(document, TagNames.Kbd))
    },
    {
      TagNames.Var,
      (HtmlElementFactory.Creator) ((document, prefix) => new HtmlElement(document, TagNames.Var))
    },
    {
      TagNames.Samp,
      (HtmlElementFactory.Creator) ((document, prefix) => new HtmlElement(document, TagNames.Samp))
    },
    {
      TagNames.Abbr,
      (HtmlElementFactory.Creator) ((document, prefix) => new HtmlElement(document, TagNames.Abbr))
    },
    {
      TagNames.Bdi,
      (HtmlElementFactory.Creator) ((document, prefix) => new HtmlElement(document, TagNames.Bdi))
    },
    {
      TagNames.Bdo,
      (HtmlElementFactory.Creator) ((document, prefix) => new HtmlElement(document, TagNames.Bdo))
    }
  };

  public HtmlElement Create(Document document, string localName, string prefix = null)
  {
    HtmlElementFactory.Creator creator;
    return this.creators.TryGetValue(localName, out creator) ? creator(document, prefix) : (HtmlElement) new HtmlUnknownElement(document, localName.HtmlLower(), prefix);
  }

  private delegate HtmlElement Creator(Document owner, string prefix);
}

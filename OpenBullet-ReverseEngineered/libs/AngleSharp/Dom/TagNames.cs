// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.TagNames
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Dom;

public static class TagNames
{
  public static readonly string Doctype = "DOCTYPE";
  public static readonly string Html = "html";
  public static readonly string Body = "body";
  public static readonly string Head = "head";
  public static readonly string Meta = "meta";
  public static readonly string Title = "title";
  public static readonly string Bgsound = "bgsound";
  public static readonly string Script = "script";
  public static readonly string Style = "style";
  public static readonly string NoEmbed = "noembed";
  public static readonly string NoScript = "noscript";
  public static readonly string NoFrames = "noframes";
  public static readonly string Menu = "menu";
  public static readonly string MenuItem = "menuitem";
  public static readonly string Var = "var";
  public static readonly string Ruby = "ruby";
  public static readonly string Sub = "sub";
  public static readonly string Sup = "sup";
  public static readonly string Rp = "rp";
  public static readonly string Rt = "rt";
  public static readonly string Rb = "rb";
  public static readonly string Rtc = "rtc";
  public static readonly string Applet = "applet";
  public static readonly string Embed = "embed";
  public static readonly string Marquee = "marquee";
  public static readonly string Param = "param";
  public static readonly string Object = "object";
  public static readonly string Canvas = "canvas";
  public static readonly string Font = "font";
  public static readonly string Ins = "ins";
  public static readonly string Del = "del";
  public static readonly string Template = "template";
  public static readonly string Slot = "slot";
  public static readonly string Caption = "caption";
  public static readonly string Col = "col";
  public static readonly string Colgroup = "colgroup";
  public static readonly string Table = "table";
  public static readonly string Thead = "thead";
  public static readonly string Tbody = "tbody";
  public static readonly string Tfoot = "tfoot";
  public static readonly string Th = "th";
  public static readonly string Td = "td";
  public static readonly string Tr = "tr";
  public static readonly string Input = "input";
  public static readonly string Keygen = "keygen";
  public static readonly string Textarea = "textarea";
  public static readonly string P = "p";
  public static readonly string Span = "span";
  public static readonly string Dialog = "dialog";
  public static readonly string Fieldset = "fieldset";
  public static readonly string Legend = "legend";
  public static readonly string Label = "label";
  public static readonly string Details = "details";
  public static readonly string Form = "form";
  public static readonly string IsIndex = "isindex";
  public static readonly string Pre = "pre";
  public static readonly string Data = "data";
  public static readonly string Datalist = "datalist";
  public static readonly string Ol = "ol";
  public static readonly string Ul = "ul";
  public static readonly string Dl = "dl";
  public static readonly string Li = "li";
  public static readonly string Dd = "dd";
  public static readonly string Dt = "dt";
  public static readonly string B = "b";
  public static readonly string Big = "big";
  public static readonly string Strike = "strike";
  public static readonly string Code = "code";
  public static readonly string Em = "em";
  public static readonly string I = "i";
  public static readonly string S = "s";
  public static readonly string Small = "small";
  public static readonly string Strong = "strong";
  public static readonly string U = "u";
  public static readonly string Tt = "tt";
  public static readonly string NoBr = "nobr";
  public static readonly string Select = "select";
  public static readonly string Option = "option";
  public static readonly string Optgroup = "optgroup";
  public static readonly string Link = "link";
  public static readonly string Frameset = "frameset";
  public static readonly string Frame = "frame";
  public static readonly string Iframe = "iframe";
  public static readonly string Audio = "audio";
  public static readonly string Video = "video";
  public static readonly string Source = "source";
  public static readonly string Track = "track";
  public static readonly string H1 = "h1";
  public static readonly string H2 = "h2";
  public static readonly string H3 = "h3";
  public static readonly string H4 = "h4";
  public static readonly string H5 = "h5";
  public static readonly string H6 = "h6";
  public static readonly string Div = "div";
  public static readonly string Quote = "quote";
  public static readonly string BlockQuote = "blockquote";
  public static readonly string Q = "q";
  public static readonly string Base = "base";
  public static readonly string BaseFont = "basefont";
  public static readonly string A = "a";
  public static readonly string Area = "area";
  public static readonly string Button = "button";
  public static readonly string Cite = "cite";
  public static readonly string Main = "main";
  public static readonly string Summary = "summary";
  public static readonly string Xmp = "xmp";
  public static readonly string Br = "br";
  public static readonly string Wbr = "wbr";
  public static readonly string Hr = "hr";
  public static readonly string Dir = "dir";
  public static readonly string Center = "center";
  public static readonly string Listing = "listing";
  public static readonly string Img = "img";
  public static readonly string Image = "image";
  public static readonly string Nav = "nav";
  public static readonly string Address = "address";
  public static readonly string Article = "article";
  public static readonly string Aside = "aside";
  public static readonly string Figcaption = "figcaption";
  public static readonly string Figure = "figure";
  public static readonly string Section = "section";
  public static readonly string Footer = "footer";
  public static readonly string Header = "header";
  public static readonly string Hgroup = "hgroup";
  public static readonly string Plaintext = "plaintext";
  public static readonly string Time = "time";
  public static readonly string Progress = "progress";
  public static readonly string Meter = "meter";
  public static readonly string Output = "output";
  public static readonly string Map = "map";
  public static readonly string Picture = "picture";
  public static readonly string Mark = "mark";
  public static readonly string Dfn = "dfn";
  public static readonly string Kbd = "kbd";
  public static readonly string Samp = "samp";
  public static readonly string Abbr = "abbr";
  public static readonly string Bdi = "bdi";
  public static readonly string Bdo = "bdo";
  public static readonly string Math = "math";
  public static readonly string Mi = "mi";
  public static readonly string Mo = "mo";
  public static readonly string Mn = "mn";
  public static readonly string Ms = "ms";
  public static readonly string Mtext = "mtext";
  public static readonly string AnnotationXml = "annotation-xml";
  public static readonly string Svg = "svg";
  public static readonly string ForeignObject = "foreignObject";
  public static readonly string Desc = "desc";
  public static readonly string Circle = "circle";
  public static readonly string Xml = "xml";
  internal static readonly HashSet<string> AllForeignExceptions = new HashSet<string>()
  {
    TagNames.B,
    TagNames.Big,
    TagNames.BlockQuote,
    TagNames.Body,
    TagNames.Br,
    TagNames.Center,
    TagNames.Code,
    TagNames.Dd,
    TagNames.Div,
    TagNames.Dl,
    TagNames.Dt,
    TagNames.Em,
    TagNames.Embed,
    TagNames.Head,
    TagNames.Hr,
    TagNames.I,
    TagNames.Img,
    TagNames.Li,
    TagNames.Ul,
    TagNames.H3,
    TagNames.H2,
    TagNames.H4,
    TagNames.H1,
    TagNames.H6,
    TagNames.H5,
    TagNames.Listing,
    TagNames.Menu,
    TagNames.Meta,
    TagNames.NoBr,
    TagNames.Ol,
    TagNames.P,
    TagNames.Pre,
    TagNames.Ruby,
    TagNames.S,
    TagNames.Small,
    TagNames.Span,
    TagNames.Strike,
    TagNames.Strong,
    TagNames.Sub,
    TagNames.Sup,
    TagNames.Table,
    TagNames.Tt,
    TagNames.U,
    TagNames.Var
  };
  internal static readonly HashSet<string> AllBeforeHead = new HashSet<string>()
  {
    TagNames.Html,
    TagNames.Body,
    TagNames.Br,
    TagNames.Head
  };
  internal static readonly HashSet<string> AllNoShadowRoot = new HashSet<string>()
  {
    TagNames.Button,
    TagNames.Details,
    TagNames.Input,
    TagNames.Marquee,
    TagNames.Meter,
    TagNames.Progress,
    TagNames.Select,
    TagNames.Textarea,
    TagNames.Keygen
  };
  internal static readonly HashSet<string> AllHead = new HashSet<string>()
  {
    TagNames.Style,
    TagNames.Link,
    TagNames.Meta,
    TagNames.Title,
    TagNames.NoFrames,
    TagNames.Template,
    TagNames.Base,
    TagNames.BaseFont,
    TagNames.Bgsound
  };
  internal static readonly HashSet<string> AllHeadNoTemplate = new HashSet<string>()
  {
    TagNames.Link,
    TagNames.Meta,
    TagNames.Script,
    TagNames.Style,
    TagNames.Title,
    TagNames.Base,
    TagNames.BaseFont,
    TagNames.Bgsound,
    TagNames.NoFrames
  };
  internal static readonly HashSet<string> AllHeadBase = new HashSet<string>()
  {
    TagNames.Link,
    TagNames.Base,
    TagNames.BaseFont,
    TagNames.Bgsound
  };
  internal static readonly HashSet<string> AllBodyBreakrow = new HashSet<string>()
  {
    TagNames.Br,
    TagNames.Area,
    TagNames.Embed,
    TagNames.Keygen,
    TagNames.Wbr
  };
  internal static readonly HashSet<string> AllBodyClosed = new HashSet<string>()
  {
    TagNames.MenuItem,
    TagNames.Param,
    TagNames.Source,
    TagNames.Track
  };
  internal static readonly HashSet<string> AllNoScript = new HashSet<string>()
  {
    TagNames.Style,
    TagNames.Link,
    TagNames.BaseFont,
    TagNames.Meta,
    TagNames.NoFrames,
    TagNames.Bgsound
  };
  internal static readonly HashSet<string> AllHeadings = new HashSet<string>()
  {
    TagNames.H3,
    TagNames.H2,
    TagNames.H4,
    TagNames.H1,
    TagNames.H6,
    TagNames.H5
  };
  internal static readonly HashSet<string> AllBlocks = new HashSet<string>()
  {
    TagNames.Ol,
    TagNames.Ul,
    TagNames.Dl,
    TagNames.Fieldset,
    TagNames.Button,
    TagNames.Figcaption,
    TagNames.Figure,
    TagNames.Article,
    TagNames.Aside,
    TagNames.BlockQuote,
    TagNames.Center,
    TagNames.Address,
    TagNames.Dialog,
    TagNames.Dir,
    TagNames.Summary,
    TagNames.Details,
    TagNames.Listing,
    TagNames.Footer,
    TagNames.Header,
    TagNames.Nav,
    TagNames.Section,
    TagNames.Menu,
    TagNames.Hgroup,
    TagNames.Main,
    TagNames.Pre
  };
  internal static readonly HashSet<string> AllBody = new HashSet<string>()
  {
    TagNames.Ol,
    TagNames.Dl,
    TagNames.Fieldset,
    TagNames.Figcaption,
    TagNames.Figure,
    TagNames.Article,
    TagNames.Aside,
    TagNames.BlockQuote,
    TagNames.Center,
    TagNames.Address,
    TagNames.Dialog,
    TagNames.Dir,
    TagNames.Summary,
    TagNames.Details,
    TagNames.Main,
    TagNames.Footer,
    TagNames.Header,
    TagNames.Nav,
    TagNames.Section,
    TagNames.Menu,
    TagNames.Hgroup
  };
  internal static readonly HashSet<string> AllBodyObsolete = new HashSet<string>()
  {
    TagNames.Applet,
    TagNames.Marquee,
    TagNames.Object
  };
  internal static readonly HashSet<string> AllInput = new HashSet<string>()
  {
    TagNames.Input,
    TagNames.Keygen,
    TagNames.Textarea
  };
  internal static readonly HashSet<string> AllBasicBlocks = new HashSet<string>()
  {
    TagNames.Address,
    TagNames.Div,
    TagNames.P
  };
  internal static readonly HashSet<string> AllSemanticFormatting = new HashSet<string>()
  {
    TagNames.B,
    TagNames.Strong,
    TagNames.Code,
    TagNames.Em,
    TagNames.U,
    TagNames.I
  };
  internal static readonly HashSet<string> AllClassicFormatting = new HashSet<string>()
  {
    TagNames.Font,
    TagNames.S,
    TagNames.Small,
    TagNames.Strike,
    TagNames.Big,
    TagNames.Tt
  };
  internal static readonly HashSet<string> AllFormatting = new HashSet<string>()
  {
    TagNames.B,
    TagNames.Strong,
    TagNames.Code,
    TagNames.Em,
    TagNames.U,
    TagNames.I,
    TagNames.NoBr,
    TagNames.Font,
    TagNames.S,
    TagNames.Small,
    TagNames.Strike,
    TagNames.Big,
    TagNames.Tt
  };
  internal static readonly HashSet<string> AllNested = new HashSet<string>()
  {
    TagNames.Tbody,
    TagNames.Td,
    TagNames.Tfoot,
    TagNames.Th,
    TagNames.Thead,
    TagNames.Tr,
    TagNames.Caption,
    TagNames.Col,
    TagNames.Colgroup,
    TagNames.Frame,
    TagNames.Head
  };
  internal static readonly HashSet<string> AllCaptionEnd = new HashSet<string>()
  {
    TagNames.Tbody,
    TagNames.Col,
    TagNames.Tfoot,
    TagNames.Td,
    TagNames.Thead,
    TagNames.Caption,
    TagNames.Th,
    TagNames.Colgroup,
    TagNames.Tr
  };
  internal static readonly HashSet<string> AllCaptionStart = new HashSet<string>()
  {
    TagNames.Tbody,
    TagNames.Col,
    TagNames.Tfoot,
    TagNames.Td,
    TagNames.Thead,
    TagNames.Tr,
    TagNames.Body,
    TagNames.Th,
    TagNames.Colgroup,
    TagNames.Html
  };
  internal static readonly HashSet<string> AllTable = new HashSet<string>()
  {
    TagNames.Tbody,
    TagNames.Col,
    TagNames.Tfoot,
    TagNames.Td,
    TagNames.Thead,
    TagNames.Tr
  };
  internal static readonly HashSet<string> AllTableRoot = new HashSet<string>()
  {
    TagNames.Caption,
    TagNames.Colgroup,
    TagNames.Tbody,
    TagNames.Tfoot,
    TagNames.Thead
  };
  internal static readonly HashSet<string> AllTableGeneral = new HashSet<string>()
  {
    TagNames.Caption,
    TagNames.Colgroup,
    TagNames.Col,
    TagNames.Tbody,
    TagNames.Tfoot,
    TagNames.Thead
  };
  internal static readonly HashSet<string> AllTableSections = new HashSet<string>()
  {
    TagNames.Tbody,
    TagNames.Tfoot,
    TagNames.Thead
  };
  internal static readonly HashSet<string> AllTableMajor = new HashSet<string>()
  {
    TagNames.Tbody,
    TagNames.Tfoot,
    TagNames.Thead,
    TagNames.Table,
    TagNames.Tr
  };
  internal static readonly HashSet<string> AllTableSpecial = new HashSet<string>()
  {
    TagNames.Td,
    TagNames.Th,
    TagNames.Body,
    TagNames.Caption,
    TagNames.Col,
    TagNames.Colgroup,
    TagNames.Html
  };
  internal static readonly HashSet<string> AllTableCore = new HashSet<string>()
  {
    TagNames.Tr,
    TagNames.Table,
    TagNames.Tbody,
    TagNames.Tfoot,
    TagNames.Thead
  };
  internal static readonly HashSet<string> AllTableInner = new HashSet<string>()
  {
    TagNames.Tbody,
    TagNames.Tr,
    TagNames.Thead,
    TagNames.Th,
    TagNames.Tfoot,
    TagNames.Td
  };
  internal static readonly HashSet<string> AllTableSelects = new HashSet<string>()
  {
    TagNames.Tr,
    TagNames.Table,
    TagNames.Tbody,
    TagNames.Tfoot,
    TagNames.Thead,
    TagNames.Td,
    TagNames.Th,
    TagNames.Caption
  };
  internal static readonly HashSet<string> AllTableCells = new HashSet<string>()
  {
    TagNames.Td,
    TagNames.Th
  };
  internal static readonly HashSet<string> AllTableCellsRows = new HashSet<string>()
  {
    TagNames.Tr,
    TagNames.Td,
    TagNames.Th
  };
  internal static readonly HashSet<string> AllTableHead = new HashSet<string>()
  {
    TagNames.Script,
    TagNames.Style,
    TagNames.Template
  };
}

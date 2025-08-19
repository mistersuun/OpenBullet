// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Parser.HtmlTagNameLookup
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using System.Text;

#nullable disable
namespace AngleSharp.Html.Parser;

internal static class HtmlTagNameLookup
{
  public static string TryGetWellKnownTagName(StringBuilder builder)
  {
    switch (builder.Length)
    {
      case 1:
        switch (builder[0])
        {
          case 'a':
            return TagNames.A;
          case 'b':
            return TagNames.B;
          case 'i':
            return TagNames.I;
          case 'p':
            return TagNames.P;
          case 'q':
            return TagNames.Q;
          case 's':
            return TagNames.S;
          case 'u':
            return TagNames.U;
          default:
            return (string) null;
        }
      case 2:
        switch (builder[1])
        {
          case '1':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.H1) ? (string) null : TagNames.H1;
          case '2':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.H2) ? (string) null : TagNames.H2;
          case '3':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.H3) ? (string) null : TagNames.H3;
          case '4':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.H4) ? (string) null : TagNames.H4;
          case '5':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.H5) ? (string) null : TagNames.H5;
          case '6':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.H6) ? (string) null : TagNames.H6;
          case 'b':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Rb) ? (string) null : TagNames.Rb;
          case 'd':
            switch (builder[0])
            {
              case 'd':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Dd) ? (string) null : TagNames.Dd;
              case 't':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Td) ? (string) null : TagNames.Td;
              default:
                return (string) null;
            }
          case 'h':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Th) ? (string) null : TagNames.Th;
          case 'i':
            switch (builder[0])
            {
              case 'l':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Li) ? (string) null : TagNames.Li;
              case 'm':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Mi) ? (string) null : TagNames.Mi;
              default:
                return (string) null;
            }
          case 'l':
            switch (builder[0])
            {
              case 'd':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Dl) ? (string) null : TagNames.Dl;
              case 'o':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Ol) ? (string) null : TagNames.Ol;
              case 'u':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Ul) ? (string) null : TagNames.Ul;
              default:
                return (string) null;
            }
          case 'm':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Em) ? (string) null : TagNames.Em;
          case 'n':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Mn) ? (string) null : TagNames.Mn;
          case 'o':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Mo) ? (string) null : TagNames.Mo;
          case 'p':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Rp) ? (string) null : TagNames.Rp;
          case 'r':
            switch (builder[0])
            {
              case 'b':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Br) ? (string) null : TagNames.Br;
              case 'h':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Hr) ? (string) null : TagNames.Hr;
              case 't':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Tr) ? (string) null : TagNames.Tr;
              default:
                return (string) null;
            }
          case 's':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Ms) ? (string) null : TagNames.Ms;
          case 't':
            switch (builder[0])
            {
              case 'd':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Dt) ? (string) null : TagNames.Dt;
              case 'r':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Rt) ? (string) null : TagNames.Rt;
              case 't':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Tt) ? (string) null : TagNames.Tt;
              default:
                return (string) null;
            }
          default:
            return (string) null;
        }
      case 3:
        switch (builder[0])
        {
          case 'b':
            switch (builder[2])
            {
              case 'g':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Big) ? (string) null : TagNames.Big;
              case 'i':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Bdi) ? (string) null : TagNames.Bdi;
              case 'o':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Bdo) ? (string) null : TagNames.Bdo;
              default:
                return (string) null;
            }
          case 'c':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Col) ? (string) null : TagNames.Col;
          case 'd':
            switch (builder[2])
            {
              case 'l':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Del) ? (string) null : TagNames.Del;
              case 'n':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Dfn) ? (string) null : TagNames.Dfn;
              case 'r':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Dir) ? (string) null : TagNames.Dir;
              case 'v':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Div) ? (string) null : TagNames.Div;
              default:
                return (string) null;
            }
          case 'i':
            switch (builder[1])
            {
              case 'm':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Img) ? (string) null : TagNames.Img;
              case 'n':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Ins) ? (string) null : TagNames.Ins;
              default:
                return (string) null;
            }
          case 'k':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Kbd) ? (string) null : TagNames.Kbd;
          case 'm':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Map) ? (string) null : TagNames.Map;
          case 'n':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Nav) ? (string) null : TagNames.Nav;
          case 'p':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Pre) ? (string) null : TagNames.Pre;
          case 'r':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Rtc) ? (string) null : TagNames.Rtc;
          case 's':
            switch (builder[2])
            {
              case 'b':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Sub) ? (string) null : TagNames.Sub;
              case 'g':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Svg) ? (string) null : TagNames.Svg;
              case 'p':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Sup) ? (string) null : TagNames.Sup;
              default:
                return (string) null;
            }
          case 'v':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Var) ? (string) null : TagNames.Var;
          case 'w':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Wbr) ? (string) null : TagNames.Wbr;
          case 'x':
            switch (builder[2])
            {
              case 'l':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Xml) ? (string) null : TagNames.Xml;
              case 'p':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Xmp) ? (string) null : TagNames.Xmp;
              default:
                return (string) null;
            }
          default:
            return (string) null;
        }
      case 4:
        switch (builder[3])
        {
          case 'a':
            switch (builder[0])
            {
              case 'a':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Area) ? (string) null : TagNames.Area;
              case 'd':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Data) ? (string) null : TagNames.Data;
              case 'm':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Meta) ? (string) null : TagNames.Meta;
              default:
                return (string) null;
            }
          case 'c':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Desc) ? (string) null : TagNames.Desc;
          case 'd':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Head) ? (string) null : TagNames.Head;
          case 'e':
            switch (builder[2])
            {
              case 'd':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Code) ? (string) null : TagNames.Code;
              case 'm':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Time) ? (string) null : TagNames.Time;
              case 's':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Base) ? (string) null : TagNames.Base;
              case 't':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Cite) ? (string) null : TagNames.Cite;
              default:
                return (string) null;
            }
          case 'h':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Math) ? (string) null : TagNames.Math;
          case 'k':
            switch (builder[0])
            {
              case 'l':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Link) ? (string) null : TagNames.Link;
              case 'm':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Mark) ? (string) null : TagNames.Mark;
              default:
                return (string) null;
            }
          case 'l':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Html) ? (string) null : TagNames.Html;
          case 'm':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Form) ? (string) null : TagNames.Form;
          case 'n':
            switch (builder[0])
            {
              case 'm':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Main) ? (string) null : TagNames.Main;
              case 's':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Span) ? (string) null : TagNames.Span;
              default:
                return (string) null;
            }
          case 'p':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Samp) ? (string) null : TagNames.Samp;
          case 'r':
            switch (builder[0])
            {
              case 'a':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Abbr) ? (string) null : TagNames.Abbr;
              case 'n':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.NoBr) ? (string) null : TagNames.NoBr;
              default:
                return (string) null;
            }
          case 't':
            switch (builder[0])
            {
              case 'f':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Font) ? (string) null : TagNames.Font;
              case 's':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Slot) ? (string) null : TagNames.Slot;
              default:
                return (string) null;
            }
          case 'u':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Menu) ? (string) null : TagNames.Menu;
          case 'y':
            switch (builder[0])
            {
              case 'b':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Body) ? (string) null : TagNames.Body;
              case 'r':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Ruby) ? (string) null : TagNames.Ruby;
              default:
                return (string) null;
            }
          default:
            return (string) null;
        }
      case 5:
        switch (builder[1])
        {
          case 'a':
            switch (builder[0])
            {
              case 'l':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Label) ? (string) null : TagNames.Label;
              case 'p':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Param) ? (string) null : TagNames.Param;
              case 't':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Table) ? (string) null : TagNames.Table;
              default:
                return (string) null;
            }
          case 'b':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Tbody) ? (string) null : TagNames.Tbody;
          case 'e':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Meter) ? (string) null : TagNames.Meter;
          case 'f':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Tfoot) ? (string) null : TagNames.Tfoot;
          case 'h':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Thead) ? (string) null : TagNames.Thead;
          case 'i':
            switch (builder[0])
            {
              case 't':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Title) ? (string) null : TagNames.Title;
              case 'v':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Video) ? (string) null : TagNames.Video;
              default:
                return (string) null;
            }
          case 'm':
            switch (builder[0])
            {
              case 'e':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Embed) ? (string) null : TagNames.Embed;
              case 'i':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Image) ? (string) null : TagNames.Image;
              case 's':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Small) ? (string) null : TagNames.Small;
              default:
                return (string) null;
            }
          case 'n':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Input) ? (string) null : TagNames.Input;
          case 'r':
            switch (builder[0])
            {
              case 'f':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Frame) ? (string) null : TagNames.Frame;
              case 't':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Track) ? (string) null : TagNames.Track;
              default:
                return (string) null;
            }
          case 's':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Aside) ? (string) null : TagNames.Aside;
          case 't':
            switch (builder[0])
            {
              case 'm':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Mtext) ? (string) null : TagNames.Mtext;
              case 's':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Style) ? (string) null : TagNames.Style;
              default:
                return (string) null;
            }
          case 'u':
            switch (builder[0])
            {
              case 'a':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Audio) ? (string) null : TagNames.Audio;
              case 'q':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Quote) ? (string) null : TagNames.Quote;
              default:
                return (string) null;
            }
          default:
            return (string) null;
        }
      case 6:
        switch (builder[3])
        {
          case 'a':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Iframe) ? (string) null : TagNames.Iframe;
          case 'c':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Circle) ? (string) null : TagNames.Circle;
          case 'd':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Header) ? (string) null : TagNames.Header;
          case 'e':
            switch (builder[0])
            {
              case 'l':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Legend) ? (string) null : TagNames.Legend;
              case 'o':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Object) ? (string) null : TagNames.Object;
              case 's':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Select) ? (string) null : TagNames.Select;
              default:
                return (string) null;
            }
          case 'g':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Keygen) ? (string) null : TagNames.Keygen;
          case 'i':
            switch (builder[1])
            {
              case 'c':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Script) ? (string) null : TagNames.Script;
              case 'p':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Option) ? (string) null : TagNames.Option;
              case 't':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Strike) ? (string) null : TagNames.Strike;
              default:
                return (string) null;
            }
          case 'l':
            switch (builder[0])
            {
              case 'a':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Applet) ? (string) null : TagNames.Applet;
              case 'd':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Dialog) ? (string) null : TagNames.Dialog;
              default:
                return (string) null;
            }
          case 'o':
            switch (builder[0])
            {
              case 'h':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Hgroup) ? (string) null : TagNames.Hgroup;
              case 's':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Strong) ? (string) null : TagNames.Strong;
              default:
                return (string) null;
            }
          case 'p':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Output) ? (string) null : TagNames.Output;
          case 'r':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Source) ? (string) null : TagNames.Source;
          case 't':
            switch (builder[0])
            {
              case 'b':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Button) ? (string) null : TagNames.Button;
              case 'c':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Center) ? (string) null : TagNames.Center;
              case 'f':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Footer) ? (string) null : TagNames.Footer;
              default:
                return (string) null;
            }
          case 'u':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Figure) ? (string) null : TagNames.Figure;
          case 'v':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Canvas) ? (string) null : TagNames.Canvas;
          default:
            return (string) null;
        }
      case 7:
        switch (builder[0])
        {
          case 'D':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Doctype) ? (string) null : TagNames.Doctype;
          case 'a':
            switch (builder[1])
            {
              case 'd':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Address) ? (string) null : TagNames.Address;
              case 'r':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Article) ? (string) null : TagNames.Article;
              default:
                return (string) null;
            }
          case 'b':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Bgsound) ? (string) null : TagNames.Bgsound;
          case 'c':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Caption) ? (string) null : TagNames.Caption;
          case 'd':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Details) ? (string) null : TagNames.Details;
          case 'i':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.IsIndex) ? (string) null : TagNames.IsIndex;
          case 'l':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Listing) ? (string) null : TagNames.Listing;
          case 'm':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Marquee) ? (string) null : TagNames.Marquee;
          case 'n':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.NoEmbed) ? (string) null : TagNames.NoEmbed;
          case 'p':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Picture) ? (string) null : TagNames.Picture;
          case 's':
            switch (builder[1])
            {
              case 'e':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Section) ? (string) null : TagNames.Section;
              case 'u':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Summary) ? (string) null : TagNames.Summary;
              default:
                return (string) null;
            }
          default:
            return (string) null;
        }
      case 8:
        switch (builder[2])
        {
          case 'a':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Frameset) ? (string) null : TagNames.Frameset;
          case 'e':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Fieldset) ? (string) null : TagNames.Fieldset;
          case 'f':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.NoFrames) ? (string) null : TagNames.NoFrames;
          case 'l':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Colgroup) ? (string) null : TagNames.Colgroup;
          case 'm':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Template) ? (string) null : TagNames.Template;
          case 'n':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.MenuItem) ? (string) null : TagNames.MenuItem;
          case 'o':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Progress) ? (string) null : TagNames.Progress;
          case 's':
            switch (builder[0])
            {
              case 'b':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.BaseFont) ? (string) null : TagNames.BaseFont;
              case 'n':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.NoScript) ? (string) null : TagNames.NoScript;
              default:
                return (string) null;
            }
          case 't':
            switch (builder[0])
            {
              case 'd':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Datalist) ? (string) null : TagNames.Datalist;
              case 'o':
                return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Optgroup) ? (string) null : TagNames.Optgroup;
              default:
                return (string) null;
            }
          case 'x':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Textarea) ? (string) null : TagNames.Textarea;
          default:
            return (string) null;
        }
      case 9:
        return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Plaintext) ? (string) null : TagNames.Plaintext;
      case 10:
        switch (builder[0])
        {
          case 'b':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.BlockQuote) ? (string) null : TagNames.BlockQuote;
          case 'f':
            return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.Figcaption) ? (string) null : TagNames.Figcaption;
          default:
            return (string) null;
        }
      case 13:
        return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.ForeignObject) ? (string) null : TagNames.ForeignObject;
      case 14:
        return !HtmlTagNameLookup.CharsAreEqual(builder, TagNames.AnnotationXml) ? (string) null : TagNames.AnnotationXml;
      default:
        return (string) null;
    }
  }

  private static bool CharsAreEqual(StringBuilder builder, string tagName)
  {
    for (int index = 0; index < tagName.Length; ++index)
    {
      if ((int) tagName[index] != (int) builder[index])
        return false;
    }
    return true;
  }
}

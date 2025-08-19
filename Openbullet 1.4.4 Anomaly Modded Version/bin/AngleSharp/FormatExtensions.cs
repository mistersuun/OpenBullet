// Decompiled with JetBrains decompiler
// Type: AngleSharp.FormatExtensions
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Css;
using AngleSharp.Html;
using AngleSharp.Text;
using System.IO;
using System.Text;

#nullable disable
namespace AngleSharp;

public static class FormatExtensions
{
  public static string ToCss(this IStyleFormattable style)
  {
    return style.ToCss(CssStyleFormatter.Instance);
  }

  public static string ToCss(this IStyleFormattable style, IStyleFormatter formatter)
  {
    StringBuilder sb = StringBuilderPool.Obtain();
    using (StringWriter writer = new StringWriter(sb))
      style.ToCss((TextWriter) writer, formatter);
    return sb.ToPool();
  }

  public static void ToCss(this IStyleFormattable style, TextWriter writer)
  {
    style.ToCss(writer, CssStyleFormatter.Instance);
  }

  public static string ToHtml(this IMarkupFormattable markup)
  {
    return markup.ToHtml(HtmlMarkupFormatter.Instance);
  }

  public static string ToHtml(this IMarkupFormattable markup, IMarkupFormatter formatter)
  {
    StringBuilder sb = StringBuilderPool.Obtain();
    using (StringWriter writer = new StringWriter(sb))
      markup.ToHtml((TextWriter) writer, formatter);
    return sb.ToPool();
  }

  public static void ToHtml(this IMarkupFormattable markup, TextWriter writer)
  {
    markup.ToHtml(writer, HtmlMarkupFormatter.Instance);
  }

  public static string Minify(this IMarkupFormattable markup)
  {
    return markup.ToHtml((IMarkupFormatter) new MinifyMarkupFormatter());
  }

  public static string Prettify(this IMarkupFormattable markup)
  {
    return markup.ToHtml((IMarkupFormatter) new PrettyMarkupFormatter());
  }
}

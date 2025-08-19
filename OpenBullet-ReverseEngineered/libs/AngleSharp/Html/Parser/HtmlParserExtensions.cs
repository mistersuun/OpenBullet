// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Parser.HtmlParserExtensions
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace AngleSharp.Html.Parser;

public static class HtmlParserExtensions
{
  public static Task<IHtmlDocument> ParseDocumentAsync(this IHtmlParser parser, string source)
  {
    return parser.ParseDocumentAsync(source, CancellationToken.None);
  }

  public static Task<IHtmlDocument> ParseDocumentAsync(this IHtmlParser parser, Stream source)
  {
    return parser.ParseDocumentAsync(source, CancellationToken.None);
  }

  public static Task<IDocument> ParseDocumentAsync(this IHtmlParser parser, IDocument document)
  {
    return parser.ParseDocumentAsync(document, CancellationToken.None);
  }
}

// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Parser.IHtmlParser
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Browser;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace AngleSharp.Html.Parser;

public interface IHtmlParser : IParser, IEventTarget
{
  IHtmlDocument ParseDocument(string source);

  IHtmlDocument ParseDocument(Stream source);

  INodeList ParseFragment(string source, IElement contextElement);

  Task<IHtmlDocument> ParseDocumentAsync(string source, CancellationToken cancel);

  Task<IHtmlDocument> ParseDocumentAsync(Stream source, CancellationToken cancel);

  Task<IDocument> ParseDocumentAsync(IDocument document, CancellationToken cancel);
}

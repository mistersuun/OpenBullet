// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Parser.TokenizerExtensions
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Html.Dom.Events;
using AngleSharp.Html.Parser.Tokens;
using AngleSharp.Text;
using System;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Html.Parser;

public static class TokenizerExtensions
{
  public static IEnumerable<HtmlToken> Tokenize(
    this TextSource source,
    IEntityProvider provider = null,
    EventHandler<HtmlErrorEvent> errorHandler = null)
  {
    HtmlTokenizer htmlTokenizer = new HtmlTokenizer(source, provider ?? HtmlEntityProvider.Resolver);
    HtmlToken token = (HtmlToken) null;
    if (errorHandler != null)
      htmlTokenizer.Error += errorHandler;
    do
    {
      token = htmlTokenizer.Get();
      yield return token;
    }
    while (token.Type != HtmlTokenType.EndOfFile);
  }
}

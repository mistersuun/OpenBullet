// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Search.RegexSearchStrategy
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Documents;

#nullable disable
namespace ICSharpCode.AvalonEdit.Search;

internal class RegexSearchStrategy : ISearchStrategy, IEquatable<ISearchStrategy>
{
  private readonly Regex searchPattern;
  private readonly bool matchWholeWords;

  public RegexSearchStrategy(Regex searchPattern, bool matchWholeWords)
  {
    this.searchPattern = searchPattern != null ? searchPattern : throw new ArgumentNullException(nameof (searchPattern));
    this.matchWholeWords = matchWholeWords;
  }

  public IEnumerable<ISearchResult> FindAll(ITextSource document, int offset, int length)
  {
    int endOffset = offset + length;
    foreach (Match result in this.searchPattern.Matches(document.Text))
    {
      int resultEndOffset = result.Length + result.Index;
      if (offset <= result.Index && endOffset >= resultEndOffset && (!this.matchWholeWords || RegexSearchStrategy.IsWordBorder(document, result.Index) && RegexSearchStrategy.IsWordBorder(document, resultEndOffset)))
      {
        SearchResult searchResult = new SearchResult();
        searchResult.StartOffset = result.Index;
        searchResult.Length = result.Length;
        searchResult.Data = result;
        yield return (ISearchResult) searchResult;
      }
    }
  }

  private static bool IsWordBorder(ITextSource document, int offset)
  {
    return TextUtilities.GetNextCaretPosition(document, offset - 1, LogicalDirection.Forward, CaretPositioningMode.WordBorder) == offset;
  }

  public ISearchResult FindNext(ITextSource document, int offset, int length)
  {
    return this.FindAll(document, offset, length).FirstOrDefault<ISearchResult>();
  }

  public bool Equals(ISearchStrategy other)
  {
    return other is RegexSearchStrategy regexSearchStrategy && regexSearchStrategy.searchPattern.ToString() == this.searchPattern.ToString() && regexSearchStrategy.searchPattern.Options == this.searchPattern.Options && regexSearchStrategy.searchPattern.RightToLeft == this.searchPattern.RightToLeft;
  }
}

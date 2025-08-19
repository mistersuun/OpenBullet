// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Search.SearchResult
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using System.Text.RegularExpressions;

#nullable disable
namespace ICSharpCode.AvalonEdit.Search;

internal class SearchResult : TextSegment, ISearchResult, ISegment
{
  public Match Data { get; set; }

  public string ReplaceWith(string replacement) => this.Data.Result(replacement);
}

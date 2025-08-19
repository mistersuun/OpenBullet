// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Search.SearchStrategyFactory
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Text;
using System.Text.RegularExpressions;

#nullable disable
namespace ICSharpCode.AvalonEdit.Search;

public static class SearchStrategyFactory
{
  public static ISearchStrategy Create(
    string searchPattern,
    bool ignoreCase,
    bool matchWholeWords,
    SearchMode mode)
  {
    if (searchPattern == null)
      throw new ArgumentNullException(nameof (searchPattern));
    RegexOptions options = RegexOptions.Multiline | RegexOptions.Compiled;
    if (ignoreCase)
      options |= RegexOptions.IgnoreCase;
    switch (mode)
    {
      case SearchMode.Normal:
        searchPattern = Regex.Escape(searchPattern);
        break;
      case SearchMode.Wildcard:
        searchPattern = SearchStrategyFactory.ConvertWildcardsToRegex(searchPattern);
        break;
    }
    try
    {
      return (ISearchStrategy) new RegexSearchStrategy(new Regex(searchPattern, options), matchWholeWords);
    }
    catch (ArgumentException ex)
    {
      throw new SearchPatternException(ex.Message, (Exception) ex);
    }
  }

  private static string ConvertWildcardsToRegex(string searchPattern)
  {
    if (string.IsNullOrEmpty(searchPattern))
      return "";
    StringBuilder stringBuilder = new StringBuilder();
    foreach (char ch in searchPattern)
    {
      switch (ch)
      {
        case '*':
          stringBuilder.Append(".*");
          break;
        case '?':
          stringBuilder.Append(".");
          break;
        default:
          stringBuilder.Append(Regex.Escape(ch.ToString()));
          break;
      }
    }
    return stringBuilder.ToString();
  }
}

// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Search.SearchOptionsChangedEventArgs
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;

#nullable disable
namespace ICSharpCode.AvalonEdit.Search;

public class SearchOptionsChangedEventArgs : EventArgs
{
  public string SearchPattern { get; private set; }

  public bool MatchCase { get; private set; }

  public bool UseRegex { get; private set; }

  public bool WholeWords { get; private set; }

  public SearchOptionsChangedEventArgs(
    string searchPattern,
    bool matchCase,
    bool useRegex,
    bool wholeWords)
  {
    this.SearchPattern = searchPattern;
    this.MatchCase = matchCase;
    this.UseRegex = useRegex;
    this.WholeWords = wholeWords;
  }
}

// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Rendering.LinkElementGenerator
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Text.RegularExpressions;

#nullable disable
namespace ICSharpCode.AvalonEdit.Rendering;

public class LinkElementGenerator : VisualLineElementGenerator, IBuiltinElementGenerator
{
  internal static readonly Regex defaultLinkRegex = new Regex("\\b(https?://|ftp://|www\\.)[\\w\\d\\._/\\-~%@()+:?&=#!]*[\\w\\d/]");
  internal static readonly Regex defaultMailRegex = new Regex("\\b[\\w\\d\\.\\-]+\\@[\\w\\d\\.\\-]+\\.[a-z]{2,6}\\b");
  private readonly Regex linkRegex;

  public bool RequireControlModifierForClick { get; set; }

  public LinkElementGenerator()
  {
    this.linkRegex = LinkElementGenerator.defaultLinkRegex;
    this.RequireControlModifierForClick = true;
  }

  protected LinkElementGenerator(Regex regex)
    : this()
  {
    this.linkRegex = regex != null ? regex : throw new ArgumentNullException(nameof (regex));
  }

  void IBuiltinElementGenerator.FetchOptions(TextEditorOptions options)
  {
    this.RequireControlModifierForClick = options.RequireControlModifierForHyperlinkClick;
  }

  private Match GetMatch(int startOffset, out int matchOffset)
  {
    int endOffset = this.CurrentContext.VisualLine.LastDocumentLine.EndOffset;
    StringSegment text = this.CurrentContext.GetText(startOffset, endOffset - startOffset);
    Match match = this.linkRegex.Match(text.Text, text.Offset, text.Count);
    matchOffset = match.Success ? match.Index - text.Offset + startOffset : -1;
    return match;
  }

  public override int GetFirstInterestedOffset(int startOffset)
  {
    int matchOffset;
    this.GetMatch(startOffset, out matchOffset);
    return matchOffset;
  }

  public override VisualLineElement ConstructElement(int offset)
  {
    int matchOffset;
    Match match = this.GetMatch(offset, out matchOffset);
    return match.Success && matchOffset == offset ? this.ConstructElementFromMatch(match) : (VisualLineElement) null;
  }

  protected virtual VisualLineElement ConstructElementFromMatch(Match m)
  {
    Uri uriFromMatch = this.GetUriFromMatch(m);
    if (uriFromMatch == (Uri) null)
      return (VisualLineElement) null;
    return (VisualLineElement) new VisualLineLinkText(this.CurrentContext.VisualLine, m.Length)
    {
      NavigateUri = uriFromMatch,
      RequireControlModifierForClick = this.RequireControlModifierForClick
    };
  }

  protected virtual Uri GetUriFromMatch(Match match)
  {
    string uriString = match.Value;
    if (uriString.StartsWith("www.", StringComparison.Ordinal))
      uriString = "http://" + uriString;
    return Uri.IsWellFormedUriString(uriString, UriKind.Absolute) ? new Uri(uriString) : (Uri) null;
  }
}

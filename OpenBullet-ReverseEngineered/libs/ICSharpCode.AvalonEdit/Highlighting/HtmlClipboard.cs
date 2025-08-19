// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Highlighting.HtmlClipboard
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using System;
using System.Globalization;
using System.Text;
using System.Windows;

#nullable disable
namespace ICSharpCode.AvalonEdit.Highlighting;

public static class HtmlClipboard
{
  private static string BuildHeader(
    int startHTML,
    int endHTML,
    int startFragment,
    int endFragment)
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.AppendLine("Version:0.9");
    stringBuilder.AppendLine("StartHTML:" + startHTML.ToString("d8", (IFormatProvider) CultureInfo.InvariantCulture));
    stringBuilder.AppendLine("EndHTML:" + endHTML.ToString("d8", (IFormatProvider) CultureInfo.InvariantCulture));
    stringBuilder.AppendLine("StartFragment:" + startFragment.ToString("d8", (IFormatProvider) CultureInfo.InvariantCulture));
    stringBuilder.AppendLine("EndFragment:" + endFragment.ToString("d8", (IFormatProvider) CultureInfo.InvariantCulture));
    return stringBuilder.ToString();
  }

  public static void SetHtml(DataObject dataObject, string htmlFragment)
  {
    if (dataObject == null)
      throw new ArgumentNullException(nameof (dataObject));
    if (htmlFragment == null)
      throw new ArgumentNullException(nameof (htmlFragment));
    string str1 = $"<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">{Environment.NewLine}<HTML>{Environment.NewLine}<BODY>{Environment.NewLine}<!--StartFragment-->{Environment.NewLine}";
    string str2 = $"<!--EndFragment-->{Environment.NewLine}</BODY>{Environment.NewLine}</HTML>{Environment.NewLine}";
    int length = HtmlClipboard.BuildHeader(0, 0, 0, 0).Length;
    int startFragment = length + str1.Length;
    int endFragment = startFragment + Encoding.UTF8.GetByteCount(htmlFragment);
    int endHTML = endFragment + str2.Length;
    string textData = HtmlClipboard.BuildHeader(length, endHTML, startFragment, endFragment) + str1 + htmlFragment + str2;
    dataObject.SetText(textData, TextDataFormat.Html);
  }

  public static string CreateHtmlFragment(
    IDocument document,
    IHighlighter highlighter,
    ISegment segment,
    HtmlOptions options)
  {
    if (document == null)
      throw new ArgumentNullException(nameof (document));
    if (options == null)
      throw new ArgumentNullException(nameof (options));
    if (highlighter != null && highlighter.Document != document)
      throw new ArgumentException("Highlighter does not belong to the specified document.");
    if (segment == null)
      segment = (ISegment) new SimpleSegment(0, document.TextLength);
    StringBuilder stringBuilder = new StringBuilder();
    int endOffset = segment.EndOffset;
    for (IDocumentLine documentLine = document.GetLineByOffset(segment.Offset); documentLine != null && documentLine.Offset < endOffset; documentLine = documentLine.NextLine)
    {
      HighlightedLine highlightedLine = highlighter == null ? new HighlightedLine(document, documentLine) : highlighter.HighlightLine(documentLine.LineNumber);
      SimpleSegment overlap = SimpleSegment.GetOverlap(segment, (ISegment) documentLine);
      if (stringBuilder.Length > 0)
        stringBuilder.AppendLine("<br>");
      stringBuilder.Append(highlightedLine.ToHtml(overlap.Offset, overlap.EndOffset, options));
    }
    return stringBuilder.ToString();
  }
}

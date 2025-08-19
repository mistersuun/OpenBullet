// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Editing.Selection
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

#nullable disable
namespace ICSharpCode.AvalonEdit.Editing;

public abstract class Selection
{
  internal readonly TextArea textArea;

  public static Selection Create(TextArea textArea, int startOffset, int endOffset)
  {
    if (textArea == null)
      throw new ArgumentNullException(nameof (textArea));
    return startOffset == endOffset ? textArea.emptySelection : (Selection) new SimpleSelection(textArea, new TextViewPosition(textArea.Document.GetLocation(startOffset)), new TextViewPosition(textArea.Document.GetLocation(endOffset)));
  }

  internal static Selection Create(TextArea textArea, TextViewPosition start, TextViewPosition end)
  {
    if (textArea == null)
      throw new ArgumentNullException(nameof (textArea));
    return textArea.Document.GetOffset(start.Location) == textArea.Document.GetOffset(end.Location) && start.VisualColumn == end.VisualColumn ? textArea.emptySelection : (Selection) new SimpleSelection(textArea, start, end);
  }

  public static Selection Create(TextArea textArea, ISegment segment)
  {
    if (segment == null)
      throw new ArgumentNullException(nameof (segment));
    return Selection.Create(textArea, segment.Offset, segment.EndOffset);
  }

  protected Selection(TextArea textArea)
  {
    this.textArea = textArea != null ? textArea : throw new ArgumentNullException(nameof (textArea));
  }

  public abstract TextViewPosition StartPosition { get; }

  public abstract TextViewPosition EndPosition { get; }

  public abstract IEnumerable<SelectionSegment> Segments { get; }

  public abstract ISegment SurroundingSegment { get; }

  public abstract void ReplaceSelectionWithText(string newText);

  internal string AddSpacesIfRequired(string newText, TextViewPosition start, TextViewPosition end)
  {
    if (this.EnableVirtualSpace && this.InsertVirtualSpaces(newText, start, end))
    {
      DocumentLine lineByNumber = this.textArea.Document.GetLineByNumber(start.Line);
      string text = this.textArea.Document.GetText((ISegment) lineByNumber);
      VisualLine constructVisualLine = this.textArea.TextView.GetOrConstructVisualLine(lineByNumber);
      int count1 = start.VisualColumn - constructVisualLine.VisualLengthWithEndOfLineMarker;
      if (count1 > 0)
      {
        string str = "";
        if (!this.textArea.Options.ConvertTabsToSpaces)
        {
          if (text.Trim('\t').Length == 0)
          {
            int count2 = count1 / this.textArea.Options.IndentationSize;
            str = new string('\t', count2);
            count1 -= count2 * this.textArea.Options.IndentationSize;
          }
        }
        return str + new string(' ', count1) + newText;
      }
    }
    return newText;
  }

  private bool InsertVirtualSpaces(string newText, TextViewPosition start, TextViewPosition end)
  {
    return (!string.IsNullOrEmpty(newText) || !this.IsInVirtualSpace(start) || !this.IsInVirtualSpace(end)) && newText != "\r\n" && newText != "\n" && newText != "\r";
  }

  private bool IsInVirtualSpace(TextViewPosition pos)
  {
    return pos.VisualColumn > this.textArea.TextView.GetOrConstructVisualLine(this.textArea.Document.GetLineByNumber(pos.Line)).VisualLength;
  }

  public abstract Selection UpdateOnDocumentChange(DocumentChangeEventArgs e);

  public virtual bool IsEmpty => this.Length == 0;

  public virtual bool EnableVirtualSpace => this.textArea.Options.EnableVirtualSpace;

  public abstract int Length { get; }

  public abstract Selection SetEndpoint(TextViewPosition endPosition);

  public abstract Selection StartSelectionOrSetEndpoint(
    TextViewPosition startPosition,
    TextViewPosition endPosition);

  public virtual bool IsMultiline
  {
    get
    {
      ISegment surroundingSegment = this.SurroundingSegment;
      if (surroundingSegment == null)
        return false;
      int offset1 = surroundingSegment.Offset;
      int offset2 = offset1 + surroundingSegment.Length;
      TextDocument document = this.textArea.Document;
      if (document == null)
        throw ThrowUtil.NoDocumentAssigned();
      return document.GetLineByOffset(offset1) != document.GetLineByOffset(offset2);
    }
  }

  public virtual string GetText()
  {
    TextDocument document = this.textArea.Document;
    if (document == null)
      throw ThrowUtil.NoDocumentAssigned();
    StringBuilder stringBuilder = (StringBuilder) null;
    string str = (string) null;
    foreach (ISegment segment in this.Segments)
    {
      if (str != null)
      {
        if (stringBuilder == null)
          stringBuilder = new StringBuilder(str);
        else
          stringBuilder.Append(str);
      }
      str = document.GetText(segment);
    }
    if (stringBuilder == null)
      return str ?? string.Empty;
    if (str != null)
      stringBuilder.Append(str);
    return stringBuilder.ToString();
  }

  public string CreateHtmlFragment(HtmlOptions options)
  {
    if (options == null)
      throw new ArgumentNullException(nameof (options));
    IHighlighter service = this.textArea.GetService(typeof (IHighlighter)) as IHighlighter;
    StringBuilder stringBuilder = new StringBuilder();
    bool flag = true;
    foreach (ISegment segment in this.Segments)
    {
      if (flag)
        flag = false;
      else
        stringBuilder.AppendLine("<br>");
      stringBuilder.Append(HtmlClipboard.CreateHtmlFragment((IDocument) this.textArea.Document, service, segment, options));
    }
    return stringBuilder.ToString();
  }

  public abstract override bool Equals(object obj);

  public abstract override int GetHashCode();

  public virtual bool Contains(int offset)
  {
    if (this.IsEmpty || !this.SurroundingSegment.Contains(offset, 0))
      return false;
    foreach (ISegment segment in this.Segments)
    {
      if (segment.Contains(offset, 0))
        return true;
    }
    return false;
  }

  public virtual DataObject CreateDataObject(TextArea textArea)
  {
    DataObject dataObject = new DataObject();
    string str = TextUtilities.NormalizeNewLines(this.GetText(), Environment.NewLine);
    if (EditingCommandHandler.ConfirmDataFormat(textArea, dataObject, DataFormats.UnicodeText))
      dataObject.SetText(str);
    if (EditingCommandHandler.ConfirmDataFormat(textArea, dataObject, typeof (string).FullName))
      dataObject.SetData(typeof (string).FullName, (object) str);
    if (EditingCommandHandler.ConfirmDataFormat(textArea, dataObject, DataFormats.Html))
      HtmlClipboard.SetHtml(dataObject, this.CreateHtmlFragment(new HtmlOptions(textArea.Options)));
    return dataObject;
  }
}

// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Folding.FoldingManager
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

#nullable disable
namespace ICSharpCode.AvalonEdit.Folding;

public class FoldingManager : IWeakEventListener
{
  internal readonly TextDocument document;
  internal readonly List<TextView> textViews = new List<TextView>();
  private readonly TextSegmentCollection<FoldingSection> foldings;
  private bool isFirstUpdate = true;

  public FoldingManager(TextDocument document)
  {
    this.document = document != null ? document : throw new ArgumentNullException(nameof (document));
    this.foldings = new TextSegmentCollection<FoldingSection>();
    document.VerifyAccess();
    WeakEventManagerBase<TextDocumentWeakEventManager.Changed, TextDocument>.AddListener(document, (IWeakEventListener) this);
  }

  protected virtual bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
  {
    if (!(managerType == typeof (TextDocumentWeakEventManager.Changed)))
      return false;
    this.OnDocumentChanged((DocumentChangeEventArgs) e);
    return true;
  }

  bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
  {
    return this.ReceiveWeakEvent(managerType, sender, e);
  }

  private void OnDocumentChanged(DocumentChangeEventArgs e)
  {
    this.foldings.UpdateOffsets(e);
    DocumentLine lineByOffset = this.document.GetLineByOffset(e.Offset + e.InsertionLength);
    int num = lineByOffset.Offset + lineByOffset.TotalLength;
    foreach (FoldingSection overlappingSegment in this.foldings.FindOverlappingSegments(e.Offset, num - e.Offset))
    {
      if (overlappingSegment.Length == 0)
        this.RemoveFolding(overlappingSegment);
      else
        overlappingSegment.ValidateCollapsedLineSections();
    }
  }

  internal void AddToTextView(TextView textView)
  {
    if (textView == null || this.textViews.Contains(textView))
      throw new ArgumentException();
    this.textViews.Add(textView);
    foreach (FoldingSection folding in this.foldings)
    {
      if (folding.collapsedSections != null)
      {
        Array.Resize<CollapsedLineSection>(ref folding.collapsedSections, this.textViews.Count);
        folding.ValidateCollapsedLineSections();
      }
    }
  }

  internal void RemoveFromTextView(TextView textView)
  {
    int index = this.textViews.IndexOf(textView);
    if (index < 0)
      throw new ArgumentException();
    this.textViews.RemoveAt(index);
    foreach (FoldingSection folding in this.foldings)
    {
      if (folding.collapsedSections != null)
      {
        CollapsedLineSection[] destinationArray = new CollapsedLineSection[this.textViews.Count];
        Array.Copy((Array) folding.collapsedSections, 0, (Array) destinationArray, 0, index);
        folding.collapsedSections[index].Uncollapse();
        Array.Copy((Array) folding.collapsedSections, index + 1, (Array) destinationArray, index, destinationArray.Length - index);
        folding.collapsedSections = destinationArray;
      }
    }
  }

  internal void Redraw()
  {
    foreach (TextView textView in this.textViews)
      textView.Redraw();
  }

  internal void Redraw(FoldingSection fs)
  {
    foreach (TextView textView in this.textViews)
      textView.Redraw((ISegment) fs);
  }

  public FoldingSection CreateFolding(int startOffset, int endOffset)
  {
    if (startOffset >= endOffset)
      throw new ArgumentException("startOffset must be less than endOffset");
    if (startOffset < 0 || endOffset > this.document.TextLength)
      throw new ArgumentException("Folding must be within document boundary");
    FoldingSection fs = new FoldingSection(this, startOffset, endOffset);
    this.foldings.Add(fs);
    this.Redraw(fs);
    return fs;
  }

  public void RemoveFolding(FoldingSection fs)
  {
    if (fs == null)
      throw new ArgumentNullException(nameof (fs));
    fs.IsFolded = false;
    this.foldings.Remove(fs);
    this.Redraw(fs);
  }

  public void Clear()
  {
    this.document.VerifyAccess();
    foreach (FoldingSection folding in this.foldings)
      folding.IsFolded = false;
    this.foldings.Clear();
    this.Redraw();
  }

  public IEnumerable<FoldingSection> AllFoldings => (IEnumerable<FoldingSection>) this.foldings;

  public int GetNextFoldedFoldingStart(int startOffset)
  {
    FoldingSection segment = this.foldings.FindFirstSegmentWithStartAfter(startOffset);
    while (segment != null && !segment.IsFolded)
      segment = this.foldings.GetNextSegment(segment);
    return segment == null ? -1 : segment.StartOffset;
  }

  public FoldingSection GetNextFolding(int startOffset)
  {
    return this.foldings.FindFirstSegmentWithStartAfter(startOffset);
  }

  public ReadOnlyCollection<FoldingSection> GetFoldingsAt(int startOffset)
  {
    List<FoldingSection> foldingSectionList = new List<FoldingSection>();
    for (FoldingSection segment = this.foldings.FindFirstSegmentWithStartAfter(startOffset); segment != null && segment.StartOffset == startOffset; segment = this.foldings.GetNextSegment(segment))
      foldingSectionList.Add(segment);
    return foldingSectionList.AsReadOnly();
  }

  public ReadOnlyCollection<FoldingSection> GetFoldingsContaining(int offset)
  {
    return this.foldings.FindSegmentsContaining(offset);
  }

  public void UpdateFoldings(IEnumerable<NewFolding> newFoldings, int firstErrorOffset)
  {
    if (newFoldings == null)
      throw new ArgumentNullException(nameof (newFoldings));
    if (firstErrorOffset < 0)
      firstErrorOffset = int.MaxValue;
    FoldingSection[] array = this.AllFoldings.ToArray<FoldingSection>();
    int index = 0;
    int num = 0;
    foreach (NewFolding newFolding in newFoldings)
    {
      if (newFolding.StartOffset < num)
        throw new ArgumentException("newFoldings must be sorted by start offset");
      num = newFolding.StartOffset;
      newFolding.StartOffset.CoerceValue(0, this.document.TextLength);
      newFolding.EndOffset.CoerceValue(0, this.document.TextLength);
      if (newFolding.StartOffset != newFolding.EndOffset)
      {
        while (index < array.Length && newFolding.StartOffset > array[index].StartOffset)
          this.RemoveFolding(array[index++]);
        FoldingSection folding;
        if (index < array.Length && newFolding.StartOffset == array[index].StartOffset)
        {
          folding = array[index++];
          folding.Length = newFolding.EndOffset - newFolding.StartOffset;
        }
        else
        {
          folding = this.CreateFolding(newFolding.StartOffset, newFolding.EndOffset);
          if (this.isFirstUpdate)
            folding.IsFolded = newFolding.DefaultClosed;
          folding.Tag = (object) newFolding;
        }
        folding.Title = newFolding.Name;
      }
    }
    this.isFirstUpdate = false;
    while (index < array.Length)
    {
      FoldingSection fs = array[index++];
      if (fs.StartOffset >= firstErrorOffset)
        break;
      this.RemoveFolding(fs);
    }
  }

  public static FoldingManager Install(TextArea textArea)
  {
    return textArea != null ? (FoldingManager) new FoldingManager.FoldingManagerInstallation(textArea) : throw new ArgumentNullException(nameof (textArea));
  }

  public static void Uninstall(FoldingManager manager)
  {
    if (manager == null)
      throw new ArgumentNullException(nameof (manager));
    if (!(manager is FoldingManager.FoldingManagerInstallation managerInstallation))
      throw new ArgumentException("FoldingManager was not created using FoldingManager.Install");
    managerInstallation.Uninstall();
  }

  private sealed class FoldingManagerInstallation : FoldingManager
  {
    private TextArea textArea;
    private FoldingMargin margin;
    private FoldingElementGenerator generator;

    public FoldingManagerInstallation(TextArea textArea)
      : base(textArea.Document)
    {
      this.textArea = textArea;
      this.margin = new FoldingMargin()
      {
        FoldingManager = (FoldingManager) this
      };
      this.generator = new FoldingElementGenerator()
      {
        FoldingManager = (FoldingManager) this
      };
      textArea.LeftMargins.Add((UIElement) this.margin);
      textArea.TextView.Services.AddService(typeof (FoldingManager), (object) this);
      textArea.TextView.ElementGenerators.Insert(0, (VisualLineElementGenerator) this.generator);
      textArea.Caret.PositionChanged += new EventHandler(this.textArea_Caret_PositionChanged);
    }

    public void Uninstall()
    {
      this.Clear();
      if (this.textArea == null)
        return;
      this.textArea.Caret.PositionChanged -= new EventHandler(this.textArea_Caret_PositionChanged);
      this.textArea.LeftMargins.Remove((UIElement) this.margin);
      this.textArea.TextView.ElementGenerators.Remove((VisualLineElementGenerator) this.generator);
      this.textArea.TextView.Services.RemoveService(typeof (FoldingManager));
      this.margin = (FoldingMargin) null;
      this.generator = (FoldingElementGenerator) null;
      this.textArea = (TextArea) null;
    }

    private void textArea_Caret_PositionChanged(object sender, EventArgs e)
    {
      int offset = this.textArea.Caret.Offset;
      foreach (FoldingSection foldingSection in this.GetFoldingsContaining(offset))
      {
        if (foldingSection.IsFolded && foldingSection.StartOffset < offset && offset < foldingSection.EndOffset)
          foldingSection.IsFolded = false;
      }
    }
  }
}

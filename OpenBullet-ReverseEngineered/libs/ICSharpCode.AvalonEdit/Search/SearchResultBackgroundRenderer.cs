// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Search.SearchResultBackgroundRenderer
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

#nullable disable
namespace ICSharpCode.AvalonEdit.Search;

internal class SearchResultBackgroundRenderer : IBackgroundRenderer
{
  private TextSegmentCollection<SearchResult> currentResults = new TextSegmentCollection<SearchResult>();
  private Brush markerBrush;
  private Pen markerPen;

  public TextSegmentCollection<SearchResult> CurrentResults => this.currentResults;

  public KnownLayer Layer => KnownLayer.Selection;

  public SearchResultBackgroundRenderer()
  {
    this.markerBrush = (Brush) Brushes.LightGreen;
    this.markerPen = new Pen(this.markerBrush, 1.0);
  }

  public Brush MarkerBrush
  {
    get => this.markerBrush;
    set
    {
      this.markerBrush = value;
      this.markerPen = new Pen(this.markerBrush, 1.0);
    }
  }

  public void Draw(TextView textView, DrawingContext drawingContext)
  {
    if (textView == null)
      throw new ArgumentNullException(nameof (textView));
    if (drawingContext == null)
      throw new ArgumentNullException(nameof (drawingContext));
    if (this.currentResults == null || !textView.VisualLinesValid)
      return;
    ReadOnlyCollection<VisualLine> visualLines = textView.VisualLines;
    if (visualLines.Count == 0)
      return;
    int offset = visualLines.First<VisualLine>().FirstDocumentLine.Offset;
    int endOffset = visualLines.Last<VisualLine>().LastDocumentLine.EndOffset;
    foreach (SearchResult overlappingSegment in this.currentResults.FindOverlappingSegments(offset, endOffset - offset))
    {
      BackgroundGeometryBuilder backgroundGeometryBuilder = new BackgroundGeometryBuilder();
      backgroundGeometryBuilder.AlignToWholePixels = true;
      backgroundGeometryBuilder.BorderThickness = this.markerPen != null ? this.markerPen.Thickness : 0.0;
      backgroundGeometryBuilder.CornerRadius = 3.0;
      backgroundGeometryBuilder.AddSegment(textView, (ISegment) overlappingSegment);
      Geometry geometry = backgroundGeometryBuilder.CreateGeometry();
      if (geometry != null)
        drawingContext.DrawGeometry(this.markerBrush, this.markerPen, geometry);
    }
  }
}

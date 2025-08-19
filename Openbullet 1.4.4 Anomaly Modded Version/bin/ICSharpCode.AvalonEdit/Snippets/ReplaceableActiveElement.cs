// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Snippets.ReplaceableActiveElement
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

#nullable disable
namespace ICSharpCode.AvalonEdit.Snippets;

internal sealed class ReplaceableActiveElement : 
  IReplaceableActiveElement,
  IActiveElement,
  IWeakEventListener
{
  private readonly InsertionContext context;
  private readonly int startOffset;
  private readonly int endOffset;
  private TextAnchor start;
  private TextAnchor end;
  private bool isCaretInside;
  private ReplaceableActiveElement.Renderer background;
  private ReplaceableActiveElement.Renderer foreground;

  public ReplaceableActiveElement(InsertionContext context, int startOffset, int endOffset)
  {
    this.context = context;
    this.startOffset = startOffset;
    this.endOffset = endOffset;
  }

  private void AnchorDeleted(object sender, EventArgs e)
  {
    this.context.Deactivate(new SnippetEventArgs(DeactivateReason.Deleted));
  }

  public void OnInsertionCompleted()
  {
    this.start = this.context.Document.CreateAnchor(this.startOffset);
    this.start.MovementType = AnchorMovementType.BeforeInsertion;
    this.end = this.context.Document.CreateAnchor(this.endOffset);
    this.end.MovementType = AnchorMovementType.AfterInsertion;
    this.start.Deleted += new EventHandler(this.AnchorDeleted);
    this.end.Deleted += new EventHandler(this.AnchorDeleted);
    WeakEventManagerBase<TextDocumentWeakEventManager.TextChanged, TextDocument>.AddListener(this.context.Document, (IWeakEventListener) this);
    this.background = new ReplaceableActiveElement.Renderer()
    {
      Layer = KnownLayer.Background,
      element = this
    };
    this.foreground = new ReplaceableActiveElement.Renderer()
    {
      Layer = KnownLayer.Text,
      element = this
    };
    this.context.TextArea.TextView.BackgroundRenderers.Add((IBackgroundRenderer) this.background);
    this.context.TextArea.TextView.BackgroundRenderers.Add((IBackgroundRenderer) this.foreground);
    this.context.TextArea.Caret.PositionChanged += new EventHandler(this.Caret_PositionChanged);
    this.Caret_PositionChanged((object) null, (EventArgs) null);
    this.Text = this.GetText();
  }

  public void Deactivate(SnippetEventArgs e)
  {
    WeakEventManagerBase<TextDocumentWeakEventManager.TextChanged, TextDocument>.RemoveListener(this.context.Document, (IWeakEventListener) this);
    this.context.TextArea.TextView.BackgroundRenderers.Remove((IBackgroundRenderer) this.background);
    this.context.TextArea.TextView.BackgroundRenderers.Remove((IBackgroundRenderer) this.foreground);
    this.context.TextArea.Caret.PositionChanged -= new EventHandler(this.Caret_PositionChanged);
  }

  private void Caret_PositionChanged(object sender, EventArgs e)
  {
    ISegment segment = this.Segment;
    if (segment == null)
      return;
    bool flag = segment.Contains(this.context.TextArea.Caret.Offset, 0);
    if (flag == this.isCaretInside)
      return;
    this.isCaretInside = flag;
    this.context.TextArea.TextView.InvalidateLayer(this.foreground.Layer);
  }

  public string Text { get; private set; }

  private string GetText()
  {
    return this.start.IsDeleted || this.end.IsDeleted ? string.Empty : this.context.Document.GetText(this.start.Offset, Math.Max(0, this.end.Offset - this.start.Offset));
  }

  public event EventHandler TextChanged;

  bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
  {
    if (!(managerType == typeof (TextDocumentWeakEventManager.TextChanged)))
      return false;
    string text = this.GetText();
    if (this.Text != text)
    {
      this.Text = text;
      if (this.TextChanged != null)
        this.TextChanged((object) this, e);
    }
    return true;
  }

  public bool IsEditable => true;

  public ISegment Segment
  {
    get
    {
      return this.start.IsDeleted || this.end.IsDeleted ? (ISegment) null : (ISegment) new SimpleSegment(this.start.Offset, Math.Max(0, this.end.Offset - this.start.Offset));
    }
  }

  private sealed class Renderer : IBackgroundRenderer
  {
    private static readonly Brush backgroundBrush = ReplaceableActiveElement.Renderer.CreateBackgroundBrush();
    private static readonly Pen activeBorderPen = ReplaceableActiveElement.Renderer.CreateBorderPen();
    internal ReplaceableActiveElement element;

    private static Brush CreateBackgroundBrush()
    {
      SolidColorBrush backgroundBrush = new SolidColorBrush(Colors.LimeGreen);
      backgroundBrush.Opacity = 0.4;
      backgroundBrush.Freeze();
      return (Brush) backgroundBrush;
    }

    private static Pen CreateBorderPen()
    {
      Pen borderPen = new Pen((Brush) Brushes.Black, 1.0);
      borderPen.DashStyle = DashStyles.Dot;
      borderPen.Freeze();
      return borderPen;
    }

    public KnownLayer Layer { get; set; }

    public void Draw(TextView textView, DrawingContext drawingContext)
    {
      ISegment segment = this.element.Segment;
      if (segment == null)
        return;
      BackgroundGeometryBuilder backgroundGeometryBuilder = new BackgroundGeometryBuilder();
      backgroundGeometryBuilder.AlignToWholePixels = true;
      backgroundGeometryBuilder.BorderThickness = ReplaceableActiveElement.Renderer.activeBorderPen != null ? ReplaceableActiveElement.Renderer.activeBorderPen.Thickness : 0.0;
      if (this.Layer == KnownLayer.Background)
      {
        backgroundGeometryBuilder.AddSegment(textView, segment);
        drawingContext.DrawGeometry(ReplaceableActiveElement.Renderer.backgroundBrush, (Pen) null, backgroundGeometryBuilder.CreateGeometry());
      }
      else
      {
        if (!this.element.isCaretInside)
          return;
        backgroundGeometryBuilder.AddSegment(textView, segment);
        foreach (BoundActiveElement boundActiveElement in this.element.context.ActiveElements.OfType<BoundActiveElement>())
        {
          if (boundActiveElement.targetElement == this.element)
          {
            backgroundGeometryBuilder.AddSegment(textView, boundActiveElement.Segment);
            backgroundGeometryBuilder.CloseFigure();
          }
        }
        drawingContext.DrawGeometry((Brush) null, ReplaceableActiveElement.Renderer.activeBorderPen, backgroundGeometryBuilder.CreateGeometry());
      }
    }
  }
}

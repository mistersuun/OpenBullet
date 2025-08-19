// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Rendering.TextView
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using System.Windows.Threading;

#nullable disable
namespace ICSharpCode.AvalonEdit.Rendering;

public class TextView : 
  FrameworkElement,
  IScrollInfo,
  IWeakEventListener,
  ITextEditorComponent,
  IServiceProvider
{
  private const double AdditionalHorizontalScrollAmount = 3.0;
  private ColumnRulerRenderer columnRulerRenderer;
  private CurrentLineHighlightRenderer currentLineHighlighRenderer;
  public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register(nameof (Document), typeof (TextDocument), typeof (TextView), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(TextView.OnDocumentChanged)));
  private TextDocument document;
  private HeightTree heightTree;
  public static readonly DependencyProperty OptionsProperty = DependencyProperty.Register(nameof (Options), typeof (TextEditorOptions), typeof (TextView), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(TextView.OnOptionsChanged)));
  private readonly ObserveAddRemoveCollection<VisualLineElementGenerator> elementGenerators;
  private readonly ObserveAddRemoveCollection<IVisualLineTransformer> lineTransformers;
  private SingleCharacterElementGenerator singleCharacterElementGenerator;
  private LinkElementGenerator linkElementGenerator;
  private MailLinkElementGenerator mailLinkElementGenerator;
  internal readonly TextLayer textLayer;
  private readonly TextView.LayerCollection layers;
  private List<InlineObjectRun> inlineObjects = new List<InlineObjectRun>();
  private List<VisualLine> visualLinesWithOutstandingInlineObjects = new List<VisualLine>();
  public static readonly DependencyProperty NonPrintableCharacterBrushProperty = DependencyProperty.Register(nameof (NonPrintableCharacterBrush), typeof (Brush), typeof (TextView), (PropertyMetadata) new FrameworkPropertyMetadata((object) Brushes.LightGray));
  public static readonly DependencyProperty LinkTextForegroundBrushProperty = DependencyProperty.Register(nameof (LinkTextForegroundBrush), typeof (Brush), typeof (TextView), (PropertyMetadata) new FrameworkPropertyMetadata((object) Brushes.Blue));
  public static readonly DependencyProperty LinkTextBackgroundBrushProperty = DependencyProperty.Register(nameof (LinkTextBackgroundBrush), typeof (Brush), typeof (TextView), (PropertyMetadata) new FrameworkPropertyMetadata((object) Brushes.Transparent));
  public static readonly DependencyProperty LinkTextUnderlineProperty = DependencyProperty.Register(nameof (LinkTextUnderline), typeof (bool), typeof (TextView), (PropertyMetadata) new FrameworkPropertyMetadata((object) true));
  private DispatcherOperation invalidateMeasureOperation;
  private List<VisualLine> allVisualLines = new List<VisualLine>();
  private ReadOnlyCollection<VisualLine> visibleVisualLines;
  private double clippedPixelsOnTop;
  private List<VisualLine> newVisualLines;
  private Size lastAvailableSize;
  private bool inMeasure;
  private TextFormatter formatter;
  internal TextViewCachedElements cachedElements;
  private readonly ObserveAddRemoveCollection<IBackgroundRenderer> backgroundRenderers;
  private Size scrollExtent;
  private Vector scrollOffset;
  private Size scrollViewport;
  private bool canVerticallyScroll;
  private bool canHorizontallyScroll;
  private bool defaultTextMetricsValid;
  private double wideSpaceWidth;
  private double defaultLineHeight;
  private double defaultBaseline;
  [ThreadStatic]
  private static bool invalidCursor;
  private readonly ServiceContainer services = new ServiceContainer();
  public static readonly RoutedEvent PreviewMouseHoverEvent = EventManager.RegisterRoutedEvent("PreviewMouseHover", RoutingStrategy.Tunnel, typeof (MouseEventHandler), typeof (TextView));
  public static readonly RoutedEvent MouseHoverEvent = EventManager.RegisterRoutedEvent("MouseHover", RoutingStrategy.Bubble, typeof (MouseEventHandler), typeof (TextView));
  public static readonly RoutedEvent PreviewMouseHoverStoppedEvent = EventManager.RegisterRoutedEvent("PreviewMouseHoverStopped", RoutingStrategy.Tunnel, typeof (MouseEventHandler), typeof (TextView));
  public static readonly RoutedEvent MouseHoverStoppedEvent = EventManager.RegisterRoutedEvent("MouseHoverStopped", RoutingStrategy.Bubble, typeof (MouseEventHandler), typeof (TextView));
  private MouseHoverLogic hoverLogic;
  public static readonly DependencyProperty ColumnRulerPenProperty = DependencyProperty.Register("ColumnRulerBrush", typeof (Pen), typeof (TextView), (PropertyMetadata) new FrameworkPropertyMetadata((object) TextView.CreateFrozenPen(Brushes.LightGray)));
  public static readonly DependencyProperty CurrentLineBackgroundProperty = DependencyProperty.Register(nameof (CurrentLineBackground), typeof (Brush), typeof (TextView));
  public static readonly DependencyProperty CurrentLineBorderProperty = DependencyProperty.Register(nameof (CurrentLineBorder), typeof (Pen), typeof (TextView));

  static TextView()
  {
    UIElement.ClipToBoundsProperty.OverrideMetadata(typeof (TextView), (PropertyMetadata) new FrameworkPropertyMetadata(Boxes.True));
    UIElement.FocusableProperty.OverrideMetadata(typeof (TextView), (PropertyMetadata) new FrameworkPropertyMetadata(Boxes.False));
  }

  public TextView()
  {
    this.services.AddService(typeof (TextView), (object) this);
    this.textLayer = new TextLayer(this);
    this.elementGenerators = new ObserveAddRemoveCollection<VisualLineElementGenerator>(new Action<VisualLineElementGenerator>(this.ElementGenerator_Added), new Action<VisualLineElementGenerator>(this.ElementGenerator_Removed));
    this.lineTransformers = new ObserveAddRemoveCollection<IVisualLineTransformer>(new Action<IVisualLineTransformer>(this.LineTransformer_Added), new Action<IVisualLineTransformer>(this.LineTransformer_Removed));
    this.backgroundRenderers = new ObserveAddRemoveCollection<IBackgroundRenderer>(new Action<IBackgroundRenderer>(this.BackgroundRenderer_Added), new Action<IBackgroundRenderer>(this.BackgroundRenderer_Removed));
    this.columnRulerRenderer = new ColumnRulerRenderer(this);
    this.currentLineHighlighRenderer = new CurrentLineHighlightRenderer(this);
    this.Options = new TextEditorOptions();
    this.layers = new TextView.LayerCollection(this);
    this.InsertLayer((UIElement) this.textLayer, KnownLayer.Text, LayerInsertionPosition.Replace);
    this.hoverLogic = new MouseHoverLogic((UIElement) this);
    this.hoverLogic.MouseHover += (EventHandler<MouseEventArgs>) ((sender, e) => this.RaiseHoverEventPair(e, TextView.PreviewMouseHoverEvent, TextView.MouseHoverEvent));
    this.hoverLogic.MouseHoverStopped += (EventHandler<MouseEventArgs>) ((sender, e) => this.RaiseHoverEventPair(e, TextView.PreviewMouseHoverStoppedEvent, TextView.MouseHoverStoppedEvent));
  }

  public TextDocument Document
  {
    get => (TextDocument) this.GetValue(TextView.DocumentProperty);
    set => this.SetValue(TextView.DocumentProperty, (object) value);
  }

  private static void OnDocumentChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
  {
    ((TextView) dp).OnDocumentChanged((TextDocument) e.OldValue, (TextDocument) e.NewValue);
  }

  internal double FontSize => (double) this.GetValue(TextBlock.FontSizeProperty);

  public event EventHandler DocumentChanged;

  private void OnDocumentChanged(TextDocument oldValue, TextDocument newValue)
  {
    if (oldValue != null)
    {
      this.heightTree.Dispose();
      this.heightTree = (HeightTree) null;
      this.formatter.Dispose();
      this.formatter = (TextFormatter) null;
      this.cachedElements.Dispose();
      this.cachedElements = (TextViewCachedElements) null;
      WeakEventManagerBase<TextDocumentWeakEventManager.Changing, TextDocument>.RemoveListener(oldValue, (IWeakEventListener) this);
    }
    this.document = newValue;
    this.ClearScrollData();
    this.ClearVisualLines();
    if (newValue != null)
    {
      WeakEventManagerBase<TextDocumentWeakEventManager.Changing, TextDocument>.AddListener(newValue, (IWeakEventListener) this);
      this.formatter = TextFormatterFactory.Create((DependencyObject) this);
      this.InvalidateDefaultTextMetrics();
      this.heightTree = new HeightTree(newValue, this.DefaultLineHeight);
      this.cachedElements = new TextViewCachedElements();
    }
    this.InvalidateMeasure(DispatcherPriority.Normal);
    if (this.DocumentChanged == null)
      return;
    this.DocumentChanged((object) this, EventArgs.Empty);
  }

  private void RecreateTextFormatter()
  {
    if (this.formatter == null)
      return;
    this.formatter.Dispose();
    this.formatter = TextFormatterFactory.Create((DependencyObject) this);
    this.Redraw();
  }

  private void RecreateCachedElements()
  {
    if (this.cachedElements == null)
      return;
    this.cachedElements.Dispose();
    this.cachedElements = new TextViewCachedElements();
  }

  protected virtual bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
  {
    if (managerType == typeof (TextDocumentWeakEventManager.Changing))
    {
      DocumentChangeEventArgs documentChangeEventArgs = (DocumentChangeEventArgs) e;
      this.Redraw(documentChangeEventArgs.Offset, documentChangeEventArgs.RemovalLength);
      return true;
    }
    if (!(managerType == typeof (PropertyChangedWeakEventManager)))
      return false;
    this.OnOptionChanged((PropertyChangedEventArgs) e);
    return true;
  }

  bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
  {
    return this.ReceiveWeakEvent(managerType, sender, e);
  }

  public TextEditorOptions Options
  {
    get => (TextEditorOptions) this.GetValue(TextView.OptionsProperty);
    set => this.SetValue(TextView.OptionsProperty, (object) value);
  }

  public event PropertyChangedEventHandler OptionChanged;

  protected virtual void OnOptionChanged(PropertyChangedEventArgs e)
  {
    if (this.OptionChanged != null)
      this.OptionChanged((object) this, e);
    if (this.Options.ShowColumnRuler)
      this.columnRulerRenderer.SetRuler(this.Options.ColumnRulerPosition, this.ColumnRulerPen);
    else
      this.columnRulerRenderer.SetRuler(-1, this.ColumnRulerPen);
    this.UpdateBuiltinElementGeneratorsFromOptions();
    this.Redraw();
  }

  private static void OnOptionsChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
  {
    ((TextView) dp).OnOptionsChanged((TextEditorOptions) e.OldValue, (TextEditorOptions) e.NewValue);
  }

  private void OnOptionsChanged(TextEditorOptions oldValue, TextEditorOptions newValue)
  {
    if (oldValue != null)
      WeakEventManagerBase<PropertyChangedWeakEventManager, INotifyPropertyChanged>.RemoveListener((INotifyPropertyChanged) oldValue, (IWeakEventListener) this);
    if (newValue != null)
      WeakEventManagerBase<PropertyChangedWeakEventManager, INotifyPropertyChanged>.AddListener((INotifyPropertyChanged) newValue, (IWeakEventListener) this);
    this.OnOptionChanged(new PropertyChangedEventArgs((string) null));
  }

  public IList<VisualLineElementGenerator> ElementGenerators
  {
    get => (IList<VisualLineElementGenerator>) this.elementGenerators;
  }

  private void ElementGenerator_Added(VisualLineElementGenerator generator)
  {
    this.ConnectToTextView((object) generator);
    this.Redraw();
  }

  private void ElementGenerator_Removed(VisualLineElementGenerator generator)
  {
    this.DisconnectFromTextView((object) generator);
    this.Redraw();
  }

  public IList<IVisualLineTransformer> LineTransformers
  {
    get => (IList<IVisualLineTransformer>) this.lineTransformers;
  }

  private void LineTransformer_Added(IVisualLineTransformer lineTransformer)
  {
    this.ConnectToTextView((object) lineTransformer);
    this.Redraw();
  }

  private void LineTransformer_Removed(IVisualLineTransformer lineTransformer)
  {
    this.DisconnectFromTextView((object) lineTransformer);
    this.Redraw();
  }

  private void UpdateBuiltinElementGeneratorsFromOptions()
  {
    TextEditorOptions options = this.Options;
    this.AddRemoveDefaultElementGeneratorOnDemand<SingleCharacterElementGenerator>(ref this.singleCharacterElementGenerator, options.ShowBoxForControlCharacters || options.ShowSpaces || options.ShowTabs);
    this.AddRemoveDefaultElementGeneratorOnDemand<LinkElementGenerator>(ref this.linkElementGenerator, options.EnableHyperlinks);
    this.AddRemoveDefaultElementGeneratorOnDemand<MailLinkElementGenerator>(ref this.mailLinkElementGenerator, options.EnableEmailHyperlinks);
  }

  private void AddRemoveDefaultElementGeneratorOnDemand<T>(ref T generator, bool demand) where T : VisualLineElementGenerator, IBuiltinElementGenerator, new()
  {
    if ((object) generator != null != demand)
    {
      if (demand)
      {
        generator = new T();
        this.ElementGenerators.Add((VisualLineElementGenerator) generator);
      }
      else
      {
        this.ElementGenerators.Remove((VisualLineElementGenerator) generator);
        generator = default (T);
      }
    }
    if ((object) generator == null)
      return;
    generator.FetchOptions(this.Options);
  }

  public UIElementCollection Layers => (UIElementCollection) this.layers;

  private void LayersChanged()
  {
    this.textLayer.index = this.layers.IndexOf((UIElement) this.textLayer);
  }

  public void InsertLayer(
    UIElement layer,
    KnownLayer referencedLayer,
    LayerInsertionPosition position)
  {
    if (layer == null)
      throw new ArgumentNullException(nameof (layer));
    if (!Enum.IsDefined(typeof (KnownLayer), (object) referencedLayer))
      throw new InvalidEnumArgumentException(nameof (referencedLayer), (int) referencedLayer, typeof (KnownLayer));
    if (!Enum.IsDefined(typeof (LayerInsertionPosition), (object) position))
      throw new InvalidEnumArgumentException(nameof (position), (int) position, typeof (LayerInsertionPosition));
    LayerPosition layerPosition1 = referencedLayer != KnownLayer.Background || position == LayerInsertionPosition.Above ? new LayerPosition(referencedLayer, position) : throw new InvalidOperationException("Cannot replace or insert below the background layer.");
    LayerPosition.SetLayerPosition(layer, layerPosition1);
    for (int index = 0; index < this.layers.Count; ++index)
    {
      LayerPosition layerPosition2 = LayerPosition.GetLayerPosition(this.layers[index]);
      if (layerPosition2 != null)
      {
        if (layerPosition2.KnownLayer == referencedLayer && layerPosition2.Position == LayerInsertionPosition.Replace)
        {
          switch (position)
          {
            case LayerInsertionPosition.Below:
              this.layers.Insert(index, layer);
              return;
            case LayerInsertionPosition.Replace:
              this.layers[index] = layer;
              return;
            case LayerInsertionPosition.Above:
              this.layers.Insert(index + 1, layer);
              return;
            default:
              continue;
          }
        }
        else if (layerPosition2.KnownLayer == referencedLayer && layerPosition2.Position == LayerInsertionPosition.Above || layerPosition2.KnownLayer > referencedLayer)
        {
          this.layers.Insert(index, layer);
          return;
        }
      }
    }
    this.layers.Add(layer);
  }

  protected override int VisualChildrenCount => this.layers.Count + this.inlineObjects.Count;

  protected override Visual GetVisualChild(int index)
  {
    int num = this.textLayer.index + 1;
    if (index < num)
      return (Visual) this.layers[index];
    return index < num + this.inlineObjects.Count ? (Visual) this.inlineObjects[index - num].Element : (Visual) this.layers[index - this.inlineObjects.Count];
  }

  protected override IEnumerator LogicalChildren
  {
    get
    {
      return (IEnumerator) this.inlineObjects.Select<InlineObjectRun, UIElement>((Func<InlineObjectRun, UIElement>) (io => io.Element)).Concat<UIElement>(this.layers.Cast<UIElement>()).GetEnumerator();
    }
  }

  internal void AddInlineObject(InlineObjectRun inlineObject)
  {
    bool flag = false;
    for (int index = 0; index < this.inlineObjects.Count; ++index)
    {
      if (this.inlineObjects[index].Element == inlineObject.Element)
      {
        this.RemoveInlineObjectRun(this.inlineObjects[index], true);
        this.inlineObjects.RemoveAt(index);
        flag = true;
        break;
      }
    }
    this.inlineObjects.Add(inlineObject);
    if (!flag)
      this.AddVisualChild((Visual) inlineObject.Element);
    inlineObject.Element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
    inlineObject.desiredSize = inlineObject.Element.DesiredSize;
  }

  private void MeasureInlineObjects()
  {
    foreach (InlineObjectRun inlineObject in this.inlineObjects)
    {
      if (!inlineObject.VisualLine.IsDisposed)
      {
        inlineObject.Element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        if (!inlineObject.Element.DesiredSize.IsClose(inlineObject.desiredSize))
        {
          inlineObject.desiredSize = inlineObject.Element.DesiredSize;
          if (this.allVisualLines.Remove(inlineObject.VisualLine))
            this.DisposeVisualLine(inlineObject.VisualLine);
        }
      }
    }
  }

  private void RemoveInlineObjects(VisualLine visualLine)
  {
    if (!visualLine.hasInlineObjects)
      return;
    this.visualLinesWithOutstandingInlineObjects.Add(visualLine);
  }

  private void RemoveInlineObjectsNow()
  {
    if (this.visualLinesWithOutstandingInlineObjects.Count == 0)
      return;
    this.inlineObjects.RemoveAll((Predicate<InlineObjectRun>) (ior =>
    {
      if (!this.visualLinesWithOutstandingInlineObjects.Contains(ior.VisualLine))
        return false;
      this.RemoveInlineObjectRun(ior, false);
      return true;
    }));
    this.visualLinesWithOutstandingInlineObjects.Clear();
  }

  private void RemoveInlineObjectRun(InlineObjectRun ior, bool keepElement)
  {
    if (!keepElement && ior.Element.IsKeyboardFocusWithin)
    {
      UIElement uiElement = (UIElement) this;
      while (uiElement != null && !uiElement.Focusable)
        uiElement = VisualTreeHelper.GetParent((DependencyObject) uiElement) as UIElement;
      if (uiElement != null)
        Keyboard.Focus((IInputElement) uiElement);
    }
    ior.VisualLine = (VisualLine) null;
    if (keepElement)
      return;
    this.RemoveVisualChild((Visual) ior.Element);
  }

  public Brush NonPrintableCharacterBrush
  {
    get => (Brush) this.GetValue(TextView.NonPrintableCharacterBrushProperty);
    set => this.SetValue(TextView.NonPrintableCharacterBrushProperty, (object) value);
  }

  public Brush LinkTextForegroundBrush
  {
    get => (Brush) this.GetValue(TextView.LinkTextForegroundBrushProperty);
    set => this.SetValue(TextView.LinkTextForegroundBrushProperty, (object) value);
  }

  public Brush LinkTextBackgroundBrush
  {
    get => (Brush) this.GetValue(TextView.LinkTextBackgroundBrushProperty);
    set => this.SetValue(TextView.LinkTextBackgroundBrushProperty, (object) value);
  }

  public bool LinkTextUnderline
  {
    get => (bool) this.GetValue(TextView.LinkTextUnderlineProperty);
    set => this.SetValue(TextView.LinkTextUnderlineProperty, (object) value);
  }

  public void Redraw() => this.Redraw(DispatcherPriority.Normal);

  public void Redraw(DispatcherPriority redrawPriority)
  {
    this.VerifyAccess();
    this.ClearVisualLines();
    this.InvalidateMeasure(redrawPriority);
  }

  public void Redraw(VisualLine visualLine, DispatcherPriority redrawPriority = DispatcherPriority.Normal)
  {
    this.VerifyAccess();
    if (!this.allVisualLines.Remove(visualLine))
      return;
    this.DisposeVisualLine(visualLine);
    this.InvalidateMeasure(redrawPriority);
  }

  public void Redraw(int offset, int length, DispatcherPriority redrawPriority = DispatcherPriority.Normal)
  {
    this.VerifyAccess();
    bool flag = false;
    for (int index = 0; index < this.allVisualLines.Count; ++index)
    {
      VisualLine allVisualLine = this.allVisualLines[index];
      int offset1 = allVisualLine.FirstDocumentLine.Offset;
      int num = allVisualLine.LastDocumentLine.Offset + allVisualLine.LastDocumentLine.TotalLength;
      if (offset <= num)
      {
        flag = true;
        if (offset + length >= offset1)
        {
          this.allVisualLines.RemoveAt(index--);
          this.DisposeVisualLine(allVisualLine);
        }
      }
    }
    if (!flag)
      return;
    this.InvalidateMeasure(redrawPriority);
  }

  public void InvalidateLayer(KnownLayer knownLayer)
  {
    this.InvalidateMeasure(DispatcherPriority.Normal);
  }

  public void InvalidateLayer(KnownLayer knownLayer, DispatcherPriority priority)
  {
    this.InvalidateMeasure(priority);
  }

  public void Redraw(ISegment segment, DispatcherPriority redrawPriority = DispatcherPriority.Normal)
  {
    if (segment == null)
      return;
    this.Redraw(segment.Offset, segment.Length, redrawPriority);
  }

  private void ClearVisualLines()
  {
    this.visibleVisualLines = (ReadOnlyCollection<VisualLine>) null;
    if (this.allVisualLines.Count == 0)
      return;
    foreach (VisualLine allVisualLine in this.allVisualLines)
      this.DisposeVisualLine(allVisualLine);
    this.allVisualLines.Clear();
  }

  private void DisposeVisualLine(VisualLine visualLine)
  {
    if (this.newVisualLines != null && this.newVisualLines.Contains(visualLine))
      throw new ArgumentException("Cannot dispose visual line because it is in construction!");
    this.visibleVisualLines = (ReadOnlyCollection<VisualLine>) null;
    visualLine.Dispose();
    this.RemoveInlineObjects(visualLine);
  }

  private void InvalidateMeasure(DispatcherPriority priority)
  {
    if (priority >= DispatcherPriority.Render)
    {
      if (this.invalidateMeasureOperation != null)
      {
        this.invalidateMeasureOperation.Abort();
        this.invalidateMeasureOperation = (DispatcherOperation) null;
      }
      this.InvalidateMeasure();
    }
    else if (this.invalidateMeasureOperation != null)
      this.invalidateMeasureOperation.Priority = priority;
    else
      this.invalidateMeasureOperation = this.Dispatcher.BeginInvoke(priority, (Delegate) (() =>
      {
        this.invalidateMeasureOperation = (DispatcherOperation) null;
        this.InvalidateMeasure();
      }));
  }

  public VisualLine GetVisualLine(int documentLineNumber)
  {
    foreach (VisualLine allVisualLine in this.allVisualLines)
    {
      int lineNumber1 = allVisualLine.FirstDocumentLine.LineNumber;
      int lineNumber2 = allVisualLine.LastDocumentLine.LineNumber;
      if (documentLineNumber >= lineNumber1 && documentLineNumber <= lineNumber2)
        return allVisualLine;
    }
    return (VisualLine) null;
  }

  public VisualLine GetOrConstructVisualLine(DocumentLine documentLine)
  {
    if (documentLine == null)
      throw new ArgumentNullException(nameof (documentLine));
    if (!this.Document.Lines.Contains(documentLine))
      throw new InvalidOperationException("Line belongs to wrong document");
    this.VerifyAccess();
    VisualLine constructVisualLine = this.GetVisualLine(documentLine.LineNumber);
    if (constructVisualLine == null)
    {
      TextRunProperties textRunProperties = this.CreateGlobalTextRunProperties();
      VisualLineTextParagraphProperties paragraphProperties = this.CreateParagraphProperties(textRunProperties);
      while (this.heightTree.GetIsCollapsed(documentLine.LineNumber))
        documentLine = documentLine.PreviousLine;
      constructVisualLine = this.BuildVisualLine(documentLine, textRunProperties, paragraphProperties, this.elementGenerators.ToArray<VisualLineElementGenerator>(), this.lineTransformers.ToArray<IVisualLineTransformer>(), this.lastAvailableSize);
      this.allVisualLines.Add(constructVisualLine);
      foreach (VisualLine allVisualLine in this.allVisualLines)
        allVisualLine.VisualTop = this.heightTree.GetVisualPosition(allVisualLine.FirstDocumentLine);
    }
    return constructVisualLine;
  }

  public ReadOnlyCollection<VisualLine> VisualLines
  {
    get
    {
      return this.visibleVisualLines != null ? this.visibleVisualLines : throw new VisualLinesInvalidException();
    }
  }

  public bool VisualLinesValid => this.visibleVisualLines != null;

  public event EventHandler<VisualLineConstructionStartEventArgs> VisualLineConstructionStarting;

  public event EventHandler VisualLinesChanged;

  public void EnsureVisualLines()
  {
    this.Dispatcher.VerifyAccess();
    if (this.inMeasure)
      throw new InvalidOperationException("The visual line build process is already running! Cannot EnsureVisualLines() during Measure!");
    if (!this.VisualLinesValid)
    {
      this.InvalidateMeasure(DispatcherPriority.Normal);
      this.UpdateLayout();
    }
    if (!this.VisualLinesValid)
      this.MeasureOverride(this.lastAvailableSize);
    if (!this.VisualLinesValid)
      throw new VisualLinesInvalidException("Internal error: visual lines invalid after EnsureVisualLines call");
  }

  protected override Size MeasureOverride(Size availableSize)
  {
    if (availableSize.Width > 32000.0)
      availableSize.Width = 32000.0;
    if (!this.canHorizontallyScroll && !availableSize.Width.IsClose(this.lastAvailableSize.Width))
      this.ClearVisualLines();
    this.lastAvailableSize = availableSize;
    foreach (UIElement layer in (UIElementCollection) this.layers)
      layer.Measure(availableSize);
    this.MeasureInlineObjects();
    this.InvalidateVisual();
    double num1;
    if (this.document == null)
    {
      this.allVisualLines = new List<VisualLine>();
      this.visibleVisualLines = this.allVisualLines.AsReadOnly();
      num1 = 0.0;
    }
    else
    {
      this.inMeasure = true;
      try
      {
        num1 = this.CreateAndMeasureVisualLines(availableSize);
      }
      finally
      {
        this.inMeasure = false;
      }
    }
    this.RemoveInlineObjectsNow();
    double num2 = num1 + 3.0;
    double num3 = this.DocumentHeight;
    if (this.Options.AllowScrollBelowDocument && !double.IsInfinity(this.scrollViewport.Height))
    {
      double num4 = Math.Max(this.DefaultLineHeight, 30.0);
      double val2 = Math.Min(num3 - num4, this.scrollOffset.Y) + this.scrollViewport.Height;
      num3 = Math.Max(num3, val2);
    }
    this.textLayer.SetVisualLines((ICollection<VisualLine>) this.visibleVisualLines);
    this.SetScrollData(availableSize, new Size(num2, num3), this.scrollOffset);
    if (this.VisualLinesChanged != null)
      this.VisualLinesChanged((object) this, EventArgs.Empty);
    return new Size(Math.Min(availableSize.Width, num2), Math.Min(availableSize.Height, num3));
  }

  private double CreateAndMeasureVisualLines(Size availableSize)
  {
    TextRunProperties textRunProperties = this.CreateGlobalTextRunProperties();
    VisualLineTextParagraphProperties paragraphProperties = this.CreateParagraphProperties(textRunProperties);
    DocumentLine byVisualPosition = this.heightTree.GetLineByVisualPosition(this.scrollOffset.Y);
    this.clippedPixelsOnTop = this.scrollOffset.Y - this.heightTree.GetVisualPosition(byVisualPosition);
    this.newVisualLines = new List<VisualLine>();
    if (this.VisualLineConstructionStarting != null)
      this.VisualLineConstructionStarting((object) this, new VisualLineConstructionStartEventArgs(byVisualPosition));
    VisualLineElementGenerator[] array1 = this.elementGenerators.ToArray<VisualLineElementGenerator>();
    IVisualLineTransformer[] array2 = this.lineTransformers.ToArray<IVisualLineTransformer>();
    DocumentLine documentLine = byVisualPosition;
    double measureVisualLines = 0.0;
    double num = -this.clippedPixelsOnTop;
    while (num < availableSize.Height && documentLine != null)
    {
      VisualLine visualLine = this.GetVisualLine(documentLine.LineNumber) ?? this.BuildVisualLine(documentLine, textRunProperties, paragraphProperties, array1, array2, availableSize);
      visualLine.VisualTop = this.scrollOffset.Y + num;
      documentLine = visualLine.LastDocumentLine.NextLine;
      num += visualLine.Height;
      foreach (TextLine textLine in visualLine.TextLines)
      {
        if (textLine.WidthIncludingTrailingWhitespace > measureVisualLines)
          measureVisualLines = textLine.WidthIncludingTrailingWhitespace;
      }
      this.newVisualLines.Add(visualLine);
    }
    foreach (VisualLine allVisualLine in this.allVisualLines)
    {
      if (!this.newVisualLines.Contains(allVisualLine))
        this.DisposeVisualLine(allVisualLine);
    }
    this.allVisualLines = this.newVisualLines;
    this.visibleVisualLines = new ReadOnlyCollection<VisualLine>((IList<VisualLine>) this.newVisualLines.ToArray());
    this.newVisualLines = (List<VisualLine>) null;
    if (this.allVisualLines.Any<VisualLine>((Func<VisualLine, bool>) (line => line.IsDisposed)))
      throw new InvalidOperationException("A visual line was disposed even though it is still in use.\nThis can happen when Redraw() is called during measure for lines that are already constructed.");
    return measureVisualLines;
  }

  private TextRunProperties CreateGlobalTextRunProperties()
  {
    return (TextRunProperties) new GlobalTextRunProperties()
    {
      typeface = this.CreateTypeface(),
      fontRenderingEmSize = this.FontSize,
      foregroundBrush = (Brush) this.GetValue(Control.ForegroundProperty),
      cultureInfo = CultureInfo.CurrentCulture
    };
  }

  private VisualLineTextParagraphProperties CreateParagraphProperties(
    TextRunProperties defaultTextRunProperties)
  {
    return new VisualLineTextParagraphProperties()
    {
      defaultTextRunProperties = defaultTextRunProperties,
      textWrapping = this.canHorizontallyScroll ? TextWrapping.NoWrap : TextWrapping.Wrap,
      tabSize = (double) this.Options.IndentationSize * this.WideSpaceWidth
    };
  }

  private VisualLine BuildVisualLine(
    DocumentLine documentLine,
    TextRunProperties globalTextRunProperties,
    VisualLineTextParagraphProperties paragraphProperties,
    VisualLineElementGenerator[] elementGeneratorsArray,
    IVisualLineTransformer[] lineTransformersArray,
    Size availableSize)
  {
    if (this.heightTree.GetIsCollapsed(documentLine.LineNumber))
      throw new InvalidOperationException("Trying to build visual line from collapsed line");
    VisualLine visualLine = new VisualLine(this, documentLine);
    VisualLineTextSource context = new VisualLineTextSource(visualLine)
    {
      Document = this.document,
      GlobalTextRunProperties = globalTextRunProperties,
      TextView = this
    };
    visualLine.ConstructVisualElements((ITextRunConstructionContext) context, elementGeneratorsArray);
    if (visualLine.FirstDocumentLine != visualLine.LastDocumentLine && !this.heightTree.GetVisualPosition(visualLine.FirstDocumentLine.NextLine).IsClose(this.heightTree.GetVisualPosition(visualLine.LastDocumentLine.NextLine ?? visualLine.LastDocumentLine)))
    {
      for (int lineNumber = visualLine.FirstDocumentLine.LineNumber + 1; lineNumber <= visualLine.LastDocumentLine.LineNumber; ++lineNumber)
      {
        if (!this.heightTree.GetIsCollapsed(lineNumber))
          throw new InvalidOperationException($"Line {(object) lineNumber} was skipped by a VisualLineElementGenerator, but it is not collapsed.");
      }
      throw new InvalidOperationException("All lines collapsed but visual pos different - height tree inconsistency?");
    }
    visualLine.RunTransformers((ITextRunConstructionContext) context, lineTransformersArray);
    int firstCharIndex = 0;
    TextLineBreak previousLineBreak = (TextLineBreak) null;
    List<TextLine> textLines = new List<TextLine>();
    paragraphProperties.indent = 0.0;
    paragraphProperties.firstLineInParagraph = true;
    while (firstCharIndex <= visualLine.VisualLengthWithEndOfLineMarker)
    {
      TextLine textLine = this.formatter.FormatLine((TextSource) context, firstCharIndex, availableSize.Width, (TextParagraphProperties) paragraphProperties, previousLineBreak);
      textLines.Add(textLine);
      firstCharIndex += textLine.Length;
      if (firstCharIndex < visualLine.VisualLengthWithEndOfLineMarker)
      {
        if (paragraphProperties.firstLineInParagraph)
        {
          paragraphProperties.firstLineInParagraph = false;
          TextEditorOptions options = this.Options;
          double num1 = 0.0;
          if (options.InheritWordWrapIndentation)
          {
            int indentationVisualColumn = TextView.GetIndentationVisualColumn(visualLine);
            if (indentationVisualColumn > 0 && indentationVisualColumn < firstCharIndex)
              num1 = textLine.GetDistanceFromCharacterHit(new CharacterHit(indentationVisualColumn, 0));
          }
          double num2 = num1 + options.WordWrapIndentation;
          if (num2 > 0.0 && num2 * 2.0 < availableSize.Width)
            paragraphProperties.indent = num2;
        }
        previousLineBreak = textLine.GetTextLineBreak();
      }
      else
        break;
    }
    visualLine.SetTextLines(textLines);
    this.heightTree.SetHeight(visualLine.FirstDocumentLine, visualLine.Height);
    return visualLine;
  }

  private static int GetIndentationVisualColumn(VisualLine visualLine)
  {
    if (visualLine.Elements.Count == 0)
      return 0;
    int visualColumn = 0;
    int index = 0;
    VisualLineElement element = visualLine.Elements[index];
    while (element.IsWhitespace(visualColumn))
    {
      ++visualColumn;
      if (visualColumn == element.VisualColumn + element.VisualLength)
      {
        ++index;
        if (index != visualLine.Elements.Count)
          element = visualLine.Elements[index];
        else
          break;
      }
    }
    return visualColumn;
  }

  protected override Size ArrangeOverride(Size finalSize)
  {
    this.EnsureVisualLines();
    foreach (UIElement layer in (UIElementCollection) this.layers)
      layer.Arrange(new Rect(new Point(0.0, 0.0), finalSize));
    if (this.document == null || this.allVisualLines.Count == 0)
      return finalSize;
    Vector scrollOffset = this.scrollOffset;
    if (this.scrollOffset.X + finalSize.Width > this.scrollExtent.Width)
      scrollOffset.X = Math.Max(0.0, this.scrollExtent.Width - finalSize.Width);
    if (this.scrollOffset.Y + finalSize.Height > this.scrollExtent.Height)
      scrollOffset.Y = Math.Max(0.0, this.scrollExtent.Height - finalSize.Height);
    if (this.SetScrollData(this.scrollViewport, this.scrollExtent, scrollOffset))
      this.InvalidateMeasure(DispatcherPriority.Normal);
    if (this.visibleVisualLines != null)
    {
      Point point = new Point(-this.scrollOffset.X, -this.clippedPixelsOnTop);
      foreach (VisualLine visibleVisualLine in this.visibleVisualLines)
      {
        int firstCharacterIndex = 0;
        foreach (TextLine textLine in visibleVisualLine.TextLines)
        {
          foreach (TextSpan<TextRun> textRunSpan in (IEnumerable<TextSpan<TextRun>>) textLine.GetTextRunSpans())
          {
            if (textRunSpan.Value is InlineObjectRun inlineObjectRun && inlineObjectRun.VisualLine != null)
            {
              double fromCharacterHit = textLine.GetDistanceFromCharacterHit(new CharacterHit(firstCharacterIndex, 0));
              inlineObjectRun.Element.Arrange(new Rect(new Point(point.X + fromCharacterHit, point.Y), inlineObjectRun.Element.DesiredSize));
            }
            firstCharacterIndex += textRunSpan.Length;
          }
          point.Y += textLine.Height;
        }
      }
    }
    this.InvalidateCursorIfMouseWithinTextView();
    return finalSize;
  }

  public IList<IBackgroundRenderer> BackgroundRenderers
  {
    get => (IList<IBackgroundRenderer>) this.backgroundRenderers;
  }

  private void BackgroundRenderer_Added(IBackgroundRenderer renderer)
  {
    this.ConnectToTextView((object) renderer);
    this.InvalidateLayer(renderer.Layer);
  }

  private void BackgroundRenderer_Removed(IBackgroundRenderer renderer)
  {
    this.DisconnectFromTextView((object) renderer);
    this.InvalidateLayer(renderer.Layer);
  }

  protected override void OnRender(DrawingContext drawingContext)
  {
    this.RenderBackground(drawingContext, KnownLayer.Background);
    foreach (VisualLine visibleVisualLine in this.visibleVisualLines)
    {
      Brush brush = (Brush) null;
      int startVC = 0;
      int num = 0;
      foreach (VisualLineElement element in visibleVisualLine.Elements)
      {
        if (brush == null || !brush.Equals((object) element.BackgroundBrush))
        {
          if (brush != null)
          {
            BackgroundGeometryBuilder backgroundGeometryBuilder = new BackgroundGeometryBuilder();
            backgroundGeometryBuilder.AlignToWholePixels = true;
            backgroundGeometryBuilder.CornerRadius = 3.0;
            foreach (Rect rectangle in BackgroundGeometryBuilder.GetRectsFromVisualSegment(this, visibleVisualLine, startVC, startVC + num))
              backgroundGeometryBuilder.AddRectangle(this, rectangle);
            Geometry geometry = backgroundGeometryBuilder.CreateGeometry();
            if (geometry != null)
              drawingContext.DrawGeometry(brush, (Pen) null, geometry);
          }
          startVC = element.VisualColumn;
          num = element.DocumentLength;
          brush = element.BackgroundBrush;
        }
        else
          num += element.VisualLength;
      }
      if (brush != null)
      {
        BackgroundGeometryBuilder backgroundGeometryBuilder = new BackgroundGeometryBuilder();
        backgroundGeometryBuilder.AlignToWholePixels = true;
        backgroundGeometryBuilder.CornerRadius = 3.0;
        foreach (Rect rectangle in BackgroundGeometryBuilder.GetRectsFromVisualSegment(this, visibleVisualLine, startVC, startVC + num))
          backgroundGeometryBuilder.AddRectangle(this, rectangle);
        Geometry geometry = backgroundGeometryBuilder.CreateGeometry();
        if (geometry != null)
          drawingContext.DrawGeometry(brush, (Pen) null, geometry);
      }
    }
  }

  internal void RenderBackground(DrawingContext drawingContext, KnownLayer layer)
  {
    foreach (IBackgroundRenderer backgroundRenderer in (Collection<IBackgroundRenderer>) this.backgroundRenderers)
    {
      if (backgroundRenderer.Layer == layer)
        backgroundRenderer.Draw(this, drawingContext);
    }
  }

  internal void ArrangeTextLayer(IList<VisualLineDrawingVisual> visuals)
  {
    Point point = new Point(-this.scrollOffset.X, -this.clippedPixelsOnTop);
    foreach (VisualLineDrawingVisual visual in (IEnumerable<VisualLineDrawingVisual>) visuals)
    {
      if (!(visual.Transform is TranslateTransform transform) || transform.X != point.X || transform.Y != point.Y)
      {
        visual.Transform = (Transform) new TranslateTransform(point.X, point.Y);
        visual.Transform.Freeze();
      }
      point.Y += visual.Height;
    }
  }

  private void ClearScrollData() => this.SetScrollData(new Size(), new Size(), new Vector());

  private bool SetScrollData(Size viewport, Size extent, Vector offset)
  {
    if (viewport.IsClose(this.scrollViewport) && extent.IsClose(this.scrollExtent) && offset.IsClose(this.scrollOffset))
      return false;
    this.scrollViewport = viewport;
    this.scrollExtent = extent;
    this.SetScrollOffset(offset);
    this.OnScrollChange();
    return true;
  }

  private void OnScrollChange() => ((IScrollInfo) this).ScrollOwner?.InvalidateScrollInfo();

  bool IScrollInfo.CanVerticallyScroll
  {
    get => this.canVerticallyScroll;
    set
    {
      if (this.canVerticallyScroll == value)
        return;
      this.canVerticallyScroll = value;
      this.InvalidateMeasure(DispatcherPriority.Normal);
    }
  }

  bool IScrollInfo.CanHorizontallyScroll
  {
    get => this.canHorizontallyScroll;
    set
    {
      if (this.canHorizontallyScroll == value)
        return;
      this.canHorizontallyScroll = value;
      this.ClearVisualLines();
      this.InvalidateMeasure(DispatcherPriority.Normal);
    }
  }

  double IScrollInfo.ExtentWidth => this.scrollExtent.Width;

  double IScrollInfo.ExtentHeight => this.scrollExtent.Height;

  double IScrollInfo.ViewportWidth => this.scrollViewport.Width;

  double IScrollInfo.ViewportHeight => this.scrollViewport.Height;

  public double HorizontalOffset => this.scrollOffset.X;

  public double VerticalOffset => this.scrollOffset.Y;

  public Vector ScrollOffset => this.scrollOffset;

  public event EventHandler ScrollOffsetChanged;

  private void SetScrollOffset(Vector vector)
  {
    if (!this.canHorizontallyScroll)
      vector.X = 0.0;
    if (!this.canVerticallyScroll)
      vector.Y = 0.0;
    if (this.scrollOffset.IsClose(vector))
      return;
    this.scrollOffset = vector;
    if (this.ScrollOffsetChanged == null)
      return;
    this.ScrollOffsetChanged((object) this, EventArgs.Empty);
  }

  ScrollViewer IScrollInfo.ScrollOwner { get; set; }

  void IScrollInfo.LineUp()
  {
    ((IScrollInfo) this).SetVerticalOffset(this.scrollOffset.Y - this.DefaultLineHeight);
  }

  void IScrollInfo.LineDown()
  {
    ((IScrollInfo) this).SetVerticalOffset(this.scrollOffset.Y + this.DefaultLineHeight);
  }

  void IScrollInfo.LineLeft()
  {
    ((IScrollInfo) this).SetHorizontalOffset(this.scrollOffset.X - this.WideSpaceWidth);
  }

  void IScrollInfo.LineRight()
  {
    ((IScrollInfo) this).SetHorizontalOffset(this.scrollOffset.X + this.WideSpaceWidth);
  }

  void IScrollInfo.PageUp()
  {
    ((IScrollInfo) this).SetVerticalOffset(this.scrollOffset.Y - this.scrollViewport.Height);
  }

  void IScrollInfo.PageDown()
  {
    ((IScrollInfo) this).SetVerticalOffset(this.scrollOffset.Y + this.scrollViewport.Height);
  }

  void IScrollInfo.PageLeft()
  {
    ((IScrollInfo) this).SetHorizontalOffset(this.scrollOffset.X - this.scrollViewport.Width);
  }

  void IScrollInfo.PageRight()
  {
    ((IScrollInfo) this).SetHorizontalOffset(this.scrollOffset.X + this.scrollViewport.Width);
  }

  void IScrollInfo.MouseWheelUp()
  {
    ((IScrollInfo) this).SetVerticalOffset(this.scrollOffset.Y - (double) SystemParameters.WheelScrollLines * this.DefaultLineHeight);
    this.OnScrollChange();
  }

  void IScrollInfo.MouseWheelDown()
  {
    ((IScrollInfo) this).SetVerticalOffset(this.scrollOffset.Y + (double) SystemParameters.WheelScrollLines * this.DefaultLineHeight);
    this.OnScrollChange();
  }

  void IScrollInfo.MouseWheelLeft()
  {
    ((IScrollInfo) this).SetHorizontalOffset(this.scrollOffset.X - (double) SystemParameters.WheelScrollLines * this.WideSpaceWidth);
    this.OnScrollChange();
  }

  void IScrollInfo.MouseWheelRight()
  {
    ((IScrollInfo) this).SetHorizontalOffset(this.scrollOffset.X + (double) SystemParameters.WheelScrollLines * this.WideSpaceWidth);
    this.OnScrollChange();
  }

  public double WideSpaceWidth
  {
    get
    {
      this.CalculateDefaultTextMetrics();
      return this.wideSpaceWidth;
    }
  }

  public double DefaultLineHeight
  {
    get
    {
      this.CalculateDefaultTextMetrics();
      return this.defaultLineHeight;
    }
  }

  public double DefaultBaseline
  {
    get
    {
      this.CalculateDefaultTextMetrics();
      return this.defaultBaseline;
    }
  }

  private void InvalidateDefaultTextMetrics()
  {
    this.defaultTextMetricsValid = false;
    if (this.heightTree == null)
      return;
    this.CalculateDefaultTextMetrics();
  }

  private void CalculateDefaultTextMetrics()
  {
    if (this.defaultTextMetricsValid)
      return;
    this.defaultTextMetricsValid = true;
    if (this.formatter != null)
    {
      TextRunProperties textRunProperties = this.CreateGlobalTextRunProperties();
      TextFormatter formatter = this.formatter;
      SimpleTextSource simpleTextSource = new SimpleTextSource("x", textRunProperties);
      VisualLineTextParagraphProperties paragraphProperties = new VisualLineTextParagraphProperties()
      {
        defaultTextRunProperties = textRunProperties
      };
      using (TextLine textLine = formatter.FormatLine((TextSource) simpleTextSource, 0, 32000.0, (TextParagraphProperties) paragraphProperties, (TextLineBreak) null))
      {
        this.wideSpaceWidth = Math.Max(1.0, textLine.WidthIncludingTrailingWhitespace);
        this.defaultBaseline = Math.Max(1.0, textLine.Baseline);
        this.defaultLineHeight = Math.Max(1.0, textLine.Height);
      }
    }
    else
    {
      this.wideSpaceWidth = this.FontSize / 2.0;
      this.defaultBaseline = this.FontSize;
      this.defaultLineHeight = this.FontSize + 3.0;
    }
    if (this.heightTree == null)
      return;
    this.heightTree.DefaultLineHeight = this.defaultLineHeight;
  }

  private static double ValidateVisualOffset(double offset)
  {
    if (double.IsNaN(offset))
      throw new ArgumentException("offset must not be NaN");
    return offset < 0.0 ? 0.0 : offset;
  }

  void IScrollInfo.SetHorizontalOffset(double offset)
  {
    offset = TextView.ValidateVisualOffset(offset);
    if (this.scrollOffset.X.IsClose(offset))
      return;
    this.SetScrollOffset(new Vector(offset, this.scrollOffset.Y));
    this.InvalidateVisual();
    this.textLayer.InvalidateVisual();
  }

  void IScrollInfo.SetVerticalOffset(double offset)
  {
    offset = TextView.ValidateVisualOffset(offset);
    if (this.scrollOffset.Y.IsClose(offset))
      return;
    this.SetScrollOffset(new Vector(this.scrollOffset.X, offset));
    this.InvalidateMeasure(DispatcherPriority.Normal);
  }

  Rect IScrollInfo.MakeVisible(Visual visual, Rect rectangle)
  {
    if (rectangle.IsEmpty || visual == null || visual == this || !this.IsAncestorOf((DependencyObject) visual))
      return Rect.Empty;
    rectangle = visual.TransformToAncestor((Visual) this).TransformBounds(rectangle);
    this.MakeVisible(Rect.Offset(rectangle, this.scrollOffset));
    return rectangle;
  }

  public virtual void MakeVisible(Rect rectangle)
  {
    Rect rect = new Rect(this.scrollOffset.X, this.scrollOffset.Y, this.scrollViewport.Width, this.scrollViewport.Height);
    Vector scrollOffset = this.scrollOffset;
    if (rectangle.Left < rect.Left)
      scrollOffset.X = rectangle.Right <= rect.Right ? rectangle.Left : rectangle.Left + rectangle.Width / 2.0;
    else if (rectangle.Right > rect.Right)
      scrollOffset.X = rectangle.Right - this.scrollViewport.Width;
    if (rectangle.Top < rect.Top)
      scrollOffset.Y = rectangle.Bottom <= rect.Bottom ? rectangle.Top : rectangle.Top + rectangle.Height / 2.0;
    else if (rectangle.Bottom > rect.Bottom)
      scrollOffset.Y = rectangle.Bottom - this.scrollViewport.Height;
    scrollOffset.X = TextView.ValidateVisualOffset(scrollOffset.X);
    scrollOffset.Y = TextView.ValidateVisualOffset(scrollOffset.Y);
    if (this.scrollOffset.IsClose(scrollOffset))
      return;
    this.SetScrollOffset(scrollOffset);
    this.OnScrollChange();
    this.InvalidateMeasure(DispatcherPriority.Normal);
  }

  protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
  {
    return (HitTestResult) new PointHitTestResult((Visual) this, hitTestParameters.HitPoint);
  }

  public static void InvalidateCursor()
  {
    if (TextView.invalidCursor)
      return;
    TextView.invalidCursor = true;
    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, (Delegate) (() =>
    {
      TextView.invalidCursor = false;
      Mouse.UpdateCursor();
    }));
  }

  internal void InvalidateCursorIfMouseWithinTextView()
  {
    if (!this.IsMouseOver)
      return;
    TextView.InvalidateCursor();
  }

  protected override void OnQueryCursor(QueryCursorEventArgs e)
  {
    this.GetVisualLineElementFromPosition(e.GetPosition((IInputElement) this) + this.scrollOffset)?.OnQueryCursor(e);
  }

  protected override void OnMouseDown(MouseButtonEventArgs e)
  {
    base.OnMouseDown(e);
    if (e.Handled)
      return;
    this.EnsureVisualLines();
    this.GetVisualLineElementFromPosition(e.GetPosition((IInputElement) this) + this.scrollOffset)?.OnMouseDown(e);
  }

  protected override void OnMouseUp(MouseButtonEventArgs e)
  {
    base.OnMouseUp(e);
    if (e.Handled)
      return;
    this.EnsureVisualLines();
    this.GetVisualLineElementFromPosition(e.GetPosition((IInputElement) this) + this.scrollOffset)?.OnMouseUp(e);
  }

  public VisualLine GetVisualLineFromVisualTop(double visualTop)
  {
    this.EnsureVisualLines();
    foreach (VisualLine visualLine in this.VisualLines)
    {
      if (visualTop >= visualLine.VisualTop && visualTop < visualLine.VisualTop + visualLine.Height)
        return visualLine;
    }
    return (VisualLine) null;
  }

  public double GetVisualTopByDocumentLine(int line)
  {
    this.VerifyAccess();
    return this.heightTree != null ? this.heightTree.GetVisualPosition(this.heightTree.GetLineByNumber(line)) : throw ThrowUtil.NoDocumentAssigned();
  }

  private VisualLineElement GetVisualLineElementFromPosition(Point visualPosition)
  {
    VisualLine lineFromVisualTop = this.GetVisualLineFromVisualTop(visualPosition.Y);
    if (lineFromVisualTop != null)
    {
      int visualColumnFloor = lineFromVisualTop.GetVisualColumnFloor(visualPosition);
      foreach (VisualLineElement element in lineFromVisualTop.Elements)
      {
        if (element.VisualColumn + element.VisualLength > visualColumnFloor)
          return element;
      }
    }
    return (VisualLineElement) null;
  }

  public Point GetVisualPosition(TextViewPosition position, VisualYPosition yPositionMode)
  {
    this.VerifyAccess();
    if (this.Document == null)
      throw ThrowUtil.NoDocumentAssigned();
    DocumentLine lineByNumber = this.Document.GetLineByNumber(position.Line);
    VisualLine constructVisualLine = this.GetOrConstructVisualLine(lineByNumber);
    int visualColumn = position.VisualColumn;
    if (visualColumn < 0)
    {
      int num = lineByNumber.Offset + position.Column - 1;
      visualColumn = constructVisualLine.GetVisualColumn(num - constructVisualLine.FirstDocumentLine.Offset);
    }
    return constructVisualLine.GetVisualPosition(visualColumn, position.IsAtEndOfLine, yPositionMode);
  }

  public TextViewPosition? GetPosition(Point visualPosition)
  {
    this.VerifyAccess();
    if (this.Document == null)
      throw ThrowUtil.NoDocumentAssigned();
    return this.GetVisualLineFromVisualTop(visualPosition.Y)?.GetTextViewPosition(visualPosition, this.Options.EnableVirtualSpace);
  }

  public TextViewPosition? GetPositionFloor(Point visualPosition)
  {
    this.VerifyAccess();
    if (this.Document == null)
      throw ThrowUtil.NoDocumentAssigned();
    return this.GetVisualLineFromVisualTop(visualPosition.Y)?.GetTextViewPositionFloor(visualPosition, this.Options.EnableVirtualSpace);
  }

  public ServiceContainer Services => this.services;

  public virtual object GetService(Type serviceType)
  {
    object service = this.services.GetService(serviceType);
    if (service == null && this.document != null)
      service = this.document.ServiceProvider.GetService(serviceType);
    return service;
  }

  private void ConnectToTextView(object obj)
  {
    if (!(obj is ITextViewConnect textViewConnect))
      return;
    textViewConnect.AddToTextView(this);
  }

  private void DisconnectFromTextView(object obj)
  {
    if (!(obj is ITextViewConnect textViewConnect))
      return;
    textViewConnect.RemoveFromTextView(this);
  }

  public event MouseEventHandler PreviewMouseHover
  {
    add => this.AddHandler(TextView.PreviewMouseHoverEvent, (Delegate) value);
    remove => this.RemoveHandler(TextView.PreviewMouseHoverEvent, (Delegate) value);
  }

  public event MouseEventHandler MouseHover
  {
    add => this.AddHandler(TextView.MouseHoverEvent, (Delegate) value);
    remove => this.RemoveHandler(TextView.MouseHoverEvent, (Delegate) value);
  }

  public event MouseEventHandler PreviewMouseHoverStopped
  {
    add => this.AddHandler(TextView.PreviewMouseHoverStoppedEvent, (Delegate) value);
    remove => this.RemoveHandler(TextView.PreviewMouseHoverStoppedEvent, (Delegate) value);
  }

  public event MouseEventHandler MouseHoverStopped
  {
    add => this.AddHandler(TextView.MouseHoverStoppedEvent, (Delegate) value);
    remove => this.RemoveHandler(TextView.MouseHoverStoppedEvent, (Delegate) value);
  }

  private void RaiseHoverEventPair(
    MouseEventArgs e,
    RoutedEvent tunnelingEvent,
    RoutedEvent bubblingEvent)
  {
    MouseDevice mouseDevice = e.MouseDevice;
    StylusDevice stylusDevice = e.StylusDevice;
    int tickCount = Environment.TickCount;
    MouseEventArgs mouseEventArgs = new MouseEventArgs(mouseDevice, tickCount, stylusDevice);
    mouseEventArgs.RoutedEvent = tunnelingEvent;
    mouseEventArgs.Source = (object) this;
    MouseEventArgs e1 = mouseEventArgs;
    this.RaiseEvent((RoutedEventArgs) e1);
    MouseEventArgs e2 = new MouseEventArgs(mouseDevice, tickCount, stylusDevice);
    e2.RoutedEvent = bubblingEvent;
    e2.Source = (object) this;
    e2.Handled = e1.Handled;
    this.RaiseEvent((RoutedEventArgs) e2);
  }

  public CollapsedLineSection CollapseLines(DocumentLine start, DocumentLine end)
  {
    this.VerifyAccess();
    if (this.heightTree == null)
      throw ThrowUtil.NoDocumentAssigned();
    return this.heightTree.CollapseText(start, end);
  }

  public double DocumentHeight => this.heightTree == null ? 0.0 : this.heightTree.TotalHeight;

  public DocumentLine GetDocumentLineByVisualTop(double visualTop)
  {
    this.VerifyAccess();
    return this.heightTree != null ? this.heightTree.GetLineByVisualPosition(visualTop) : throw ThrowUtil.NoDocumentAssigned();
  }

  protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
  {
    base.OnPropertyChanged(e);
    if (TextFormatterFactory.PropertyChangeAffectsTextFormatter(e.Property))
    {
      this.RecreateTextFormatter();
      this.RecreateCachedElements();
      this.InvalidateDefaultTextMetrics();
    }
    else if (e.Property == Control.ForegroundProperty || e.Property == TextView.NonPrintableCharacterBrushProperty || e.Property == TextView.LinkTextBackgroundBrushProperty || e.Property == TextView.LinkTextForegroundBrushProperty || e.Property == TextView.LinkTextUnderlineProperty)
    {
      this.RecreateCachedElements();
      this.Redraw();
    }
    if (e.Property == Control.FontFamilyProperty || e.Property == Control.FontSizeProperty || e.Property == Control.FontStretchProperty || e.Property == Control.FontStyleProperty || e.Property == Control.FontWeightProperty)
    {
      this.RecreateCachedElements();
      this.InvalidateDefaultTextMetrics();
      this.Redraw();
    }
    if (e.Property == TextView.ColumnRulerPenProperty)
      this.columnRulerRenderer.SetRuler(this.Options.ColumnRulerPosition, this.ColumnRulerPen);
    if (e.Property == TextView.CurrentLineBorderProperty)
      this.currentLineHighlighRenderer.BorderPen = this.CurrentLineBorder;
    if (e.Property != TextView.CurrentLineBackgroundProperty)
      return;
    this.currentLineHighlighRenderer.BackgroundBrush = this.CurrentLineBackground;
  }

  private static Pen CreateFrozenPen(SolidColorBrush brush)
  {
    Pen frozenPen = new Pen((Brush) brush, 1.0);
    frozenPen.Freeze();
    return frozenPen;
  }

  public Pen ColumnRulerPen
  {
    get => (Pen) this.GetValue(TextView.ColumnRulerPenProperty);
    set => this.SetValue(TextView.ColumnRulerPenProperty, (object) value);
  }

  public Brush CurrentLineBackground
  {
    get => (Brush) this.GetValue(TextView.CurrentLineBackgroundProperty);
    set => this.SetValue(TextView.CurrentLineBackgroundProperty, (object) value);
  }

  public Pen CurrentLineBorder
  {
    get => (Pen) this.GetValue(TextView.CurrentLineBorderProperty);
    set => this.SetValue(TextView.CurrentLineBorderProperty, (object) value);
  }

  public int HighlightedLine
  {
    get => this.currentLineHighlighRenderer.Line;
    set => this.currentLineHighlighRenderer.Line = value;
  }

  public virtual double EmptyLineSelectionWidth => 1.0;

  private sealed class LayerCollection : UIElementCollection
  {
    private readonly TextView textView;

    public LayerCollection(TextView textView)
      : base((UIElement) textView, (FrameworkElement) textView)
    {
      this.textView = textView;
    }

    public override void Clear()
    {
      base.Clear();
      this.textView.LayersChanged();
    }

    public override int Add(UIElement element)
    {
      int num = base.Add(element);
      this.textView.LayersChanged();
      return num;
    }

    public override void RemoveAt(int index)
    {
      base.RemoveAt(index);
      this.textView.LayersChanged();
    }

    public override void RemoveRange(int index, int count)
    {
      base.RemoveRange(index, count);
      this.textView.LayersChanged();
    }
  }
}

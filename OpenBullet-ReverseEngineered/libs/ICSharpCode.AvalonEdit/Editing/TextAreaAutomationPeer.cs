// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Editing.TextAreaAutomationPeer
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

#nullable disable
namespace ICSharpCode.AvalonEdit.Editing;

internal class TextAreaAutomationPeer : FrameworkElementAutomationPeer, IValueProvider, ITextProvider
{
  public TextAreaAutomationPeer(TextArea owner)
    : base((FrameworkElement) owner)
  {
    owner.Caret.PositionChanged += new EventHandler(this.OnSelectionChanged);
    owner.SelectionChanged += new EventHandler(this.OnSelectionChanged);
  }

  private void OnSelectionChanged(object sender, EventArgs e)
  {
    this.RaiseAutomationEvent(AutomationEvents.TextPatternOnTextSelectionChanged);
  }

  private TextArea TextArea => (TextArea) this.Owner;

  protected override AutomationControlType GetAutomationControlTypeCore()
  {
    return AutomationControlType.Document;
  }

  internal IRawElementProviderSimple Provider => this.ProviderFromPeer((AutomationPeer) this);

  public bool IsReadOnly
  {
    get => this.TextArea.ReadOnlySectionProvider == ReadOnlySectionDocument.Instance;
  }

  public void SetValue(string value) => this.TextArea.Document.Text = value;

  public string Value => this.TextArea.Document.Text;

  public ITextRangeProvider DocumentRange
  {
    get
    {
      return (ITextRangeProvider) new TextRangeProvider(this.TextArea, this.TextArea.Document, 0, this.TextArea.Document.TextLength);
    }
  }

  public ITextRangeProvider[] GetSelection()
  {
    if (!this.TextArea.Selection.IsEmpty)
      return (ITextRangeProvider[]) this.TextArea.Selection.Segments.Select<SelectionSegment, TextRangeProvider>((Func<SelectionSegment, TextRangeProvider>) (s => new TextRangeProvider(this.TextArea, this.TextArea.Document, (ISegment) s))).ToArray<TextRangeProvider>();
    TextAnchor anchor = this.TextArea.Document.CreateAnchor(this.TextArea.Caret.Offset);
    anchor.SurviveDeletion = true;
    return new ITextRangeProvider[1]
    {
      (ITextRangeProvider) new TextRangeProvider(this.TextArea, this.TextArea.Document, (ISegment) new AnchorSegment(anchor, anchor))
    };
  }

  public ITextRangeProvider[] GetVisibleRanges() => throw new NotImplementedException();

  public ITextRangeProvider RangeFromChild(IRawElementProviderSimple childElement)
  {
    throw new NotImplementedException();
  }

  public ITextRangeProvider RangeFromPoint(Point screenLocation)
  {
    throw new NotImplementedException();
  }

  public SupportedTextSelection SupportedTextSelection => SupportedTextSelection.Single;

  public override object GetPattern(PatternInterface patternInterface)
  {
    switch (patternInterface)
    {
      case PatternInterface.Value:
        return (object) this;
      case PatternInterface.Scroll:
        if (this.TextArea.GetService(typeof (TextEditor)) is TextEditor service)
          return UIElementAutomationPeer.FromElement((UIElement) service).GetPattern(patternInterface);
        break;
      case PatternInterface.Text:
        return (object) this;
    }
    return base.GetPattern(patternInterface);
  }
}

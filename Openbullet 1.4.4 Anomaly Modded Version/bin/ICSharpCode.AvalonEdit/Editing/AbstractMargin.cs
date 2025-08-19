// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Editing.AbstractMargin
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Windows;

#nullable disable
namespace ICSharpCode.AvalonEdit.Editing;

public abstract class AbstractMargin : FrameworkElement, ITextViewConnect
{
  public static readonly DependencyProperty TextViewProperty = DependencyProperty.Register(nameof (TextView), typeof (TextView), typeof (AbstractMargin), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(AbstractMargin.OnTextViewChanged)));
  private bool wasAutoAddedToTextView;
  private TextDocument document;

  public TextView TextView
  {
    get => (TextView) this.GetValue(AbstractMargin.TextViewProperty);
    set => this.SetValue(AbstractMargin.TextViewProperty, (object) value);
  }

  private static void OnTextViewChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
  {
    AbstractMargin abstractMargin = (AbstractMargin) dp;
    abstractMargin.wasAutoAddedToTextView = false;
    abstractMargin.OnTextViewChanged((TextView) e.OldValue, (TextView) e.NewValue);
  }

  void ITextViewConnect.AddToTextView(TextView textView)
  {
    if (this.TextView == null)
    {
      this.TextView = textView;
      this.wasAutoAddedToTextView = true;
    }
    else if (this.TextView != textView)
      throw new InvalidOperationException("This margin belongs to a different TextView.");
  }

  void ITextViewConnect.RemoveFromTextView(TextView textView)
  {
    if (!this.wasAutoAddedToTextView || this.TextView != textView)
      return;
    this.TextView = (TextView) null;
  }

  public TextDocument Document => this.document;

  protected virtual void OnTextViewChanged(TextView oldTextView, TextView newTextView)
  {
    if (oldTextView != null)
      oldTextView.DocumentChanged -= new EventHandler(this.TextViewDocumentChanged);
    if (newTextView != null)
      newTextView.DocumentChanged += new EventHandler(this.TextViewDocumentChanged);
    this.TextViewDocumentChanged((object) null, (EventArgs) null);
  }

  private void TextViewDocumentChanged(object sender, EventArgs e)
  {
    this.OnDocumentChanged(this.document, this.TextView != null ? this.TextView.Document : (TextDocument) null);
  }

  protected virtual void OnDocumentChanged(TextDocument oldDocument, TextDocument newDocument)
  {
    this.document = newDocument;
  }
}

// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.TextEditorAutomationPeer
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Utils;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;

#nullable disable
namespace ICSharpCode.AvalonEdit;

public class TextEditorAutomationPeer(TextEditor owner) : 
  FrameworkElementAutomationPeer((FrameworkElement) owner),
  IValueProvider
{
  private TextEditor TextEditor => (TextEditor) this.Owner;

  void IValueProvider.SetValue(string value) => this.TextEditor.Text = value;

  string IValueProvider.Value => this.TextEditor.Text;

  bool IValueProvider.IsReadOnly => this.TextEditor.IsReadOnly;

  protected override AutomationControlType GetAutomationControlTypeCore()
  {
    return AutomationControlType.Document;
  }

  public override object GetPattern(PatternInterface patternInterface)
  {
    if (patternInterface == PatternInterface.Value)
      return (object) this;
    if (patternInterface == PatternInterface.Scroll)
    {
      ScrollViewer scrollViewer = this.TextEditor.ScrollViewer;
      if (scrollViewer != null)
        return (object) UIElementAutomationPeer.FromElement((UIElement) scrollViewer);
    }
    return patternInterface == PatternInterface.Text ? (object) UIElementAutomationPeer.FromElement((UIElement) this.TextEditor.TextArea) : base.GetPattern(patternInterface);
  }

  internal void RaiseIsReadOnlyChanged(bool oldValue, bool newValue)
  {
    this.RaisePropertyChangedEvent(ValuePatternIdentifiers.IsReadOnlyProperty, Boxes.Box(oldValue), Boxes.Box(newValue));
  }
}

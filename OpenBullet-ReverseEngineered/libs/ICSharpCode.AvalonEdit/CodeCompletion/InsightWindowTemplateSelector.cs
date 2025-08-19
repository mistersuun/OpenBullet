// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.CodeCompletion.InsightWindowTemplateSelector
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace ICSharpCode.AvalonEdit.CodeCompletion;

internal sealed class InsightWindowTemplateSelector : DataTemplateSelector
{
  public override DataTemplate SelectTemplate(object item, DependencyObject container)
  {
    return item is string ? (DataTemplate) ((FrameworkElement) container).FindResource((object) "TextBlockTemplate") : (DataTemplate) null;
  }
}

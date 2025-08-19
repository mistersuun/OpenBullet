// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.Editors.TextBlockEditor
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid.Editors;

public class TextBlockEditor : TypeEditor<TextBlock>
{
  protected override TextBlock CreateEditor() => (TextBlock) new PropertyGridEditorTextBlock();

  protected override void SetValueDependencyProperty()
  {
    this.ValueProperty = TextBlock.TextProperty;
  }

  protected override void SetControlProperties(PropertyItem propertyItem)
  {
    this.Editor.Margin = new Thickness(5.0, 0.0, 0.0, 0.0);
    this.Editor.TextTrimming = TextTrimming.CharacterEllipsis;
  }
}

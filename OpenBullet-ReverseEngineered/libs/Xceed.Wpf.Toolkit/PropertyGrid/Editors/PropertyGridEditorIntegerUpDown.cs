// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.Editors.PropertyGridEditorIntegerUpDown
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.Windows;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid.Editors;

public class PropertyGridEditorIntegerUpDown : IntegerUpDown
{
  static PropertyGridEditorIntegerUpDown()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (PropertyGridEditorIntegerUpDown), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (PropertyGridEditorIntegerUpDown)));
  }
}

// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.CheckListBox
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.Windows;
using Xceed.Wpf.Toolkit.Primitives;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public class CheckListBox : SelectAllSelector
{
  static CheckListBox()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (CheckListBox), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (CheckListBox)));
  }
}

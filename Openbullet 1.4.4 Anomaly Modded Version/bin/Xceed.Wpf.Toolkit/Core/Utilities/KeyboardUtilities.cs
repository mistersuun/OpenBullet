// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.Utilities.KeyboardUtilities
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.Windows.Input;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core.Utilities;

internal class KeyboardUtilities
{
  internal static bool IsKeyModifyingPopupState(KeyEventArgs e)
  {
    return (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt && (e.SystemKey == Key.Down || e.SystemKey == Key.Up) || e.Key == Key.F4;
  }
}
